// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using Jih.Unity.Infrastructure.HexaGrid;
using System.Collections.Generic;
using System.Linq;

namespace Jih.Unity.EraOfNitrogen.Worlds
{
    public class MapGrid : HexaMap
    {
        public MapGrid(int width, int height)
            : base(width, height,
                  (map, index, coord) => new MapCell(map, index, coord),
                  null,
                  null)
        {
        }

        public new MapCell? GetCell(HexaCoord coord)
        {
            return (MapCell?)base.GetCell((HexaIndex)coord);
        }
        public new MapCell? GetCell(HexaIndex index)
        {
            return (MapCell?)base.GetCell(index.X, index.Y);
        }
        public new MapCell? GetCell(int x, int y)
        {
            return (MapCell?)base.GetCell(x, y);
        }

        public new IEnumerable<MapCell> EnumerateCells()
        {
            return base.EnumerateCells().Cast<MapCell>();
        }
    }
}
