
using UnityEngine;

namespace Unity.Rendering.Toon {

internal static class Toon3Das2DMaterialUtility {

    internal static bool IsOutlineEnabled(Material m) {
        return m.GetShaderPassEnabled(ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE);
    }

    internal static void EnableOutline(Material m, bool enabled) {
        m.SetShaderPassEnabled(ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE, enabled);
    }
}

}