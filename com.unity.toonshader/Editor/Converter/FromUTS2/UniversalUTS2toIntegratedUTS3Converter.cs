using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UnityEditor.Rendering.Toon
{
    internal sealed class UniversalUTS2toIntegratedUTS3Converter : RenderPipelineConverterContainer
    {
        public override string name => "Universal Toon Shader";
        public override string info => "This tool converts project materials from Universal Toon Shader to Unity Toon Shader " + UTS3Converter.versionString;
        public override int priority => -9000;

        public override void SetupConverter(ScrollView scrollView) { }
        public override void Convert() { }
        public override void PostConverting() { }
    }
}