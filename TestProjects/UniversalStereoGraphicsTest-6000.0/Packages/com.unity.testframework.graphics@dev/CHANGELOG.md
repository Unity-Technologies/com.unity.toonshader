# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

---

## [8.9.1-exp.1] - 2024-10-10

- Updated the regular expression to match the new log format for shader variants sent to the GPU driver.

## [8.9.0-exp.1] - 2024-07-30

- Added parameter to `ImageAssert.AreEqual` to allow for saving images to the application persistent data path without an Editor connection.

## [8.8.0-exp.1] - 2024-07-23

- There is now an ImageAssertAsync function to use AsyncGPUReadback to get test results
- Added support for the WebGPU graphics API

## [8.7.1-exp.1] - 2024-07-15

- There is now a function to check if there are any "useless" variants in the shader variant list (multiple variant leading to the same compiled shader code).

## [8.7.0-exp.1] - 2024-06-12

- Enable the use of GraphicsTestLogger at Runtime for certain platforms.
- Reorganized menu items in the Tools/Graphics Test Framework menu.
- Several minor fixes and improvements.

## [8.6.3-exp.1] - 2024-05-29

- Update ImageAssert to properly encode RGBAFloat images as full-precision EXR files, rather than half-precision.
- Fixed UUM-73039: Error is thrown if reference image is not present on Android and WebGL

## [8.6.2-exp.2] - 2024-05-10

- Renamed package displayName to "Graphics Test Framework" in the package manifest
- Reformatted this document and the package manifest

## [8.6.2-exp.1] - 2024-05-03

- Added CliArgumentsCheck class to validate graphics related command-line arguments.

## [8.6.1-exp.1] - 2024-04-29

- Added the GraphicsTestLogger class to log the graphics test-related information.
- Added utility tool to copy image references to the appropriate folder.

## [8.6.0-exp.1] - 2024-04-09

- Added EditorWindowCapture class to capture the content of an EditorWindow.
- Made CaptureSceneView methods obsolete as it was replaced by EditorWindowCapture.

## [8.5.1-exp.1] - 2024-03-21

- Added Windows ARM64 support.
- Added TestPlatform as an abstraction of the platform used in the test run.
- Added tests for the RuntimePlatformExtensions class.

## [8.5.0-exp.1] - 2024-03-12

- Introduced GraphicsDeviceInfo class to provide information about the graphics device used in the test run.

## [8.4.1-exp.1] - 2024-03-08

- Moved and renamed the Validation Tests to be more descriptive

## [8.4.0-exp.1] - 2024-03-07

- Introduced VisionOS support.

## [8.3.3-exp.1] - 2024-03-06

- Update the regex used to match the shader variant compilation to reflect changes in the editor logger.

## [8.3.2-exp.1] - 2024-01-29

- Only bake scenes included in the build

## [8.3.1-exp.1] - 2023-12-01

- Force the Shader Variant List generator to always use Ordinal sorting (instead of undetermined comparer).

## [8.3.0-exp.1] - 2023-09-29

- Add the Architecture enum to TestFilterConfig
- Update SetupGraphicsTestCases code to allow tests to be filtered according to processor architecture

## [8.2.2-exp.1] - 2023-08-18

- Move scene filter into GetScenePaths method so all callers get the correct scene list.

## [8.2.1-exp.1] - 2023-08-16

- Make GetScenePaths method public for reuse outside of the class

## [8.2.0-exp.1] - 2023-08-10

- Remove warning from CustomTestRunCallback due to unnecessary Monobehaviour inheritance

## [8.1.0-exp.1] - 2023-08-08

- Update the BuildScenes method, in that way so it also cover consoles behavior

## [8.0.0-exp.1] - 2023-08-07

- Enable asynchronous AssetBundle loading for WebGL and Android platforms

## [7.17.4-exp.1] - 2023-08-04

- Added conditional for the custom BuildPlayer, to avoid executing it when running in the standalone mode.

## [7.17.3-exp.1] - 2023-07-07

- Added a check in the HDR image comparison to avoid propagating NaNs in the image comparison.

## [7.17.2-exp.1] - 2023-06-06

- Added a custom test run callback to prevent OnGUI() callbacks from UTF component at runtime while testing.

## [7.17.1-exp.1] - 2023-05-30

- Update com.unity.addressables dependency.

