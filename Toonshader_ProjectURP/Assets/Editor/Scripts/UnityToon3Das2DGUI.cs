using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class UnityToon3Das2DGUI : UnityEditor.ShaderGUI {

    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
        
        Material material = materialEditor.target as Material;
        if (material == null)
            return;
        
        InitMaterialPropertyUIElements(props);
        
        EditorGUI.BeginChangeCheck();
        GUI_BasicThreeColors(materialEditor, material, m_materialPropertyUIElements);
        
        DrawOutlineGUI(materialEditor, material, m_materialPropertyUIElements);
        

        if (EditorGUI.EndChangeCheck()) {
            materialEditor.PropertiesChanged();
        }
    }

    
    void DrawOutlineGUI(MaterialEditor materialEditor, Material material, 
        Dictionary<string, MaterialPropertyUIElement> uiElements) 
    {
        //Doc: Use this LightMode tag value to draw an extra Pass when rendering objects.
        const string LIGHT_MODE_NAME_FOR_OUTLINE = "SRPDefaultUnlit";
        bool isOutlineEnabled = material.GetShaderPassEnabled(LIGHT_MODE_NAME_FOR_OUTLINE);

        EditorGUI.BeginChangeCheck();
        
        //Draw custom foldout with toggle

        if (DrawFoldoutWithToggleGUI(materialEditor, material, ref m_outlineFoldout, ref isOutlineEnabled, "Outline")) 
        {
            material.SetShaderPassEnabled(LIGHT_MODE_NAME_FOR_OUTLINE, isOutlineEnabled);
        }
        
        
        if (!m_outlineFoldout)
            return;

        //Outline Settings
        EditorGUI.indentLevel++;
        EditorGUI.BeginDisabledGroup(!isOutlineEnabled);

        int outlineMode = DrawIntPopupGUI(materialEditor, material, uiElements[ShaderProp_OutlineMode], 
            m_outlineModeEnums, m_outlineModeIndices);
        
        const string OUTLINE_NORMAL_KEYWORD = "_OUTLINE_NML";;
        const string OUTLINE_POSITION_KEYWORD = "_OUTLINE_POS";
        
        switch (outlineMode) {
            case (int) OutlineMode.NormalDirection:
                material.EnableKeyword(OUTLINE_NORMAL_KEYWORD);
                material.DisableKeyword(OUTLINE_POSITION_KEYWORD);
                break;
            case (int) OutlineMode.PositionScaling:
                material.EnableKeyword(OUTLINE_POSITION_KEYWORD);
                material.DisableKeyword(OUTLINE_NORMAL_KEYWORD);
                break;
        }


        EditorGUI.BeginDisabledGroup(outlineMode != (int) OutlineMode.NormalDirection);
        {
            bool useCustom = DrawToggleGUI(materialEditor, material, uiElements[ShaderProp_Outline_UseCustomNormalMap]);
            EditorGUI.BeginDisabledGroup(!useCustom);
            DrawTexturePropertySingleLineGUI(materialEditor,uiElements[ShaderProp_Outline_CustomNormalMap]);
            EditorGUI.EndDisabledGroup();
        }
        EditorGUI.EndDisabledGroup();
        

        DrawFloatFieldGUI(materialEditor, material, uiElements[ShaderProp_OutlineWidth]);
        
        DrawTexturePropertySingleLineGUI(materialEditor, uiElements[ShaderProp_OutlineTex]);
        DrawToggleGUI(materialEditor, material, uiElements[ShaderProp_Outline_BlendBaseColor]);
        
        DrawTexturePropertySingleLineGUI(materialEditor, uiElements[ShaderProp_OutlineWidthMap]);
        
        DrawFloatFieldGUI(materialEditor, material, uiElements[ShaderProp_OutlineOffsetZ]);


        EditorGUILayout.Space();
        {
            EditorGUILayout.LabelField("Camera Distance for Outline Width");
            EditorGUI.indentLevel++;
            DrawFloatFieldGUI(materialEditor, material, uiElements[ShaderProp_OutlineNear]);
            DrawFloatFieldGUI(materialEditor, material, uiElements[ShaderProp_OutlineFar]);
            EditorGUI.indentLevel--;

            
        }
        EditorGUI.EndDisabledGroup(); //!isOutlineEnabled
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
    }
    
    
