// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using GK;
using Jih.Unity.Infrastructure.Editor.Geometries;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Jih.Unity.EraOfNitrogen.Editor
{
    public class ConvexHullGeneratorWindow : BaseConvexHullGeneratorWindow
    {
        [MenuItem("JIH/Generate Convex Hull")]
        private static void ShowWindow()
        {
            GetWindow<ConvexHullGeneratorWindow>("Convex Hull Generator");
        }

        protected override Material CreatePreviewMaterial()
        {
            Shader? shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                Debug.LogWarning("Preview shader not found.", this);
            }
            return new Material(shader)
            {
                color = new Color(0.0f, 0.8f, 0.2f, 0.5f),
            };
        }

        protected override void GenerateHull(Vector3[] points, out List<Vector3> hullVertices, out List<int> hullTriangles)
        {
            List<Vector3> pointList = new(points);

            hullVertices = new List<Vector3>();
            hullTriangles = new List<int>();
            List<Vector3> hullNormals = new();

            ConvexHullCalculator calculator = new();
            calculator.GenerateHull(pointList, false, ref hullVertices, ref hullTriangles, ref hullNormals);
        }
    }
}
