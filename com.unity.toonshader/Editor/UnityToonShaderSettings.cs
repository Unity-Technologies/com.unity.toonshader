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
    [System.Serializable]
    [ExcludeFromPresetAttribute]
    internal class UnityToonShaderSettings : ScriptableObject
    {
        private const string kPrefabPath = "Assets/ToonShader/Prefabs/";
        private const string kSettingFilePath = "Assets/ToonShader/StoryboardSetting.asset";
        private static UnityToonShaderSettings s_Instance;
        static public string settingFileInstalledPath
        {
            get => kSettingFilePath;
        }

        [SerializeField]
        bool m_ConverterWindowOnStartup;
        public static UnityToonShaderSettings instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var settings = AssetDatabase.LoadAssetAtPath<UnityToonShaderSettings>(kSettingFilePath);
                    s_Instance = settings;

                }

                return s_Instance;
            }
        }

    }
}