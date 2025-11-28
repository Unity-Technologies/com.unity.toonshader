using System.Collections.Generic;
using UnityEngine;

namespace Unity.Rendering.Toon {

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

        Vector3 lightDir = m_light.transform.forward;
        Color lightColor = m_light.color;
        float lightIntensity = m_light.intensity;

        foreach (Material mat in m_materials) {
            if (mat == null) continue;
            mat.SetVector(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_DIRECTION, lightDir);
            mat.SetColor(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_COLOR, lightColor);
            mat.SetFloat(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_INTENSITY, lightIntensity);
        }
    }


//----------------------------------------------------------------------------------------------------------------------
    [SerializeField] private List<Material> m_materials = new List<Material>();

//----------------------------------------------------------------------------------------------------------------------
    Light m_light;
}

}