## [7.17.0-exp.1] - 2023-05-11

- Fixed Shader Variant stripper not correctly supported on Vulkan.

## [7.16.0-exp.1] - 2023-04-21

- Added a custom runtimeplayer platform parameter as overload to the method BuildTargetToRuntimePlatform.

## [7.15.0-exp.1] - 2023-03-22

- Added log message for the expected image path to the test failure

## [7.14.1-exp.1] - 2023-03-08

- Fixed DXR shaders not compatible with graphics test stripper.

## [7.14.0-exp.1] - 2023-03-06

- Add EmbeddedLinux and QNX to EditorUtils

## [7.13.0-exp.1] - 2023-03-06

- Add Graphics Test Shader Stripping system, when used in test projects, allows to speed up a lot the shader compilation by removing unused shader variants.

## [7.12.0-exp.3] - 2023-01-31

- Fixed XR filter check that returned true even with 0 loaders selected.
- Added functionality to allow setting the resolution in code instead of per-scene assets.

## [7.12.0-exp.2] - 2023-01-16

- Wrap Stadia platform and GLES2 graphics API in !2023_1_OR_NEWER due to deprecation in 2023.1 and higher.

## [7.12.0-exp.1] - 2023-01-16

- Added GUI setting option, which enabled, saves actual images even if the tests pass. Added an extra "-save-actual-images" command line option to the player so that the feature can be enabled while running the tests form command line.
- Renamed `HandleFailedImageEvent` method to `HandleImageEvent`, `FailedImageMessage` class to `ImageMessage`

## [7.11.1-exp.1] - 2022-11-22

- Added 'ImageComparisonSettings.TargetMSAASamples' integer that is respected when creating a target texture for the test framework to run when UseBackbuffer is false.

## [7.11.0-exp.1] - 2022-11-22

- Added `ImageAssert.CheckGCAllocWithCallstack()` to render an image from the given camera and check if it allocated memory while doing so, outputting the callstack of the GC.Alloc if found.
- Moved the `SetupProject` class behind the `UnityEditor.TestTools.Graphics` namespace to be able to call it from external code. Also added a non-namespaced wrapper that calls it to make the change non-breaking.

## [7.10.1-exp.1] - 2022-10-17

- Added `GenerateCodeCoverage` class that contains the method used to automate the code coverage analysis on the scene-based graphics tests using the `com.unity.testtools.codecoverage` package's on demand recording.

## [7.10.0-exp.8] - 2022-09-26

- Update documentation

## [7.10.0-exp.7] - 2022-09-08

- Fix version of UTF in the asmdef define

## [7.10.0-exp.6] - 2022-09-07

- Added an extra "-render-graph-reuse-tests" command line option and define to the player so that RenderGraph test code can be enabled while still using the regular (non-RG) reference images.
- Add support for LinuxHeadlessSimulation platform

## [7.10.0-exp.5] - 2022-08-30

- When using the Test Scene Asset, if the "SceneName_RPAssetName" reference image is not found, fall back to the base "SceneName" reference image.

## [7.10.0-exp.4] - 2022-08-17

- Fixed a performance regression that caused reference image assets to be reimported at every selection in the Test Results window.

## [7.10.0-exp.3] - 2022-08-17

- Update version defines for Unity Test Framework

## [7.10.0-exp.2] - 2022-07-25

- Removed `com.unity.subsystemregistration` package dependency.

## [7.10.0-exp.1] - 2022-05-27

- Added new image assert function that implements functionality to do image asserts with floating point images. These images can have negative numbers and are written out as EXR.
- Added new image comparison function that implements MSE/RMSE comparisons.
- Added support for applying texture import settings for the written ActualImages.

## [7.9.0-exp.1] - 2022-05-18

- Added SRP Test Scene Asset. Which allows users to add scenes combined with SRP Assets. So that we can test 1 scene with several different SRP's.

## [7.8.23-exp.2] - 2022-04-21

- Added support for parametric CodeBasedGraphicsTests.
- Added support for Playmode CodeBasedGraphicsTests.
- Made Graphics Test Results window TreeView scrollable
- Added compatibility between CodeBasedGraphicsTests and the ReferenceImagesBase folder

## [7.8.23-exp.1] - 2022-04-11

- Added support for new PS5 GraphicsDeviceType.

## [7.8.22-exp.2] - 2022-04-06

