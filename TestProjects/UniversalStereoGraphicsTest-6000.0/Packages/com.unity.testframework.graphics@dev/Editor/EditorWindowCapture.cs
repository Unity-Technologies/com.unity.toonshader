#if UNITY_6000_0_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.TestTools.Graphics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using NUnit.Framework;

namespace UnityEditor.TestTools.Graphics
{
    /// <summary>
    /// Settings for capturing an EditorWindow.
    /// </summary>
    public class EditorWindowCaptureSettings
    {
        int width;
        int height;
        TimeSpan delayBeforeCapture;
        List<Action<EditorWindow>> additionalSetupActions;

        /// <summary>
        /// The width of the captured image. Defaults to 512.
        /// </summary>
        public int Width { get => width; internal set => width = value; }

        /// <summary>
        /// The height of the captured image. Defaults to 512.
        /// </summary>
        public int Height { get => height; internal set => height = value; }

        /// <summary>
        /// The delay before capturing the window. Defaults to zero.
        /// </summary>
        /// <remarks>
        /// This can be useful for waiting for async operations to complete before capturing. This wait step happens after all other setup actions.
        /// </remarks>
        public TimeSpan DelayBeforeCapture { get => delayBeforeCapture; internal set => delayBeforeCapture = value; }

        /// <summary>
        /// Additional setup actions to be performed before capturing the window.
        /// </summary>
        /// <remarks>
        /// These actions are performed after the window is set up and before the delay before capture.
        /// </remarks>
        public List<Action<EditorWindow>> AdditionalSetupActions { get => additionalSetupActions; internal set => additionalSetupActions = value; }

