using System.Collections.Generic;
using UnityEngine;

namespace Unity.Rendering.Toon {

/// <summary>
/// Provides utility methods for materials using the Unity Toon Shader.
/// </summary>
internal static class ToonMaterialUtility {
    
    /// <summary>
    /// Updates the material parameters based on its current property values.
    /// <param name="mat">The material to update. Assumed to be using the Toon shader (3D) </param>
    /// </summary>
    public static void UpdateProperties(Material mat) {

        float highlightPower = mat.GetFloat(ToonConstants.SHADER_PROP_HIGHLIGHT_POWER_ID);
        float rimlightPower = mat.GetFloat(ToonConstants.SHADER_PROP_RIMLIGHT_POWER_ID);
        float apRimlightPower = mat.GetFloat(ToonConstants.SHADER_PROP_AP_RIMLIGHT_POWER_ID);
        
        Vector4 highlightAndRimMathParams = new Vector4(
            Mathf.Pow(2, Mathf.Lerp(11f, 1f, highlightPower)), //exp2(lerp(11, 1, _HighColor_Power))
            1.0f - Mathf.Pow(Mathf.Abs(highlightPower), 5f), //1.0 - pow(abs(_HighColor_Power), 5);
            Mathf.Pow(2f, Mathf.Lerp(3f, 0f, rimlightPower)), //exp2(lerp(3, 0, _RimLight_Power));
            Mathf.Pow(2f, Mathf.Lerp(3f, 0f, apRimlightPower)) //exp2(lerp(3, 0, _Ap_RimLight_Power));
        );
        mat.SetVector(ToonConstants.SHADER_PROP_HIGHLIGHT_RIM_MATH_ID, highlightAndRimMathParams);
    }
    
//----------------------------------------------------------------------------------------------------------------------    
    internal static Dictionary<string, MaterialPropertyValue> CaptureMaterialValues(Material mat) {
        
        Dictionary<string, MaterialPropertyValue> store = new Dictionary<string, MaterialPropertyValue>();
        Shader shader = mat.shader;
        int count = shader.GetPropertyCount();
        for (int i = 0; i < count; i++) {
            string name = shader.GetPropertyName(i);
            
            if (!mat.HasProperty(name)) 
                continue;
            MaterialPropertyValue value = MaterialPropertyValue.FromMaterial(mat, i);
            store[name] = value;
        }
        return store;
    }

}

}