- Added `UNITY_TESTS_FRAMEWORK` defines for the package assemblies so they can be referred by test projects without adding to the `testables` section of the manifest.json file.

## [7.8.22-exp.1] - 2022-04-04

- Reworked `CaptureSceneView` to use the back-buffer instead of a screenshot.

## [7.8.21-exp.2] - 2022-04-04

- Fixed the issue in `ImageAssert.cs` with running the optimization for reference images.
- Fixed the issue in the optimization implementation.

## [7.8.21-exp.1] - 2022-03-22

- Added `CodeBasedGraphicsTestAttribute` which allows a unit test to be marked as a graphics test, and root paths of reference images and actual images to be specified.
- Added `UnityEditor.TestTools.Graphics.EditorUtils` class which provides a few utility functions. Please refer to the documentation page.
- Added `UnityEngine.TestTools.Graphics.TestUtils` class which provides utility functions for generating test result folder path based on give test configurations.
- Changed `GraphicsTestCase` class now provides new properties returning the test name and the `CodeBasedGraphicsTestAttribute` for unit test based graphics tests.

## [7.8.20-exp.1] - 2022-03-21

- Add Optimization feature available for reference images.

## [7.8.19-exp.2] - 2022-02-16

- Allow custom scene views as input to CaptureSceneView.

## [7.8.19-exp.1] - 2022-02-16

- Add the CaptureSceneView class that enables scene view graphics tests.

## [7.8.18-exp.1] - 2022-02-15

- Updated xbox references to reflect enum changes in the editor.

## [7.8.17-exp.3] - 2022-02-07

- Updated `nuget.newtonsoft-json` and `xr.legacyinputhelpers` dependencies.

## [7.8.17-exp.2] - 2022-01-28

- Fixed a missing clear in UTR framework in order to make Gamecore pass correctly the Graphic Compositor test.

## [7.8.17-exp.1] - 2021-11-18

- Add support for tests on Apple Silicon (M1) devices.
- Moves reference images for M1 macs under OSXPlayer_AppleSilicon and OSXEditor_AppleSilicon.
- Leaves reference image paths for Intel macs untouched (they remain under OSXPlayer and OSXEditor).

## [7.8.16-preview] - 2021-08-06

- Fixed an issue where the framework would build asset bundles for standalone when running in the editor, when using 1.2 version of Unity Test Framework package.

## [7.8.15-preview] - 2021-07-29

- Add [RequiresPlayMode] tags to the tests intended for PlayMode, allowing them to still run in PlayMode in the newest UTF versions.
- Change the logic in SetupGraphicsTestCases to draw the targetPlatform from the settings directly, rather than from the filter.

## [7.8.14-preview] - 2021-07-23

- Add "GlobalResolutionSetter" component that lets you set different device resolution per platform

## [7.8.12-preview] - 2021-05-26

- Fix issue with test filter and XR reusable tests

## [7.8.11-preview] - 2021-05-11

- Bump com.unity.xr.management from 3.2.15 to 4.0.5

## [7.8.10-preview] - 2021-04-26

- Make `StripParametricTestCharacters` replace "," with "-".
- Make `StripParametricTestCharacters` replace "(" with "_".
- Make `StripParametricTestCharacters` replace ")" with "_".

## [7.8.9-preview] - 2021-04-23

- Make `StripParametricTestCharacters` replace "," with "_".

## [7.8.8-preview] - 2021-04-08

- Reenable AreEqual_WidthDifferentSizeImages_ThrowsAssertionException (was removed in 7.8.2-preview)
- Bump com.unity.addressables from 1.16.15 to 1.17.15

## [7.8.7-preview] - 2021-03-10

- Remove BlackBerry player support.
- Don't clear the GICache on every bake.

## [7.8.6-preview] - 2020-03-08

- Fix typo in GC Alloc messages
- Remove unused code
- Fix for undeterministic RuntimePlatform -> string conversion

## [7.8.5-preview] - 2020-02-18

- Fix buildOptions error
- Avoid RenderTexture usage for GC tests when possible

## [7.8.4-preview] - 2020-02-10

- More build options in ApplySettings

## [7.8.3-preview] - 2021-02-03

- Add support for new console platforms
- Fixes for the CHANGELOG.md validation
- NDA platform validator configuration added

## [7.8.2-preview] - 2021-01-29

