
using System.IO;
using Unity.Rendering.Toon;

namespace UnityEditor.Rendering.Toon {

internal static class ToonEditorConstants {

    internal const int CUR_MATERIAL_VERSION = (int) ToonMaterialVersion.Initial;
    
    internal static readonly string PACKAGE_PATH = Path.Combine("Packages", ToonConstants.PACKAGE_NAME).Replace('\\','/');
    
    
    internal static readonly string TOON_SHADER_PATH = 
        Path.Combine(PACKAGE_PATH,"Runtime/Shaders/UnityToon.shader").Replace('\\','/');
    internal static readonly string TOON_TESS_SHADER_PATH = 
        Path.Combine(PACKAGE_PATH,"Runtime/Shaders/UnityToonTessellation.shader").Replace('\\','/');
    

}

} //end namespace