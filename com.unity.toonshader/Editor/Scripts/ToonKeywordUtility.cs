using Unity.Rendering.Toon;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal static class ToonKeywordUtility {

    public static void SetRenderPipelineKeyword(Material material) {
        material.DisableKeyword(ToonConstants.SHADER_KEYWORD_URP);
        material.DisableKeyword(ToonConstants.SHADER_KEYWORD_HDRP);

#if HDRP_IS_INSTALLED_FOR_UTS
        material.EnableKeyword(ToonConstants.SHADER_KEYWORD_HDRP);
#elif URP_IS_INSTALLED_FOR_UTS
        material.EnableKeyword(ToonConstants.SHADER_KEYWORD_URP);
#endif
        EditorUtility.SetDirty(material);
    }
}

}