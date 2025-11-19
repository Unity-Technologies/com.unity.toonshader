using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Light))]
internal class ToonDirectionalLightController : MonoBehaviour {
    void OnEnable() {
        m_light = GetComponent<Light>();
    }


    void Update() {
        if (m_light.type != LightType.Directional) {
            return;
        }

        Vector3 lightDir = -m_light.transform.forward; 
        Color lightColor = m_light.color;
        float lightIntensity = m_light.intensity;
        
        foreach (Material mat in m_materials) {
            if (mat == null) continue;
            mat.SetVector(Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Direction, lightDir);
            mat.SetColor(Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Color, lightColor);
            mat.SetFloat(Toon3Das2DConstants.ShaderPropUnlit_DirectionalLight_Intensity, lightIntensity);
        }
    }


//----------------------------------------------------------------------------------------------------------------------
    [SerializeField] private List<Material> m_materials = new List<Material>();

//----------------------------------------------------------------------------------------------------------------------
    Light m_light;
}