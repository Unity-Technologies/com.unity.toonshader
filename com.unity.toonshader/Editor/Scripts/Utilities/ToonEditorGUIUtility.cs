
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal static class ToonEditorGUIUtility {

    internal static void DrawTexturePropertySingleLineGUI(MaterialEditor mEditor, MaterialPropertyUIElement element) {
        if (null != element.extraProperty2)
            mEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop, element.extraProperty1.prop, element.extraProperty2.prop);
        else if (null != element.extraProperty1)
            mEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop, element.extraProperty1.prop);
        else
            mEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop);
    }

    internal static void DrawColorPropertyGUI(MaterialEditor mEditor, MaterialPropertyUIElement element) {
        mEditor.ColorProperty(element.mainProperty.prop, element.label.text);
    }

    internal static void DrawRangePropertyGUI(MaterialEditor mEditor, MaterialPropertyUIElement element) {
        mEditor.RangeProperty(element.mainProperty.prop, element.label.text);
    }


    internal static bool DrawToggleGUI(MaterialEditor mEditor, Material[] mats,
        MaterialPropertyUIElement element) {
        return DrawToggleGUI(mEditor, mats, element, out bool _);
    }

    internal static bool DrawToggleGUI(MaterialEditor mEditor, Material[] mats,
        MaterialPropertyUIElement element, out bool newValue) {
        bool prevValue = mats[0].GetInteger(element.mainProperty.id) != 0;
        EditorGUI.BeginChangeCheck();
        newValue = EditorGUILayout.Toggle(element.label, prevValue);
        if (!EditorGUI.EndChangeCheck())
            return false;

        mEditor.RegisterPropertyChangeUndo(element.label.text);
        foreach (Material m in mats)
            m.SetInteger(element.mainProperty.id, newValue ? 1 : 0);
        return true;
    }

    internal static void DrawFloatFieldGUI(MaterialEditor mEditor, MaterialPropertyUIElement element) {
        mEditor.FloatProperty(element.mainProperty.prop, element.label.text);
    }

    internal static void DrawColorFieldGUI(MaterialEditor mEditor, MaterialPropertyUIElement element) {
        mEditor.ColorProperty(element.mainProperty.prop, element.label.text);
    }

    internal static bool DrawVector3FieldGUI(MaterialEditor mEditor, Material[] mats,
        MaterialPropertyUIElement element) {
        return DrawVector3FieldGUI(mEditor, mats, element, out Vector3 _);
    }

    //return true if changed, false otherwise
    internal static bool DrawVector3FieldGUI(MaterialEditor mEditor, Material[] mats,
        MaterialPropertyUIElement element, out Vector3 newValue) {
        Vector3 prevValue = mats[0].GetVector(element.mainProperty.id);
        EditorGUI.BeginChangeCheck();
        newValue = EditorGUILayout.Vector3Field(element.label, prevValue);

        if (!EditorGUI.EndChangeCheck())
            return false;

        mEditor.RegisterPropertyChangeUndo(element.label.text);
        foreach (Material m in mats)
            m.SetVector(element.mainProperty.id, newValue);
        return true;

    }

    //return true if changed, false otherwise
    internal static bool DrawIntPopupGUI(MaterialEditor mEditor, Material[] mats,
        MaterialPropertyUIElement element, GUIContent[] displayedOptions, int[] optionValues, out int newValue) {
        int prevValue = mats[0].GetInteger(element.mainProperty.id);

        EditorGUI.BeginChangeCheck();
        newValue = EditorGUILayout.IntPopup(element.label, prevValue, displayedOptions, optionValues);

        if (!EditorGUI.EndChangeCheck())
            return false;

        mEditor.RegisterPropertyChangeUndo(element.label.text);
        foreach (Material m in mats)
            m.SetInteger(element.mainProperty.id, newValue);
        return true;

    }


    //return true if changed, false otherwise
    internal static bool DrawFoldoutGUI(ref bool state, GUIContent label) {

        Rect lineRect = EditorGUILayout.GetControlRect(false, 16);

        DrawBGRect(lineRect);

        EditorGUI.BeginChangeCheck();
        state = EditorGUI.Foldout(lineRect, state, label);
        return EditorGUI.EndChangeCheck();
    }

    //return true if changed, false otherwise
    internal static bool DrawFoldoutWithToggleGUI(MaterialEditor mEditor,
        ref bool foldoutState, ref bool toggleEnabled, string label) {
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

        if (!EditorGUI.EndChangeCheck())
            return false;

        mEditor.RegisterPropertyChangeUndo(label);
        return true;

    }

    //return true if changed, false otherwise
    internal static bool DrawFoldoutWithToggleGUI(MaterialEditor mEditor, Material[] mats,
        MaterialPropertyUIElement element, ref bool foldoutState, out bool toggleEnabled) {
        toggleEnabled = mats[0].GetInteger(element.mainProperty.id) != 0;
        bool ret = DrawFoldoutWithToggleGUI(mEditor, ref foldoutState, ref toggleEnabled, element.label.text);
        if (!ret)
            return false;
        foreach (Material m in mats)
            m.SetInteger(element.mainProperty.id, toggleEnabled ? 1 : 0);

        return true;
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

} //end namespace