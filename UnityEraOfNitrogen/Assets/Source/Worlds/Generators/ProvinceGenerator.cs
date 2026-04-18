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

namespace Jih.Unity.EraOfNitrogen.Worlds.Generators
{
    class ProvinceGenerator
    {
        readonly Settings _settings;
        readonly RandomStream _random;
        readonly GeneratorGrid _grid;
        readonly IReadOnlyList<GeneratorCell> _landCells;

        public List<GeneratorProvince>? ResultProvinces { get; private set; }

        public ProvinceGenerator(Settings settings, RandomStream random, GeneratorGrid grid, IReadOnlyList<GeneratorCell> landCells)
        {
            _settings = settings;
            _random = random;
            _grid = grid;
            _landCells = landCells;
        }

        public void Execute()
        {
            List<GeneratorCell> cityCells = GenerateCities(_random, _landCells, _settings.MaxProvinceCount, _settings.MinCityDistance);
            if (cityCells.Count <= 0)
            {
                return;
            }

            List<GeneratorProvince> provinces = GenerateProvinces(_grid, cityCells);
            
            GeneratePorts(_random, _grid, provinces, _settings.MaxPortCount);
            AllocateOceanCellsToPorts(_grid, provinces, _settings.MaxOceanPortDistance);

            ResultProvinces = provinces;
        }

