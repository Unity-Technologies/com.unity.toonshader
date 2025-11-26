using System.Collections.Generic;
using Unity.Rendering.Toon;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal static class ToonMaterialEditorUtility {
    internal static void ApplyRenderPipelineKeyword(Material m) {
#if (HDRP_IS_INSTALLED_FOR_UTS || URP_IS_INSTALLED_FOR_UTS)
        m.DisableKeyword(ToonConstants.SHADER_KEYWORD_RP_BUILTIN);
#else
        m.EnableKeyword(ToonConstants.SHADER_KEYWORD_RP_BUILTIN);
#endif
    }
    
    
    
}
}