// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using Jih.Unity.Infrastructure;
using Jih.Unity.Infrastructure.HexaGrid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jih.Unity.EraOfNitrogen.Worlds.Runtime
{
    class ProvinceGenerator
    {
        readonly Settings _settings;
        readonly RandomStream _random;
        readonly IReadOnlyList<MapCell> _landCells;

        public List<MapCell>? ResultCapitalCells { get; private set; }
        public List<MapProvince>? ResultProvinces { get; private set; }

        public ProvinceGenerator(Settings settings, RandomStream random, IReadOnlyList<MapCell> landCells)
        {
            _settings = settings;
            _random = random;
            _landCells = landCells;
        }

        public void Execute()
        {
            List<MapCell> capitalCells = GenerateCapitals(_random, _landCells, _settings.MaxProvinceCount, _settings.MinCapitalDistance);
            if (capitalCells.Count <= 0)
            {
                return;
            }

            List<MapProvince> provinces = GenerateProvinces(_landCells, capitalCells);

            ResultCapitalCells = capitalCells;
            ResultProvinces = provinces;
        }

        static List<MapCell> GenerateCapitals(RandomStream random, IReadOnlyList<MapCell> landCells, int maxProvinceCountSetting, int minCapitalDistanceSetting)
        {
            List<MapCell> result = new();

            List<MapCell> candidates = new(landCells.Count);
            // Except coastline cells.
            candidates.AddRange(landCells.Where(l => !l.IsCoastlineLand));

            while (candidates.Count > 0 && result.Count < maxProvinceCountSetting)
            {
                bool anyFound = false;
                for (int retryCount = 0; retryCount < landCells.Count; retryCount++)
                {
                    int pickedIndex = random.NextInt32(0, candidates.Count);
                    MapCell pickedCell = candidates[pickedIndex];
                    candidates.RemoveAt(pickedIndex);

                    if (CheckDistance(pickedCell, result, minCapitalDistanceSetting))
                    {
                        result.Add(pickedCell);
                        anyFound = true;
                        break;
                    }
                }

                if (!anyFound)
                {
                    break;
                }
            }

            return result;
        }

        static bool CheckDistance(MapCell cell, List<MapCell> capitalCells, int minCapitalDistanceSetting)
        {
            if (capitalCells.Contains(cell))
            {
                return false;
            }

            foreach (var capitalCell in capitalCells)
            {
                if (HexaCoord.Distance(cell.Coord, capitalCell.Coord) < minCapitalDistanceSetting)
                {
                    return false;
                }
            }
            return true;
        }

        static List<MapProvince> GenerateProvinces(IReadOnlyList<MapCell> landCells, IReadOnlyList<MapCell> capitalCells)
        {
            List<MapProvince> result = new(capitalCells.Count);

            foreach (var capitalCell in capitalCells)
            {
                MapProvince province = new(capitalCell);

                province.Cells.Add(capitalCell);
                capitalCell.Province = province;

                result.Add(province);
            }

            // Except capitals. Already processed.
            foreach (var expandTarget in landCells.Where(l => !capitalCells.Contains(l)))
            {
                // Propagate province by capitals.
                MapProvince province = FindNearestCapital(expandTarget, capitalCells).Province ?? throw new InvalidOperationException();

                expandTarget.Province = province;
                province.Cells.Add(expandTarget);
            }

            return result;
        }

        static MapCell FindNearestCapital(MapCell cell, IReadOnlyList<MapCell> capitalCells)
        {
            HexaCoord cellCoord = cell.Coord;

            int nearestDistance = int.MaxValue;
            MapCell? nearestCapital = null;

            foreach (var capital in capitalCells)
            {
                int distance = HexaCoord.Distance(cellCoord, capital.Coord);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCapital = capital;
                }
            }

            return nearestCapital ?? throw new InvalidOperationException();
        }

        public struct Settings
        {
            public static Settings Default => new(4, int.MaxValue);

            public int MinCapitalDistance;
            public int MaxProvinceCount;

            public Settings(int minCapitalDistance, int maxProvinceCount)
            {
                MinCapitalDistance = minCapitalDistance;
                MaxProvinceCount = maxProvinceCount;
            }
        }
    }
}
