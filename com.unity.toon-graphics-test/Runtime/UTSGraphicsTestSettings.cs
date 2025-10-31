using UnityEngine.TestTools.Graphics;

namespace Unity.ToonShader.GraphicsTest
{
    public class UTSGraphicsTestSettings : GraphicsTestSettings {
        public UTSGraphicsTestSettingsSO SO;
        
        public int WaitFrames = 0;
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        public bool CheckMemoryAllocation = false;
#else
        public bool CheckMemoryAllocation = true;
#endif //#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

        public UTSGraphicsTestSettings()
        {
            ImageComparisonSettings.TargetWidth = 960;
            ImageComparisonSettings.TargetHeight = 540;
            ImageComparisonSettings.AverageCorrectnessThreshold = 0.005f;
            ImageComparisonSettings.PerPixelCorrectnessThreshold = 0.001f;
            ImageComparisonSettings.UseHDR = false;
            ImageComparisonSettings.UseBackBuffer = false;
        }
    }
}