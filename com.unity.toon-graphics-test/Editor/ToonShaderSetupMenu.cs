using UnityEngine;
using UnityEditor.SceneManagement;
using Unity.ToonShader.GraphicsTest;
using UnityEngine.SceneManagement;

namespace UnityEditor.Rendering.Toon {
internal class ToonShaderSetupMenu {
    [MenuItem("Toon Shader/Remove UTSGraphicsTestSettings component in All Scenes")]
    private static void SetupTestSettingsInAllScenes()
    {
        bool proceed = EditorUtility.DisplayDialog(
            "Setup Test Settings",
            "Proceed in removing UTSGraphicsTestSettings in all scenes ?",
            "OK",
            "Cancel"
        );

        if (!proceed)
            return;

        foreach (EditorBuildSettingsScene sceneSettings in EditorBuildSettings.scenes) {
            Scene scene = EditorSceneManager.OpenScene(sceneSettings.path);

            Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            UTSGraphicsTestSettings testSettings = mainCamera.GetComponent<UTSGraphicsTestSettings>();
            if (null == testSettings) 
                continue; 
            Object.DestroyImmediate(testSettings);
            EditorSceneManager.SaveScene(scene);

        }
    }
}

}