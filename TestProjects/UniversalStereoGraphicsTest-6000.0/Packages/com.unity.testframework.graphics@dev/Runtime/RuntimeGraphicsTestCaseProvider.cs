using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID || UNITY_WEBGL
using System.Collections;
using UnityEngine.Networking;
#endif


namespace UnityEngine.TestTools.Graphics
{
    public class RuntimeGraphicsTestCaseProvider : IGraphicsTestCaseProvider
    {
        public static readonly string ReferenceImagesBaseBundleName = "referenceimagesbase";
        public static readonly string ReferenceImagesBundlePrefix = "referenceimages";
        public static readonly string RefImageLoadedMsg = "Expected reference image loaded from ReferenceImages bundle";

        public static readonly string BaseRefImageLoadedMsg =
            "Expected reference image loaded from BaseReferenceImages bundle";

        public static AssetBundle ReferenceImagesBundle { get; set; }

        public static AssetBundle BaseReferenceImagesBundle { get; set; }

#if UNITY_WEBGL || UNITY_ANDROID
        private static bool haveTriedToLoadReferenceImagesBundle;
        private static bool haveTriedToLoadBaseReferenceImagesBundle;

        private static IEnumerator GetReferenceImagesBundleAsync()
        {
            Debug.Log("Loading reference images bundle");

            string assetBundlePath = GetReferenceImagesBundlePath();
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(
                    $"Encountered error loading reference image asset bundle from {assetBundlePath}: {www.error}");
            }
            else
            {
                ReferenceImagesBundle = DownloadHandlerAssetBundle.GetContent(www);
            }
        }

        private static IEnumerator GetBaseReferenceImagesBundleAsync()
        {
            Debug.Log("Loading base reference images bundle");

            string assetBundlePath = GetReferenceImagesBaseBundlePath();
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(
                    $"Encountered error loading base reference image asset bundle from {assetBundlePath}: {www.error}");
            }
            else
            {
                BaseReferenceImagesBundle = DownloadHandlerAssetBundle.GetContent(www);
            }
        }

        /// <summary>
        /// Ensures that both the ReferenceImages and BaseReferenceImages AssetBundles are loaded.
        /// This is required for WebGL and Android platforms, as AssetBundles are loaded asynchronously.
        /// This method should be called prior to running a test from a UnitySetUp method.
        /// </summary>
        /// <returns>IEnumerator</returns>
        public static IEnumerator EnsureGetReferenceImageBundlesAsync()
        {
            if (ReferenceImagesBundle == null && !haveTriedToLoadReferenceImagesBundle)
            {
                yield return GetReferenceImagesBundleAsync();
                haveTriedToLoadReferenceImagesBundle = true;
            }

            if (BaseReferenceImagesBundle == null && !haveTriedToLoadBaseReferenceImagesBundle)
            {
                yield return GetBaseReferenceImagesBundleAsync();
                haveTriedToLoadBaseReferenceImagesBundle = true;
            }

            yield return null;
        }

        /// <summary>
        /// Given a GraphicsTestCase, this method will attempt to load the reference image from the
        /// ReferenceImages or BaseReferenceImages AssetBundles loaded previously using the EnsureGetReferenceImageBundlesAsync method.
        /// </summary>
        /// <param name="testCase"></param>
        public static void AssociateReferenceImageWithTest(GraphicsTestCase testCase)
        {
            var referenceImagePath = Path.GetFileNameWithoutExtension(testCase.ScenePath);

            if (ReferenceImagesBundle != null &&
                ReferenceImagesBundle.Contains(referenceImagePath))
            {
                Debug.Log($"Found reference image {referenceImagePath} and assigning it to test case {testCase.Name}");
                testCase.ReferenceImage = ReferenceImagesBundle.LoadAsset<Texture2D>(referenceImagePath);
                testCase.ReferenceImagePathLog = RefImageLoadedMsg;
            }
            else if (BaseReferenceImagesBundle != null &&
                     BaseReferenceImagesBundle.Contains(referenceImagePath))
            {
                Debug.Log(
                    $"Found base reference image {referenceImagePath} and assigning it to test case {testCase.Name}");
                testCase.ReferenceImage =
                    BaseReferenceImagesBundle.LoadAsset<Texture2D>(referenceImagePath);
                testCase.ReferenceImagePathLog = BaseRefImageLoadedMsg;
            }
            else
            {
                Debug.Log(
                    $"Couldn't find reference or base reference image {referenceImagePath} to associate with test case {testCase.Name}");
                testCase.ReferenceImagePathLog = "No reference image found";
            }
        }

