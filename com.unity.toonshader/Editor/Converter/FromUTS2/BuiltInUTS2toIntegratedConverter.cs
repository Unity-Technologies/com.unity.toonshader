using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor.Rendering.Toon
{
    internal sealed class BuiltInUTS2toIntegratedConverter : RenderPipelineConverterContainer
    {
        public override string name => "Unity-chan Toon Shader 2";
        public override string info => "This tool converts project elements from Unity-chan Toon Shader to Unity Toon Shader " + UTS3Converter.versionString;
        public override int priority => -9000;
    }
}