- Disable AreEqual_WidthDifferentSizeImages_ThrowsAssertionException

## [7.8.1-preview] - 2021-01-28

- Test filter sort now uses stable sorting with additional properties

## [7.8.0-preview] - 2021-01-07

- Reference dependencies needed for isolation testing

## [7.7.1-preview] - 2020-11-30

- Add support for new GraphicsDeviceTypes

## [7.7.0-preview] - 2020-11-16

- Add support for XR reusable tests

### [7.6.0-preview] - 2020-11-04

- Add SetupProject class

## [7.5.1-preview] - 2020-10-07

- Update SetupGraphicsTestCases.cs to support "Player Build: BuildConfiguration" setting for Hybrid scenes

## [7.5.0-preview] - 2020-09-24

- Bump XR Management version from 3.0.6 to 3.2.15

## [7.4.1-preview] - 2020-09-09

- Disabled ImageAssertTests.PerPixelTest on device to avoid issues with TestCaseSource.

## [7.4.0-preview] - 2020-08-27

- Added the ability to test the number of incorrect pixels against a set ratio.
- Added the ability to test the sRGB-encoded color channels against a threshold.
- Added the ability to test the alpha channel against a threshold.

## [7.3.0-preview] - 2020-07-09

- Added optional callback on ImageAssert triggered after all cameras are rendered.

## [7.2.3-preview] - 2020-07-06

- Enable multiple scenes per test filter and clean up UI a bit.
- Fixes a memory allocation in the Profiler.Get function that was counted as memory allocation in the render loop of SRP.

## [7.2.2-preview] - 2020-06-08

- Wrap built in xr checks in 2020_2_OR_NEWER due to built in xr deprecation in 2020.2 and higher.
- Test filter fixes for multiple matching filters

## [7.2.1-preview] - 2020-05-01

- Backwards compatibility to 2019.3

## [7.2.0-preview] - 2020-04-30

- Add the option for tests to use the back buffer instead of rendering to a render texture first
- Fix LoadedXRDevice to use XR SDK first

## [7.1.13-preview] - 2020-04-06

- Update reference versions of json and utp

## [7.1.12-preview] - 2020-03-24

- Bug fix for where all scenes would be baked when only one was selected.
- Bug fix for Xbox where tests would fail due to XR APIs

## [7.1.11-preview] - 2020-03-20

- Fix for OSX Metal automation

## [7.1.10-preview] - 2020-03-20

- Add build targets for DX12 and OSX Metal

## [7.1.9-preview] - 2010-03-19

- Use Standalone XR settings for Editor play mode XR

## [7.1.8-preview] - 2020-03-18

- Fix Test Result Window

## [7.1.7-preview] - 2020-03-17

- Change MockHMD folder to None for playmode

## [7.1.6-preview] - 2020-03-16

- Improved messaging in GC Alloc
- Test filters no longer override disabled tests in build settings
- Adds a check so if vr is supported and that array is empty, set xrsdk to MockHMD

## [7.1.5-preview] - 2020-02-14

- Fixing issues where Standalone tests wouldn't work for some projects

## [7.1.4-preview] - 2020-02-13

- Adding GC Alloc changes for HDRP

## [7.1.3-preview] - 2019-11-25

- Updating dependency names

## [7.1.2-preview] - 2019-11-04

- Adding com.unity.nuget.test-protocol and com.unity.newtonsoft-json as dependencies

## [7.1.1-preview] - 2019-09-23

- Adding script for testing with different Graphics APIs

## [7.1.0-preview] - 2019-09-09

- Separated Graphics Test Framework into its own repository

## [6.6.0] - 2019-04-01

## [6.5.0] - 2019-03-07

## [6.4.0] - 2019-02-21

## [6.3.0] - 2019-02-18

## [6.2.0] - 2019-02-15

## [6.1.0] - 2019-02-13

## [6.0.0] - 2019-02-23

## [5.2.0] - 2018-11-27

## [5.1.0] - 2018-11-18

## [5.0.0-preview] - 2018-09-28

## [4.0.0-preview] - 2019-09-21

## [3.3.0] - 2018-08-03

## [3.2.0] - 2018-07-30

## [3.1.0] - 2018-07-26

## [0.1.0] - 2018-05-04

### This is the first release of *Unity Package com.unity.testframework.graphics*

- ImageAssert for comparing images
- Automatic management of reference images and test case generation
