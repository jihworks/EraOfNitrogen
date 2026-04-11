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
    class BiomeGenerator
    {
        public enum ModelType
        {
            NorthernHemisphere,
            Earth,
        }

        readonly Settings _settings;
        readonly RandomStream _random;
        readonly IReadOnlyList<GeneratorProvince> _provinces;

        public BiomeGenerator(Settings settings, RandomStream random, IReadOnlyList<GeneratorProvince> provinces)
        {
            _settings = settings;
            _random = random;
            _provinces = provinces;
        }

        public void Execute()
        {
            CalculateLandBounds(_provinces, out int minX, out int maxX, out int minY, out int maxY);

            foreach (var province in _provinces)
            {
                province.Biome = DetermineBiome(_random, minX, maxX, minY, maxY, province, _settings.Model, _settings.TemperatureNoiseAmount, _settings.HumidityNoiseAmount, _settings.MinContinentalHumidity);
            }
        }

        static void CalculateLandBounds(IReadOnlyList<GeneratorProvince> provinces, out int minX, out int maxX, out int minY, out int maxY)
        {
            minX = minY = int.MaxValue;
            maxX = maxY = int.MinValue;

            foreach (var province in provinces)
            {
                foreach (var cell in province.Cells)
                {
                    HexaIndex index = cell.Index;

                    MathEx.Min(ref minX, index.X);
                    MathEx.Max(ref maxX, index.X);

                    MathEx.Min(ref minY, index.Y);
                    MathEx.Max(ref maxY, index.Y);
                }
            }

            MathEx.Max(ref maxX, minX + 1);
            MathEx.Max(ref maxY, minY + 1);
        }

        static Biome DetermineBiome(RandomStream random, int minX, int maxX, int minY, int maxY, GeneratorProvince province, ModelType model, double temperatureNoiseAmount, double humidityNoiseAmount, double minContinentalHumidity)
        {
            double avgX = province.Cells.Average(c => c.Index.X);
            double avgY = province.Cells.Average(c => c.Index.Y);

            double temperature = CalculateBaseTemperature(minY, maxY, avgY, model);
            temperature += (random.NextDouble() - 0.5) * temperatureNoiseAmount;
            temperature = Math.Clamp(temperature, 0.0, 1.0);

            double humidity = CalculateBaseHumidity(minX, maxX, avgX, minContinentalHumidity);
            humidity += (random.NextDouble() - 0.5) * humidityNoiseAmount;
            humidity = Math.Clamp(humidity, 0.0, 1.0);

            if (temperature < 0.05)
            {
                return Biome.Snow; // 극지방: 설원
            }
            else if (temperature < 0.15)
            {
                return Biome.Tundra; // 한대: 툰드라
            }
            else if (temperature < 0.85)
            {
                // 온대
                return humidity >= 0.45 ? Biome.Grassland : Biome.Steppe;
            }
            else
            {
                // 열대
                return humidity >= 0.55 ? Biome.Rainforest : Biome.Desert;
            }
        }

        static double CalculateBaseTemperature(int minY, int maxY, double avgY, ModelType model)
        {
            double landHeight = maxY - minY;
            double normalizedY = (avgY - minY) / landHeight;

            if (model is ModelType.Earth)
            {
                double distanceFromEquator = Math.Abs(normalizedY - 0.5) * 2.0;
                return 1.0 - distanceFromEquator;
            }
            else
            {
                return normalizedY;
            }
        }

        static double CalculateBaseHumidity(int minX, int maxX, double avgX, double minContinentalHumidity)
        {
            double landWidth = maxX - minX;
            double normalizedX = (avgX - minX) / landWidth;

            // 대륙의 중심(0.5)일수록 0에 가깝고, 양 끝단(0.0, 1.0)일수록 1에 가까워짐.
            double distanceFromCenter = Math.Abs(normalizedX - 0.5) * 2.0;

            // 내륙 한가운데라도 습도가 0이 되지 않도록 최소 보장치 적용.
            return minContinentalHumidity + (1.0 - minContinentalHumidity) * distanceFromCenter;
        }

        public struct Settings
        {
            public static Settings Default => new(ModelType.NorthernHemisphere, 0.5, 0.75, 0.2);

            public ModelType Model;
            public double TemperatureNoiseAmount;
            public double HumidityNoiseAmount;
            public double MinContinentalHumidity;

            public Settings(ModelType model, double tempNoise, double humidNoise, double minContinentalHumidity)
            {
                Model = model;
                TemperatureNoiseAmount = tempNoise;
                HumidityNoiseAmount = humidNoise;
                MinContinentalHumidity = minContinentalHumidity;
            }
        }
    }
}
