using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

class UnityToon3Das2DGUI : UnityEditor.ShaderGUI {
    private Material m_lastMaterial;
    
    public override void OnGUI(MaterialEditor mEditor, MaterialProperty[] props) {

        if (null == mEditor.targets || mEditor.targets.Length == 0) {
            return;
        }
        
        int numTargets = mEditor.targets.Length;
        Material[] mats = new Material[numTargets];
        for (int i = 0; i < numTargets; ++i) {
            mats[i] = mEditor.targets[i] as Material;
        }

        if (mats[0] != m_lastMaterial) {
            RefreshFoldouts(mats[0]);
        }

        InitMaterialPropertyUIElements(props);
        
        EditorGUI.BeginChangeCheck();
        DrawThreeColorsGUI(mEditor, mats, m_materialPropertyUIElements);

        DrawNormalMapGUI(mEditor, m_materialPropertyUIElements, ref m_normalMapFoldout);
        DrawOutlineGUI(mEditor, mats, m_materialPropertyUIElements, ref m_outlineFoldout);
        DrawDirectionalLightGUI(mEditor, mats, m_materialPropertyUIElements, ref m_specularFoldout);
        

        if (EditorGUI.EndChangeCheck()) {
            mEditor.PropertiesChanged();
        }

        m_lastMaterial = mats[0];
    }
    
    void RefreshFoldouts(Material mat) {
        
        m_normalMapFoldout = true;
        m_outlineFoldout = mat.GetShaderPassEnabled(LIGHT_MODE_NAME_FOR_OUTLINE);
        m_specularFoldout = false;
    }
    
//----------------------------------------------------------------------------------------------------------------------
    static void DrawNormalMapGUI(MaterialEditor mEditor, Dictionary<string, MaterialPropertyUIElement> uiElements, 
        ref bool foldout) {

        ToonEditorGUIUtility.DrawFoldoutGUI(ref foldout, uiElements[ShaderProp_NormalMap].label);
        if (!foldout) 
            return;
        
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ShaderProp_NormalMap]);
        mEditor.TextureScaleOffsetProperty(uiElements[ShaderProp_NormalMap].mainProperty.prop);
        
        EditorGUILayout.Space();
    }
    
    
    static void DrawOutlineGUI(MaterialEditor mEditor, Material[] mats, Dictionary<string, 
        MaterialPropertyUIElement> uiElements, ref bool foldout) 
    {
        bool isOutlineEnabled = mats[0].GetShaderPassEnabled(LIGHT_MODE_NAME_FOR_OUTLINE);


        if (ToonEditorGUIUtility.DrawFoldoutWithToggleGUI(mEditor, ref foldout, ref isOutlineEnabled, "Outline")) 
        {
            foreach (Material m in mats)
                m.SetShaderPassEnabled(LIGHT_MODE_NAME_FOR_OUTLINE, isOutlineEnabled);
        }
        
        
        if (!foldout)
            return;

        //Outline Settings
        EditorGUI.indentLevel++;
        EditorGUI.BeginDisabledGroup(!isOutlineEnabled);

        ToonEditorGUIUtility.DrawIntPopupGUI(mEditor, mats, uiElements[ShaderProp_OutlineMode], 
            m_outlineModeEnums, m_outlineModeIndices, out int outlineMode);
        
        const string OUTLINE_NORMAL_KEYWORD = "_OUTLINE_NML";;
        const string OUTLINE_POSITION_KEYWORD = "_OUTLINE_POS";
        
        switch (outlineMode) {
            case (int) OutlineMode.NormalDirection:
                foreach (Material m in mats) {
                    m.EnableKeyword(OUTLINE_NORMAL_KEYWORD);
                    m.DisableKeyword(OUTLINE_POSITION_KEYWORD);
                }
                break;
            case (int) OutlineMode.PositionScaling:
                foreach (Material m in mats) {
                    m.DisableKeyword(OUTLINE_NORMAL_KEYWORD);
                    m.EnableKeyword(OUTLINE_POSITION_KEYWORD);
                }
                break;
        }


        EditorGUI.BeginDisabledGroup(outlineMode != (int) OutlineMode.NormalDirection);
        {
            ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[ShaderProp_Outline_UseCustomNormalMap], 
                out bool useCustom);
            EditorGUI.BeginDisabledGroup(!useCustom);
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor,uiElements[ShaderProp_Outline_CustomNormalMap]);
            EditorGUI.EndDisabledGroup();
        }
        EditorGUI.EndDisabledGroup();
        

        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ShaderProp_OutlineWidth]);
        
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ShaderProp_OutlineTex]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ShaderProp_Outline_BaseColorBlend]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ShaderProp_Outline_LightColorBlend]);
        
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ShaderProp_OutlineWidthMap]);
        
        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ShaderProp_OutlineOffsetZ]);


        EditorGUILayout.Space();
        {
            EditorGUILayout.LabelField("Camera Distance for Outline Width");
            EditorGUI.indentLevel++;
            ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ShaderProp_OutlineNear]);
            ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ShaderProp_OutlineFar]);
            EditorGUI.indentLevel--;

            
        }
        EditorGUI.EndDisabledGroup(); //!isOutlineEnabled
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
    }
    
    
