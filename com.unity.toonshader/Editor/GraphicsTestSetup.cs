using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.Rendering.Toon
{
    public class GraphicsTestSettingsAttacher : EditorWindow
    {
        // https://docs.unity3d.com/ScriptReference/EditorBuildSettings-scenes.html
        Vector2 m_scrollPos;
        bool m_initialzed;
        List<EditorBuildSettingsScene> m_SceneAssets = new List<EditorBuildSettingsScene>();
        List<string> m_SceneNames = new List<string>();
        [MenuItem("Window/Toon Shader/Graphics Test Component Attacher", false, 9999)]
        static private void OpenWindow()
        {
            var window = GetWindow<GraphicsTestSettingsAttacher>(true, "Graphics Test Component Attacher");
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

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();



            using (new EditorGUI.DisabledScope(EditorBuildSettings.scenes.Length == 0))
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Button("Start");
                //            EditorGUILayout.LabelField("Convert to ");
                //            m_selectedRenderPipeline = EditorGUILayout.Popup(m_selectedRenderPipeline, m_RendderPipelineNames);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}