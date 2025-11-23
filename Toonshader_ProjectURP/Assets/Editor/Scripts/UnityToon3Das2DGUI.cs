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

        InitMaterialPropertyUIElements(props);

        if (mats[0] != m_lastMaterial) {
            RefreshFoldouts(mats[0], m_materialPropertyUIElements);
        }
        
        EditorGUI.BeginChangeCheck();
        DrawThreeColorsGUI(mEditor, mats, m_materialPropertyUIElements);
        DrawDirectionalLightGUI(mEditor, mats, m_materialPropertyUIElements, ref m_directionalLightFoldout);

        DrawNormalMapGUI(mEditor, m_materialPropertyUIElements, ref m_normalMapFoldout);
        DrawOutlineGUI(mEditor, mats, m_materialPropertyUIElements, ref m_outlineFoldout);
        

        if (EditorGUI.EndChangeCheck()) {
            mEditor.PropertiesChanged();
        }

        m_lastMaterial = mats[0];
    }
    
    void RefreshFoldouts(Material mat, Dictionary<string, MaterialPropertyUIElement> uiElements) {
        
        m_normalMapFoldout = true;
        m_outlineFoldout = mat.GetShaderPassEnabled(LIGHT_MODE_NAME_FOR_OUTLINE);

        bool lightEnabled = mat.GetInteger(uiElements[SHADER_PROP_DIRECTIONAL_LIGHT_USE].mainProperty.id) !=0;
        m_directionalLightFoldout = lightEnabled;
        
    }
    
