using System.Collections.Generic;
using Unity.Rendering.Toon;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Toon {
internal class UnityToon3Das2DGUI : UnityEditor.ShaderGUI {

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
        DrawThreeColorsGUI(mEditor, mats, m_materialPropertyUIElements, ref m_colorsFoldout);
        DrawShadingGUI(mEditor, mats, m_materialPropertyUIElements, ref m_shadingFoldout);
        DrawLightingGUI(mEditor, mats, m_materialPropertyUIElements, ref m_lightingFoldout);

        DrawNormalMapGUI(mEditor, m_materialPropertyUIElements, ref m_normalMapFoldout);

        m_materialState.useOutline = Toon3Das2DMaterialUtility.IsOutlineEnabled(mats[0]);
        DrawOutlineGUI(mEditor, mats, m_materialPropertyUIElements, ref m_outlineFoldout, ref m_materialState);

        if (EditorGUI.EndChangeCheck()) {
            mEditor.PropertiesChanged();
            
            foreach (Material m in mats) {
                Toon3Das2DMaterialUtility.EnableOutline(m, m_materialState.useOutline);
            }
            
            Toon3Das2DMaterialUtility.SetOutlineMode(mats, m_materialState.outlineMode);
        }

        m_lastMaterial = mats[0];
    }

    void RefreshFoldouts(Material mat, Dictionary<string, MaterialPropertyUIElement> uiElements) {

        m_colorsFoldout = true;
        m_shadingFoldout = true;

        m_normalMapFoldout = true;
        m_outlineFoldout = Toon3Das2DMaterialUtility.IsOutlineEnabled(mat);

        bool lightEnabled = mat.GetInteger(uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_USE].mainProperty.id) != 0;
        m_lightingFoldout = lightEnabled;

    }

//----------------------------------------------------------------------------------------------------------------------
    static void DrawThreeColorsGUI(MaterialEditor mEditor, Material[] mats,
        Dictionary<string, MaterialPropertyUIElement> uiElements, ref bool foldout) {

        ToonEditorGUIUtility.DrawFoldoutGUI(ref foldout, COLORS_FOLDOUT);
        if (!foldout)
            return;

        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_MAIN_TEX]);

        EditorGUI.indentLevel += 2;
        ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[ToonConstants.SHADER_PROP_USE_BASE_AS_1ST], out bool applyTo1st);
        EditorGUI.indentLevel -= 2;

        if (applyTo1st) {
            EditorGUI.indentLevel += 2;
            ToonEditorGUIUtility.DrawColorPropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_1_ST_SHADE_COLOR]);
            EditorGUI.indentLevel -= 2;
        } else {
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_1_ST_SHADE_MAP]);
        }

        EditorGUI.indentLevel += 2;
        ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[ToonConstants.SHADER_PROP_USE_1ST_AS_2ND], out bool applyTo2nd);
        EditorGUI.indentLevel -= 2;


        if (applyTo2nd) {
            EditorGUI.indentLevel += 2;
            ToonEditorGUIUtility.DrawColorPropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_2ND_SHADE_COLOR]);
            EditorGUI.indentLevel -= 2;
        } else {
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_2ND_SHADE_MAP]);
        }

        EditorGUILayout.Space();
    }

//----------------------------------------------------------------------------------------------------------------------    
    static void DrawShadingGUI(MaterialEditor mEditor, Material[] mats,
        Dictionary<string, MaterialPropertyUIElement> uiElements, ref bool foldout) {

        ToonEditorGUIUtility.DrawFoldoutGUI(ref foldout, SHADING_FOLDOUT);
        if (!foldout)
            return;

        EditorGUILayout.LabelField("Base to 1st Shade");
        EditorGUI.indentLevel += INDENT_SIZE;
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_BASE_TO_1ST_SHADE_START]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_BASE_TO_1ST_SHADE_FEATHER]);
        EditorGUI.indentLevel -= INDENT_SIZE;

        EditorGUILayout.LabelField("1st to 2nd Shade");
        EditorGUI.indentLevel += INDENT_SIZE;
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_1ST_TO_2ND_SHADE_START]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_1ST_TO_2ND_SHADE_FEATHER]);
        EditorGUI.indentLevel -= INDENT_SIZE;

        EditorGUILayout.Space();
    }

