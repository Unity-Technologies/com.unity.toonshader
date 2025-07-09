using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Graphics;
using System.IO;
using Tests;
using UnityEditor;
using UnityEngine.TestTools;


namespace Unity.ToonShader.GraphicsTest.Editor
{
public class UTSGraphicsTestsXR {
    
    [UnityTest]
    [UseGraphicsTestCases(UTSGraphicsTestConstants.ReferenceImagePath)]
    [Timeout(3600000)] //1 hour
    public IEnumerator Run(GraphicsTestCase testCase) {

        //Enable XR
        XRUtility.EnableXR();
        
#if UNITY_EDITOR
        GameViewUtility.FindAndSelectSize(
            1920, 1080, "Full HD (1920x1080)"
        );
#endif
        string loadedXRDevice = UseGraphicsTestCasesAttribute.LoadedXRDevice;
        
        //Manually load the reference image for XR. Ex: URP/Linear/WindowsEditor/Vulkan/None/AngelRing.png
        Assert.IsNotNull(testCase.ReferenceImage);
        string imagePath = AssetDatabase.GetAssetPath(testCase.ReferenceImage);
        string imageFileName = Path.GetFileName(imagePath);
        string imageFolderName = Path.GetDirectoryName(Path.GetDirectoryName(imagePath));
        Assert.IsNotNull(imageFolderName);
        string xrImagePath = Path.Combine(imageFolderName, loadedXRDevice,imageFileName);
        testCase.ReferenceImagePathLog = xrImagePath;
        Assert.IsTrue(File.Exists(xrImagePath));
        testCase.ReferenceImage = AssetDatabase.LoadAssetAtPath<Texture2D>(xrImagePath);

        //Run
        yield return UTS_GraphicsTests.RunInternal(testCase, isXR:true);
        
        XRUtility.DisableXR();
    }
   
}

} //end namespace 
