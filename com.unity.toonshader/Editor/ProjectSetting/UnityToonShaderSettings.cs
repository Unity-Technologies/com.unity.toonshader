using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace UnityEditor.Rendering.Toon
{
    internal class UnityToonShaderSettings : ScriptableSingleton<UnityToonShaderSettings>
    {
        [SerializeField]
        internal bool m_ShowConverter = true;
    }
}