        public EditorWindowCaptureSettings(int width, int height, TimeSpan delayBeforeCapture, params Action<EditorWindow>[] additionalSetupActions)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width))
                {
                    Data = { { "width", width } }
                };

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height))
                {
                    Data = { { "height", height } }
                };

            if (additionalSetupActions is null || Array.Exists<Action<EditorWindow>>(additionalSetupActions, a => a is null))
                throw new ArgumentNullException(nameof(additionalSetupActions));

            this.width = width;
            this.height = height;
            this.delayBeforeCapture = delayBeforeCapture;
            this.additionalSetupActions = additionalSetupActions.ToList();
        }

        public static EditorWindowCaptureSettings Default => new EditorWindowCaptureSettings(512, 512, TimeSpan.Zero);
    }

    /// <summary>
    /// Settings for capturing a SceneView.
    /// </summary>
    public class SceneViewCaptureSettings : EditorWindowCaptureSettings
    {
        Transform imageComparisonViewpoint;
        TimeSpan timeout;

        /// <summary>
        /// The viewpoint transform to use for the image comparison.
        /// </summary>
        public Transform ImageComparisonViewpoint { get => imageComparisonViewpoint; internal set => imageComparisonViewpoint = value; }

        /// <summary>
        /// The maximum time to wait for the scene view to update (for async shader compilation etc.)
        /// </summary>
        /// <remarks>
        /// This is used in the SceneView setup to wait for the viewpoint to update to the correct state and for shader compilation to finish. If you have a scene that takes a long time to compile shaders or the viewpoint is very far from the default scene view camera position, you may need to increase this value to ensure the scene view is ready for capture. An error will be logged if the timeout is exceeded.
        /// </remarks>
        public TimeSpan Timeout { get => timeout; internal set => timeout = value; }

        public SceneViewCaptureSettings(int width, int height, TimeSpan delayBeforeCapture, TimeSpan timeout, Transform imageComparisonViewpoint, params Action<EditorWindow>[] additionalSetupActions) : base(width, height, delayBeforeCapture, additionalSetupActions)
        {
            if (imageComparisonViewpoint is null)
                throw new ArgumentNullException(nameof(imageComparisonViewpoint));

            this.timeout = timeout;
            this.imageComparisonViewpoint = imageComparisonViewpoint;
        }

        public static new SceneViewCaptureSettings Default => new SceneViewCaptureSettings(width: 512, height: 512, delayBeforeCapture: TimeSpan.Zero, timeout: new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 10), imageComparisonViewpoint: SceneView.lastActiveSceneView.camera.transform);
    }

    /// <summary>
    /// Capture the current scene view into a Texture2D for use in ImageAssert tests.
    /// </summary>
    public static class EditorWindowCapture
    {
        /// <summary>
        /// Captures a scene view from the perspective of the chosen viewpoint transform.
        /// </summary>
        /// <param name="window"> An existing window that will be used for the capture. </param>
        /// <param name="settings"> Settings for the capture. </param>
        /// <returns> A Texture2D containing the captured scene view. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when the window is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when the width or height is less than or equal to zero. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when the wait time is negative. </exception>
        /// <exception cref="ArgumentNullException"> Thrown when no viewpoint transform was provided. </exception>
        /// <example>
        /// <code>
        /// var sceneView = EditorWindow.CreateWindow&lt;SceneView&gt;();
        /// var capturedTexture = await EditorWindowCapture.CaptureAsync(sceneView, SceneViewCaptureSettings.Default);
        /// </code>
        /// </example>
        /// <example>
        /// <code>
        /// static DrawCameraMode[] viewmodes = (DrawCameraMode[]) Enum.GetValues(typeof(DrawCameraMode));
        ///
        /// [Test]
        /// public async Task SceneViewWindow_WithDebugDrawMode_ImageMatchesReference([ValueSource("viewmodes")] DrawCameraMode viewmode)
        /// {
        ///    ...
        ///    SceneView sceneView = EditorWindow.CreateWindow&lt;SceneView&gt;();
        ///    sceneView.overlayCanvas.overlaysEnabled = false;
        ///    sceneView.drawGizmos = false;
        ///    sceneView.showGrid = false;
        ///    sceneView.cameraMode = SceneView.GetBuiltinCameraMode(viewmode);
        ///    var capturedTexture = await EditorWindowCapture.CaptureAsync(sceneView, new SceneViewCaptureSettings(512, 512, TimeSpan.Zero, new TimeSpan(0, 0, 0, 1), Camera.allCameras.Where(c => c.name == "Camera_2").FirstOrDefault().transform);
        ///    var expected = AssetDatabase.LoadAssetAtPath&lt;Texture2D&gt;(...);
        ///    ImageAssert.AreEqual(expected, capturedTexture);
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// <code>
        /// var customWindow = EditorWindow.CreateWindow&lt;CustomEditorWindow&gt;();
        /// var capturedTexture = await EditorWindowCapture.CaptureAsync(customWindow, new EditorWindowCaptureSettings(512, 512, TimeSpan.Zero, new Action&lt;EditorWindow&gt;[] { (customWindow) =>
        /// {
        ///   customWindow.titleContent = new GUIContent("Hello World");
        /// }
        /// }));
        /// </code>
        /// </example>
        public static async Awaitable<Texture2D> CaptureAsync(EditorWindow window, EditorWindowCaptureSettings settings)
        {
            if (window is null)
                throw new ArgumentNullException(nameof(window));

            // Set up the window
            window.Focus();
            window.minSize = new Vector2(settings.Width, settings.Height);
            window.maxSize = new Vector2(settings.Width, settings.Height);
            await Task.Delay(1);

            // Do any additional setup for specific window types
            switch (window)
            {
                case SceneView sceneView:
                    await SetUpSceneView(sceneView, settings as SceneViewCaptureSettings);
                    break;
                default:
                    break;
            }

            // Perform any additional setup actions provided
            if (settings.AdditionalSetupActions is not null)
                foreach (var action in settings.AdditionalSetupActions)
                    if (action is not null)
                        action(window);
                    else
                        GraphicsTestLogger.Log(LogType.Warning, "Null action provided in the additional setup actions list");

            // Additional wait
            await Task.Delay((int) settings.DelayBeforeCapture.TotalMilliseconds);

            // Capture and set to the active RenderTexture
            RenderTexture backBufferCapture = RenderTexture.GetTemporary(settings.Width, settings.Height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            bool result = InternalEditorUtility.CaptureEditorWindow(window, backBufferCapture);

            if (!result)
            {
                GraphicsTestLogger.Log(LogType.Error, "Failed to capture the EditorWindow.");
                RenderTexture.ReleaseTemporary(backBufferCapture);
                return null;
            }

            RenderTexture.active = backBufferCapture;

            // Apply to our Tex2D
            Texture2D capturedTexture = new Texture2D(settings.Width, settings.Height, TextureFormat.RGB24, false);
            capturedTexture.ReadPixels(new Rect(0, 0, settings.Width, settings.Height), 0, 0);
            capturedTexture.Apply();

            // Clean up
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(backBufferCapture);

            return capturedTexture;
        }

        static async Task SetUpSceneView(SceneView sceneView, SceneViewCaptureSettings settings)
        {
            // Move the scene view camera to the scene's MainCamera
            sceneView.AlignViewToObject(settings.ImageComparisonViewpoint);

            await Task.WhenAll(
                AlignTransforms(sceneView.camera.transform, settings.ImageComparisonViewpoint, (int) settings.Timeout.TotalMilliseconds),
                WaitForShadersToCompile((int) settings.Timeout.TotalMilliseconds)
            );
        }

        static async Task AlignTransforms(Transform a, Transform b, int timeoutMillis)
        {
            while (!AreTransformsAligned(a, b))
            {
                if (timeoutMillis-- <= 0)
                {
                    GraphicsTestLogger.Log(LogType.Error, "Transforms did not align in time. Timeout exceeded.");
                    return;
                }
                await Task.Delay(1);
            }
        }

        static async Task WaitForShadersToCompile(int timeoutMillis)
        {
            bool asyncAllowedPriorState = ShaderUtil.allowAsyncCompilation;
            ShaderUtil.allowAsyncCompilation = false;

            while (ShaderUtil.anythingCompiling)
            {
                if (timeoutMillis-- <= 0)
                {
                    GraphicsTestLogger.Log(LogType.Error, "Shaders did not finish compiling in time. Timeout exceeded.");
                    return;
                }
                await Task.Delay(1);
            }

            ShaderUtil.allowAsyncCompilation = asyncAllowedPriorState;
        }

        static bool AreTransformsAligned(Transform a, Transform b)
            => a.position == b.position && a.rotation.eulerAngles == b.rotation.eulerAngles;
    }
}
#endif // UNITY_6000_0_OR_NEWER
