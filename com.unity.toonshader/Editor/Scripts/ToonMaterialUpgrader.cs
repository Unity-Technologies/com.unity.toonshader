using System.Collections.Generic;
using Unity.Rendering.Toon;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal class ToonMaterialUpgrader : EditorWindow {

    [MenuItem("Window/Rendering/Unity Toon Material Upgrader", false, 60)]
    public static void ShowWindow() {
        ToonMaterialUpgrader window = GetWindow<ToonMaterialUpgrader>("Toon Material Upgrader");
        window.minSize = new Vector2(420f, 180f);
    }

    private void OnGUI() {
        EditorGUILayout.HelpBox(
            "This process makes irreversible changes to the project. Back up your project before proceeding.",
            MessageType.Warning);

        EditorGUILayout.Space();

        using (new EditorGUILayout.VerticalScope("box")) {
            EditorGUILayout.LabelField("Upgrade Materials Using ToonShader", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            if (GUILayout.Button("Upgrade Materials")) {
                UpgradeMaterials();
            }
        }
    }

//----------------------------------------------------------------------------------------------------------------------    
    private void UpgradeMaterials() {
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        if (materialGuids == null || materialGuids.Length == 0) {
            return;
        }

        HashSet<Shader> toonShaders = new HashSet<Shader>(FindToonShaders());
        if (toonShaders.Count<=0) {
            Debug.LogWarning("[UTS] No toon shaders detected.");
            return;
        }

        int processedCount = 0;

        for (int i = 0; i < materialGuids.Length; i++) {
            string guid = materialGuids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) {
                continue;
            }
            
            Shader matShader = mat.shader;
            if (!toonShaders.Contains(matShader)) {
                continue;
            }

            UpgradeMaterial(mat);
            EditorUtility.SetDirty(mat);
            processedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[UTS] Upgraded " + processedCount + " materials.");
    }

//----------------------------------------------------------------------------------------------------------------------
    private void UpgradeMaterial(Material m) {
        ToonMaterialEditorUtility.ApplyRenderPipelineKeyword(m);
    }

//----------------------------------------------------------------------------------------------------------------------

    private static List<Shader> FindToonShaders() {
        string[] shaderGuids = AssetDatabase.FindAssets("t:Shader", new string[] { PACKAGE_ROOT });

        if (shaderGuids == null || shaderGuids.Length == 0) {
            return new List<Shader>();
        }

        List<Shader> shaders = new List<Shader>(shaderGuids.Length);

        for (int i = 0; i < shaderGuids.Length; i++) {
            string guid = shaderGuids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // extra safety check
            if (string.IsNullOrEmpty(path) || !path.StartsWith(PACKAGE_ROOT))
                continue;

            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (shader != null) {
                shaders.Add(shader);
            }
        }

        return shaders;
    }

//----------------------------------------------------------------------------------------------------------------------
    static readonly string PACKAGE_ROOT = $"Packages/{ToonConstants.PACKAGE_NAME}";
    
}

}