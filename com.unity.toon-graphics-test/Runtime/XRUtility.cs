using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.ToonShader.GraphicsTest {

public static class XRUtility {

#if UNITY_EDITOR
public static void EnableXRInEditor() {

    if (null == XRGeneralSettings.Instance) {
        XRGeneralSettings.Instance = AssetDatabase.LoadAssetAtPath<XRGeneralSettings>(
            "Assets/XR/XRGeneralSettingsPerBuildTarget.asset");
    }

    //Disable everything first
    if (XRGeneralSettings.Instance.Manager.activeLoader ||
        XRGeneralSettings.Instance.Manager.isInitializationComplete)
    {
        DisableXR();
    }

    if (!XRGeneralSettings.Instance.Manager.activeLoader) {
        XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
    }


    if (XRGeneralSettings.Instance.Manager.activeLoader
        && XRGeneralSettings.Instance.Manager.isInitializationComplete)
    {
        XRGeneralSettings.Instance.Manager.StartSubsystems();
    }

    List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
    SubsystemManager.GetSubsystems<XRDisplaySubsystem>(xrDisplaySubsystems);
    int count = xrDisplaySubsystems.Count;
    for (int i = 0; i < count; i++) {
        xrDisplaySubsystems[i].Start();
    }

}

#endif //UNITY_EDITOR

public static void DisableXR() {

    XRManagerSettings xrManager = XRGeneralSettings.Instance?.Manager;
    if (null!= xrManager && xrManager.isInitializationComplete)
    {
        xrManager.StopSubsystems();
        xrManager.DeinitializeLoader();
    }


    List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
    SubsystemManager.GetSubsystems<XRDisplaySubsystem>(xrDisplaySubsystems);
    int count = xrDisplaySubsystems.Count;
    for (int i = 0; i < count; i++) {
        xrDisplaySubsystems[i].Stop();
    }
}

}

} //end namespace
