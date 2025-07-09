using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.SceneManagement;
using System.IO;


namespace Tests
{
public class UTS_GraphicsTestsXR {
#if UTS_TEST_USE_HDRP        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/HDRP";
#elif UTS_TEST_USE_URP
    private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/URP";
#else        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/Built-In";
#endif

    [UnityTest]
    // [PrebuildSetup(typeof(Unity.ToonShader.GraphicsTest.SetupUTSGraphicsXRTestCases))]
    // [PostBuildCleanup(typeof(Unity.ToonShader.GraphicsTest.SetupUTSGraphicsXRTestCases))]
    [UseGraphicsTestCases(ReferenceImagePath)]
    [Timeout(3600000)] //1 hour
    public IEnumerator Run(GraphicsTestCase testCase) {
        Unity.ToonShader.GraphicsTest.SetupUTSGraphicsXRTestCases.Setup();
        Debug.Log("RunXR Number: "+ UTS_GraphicsTests.number++);
        yield return UTS_GraphicsTests.RunInternal(testCase);
        Unity.ToonShader.GraphicsTest.SetupUTSGraphicsXRTestCases.Cleanup();
    }
    
} 
public class UTS_GraphicsTestsNonXR {
#if UTS_TEST_USE_HDRP        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/HDRP";
#elif UTS_TEST_USE_URP
    private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/URP";
#else        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/Built-In";
#endif
    
    [UnityTest]
    // [PrebuildSetup(typeof(Unity.ToonShader.GraphicsTest.SetupUTSGraphicsNonXRTestCases))]
    // [PostBuildCleanup(typeof(Unity.ToonShader.GraphicsTest.SetupUTSGraphicsNonXRTestCases))]
    [UseGraphicsTestCases(ReferenceImagePath)]
    [Timeout(3600000)] //1 hour
    public IEnumerator Run(GraphicsTestCase testCase) {
        Unity.ToonShader.GraphicsTest.SetupUTSGraphicsNonXRTestCases.Setup();
        Debug.Log("RunNonXR Number: "+ UTS_GraphicsTests.number++);
        yield return UTS_GraphicsTests.RunInternal(testCase);
        Unity.ToonShader.GraphicsTest.SetupUTSGraphicsNonXRTestCases.Cleanup();
    }
} 
    

    public class UTS_GraphicsTests {

        public static int number = 0;
        
#if UTS_TEST_USE_HDRP        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/HDRP";
#elif UTS_TEST_USE_URP
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/URP";
#else        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/Built-In";
#endif




        
        internal static IEnumerator RunInternal(GraphicsTestCase testCase) {
            SceneManager.LoadScene(testCase.ScenePath);

            // Always wait one frame for scene load
            yield return null;

            var cameras = GameObject.FindGameObjectsWithTag("MainCamera").Select(x => x.GetComponent<Camera>());
            UTS_GraphicsTestSettings settings = Object.FindFirstObjectByType<UTS_GraphicsTestSettings>();
            Assert.IsNotNull(settings, "Invalid test scene, couldn't find UTS_GraphicsTestSettings");

            
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
