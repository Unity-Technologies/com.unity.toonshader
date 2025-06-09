using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System.Collections.Generic;

namespace UnityEditor.TestTools.Graphics
{
    [System.Serializable]
    public class TestFilterConfig
    {
        public SceneAsset FilteredScene;
        public SceneAsset[] FilteredScenes;
        public ColorSpace ColorSpace = ColorSpace.Uninitialized;
        public BuildTarget BuildPlatform = BuildTarget.NoTarget;
        public GraphicsDeviceType GraphicsDevice = GraphicsDeviceType.Null;
        public Architecture Architecture = Architecture.Unknown;
        public string XrSdk;
        public StereoRenderingModeFlags StereoModes;
        public string Reason;
    }

    public enum StereoRenderingModeFlags
    {
        None = 0,
        MultiPass = 1,
        SinglePass = 2,
        Instancing = 4
    }

    public enum Architecture
    {
        Unknown = 0,
        ARM = 1,
        ARM64 = 2,
        x86 = 3,
        x86_64 = 4,
    }
}
