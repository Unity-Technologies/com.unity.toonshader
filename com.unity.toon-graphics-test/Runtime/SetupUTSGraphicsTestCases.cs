using UnityEngine.TestTools;

namespace Unity.ToonShader.GraphicsTest {
    
public class SetupUTSGraphicsTestCases : IPrebuildSetup {
    public void Setup() {
        XRUtility.DisableXR();
    }
}
    
} //end namespace
