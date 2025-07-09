using UnityEngine.TestTools;

namespace Unity.ToonShader.GraphicsTest {
    
public class SetupUTSGraphicsXRTestCases : IPrebuildSetup, IPostBuildCleanup {
    public void Setup() {
        
        //Enable XR
        XRUtility.EnableXR();
    }
    
    public void Cleanup() {
        XRUtility.DisableXR();
    }
}
    
} //end namespace