        static List<GeneratorCell> GenerateCities(RandomStream random, IReadOnlyList<GeneratorCell> landCells, int maxProvinceCountSetting, int minCityDistanceSetting)
        {
            List<GeneratorCell> result = new();

            int maxRetryCount = landCells.Count;

            List<GeneratorCell> candidates = new(landCells.Count);
            // 해안선은 도시 후보에서 제외.
            candidates.AddRange(landCells.Where(l => !l.IsCoastlineLand));

            while (candidates.Count > 0 && result.Count < maxProvinceCountSetting)
            {
                bool anyFound = false;
                for (int retryCount = 0; retryCount < maxRetryCount; retryCount++)
                {
                    int pickedIndex = random.NextInt32(0, candidates.Count);
                    GeneratorCell pickedCell = candidates[pickedIndex];
                    candidates.RemoveAt(pickedIndex);

                    if (CheckDistance(pickedCell, result, minCityDistanceSetting))
                    {
                        result.Add(pickedCell);
                        anyFound = true;
                        break;
                    }

                    if (candidates.Count <= 0)
                    {
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

        static bool CheckDistance(GeneratorCell cell, List<GeneratorCell> cityCells, int minCityDistanceSetting)
        {
            if (cityCells.Contains(cell))
            {
                return false;
            }

            foreach (var cityCell in cityCells)
            {
                if (HexaCoord.Distance(cell.Coord, cityCell.Coord) < minCityDistanceSetting)
                {
                    return false;
                }
            }
            return true;
        }

        static List<GeneratorProvince> GenerateProvinces(GeneratorGrid grid, IReadOnlyList<GeneratorCell> cityCells)
        {
            List<GeneratorProvince> result = new(cityCells.Count);

            uint provinceId = 1;
            foreach (var cityCell in cityCells)
            {
                GeneratorProvince province = new(provinceId, cityCell);

                province.LandCells.Add(cityCell);
                cityCell.Province = province;

                result.Add(province);

                provinceId++;
            }

            {
                HexaMultiAreasResult areaResult = new();
                LandAreaContext areaContext = new();

                grid.CollectMultiAreas(cityCells, areaResult, areaContext.Access, null);

                foreach (var pair in areaResult.CellToStartingCells)
                {
                    if (cityCells.Contains(pair.Key))
                    {
                        continue;
                    }

                    GeneratorProvince province = ((GeneratorCell)pair.Value).Province ?? throw new InvalidOperationException();
                    GeneratorCell cell = (GeneratorCell)pair.Key;
                    cell.Province = province;
                    province.LandCells.Add(cell);
                }
            }

            foreach (var province in result)
            {
                HashSet<GeneratorProvince> adjacentProvinces = new(result.Count);

                foreach (var cell in province.LandCells)
                {
                    foreach (var neighbor in cell.EnumerateNeighbors())
                    {
                        GeneratorProvince? neighborProvince = neighbor.Province;
                        if (neighborProvince is null || neighborProvince == province)
                        {
                            continue;
                        }
                        adjacentProvinces.Add(neighborProvince);
                    }
                }

                province.AdjacentProvinces.AddRange(adjacentProvinces);
            }

            return result;
        }
        
        static void GeneratePorts(RandomStream random, GeneratorGrid grid, IReadOnlyList<GeneratorProvince> provinces, int maxPortCountSetting)
        {
            List<GeneratorProvince> candidates = new(provinces);

            HexaPathResult pathResult = new();
            PortPathContext pathContext = new();

            int count = 0;
            while (candidates.Count > 0 && count < maxPortCountSetting)
            {
                int selectIndex = random.NextInt32(0, candidates.Count);
                GeneratorProvince province = candidates[selectIndex];
                candidates.RemoveAt(selectIndex);

                GeneratorCell cityCell = province.CityCell;

                List<(GeneratorCell Cell, int Distance, int PathLength)> coastlineCells = province.LandCells
                    .Where(c => c.IsCoastlineLand)
                    .Select(c => (c, HexaCoord.Distance(c.Coord, cityCell.Coord), -1/*유효하지 않은 초기값*/))
                    .ToList();
                if (coastlineCells.Count <= 0)
                {
                    continue;
                }

                pathContext.Reset(province);
                for (int i = 0; i < coastlineCells.Count; i++)
                {
                    var (candidateCell, distance, _) = coastlineCells[i];

                    grid.FindPath(cityCell, candidateCell, pathResult, pathContext.Access, null, null);

                    if (pathResult.IsSucceed)
                    {
                        coastlineCells[i] = (candidateCell, distance, pathResult.ResultPath.Count);
                    }
                }

                List<(GeneratorCell Cell, int Distance, int PathLength)> reachableCells = coastlineCells
                    .Where(x => x.PathLength >= 0) // 음수는 유효하지 않은 값.
                    .OrderBy(x => x.PathLength).ThenBy(x => x.Distance)
                    .ToList();
                if (reachableCells.Count <= 0)
                {
                    continue;
                }

                var (_, minDistance, minPathLength) = reachableCells[0];

                List<(GeneratorCell Cell, int Distance, int PathLength)> minPathCells = reachableCells
                    .Where(x => x.Distance == minDistance && x.PathLength == minPathLength)
                    .ToList();

                GeneratorCell portCell;
                if (minPathCells.Count > 1)
                {
                    selectIndex = random.NextInt32(0, minPathCells.Count);
                    portCell = minPathCells[selectIndex].Cell;
                }
                else
                {
                    portCell = minPathCells[0].Cell;
                }

                province.PortCell = portCell;
                count++;
            }
        }

        static void AllocateOceanCellsToPorts(GeneratorGrid grid, IReadOnlyList<GeneratorProvince> provinces, int maxOceanPortDistanceSetting)
        {
            List<GeneratorCell> portCells = provinces
                .Where(p => p.PortCell is not null)
                .Select(p => p.PortCell!)
                .ToList();

            HexaMultiAreasResult areaResult = new();
            OceanAreaContext areaContext = new(maxOceanPortDistanceSetting);

            grid.CollectMultiAreas(portCells, areaResult, areaContext.Access, areaContext.Expand);

            foreach (var pair in areaResult.CellToStartingCells)
            {
                if (portCells.Contains(pair.Key))
                {
                    continue;
                }

                GeneratorProvince province = ((GeneratorCell)pair.Value).Province ?? throw new InvalidOperationException();
                GeneratorCell cell = (GeneratorCell)pair.Key;
                cell.Province = province;
                province.OceanCells.Add(cell);
            }
        }

        class LandAreaContext
        {
            public IEnumerable<HexaCell> Access(HexaCell _0/*start*/, HexaCell current)
            {
                return current.EnumerateNeighbors()
                    .Cast<GeneratorCell>()
                    .Where(c => c.IsLand);
            }
        }

        class PortPathContext
        {
            public GeneratorProvince? Province { get; private set; }

            public void Reset(GeneratorProvince province)
            {
                Province = province;
            }

            public IEnumerable<HexaCell> Access(HexaCell current)
            {
                return current.EnumerateNeighbors()
                    .Cast<GeneratorCell>()
                    .Where(c => c.IsLand && c.Province == Province);
            }
        }

        class OceanAreaContext
        {
            public int MaxDistance { get; private set; }

            public OceanAreaContext(int maxDistance)
            {
                MaxDistance = maxDistance;
            }

            public IEnumerable<HexaCell> Access(HexaCell _0/*start*/, HexaCell current)
            {
                return current.EnumerateNeighbors()
                    .Cast<GeneratorCell>()
                    .Where(c => !c.IsLand);
            }

            public bool Expand(HexaCell start, HexaCell _0/*current*/, HexaCell next)
            {
                return HexaCoord.Distance(start.Coord, next.Coord) <= MaxDistance;
            }
        }

        public struct Settings
        {
            public static Settings Default => new(4, 128, 32, 4);

            public int MinCityDistance;
            public int MaxProvinceCount;
            public int MaxPortCount;
            public int MaxOceanPortDistance;

            public Settings(int minCityDistance, int maxProvinceCount, int maxPortCount, int maxOceanPortDistance)
            {
                MinCityDistance = minCityDistance;
                MaxProvinceCount = maxProvinceCount;
                MaxPortCount = maxPortCount;
                MaxOceanPortDistance = maxOceanPortDistance;
            }
        }
    }
}
