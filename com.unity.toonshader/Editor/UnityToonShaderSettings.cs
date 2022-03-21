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
    internal class UnityToonShaderSettings : ScriptableSingleton<UnityToonShaderSettings>
    {
        private static UnityToonShaderSettings s_Instance;


        bool m_ConverterWindowOnStartup;
        public bool converterWindowOnStartup
        {
            get => m_ConverterWindowOnStartup;
            set => m_ConverterWindowOnStartup = value;
        }

        void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            Save(true);
        }

        internal SerializedObject GetSerializedObject()
        {
            return new SerializedObject(this);
        }

        private void OnValidate()
        {

        }


    }
}