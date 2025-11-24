using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
internal class ToonCameraViewController : MonoBehaviour {


    void Update() {

        Vector3 dir = transform.forward;
        
        foreach (Material mat in m_materials) {
            if (mat == null) continue;
            mat.SetVector(Toon3Das2DConstants.SHADER_PROP_VIEW_DIRECTION, dir);
        }
    }


//----------------------------------------------------------------------------------------------------------------------
    [SerializeField] private List<Material> m_materials = new List<Material>();

}