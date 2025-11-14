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

        if (EditorGUI.EndChangeCheck()) {
            materialEditor.PropertiesChanged();
        }
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
    

//----------------------------------------------------------------------------------------------------------------------

    void InitMaterialPropertyUIElements(MaterialProperty[] allProps) {
        int numProperties = m_materialUIElements.Count;
        for (int i = 0; i < numProperties; ++i) {
            MaterialUIElement propInfo = m_materialUIElements[i];
            
            MaterialPropertyInfo mainProp = new MaterialPropertyInfo(propInfo.mainPropertyName, allProps);
            MaterialPropertyInfo extraProperty1 = null!= propInfo.extraPropertyName1 ? 
                new MaterialPropertyInfo(propInfo.extraPropertyName1, allProps) : null;

            MaterialPropertyInfo extraProperty2 = null!= propInfo.extraPropertyName2 ? 
                new MaterialPropertyInfo(propInfo.extraPropertyName2, allProps) : null;
            

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

    class MaterialPropertyInfo {
        public readonly MaterialProperty prop;
        public readonly int id;

        public MaterialPropertyInfo(MaterialName m, MaterialProperty[] allProps) {
            prop = FindProperty(m.name, allProps); id = m.nameID;
        }
    }

    class MaterialPropertyUIElement {
        public GUIContent label;
        public MaterialPropertyInfo mainProperty;
        public MaterialPropertyInfo extraProperty1;
        public MaterialPropertyInfo extraProperty2;
    }

    class MaterialName {
        public readonly string name;
        public readonly int nameID;
        public MaterialName(string s) { name = s; nameID = Shader.PropertyToID(s); }
    }

    
    struct MaterialUIElement {
        public GUIContent label;
        public MaterialName mainPropertyName;
        public MaterialName extraPropertyName1;
        public MaterialName extraPropertyName2;
    }

    private readonly Dictionary<string, MaterialPropertyUIElement> m_materialPropertyUIElements = new Dictionary<string, MaterialPropertyUIElement>();

    private static readonly List<MaterialUIElement> m_materialUIElements = new List<MaterialUIElement>() {
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropMainTex),
            label = new GUIContent("Base Map", "Base Color : Texture(sRGB) × Color(RGB) Default:White"),
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
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUse_1stAs2nd),
            label = new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map."),
        },
        new MaterialUIElement {
            mainPropertyName = new MaterialName(ShaderPropUse_1stAs2nd),
            label = new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map."),
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
    
}

