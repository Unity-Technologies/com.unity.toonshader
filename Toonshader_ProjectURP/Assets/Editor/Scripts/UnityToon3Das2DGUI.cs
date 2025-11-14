using UnityEditor;
using UnityEngine;

class UnityToon3Das2DGUI : UnityEditor.ShaderGUI {

    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
        
        Material material = materialEditor.target as Material;
        if (material == null)
            return;

        FindProperties(props);
        EditorGUI.BeginChangeCheck();
        GUI_BasicThreeColors(materialEditor, material);

        if (EditorGUI.EndChangeCheck()) {
            materialEditor.PropertiesChanged();
        }
    }
    
//----------------------------------------------------------------------------------------------------------------------    
    
    void GUI_BasicThreeColors(MaterialEditor materialEditor, Material material) {
        materialEditor.TexturePropertySingleLine(Styles.baseColorText, mainTex, baseColor);
        
        if (material.HasProperty("_Color")) {
            material.SetColor("_Color", material.GetColor("_BaseColor"));
        }

        EditorGUI.indentLevel += 2;
        bool applyTo1st = GUI_Toggle(materialEditor, material, Styles.applyTo1stShademapText, ShaderPropUse_BaseAs1st,
            MaterialGetInt(material, ShaderPropUse_BaseAs1st) != 0);
        EditorGUI.indentLevel -= 2;


        if (applyTo1st) {
            EditorGUI.indentLevel += 2;
            materialEditor.ColorProperty(firstShadeColor, Styles.firstShadeColorText.text);
            EditorGUI.indentLevel -= 2;
        }
        else {
            materialEditor.TexturePropertySingleLine(Styles.firstShadeColorText, firstShadeMap, firstShadeColor);
        }

        EditorGUI.indentLevel += 2;
        bool applyTo2nd = GUI_Toggle(materialEditor, material, Styles.applyTo2ndShademapText, ShaderPropUse_1stAs2nd, MaterialGetInt(material, ShaderPropUse_1stAs2nd) != 0);
        EditorGUI.indentLevel -= 2;


        if (applyTo2nd) {
            EditorGUI.indentLevel += 2;
            materialEditor.ColorProperty(secondShadeColor, Styles.secondShadeColorText.text);
            EditorGUI.indentLevel -= 2;
        }
        else {
            materialEditor.TexturePropertySingleLine(Styles.secondShadeColorText, secondShadeMap, secondShadeColor);
        }
    }

    void FindProperties(MaterialProperty[] props) {
        
        mainTex = FindProperty(ShaderPropMainTex, props);
        baseColor = FindProperty("_BaseColor", props);
        firstShadeMap = FindProperty("_1st_ShadeMap", props);
        firstShadeColor = FindProperty("_1st_ShadeColor", props);
        secondShadeMap = FindProperty("_2nd_ShadeMap", props);
        secondShadeColor = FindProperty("_2nd_ShadeColor", props);
        
        
    }


//----------------------------------------------------------------------------------------------------------------------
    bool GUI_Toggle(MaterialEditor materialEditor, Material material, GUIContent guiContent, string prop, bool value) {
        EditorGUI.BeginChangeCheck();
        bool ret = EditorGUILayout.Toggle(guiContent, value);
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(guiContent.text);
            MaterialSetInt(material, prop, ret ? 1 : 0);
        }

        return ret;
    }
    
    internal static int MaterialGetInt(Material material, string prop) {
        return (int)material.GetFloat(prop);
    }
    
    internal static void MaterialSetInt(Material material, string prop, int value) {
        material.SetFloat(prop, value);
    }
    
    
//----------------------------------------------------------------------------------------------------------------------    


    //Common constants
    private static class Styles {
        public static readonly GUIContent baseColorText = new GUIContent("Base Map", "Base Color : Texture(sRGB) × Color(RGB) Default:White");
        public static readonly GUIContent firstShadeColorText = new GUIContent("1st Shading Map", "The map used for the brighter portions of the shadow.");
        public static readonly GUIContent secondShadeColorText = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow.");
        public static readonly GUIContent applyTo1stShademapText = new GUIContent("Apply to 1st shading map", "Apply Base map to the 1st shading map.");

        public static readonly GUIContent applyTo2ndShademapText =
            new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map.");
    }

    internal const string ShaderPropMainTex = "_MainTex";
    internal const string ShaderPropUse_BaseAs1st = "_Use_BaseAs1st";
    internal const string ShaderPropUse_1stAs2nd = "_Use_1stAs2nd";

    //Common properties
    protected MaterialProperty mainTex = null;
    protected MaterialProperty baseColor = null;
    protected MaterialProperty firstShadeMap = null;
    protected MaterialProperty firstShadeColor = null;
    protected MaterialProperty secondShadeMap = null;
    protected MaterialProperty secondShadeColor = null;

    //materialEditor.TexturePropertySingleLine(Styles.secondShadeColorText, secondShadeMap, secondShadeColor);

    
}

