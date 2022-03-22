using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using System.Text;
using UnityEngine.UIElements;
namespace UnityEditor.Rendering.Toon
{
    internal  class ProjectSettingProvider : SettingsProvider
    {
        #region fields
        internal int m_ContentWidth = 1920;
        internal int m_ContentHeight = 1080;

        internal float m_CanvasScaleX = 1.5f;
        internal float m_CanvasScaleY = 1.5f;

        GUIContent guiContentWidth = new GUIContent("Content Width");
        GUIContent guiContentHeight = new GUIContent("Content Height");
        GUIContent guiCanvasScaleX = new GUIContent("Canvas Scale X");
        GUIContent guiCanvasScaleY = new GUIContent("Canvas Scale Y");



        #endregion

        [SettingsProvider]
        private static SettingsProvider CreateProjectSettingsProvider()
        {
            var path = "Project/Unity Toon Shader";

            var provider = new ProjectSettingProvider(path, SettingsScope.Project);
            provider.keywords = new[] { "Toon Shader", "ToonShader", "UTS" };

            return provider;
        }

        internal ProjectSettingProvider(string path, SettingsScope scope)
            : base(path, scope)
        {



        }


        public override void OnActivate
        (
            string searchContext,
            VisualElement rootElement
        )
        {

        }


        public override void OnDeactivate()
        {

        }

        public override void OnGUI(string searchContext)
        {
            using (new GUIScope())
            {
 
                EditorGUILayout.BeginVertical();
                var strContentWidth = m_ContentWidth.ToString();
                var strContentHeight = m_ContentHeight.ToString();

                var strCanvasWidth = m_CanvasScaleX.ToString();
                var strCanvasHeight = m_CanvasScaleY.ToString();

                strContentWidth = EditorGUILayout.TextField(guiContentWidth, strContentWidth);
                strContentHeight = EditorGUILayout.TextField(guiContentHeight, strContentHeight);
                EditorGUILayout.Space();
                strCanvasWidth = EditorGUILayout.TextField(guiContentWidth, strCanvasWidth);
                strCanvasHeight = EditorGUILayout.TextField(guiContentHeight, strCanvasHeight);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();


                if (GUILayout.Button("Revert"))
                {

                }
                if (GUILayout.Button("Apply"))
                {
                    AssetDatabase.SaveAssets();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

            }
        }


        public override void OnTitleBarGUI()
        {
            //            EditorGUILayout.LabelField("");
        }



        public override void OnFooterBarGUI()
        {
            EditorGUILayout.LabelField("Settings become effective after pressing Apply button.");
        }


    }
    internal class GUIScope : GUI.Scope
    {
        float m_LabelWidth;
        public GUIScope(float layoutMaxWidth)
        {
            m_LabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            GUILayout.Space(15);
        }

        public GUIScope() : this(500)
        {
        }

        protected override void CloseScope()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = m_LabelWidth;
        }
    }
}