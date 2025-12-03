using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Rendering.Toon {

[System.Serializable]
internal class MaterialPropertyValue {
    internal static MaterialPropertyValue FromMaterial(Material mat, int propIndex) {
        MaterialPropertyValue value = new MaterialPropertyValue();
        Shader shader = mat.shader;
        string name = shader.GetPropertyName(propIndex);
        ShaderPropertyType type = shader.GetPropertyType(propIndex);
        value.type = type;

        switch (type) {
            case ShaderPropertyType.Color:
                value.color = mat.GetColor(name);
                break;
            case ShaderPropertyType.Vector:
                value.vector = mat.GetVector(name);
                break;
            case ShaderPropertyType.Float:
            case ShaderPropertyType.Range:
                value.floatValue = mat.GetFloat(name);
                break;
            case ShaderPropertyType.Texture:
                value.texture = mat.GetTexture(name);
                value.texOffset = mat.GetTextureOffset(name);
                value.texScale = mat.GetTextureScale(name);
                break;
        }

        return value;
    }
    
    internal void ApplyToMaterial(Material mat, string targetName) {
        switch (type) {
            case ShaderPropertyType.Color:
                mat.SetColor(targetName, color);
                break;
            case ShaderPropertyType.Vector:
                mat.SetVector(targetName, vector);
                break;
            case ShaderPropertyType.Float:
            case ShaderPropertyType.Range:
                mat.SetFloat(targetName, floatValue);
                break;
            case ShaderPropertyType.Texture:
                mat.SetTexture(targetName, texture);
                mat.SetTextureOffset(targetName, texOffset);
                mat.SetTextureScale(targetName, texScale);
                break;
        }
    }
    
//----------------------------------------------------------------------------------------------------------------------    
    
    internal ShaderPropertyType type;
    internal Color color;
    internal Vector4 vector;
    internal float floatValue;
    internal Texture texture;
    internal Vector2 texOffset;
    internal Vector2 texScale;
    

}

} //end namespace