using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Toon
{
#if SRPCORE_NEWERTHAN12_IS_INSTALLED_FOR_UTS
#else
    internal struct UTS3MaterialHeaderScopeItem
    {
        /// <summary><see cref="GUIContent"></see> that will be rendered on the <see cref="MaterialHeaderScope"></see></summary>
        public GUIContent headerTitle { get; set; }
        /// <summary>The bitmask for this scope</summary>
        public uint expandable { get; set; }
        /// <summary>The action that will draw the controls for this scope</summary>
        public Action<Material> drawMaterialScope { get; set; }
        /// <summary>The url of the scope</summary>
        public string url { get; set; }
    }

#endif // SRPCORE_NEWERTHAN12_IS_INSTALLED_FOR_UTS
}
