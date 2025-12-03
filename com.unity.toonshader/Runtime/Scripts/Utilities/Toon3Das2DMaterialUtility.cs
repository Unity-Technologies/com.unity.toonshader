
using UnityEngine;

namespace Unity.Rendering.Toon {

internal static class Toon3Das2DMaterialUtility {

    internal static bool IsOutlineEnabled(Material m) {
        return m.GetShaderPassEnabled(ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE);
    }

    internal static void EnableOutline(Material m, bool enabled) {
        m.SetShaderPassEnabled(ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE, enabled);
    }

    internal static void SetOutlineMode(Material[] mats, ToonOutlineMode outlineMode) {

        foreach (Material m in mats) {
            m.SetInteger(ToonConstants.SHADER_PROP_OUTLINE_MODE_ID, (int) outlineMode);
        }
            
        const string OUTLINE_NORMAL_KEYWORD = "TOON_OUTLINE_NORMAL";
        const string OUTLINE_POSITION_KEYWORD = "TOON_OUTLINE_POS";

        switch (outlineMode) {
            case ToonOutlineMode.NormalDirection:
                foreach (Material m in mats) {
                    m.EnableKeyword(OUTLINE_NORMAL_KEYWORD);
                    m.DisableKeyword(OUTLINE_POSITION_KEYWORD);
                }

                break;
            case ToonOutlineMode.PositionScaling:
                foreach (Material m in mats) {
                    m.DisableKeyword(OUTLINE_NORMAL_KEYWORD);
                    m.EnableKeyword(OUTLINE_POSITION_KEYWORD);
                }

                break;
        }
            
    }
    
}

}