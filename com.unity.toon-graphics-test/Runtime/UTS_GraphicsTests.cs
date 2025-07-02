﻿using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Toon.GraphicsTest;

namespace Tests
{
    public class UTS_GraphicsTests
    {
#if UTS_TEST_USE_HDRP        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/HDRP";
#elif UTS_TEST_USE_URP
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/URP";
#else        
        private const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/Built-In";
#endif
        
        [UnityTest]
        [PrebuildSetup("SetupGraphicsTestCases")]
        [UseGraphicsTestCases(ReferenceImagePath)]
        [Timeout(3600000)] //1 hour
        public IEnumerator Run(GraphicsTestCase testCase)
        {

            SceneManager.LoadScene(testCase.ScenePath);

            // Always wait one frame for scene load
            yield return null;

            var cameras = GameObject.FindGameObjectsWithTag("MainCamera").Select(x => x.GetComponent<Camera>());
            var settings = Object.FindObjectOfType<UTS_GraphicsTestSettings>();
            Assert.IsNotNull(settings, "Invalid test scene, couldn't find UTS_GraphicsTestSettings");

            Scene scene = SceneManager.GetActiveScene();


            if (scene.name.Length > (3+ 4) && scene.name.Substring(3, 4).Equals("_xr_"))
            {
#if ENABLE_VR && ENABLE_VR_MODULE
            Assume.That((Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer), "Stereo tests do not run on MacOSX.");

            XRSettings.LoadDeviceByName("MockHMD");
            yield return null;

            XRSettings.enabled = true;
            yield return null;

            XRSettings.gameViewRenderMode = GameViewRenderMode.BothEyes;
            yield return null;

            foreach (var camera in cameras)
                camera.stereoTargetEye = StereoTargetEyeMask.Both;
#else
                yield return null;
#endif
            }
            else
            {
#if ENABLE_VR && ENABLE_VR_MODULE
            XRSettings.enabled = false;
#endif
                yield return null;
            }

            int waitFrames = settings.WaitFrames;
            ImageComparisonSettings imageComparisonSettings = settings.FindImageComparisonSettings();
            Assert.IsNotNull(imageComparisonSettings);

            if (imageComparisonSettings.UseBackBuffer && settings.WaitFrames < 1)
            {
                waitFrames = 1;
            }


            for (int i = 0; i < waitFrames; i++)
                yield return new WaitForEndOfFrame();

            ImageAssert.AreEqual(testCase.ReferenceImage, cameras.Where(x => x != null), imageComparisonSettings);

            // Does it allocate memory when it renders what's on the main camera?
            bool allocatesMemory = false;
            var mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

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
