using UnityEditor;
using UnityEngine;

internal class TwoPassesGUI : UnityEditor.ShaderGUI {

    void OnOpenGUI(Material material, MaterialEditor materialEditor, MaterialProperty[] props) {
        
        Debug.Log("OnOpenGUI())");
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
        
        Material material = materialEditor.target as Material;
        if (material == null)
            return;

        // Draw default properties
        base.OnGUI(materialEditor, props);

        // Checkbox for SecondPass
        string lightModeName = "SRPDefaultUnlit";
        bool enabled = material.GetShaderPassEnabled(lightModeName);
        EditorGUI.BeginChangeCheck();
        bool newEnabled = EditorGUILayout.Toggle("Enable Second Pass", enabled);
        if (EditorGUI.EndChangeCheck())
        {
            material.SetShaderPassEnabled(lightModeName, newEnabled);
            
            EditorUtility.SetDirty(material);
            Debug.Log(material.GetShaderPassEnabled(lightModeName));
        }        
    }


}

