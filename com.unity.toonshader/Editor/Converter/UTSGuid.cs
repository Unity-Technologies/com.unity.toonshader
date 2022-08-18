using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.IO;
using System.Text;

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
    enum UTS2RenderQueue
    {
        None,
        AlphaTestMinus1,
        AlphaTest,
        Transparent,
    };

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
//        internal bool m_transparency;
        internal UTS2RenderQueue m_renderQueue;
        internal UTS3GUI.UTS_StencilMode m_stencilMode;
        internal int m_clippingMode;

        internal UTS2INFO(string guid, string shaderName, string renderType, bool transparency, UTS2RenderQueue renderQueue, UTS3GUI.UTS_StencilMode stencilMode, int clippingMode ) : base(guid, shaderName)
        {
            m_renderType = renderType;
//            m_transparency = transparency;
            m_renderQueue = renderQueue;
            m_stencilMode = stencilMode;
            m_clippingMode = clippingMode;
        }

        internal int clippingMode
        {
            get
            {
                return m_clippingMode;
#if false
                if (m_ShaderName.Contains("TransClipping"))
                {
                    return 2;
                }
                if (m_ShaderName.Contains("Clipping"))
                {
                    return 1;
                }
                return 0;
#endif
            }
        }

        internal string GetConstructorString()
        {
            StringBuilder sb = new StringBuilder("new UTS2INFO(", 1024);
            sb.AppendFormat("\"{0}\",\"{1}\",\"{2}\",transparency:{3},UTS2RenderQueue.{4},UTS3GUI.UTS_StencilMode.{5},{6}", 
                m_Guid, 
                m_ShaderName, 
                m_renderType, 
                "false",
                m_renderQueue,
                m_stencilMode, 
                m_clippingMode );
            sb.Append("),");
            return sb.ToString();
        }
    }

}