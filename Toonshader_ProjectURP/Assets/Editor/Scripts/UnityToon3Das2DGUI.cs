using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class UnityToon3Das2DGUI : UnityEditor.ShaderGUI {

    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
        
        Material material = materialEditor.target as Material;
        if (material == null)
            return;
        
        something2(props);
        
        EditorGUI.BeginChangeCheck();
        GUI_BasicThreeColors(materialEditor, material);

        if (EditorGUI.EndChangeCheck()) {
            materialEditor.PropertiesChanged();
        }
    }
    
//----------------------------------------------------------------------------------------------------------------------    
    
    void GUI_BasicThreeColors(MaterialEditor materialEditor, Material material) {

        DrawTexturePropertySingleLineGUI(materialEditor, propertyInfos[ShaderPropMainTex]);

        EditorGUI.indentLevel += 2;
        bool applyTo1st = DrawToggleGUI(materialEditor, material, propertyInfos[ShaderPropUse_BaseAs1st]);        
        EditorGUI.indentLevel -= 2;

        if (applyTo1st) {
            EditorGUI.indentLevel += 2;
            DrawColorPropertyGUI(materialEditor, propertyInfos[ShaderProp_1st_ShadeColor]);
            EditorGUI.indentLevel -= 2;
        }
        else {
            DrawTexturePropertySingleLineGUI(materialEditor, propertyInfos[ShaderProp_1st_ShadeMap]);
        }

        EditorGUI.indentLevel += 2;
        bool applyTo2nd = DrawToggleGUI(materialEditor, material, propertyInfos[ShaderPropUse_1stAs2nd]);        
        EditorGUI.indentLevel -= 2;


        if (applyTo2nd) {
            EditorGUI.indentLevel += 2;
            DrawColorPropertyGUI(materialEditor, propertyInfos[ShaderProp_2nd_ShadeColor]);
            EditorGUI.indentLevel -= 2;
        }
        else {
            DrawTexturePropertySingleLineGUI(materialEditor, propertyInfos[ShaderProp_2nd_ShadeMap]);
        }
    }
    
    void DrawTexturePropertySingleLineGUI(MaterialEditor materialEditor, something element) {
        if (null!= element.extraProperty2)
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop,element.extraProperty1.prop, element.extraProperty2.prop);
        else if (null!= element.extraProperty1)
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop,element.extraProperty1.prop);
        else
            materialEditor.TexturePropertySingleLine(element.label, element.mainProperty.prop);
    }

    void DrawColorPropertyGUI(MaterialEditor materialEditor, something element) {
        materialEditor.ColorProperty(element.mainProperty.prop, element.label.text);
        
    }
    
    bool DrawToggleGUI(MaterialEditor materialEditor, Material material, something element) {
        EditorGUI.BeginChangeCheck();
        bool ret = EditorGUILayout.Toggle(element.label, material.GetInteger(element.mainProperty.id) !=0);
        if (EditorGUI.EndChangeCheck()) {
            materialEditor.RegisterPropertyChangeUndo(element.label.text);
            material.SetInteger(element.mainProperty.id, ret ? 1 : 0);
        }

        return ret;
    }
    

//----------------------------------------------------------------------------------------------------------------------

    void something2(MaterialProperty[] allProps) {
        int numProperties = propertyList.Count;
        for (int i = 0; i < numProperties; ++i) {
            haha propInfo = propertyList[i];
            
            MaterialPropertyWithID mainProp = new MaterialPropertyWithID(propInfo.mainPropertyName, allProps);
            MaterialPropertyWithID extraProperty1 = null!= propInfo.extraPropertyName1 ? 
                new MaterialPropertyWithID(propInfo.extraPropertyName1, allProps) : null;

            MaterialPropertyWithID extraProperty2 = null!= propInfo.extraPropertyName2 ? 
                new MaterialPropertyWithID(propInfo.extraPropertyName2, allProps) : null;
            

            something newElement10 = new something {
                label = propInfo.label,
                mainProperty = mainProp,
                extraProperty1 = extraProperty1,
                extraProperty2 = extraProperty2,
            };

            propertyInfos[propInfo.mainPropertyName.name] = newElement10;
        }
        
    }
    
//----------------------------------------------------------------------------------------------------------------------

    class MaterialPropertyWithID {
        public MaterialProperty prop;
        public int id;

        public MaterialPropertyWithID(MaterialNameWithID m, MaterialProperty[] allProps) {
            prop = FindProperty(m.name, allProps); id = m.id;
        }
    }

    class something {
        public GUIContent label;
        public MaterialPropertyWithID mainProperty;
        public MaterialPropertyWithID extraProperty1;
        public MaterialPropertyWithID extraProperty2;
    }

    class MaterialNameWithID {
        public string name;
        public int id;
        public MaterialNameWithID(string s) { name = s; id = Shader.PropertyToID(s); }
    }

    
    struct haha {
        public MaterialNameWithID mainPropertyName;
        
        public GUIContent label;
        public MaterialNameWithID extraPropertyName1;
        public MaterialNameWithID extraPropertyName2;
    }
    
    Dictionary<string, something> propertyInfos = new Dictionary<string, something>();
    
    static List<haha> propertyList = new List<haha>() {
        new haha {
            mainPropertyName = new MaterialNameWithID(ShaderPropMainTex),
            label = new GUIContent("Base Map", "Base Color : Texture(sRGB) × Color(RGB) Default:White"),
            extraPropertyName1 = new MaterialNameWithID(ShaderProp_BaseColor), 
        },
        new haha {
            mainPropertyName = new MaterialNameWithID(ShaderProp_1st_ShadeMap),
            label = new GUIContent("1st Shading Map", "The map used for the brighter portions of the shadow."),
            extraPropertyName1 = new MaterialNameWithID(ShaderProp_1st_ShadeColor), 
        },
        new haha {
            mainPropertyName = new MaterialNameWithID(ShaderProp_2nd_ShadeMap),
            label = new GUIContent("2nd Shading Map", "The map used for the darker portions of the shadow."),
            extraPropertyName1 = new MaterialNameWithID(ShaderProp_2nd_ShadeColor) 
        },
        new haha {
            mainPropertyName = new MaterialNameWithID(ShaderPropUse_BaseAs1st),
            label = new GUIContent("Apply to 1st shading map", "Apply Base map to the 1st shading map."),
        },
        new haha {
            mainPropertyName = new MaterialNameWithID(ShaderPropUse_1stAs2nd),
            label = new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map."),
        },
        new haha {
            mainPropertyName = new MaterialNameWithID(ShaderPropUse_1stAs2nd),
            label = new GUIContent("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map."),
        },
        new haha {
            mainPropertyName = new MaterialNameWithID(ShaderPropUse_1stAs2nd),
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

