using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Rendering.Toon
{
    internal abstract class RenderPipelineConverterContainer 
    {
        protected int m_materialCount = 0;
        protected string[] m_guids;
        protected int m_versionErrorCount = 0;

        protected readonly string[] lineSeparators = new[] { "\r\n", "\r", "\n" };
        protected readonly string[] targetSepeartors = new[] { ":", "," };
        protected readonly string[] targetSepeartors2 = new[] { ":" };

        protected List<Material> m_ConvertingMaterials = new List<Material>();
        protected Dictionary<Material, string> m_Material2GUID_Dictionary = new Dictionary<Material, string>();
        protected Dictionary<string, UTSGUID> m_GuidToUTSID_Dictionary = new Dictionary<string, UTSGUID>();

        protected ScrollView m_ScrollView;

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


        public abstract void SetupConverter(ScrollView scrollView);
        public abstract void Convert();
        public abstract void PostConverting();
    }
}