//----------------------------------------------------------------------------------------------------------------------    

    static void DrawLightingGUI(MaterialEditor mEditor, Material[] mats,
        Dictionary<string, MaterialPropertyUIElement> uiElements, ref bool foldout) {

        ToonEditorGUIUtility.DrawFoldoutGUI(ref foldout, LIGHTING_FOLDOUT);



        if (!foldout)
            return;

        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_2D_LIGHT_STRENGTH]);
        EditorGUILayout.Space();

        ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_USE],
            out bool directionalLightEnabled);

        EditorGUI.indentLevel += INDENT_SIZE;

        EditorGUI.BeginDisabledGroup(!directionalLightEnabled);
        ToonEditorGUIUtility.DrawVector3FieldGUI(mEditor, mats, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_DIRECTION]);
        ToonEditorGUIUtility.DrawColorFieldGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_COLOR]);
        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_INTENSITY]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_DIFFUSE_STRENGTH]);

        EditorGUILayout.LabelField("Highlight Settings");
        EditorGUI.indentLevel += INDENT_SIZE;
        ToonEditorGUIUtility.DrawVector3FieldGUI(mEditor, mats, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_VIEW_POSITION]);
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_HIGHLIGHT_TEX]);
        ToonEditorGUIUtility.DrawIntPopupGUI(mEditor, mats, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_MODE],
            m_highlightModeEnums, m_highlightModeIndices, out int _);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_STRENGTH]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_SIZE]);

        EditorGUI.indentLevel -= INDENT_SIZE;

        EditorGUI.EndDisabledGroup();

        EditorGUI.indentLevel -= INDENT_SIZE;
        EditorGUILayout.Space();

    }

//----------------------------------------------------------------------------------------------------------------------
    static void DrawNormalMapGUI(MaterialEditor mEditor, Dictionary<string, MaterialPropertyUIElement> uiElements,
        ref bool foldout) {

        ToonEditorGUIUtility.DrawFoldoutGUI(ref foldout, uiElements[ToonConstants.SHADER_PROP_NORMAL_MAP].label);
        if (!foldout)
            return;

        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_NORMAL_MAP]);
        mEditor.TextureScaleOffsetProperty(uiElements[ToonConstants.SHADER_PROP_NORMAL_MAP].mainProperty.prop);

        EditorGUILayout.Space();
    }


    static void DrawOutlineGUI(MaterialEditor mEditor, Material[] mats, Dictionary<string,
        MaterialPropertyUIElement> uiElements, ref bool foldout, ref ToonMaterialState state) {

        ToonEditorGUIUtility.DrawFoldoutWithToggleGUI(mEditor, ref foldout, ref state.useOutline, "Outline");
        
        if (!foldout)
            return;

        //Outline Settings
        EditorGUI.indentLevel++;
        EditorGUI.BeginDisabledGroup(!state.useOutline);

        ToonEditorGUIUtility.DrawIntPopupGUI(mEditor, mats, uiElements[ToonConstants.SHADER_PROP_OUTLINE_MODE],
            m_outlineModeEnums, m_outlineModeIndices, out int outlineMode);

        state.outlineMode = (ToonOutlineMode) outlineMode;

        EditorGUI.BeginDisabledGroup(outlineMode != (int)ToonOutlineMode.NormalDirection);
        {
            ToonEditorGUIUtility.DrawToggleGUI(mEditor, mats, uiElements[ToonConstants.SHADER_PROP_OUTLINE_USE_NORMAL_MAP],
                out bool useCustom);
            EditorGUI.BeginDisabledGroup(!useCustom);
            ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_NORMAL_MAP]);
            EditorGUI.EndDisabledGroup();
        }
        EditorGUI.EndDisabledGroup();


        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_WIDTH]);
        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_WIDTH_MAP]);

        ToonEditorGUIUtility.DrawTexturePropertySingleLineGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_TEX]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_BASE_COLOR_BLEND]);
        ToonEditorGUIUtility.DrawRangePropertyGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_LIGHT_COLOR_BLEND]);


        ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_OFFSET_Z]);


        EditorGUILayout.Space();
        {
            EditorGUILayout.LabelField("Camera Distance for Outline Width");
            EditorGUI.indentLevel++;
            ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_NEAR]);
            ToonEditorGUIUtility.DrawFloatFieldGUI(mEditor, uiElements[ToonConstants.SHADER_PROP_OUTLINE_FAR]);
            EditorGUI.indentLevel--;


        }
        EditorGUI.EndDisabledGroup(); //!isOutlineEnabled
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
    }


