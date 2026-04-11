// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using Jih.Unity.EraOfNitrogen.Worlds;
using Jih.Unity.Infrastructure;
using Jih.Unity.Infrastructure.Runtime;
using System;
using UnityEngine;

namespace Jih.Unity.EraOfNitrogen
{
    public class Assets : MonoBehaviour
    {
        static SingletonStorage<Assets> _instance;
        public static Assets Instance => _instance.Get();

        [Header("Terrain")]
        [SerializeField] Color32 _grasslandColor = new(0x91, 0xB9, 0x5B, 0xFF);
        [SerializeField] Color32 _rainforestColor = new(0x3B, 0x5D, 0x38, 0xFF);
        [SerializeField] Color32 _tundraColor = new(0xD0, 0xD7, 0xD9, 0xFF);
        [SerializeField] Color32 _steppColor = new(0xC4, 0xA4, 0x6B, 0xFF);
        [SerializeField] Color32 _desertColor = new(0xEB, 0xC0, 0x7A, 0xFF);
        [SerializeField] Color32 _snowColor = new(0xF2, 0xF2, 0xF2, 0xFF);

        [Space(12f)]
        [SerializeField] Color32 _nearOceanColor = new(0x59, 0xC9, 0xD5, 0xFF);
        [SerializeField] Color32 _farOceanColor = new(0x2C, 0x4A, 0x73, 0xFF);

        [Space(12f)]
        [SerializeField] Material? _landMaterial;
        public Material LandMaterial => _landMaterial.ThrowIfNull(nameof(LandMaterial));

        [SerializeField] Material? _oceanMaterial;
        public Material OceanMaterial => _oceanMaterial.ThrowIfNull(nameof(OceanMaterial));

        public Color32 GetColor(Biome biome)
        {
            return biome switch
            {
                Biome.Grassland => _grasslandColor,
                Biome.Rainforest => _rainforestColor,
                Biome.Tundra => _tundraColor,
                Biome.Steppe => _steppColor,
                Biome.Desert => _desertColor,
                Biome.Snow => _snowColor,
                _ => throw new NotImplementedException(),
            };
        }

        public Color32 GetOceanColor(bool isNearOcean)
        {
            return isNearOcean ? _nearOceanColor : _farOceanColor;
        }

        public Assets()
        {
            _instance = new SingletonStorage<Assets>(this);
        }
    }
}
