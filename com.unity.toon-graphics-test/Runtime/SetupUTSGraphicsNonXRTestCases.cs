using System.IO;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.ToonShader.GraphicsTest {
    
public static class SetupUTSGraphicsNonXRTestCases  {
    public static void Setup() {
        //[TODO-sin: 2025-7-2] Hack for now to disable XR for non-Stereo projects
        string projectName = Path.GetFileName(Path.GetDirectoryName(UnityEngine.Application.dataPath));
        if (!string.IsNullOrEmpty(projectName) && !projectName.Contains("Stereo")) {
            XRUtility.DisableXR();
        }
    }
    
    public static void Cleanup() {
        
    }
}
    
} //end namespace