//----------------------------------------------------------------------------------------------------------------------    
    
    static void DrawThreeColorsGUI(MaterialEditor mEditor, Material[] mats,
        Dictionary<string, MaterialPropertyUIElement> uiElements) 
    {
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ShaderPropMainTex]);

        EditorGUI.indentLevel += 2;
        ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[ShaderPropUse_BaseAs1st], out bool applyTo1st );
        EditorGUI.indentLevel -= 2;

        if (applyTo1st) {
            EditorGUI.indentLevel += 2;
            ToonEditorGUIUtility.DrawColorPropertyGUI(mEditor, uiElements[ShaderProp_1st_ShadeColor]);
            EditorGUI.indentLevel -= 2;
        } else {
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ShaderProp_1st_ShadeMap]);
        }

        EditorGUI.indentLevel += 2;
        ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[ShaderPropUse_1stAs2nd], out bool applyTo2nd);
        EditorGUI.indentLevel -= 2;


        if (applyTo2nd) {
            EditorGUI.indentLevel += 2;
            ToonEditorGUIUtility.DrawColorPropertyGUI(mEditor, uiElements[ShaderProp_2nd_ShadeColor]);
            EditorGUI.indentLevel -= 2;
        } else {
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ShaderProp_2nd_ShadeMap]);
        }
    }
    
    static void DrawDirectionalLightGUI(MaterialEditor mEditor, Material[] mats, 
        Dictionary<string, MaterialPropertyUIElement> uiElements, ref bool foldout) {

        ToonEditorGUIUtility.DrawFoldoutWithToggleGUI(mEditor, mats, uiElements[ShaderPropUnlit_DirectionalLight_Use],
            ref foldout);

        
        if (!foldout)
            return;

        ToonEditorGUIUtility.DrawVector3FieldGUI(mEditor, mats, uiElements[ShaderPropUnlit_DirectionalLight_Direction]);
        ToonEditorGUIUtility.DrawColorFieldGUI(mEditor, uiElements[ShaderPropUnlit_DirectionalLight_Color]);
        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ShaderPropUnlit_DirectionalLight_Intensity]);
        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ShaderPropUnlit_DirectionalLight_2DLightFactor]);
        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ShaderPropUnlit_DirectionalLight_DiffuseFactor]);
        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ShaderPropUnlit_DirectionalLight_SpecularFactor]);
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
            mainPropertyName = new MaterialName(ShaderProp_1st_ShadeColor),
            label = new GUIContent("1st Shading Map", "The map used for the brighter portions of the shadow."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_2nd_ShadeMap),
            label = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow."),
            extraPropertyName1 = new MaterialName(ShaderProp_2nd_ShadeColor) 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_2nd_ShadeColor),
            label = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUse_BaseAs1st),
            label = new GUIContent("Apply to 1st shading map", "Apply Base map to the 1st shading map."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUse_1stAs2nd),
            label = new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map."),
        },
        
        //Normal Map
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_NormalMap),
            label = new GUIContent("Normal Map", "A texture that specifies the bumpiness of the material."),
            extraPropertyName1 = new MaterialName(ShaderProp_BumpScale),
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
            mainPropertyName = new MaterialName(ShaderProp_Outline_BaseColorBlend),
            label = new GUIContent("Blend Base Color to Outline",
                "Blend base color to outline color."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderProp_Outline_LightColorBlend),
            label = new GUIContent("Blend Light Color to Outline",
                "Blend light color to outline color."),
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

        //Custom Directional Light
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUnlit_DirectionalLight_Use),
            label = new GUIContent("Custom Directional Light",
                "Apply a custom directional light."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUnlit_DirectionalLight_Direction),
            label = new GUIContent("Light Direction",
                "The direction of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUnlit_DirectionalLight_Color),
            label = new GUIContent("Light Color",
                "The color of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUnlit_DirectionalLight_Intensity),
            label = new GUIContent("Light Intensity",
                "The intensity of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUnlit_DirectionalLight_2DLightFactor),
            label = new GUIContent("2D Light Factor",
                "Multiplier for the 2D light contribution."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUnlit_DirectionalLight_DiffuseFactor),
            label = new GUIContent("Diffuse Factor",
                "Multiplier for the diffuse lighting contribution."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUnlit_DirectionalLight_SpecularFactor),
            label = new GUIContent("Specular Factor",
                "Multiplier for the specular lighting contribution."),
        },
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

    internal const string ShaderProp_NormalMap = "_NormalMap";
    internal const string ShaderProp_BumpScale = "_BumpScale";
    
    internal const string ShaderProp_OutlineMode = "_OutlineMode";
    internal const string ShaderProp_OutlineWidth = "_OutlineWidth";
    internal const string ShaderProp_OutlineWidthMap = "_OutlineWidthMap";
    internal const string ShaderProp_OutlineTex = "_OutlineTex";
    internal const string ShaderProp_OutlineColor = "_OutlineColor";
    internal const string ShaderProp_Outline_BaseColorBlend  = "_Outline_BaseColorBlend";
    internal const string ShaderProp_Outline_LightColorBlend = "_Outline_LightColorBlend";
    internal const string ShaderProp_OutlineOffsetZ = "_OutlineOffsetZ";
    internal const string ShaderProp_OutlineNear = "_OutlineNear";
    internal const string ShaderProp_OutlineFar = "_OutlineFar";

    internal const string ShaderProp_Outline_UseCustomNormalMap = "_Outline_UseCustomNormalMap";
    internal const string ShaderProp_Outline_CustomNormalMap    = "_Outline_CustomNormalMap";

    internal const string ShaderPropUnlit_DirectionalLight_Use = "_DirectionalLight_Use";
    internal const string ShaderPropUnlit_DirectionalLight_Direction = "_DirectionalLight_Direction";
    internal const string ShaderPropUnlit_DirectionalLight_Color  = "_DirectionalLight_Color";
    internal const string ShaderPropUnlit_DirectionalLight_Intensity  = "_DirectionalLight_Intensity";
    internal const string ShaderPropUnlit_DirectionalLight_2DLightFactor  = "_DirectionalLight_2DLightFactor";
    internal const string ShaderPropUnlit_DirectionalLight_DiffuseFactor  = "_DirectionalLight_DiffuseFactor";
    internal const string ShaderPropUnlit_DirectionalLight_SpecularFactor = "_DirectionalLight_SpecularFactor";
    
    //Doc: Use this LightMode tag value to draw an extra Pass when rendering objects.
    const string LIGHT_MODE_NAME_FOR_OUTLINE = "SRPDefaultUnlit";

    internal enum OutlineMode {
        NormalDirection,
        PositionScaling
    }
    
    private static readonly GUIContent[] m_outlineModeEnums= EnumUtility.ToInspectorNamesAsGUIContent(typeof(OutlineMode));
    private static readonly int[] m_outlineModeIndices = EnumUtility.ToIndices(typeof(OutlineMode));

    bool m_normalMapFoldout = false;
    bool m_outlineFoldout = false;
    bool m_specularFoldout = false;
    
}

