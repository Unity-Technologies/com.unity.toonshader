using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor.Rendering.Toon
{
    internal sealed class HdrpUTS3toIntegratedUTS3Converter : RenderPipelineConverterContainer
    {
        public override string name => "Unity Toon Shader(HDRP) 0.7.x or older";
        public override string info => "This tool converts project elements from Unity Toon Shader 0.7.x or older to Unity Toon Shader " + UTS3Converter.versionString;
        public override int priority => -9000;

    }
}