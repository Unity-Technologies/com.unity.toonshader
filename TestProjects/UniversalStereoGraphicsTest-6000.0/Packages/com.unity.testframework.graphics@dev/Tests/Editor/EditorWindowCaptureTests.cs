#if UNITY_6000_0_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using System.Linq;
using System.Diagnostics;

namespace UnityEditor.TestTools.Graphics.Tests
{
    [TestOf(typeof(EditorWindowCapture))]
    public class EditorWindowCaptureTestsNullWindow
    {
        [Test]
        public void NullWindow_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await EditorWindowCapture.CaptureAsync(null, EditorWindowCaptureSettings.Default));
        }
    }

    [TestOf(typeof(EditorWindowCapture))]
    public class EditorWindowCaptureTestsSceneView
    {
        [Test]
        public async Task SceneViewWindow_WithDefaultSettings_CaptureIsTaken()
        {
            SceneView sceneView = EditorWindow.CreateWindow<SceneView>();
            sceneView.overlayCanvas.overlaysEnabled = false;
            sceneView.drawGizmos = false;
            sceneView.showGrid = false;
            Texture2D capturedTexture = await EditorWindowCapture.CaptureAsync(sceneView, SceneViewCaptureSettings.Default);
            Assert.That(capturedTexture, Is.Not.Null);
        }

        [Test]
        public void SceneViewWindow_WithNullImageComparisonViewpoint_ThrowsArgumentNullException()
        {
            SceneView sceneView = EditorWindow.CreateWindow<SceneView>();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await EditorWindowCapture.CaptureAsync(sceneView, new SceneViewCaptureSettings(512, 512, TimeSpan.Zero, TimeSpan.Zero, null)));
        }

        [Test]
        public async Task SceneViewWindow_WithShaderRecompilation_ShaderSettingsArePreserved()
        {
            SceneView sceneView = EditorWindow.CreateWindow<SceneView>();
            bool asyncAllowedPriorState = ShaderUtil.allowAsyncCompilation;
            await EditorWindowCapture.CaptureAsync(sceneView, SceneViewCaptureSettings.Default);
            Assert.That(ShaderUtil.allowAsyncCompilation, Is.EqualTo(asyncAllowedPriorState));
        }

        [TearDown]
        public void TearDown()
        {
            while (EditorWindow.HasOpenInstances<SceneView>())
                EditorWindow.GetWindow<SceneView>().Close();
        }
    }

    [TestOf(typeof(EditorWindowCapture))]
    public class EditorWindowCaptureTestsCustomEditor
    {
        [Test]
        public async Task CustomEditorWindow_ImageIsNotNullOrEmpty()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Texture2D capturedTexture = await EditorWindowCapture.CaptureAsync(customWindow, EditorWindowCaptureSettings.Default);
            Assert.That(capturedTexture, Is.Not.Null);
            Texture2D expected = new Texture2D(512, 512, TextureFormat.RGB24, false);
            Assert.Throws<AssertionException>(() => ImageAssert.AreEqual(expected, capturedTexture));
        }

        [Test]
        public async Task CustomEditorWindow_ImageMatchesReference()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Texture2D capturedTexture = await EditorWindowCapture.CaptureAsync(customWindow, EditorWindowCaptureSettings.Default);
            Assert.That(capturedTexture, Is.Not.Null);
            Texture2D expected = new Texture2D(512, 512, TextureFormat.RGB24, false);
            // ImageAssert.AreEqual(expected, capturedTexture); https://jira.unity3d.com/browse/GSDET-134
        }

        [Test]
        public async Task CustomEditorWindow_WithAdditionalActions_ActionsArePerformed()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Texture2D capturedTexture = await EditorWindowCapture.CaptureAsync(customWindow, new EditorWindowCaptureSettings(512, 512, TimeSpan.Zero, new Action<EditorWindow>[] { (customWindow) =>
            {
                customWindow.titleContent = new GUIContent("Hello World");
            }
            }));
            Assert.That(customWindow.titleContent.text, Is.EqualTo("Hello World"));
        }

        [Test]
        public async Task CustomEditorWindow_WithDefaultSettingsAndAdditionalActions_ActionsArePerformed()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            var settings = EditorWindowCaptureSettings.Default;
            settings.AdditionalSetupActions.Add(
                (customWindow) =>
                {
                    customWindow.titleContent = new GUIContent("Hello World");
                }
            );
            Texture2D capturedTexture = await EditorWindowCapture.CaptureAsync(customWindow, settings);
            Assert.That(customWindow.titleContent.text, Is.EqualTo("Hello World"));
        }

        [Test]
        public async Task CustomEditorWindow_WithDefaultSettingsAndNullAdditionalAction_ActionsArePerformed()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            var settings = EditorWindowCaptureSettings.Default;
            settings.AdditionalSetupActions.Add(null);
            LogAssert.Expect(LogType.Warning, "Graphics Tests: Null action provided in the additional setup actions list");
            Texture2D capturedTexture = await EditorWindowCapture.CaptureAsync(customWindow, settings);
        }

        [Test, Pairwise]
        public async Task CustomEditorWindow_WithCustomDimensions_DimensionsAreKept(
            [Values(1, 128, 256)] int width,
            [Values(1, 128, 256)] int height
        )
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Texture2D capturedTexture = await EditorWindowCapture.CaptureAsync(customWindow, new EditorWindowCaptureSettings(width, height, TimeSpan.Zero));
            Assert.That(capturedTexture, Is.Not.Null);
            Assert.That(capturedTexture.width, Is.EqualTo(width));
            Assert.That(capturedTexture.height, Is.EqualTo(height));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 512)]
        [TestCase(512, 0)]
        public void CustomEditorWindow_WithInvalidDimensions_ThrowsArgumentException(int width, int height)
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await EditorWindowCapture.CaptureAsync(customWindow, new EditorWindowCaptureSettings(width, height, TimeSpan.Zero)));
        }

        [Test]
        public void CustomEditorWindow_WithNullAdditionalActions_ThrowsArgumentNullException()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await EditorWindowCapture.CaptureAsync(customWindow, new EditorWindowCaptureSettings(512, 512, TimeSpan.Zero, null)));
        }

        [Test]
        public void CustomEditorWindow_WithNullAdditionalAction_ThrowsArgumentNullException()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await EditorWindowCapture.CaptureAsync(customWindow, new EditorWindowCaptureSettings(512, 512, TimeSpan.Zero, new Action<EditorWindow>[] { null })));
        }

        [Test]
        public async Task CustomEditorWindow_WithAdditionalWait_TakesLongerThanWithoutAdditionalWait()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await EditorWindowCapture.CaptureAsync(customWindow, new EditorWindowCaptureSettings(512, 512, TimeSpan.Zero));
            stopwatch.Stop();
            TimeSpan noWait = stopwatch.Elapsed;
            stopwatch.Restart();
            await EditorWindowCapture.CaptureAsync(customWindow, new EditorWindowCaptureSettings(512, 512, new TimeSpan(0, 0, 0, 1)));
            stopwatch.Stop();
            TimeSpan withWait = stopwatch.Elapsed;
            TestContext.Out.WriteLine($"No wait: {noWait}, with wait: {withWait}");
            Assert.That(withWait, Is.GreaterThan(noWait));
            Assert.That(withWait, Is.GreaterThanOrEqualTo(new TimeSpan(0, 0, 0, 1)));
        }

        [Test]
        public async Task CustomEditorWindow_WithLostFocus_ErrorIsThrown()
        {
            CustomEditorWindow customWindow = EditorWindow.CreateWindow<CustomEditorWindow>();
            Action<EditorWindow> lostFocus = (customWindow) =>
            {
                customWindow.Close();
            };

            var settings = EditorWindowCaptureSettings.Default;
            settings.AdditionalSetupActions.Add(lostFocus);
            LogAssert.Expect(LogType.Error, "CaptureEditorWindow: window must have focus");
            LogAssert.Expect(LogType.Error, "Graphics Tests: Failed to capture the EditorWindow.");
            var result = await EditorWindowCapture.CaptureAsync(customWindow, settings);
            Assert.That(result, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            while (EditorWindow.HasOpenInstances<CustomEditorWindow>())
                EditorWindow.GetWindow<CustomEditorWindow>().Close();
        }
    }

    public class CustomEditorWindow : EditorWindow
    {
        public CustomEditorWindow()
        {
            minSize = new Vector2(512, 512);
            maxSize = new Vector2(512, 512);
        }

        public void OnGUI()
        {
            GUILayout.Label("Hello World");
        }
    }
}
#endif // UNITY_6000_0_OR_NEWER