#endif
        /// <summary>
        /// This method generates a list of GraphicsTestCases based on the scenes in the project.
        /// For non-WebGL and non-Android platforms, this method also loads the ReferenceImages
        /// and BaseReferenceImages AssetBundles synchronously, then uses them to associate reference
        /// images with each test case.
        /// For WebGL and Android platforms, this method does not load the AssetBundles or associate
        /// reference images the test case, instead deferring these two steps to the user to perform
        /// in the UnitySetUp using the EnsureGetReferenceImageBundlesAsync and in the test method using
        /// AssociateReferenceImageWithTest.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GraphicsTestCase> GetTestCases()
        {
#if !UNITY_WEBGL && !UNITY_ANDROID // For non-WebGL and non-Android, AssetBundles are loaded synchronously as part of the test case creation flow

            GetReferenceImagesBundle();
            GetReferenceImagesBaseBundle();
#endif
            var srpTestSceneAsset = Resources.Load<SRPTestSceneAsset>("SRPTestSceneSO");
            var scenePaths = GetScenePaths();

            foreach (var scenePath in scenePaths)
            {
#if !UNITY_WEBGL && !UNITY_ANDROID // For non-WebGL and non-Android, reference images for each scene are associated with the test case as part of the test case creation flow

                Texture2D referenceImage = null;
                string referenceImagePathLog = null;
#endif
                bool srpTestSceneAssetUsed = false;
                if (srpTestSceneAsset != null)
                {
                    foreach (SRPTestSceneAsset.TestData testData in srpTestSceneAsset.testDatas)
                    {
                        if (testData.path == scenePath && testData.enabled)
                        {
                            srpTestSceneAssetUsed = true;
                            foreach (RenderPipelineAsset srpAsset in testData.srpAssets)
                            {
#if !UNITY_WEBGL && !UNITY_ANDROID
                                var imagePath = $"{Path.GetFileNameWithoutExtension(scenePath)}_{srpAsset.name}";

                                // First look for a scenePath_srpAssetName reference image in both bundles
                                if (ReferenceImagesBundle != null && ReferenceImagesBundle.Contains(imagePath))
                                {
                                    referenceImage = ReferenceImagesBundle.LoadAsset<Texture2D>(imagePath);
                                    referenceImagePathLog = RefImageLoadedMsg;
                                    Debug.Log(RefImageLoadedMsg);
                                }
                                else if (BaseReferenceImagesBundle != null &&
                                         BaseReferenceImagesBundle.Contains(imagePath))
                                {
                                    referenceImage = BaseReferenceImagesBundle.LoadAsset<Texture2D>(imagePath);
                                    referenceImagePathLog = BaseRefImageLoadedMsg;
                                    Debug.Log(BaseRefImageLoadedMsg);
                                }

                                // If no scenePath_srpAssetName reference image was found, fall back to scenePath only
                                if (!referenceImage)
                                {
                                    var baseImagePath = $"{Path.GetFileNameWithoutExtension(scenePath)}";

                                    if (ReferenceImagesBundle != null && ReferenceImagesBundle.Contains(baseImagePath))
                                    {
                                        referenceImage = ReferenceImagesBundle.LoadAsset<Texture2D>(baseImagePath);
                                        referenceImagePathLog = RefImageLoadedMsg;
                                        Debug.Log(RefImageLoadedMsg);
                                    }
                                    else if (BaseReferenceImagesBundle != null &&
                                             BaseReferenceImagesBundle.Contains(baseImagePath))
                                    {
                                        referenceImage = BaseReferenceImagesBundle.LoadAsset<Texture2D>(baseImagePath);
                                        referenceImagePathLog = BaseRefImageLoadedMsg;
                                        Debug.Log(BaseRefImageLoadedMsg);
                                    }
                                }

                                yield return new GraphicsTestCase(scenePath, referenceImage, srpAsset,
                                    referenceImagePathLog);
#else
                                yield return GetTestCaseFromPath(scenePath, srpAsset);
#endif
                            }

                            break;
                        }
                    }
                }

                if (!srpTestSceneAssetUsed)
                {
#if !UNITY_WEBGL && !UNITY_ANDROID
                    var imagePath = Path.GetFileNameWithoutExtension(scenePath);
                    Debug.Log("imagePath: " + imagePath);

                    // The bundle might not exist if there are no reference images for this configuration yet
                    if (ReferenceImagesBundle != null && ReferenceImagesBundle.Contains(imagePath))
                    {
                        referenceImage = ReferenceImagesBundle.LoadAsset<Texture2D>(imagePath);
                        Debug.Log("Tried to load reference image from ReferenceImagesBundle. referenceImage: " +
                                  referenceImage.name);
                        referenceImagePathLog = RefImageLoadedMsg;
                        Debug.Log(RefImageLoadedMsg);
                    }
                    else if (BaseReferenceImagesBundle != null && BaseReferenceImagesBundle.Contains(imagePath))
                    {
                        referenceImage = BaseReferenceImagesBundle.LoadAsset<Texture2D>(imagePath);
                        Debug.Log("Tried to load reference image from BaseReferenceImagesBundle. referenceImage: " +
                                  referenceImage);
                        referenceImagePathLog = BaseRefImageLoadedMsg;
                        Debug.Log(BaseRefImageLoadedMsg);
                    }

                    yield return new GraphicsTestCase(scenePath, referenceImage,
                        referenceImagePathLog: referenceImagePathLog);

#else // For WebGL and Android, AssetBundles need to be loaded asynchronously using GetReferenceImagesBundleAsync and GetBaseReferenceImagesBundleAsync after the test cases are created. Just create the test cases without reference images for now.
                    yield return GetTestCaseFromPath(scenePath);
#endif
                }
            }
        }

        public GraphicsTestCase GetTestCaseFromPath(string scenePath)
        {
            return GetTestCaseFromPath(scenePath, null);
        }

        public GraphicsTestCase GetTestCaseFromPath(string scenePath, RenderPipelineAsset srpAsset)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                return null;
            }

            GraphicsTestCase testCase = null;