//----------------------------------------------------------------------------------------------------------------------    
    
    static void GUI_BasicThreeColors(MaterialEditor materialEditor, Material material, 
        Dictionary<string, MaterialPropertyUIElement> uiElements) 
    {
        DrawTexturePropertySingleLineGUI(materialEditor, uiElements[ShaderPropMainTex]);

        EditorGUI.indentLevel += 2;
        bool applyTo1st = DrawToggleGUI(materialEditor, material, uiElements[ShaderPropUse_BaseAs1st]);        
        EditorGUI.indentLevel -= 2;

        if (applyTo1st) {
            EditorGUI.indentLevel += 2;
            DrawColorPropertyGUI(materialEditor, uiElements[ShaderProp_1st_ShadeColor]);
            EditorGUI.indentLevel -= 2;
        }
        else {
            DrawTexturePropertySingleLineGUI(materialEditor, uiElements[ShaderProp_1st_ShadeMap]);
        }

        EditorGUI.indentLevel += 2;
        bool applyTo2nd = DrawToggleGUI(materialEditor, material, uiElements[ShaderPropUse_1stAs2nd]);        
        EditorGUI.indentLevel -= 2;


        if (applyTo2nd) {
            EditorGUI.indentLevel += 2;
            DrawColorPropertyGUI(materialEditor, uiElements[ShaderProp_2nd_ShadeColor]);
            EditorGUI.indentLevel -= 2;
        }
        else {
            DrawTexturePropertySingleLineGUI(materialEditor, uiElements[ShaderProp_2nd_ShadeMap]);
        }
    }
    
//----------------------------------------------------------------------------------------------------------------------
    static void DrawTexturePropertySingleLineGUI(MaterialEditor materialEditor, MaterialPropertyUIElement element) {
        if (null!= element.extraProperty2)
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop,element.extraProperty1.prop, element.extraProperty2.prop);
        else if (null!= element.extraProperty1)
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop,element.extraProperty1.prop);
        else
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop);
    }

    static void DrawColorPropertyGUI(MaterialEditor materialEditor, MaterialPropertyUIElement element) {
        materialEditor.ColorProperty(element.mainProperty.prop, element.label.text);
    }
    
    static bool DrawToggleGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element) {
        EditorGUI.BeginChangeCheck();
        bool ret = EditorGUILayout.Toggle(element.label, material.GetInteger(element.mainProperty.id) !=0);
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetInteger(element.mainProperty.id, ret ? 1 : 0);
        }

        return ret;
    }

    static float DrawFloatFieldGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element) {
        
        float ret = material.GetFloat(element.mainProperty.id);
        EditorGUI.BeginChangeCheck();
        ret = EditorGUILayout.FloatField(element.label, ret);
        
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetFloat(element.mainProperty.id, ret);
        }
        return ret;
    }

    static Color DrawColorFieldGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element) {
        
        Color ret = material.GetColor(element.mainProperty.id);
        EditorGUI.BeginChangeCheck();
        ret = EditorGUILayout.ColorField(element.label, ret);
        
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetColor(element.mainProperty.id, ret);
        }
        return ret;
    }

    //Return the index
    static int DrawIntPopupGUI(MaterialEditor materialEditor, Material material, MaterialPropertyUIElement element,
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
    static bool DrawFoldoutWithToggleGUI(MaterialEditor materialEditor, Material material, 
        ref bool foldoutState, ref bool toggleEnabled, string label) 
    {
        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        Rect lineRect = EditorGUILayout.GetControlRect(false, 16);
        Rect foldoutRect = new Rect(lineRect.x, lineRect.y, 16, lineRect.height);
        Rect toggleRect = new Rect(foldoutRect.xMax, lineRect.y, 16, lineRect.height);
        Rect labelRect = new Rect(toggleRect.xMax + 2, lineRect.y, lineRect.width - 34, lineRect.height);

        const float BG_COLOR = 0.20f;
        EditorGUI.DrawRect(lineRect, new Color(BG_COLOR, BG_COLOR, BG_COLOR, 1f)); //BG

        // Draw top border
        Rect topBorderRect = new Rect(lineRect.x, lineRect.y, lineRect.width, 1);
        EditorGUI.DrawRect(topBorderRect, new Color(0.12f, 0.12f, 0.12f, 1f));
        
        
        foldoutState = EditorGUI.Foldout(foldoutRect, foldoutState, GUIContent.none, true, foldoutStyle);
        toggleEnabled = EditorGUI.Toggle(toggleRect, toggleEnabled);
        EditorGUI.LabelField(labelRect, label);
        
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(label);
            return true;
        }

        return false;
    }
    
//----------------------------------------------------------------------------------------------------------------------

    void InitMaterialPropertyUIElements(MaterialProperty[] allProps) {
        int numProperties = m_materialUIElements.Count;
        for (int i = 0; i < numProperties; ++i) {
            MaterialUIElement propInfo = m_materialUIElements[i];
            
            MaterialPropertyInfo mainProp = MaterialNameToPropertyInfo(propInfo.mainPropertyName, allProps);
            MaterialPropertyInfo extraProperty1 = null!= propInfo.extraPropertyName1 ? 
                MaterialNameToPropertyInfo(propInfo.extraPropertyName1, allProps) : null;

            MaterialPropertyInfo extraProperty2 = null!= propInfo.extraPropertyName2 ? 
                MaterialNameToPropertyInfo(propInfo.extraPropertyName2, allProps) : null;
            

            MaterialPropertyUIElement newElement10 = new MaterialPropertyUIElement {
                label = propInfo.label,
                mainProperty = mainProp,
                extraProperty1 = extraProperty1,
                extraProperty2 = extraProperty2,
            };

            m_materialPropertyUIElements[propInfo.mainPropertyName.name] = newElement10;
        }
        
    }
    
