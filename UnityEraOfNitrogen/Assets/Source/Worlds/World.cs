// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using Jih.Unity.EraOfNitrogen.Worlds.Runtime;
using Jih.Unity.Infrastructure;
using Jih.Unity.Infrastructure.Collisions.Common3D;
using Jih.Unity.Infrastructure.HexaGrid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Jih.Unity.EraOfNitrogen.Worlds
{
    [JsonObject]
    public class World
    {
        [JsonIgnore] Map? _map;
        [JsonIgnore] Map Map => _map.ThrowIfNull(nameof(Map));

        [JsonIgnore] public int Width => Map.Width;
        [JsonIgnore] public int Height => Map.Height;

        [JsonIgnore] public int RandomSeed => Map.RandomSeed;
        [JsonProperty] public long RandomPosition { get; private set; }

        [JsonProperty(nameof(Tiles))] readonly List<Tile> _tiles = new();
        [JsonIgnore] public IReadOnlyList<Tile> Tiles => _tiles;

        [JsonProperty(nameof(Provinces))] readonly List<Province> _provinces = new();
        [JsonIgnore] public IReadOnlyList<Province> Provinces => _provinces;

        [JsonIgnore, MemberNotNullWhen(true,
            nameof(_randomStream),
            nameof(_worldGrid))]
        public bool IsInitialized { get; private set; }

        [JsonIgnore] RandomStream? _randomStream;
        [JsonIgnore] public RandomStream RandomStream => _randomStream.ThrowIfNull(nameof(RandomStream));

        [JsonIgnore] WorldGrid? _worldGrid;
        [JsonIgnore] public WorldGrid MapGrid => _worldGrid.ThrowIfNull(nameof(MapGrid));

        // 노트 260415 참고.
        [JsonIgnore] readonly CollisionWorld _collisionWorld = new(cellSize: 0.5f);
        [JsonIgnore] public CollisionWorld CollisionWorld => _collisionWorld;

        [JsonConstructor]
        public World()
        {
        }

        public void Bind(Map map, bool initialBind)
        {
            _map = map;

            RandomPosition = 0;

            if (initialBind)
            {
                for (int i = 0; i < map.Tiles.Count; i++)
                {
                    _tiles.Add(new Tile());
                }
            }
            Dictionary<TileCoord, Tile> tilesMap = new(map.Tiles.Count);
            for (int i = 0; i < map.Tiles.Count; i++)
            {
                MapTile srcTile = map.Tiles[i];
                Tile destTile = _tiles[i];
                // 타일들에 대한 바인딩 일괄 진행.
                destTile.Bind(srcTile, initialBind);

                tilesMap.Add(srcTile.Coord, destTile);
            }

            if (initialBind)
            {
                for (int i = 0; i < map.Provinces.Count; i++)
                {
                    _provinces.Add(new Province());
                }
            }
            for (int i = 0; i < map.Provinces.Count; i++)
            {
                _provinces[i].Bind(map.Provinces[i], tilesMap, initialBind);
            }
        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            _randomStream = new RandomStream()
            {
                Position = RandomPosition,
            };

            Dictionary<uint, Province> provinceMap = new(_provinces.Count);
            foreach (var province in _provinces)
            {
                if (!provinceMap.TryAdd(province.Id, province))
                {
                    throw new InvalidOperationException($"프로빈스 ID {province.Id} 가 중복됨.");
                }
            }

            Tile[,] tiles = new Tile[Height, Width];
            foreach (var tile in _tiles)
            {
                HexaIndex index = (HexaIndex)tile.Coord;
                tiles[index.Y, index.X] = tile;
            }

            _worldGrid = new WorldGrid(Width, Height, tiles);

            foreach (var province in _provinces)
            {
                province.Initialize(this, provinceMap, _worldGrid);
            }

            foreach (var tile in _tiles)
            {
                if (tile.IsInitialized)
                {
                    continue;
                }
                tile.Initialize(this, null, _worldGrid[tile.Coord]);
            }

            IsInitialized = true;
        }

        [OnSerializing]
        void OnSerializingMethod(StreamingContext context)
        {
            RandomPosition = RandomStream?.Position ?? 0L;
        }
    }
}
