using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.ToonShader.GraphicsTest;
using UnityEditor;


namespace Tests
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
        
        //Unity.ToonShader.GraphicsTest.SetupUTSGraphicsXRTestCases.Setup();
        yield return UTS_GraphicsTests.RunInternal(testCase, isXR:true);
        
        XRUtility.DisableXR();
    }
   
}

#endif //UNITY_EDITOR

public class UTSGraphicsTestsNonXR  {
    [UnityTest]
    [UseGraphicsTestCases(UTSGraphicsTestConstants.ReferenceImagePath)]
    [Timeout(3600000)] //1 hour
    public IEnumerator Run(GraphicsTestCase testCase) {
        //[TODO-sin: 2025-7-2] Hack for now to disable XR for non-Stereo projects
        string projectName = Path.GetFileName(Path.GetDirectoryName(UnityEngine.Application.dataPath));
        if (!string.IsNullOrEmpty(projectName) && !projectName.Contains("Stereo")) {
            XRUtility.DisableXR();
        }
        
        yield return UTS_GraphicsTests.RunInternal(testCase);
    }
} 

//----------------------------------------------------------------------------------------------------------------------

    public class UTS_GraphicsTests {
        internal static IEnumerator RunInternal(GraphicsTestCase testCase, bool isXR = false) {
            SceneManager.LoadScene(testCase.ScenePath);

            // Always wait one frame for scene load
            yield return null;

            var cameras = GameObject.FindGameObjectsWithTag("MainCamera").Select(x => x.GetComponent<Camera>());
            UTS_GraphicsTestSettings settings = Object.FindFirstObjectByType<UTS_GraphicsTestSettings>();
            Assert.IsNotNull(settings, "Invalid test scene, couldn't find UTS_GraphicsTestSettings");

            if (isXR) {
                settings.ImageComparisonSettings.UseBackBuffer = true; //results using both eyes need backbuffer 

                //[TODO-sin: 2025-7-9] Hack for now. The resolution will be set to this later
                settings.ImageComparisonSettings.ImageResolution = ImageComparisonSettings.Resolution.w1920h1080;
            }

            
            int waitFrames = settings.WaitFrames;

            if (settings.ImageComparisonSettings.UseBackBuffer && settings.WaitFrames < 1)
            {
                waitFrames = 1;
            }


            for (int i = 0; i < waitFrames; i++)
                yield return new WaitForEndOfFrame();

            ImageAssert.AreEqual(testCase.ReferenceImage, cameras.Where(x => x != null), 
                settings.ImageComparisonSettings, testCase.ReferenceImagePathLog);

            // Does it allocate memory when it renders what's on the main camera?
            bool allocatesMemory = false;
            var mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            if (settings == null || settings.CheckMemoryAllocation)
            {
                try
                {
                    ImageAssert.AllocatesMemory(mainCamera, settings.ImageComparisonSettings);
                }
                catch (AssertionException)
                {
                    allocatesMemory = true;
                }
                if (allocatesMemory)
                    Assert.Fail("Allocated memory when rendering what is on main camera");
            }
        }

        public static Texture2D LoadPNG(string filePath)
        {

            Texture2D tex2D = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex2D = new Texture2D(2, 2);
                tex2D.LoadImage(fileData);
            }
            return tex2D;
        }
    }


}
