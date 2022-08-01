using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Rendering.Toon
{
    internal abstract class RenderPipelineConverterContainer
    {
        public enum InstalledStatus
        {
            NotInstalled,
            InstalledUnsupportedVersion,
            Installed
        };
        protected InstalledStatus m_sourceShaderInstalledStatus;
        protected int m_materialCount = 0;
        protected string[] m_materialGuids;
        protected int m_versionErrorCount = 0;

        protected readonly string[] lineSeparators = new[] { "\r\n", "\r", "\n" };
        protected readonly string[] targetSepeartors = new[] { ":", "," };
        protected readonly string[] targetSepeartors2 = new[] { ":" };

        protected List<Material> m_ConvertingMaterials = new List<Material>();
        protected Dictionary<Material, string> m_Material2GUID_Dictionary = new Dictionary<Material, string>();
        protected Dictionary<string, UTSGUID> m_GuidToUTSID_Dictionary = new Dictionary<string, UTSGUID>();


        protected const string utsVersionProp = "_utsVersion";
        protected void Error(string path)
        {
            Debug.LogErrorFormat("File: {0} is corrupted.", path);
        }

        /// <summary>
        /// The name of the container. This name shows up in the UI.
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The description of the container.
        /// It is shown in the UI. Describe the converters in this container.
        /// </summary>
        public abstract string info { get; }

        /// <summary>
        /// The priority of the container. The lower the number (can be negative), the earlier Unity executes the container, and the earlier it shows up in the converter container menu.
        /// </summary>
        public virtual int priority => 0;


        public abstract void SetupConverter();
        public abstract void Convert();
        public abstract void PostConverting();

        public abstract int CountErrors(bool addToScrollView);
        public abstract InstalledStatus CheckSourceShaderInstalled();

        public void Reset()
        {
            m_materialCount = 0;
            m_ConvertingMaterials.Clear();

            m_versionErrorCount = 0;
            m_ConvertingMaterials.Clear();
            m_Material2GUID_Dictionary.Clear();
            m_GuidToUTSID_Dictionary.Clear();
            UTS3Converter.scrollView.Clear();

        }
        public void CommonSetup()
        {
            Reset();
            Debug.Assert(UTS3Converter.scrollView != null);
            if (m_materialGuids == null)
            {
                m_materialGuids = AssetDatabase.FindAssets("t:Material", null);
            }
            // CheckSourceShaderInstalled(); // Not necessary? 
        }
        /// <summary>
        /// Returns number of materials which are unable to convert.
        /// </summary>


        public void AddMaterialToScrollview(Material material)
        {
            Label item = new Label(material.name);
            UTS3Converter.scrollView.Add(item);
        }

        protected UTSGUID FindUTS2GUID(string guid)
        {
            var ret = Array.Find<UTSGUID>(UTS2ShaderInfo.stdShaders, element => element.m_Guid == guid);
            foreach (var shader in UTS2ShaderInfo.stdShaders)
            {
                if (shader.m_Guid == guid)
                {
                    return shader;
                }
            }
            foreach (var shader in UTS2ShaderInfo.tessShaders)
            {
                if (shader.m_Guid == guid)
                {
                    return shader;
                }
            }
            return null;
        }

    }
}