//----------------------------------------------------------------------------------------------------------------------

    void InitMaterialPropertyUIElements(MaterialProperty[] allProps) {
        int numProperties = m_materialUIElements.Count;
        for (int i = 0; i < numProperties; ++i) {
            MaterialUIElement propInfo = m_materialUIElements[i];

            MaterialPropertyInfo mainProp = MaterialNameToPropertyInfo(propInfo.mainPropertyName, allProps);
            MaterialPropertyInfo extraProperty1 =
                null != propInfo.extraPropertyName1 ? MaterialNameToPropertyInfo(propInfo.extraPropertyName1, allProps) : null;

            MaterialPropertyInfo extraProperty2 =
                null != propInfo.extraPropertyName2 ? MaterialNameToPropertyInfo(propInfo.extraPropertyName2, allProps) : null;


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

    //key: Toon3Das2D prop. value: Toon3D prop
    static readonly Dictionary<string, string> TOON_3D_AS_2D_TO_3D_MAP = new Dictionary<string, string> {

        // Shading thresholds
        { ToonConstants.SHADER_PROP_BASE_TO_1ST_SHADE_START, ToonConstants.SHADER_PROP_TOON3D_BASE_COLOR_STEP },
        { ToonConstants.SHADER_PROP_BASE_TO_1ST_SHADE_FEATHER, ToonConstants.SHADER_PROP_TOON3D_BASE_SHADE_FEATHER },
        { ToonConstants.SHADER_PROP_1ST_TO_2ND_SHADE_START, ToonConstants.SHADER_PROP_TOON3D_1ST_SHADE_COLOR_STEP },
        { ToonConstants.SHADER_PROP_1ST_TO_2ND_SHADE_FEATHER, ToonConstants.SHADER_PROP_TOON3D_1ST_SHADE_COLOR_FEATHER },

        // Highlight
        { ToonConstants.SHADER_PROP_HIGHLIGHT_COLOR, ToonConstants.SHADER_PROP_TOON3D_HIGH_COLOR },
        { ToonConstants.SHADER_PROP_HIGHLIGHT_TEX, ToonConstants.SHADER_PROP_TOON3D_HIGH_COLOR_TEX },

        // Outline
        { ToonConstants.SHADER_PROP_OUTLINE_MODE, ToonConstants.SHADER_PROP_TOON3D_OUTLINE },
        { ToonConstants.SHADER_PROP_OUTLINE_WIDTH, ToonConstants.SHADER_PROP_TOON3D_OUTLINE_WIDTH },
        { ToonConstants.SHADER_PROP_OUTLINE_WIDTH_MAP, ToonConstants.SHADER_PROP_TOON3D_OUTLINE_SAMPLER },
        { ToonConstants.SHADER_PROP_OUTLINE_COLOR, ToonConstants.SHADER_PROP_TOON3D_OUTLINE_COLOR },
        { ToonConstants.SHADER_PROP_OUTLINE_BASE_COLOR_BLEND, ToonConstants.SHADER_PROP_TOON3D_IS_BLEND_BASE_COLOR },
        { ToonConstants.SHADER_PROP_OUTLINE_LIGHT_COLOR_BLEND, ToonConstants.SHADER_PROP_TOON3D_IS_LIGHT_COLOR_OUTLINE },
        { ToonConstants.SHADER_PROP_OUTLINE_OFFSET_Z, ToonConstants.SHADER_PROP_TOON3D_OFFSET_Z },
        { ToonConstants.SHADER_PROP_OUTLINE_NEAR, ToonConstants.SHADER_PROP_TOON3D_NEAREST_DISTANCE },
        { ToonConstants.SHADER_PROP_OUTLINE_FAR, ToonConstants.SHADER_PROP_TOON3D_FARTHEST_DISTANCE },

        // Outline normal options (closest equivalents)
        { ToonConstants.SHADER_PROP_OUTLINE_USE_NORMAL_MAP, ToonConstants.SHADER_PROP_TOON3D_IS_BAKED_NORMAL },
        { ToonConstants.SHADER_PROP_OUTLINE_NORMAL_MAP, ToonConstants.SHADER_PROP_TOON3D_BAKED_NORMAL },
    };
 

    
//----------------------------------------------------------------------------------------------------------------------    
    public override void AssignNewShaderToMaterial(
        Material mat,
        Shader oldShader,
        Shader newShader) 
    {
        
        string oldShaderPath = AssetDatabase.GetAssetPath(oldShader);
        if (!oldShaderPath.StartsWith(ToonEditorConstants.PACKAGE_PATH)) {
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);
            return;
        }

        //Upgrade from Toon 3D
        bool upgradeFromToon3D = oldShader.name.EndsWith("Toon") || oldShader.name.EndsWith("Toon(Tessellation)"); 
        if (!upgradeFromToon3D) {
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);
            return;
        }
        Dictionary<string, MaterialPropertyValue> captured = ToonMaterialUtility.CaptureMaterialValues(mat);
        
        base.AssignNewShaderToMaterial(mat, oldShader, newShader);
        
        //Assign captured values
        foreach (KeyValuePair<string, string> kv in TOON_3D_AS_2D_TO_3D_MAP) {
            string targetName = kv.Key; 
            string srcName = kv.Value;  

            // Ensure target exists in new material and we captured the source
            if (!mat.HasProperty(targetName) || !captured.TryGetValue(srcName, out MaterialPropertyValue srcVal)) {
                continue;
            }

            // Ensure types match between new target and captured source
            int targetIndex = newShader.FindPropertyIndex(targetName);
            if (targetIndex < 0) {
                continue;
            }
            ShaderPropertyType targetType = newShader.GetPropertyType(targetIndex);
            if (targetType != srcVal.type) {
                continue;
            }

            srcVal.ApplyToMaterial(mat, targetName);
        }
        
    }
    


    private readonly Dictionary<string, MaterialPropertyUIElement> m_materialPropertyUIElements = new Dictionary<string, MaterialPropertyUIElement>();

    private ToonMaterialState m_materialState;
    
    private static readonly List<MaterialUIElement> m_materialUIElements = new List<MaterialUIElement>() {
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_MAIN_TEX),
            label = new GUIContent("Base Map", "Base Color : Texture(sRGB) × Color(RGB)."),
            extraPropertyName1 = new MaterialName(ToonConstants.SHADER_PROP_BASE_COLOR),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_1_ST_SHADE_MAP),
            label = new GUIContent("1st Shading Map", "The map used for the brighter portions of the shadow."),
            extraPropertyName1 = new MaterialName(ToonConstants.SHADER_PROP_1_ST_SHADE_COLOR),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_1_ST_SHADE_COLOR),
            label = new GUIContent("1st Shading Map", "The map used for the brighter portions of the shadow."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_2ND_SHADE_MAP),
            label = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow."),
            extraPropertyName1 = new MaterialName(ToonConstants.SHADER_PROP_2ND_SHADE_COLOR)
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_2ND_SHADE_COLOR),
            label = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_USE_BASE_AS_1ST),
            label = new GUIContent("Apply to 1st shading map", "Apply Base map to the 1st shading map."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_USE_1ST_AS_2ND),
            label = new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map."),
        },

        //Shading
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_BASE_TO_1ST_SHADE_START),
            label = new GUIContent("Start",
                "The threshold for transitioning to the 1st shade color. " +
                "0: use the base color (no transition), " +
                "1: starts transitioning immediately."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_BASE_TO_1ST_SHADE_FEATHER),
            label = new GUIContent("Feather", "Controls feathering to the 1st shade color. 0: sharp transition, 1: fully feathered."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_1ST_TO_2ND_SHADE_START),
            label = new GUIContent("Start",
                "The threshold for transitioning to the 2nd shade color. " +
                "0: use the 1st shade color (no transition), " +
                "1: starts transitioning immediately."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_1ST_TO_2ND_SHADE_FEATHER),
            label = new GUIContent("Feather", "Controls feathering to the 2nd shade color. 0: sharp transition, 1: fully feathered."),
        },

        //Lighting
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_2D_LIGHT_STRENGTH),
            label = new GUIContent("2D Light Factor",
                "Multiplier for the 2D light contribution."),
        },
        //Custom Directional Light
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_USE),
            label = new GUIContent("Custom Directional Light",
                "Apply a custom directional light."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_DIRECTION),
            label = new GUIContent("Light Direction",
                "The direction of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_COLOR),
            label = new GUIContent("Light Color",
                "The color of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_INTENSITY),
            label = new GUIContent("Light Intensity",
                "The intensity of the custom directional light. "),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_DIFFUSE_STRENGTH),
            label = new GUIContent("Diffuse Strength",
                "Multiplier for the diffuse contribution."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_VIEW_POSITION),
            label = new GUIContent("View Position", "Camera View Position"),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_HIGHLIGHT_TEX),
            label = new GUIContent("Highlight Map", "Highlight Map."),
            extraPropertyName1 = new MaterialName(ToonConstants.SHADER_PROP_HIGHLIGHT_COLOR)
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_MODE),
            label = new GUIContent("Mode", "Highlight mode."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_STRENGTH),
            label = new GUIContent("Highlight Strength", "Multiplier for the highlight contribution."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_SIZE),
            label = new GUIContent("Highlight Size", "Highlight size."),
        },


        //Normal Map
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_NORMAL_MAP),
            label = new GUIContent("Normal Map", "A texture that specifies the bumpiness of the material."),
            extraPropertyName1 = new MaterialName(ToonConstants.SHADER_PROP_BUMP_SCALE),
        },

        //Outline Start
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_WIDTH),
            label = new GUIContent("Outline Width",
                "The width of the outline."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_WIDTH_MAP),
            label = new GUIContent("Outline Width Map",
                "Outline Width Map (grayscale, linear): White = full width, Black = 0 width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_TEX),
            label = new GUIContent("Outline Color", "The color of outline."),
            extraPropertyName1 = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_COLOR),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_BASE_COLOR_BLEND),
            label = new GUIContent("Blend Base Color to Outline",
                "Blend base color to outline color."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_LIGHT_COLOR_BLEND),
            label = new GUIContent("Blend Light Color to Outline",
                "Blend the combined effect of 2D lighting and custom directional light to the outline color."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_OFFSET_Z),
            label = new GUIContent("Z Offset",
                "Offsets the outline in the depth (Z) direction of the camera."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_NEAR),
            label = new GUIContent("Near",
                "Nearest distance for maximum outline width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_FAR),
            label = new GUIContent("Far",
                "Furthest distance where outline fades to zero width."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_MODE),
            label = new GUIContent("Outline Mode",
                "Specifies how the outline is generated."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_USE_NORMAL_MAP),
            label = new GUIContent("Use Normal Map",
                "Use a normal map for outline."),
        },

        new MaterialUIElement {
            mainPropertyName = new MaterialName(ToonConstants.SHADER_PROP_OUTLINE_NORMAL_MAP),
            label = new GUIContent("Normal Map",
                "Normal map for outline. "),
        },
        //Outline End

    };


    internal enum HighlightMode {
        Hard,
        Soft,
    }

    private static readonly GUIContent[] m_outlineModeEnums = ToonEnumUtility.ToInspectorNamesAsGUIContent(typeof(ToonOutlineMode));
    private static readonly int[] m_outlineModeIndices = ToonEnumUtility.ToIndices(typeof(ToonOutlineMode));

    private static readonly GUIContent[] m_highlightModeEnums = ToonEnumUtility.ToInspectorNamesAsGUIContent(typeof(HighlightMode));
    private static readonly int[] m_highlightModeIndices = ToonEnumUtility.ToIndices(typeof(HighlightMode));

    static readonly GUIContent COLORS_FOLDOUT = EditorGUIUtility.TrTextContent("Colors",
        "Colors for basic cel-shading settings in Unity Toon Shader.");

    static readonly GUIContent SHADING_FOLDOUT = EditorGUIUtility.TrTextContent("Shading", "Shading settings.");

    static readonly GUIContent LIGHTING_FOLDOUT
        = EditorGUIUtility.TrTextContent("Lighting", "Lighting settings.");

    bool m_colorsFoldout = true;
    bool m_shadingFoldout = true;
    bool m_normalMapFoldout = false;
    bool m_outlineFoldout = false;
    bool m_lightingFoldout = false;
    
    private Material m_lastMaterial;
    

    struct ToonMaterialState {
        internal bool useOutline;
        internal ToonOutlineMode outlineMode;
    } 
    

    private const int INDENT_SIZE = 2;

}

} //end namespace