// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Geometries;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Editor.Geometries
{
    public abstract class BaseConvexHullGeneratorWindow : EditorWindow
    {
        [SerializeField] List<Mesh?> _sourceMeshes = new();
        SerializedObject? _serializedObject;
        SerializedProperty? _sourceMeshesProperty;

        string _saveDirectory = "Assets";
        string _fileNameFormat = "{0} ConvexHull";

        bool _previewConvexHull = true;
        GameObject? _previewRoot;
        Material? _convexHullPreviewMaterial, _sourceMeshPreviewMaterial;

        readonly List<Stats> _stats = new();
        bool _showStats = false;
        Vector2 _scrollPosition;

        protected abstract void CreatePreviewMaterials(out Material convexHullPreviewMaterial, out Material sourceMeshPreviewMaterial);
        protected abstract void GenerateHull(Vector3[] points, out List<Vector3> hullVertices, out List<int> hullTriangles);

        protected virtual void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _sourceMeshesProperty = _serializedObject.FindProperty(nameof(_sourceMeshes));

            CreatePreviewMaterials(out _convexHullPreviewMaterial, out _sourceMeshPreviewMaterial);
            ResetStats();
        }

        protected virtual void OnDisable()
        {
            DestroyPreview();
        }

        protected virtual void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _serializedObject?.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_sourceMeshesProperty, new GUIContent("Source Meshes"), true);
            if (EditorGUI.EndChangeCheck())
            {
                ResetStats();
                DestroyPreview();
            }
            _serializedObject?.ApplyModifiedProperties();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Save Directory", GUILayout.Width(110));
            _saveDirectory = EditorGUILayout.TextField(_saveDirectory, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
            {
                BrowseSaveDirectory();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Name Format", GUILayout.Width(110));
            _fileNameFormat = EditorGUILayout.TextField(_fileNameFormat, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("'{0}' will be replaced by source mesh name.\nExample: '{0} ConvexHull' -> 'SourceMeshName ConvexHull.asset'", MessageType.Info);

            EditorGUILayout.Space();
            _previewConvexHull = EditorGUILayout.Toggle("Preview Convex Hull", _previewConvexHull);

            EditorGUILayout.Space();
            bool hasValidMesh = _sourceMeshes.Exists(m => m != null);
            EditorGUI.BeginDisabledGroup(!hasValidMesh);

            if (GUILayout.Button("Save Convex Hulls"))
            {
                SaveConvexHulls();
            }

            EditorGUI.EndDisabledGroup();

            if (hasValidMesh && _previewConvexHull)
            {
                if (GUILayout.Button("Update Preview"))
                {
                    UpdatePreview();
                }
            }

            if (_previewRoot != null)
            {
                if (GUILayout.Button("Clear Preview"))
                {
                    DestroyPreview();
                }
            }

            DrawEfficiencyStatsList();

            EditorGUILayout.EndScrollView();
        }

        void DrawEfficiencyStatsList()
        {
            if (!_showStats || _stats.Count == 0)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Efficiency Statistics", EditorStyles.boldLabel);

            foreach (var stats in _stats)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField($"Target: {stats.MeshName}", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Original", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"Verts: {stats.OriginalVertices}");
                EditorGUILayout.LabelField($"Tris: {stats.OriginalTriangles}");
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Convex Hull", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"Verts: {stats.HullVertices}");
                EditorGUILayout.LabelField($"Tris: {stats.HullTriangles}");
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Efficiency", EditorStyles.miniBoldLabel);

                GUIStyle vStyle = new(EditorStyles.label) { normal = { textColor = GetEfficiencyColor(stats.VertexReduction), }, };
                GUIStyle tStyle = new(EditorStyles.label) { normal = { textColor = GetEfficiencyColor(stats.TriangleReduction), }, };

                EditorGUILayout.LabelField($"-{stats.VertexReduction:F1}% (V)", vStyle);
                EditorGUILayout.LabelField($"-{stats.TriangleReduction:F1}% (T)", tStyle);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }
        }

        void BrowseSaveDirectory()
        {
            string path = EditorUtility.OpenFolderPanel("Select Save Directory", _saveDirectory, "");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (!path.StartsWith(Application.dataPath))
            {
                Debug.LogWarning("Save directory must be inside the project's Assets folder.");
                return;
            }

            _saveDirectory = "Assets" + path[Application.dataPath.Length..];
        }

        void SaveConvexHulls()
        {
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }

            ResetStats();

            for (int i = 0; i < _sourceMeshes.Count; i++)
            {
                Mesh? mesh = _sourceMeshes[i];
                if (mesh == null)
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("Generating", $"Processing {mesh.name}", (float)i / _sourceMeshes.Count);

                GenerateHull(mesh.vertices, out List<Vector3> hullVertices, out List<int> hullTriangles);

                _stats.Add(CreateStats(mesh, hullVertices, hullTriangles));

                SerializableMesh serializableMesh = CreateInstance<SerializableMesh>();
                SetSerializableMeshData(serializableMesh, hullVertices, hullTriangles);

                string savePath = Path.Combine(_saveDirectory, string.Format(_fileNameFormat, mesh.name) + ".asset").Replace("\\", "/");
                AssetDatabase.CreateAsset(serializableMesh, savePath);

                Debug.Log("Saved: " + savePath);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            _showStats = true;

            if (_previewConvexHull)
            {
                UpdatePreview();
            }
        }

        static Stats CreateStats(Mesh source, IReadOnlyList<Vector3> hullVertices, IReadOnlyList<int> hullTriangles)
        {
            int originalV = source.vertexCount;
            int originalT = source.triangles.Length / 3;
            int hullV = hullVertices.Count;
            int hullT = hullTriangles.Count / 3;

            return new Stats(source.name, originalV, originalT, hullV, hullT,
                100f * (1f - (float)hullV / originalV),
                100f * (1f - (float)hullT / originalT));
        }

        void UpdatePreview()
        {
            DestroyPreview();
            ResetStats();

            if (_sourceMeshes.Count <= 0)
            {
                return;
            }

            _previewRoot = new GameObject("ConvexHull_Preview_Root") { hideFlags = HideFlags.DontSave, };

            foreach (var mesh in _sourceMeshes)
            {
                if (mesh == null)
                {
                    continue;
                }

                GenerateHull(mesh.vertices, out List<Vector3> hullVertices, out List<int> hullTriangles);
                _stats.Add(CreateStats(mesh, hullVertices, hullTriangles));

                string previewName = $"Preview_{mesh.name}";

                Mesh previewMesh = new() { name = previewName, };
                previewMesh.SetVertices(hullVertices);
                previewMesh.SetTriangles(hullTriangles, 0);
                previewMesh.RecalculateNormals();

                {
                    GameObject gameObject = new(previewName);
                    gameObject.transform.SetParent(_previewRoot.transform, false);

                    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = previewMesh;

                    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = _convexHullPreviewMaterial;
                }
                {
                    GameObject gameObject = new($"Source_{mesh.name}");
                    gameObject.transform.SetParent(_previewRoot.transform, false);

                    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = mesh;

                    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = _sourceMeshPreviewMaterial;
                }
            }

            _showStats = true;
            if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.Repaint();
        }

        void SetSerializableMeshData(SerializableMesh mesh, List<Vector3> vertices, List<int> triangles)
        {
            mesh.SetVertices(vertices);
            SerializableSubMesh subMesh = new();
            subMesh.SetIndices(triangles);
            mesh.SetSubMeshes(subMesh);
        }

        static Color GetEfficiencyColor(float reduction)
        {
            if (reduction >= 90f)
            {
                return Color.green;
            }
            if (reduction >= 70f)
            {
                return Color.Lerp(Color.yellow, Color.green, (reduction - 70f) / 20f);
            }
            if (reduction >= 30f)
            {
                return Color.Lerp(Color.red, Color.yellow, (reduction - 30f) / 40f);
            }
            return Color.red;
        }

        void ResetStats()
        {
            _stats.Clear();
            _showStats = false;
        }

        void DestroyPreview()
        {
            if (_previewRoot != null)
            {
                DestroyImmediate(_previewRoot);
                _previewRoot = null;
            }
        }

        struct Stats
        {
            public string MeshName;
            public int OriginalVertices;
            public int OriginalTriangles;
            public int HullVertices;
            public int HullTriangles;
            public float VertexReduction;
            public float TriangleReduction;

            public Stats(string meshName, int originalVertices, int originalTriangles, int hullVertices, int hullTriangles, float vertexReduction, float triangleReduction)
            {
                MeshName = meshName;
                OriginalVertices = originalVertices;
                OriginalTriangles = originalTriangles;
                HullVertices = hullVertices;
                HullTriangles = hullTriangles;
                VertexReduction = vertexReduction;
                TriangleReduction = triangleReduction;
            }
        }
    }
}
