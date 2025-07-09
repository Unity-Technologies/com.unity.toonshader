using System.IO;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.ToonShader.GraphicsTest {
    
public static class SetupUTSGraphicsNonXRTestCases  {
    public static void Setup() {
        Debug.Log("Setup Non-XR: "+ UTS_GraphicsTests.number++);
        
        //[TODO-sin: 2025-7-2] Hack for now to disable XR for non-Stereo projects
        string projectName = Path.GetFileName(Path.GetDirectoryName(UnityEngine.Application.dataPath));
        if (!string.IsNullOrEmpty(projectName) && !projectName.Contains("Stereo")) {
            XRUtility.DisableXR();
        }
    }
    
    public static void Cleanup() {
        Debug.Log("Cleanup Non-XR: "+ UTS_GraphicsTests.number++);
        
    }
}
    
} //end namespace
