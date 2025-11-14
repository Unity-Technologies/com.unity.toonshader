using Unity.Rendering.Toon;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal class ToonKeywordPostprocessor : AssetPostprocessor {

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths) {
        foreach (string path in importedAssets) {

            // Only process materials
            if (!path.EndsWith(".mat"))
                continue;

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null || mat.shader == null)
                continue;

            string shaderPath = AssetDatabase.GetAssetPath(mat.shader);
            
            if (!shaderPath.StartsWith(ToonConstants.PACKAGE_PATH))
                continue;

Debug.Log("Refreshing keyword");

            ToonKeywordUtility.SetRenderPipelineKeyword(mat);
        }
    }
}

}