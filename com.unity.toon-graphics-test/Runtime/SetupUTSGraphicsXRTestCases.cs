using UnityEngine.TestTools;

namespace Unity.ToonShader.GraphicsTest {
    
public class SetupUTSGraphicsXRTestCases : IPrebuildSetup, IPostBuildCleanup {
    public void Setup() {
        
        //Enable XR
        XRUtility.EnableXR();
        
        GameViewUtility.AddAndSelectCustomSize(
            GameViewUtility.GameViewSizeType.FixedResolution,
            GameViewSizeGroupType.Standalone,
            1920, 1080, "Full HD (1920x1080)"
        );
    }
    
    public void Cleanup() {
        XRUtility.DisableXR();
    }
}
    
} //end namespace