#if !UNITY_WEBGL && !UNITY_ANDROID // WebGL and Android don't support AssetBundles.LoadFromFile
            var referenceImagesBundlePath = string.Format(ReferenceImagesBundlePrefix,
                UseGraphicsTestCasesAttribute.ColorSpace,
                UseGraphicsTestCasesAttribute.Platform.ToUniqueString(TestPlatform.GetCurrent().Arch),
                UseGraphicsTestCasesAttribute.GraphicsDevice,
                UseGraphicsTestCasesAttribute.LoadedXRDevice).ToLower();

            referenceImagesBundlePath = Path.Combine(Application.streamingAssetsPath, referenceImagesBundlePath);

            var referenceImagesBaseBundlePath = ReferenceImagesBaseBundleName;
            referenceImagesBaseBundlePath =
                Path.Combine(Application.streamingAssetsPath, referenceImagesBaseBundlePath);

            if (File.Exists(referenceImagesBundlePath))
            {
                ReferenceImagesBundle = AssetBundle.LoadFromFile(referenceImagesBundlePath);
            }

            if (File.Exists(referenceImagesBaseBundlePath))
            {
                BaseReferenceImagesBundle = AssetBundle.LoadFromFile(referenceImagesBaseBundlePath);
            }

            var imagePath = Path.GetFileNameWithoutExtension(scenePath);

            Texture2D referenceImage = null;
            string referenceImagePathLog = null;

            // The bundle might not exist if there are no reference images for this configuration yet
            if (ReferenceImagesBundle != null && ReferenceImagesBundle.Contains(imagePath))
            {
                referenceImage = ReferenceImagesBundle.LoadAsset<Texture2D>(imagePath);
                referenceImagePathLog = RefImageLoadedMsg;
                Debug.Log(RefImageLoadedMsg);
            }
            else if (BaseReferenceImagesBundle != null)
            {
                referenceImage = BaseReferenceImagesBundle.LoadAsset<Texture2D>(imagePath);
                referenceImagePathLog = BaseRefImageLoadedMsg;
                Debug.Log(BaseRefImageLoadedMsg);
            }
            else
            {
                Debug.Log($"{nameof(ReferenceImagesBundle)} is null = {ReferenceImagesBundle = null}");
                Debug.Log($"{nameof(BaseReferenceImagesBundle)}  is null = {BaseReferenceImagesBundle = null}");
                Debug.Log($"imagePath = {imagePath}");
                if (ReferenceImagesBundle != null)
                {
                    Debug.Log(
                        $"ReferenceImagesBundle.Contains(imagePath) = {ReferenceImagesBundle.Contains(imagePath)}");
                }

                if (BaseReferenceImagesBundle != null)
                {
                    Debug.Log(
                        $"BaseReferenceImagesBundle.Contains(imagePath) = {BaseReferenceImagesBundle.Contains(imagePath)}");
                }
            }

            testCase = new GraphicsTestCase(scenePath, referenceImage, referenceImagePathLog: referenceImagePathLog);
