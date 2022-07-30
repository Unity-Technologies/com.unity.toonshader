using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UnityEditor.Rendering.Toon
{
    internal sealed class UniversalUTS3toIntegratedUTS3Converter : RenderPipelineConverterContainer
    {
        public override string name => "Unity Toon Shader(URP) 0.7.x or older";
        public override string info => "This tool materials project elements from Unity Toon Shader 0.7.x or older to Unity Toon Shader " + UTS3Converter.versionString;
        public override int priority => -9000;

        public override void SetupConverter() { }
        public override void Convert() { }
        public override void PostConverting() { }

        public override InstalledStatus CheckSourceShaderInstalled() { return InstalledStatus.NotInstalled; }
    }
}