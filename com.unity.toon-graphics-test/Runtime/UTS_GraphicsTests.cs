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
public class UTS_GraphicsTestsXR {
    
    [UnityTest]
    [UseGraphicsTestCases(UTSGraphicsTestConstants.ReferenceImagePath)]
    [Timeout(3600000)] //1 hour
    public IEnumerator Run(GraphicsTestCase testCase) {

        //Enable XR
        XRUtility.EnableXR();
        
#if UNITY_EDITOR
        GameViewUtility.AddAndSelectCustomSize(
            GameViewUtility.GameViewSizeType.FixedResolution,
            GameViewSizeGroupType.Standalone,
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
        
        //Unity.ToonShader.GraphicsTest.SetupUTSGraphicsXRTestCases.Setup();
        yield return UTS_GraphicsTests.RunInternal(testCase, isXR:true);
        
        XRUtility.DisableXR();
    }
   
} 
public class UTS_GraphicsTestsNonXR  {
#if UTS_TEST_USE_HDRP        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/HDRP";
#elif UTS_TEST_USE_URP
    private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/URP";
#else        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/Built-In";
#endif
    

    [UnityTest]
    [UseGraphicsTestCases(ReferenceImagePath)]
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

            ImageAssert.AreEqual(testCase.ReferenceImage, cameras.Where(x => x != null), settings.ImageComparisonSettings);

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
