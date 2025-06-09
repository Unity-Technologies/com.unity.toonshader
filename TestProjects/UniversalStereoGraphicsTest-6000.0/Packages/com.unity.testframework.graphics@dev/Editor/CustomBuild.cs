using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools.Graphics;

public static class CustomBuild
{

#if !UNITY_2023_1_OR_NEWER
    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Android (GLES2)", false)]
    static void BuildAndroidGLES2()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Build", "", "");
        BuildTarget buildTarget = BuildTarget.Android;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.OpenGLES2 };
        BuildScenes(path, graphicsAPIs[0].ToString(), buildTarget);
    }
#endif

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Android (Vulkan)", false)]
    static void BuildAndroidVulkan()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Build", "", "");
        BuildTarget buildTarget = BuildTarget.Android;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.Vulkan };
        BuildScenes(path, graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Android (GLES3)", false)]
    static void BuildAndroidGLES3()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Build", "", "");
        BuildTarget buildTarget = BuildTarget.Android;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.OpenGLES3 };
        BuildScenes(path, graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../iOS (Metal)", false)]
    static void BuildiOSMetal()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Build", "", "");
        BuildTarget buildTarget = BuildTarget.iOS;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.Metal };
        BuildScenes(path, graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../MacOS (Metal)", false)]
    static void BuildOSXMetal()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneOSX;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.Metal };
        BuildScenes(".", graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Windows (Vulkan)", false)]
    static void BuildWindowsVulkan()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.Vulkan };
        BuildScenes(".", graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Linux (Vulkan)", false)]
    static void BuildLinuxVulkan()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneLinux64;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.Vulkan };
        BuildScenes(".", graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Linux (GLCore)", false)]
    static void BuildLinuxOpenGLCore()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneLinux64;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.OpenGLCore };
        BuildScenes(".", graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Windows (DX11)", false)]
    static void BuildWindowsDX11()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.Direct3D11 };
        BuildScenes(".", graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Windows (DX12)", false)]
    static void BuildWindowsDX12()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.Direct3D12 };
        BuildScenes(".", graphicsAPIs[0].ToString(), buildTarget);
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Linux (Headless Simulation | Vulkan)", false)]
    static void BuildLinuxHeadlessSimulation()
    {
        //PlayerSettings.colorSpace = ColorSpace.Linear;
        BuildTarget buildTarget = BuildTarget.LinuxHeadlessSimulation;
        // BuildOptions buildOptions = BuildOptions.None;

        GraphicsDeviceType[] graphicsAPIs = { GraphicsDeviceType.Vulkan };
        BuildScenes(".", graphicsAPIs[0].ToString(), buildTarget);
    }

    public static void BuildScenes(string path, string name, BuildTarget buildTarget, bool buildPlayer=true)
    {
        string buildName = string.Format("{0}{1}", "TestScenes", name);

        PlayerSettings.productName = buildName;
        PlayerSettings.applicationIdentifier = string.Format("com.unity.{0}", buildName);
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenHeight = ImageAssert.kBackBufferHeight;
        PlayerSettings.defaultScreenWidth = ImageAssert.kBackBufferWidth;

        if (buildPlayer)
        {
            List<string> scenesToBuild = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                if (scene.enabled)
                    scenesToBuild.Add(scene.path);

            string suffix = (buildTarget == BuildTarget.Android) ? ".apk" : "";

            BuildOptions buildOptions = BuildOptions.None;

            BuildPipeline.BuildPlayer(scenesToBuild.ToArray(), string.Format("{0}/{1}{2}", path, buildName, suffix), buildTarget, buildOptions);
        }
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Color Space - Linear", false, 401)]
    static void SetColorSpaceLinear()
    {
        PlayerSettings.colorSpace = ColorSpace.Linear;
    }

    [MenuItem("Tools/Graphics Test Framework/Custom Build.../Color Space - Gamma", false, 401)]
    static void SetColorSpaceGamma()
    {
        PlayerSettings.colorSpace = ColorSpace.Gamma;
    }
}
