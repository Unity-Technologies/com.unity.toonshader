using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.TestRunner;
using System;
using System.Reflection;

// SRPs now control screen space UI overlays rendering by default, for all types of UI (IMGUI, uGUI, UIToolkit).
// UTF-generated scene contains a IMGUI component whose OnGUI() callback triggers dynamic mem alloc while running SRPs tests,
// therefore creating false negatives when we check memory allocation in SRPs.
// To prevent that, this script temporarily disables this IMGUI component (no more OnGUI()) while running SRP tests
// to re-enable it only at the end of the test run to correctly display the test results.
[assembly:TestRunCallback(typeof(CustomTestRunCallback))]

public class CustomTestRunCallback : ITestRunCallback
{
    // Retrieve through reflection UTF types and method
    readonly static Type UTFPlayModeTestControllerType = Type.GetType("UnityEngine.TestTools.TestRunner.PlaymodeTestsController, UnityEngine.TestRunner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"); // type: PlayModeRunner
    readonly static Type UTFPlayModeCallbackRunnerType = Type.GetType("UnityEngine.TestTools.TestRunner.Callbacks.PlayModeRunnerCallback, UnityEngine.TestRunner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"); // type: PlayModeRunnerCallback
    readonly static MethodInfo UTFIsControllerOnSceneMethodInfo = UTFPlayModeTestControllerType?.GetMethod("IsControllerOnScene", BindingFlags.Static | BindingFlags.NonPublic); // type: IsControllerOnScene
    readonly static MethodInfo UTFGetControllerMethodInfo = UTFPlayModeTestControllerType?.GetMethod("GetController", BindingFlags.Static | BindingFlags.NonPublic); // type: GetController

    void EnableOnGUICallbackInUTF(bool enable)
    {
        // Invoke static IsControllerOnScene method to check whether UTF PlayModeTestsController exists (PlayMode, Standalone)
        bool isControllerOnScene = (bool) UTFIsControllerOnSceneMethodInfo?.Invoke(null, null);
    
        if(isControllerOnScene)
        {
            // Invoke static GetController method to obtain existing UTF PlayModeTestsController
            object playModeTestControllerObj = UTFGetControllerMethodInfo?.Invoke(null, null);

            if(playModeTestControllerObj is MonoBehaviour monoBehaviour && UTFPlayModeCallbackRunnerType != null)
            {
                // Retrieve IMGUI component from UTF PlayModeTestController
                MonoBehaviour IMGUIComponent = (MonoBehaviour) monoBehaviour.GetComponent(UTFPlayModeCallbackRunnerType);
                
                if(IMGUIComponent != null)
                {
                    IMGUIComponent.enabled = enable;
                }
            }
        }
    }

    // UTF run is about to start, disabling OnGUI() to prevent mem alloc in render loop
    public void RunStarted(ITest testsToRun)
    {
        EnableOnGUICallbackInUTF(false);
    }

    // UTF run has just finished, enabling OnGUI() back to display test results
    public void RunFinished(ITestResult testResults)
    {
        EnableOnGUICallbackInUTF(true);
    }

    public void TestStarted(ITest test)
    {
    }

    public void TestFinished(ITestResult result)
    {
    }
}