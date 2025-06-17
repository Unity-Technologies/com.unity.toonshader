using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.U2D.Animation.Sample
{
    // Ensure class initializer is called whenever scripts recompile
    [InitializeOnLoad]
    internal static class BuildAssetBundle
    {
        const string k_AssetBundleName = "2DAnimationSampleAssetBundles";

        // Register an event handler when the class is initialized
        static BuildAssetBundle()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChange;
        }

        static void PlayModeStateChange(PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingEditMode)
                BuildAssetBundles();
        }

        static void BuildAssetBundles()
        {
#if ASSETBUNDLE_ENABLED
            var assetBundleDirectory = Path.Combine(Application.streamingAssetsPath, k_AssetBundleName);
            if (!Directory.Exists(assetBundleDirectory))
                Directory.CreateDirectory(assetBundleDirectory);
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
#endif
        }
    }
}