//----------------------------------------------------------------------------------------------------------------------
    static void DrawNormalMapGUI(MaterialEditor mEditor, Dictionary<string, MaterialPropertyUIElement> uiElements, 
        ref bool foldout) {

        ToonEditorGUIUtility.DrawFoldoutGUI(ref foldout, uiElements[SHADER_PROP_NORMAL_MAP].label);
        if (!foldout) 
            return;
        
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[SHADER_PROP_NORMAL_MAP]);
        mEditor.TextureScaleOffsetProperty(uiElements[SHADER_PROP_NORMAL_MAP].mainProperty.prop);
        
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

        ToonEditorGUIUtility.DrawIntPopupGUI(mEditor, mats, uiElements[SHADER_PROP_OUTLINE_MODE], 
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
            ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[SHADER_PROP_OUTLINE_USE_CUSTOM_NORMAL_MAP], 
                out bool useCustom);
            EditorGUI.BeginDisabledGroup(!useCustom);
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor,uiElements[SHADER_PROP_OUTLINE_CUSTOM_NORMAL_MAP]);
            EditorGUI.EndDisabledGroup();
        }
        EditorGUI.EndDisabledGroup();
        

        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[SHADER_PROP_OUTLINE_WIDTH]);
        
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[SHADER_PROP_OUTLINE_TEX]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[SHADER_PROP_OUTLINE_BASE_COLOR_BLEND]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[SHADER_PROP_OUTLINE_LIGHT_COLOR_BLEND]);
        
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[SHADER_PROP_OUTLINE_WIDTH_MAP]);
        
        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[SHADER_PROP_OUTLINE_OFFSET_Z]);


        EditorGUILayout.Space();
        {
            EditorGUILayout.LabelField("Camera Distance for Outline Width");
            EditorGUI.indentLevel++;
            ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[SHADER_PROP_OUTLINE_NEAR]);
            ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[SHADER_PROP_OUTLINE_FAR]);
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
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[SHADER_PROP_MAIN_TEX]);

        EditorGUI.indentLevel += 2;
        ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[SHADER_PROP_USE_BASE_AS1_ST], out bool applyTo1st );
        EditorGUI.indentLevel -= 2;

        if (applyTo1st) {
            EditorGUI.indentLevel += 2;
            ToonEditorGUIUtility.DrawColorPropertyGUI(mEditor, uiElements[SHADER_PROP_1_ST_SHADE_COLOR]);
            EditorGUI.indentLevel -= 2;
        } else {
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[SHADER_PROP_1_ST_SHADE_MAP]);
        }

        EditorGUI.indentLevel += 2;
        ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[SHADER_PROP_USE_1_ST_AS2_ND], out bool applyTo2nd);
        EditorGUI.indentLevel -= 2;


        if (applyTo2nd) {
            EditorGUI.indentLevel += 2;
            ToonEditorGUIUtility.DrawColorPropertyGUI(mEditor, uiElements[SHADER_PROP_2_ND_SHADE_COLOR]);
            EditorGUI.indentLevel -= 2;
        } else {
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[SHADER_PROP_2_ND_SHADE_MAP]);
        }
        
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[SHADER_PROP_2D_LIGHT_STRENGTH]);
        
    }
    
    static void DrawDirectionalLightGUI(MaterialEditor mEditor, Material[] mats, 
        Dictionary<string, MaterialPropertyUIElement> uiElements, ref bool foldout) {

        ToonEditorGUIUtility.DrawFoldoutWithToggleGUI(mEditor, mats, 
            uiElements[SHADER_PROP_DIRECTIONAL_LIGHT_USE], ref foldout, out bool toggleEnabled);
        
        if (!foldout)
            return;
        
        EditorGUI.BeginDisabledGroup(!toggleEnabled);
        ToonEditorGUIUtility.DrawVector3FieldGUI(mEditor, mats, uiElements[Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Direction]);
        ToonEditorGUIUtility.DrawColorFieldGUI(mEditor, uiElements[Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Color]);
        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Intensity]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[SHADER_PROP_DIRECTIONAL_LIGHT_DIFFUSE_FACTOR]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[SHADER_PROP_DIRECTIONAL_LIGHT_SPECULAR_FACTOR]);
        EditorGUI.EndDisabledGroup();
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
            mainPropertyName = new MaterialName(SHADER_PROP_MAIN_TEX),
            label = new GUIContent("Base Map", "Base Color : Texture(sRGB) × Color(RGB)."),
            extraPropertyName1 = new MaterialName(SHADER_PROP_BASE_COLOR), 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_1_ST_SHADE_MAP),
            label = new GUIContent("1st Shading Map", "The map used for the brighter portions of the shadow."),
            extraPropertyName1 = new MaterialName(SHADER_PROP_1_ST_SHADE_COLOR), 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_1_ST_SHADE_COLOR),
            label = new GUIContent("1st Shading Map", "The map used for the brighter portions of the shadow."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_2_ND_SHADE_MAP),
            label = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow."),
            extraPropertyName1 = new MaterialName(SHADER_PROP_2_ND_SHADE_COLOR) 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_2_ND_SHADE_COLOR),
            label = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_USE_BASE_AS1_ST),
            label = new GUIContent("Apply to 1st shading map", "Apply Base map to the 1st shading map."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_USE_1_ST_AS2_ND),
            label = new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map."),
        },
        
        //Normal Map
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_NORMAL_MAP),
            label = new GUIContent("Normal Map", "A texture that specifies the bumpiness of the material."),
            extraPropertyName1 = new MaterialName(SHADER_PROP_BUMP_SCALE),
        },
        
        //Outline Start
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_WIDTH),
            label = new GUIContent("Outline Width",
                "The width of the outline."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_WIDTH_MAP),
            label = new GUIContent("Outline Width Map",
                "Outline Width Map (grayscale, linear): White = full width, Black = 0 width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_TEX),
            label = new GUIContent("Outline Color", "The color of outline."),
            extraPropertyName1 = new MaterialName(SHADER_PROP_OUTLINE_COLOR), 
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_BASE_COLOR_BLEND),
            label = new GUIContent("Blend Base Color to Outline",
                "Blend base color to outline color."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_LIGHT_COLOR_BLEND),
            label = new GUIContent("Blend Light Color to Outline",
                "Blend light color to outline color."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_OFFSET_Z),
            label = new GUIContent("Z Offset",
                "Offsets the outline in the depth (Z) direction of the camera."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_NEAR),
            label = new GUIContent("Near",
                "Nearest distance for maximum outline width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_FAR),
            label = new GUIContent("Far",
                "Furthest distance where outline fades to zero width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_MODE),
            label = new GUIContent("Outline Mode",
                "Specifies how the outline is generated."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_USE_CUSTOM_NORMAL_MAP),
            label = new GUIContent("Use Custom Normal Map",
                "Use a custom normal map for outline."),
        },
        
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_OUTLINE_CUSTOM_NORMAL_MAP),
            label = new GUIContent("Custom Normal Map",
                "Custom normal map (linear) for outline. "),
        },
        //Outline End

        //Custom Directional Light
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_DIRECTIONAL_LIGHT_USE),
            label = new GUIContent("Custom Directional Light",
                "Apply a custom directional light."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Direction),
            label = new GUIContent("Light Direction",
                "The direction of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Color),
            label = new GUIContent("Light Color",
                "The color of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Intensity),
            label = new GUIContent("Light Intensity",
                "The intensity of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_2D_LIGHT_STRENGTH),
            label = new GUIContent("2D Light Factor",
                "Multiplier for the 2D light contribution."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_DIRECTIONAL_LIGHT_DIFFUSE_FACTOR),
            label = new GUIContent("Diffuse Factor",
                "Multiplier for the diffuse lighting contribution."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(SHADER_PROP_DIRECTIONAL_LIGHT_SPECULAR_FACTOR),
            label = new GUIContent("Specular Factor",
                "Multiplier for the specular lighting contribution."),
        },
    };

    
    //Common constants
    internal const string SHADER_PROP_MAIN_TEX = "_MainTex";
    internal const string SHADER_PROP_USE_BASE_AS1_ST = "_Use_BaseAs1st";
    internal const string SHADER_PROP_USE_1_ST_AS2_ND = "_Use_1stAs2nd";
    internal const string SHADER_PROP_BASE_COLOR = "_BaseColor";
    internal const string SHADER_PROP_1_ST_SHADE_MAP = "_1st_ShadeMap";
    internal const string SHADER_PROP_1_ST_SHADE_COLOR = "_1st_ShadeColor";
    internal const string SHADER_PROP_2_ND_SHADE_MAP = "_2nd_ShadeMap";
    internal const string SHADER_PROP_2_ND_SHADE_COLOR = "_2nd_ShadeColor";
    internal const string SHADER_PROP_2D_LIGHT_STRENGTH  = "_2DLightStrength";

    internal const string SHADER_PROP_NORMAL_MAP = "_NormalMap";
    internal const string SHADER_PROP_BUMP_SCALE = "_BumpScale";
    
    internal const string SHADER_PROP_OUTLINE_MODE = "_OutlineMode";
    internal const string SHADER_PROP_OUTLINE_WIDTH = "_OutlineWidth";
    internal const string SHADER_PROP_OUTLINE_WIDTH_MAP = "_OutlineWidthMap";
    internal const string SHADER_PROP_OUTLINE_TEX = "_OutlineTex";
    internal const string SHADER_PROP_OUTLINE_COLOR = "_OutlineColor";
    internal const string SHADER_PROP_OUTLINE_BASE_COLOR_BLEND  = "_Outline_BaseColorBlend";
    internal const string SHADER_PROP_OUTLINE_LIGHT_COLOR_BLEND = "_Outline_LightColorBlend";
    internal const string SHADER_PROP_OUTLINE_OFFSET_Z = "_OutlineOffsetZ";
    internal const string SHADER_PROP_OUTLINE_NEAR = "_OutlineNear";
    internal const string SHADER_PROP_OUTLINE_FAR = "_OutlineFar";

    internal const string SHADER_PROP_OUTLINE_USE_CUSTOM_NORMAL_MAP = "_Outline_UseCustomNormalMap";
    internal const string SHADER_PROP_OUTLINE_CUSTOM_NORMAL_MAP    = "_Outline_CustomNormalMap";

    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_USE = "_DirectionalLight_Use";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_DIFFUSE_FACTOR  = "_DirectionalLight_DiffuseFactor";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_SPECULAR_FACTOR = "_DirectionalLight_SpecularFactor";
    
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
    bool m_directionalLightFoldout = false;
    
}

