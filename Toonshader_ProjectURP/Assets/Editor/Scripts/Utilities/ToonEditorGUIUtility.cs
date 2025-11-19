
using UnityEditor;
using UnityEngine;

internal static class ToonEditorGUIUtility {
    
    internal static void DrawTexturePropertySingleLineGUI(MaterialEditor materialEditor, MaterialPropertyUIElement element) {
        if (null!= element.extraProperty2)
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop,element.extraProperty1.prop, element.extraProperty2.prop);
        else if (null!= element.extraProperty1)
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop,element.extraProperty1.prop);
        else
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop);
    }

    internal static void DrawColorPropertyGUI(MaterialEditor materialEditor, MaterialPropertyUIElement element) {
        materialEditor.ColorProperty(element.mainProperty.prop, element.label.text);
    }
    
    internal static bool DrawToggleGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element) {
        EditorGUI.BeginChangeCheck();
        bool ret = EditorGUILayout.Toggle(element.label, material.GetInteger(element.mainProperty.id) !=0);
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetInteger(element.mainProperty.id, ret ? 1 : 0);
        }

        return ret;
    }

    internal static float DrawFloatFieldGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element) {
        
        float ret = material.GetFloat(element.mainProperty.id);
        EditorGUI.BeginChangeCheck();
        ret = EditorGUILayout.FloatField(element.label, ret);
        
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetFloat(element.mainProperty.id, ret);
        }
        return ret;
    }

    internal static Color DrawColorFieldGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element) {
        
        Color ret = material.GetColor(element.mainProperty.id);
        EditorGUI.BeginChangeCheck();
        ret = EditorGUILayout.ColorField(element.label, ret);
        
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetColor(element.mainProperty.id, ret);
        }
        return ret;
    }

    internal static Vector3 DrawVector3FieldGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element) {
        
        Vector3 ret = material.GetVector(element.mainProperty.id);
        EditorGUI.BeginChangeCheck();
        ret = EditorGUILayout.Vector3Field(element.label, ret);
        
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetVector(element.mainProperty.id, ret);
        }
        return ret;
    }
    
    //Return the index
    internal static int DrawIntPopupGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element,
        GUIContent[] displayedOptions, int[] optionValues)
    {
        int propValue = material.GetInteger(element.mainProperty.id);
        
        EditorGUI.BeginChangeCheck();
        int ret = EditorGUILayout.IntPopup(element.label, propValue, displayedOptions, optionValues);
        
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetInteger(element.mainProperty.id, ret);
        }

        return ret;
    }


    //return true if changed, false otherwise
    internal static bool DrawFoldoutGUI(ref bool state, GUIContent label) {
        
        Rect lineRect = EditorGUILayout.GetControlRect(false, 16);

        DrawBGRect(lineRect);
        
        EditorGUI.BeginChangeCheck();
        state = EditorGUI.Foldout(lineRect, state, label);
        if (EditorGUI.EndChangeCheck()) {
            return true;
        }

        return false;

    }
    
    //return true if changed, false otherwise
    internal static bool DrawFoldoutWithToggleGUI(MaterialEditor materialEditor,  
        ref bool foldoutState, ref bool toggleEnabled, string label) 
    {
        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        Rect lineRect = EditorGUILayout.GetControlRect(false, 16);
        Rect foldoutRect = new Rect(lineRect.x, lineRect.y, 16, lineRect.height);
        Rect toggleRect = new Rect(foldoutRect.xMax, lineRect.y, 16, lineRect.height);
        Rect labelRect = new Rect(toggleRect.xMax + 2, lineRect.y, lineRect.width - 34, lineRect.height);

        DrawBGRect(lineRect);
        
        EditorGUI.BeginChangeCheck();
        foldoutState = EditorGUI.Foldout(foldoutRect, foldoutState, GUIContent.none, true, foldoutStyle);
        toggleEnabled = EditorGUI.Toggle(toggleRect, toggleEnabled);
        EditorGUI.LabelField(labelRect, label);
        
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(label);
            return true;
        }

        return false;
    }
    
    internal static bool DrawFoldoutWithToggleGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element,
        ref bool foldoutState)
    {
        bool enabled = material.GetInteger(element.mainProperty.id) !=0;
        bool ret = DrawFoldoutWithToggleGUI(materialEditor, ref foldoutState, ref enabled, element.label.text);
        if (ret) {
            material.SetInteger(element.mainProperty.id, enabled ? 1 : 0);
        } 

        return ret;

    }
    
    
    static void DrawBGRect(Rect lineRect) {
        
        float initialPadding = lineRect.x;
        Rect bgRect = new Rect(0, lineRect.y, lineRect.width + initialPadding, lineRect.height);

        Color bgColor = GetBGColor();
        EditorGUI.DrawRect(bgRect, bgColor); 

        // Draw top border
        Rect topBorderRect = new Rect(bgRect.x, bgRect.y, bgRect.width, 1);
        EditorGUI.DrawRect(topBorderRect, new Color(0.12f, 0.12f, 0.12f, 1f));
    }

    static Color GetBGColor() {
        return !EditorGUIUtility.isProSkin
            ? new Color(0.6f, 0.6f, 0.6f, 1.0f)
            : new Color(0.20f, 0.20f, 0.20f, 1.0f);
    }
    
    
}
