using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.IO;

namespace UnityEditor.Rendering.Toon
{
    internal class UTSGUID
    {
        internal UTSGUID(string guid, string shaderName)
        {
            m_ShaderName = shaderName;
            m_Guid = guid;
        }
        internal string m_ShaderName;
        internal string m_Guid;
    }

    internal class UTS2INFO : UTSGUID
    {

        internal const string OPAQUE = "Opaque";
        internal const string TRANSPARENTCUTOUT = "TransparentCutOut";
        internal const string TRANSPARENT = "Transparent";
        internal const string RENDERTYPE = "RenderType";
        internal const string IGNOREPROJECTION = "IgnoreProjection";
        internal const string DO_IGNOREPROJECTION = "True";
        internal const string DONT_IGNOREPROJECTION = "False";
        internal string m_renderType;
        internal bool m_transparency;
        internal UTS2INFO(string guid, string shaderName, string renderType, bool transparency  ) : base(guid, shaderName)
        {
            m_renderType = renderType;
            m_transparency = transparency;
        }
        internal int clippingMode
        {
            get
            {
                if (m_ShaderName.Contains("TransClipping"))
                {
                    return 2;
                }
                if (m_ShaderName.Contains("Clipping"))
                {
                    return 1;
                }
                return 0;
            }
        }
    }

}