//----------------------------------------------------------------------------------------------------------------------

    MaterialPropertyInfo MaterialNameToPropertyInfo(MaterialName m, MaterialProperty[] allProps) {
        MaterialPropertyInfo info = new MaterialPropertyInfo();
        info.prop = FindProperty(m.name, allProps); 
        info.id = m.nameID;
        return info;
    }

//----------------------------------------------------------------------------------------------------------------------

    private readonly Dictionary<string, MaterialPropertyUIElement> m_materialPropertyUIElements = new Dictionary<string, MaterialPropertyUIElement>();

    private static readonly List<MaterialUIElement> m_materialUIElements = new List<MaterialUIElement>() {
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropMainTex),
            label = new GUIContent("Base Map", "Base Color : Texture(sRGB) × Color(RGB)."),
            extraPropertyName1 = new MaterialName(ShaderProp_BaseColor), 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_1st_ShadeMap),
            label = new GUIContent("1st Shading Map", "The map used for the brighter portions of the shadow."),
            extraPropertyName1 = new MaterialName(ShaderProp_1st_ShadeColor), 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_2nd_ShadeMap),
            label = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow."),
            extraPropertyName1 = new MaterialName(ShaderProp_2nd_ShadeColor) 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUse_BaseAs1st),
            label = new GUIContent("Apply to 1st shading map", "Apply Base map to the 1st shading map."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUse_1stAs2nd),
            label = new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map."),
        },
        
        //Outline Start
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_OutlineWidth),
            label = new GUIContent("Outline Width",
                "The width of the outline."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_OutlineWidthMap),
            label = new GUIContent("Outline Width Map",
                "Outline Width Map (grayscale, linear): White = full width, Black = 0 width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_OutlineTex),
            label = new GUIContent("Outline Color", "The color of outline."),
            extraPropertyName1 = new MaterialName(ShaderProp_OutlineColor), 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_Outline_BlendBaseColor),
            label = new GUIContent("Blend Base Color to Outline",
                "Blend base color to outline color."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_OutlineOffsetZ),
            label = new GUIContent("Z Offset",
                "Offsets the outline in the depth (Z) direction of the camera."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_OutlineNear),
            label = new GUIContent("Near",
                "Nearest distance for maximum outline width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_OutlineFar),
            label = new GUIContent("Far",
                "Furthest distance where outline fades to zero width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_OutlineMode),
            label = new GUIContent("Outline Mode",
                "Specifies how the outline is generated."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_Outline_UseCustomNormalMap),
            label = new GUIContent("Use Custom Normal Map",
                "Use a custom normal map for outline."),
        },
        
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_Outline_CustomNormalMap),
            label = new GUIContent("Custom Normal Map",
                "Custom normal map (linear) for outline. "),
        },
        //Outline End
        
    };
   
    //Common constants
    internal const string ShaderPropMainTex = "_MainTex";
    internal const string ShaderPropUse_BaseAs1st = "_Use_BaseAs1st";
    internal const string ShaderPropUse_1stAs2nd = "_Use_1stAs2nd";
    internal const string ShaderProp_BaseColor = "_BaseColor";
    internal const string ShaderProp_1st_ShadeMap = "_1st_ShadeMap";
    internal const string ShaderProp_1st_ShadeColor = "_1st_ShadeColor";
    internal const string ShaderProp_2nd_ShadeMap = "_2nd_ShadeMap";
    internal const string ShaderProp_2nd_ShadeColor = "_2nd_ShadeColor";
    
    internal const string ShaderProp_OutlineMode = "_OutlineMode";
    internal const string ShaderProp_OutlineWidth = "_OutlineWidth";
    internal const string ShaderProp_OutlineWidthMap = "_OutlineWidthMap";
    internal const string ShaderProp_OutlineTex = "_OutlineTex";
    internal const string ShaderProp_OutlineColor = "_OutlineColor";
    internal const string ShaderProp_Outline_BlendBaseColor = "_Outline_BlendBaseColor";
    internal const string ShaderProp_OutlineOffsetZ = "_OutlineOffsetZ";
    internal const string ShaderProp_OutlineNear = "_OutlineNear";
    internal const string ShaderProp_OutlineFar = "_OutlineFar";

    internal const string ShaderProp_Outline_UseCustomNormalMap = "_Outline_UseCustomNormalMap";
    internal const string ShaderProp_Outline_CustomNormalMap    = "_Outline_CustomNormalMap";


    internal enum OutlineMode {
        NormalDirection,
        PositionScaling
    }
    
    private static readonly GUIContent[] m_outlineModeEnums= EnumUtility.ToInspectorNamesAsGUIContent(typeof(OutlineMode));
    private static readonly int[] m_outlineModeIndices = EnumUtility.ToIndices(typeof(OutlineMode));

    bool m_outlineFoldout = false;
    
}

