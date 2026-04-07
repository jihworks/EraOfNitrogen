// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using Jih.Unity.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jih.Unity.EraOfNitrogen.Worlds.Runtime
{
    class PangaeaGenerator
    {
        readonly Settings _settings;
        readonly MapGrid _mapGrid;
        readonly RandomStream _random;

        public List<MapCell>? ResultLandCells { get; private set; }

        public PangaeaGenerator(Settings settings, MapGrid mapGrid, RandomStream random)
        {
            _settings = settings;
            _mapGrid = mapGrid;
            _random = random;
        }

        public void Execute()
        {
            HashSet<MapCell> landCells = GenerateLandmass(_random, _mapGrid, _settings.LandRatio);

            RemoveInlandWater(landCells, _mapGrid);

            List<MapCell> landCellsList = new(landCells);

            foreach (var cell in _mapGrid.EnumerateCells())
            {
                cell.IsLand = false;
                cell.IsCoastlineLand = false;
            }
            foreach (var landCell in landCellsList)
            {
                landCell.IsLand = true;
            }
            foreach (var landCell in landCellsList)
            {
                // If there is any ocean cell, it is coastline land.
                landCell.IsCoastlineLand = landCell.EnumerateNeighbors().Any(n => !n.IsLand);
            }

            ResultLandCells = landCellsList;
        }

        static HashSet<MapCell> GenerateLandmass(RandomStream random, MapGrid mapGrid, double landRatioSetting)
        {
            HashSet<MapCell> landCells = new();
            int centerX = mapGrid.Width / 2;
            int centerY = mapGrid.Height / 2;

            // Set starting point.
            MapCell? centerCell = mapGrid.GetCell(centerX, centerY);
            if (centerCell is not null)
            {
                landCells.Add(centerCell);
            }

            int targetLandCells = (int)(mapGrid.Width * mapGrid.Height * landRatioSetting);
            Queue<MapCell> expansionFrontier = new();
            expansionFrontier.Enqueue(centerCell!);

            while (landCells.Count < targetLandCells && expansionFrontier.Count > 0)
            {
                MapCell current = expansionFrontier.Dequeue();

                foreach (var neighbor in current.EnumerateNeighbors())
                {
                    if (landCells.Contains(neighbor))
                    {
                        continue;
                    }

                    float expandChance = CalculateExpandProbability(neighbor, landCells, mapGrid);
                    if (random.NextDouble() < expandChance)
                    {
                        landCells.Add(neighbor);
                        expansionFrontier.Enqueue(neighbor);
                    }
                }
            }

            return landCells;
        }

        static float CalculateExpandProbability(MapCell cell, HashSet<MapCell> landCells, MapGrid mapGrid)
        {
            // Reduce probability by distance from center.
            Vector2Int centerIndex = new(mapGrid.Width / 2, mapGrid.Height / 2);
            Vector2Int cellIndex = new(cell.Index.X, cell.Index.Y);
            float distanceFromCenter = (cellIndex - cellIndex).magnitude;

            float maxDistance = new Vector2Int(mapGrid.Width, mapGrid.Height).magnitude * 0.5f;
            float baseChance = 1f - (distanceFromCenter / maxDistance);

            // Increase probability by land adjacent cell count.
            int adjacentLandCount = cell.EnumerateNeighbors().Count(adj => landCells.Contains(adj));
            float adjacentBonus = adjacentLandCount * 0.1f;

            return Mathf.Clamp01(baseChance + adjacentBonus);
        }

        static void RemoveInlandWater(HashSet<MapCell> landCells, MapGrid mapGrid)
        {
            HashSet<MapCell> waterCells = new(mapGrid.EnumerateCells().Where(c => !landCells.Contains(c)));
            HashSet<MapCell> processedWater = new();

            while (waterCells.Count > 0)
            {
                HashSet<MapCell> waterGroup = FindConnectedWaterCells(waterCells.First(), waterCells);

                bool isInland = IsWaterGroupInland(waterGroup, landCells);

                // If inland lake, change to land.
                if (isInland)
                {
                    foreach (var cell in waterGroup)
                    {
                        landCells.Add(cell);
                        waterCells.Remove(cell);
                    }
                }
                else
                {
                    // If coast water, check as processed.
                    foreach (var cell in waterGroup)
                    {
                        waterCells.Remove(cell);
                        processedWater.Add(cell);
                    }
                }
            }
        }

        static HashSet<MapCell> FindConnectedWaterCells(MapCell start, HashSet<MapCell> waterCells)
        {
            HashSet<MapCell> connected = new();
            Queue<MapCell> queue = new();

            queue.Enqueue(start);
            connected.Add(start);

            while (queue.Count > 0)
            {
                MapCell current = queue.Dequeue();

                foreach (var neighbor in current.EnumerateNeighbors())
                {
                    if (waterCells.Contains(neighbor) && !connected.Contains(neighbor))
                    {
                        connected.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return connected;
        }

        static bool IsWaterGroupInland(HashSet<MapCell> waterGroup, HashSet<MapCell> landCells)
        {
            foreach (var waterCell in waterGroup)
            {
                // If a cell at border of the map, it is ocean.
                if (waterCell.EnumerateNeighbors().Count() < 6)
                {
                    return false;
                }
            }

            // Collect border cells of the water group.
            HashSet<MapCell> borderCells = new();
            foreach (var waterCell in waterGroup)
            {
                foreach (var neighbor in waterCell.EnumerateNeighbors())
                {
                    if (!waterGroup.Contains(neighbor))
                    {
                        borderCells.Add(waterCell);
                        break;
                    }
                }
            }

            // If all border cells are contact with land, it is a lake.
            foreach (var borderCell in borderCells)
            {
                foreach (var neighbor in borderCell.EnumerateNeighbors())
                {
                    // If neighbor is not land and not included in this water group, it is coast.
                    if (!landCells.Contains(neighbor) && !waterGroup.Contains(neighbor))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public struct Settings
        {
            public static Settings Default => new(0.7);

            public double LandRatio;

            public Settings(double landRatio)
            {
                LandRatio = landRatio;
            }
        }
    }
}
