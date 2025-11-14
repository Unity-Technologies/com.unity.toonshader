using UnityEditor;
using UnityEngine;

internal class TwoPassesShaderGUI : UnityEditor.ShaderGUI {
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {

        Material material = materialEditor.target as Material;
        if (material == null)
            return;

        // Draw default properties
        base.OnGUI(materialEditor, props);

        //Doc: Use this LightMode tag value to draw an extra Pass when rendering objects.
        string lightModeName = "SRPDefaultUnlit";
        bool enabled = material.GetShaderPassEnabled(lightModeName);
        EditorGUI.BeginChangeCheck();
        bool newEnabled = EditorGUILayout.Toggle("Enable Second Pass", enabled);
        if (EditorGUI.EndChangeCheck())
        {
            material.SetShaderPassEnabled(lightModeName, newEnabled);
            EditorUtility.SetDirty(material);
        }
    }


}

