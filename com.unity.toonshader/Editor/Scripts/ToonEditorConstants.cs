
using System.IO;
using Unity.Rendering.Toon;

namespace UnityEditor.Rendering.Toon {

internal static class ToonEditorConstants {

    internal const int CUR_MATERIAL_VERSION = (int) ToonMaterialVersion.Initial;
    
    internal static readonly string PACKAGE_PATH = Path.Combine("Packages", ToonConstants.PACKAGE_NAME).Replace('\\','/');

}

} //end namespace