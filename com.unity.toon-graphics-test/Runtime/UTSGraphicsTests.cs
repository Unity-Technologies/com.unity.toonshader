using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;


namespace Unity.ToonShader.GraphicsTest
{
#if UNITY_EDITOR
public class UTSGraphicsTestsXR {

    [UnityTest]
    [UseGraphicsTestCases(UTSGraphicsTestConstants.ReferenceImagePath)]
    [Timeout(3600000)] //1 hour
    public IEnumerator Run(GraphicsTestCase testCase) {
        
        //[TODO-sin: 2025-7-18] ECS projects were never tested with XR, and currently they don't support XR.
        string projectName = Path.GetFileName(Path.GetDirectoryName(UnityEngine.Application.dataPath));
        if (!string.IsNullOrEmpty(projectName) && projectName.Contains("ECS")) {
            Assert.Ignore();
        }

        string sceneFileName = Path.GetFileNameWithoutExtension(testCase.ScenePath);

#if UTS_TEST_USE_HDRP && UNITY_STANDALONE_OSX 

        //[TODO-sin: 2025-12-1] ToonTesselation did not work on Metal in HDRP
        if (sceneFileName.Contains("Tessellation")) {
            Assert.Ignore();
        }
#endif //UTS_TEST_USE_HDRP && UNITY_STANDALONE_OSX 

        //Ignore XR tests for 2D scenes
        if (sceneFileName.EndsWith("2D")) {
            Assert.Ignore();
        }
        

        //Enable XR
        XRUtility.EnableXRInEditor();

        //Rendering both eyes in XR requires backbuffer, which depends on the game view resolution
        object gameViewSizeObj = UnityEditor.TestTools.Graphics.GameViewSize.SetCustomSize(1920, 1080);
        Assert.IsNotNull(gameViewSizeObj, "Failed to add custom game view size for XR tests.");
        UnityEditor.TestTools.Graphics.GameViewSize.SelectSize(gameViewSizeObj);

        string loadedXRDevice = UseGraphicsTestCasesAttribute.LoadedXRDevice;

        //Manually load the reference image for XR. Ex: URP/Linear/WindowsEditor/Vulkan/None/AngelRing.png
        Assert.IsNotNull(testCase.ReferenceImage);
        string imagePath = AssetDatabase.GetAssetPath(testCase.ReferenceImage);
        string imageFileName = Path.GetFileName(imagePath);
        string imageFolderName = Path.GetDirectoryName(Path.GetDirectoryName(imagePath));
        Assert.IsNotNull(imageFolderName);
        string xrImagePath = Path.Combine(imageFolderName, loadedXRDevice,imageFileName);
        testCase.ReferenceImagePathLog = xrImagePath;
        Assert.IsTrue(File.Exists(xrImagePath),$"XR Reference image not found at: {xrImagePath}");
        testCase.ReferenceImage = AssetDatabase.LoadAssetAtPath<Texture2D>(xrImagePath);

        yield return UTSGraphicsTests.RunInternal(testCase, isXR:true);

        XRUtility.DisableXR();
    }

}

#endif //UNITY_EDITOR

public class UTSGraphicsTestsNonXR  {
    [UnityTest]
    [UseGraphicsTestCases(UTSGraphicsTestConstants.ReferenceImagePath)]
    [Timeout(3600000)] //1 hour
    public IEnumerator Run(GraphicsTestCase testCase) {
        yield return UTSGraphicsTests.RunInternal(testCase);
    }
}

//----------------------------------------------------------------------------------------------------------------------

    public static class UTSGraphicsTests {
        internal static IEnumerator RunInternal(GraphicsTestCase testCase, bool isXR = false) {
            SceneManager.LoadScene(testCase.ScenePath);

            // Always wait one frame for scene load
            yield return null;

            Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            UTSGraphicsTestSettings settings = Object.FindFirstObjectByType<UTSGraphicsTestSettings>();
            Assert.IsNotNull(settings, "Invalid test scene, couldn't find UTS_GraphicsTestSettings");

            UTSGraphicsTestSettingsSO settingsSO = settings.SO;
            Assert.IsNotNull(settingsSO);

            ImageComparisonSettings imageComparisonSettings = settingsSO.ImageComparisonSettings;
            Assert.IsNotNull(imageComparisonSettings);


            if (isXR) {
                imageComparisonSettings.UseBackBuffer = true; //results using both eyes need backbuffer
            }


            int waitFrames = settingsSO.WaitFrames;

            if (imageComparisonSettings.UseBackBuffer && settingsSO.WaitFrames < 1) {
                waitFrames = 1;
            }


            for (int i = 0; i < waitFrames; i++)
                yield return new WaitForEndOfFrame();

            ImageAssert.AreEqual(testCase.ReferenceImage, mainCamera,
                imageComparisonSettings, testCase.ReferenceImagePathLog);

            // Does it allocate memory when it renders what's on the main camera?
            bool allocatesMemory = false;

            if (settings == null || settings.CheckMemoryAllocation)
            {
                try
                {
                    ImageAssert.AllocatesMemory(mainCamera, imageComparisonSettings);
                }
                catch (AssertionException)
                {
                    allocatesMemory = true;
                }
                if (allocatesMemory)
                    Assert.Fail("Allocated memory when rendering what is on main camera");
            }
        }
    }

}
