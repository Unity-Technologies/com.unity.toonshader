using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor.Rendering.Toon
{
    internal sealed class UniversalUTS2toIntegratedUTS3Converter : RenderPipelineConverterContainer
    {
        public override string name => "Universal Toon Shader";
        public override string info => "This tool converts project elements from Universal Toon Shader to Unity Toon Shader " + UTS3ConverterWindow.versionString;
        public override int priority => -9000;
    }
}