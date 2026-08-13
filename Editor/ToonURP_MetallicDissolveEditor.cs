using UnityEditor;
using UnityEngine;

public class ToonURP_MetallicDissolveEditor : ShaderGUI
{
    MaterialEditor m_MaterialEditor;
    MaterialProperty[] m_Properties;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        m_MaterialEditor = materialEditor;
        m_Properties = props;

        Material targetMat = materialEditor.target as Material;

        EditorGUILayout.LabelField("Toon URP - Metallic + Dissolve", EditorStyles.boldLabel);

        // Base
        m_MaterialEditor.ShaderProperty(FindProp("_BaseColor", props), "Base Color");
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Base Map"), FindProp("_BaseMap", props));
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), FindProp("_BumpMap", props));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("PBR (Metallic/Roughness)", EditorStyles.boldLabel);
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Metallic Map"), FindProp("_MetallicMap", props));
        m_MaterialEditor.ShaderProperty(FindProp("_Metallic", props), "Metallic");
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Roughness Map"), FindProp("_RoughnessMap", props));
        m_MaterialEditor.ShaderProperty(FindProp("_Roughness", props), "Roughness");
        m_MaterialEditor.ShaderProperty(FindProp("_InvertSmoothness", props), "Invert Roughness Map (as Smoothness)");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Toon Blend", EditorStyles.boldLabel);
        m_MaterialEditor.ShaderProperty(FindProp("_ToonBlend", props), "Toon Blend (0 = PBR, 1 = Toon)");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dissolve", EditorStyles.boldLabel);
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Dissolve Map"), FindProp("_DissolveMap", props));
        m_MaterialEditor.ShaderProperty(FindProp("_DissolveThreshold", props), "Dissolve Threshold");
        m_MaterialEditor.ShaderProperty(FindProp("_DissolveSoftness", props), "Dissolve Softness");
        m_MaterialEditor.ShaderProperty(FindProp("_DissolveEdgeColor", props), "Edge Color");
        m_MaterialEditor.ShaderProperty(FindProp("_DissolveEdgeWidth", props), "Edge Width");
        m_MaterialEditor.ShaderProperty(FindProp("_DissolvePulseSpeed", props), "Pulse Speed");

        EditorGUILayout.Space();
        m_MaterialEditor.ShaderProperty(FindProp("_Cutoff", props), "Alpha Cutoff");
        m_MaterialEditor.ShaderProperty(FindProp("_EmissionColor", props), "Emission Color");

        EditorGUILayout.Space();

        // Help
        EditorGUILayout.HelpBox("This shader is a URP-compatible variant of the Unity Toon Shader with added Metallic/Roughness and Dissolve features. All original UTS features are preserved by using the existing UTS includes where possible.", MessageType.Info);

        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(targetMat);
        }
    }

    private MaterialProperty FindProp(string name, MaterialProperty[] props)
    {
        return FindProperty(name, props, false);
    }
}
