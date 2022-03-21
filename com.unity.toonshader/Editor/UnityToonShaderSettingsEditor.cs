using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using System.Text;

namespace UnityEditor.Rendering.Toon
{
    [CustomEditor(typeof(UnityToonShaderSettings))]
    public class UnityToonShaderSettingsEditor : Editor
    {
        internal class Styles
        {
            public static readonly GUIContent converterWindowLabel = new GUIContent("Show Converter Window on Startup", "TBD.");

        }

        SerializedProperty m_ConverterWindowOnStartup;

        public void OnEnabe()
        {
            if (target == null)
            {
                return;
            }

        }
 
    }
    class ToonShaderResourceImporterProvider : SettingsProvider
    {

        public ToonShaderResourceImporterProvider()
            : base("Project/ToonShader", SettingsScope.Project)
        {
        }

        public override void OnGUI(string searchContext)
        {
            // Lazy creation that supports domain reload
            //    if (m_ResourceImporter == null)
            //        m_ResourceImporter = new StoryboardResouceImporeter();

            //            m_ResourceImporter.OnGUI();
        }
        public override void OnDeactivate()
        {
            //if (m_ResourceImporter != null)
            //m_ResourceImporter.OnDestroy();
        }
        static UnityEngine.Object GetToonShaderSettings()
        {
            return UnityToonShaderSettings.instance;
        }

        [SettingsProviderGroup]
        static SettingsProvider[] CreateToonShaderSettingsProvider()
        {
            var providers = new List<SettingsProvider> { new ToonShaderResourceImporterProvider() };

            if (GetToonShaderSettings() != null)
            {
                var provider = new AssetSettingsProvider("Project/ToonShader/Settings", GetToonShaderSettings);
                provider.PopulateSearchKeywordsFromGUIContentProperties<UnityToonShaderSettingsEditor.Styles>();
                providers.Add(provider);
            }

            return providers.ToArray();
        }
    }

}
