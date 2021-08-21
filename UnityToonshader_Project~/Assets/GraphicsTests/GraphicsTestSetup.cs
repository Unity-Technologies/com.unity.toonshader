using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Tests;
using System.Linq;

namespace UnityEditor.Rendering.Toon
{
    public class GraphicsTestSetup : EditorWindow
    {
        // https://docs.unity3d.com/ScriptReference/EditorBuildSettings-scenes.html
        Vector2 m_scrollPos;
        bool m_initialzed;
        List<EditorBuildSettingsScene> m_SceneAssets = new List<EditorBuildSettingsScene>();
        List<string> m_SceneNames = new List<string>();

        string[] monobehavioursToDisable =
        {
            "Rotator",
            "IdleChanger",
            "AutoBlink",
            "AutoBlinkforSD",
            "FaceUpdate",
            "IdleChanger",
            "IKCtrlRightHand",
            "RandomWind",
            "RefleshProbe",
            "SpringManager",
            "Animation",
            "Animator"
        };

        [MenuItem("Window/Toon Shader/Graphics Test Component Attacher", false, 9999)]
        static private void OpenWindow()
        {
            var window = GetWindow<GraphicsTestSetup>(true, "Graphics Test Setup");
            window.Show();
        }
        private void OnGUI()
        {
            if (!m_initialzed)
            {

                for (int ii = 0; ii < EditorBuildSettings.scenes.Length; ii++)
                {
                    m_SceneNames.Add(EditorBuildSettings.scenes[ii].ToString());

                }
                m_initialzed = true;
            }


            // scroll view 
            m_scrollPos =
                 EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(position.width - 4));
            EditorGUILayout.BeginVertical();


            int sceneCount = 0;

            for (int ii = 0; ii < EditorBuildSettings.scenes.Length; ii++)
            {
                var guid = EditorBuildSettings.scenes[ii].guid;
                string path = EditorBuildSettings.scenes[ii].path;
                if (EditorBuildSettings.scenes[ii].enabled )
                {
                    SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

                    sceneCount++;

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(16);
                    string str = "" + sceneCount + ":";

                    EditorGUILayout.LabelField(str, GUILayout.Width(40));
                    EditorGUILayout.LabelField(scene.name, GUILayout.Width(Screen.width - 130));
                    GUILayout.Space(1);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();



            using (new EditorGUI.DisabledScope(EditorBuildSettings.scenes.Length == 0))
            {
                EditorGUILayout.BeginHorizontal();
                if ( GUILayout.Button("Set up scenes above.") )
                {
                    SetupScenes();
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        void SetupScenes()
        {
            var scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[3].path);
            var cameras = GameObject.FindGameObjectsWithTag("MainCamera").Select(x => x.GetComponent<Camera>());
            var cameraList = cameras.ToList<Camera>();

            LegacylUTS_GraphicsTestSettings settings = cameraList[0].gameObject.GetComponent<LegacylUTS_GraphicsTestSettings>();


            if ( settings == null )
            {
                settings = cameraList[0].gameObject.AddComponent<LegacylUTS_GraphicsTestSettings>();
            }
            settings.ImageComparisonSettings.ImageResolution = UnityEngine.TestTools.Graphics.ImageComparisonSettings.Resolution.w960h540;
            settings.CheckMemoryAllocation = false;
            foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
            {
                if (! obj.activeInHierarchy )
                {
                    continue;
                }
                for ( int jj = 0; jj < monobehavioursToDisable.Length; jj++)
                {
                    var component = obj.GetComponent(monobehavioursToDisable[jj]);
                    var mb = component as Behaviour;
                    if (mb == null )
                    {
                        continue;
                    }
                    mb.enabled = false;
                }
                var renderer = obj.GetComponent<Renderer>();

                for ( int kk = 0; kk < renderer.materials.Length; kk++)
                {
                    if (renderer.materials[kk] == null )
                    {
                        continue;
                    }
                    if (renderer.materials[kk].HasProperty("_EMISSIVE_ANIMATION"))
                    {
                        renderer.materials[kk].SetFloat("_EMISSIVE", 0);
                        renderer.materials[kk].EnableKeyword("_EMISSIVE_SIMPLE");
                        renderer.materials[kk].DisableKeyword("_EMISSIVE_ANIMATION");
                    }
                }
            }

            EditorSceneManager.SaveScene(scene);
        }
    }
}