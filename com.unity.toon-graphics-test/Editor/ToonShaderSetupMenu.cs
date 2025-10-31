using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;
using Unity.ToonShader.GraphicsTest;
using UnityEngine.SceneManagement;

namespace UnityEditor.Rendering.Toon {
    public class ToonShaderSetupMenu {
        [MenuItem("Toon Shader/Setup Test Settings in All Scenes")]
        private static void SetupTestSettingsInAllScenes()
        {
            bool proceed = EditorUtility.DisplayDialog(
                "Setup Test Settings",
                "Are you sure you want to setup test settings in all scenes?",
                "OK",
                "Cancel"
            );

            if (!proceed)
                return;
            
            string testSettingsSOPath = "Packages/com.unity.toon-graphics-test/Runtime/UTSGraphicsSettings.asset"; 
            UTSGraphicsTestSettingsSO testSettingsSO = AssetDatabase.LoadAssetAtPath<UTSGraphicsTestSettingsSO>(
                testSettingsSOPath);

            if (null == testSettingsSO) {
                Debug.LogError("Test settings not found: " + testSettingsSOPath);
                return;
            }
            
            foreach (EditorBuildSettingsScene sceneSettings in EditorBuildSettings.scenes) {
                Scene scene = EditorSceneManager.OpenScene(sceneSettings.path);
                
                Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                UTSGraphicsTestSettings testSettings = mainCamera.GetComponent<UTSGraphicsTestSettings>();
                testSettings.SO = testSettingsSO;
                EditorSceneManager.SaveScene(scene);
            }
        }
        
    }
}