using System.Collections.Generic;
using UnityEngine;

namespace Unity.Rendering.Toon {

internal static class ToonMaterialUtility {
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