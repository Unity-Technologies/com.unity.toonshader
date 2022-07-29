using UnityEngine.UIElements;

namespace UnityEditor.Rendering.Toon
{
    internal abstract class RenderPipelineConverterContainer 
    {
        protected int s_materialCount = 0;
        protected string[] s_guids;
        protected int s_versionErrorCount = 0;

        protected ScrollView m_ScrollView;
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