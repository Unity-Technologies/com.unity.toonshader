using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Rendering.Toon {

[ExecuteAlways]
internal class ToonCameraViewController : MonoBehaviour {
    private void OnEnable() {
        m_transform = transform;
    }

    void Update() {
        Vector3 pos = m_transform.position;
        foreach (Material mat in m_materials) {
            if (mat == null) continue;
            mat.SetVector(ToonConstants.SHADER_PROP_DIRECTIONAL_LIGHT_VIEW_POSITION, pos);
        }
    }


//----------------------------------------------------------------------------------------------------------------------
    [SerializeField] private List<Material> m_materials = new List<Material>();

    private Transform m_transform;
}

}