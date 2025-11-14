using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Rendering;

public class ConvertMaterialsEditorWindow : EditorWindow {

    [MenuItem("Tools/Convert Materials")]
    public static void ShowWindow() {
        GetWindow<ConvertMaterialsEditorWindow>("Convert Materials");
    }

    void OnGUI() {
        m_targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", m_targetObject, typeof(GameObject), true);
        m_sourceShader = (Shader)EditorGUILayout.ObjectField("Source Shader", m_sourceShader, typeof(Shader), false);
        m_targetShader = (Shader)EditorGUILayout.ObjectField("Target Shader", m_targetShader, typeof(Shader), false);
        m_newMaterialsSuffix = EditorGUILayout.TextField("New Materials Suffix", m_newMaterialsSuffix);
        m_targetFolder = EditorGUILayout.TextField("Target Folder", m_targetFolder);

        if (GUILayout.Button("Convert Materials")) {
            ConvertMaterials();
        }
    }

    void ConvertMaterials() {
        if (m_targetObject == null || m_sourceShader == null || m_targetShader == null) {
            Debug.LogError("Please assign all fields.");
            return;
        }

        // Ensure unique folder
        string uniqueFolder = AssetDatabase.GenerateUniqueAssetPath(m_targetFolder);
        Directory.CreateDirectory(uniqueFolder);
        AssetDatabase.Refresh();

        Renderer[] renderers = m_targetObject.GetComponentsInChildren<Renderer>(true);
        for (int r = 0; r < renderers.Length; r++) {
            Renderer renderer = renderers[r];
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
                Material mat = renderer.sharedMaterials[i];
                if (mat != null && mat.shader == m_sourceShader) {
                    Material newMat = new Material(m_targetShader);
                    newMat.name = mat.name + m_newMaterialsSuffix;
                    CopyMaterialProperties(mat, newMat);
                    string path = AssetDatabase.GenerateUniqueAssetPath(uniqueFolder + "/" + newMat.name + ".mat");
                    AssetDatabase.CreateAsset(newMat, path);
                    newMaterials[i] = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
                else {
                    newMaterials[i] = mat;
                }
            }
            renderer.sharedMaterials = newMaterials;
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Material conversion complete.");
    }

    void CopyMaterialProperties(Material source, Material target) {
        Shader shader = source.shader;
        int propertyCount = shader.GetPropertyCount();
        for (int i = 0; i < propertyCount; i++) {
            string propName = shader.GetPropertyName(i);
            ShaderPropertyType propType = shader.GetPropertyType(i);

            if (target.HasProperty(propName)) {
                switch (propType) {
                    case ShaderPropertyType.Color:
                        target.SetColor(propName, source.GetColor(propName));
                        break;
                    case ShaderPropertyType.Vector:
                        target.SetVector(propName, source.GetVector(propName));
                        break;
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        target.SetFloat(propName, source.GetFloat(propName));
                        break;
                    case ShaderPropertyType.Texture:
                        target.SetTexture(propName, source.GetTexture(propName));
                        target.SetTextureOffset(propName, source.GetTextureOffset(propName));
                        target.SetTextureScale(propName, source.GetTextureScale(propName));
                        break;
                }
            }
        }
    }

    private GameObject m_targetObject;
    private Shader m_sourceShader;
    private Shader m_targetShader;
    private string m_targetFolder = "Assets/ConvertedMaterials";
    private string m_newMaterialsSuffix = "_Converted";

}
