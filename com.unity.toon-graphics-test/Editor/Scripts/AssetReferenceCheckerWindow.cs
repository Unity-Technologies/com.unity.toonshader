using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal class AssetReferenceCheckerWindow : EditorWindow {

    private enum AssetTypeFilter {
        Materials,
        Textures,
        Both
    }

    [MenuItem("Toon Shader/Asset Reference Checker")]
    public static void ShowWindow() {
        AssetReferenceCheckerWindow window = GetWindow<AssetReferenceCheckerWindow>("Asset Reference Checker");
        window.minSize = new Vector2(500, 300);
    }

    private AssetTypeFilter m_assetTypeFilter = AssetTypeFilter.Materials;

    private void OnGUI() {
        EditorGUILayout.LabelField("Scan Assets for References", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        m_rootPath = EditorGUILayout.TextField("Folder Path (under Assets)", m_rootPath);
        if (GUILayout.Button("Select...", GUILayout.Width(90))) {
            string selected = EditorUtility.OpenFolderPanel("Select Assets Folder", INITIAL_PATH, "");
            if (!string.IsNullOrEmpty(selected)) {
                string projectPath = Application.dataPath.Replace("/Assets", "");
                if (selected.StartsWith(projectPath)) {
                    m_rootPath = "Assets" + selected.Substring(projectPath.Length).Replace('\\', '/');
                } else {
                    EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder inside the project's Assets directory.", "OK");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        m_assetTypeFilter = (AssetTypeFilter)EditorGUILayout.EnumPopup("Asset Type", m_assetTypeFilter);

        EditorGUILayout.LabelField("Reference Sources", EditorStyles.boldLabel);
        m_includeScenes = EditorGUILayout.ToggleLeft("Scenes", m_includeScenes);
        m_includePrefabs = EditorGUILayout.ToggleLeft("Prefabs", m_includePrefabs);
        m_includeOtherAssets = EditorGUILayout.ToggleLeft("Other Assets (scripts, textures, etc.)", m_includeOtherAssets);

        EditorGUILayout.Space();

        if (GUILayout.Button("Scan Assets")) {
            ScanAssets();
        }

        EditorGUILayout.Space();

        if (m_hasScanned) {
            EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Shows which assets reference each material/texture. If an asset has no referencers, it may be unused.",
                MessageType.Info);

            m_scroll = EditorGUILayout.BeginScrollView(m_scroll);

            List<string> sortedKeys = new List<string>(m_assetToReferencers.Keys);
            sortedKeys.Sort();

            for (int i = 0; i < sortedKeys.Count; i++) {
                string assetPath = sortedKeys[i];
                List<string> referencers = m_assetToReferencers[assetPath];

                EditorGUILayout.BeginVertical("box");
                Object assetObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                EditorGUILayout.ObjectField("Asset", assetObj, assetObj != null ? assetObj.GetType() : typeof(Object), false);
                EditorGUILayout.LabelField("Path: " + assetPath);

                if (referencers == null || referencers.Count == 0) {
                    Color previousColor = GUI.color;
                    GUI.color = new Color(1f, 0.6f, 0.6f);
                    EditorGUILayout.LabelField("Referenced by: None", EditorStyles.miniLabel);
                    GUI.color = previousColor;
                } else {
                    EditorGUILayout.LabelField("Referenced by (" + referencers.Count + "):", EditorStyles.miniBoldLabel);
                    for (int r = 0; r < referencers.Count; r++) {
                        string refPath = referencers[r];
                        Object obj = AssetDatabase.LoadMainAssetAtPath(refPath);
                        EditorGUILayout.ObjectField(obj, typeof(Object), false);
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void ScanAssets() {
        m_assetToReferencers.Clear();
        m_hasScanned = false;

        if (string.IsNullOrEmpty(m_rootPath) || !m_rootPath.StartsWith("Assets")) {
            EditorUtility.DisplayDialog("Invalid Path", "Please enter a valid folder path under Assets.", "OK");
            return;
        }

        List<string> assetPaths = new List<string>();

        if (m_assetTypeFilter == AssetTypeFilter.Materials || m_assetTypeFilter == AssetTypeFilter.Both) {
            string[] materialGUIDs = AssetDatabase.FindAssets("t:Material", new[] { m_rootPath });
            for (int i = 0; i < materialGUIDs.Length; i++) {
                string path = AssetDatabase.GUIDToAssetPath(materialGUIDs[i]);
                if (!string.IsNullOrEmpty(path)) {
                    assetPaths.Add(path);
                    m_assetToReferencers[path] = new List<string>();
                }
            }
        }

        if (m_assetTypeFilter == AssetTypeFilter.Textures || m_assetTypeFilter == AssetTypeFilter.Both) {
            string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture", new[] { m_rootPath });
            for (int i = 0; i < textureGUIDs.Length; i++) {
                string path = AssetDatabase.GUIDToAssetPath(textureGUIDs[i]);
                if (!string.IsNullOrEmpty(path)) {
                    assetPaths.Add(path);
                    m_assetToReferencers[path] = new List<string>();
                }
            }
        }

        if (assetPaths.Count == 0) {
            string assetTypeMessage;
            switch (m_assetTypeFilter) {
                case AssetTypeFilter.Materials:
                    assetTypeMessage = "materials";
                    break;
                case AssetTypeFilter.Textures:
                    assetTypeMessage = "textures";
                    break;
                case AssetTypeFilter.Both:
                default:
                    assetTypeMessage = "materials or textures";
                    break;
            }
            EditorUtility.DisplayDialog("No Assets Found", $"No {assetTypeMessage} were found under: {m_rootPath}", "OK");
            m_hasScanned = true;
            return;
        }

        HashSet<string> candidatePaths = new HashSet<string>();

        if (m_includeScenes) {
            AddPathsByType(candidatePaths, "t:Scene");
        }

        if (m_includePrefabs) {
            AddPathsByType(candidatePaths, "t:Prefab");
        }

        if (m_includeOtherAssets) {
            string[] allGuids = AssetDatabase.FindAssets("", new[] { m_rootPath });
            for (int i = 0; i < allGuids.Length; i++) {
                string p = AssetDatabase.GUIDToAssetPath(allGuids[i]);
                if (string.IsNullOrEmpty(p)) continue;
                if (AssetDatabase.IsValidFolder(p)) continue;

                bool isTargetAsset = false;
                for (int m = 0; m < assetPaths.Count; m++) {
                    if (assetPaths[m] == p) {
                        isTargetAsset = true;
                        break;
                    }
                }

                if (isTargetAsset) continue;

                candidatePaths.Add(p);
            }
        }

        if (!m_includeScenes && !m_includePrefabs && !m_includeOtherAssets) {
            AddPathsByType(candidatePaths, "t:Scene");
            AddPathsByType(candidatePaths, "t:Prefab");
        }

        Dictionary<string, List<string>> dependencyToReferencers = new Dictionary<string, List<string>>();

        int processed = 0;
        int total = candidatePaths.Count;

        try {
            foreach (string assetPath in candidatePaths) {
                processed++;
                if (EditorUtility.DisplayCancelableProgressBar("Scanning Dependencies", assetPath, (float)processed / (float)total)) {
                    break;
                }

                string[] deps = AssetDatabase.GetDependencies(assetPath, true);
                for (int d = 0; d < deps.Length; d++) {
                    string dep = deps[d];
                    if (!m_assetToReferencers.ContainsKey(dep)) continue;

                    List<string> list;
                    if (!dependencyToReferencers.TryGetValue(dep, out list)) {
                        list = new List<string>();
                        dependencyToReferencers[dep] = list;
                    }

                    bool alreadyAdded = false;
                    for (int k = 0; k < list.Count; k++) {
                        if (list[k] == assetPath) {
                            alreadyAdded = true;
                            break;
                        }
                    }

                    if (!alreadyAdded) {
                        list.Add(assetPath);
                    }
                }
            }
        }
        finally {
            EditorUtility.ClearProgressBar();
        }

        for (int i = 0; i < assetPaths.Count; i++) {
            string assetPath = assetPaths[i];
            List<string> refs;
            if (dependencyToReferencers.TryGetValue(assetPath, out refs)) {
                refs.Sort();
                m_assetToReferencers[assetPath] = refs;
            } else {
                m_assetToReferencers[assetPath] = new List<string>();
            }
        }

        m_hasScanned = true;
    }

    private static void AddPathsByType(HashSet<string> set, string typeFilter) {
        string[] guids = AssetDatabase.FindAssets(typeFilter, null);
        for (int i = 0; i < guids.Length; i++) {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.IsNullOrEmpty(path)) continue;
            if (AssetDatabase.IsValidFolder(path)) continue;
            set.Add(path);
        }
    }

    private string m_rootPath = INITIAL_PATH;
    private Vector2 m_scroll;
    private bool m_includeScenes = true;
    private bool m_includePrefabs = true;
    private bool m_includeOtherAssets = true;

    private readonly Dictionary<string, List<string>> m_assetToReferencers = new Dictionary<string, List<string>>();
    private bool m_hasScanned = false;

    private const string INITIAL_PATH = "Assets/UnityChan/SD/Materials";
}
}
