// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using Jih.Unity.Infrastructure;
using Jih.Unity.Infrastructure.Geometries;
using Jih.Unity.Infrastructure.HexaGrid;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Jih.Unity.EraOfNitrogen.CoordinateSpaceEx;

namespace Jih.Unity.EraOfNitrogen.Worlds.Runtime
{
    public class WorldMeshBuilder
    {
        public World World { get; }
        public Assets Assets { get; }

        public WorldMeshBuilder(World world, Assets? assets = null)
        {
            World = world;
            Assets = assets != null ? assets : Assets.Instance;
        }

        public List<LandChunk> BuildLand()
        {
            if (!World.IsInitialized)
            {
                throw new InvalidOperationException("초기화되지 않은 월드로부터 메시를 빌드할 수 없음.");
            }

            MapGrid grid = World.MapGrid;

            int chunkCountX = grid.Width.CeilDivision(ChunkSize);
            int chunkCountY = grid.Height.CeilDivision(ChunkSize);

            List<LandChunk> chunks = new(chunkCountX * chunkCountY);
            for (int cy = 0; cy < chunkCountY; cy++)
            {
                int baseGridY = cy * ChunkSize;

                for (int cx = 0; cx < chunkCountX; cx++)
                {
                    int baseGridX = cx * ChunkSize;

                    List<MapCell> cells = new(ChunkSize * ChunkSize);
                    for (int dy = 0; dy < ChunkSize; dy++)
                    {
                        int gridY = baseGridY + dy;

                        for (int dx = 0; dx < ChunkSize; dx++)
                        {
                            int gridX = baseGridX + dx;

                            MapCell? cell = grid.GetCell(new HexaIndex(gridX, gridY));
                            if (cell is null)
                            {
                                continue;
                            }
                            if (!cell.Tile.IsLand) // 땅인지 확인.
                            {
                                continue;
                            }

                            cells.Add(cell);
                        }
                    }

                    if (cells.Count > 0)
                    {
                        chunks.Add(new LandChunk(cx, cy, baseGridX, baseGridY, cells, new MeshCollector(AdditionalAttributes.Color)));
                    }
                }
            }

            foreach (var chunk in chunks)
            {
                CollectLandChunk(Assets, chunk);
            }

            return chunks;
        }

        public List<GameObject> Spawn(IReadOnlyList<LandChunk> chunks)
        {
            Material landMaterial = Assets.LandMaterial;
            Material[] materials = new Material[] { landMaterial, };

            List<GameObject> meshObjs = new(chunks.Count);

            foreach (var chunk in chunks)
            {
                string name = $"Land Mesh {chunk.ChunkX} x {chunk.ChunkY}";

                Mesh mesh = chunk.MeshCollector.GetTrianglesMesh(true, true);
                mesh.name = name;

                GameObject meshObj = new() { name = name, };
                meshObjs.Add(meshObj);

                MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;

                MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterials = materials;
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            }

            return meshObjs;
        }

        static void CollectLandChunk(Assets assets, in LandChunk chunk)
        {
            MeshCollector meshCollector = chunk.MeshCollector;

            VertexData[] cwCornersBuffer = new VertexData[6];

            foreach (var cell in chunk.Cells)
            {
                Tile tile = cell.Tile;
                if (!tile.IsLand)
                {
                    throw new InvalidOperationException($"땅이 아닌 타일 {cell.Coord} 로부터 땅 메시를 생성할 수 없음.");
                }

                if (!tile.IsInitialized)
                {
                    throw new InvalidOperationException($"초기화되지 않은 타일 {cell.Coord} 로부터 땅 메시를 생성할 수 없음.");
                }

                Province? province = tile.Province ?? throw new InvalidOperationException($"땅 타일 {cell.Coord} 인데 프로빈스가 없음.");

                Color32 biomeColor = assets.GetColor(province.Biome);

                Vector2 screenCenterLocation = HexaToScreen(cell.Coord);
                Vector3 unityCenterLocation = ScreenToUnity(screenCenterLocation);

                VertexData center = new(unityCenterLocation, biomeColor);

                for (int v = 0; v < 6; v++)
                {
                    HexaVertexPosition position = (HexaVertexPosition)v;
                    Vector2 screenVertexOffset = GetScreenVertexOffset(position);
                    Vector3 unityVertexOffset = ScreenToUnity(screenVertexOffset);

                    cwCornersBuffer[v] = new VertexData(unityCenterLocation + unityVertexOffset, biomeColor);
                }

                meshCollector.AppendNGon(center, cwCornersBuffer);
            }
        }

        public readonly struct LandChunk
        {
            public readonly int ChunkX, ChunkY;
            public readonly int GridX, GridY;
            public readonly IReadOnlyList<MapCell> Cells;
            public readonly MeshCollector MeshCollector;

            public LandChunk(int chunkX, int chunkY, int gridX, int gridY, IReadOnlyList<MapCell> cells, MeshCollector meshCollector)
            {
                ChunkX = chunkX;
                ChunkY = chunkY;
                GridX = gridX;
                GridY = gridY;
                Cells = cells;
                MeshCollector = meshCollector;
            }
        }

        const int ChunkSize = 16;
    }
}
