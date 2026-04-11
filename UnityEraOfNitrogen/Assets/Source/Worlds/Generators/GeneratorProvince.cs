// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using System.Collections.Generic;

namespace Jih.Unity.EraOfNitrogen.Worlds.Generators
{
    public class GeneratorProvince
    {
        /// <summary>
        /// 동일 월드 내 프로빈스들 사이에서 유일한 값.
        /// </summary>
        public uint Id { get; }

        public GeneratorCell CityCell { get; }
        /// <summary>
        /// <see cref="CityCell"/> 포함.
        /// </summary>
        public List<GeneratorCell> Cells { get; } = new();

        public List<GeneratorProvince> AdjacentProvinces { get; } = new();

        public Biome Biome { get; set; }

        public GeneratorProvince(uint id, GeneratorCell cityCell)
        {
            Id = id;
            CityCell = cityCell;
        }
    }
}
