using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Rendering.Toon;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal class ToonMaterialUpgrader : EditorWindow {

    [MenuItem("Window/Rendering/Unity Toon Material Upgrader", false, 60)]
    public static void ShowWindow() {
        ToonMaterialUpgrader window = GetWindow<ToonMaterialUpgrader>("Unity Toon Material Upgrader");
        window.minSize = new Vector2(420f, 180f);
    }

    private void OnGUI() {
        EditorGUILayout.HelpBox(
            "This process makes irreversible changes to the project. Back up your project before proceeding.",
            MessageType.Warning);

        EditorGUILayout.Space();

        using (new EditorGUILayout.VerticalScope("box")) {
            EditorGUILayout.LabelField($"Upgrade Unity ToonShader Materials to version " +
                $"{ToonEditorConstants.CUR_MATERIAL_VERSION}", EditorStyles.boldLabel);

            EditorGUILayout.LabelField($"(Compatible with " +
                $"{ToonConstants.PACKAGE_NAME}@{ToonConstants.PACKAGE_VERSION_MAJOR_MINOR})", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Find Old Unity ToonShader Materials")) {
                ListApplicableMaterials();
            }
            EditorGUILayout.Space();
            if (m_materials.Count > 0) {
                EditorGUILayout.LabelField($"Found {m_materials.Count} applicable materials:", EditorStyles.boldLabel);
                m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.ExpandHeight(true));
                for (int i = 0; i < m_materials.Count; i++) {
                    Material mat = m_materials[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(mat, typeof(Material), false);
                    EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(mat));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Upgrade Materials")) {
                    UpgradeMaterials();
                }
            }
            else {
                EditorGUILayout.LabelField("No applicable materials found.");
            }
        }
    }

//----------------------------------------------------------------------------------------------------------------------    
    private void UpgradeMaterials() {
        if (m_materials == null || m_materials.Count == 0) {
            Debug.LogWarning("[UTS] No applicable materials to upgrade.");
            return;
        }
        int processedCount = 0;
        for (int i = 0; i < m_materials.Count; i++) {
            Material mat = m_materials[i];
            UpgradeMaterial(mat);
            EditorUtility.SetDirty(mat);
            processedCount++;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[UTS] Upgraded " + processedCount + " materials.");
    }
    
//----------------------------------------------------------------------------------------------------------------------    
    // Lists all applicable materials and stores them for display and upgrade
    private void ListApplicableMaterials() {
        m_materials.Clear();
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        if (materialGuids == null || materialGuids.Length == 0) {
            return;
        }

        if (null == m_cachedToonShaders || m_cachedToonShaders.Count <= 0) {
            m_cachedToonShaders = FindToonShaders();
        }

        if (null == m_cachedToonShaders || m_cachedToonShaders.Count <= 0) {
            Debug.LogWarning("[UTS] No toon shaders detected.");
            return;
        }
        
        for (int i = 0; i < materialGuids.Length; i++) {
            string guid = materialGuids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) {
                continue;
            }
            Shader matShader = mat.shader;
            if (!m_cachedToonShaders.Contains(matShader)) {
                continue;
            }
            
            //check version
            int matVersion = ToonMaterialEditorUtility.GetMaterialVersion(mat);
            if (matVersion >= ToonEditorConstants.CUR_MATERIAL_VERSION) {
                continue;
            }
            
            m_materials.Add(mat);
        }
    }

//----------------------------------------------------------------------------------------------------------------------
    private void UpgradeMaterial(Material m) {
        ToonMaterialEditorUtility.ApplyRenderPipelineKeyword(m);
    }

//----------------------------------------------------------------------------------------------------------------------

    [CanBeNull]
    private static ISet<Shader> FindToonShaders() {
        string[] shaderGuids = AssetDatabase.FindAssets("t:Shader", new string[] { PACKAGE_ROOT });
        if (shaderGuids == null || shaderGuids.Length == 0) {
            return null;
        }

        HashSet<Shader> shaders = new HashSet<Shader>(shaderGuids.Length);
        for (int i = 0; i < shaderGuids.Length; i++) {
            string guid = shaderGuids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // extra safety check
            if (string.IsNullOrEmpty(path) || !path.StartsWith(PACKAGE_ROOT)) {
                continue;
            }

            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (shader != null) {
                shaders.Add(shader);
            }
        }

        return shaders;
    }

//----------------------------------------------------------------------------------------------------------------------

    // Store found materials for listing and upgrade
    private readonly List<Material> m_materials = new List<Material>();
    private Vector2 m_scrollPos = Vector2.zero;

    // Cache for toon shaders
    private ISet<Shader> m_cachedToonShaders = null;
    
    
//----------------------------------------------------------------------------------------------------------------------
    static readonly string PACKAGE_ROOT = $"Packages/{ToonConstants.PACKAGE_NAME}";
    
}

}