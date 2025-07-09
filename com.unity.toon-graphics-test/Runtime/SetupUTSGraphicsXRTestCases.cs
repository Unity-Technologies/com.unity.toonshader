#if UNITY_EDITOR
using UnityEditor;
#endif

using Tests;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.ToonShader.GraphicsTest {
    
public static class SetupUTSGraphicsXRTestCases {
    public static void Setup() {
        
        Debug.Log("Setup XR: "+ UTS_GraphicsTests.number++);
        
        //Enable XR
        XRUtility.EnableXR();
        
#if UNITY_EDITOR
        GameViewUtility.AddAndSelectCustomSize(
            GameViewUtility.GameViewSizeType.FixedResolution,
            GameViewSizeGroupType.Standalone,
            2*1920, 1080, "XR Full HD (3840x1080)"
        );
#endif        
    }
    
    public static void Cleanup() {
        Debug.Log("Cleanup XR: "+ UTS_GraphicsTests.number++);
        XRUtility.DisableXR();
    }
}
    
} //end namespace
