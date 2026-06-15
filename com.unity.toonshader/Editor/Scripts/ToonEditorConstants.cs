
using System.IO;
using Unity.Rendering.Toon;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal static class ToonEditorConstants {

    internal const int CUR_MATERIAL_VERSION = (int) ToonMaterialVersion.Initial;
    
    internal static readonly string PACKAGE_PATH = Path.Combine("Packages", ToonConstants.PACKAGE_NAME).Replace('\\','/');
    
    
    internal static readonly string TOON_SHADER_PATH = 
        Path.Combine(PACKAGE_PATH,"Runtime/Shaders/UnityToon.shader").Replace('\\','/');
    internal static readonly string TOON_TESS_SHADER_PATH =
        Path.Combine(PACKAGE_PATH,"Runtime/Shaders/UnityToonTessellation.shader").Replace('\\','/');

    internal static readonly GUIContent[] STENCIL_COMP_ENUMS   = ToonEnumUtility.ToInspectorNamesAsGUIContent(typeof(UnityEngine.Rendering.CompareFunction));
    internal static readonly int[]        STENCIL_COMP_VALUES = ToonEnumUtility.ToIntValues(typeof(UnityEngine.Rendering.CompareFunction));
    internal static readonly GUIContent[] STENCIL_OP_ENUMS     = ToonEnumUtility.ToInspectorNamesAsGUIContent(typeof(UnityEngine.Rendering.StencilOp));
    internal static readonly int[]        STENCIL_OP_VALUES   = ToonEnumUtility.ToIntValues(typeof(UnityEngine.Rendering.StencilOp));


}

} //end namespace