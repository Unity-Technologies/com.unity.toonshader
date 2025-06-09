using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class SetupProject
{
    public static void ApplySettings()
    {
        UnityEditor.TestTools.Graphics.SetupProject.ApplySettings();
    }

}

namespace UnityEditor.TestTools.Graphics
{
    public static class SetupProject
    {
        public static Dictionary<string, Action> Options = new Dictionary<string, Action>
        {
            { "gamma", () => PlayerSettings.colorSpace = ColorSpace.Gamma },
            { "linear", () => PlayerSettings.colorSpace = ColorSpace.Linear },
            { "glcore", () => SetGraphicsAPI(GraphicsDeviceType.OpenGLCore) },
            { "d3d11", () => SetGraphicsAPI(GraphicsDeviceType.Direct3D11) },
            { "d3d12", () => SetGraphicsAPI(GraphicsDeviceType.Direct3D12) },
            { "metal", () => SetGraphicsAPI(GraphicsDeviceType.Metal) },
            { "vulkan", () => SetGraphicsAPI(GraphicsDeviceType.Vulkan) },
            { "gles3", () => SetGraphicsAPI(GraphicsDeviceType.OpenGLES3) },
#if !UNITY_2023_1_OR_NEWER
            { "gles2", () => SetGraphicsAPI(GraphicsDeviceType.OpenGLES2) },
#endif
            { "ps4", () => SetGraphicsAPI(GraphicsDeviceType.PlayStation4) },
            { "ps5", () => SetGraphicsAPI(GraphicsDeviceType.PlayStation5) },
            { "ps5nggc", () => SetGraphicsAPI(GraphicsDeviceType.PlayStation5NGGC) },
            { "xb1d3d11", () => SetGraphicsAPI(GraphicsDeviceType.XboxOne) },
            { "xb1d3d12", () => SetGraphicsAPI(GraphicsDeviceType.XboxOneD3D12) },
            { "gamecorexboxone", () => SetGraphicsAPI(GraphicsDeviceType.GameCoreXboxOne) },
            { "gamecorexboxseries", () => SetGraphicsAPI(GraphicsDeviceType.GameCoreXboxSeries) },
            { "switch", () => SetGraphicsAPI(GraphicsDeviceType.Switch) },
#if UNITY_2023_2_OR_NEWER
            { "webgpu", () => SetGraphicsAPI(GraphicsDeviceType.WebGPU) }
#endif
        };

        public static void ApplySettings()
        {
            var args = Environment.GetCommandLineArgs();
            string apiName = "";
            foreach (var arg in args)
            {
                Action action;
                if (Options.TryGetValue(arg, out action))
                {
                    apiName = arg;
                    action();
                }
            }

            CustomBuild.BuildScenes(".", apiName, EditorUserBuildSettings.activeBuildTarget, false);
        }
        static void SetGraphicsAPI(GraphicsDeviceType api)
        {
            var currentTarget = EditorUserBuildSettings.activeBuildTarget;
            PlayerSettings.SetGraphicsAPIs(currentTarget, new[] { api });
        }
    }
}