#else
            testCase = new GraphicsTestCase(scenePath, srpAsset);
#endif

            return testCase;
        }

        private static string GetReferenceImagesBundlePath()
        {
            var streamingAssetsPath = Application.streamingAssetsPath;
            var colorSpace = UseGraphicsTestCasesAttribute.ColorSpace;
            var platform = UseGraphicsTestCasesAttribute.Platform.ToUniqueString(TestPlatform.GetCurrent().Arch);
            var graphicsDevice = UseGraphicsTestCasesAttribute.GraphicsDevice;
            var loadedXrDevice = UseGraphicsTestCasesAttribute.LoadedXRDevice;

            var referenceImageBundlePath = string.Format(
                "{0}-{1}-{2}-{3}-{4}",
                ReferenceImagesBundlePrefix,
                colorSpace,
                platform,
                graphicsDevice,
                loadedXrDevice).ToLower();
            return Path.Combine(streamingAssetsPath, referenceImageBundlePath);
        }

        private static string GetReferenceImagesBaseBundlePath()
        {
            return Path.Combine(Application.streamingAssetsPath, ReferenceImagesBaseBundleName);
        }

#if !UNITY_WEBGL && !UNITY_ANDROID // WebGL and Android don't support AssetBundles.LoadFromFile
        private void GetReferenceImagesBundle()
        {
            string referenceImagesBundlePath = GetReferenceImagesBundlePath();
            if (File.Exists(referenceImagesBundlePath) && ReferenceImagesBundle == null)
            {
                ReferenceImagesBundle = AssetBundle.LoadFromFile(referenceImagesBundlePath);
            }
        }

        private void GetReferenceImagesBaseBundle()
        {
            string referenceImagesBaseBundlePath = GetReferenceImagesBaseBundlePath();
            if (File.Exists(referenceImagesBaseBundlePath) && BaseReferenceImagesBundle == null)
            {
                BaseReferenceImagesBundle = AssetBundle.LoadFromFile(referenceImagesBaseBundlePath);
            }
        }
#endif
        public static string[] GetScenePaths()
        {
            var sceneList = Resources.Load<TextAsset>("SceneList");
            if (sceneList == null)
            {
                Debug.LogError("SceneList.txt was not found in the Resources folder. This file should have been generated by SetupGraphicsTestCases.Setup. Have you called SetupGraphicsTestCases.Setup in the  prebuild-setup? If so, please try to close and reopen the project, then try to run the Player tests again. If the problem persists, please file a bug report.");
            }

            var scenePaths = sceneList.text
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                // The Unity Test Framework creates a transient scene to run the test, so we skip it
                // here if it's present.
                // A race condition can occur where the scene is present and incorrectly included in the test run.
                if (SceneManager.GetSceneAt(i).path.ToLower().Contains("inittestscene"))
                {
                    continue;
                }

                if (!scenePaths.Contains(SceneManager.GetSceneAt(i).path))
                {
                    scenePaths.Add(SceneManager.GetSceneAt(i).path);
                }
            }

            return scenePaths.ToArray();
        }
    }
}
