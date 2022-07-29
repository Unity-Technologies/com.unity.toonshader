using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UnityEditor.Rendering.Toon
{
    internal sealed class BuiltInUTS2toIntegratedConverter : RenderPipelineConverterContainer
    {
        public override string name => "Unity-chan Toon Shader 2";
        public override string info => "This tool converts project materials from Unity-chan Toon Shader to Unity Toon Shader " + UTS3Converter.versionString;
        public override int priority => -9000;

        public override void SetupConverter(ScrollView scrollView) {
            bool isUts2Installed = CheckUTS2isInstalled();
            bool isUts2SupportedVersion = CheckUTS2VersionError();
        }
        public override void Convert() { }
        public override void PostConverting() { }


        const string legacyShaderPrefix = "UnityChanToonShader/";

        bool CheckUTS2VersionError()
        {
            s_guids = AssetDatabase.FindAssets("t:Material", null);
            int materialCount = 0;

            for (int ii = 0; ii < s_guids.Length; ii++)
            {
                var guid = s_guids[ii];


                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

                var shaderName = material.shader.ToString();
                if (!shaderName.StartsWith(legacyShaderPrefix))
                {
                    continue;

                }
                const string utsVersionProp = "_utsVersion";
                if (material.HasProperty(utsVersionProp))
                {
                    float utsVersion = material.GetFloat(utsVersionProp);
                    if (utsVersion < 2.07)
                    {
                        s_versionErrorCount++;
                        continue;
                    }
                }
                else
                {
                    s_versionErrorCount++;
                    continue;
                }
                materialCount++;
            }
            s_materialCount = materialCount;
            if (s_versionErrorCount > 0)
            {
                return true;
            }
            return false;
        }

        bool CheckUTS2isInstalled()
        {
            var shaders = AssetDatabase.FindAssets("t:Shader", new string[] { "Assets" });
            foreach (var guid in shaders)
            {
                foreach (var shader in UTS2ShaderInfo.stdShaders)
                {
                    if (guid == shader.m_Guid)
                    {
                        /*
                                            var filename = AssetDatabase.GUIDToAssetPath(guid);

                                            if (!filename.EndsWith(kLegacyShaderFileName + kShaderFileNameExtention))
                                            {
                                                return true;
                                            }
                        */
                        return true;

                    }
                }
                foreach (var shader in UTS2ShaderInfo.tessShaders)
                {
                    if (guid == shader.m_Guid)
                    {
                        /*
                                            var filename = AssetDatabase.GUIDToAssetPath(guid);

                                            if (!filename.EndsWith(kLegacyShaderFileName + kShaderFileNameExtention))
                                            {
                                                return true;
                                            }
                        */
                        return true;

                    }
                }
            }
            return false;
        }
        UTSGUID FindUTS2GUID(string guid)
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