using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal class MaterialReferenceCheckerWindow : EditorWindow {

    [MenuItem("Tools/Material Reference Checker")]
    public static void ShowWindow() {
        MaterialReferenceCheckerWindow window = GetWindow<MaterialReferenceCheckerWindow>("Material Reference Checker");
        window.minSize = new Vector2(500, 300);
    }

    private void OnGUI() {
        EditorGUILayout.LabelField("Scan Materials for References", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        rootPath = EditorGUILayout.TextField("Folder Path (under Assets)", rootPath);
        if (GUILayout.Button("Select...", GUILayout.Width(90))) {
            string selected = EditorUtility.OpenFolderPanel("Select Assets Folder", INITIAL_PATH, "");
            if (!string.IsNullOrEmpty(selected)) {
                Debug.Log(selected);
                string projectPath = Application.dataPath.Replace("/Assets", "");
                if (selected.StartsWith(projectPath)) {
                    rootPath = "Assets" + selected.Substring(projectPath.Length).Replace('\\', '/');
                } else {
                    EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder inside the project's Assets directory.", "OK");
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Reference Sources", EditorStyles.boldLabel);
        includeScenes = EditorGUILayout.ToggleLeft("Scenes", includeScenes);
        includePrefabs = EditorGUILayout.ToggleLeft("Prefabs", includePrefabs);
        includeOtherAssets = EditorGUILayout.ToggleLeft("Other Assets (scripts, textures, etc.)", includeOtherAssets);

        EditorGUILayout.Space();

        if (GUILayout.Button("Scan Materials")) {
            ScanMaterials();
        }

        EditorGUILayout.Space();

        if (hasScanned) {
            EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Shows which assets reference each material. If a material has no referencers, it may be unused.",
                MessageType.Info);

            scroll = EditorGUILayout.BeginScrollView(scroll);

            List<string> sortedKeys = new List<string>(materialToReferencers.Keys);
            sortedKeys.Sort();

            for (int i = 0; i < sortedKeys.Count; i++) {
                string materialPath = sortedKeys[i];
                List<string> referencers = materialToReferencers[materialPath];

                EditorGUILayout.BeginVertical("box");
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                EditorGUILayout.ObjectField("Material", mat, typeof(Material), false);
                EditorGUILayout.LabelField("Path: " + materialPath);

                if (referencers == null || referencers.Count == 0) {
                    Color previousColor = GUI.color;
                    GUI.color = new Color(1f, 0.6f, 0.6f); // softer red than pure Color.red
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

    private void ScanMaterials() {
        materialToReferencers.Clear();
        hasScanned = false;

        if (string.IsNullOrEmpty(rootPath) || !rootPath.StartsWith("Assets")) {
            EditorUtility.DisplayDialog("Invalid Path", "Please enter a valid folder path under Assets.", "OK");
            return;
        }

        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material", new[] { rootPath });
        List<string> materialPaths = new List<string>();
        for (int i = 0; i < materialGUIDs.Length; i++) {
            string path = AssetDatabase.GUIDToAssetPath(materialGUIDs[i]);
            if (!string.IsNullOrEmpty(path)) {
                materialPaths.Add(path);
                materialToReferencers[path] = new List<string>();
            }
        }

        if (materialPaths.Count == 0) {
            EditorUtility.DisplayDialog("No Materials Found", "No materials were found under: " + rootPath, "OK");
            hasScanned = true;
            return;
        }

        // Build candidate referencer set
        HashSet<string> candidatePaths = new HashSet<string>();

        if (includeScenes) {
            AddPathsByType(candidatePaths, "t:Scene");
        }

        if (includePrefabs) {
            AddPathsByType(candidatePaths, "t:Prefab");
        }

        if (includeOtherAssets) {
            string[] allGuids = AssetDatabase.FindAssets("", new[] { rootPath });
            for (int i = 0; i < allGuids.Length; i++) {
                string p = AssetDatabase.GUIDToAssetPath(allGuids[i]);
                if (string.IsNullOrEmpty(p)) continue;
                if (AssetDatabase.IsValidFolder(p)) continue;

                // skip materials themselves
                bool isMaterialPath = false;
                for (int m = 0; m < materialPaths.Count; m++) {
                    if (materialPaths[m] == p) {
                        isMaterialPath = true;
                        break;
                    }
                }

                if (isMaterialPath) continue;

                candidatePaths.Add(p);
            }
        }

        if (!includeScenes && !includePrefabs && !includeOtherAssets) {
            AddPathsByType(candidatePaths, "t:Scene");
            AddPathsByType(candidatePaths, "t:Prefab");
        }

        Dictionary<string, List<string>> dependencyToReferencers = new Dictionary<string, List<string>>();

        int processed = 0;
        int total = candidatePaths.Count;

        Debug.Log("rootPath: " + rootPath);

        try {
            foreach (string assetPath in candidatePaths) {
                processed++;
                if (EditorUtility.DisplayCancelableProgressBar("Scanning Dependencies", assetPath, (float)processed / (float)total)) {
                    break;
                }

                Debug.Log("processing: " + assetPath);

                string[] deps = AssetDatabase.GetDependencies(assetPath, true);
                for (int d = 0; d < deps.Length; d++) {
                    string dep = deps[d];
                    if (!materialToReferencers.ContainsKey(dep)) continue;

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

        for (int i = 0; i < materialPaths.Count; i++) {
            string matPath = materialPaths[i];
            List<string> refs;
            if (dependencyToReferencers.TryGetValue(matPath, out refs)) {
                refs.Sort();
                materialToReferencers[matPath] = refs;
            } else {
                materialToReferencers[matPath] = new List<string>();
            }
        }

        hasScanned = true;
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

//----------------------------------------------------------------------------------------------------------------------
    private string rootPath = INITIAL_PATH;
    private Vector2 scroll;
    private bool includeScenes = true;
    private bool includePrefabs = true;
    private bool includeOtherAssets = true;

    private Dictionary<string, List<string>> materialToReferencers = new Dictionary<string, List<string>>();
    private bool hasScanned = false;

    private const string INITIAL_PATH = "Assets/UnityChan/SD/Materials";

}
}