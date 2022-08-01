using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
namespace UnityEditor.Rendering.Toon
{
    internal sealed class BuiltInUTS2toIntegratedConverter : RenderPipelineConverterContainer
    {
        public override string name => "Unity-chan Toon Shader 2";
        public override string info => "This tool converts project materials from Unity-chan Toon Shader to Unity Toon Shader " + UTS3Converter.versionString;
        public override int priority => -9000;

        public override void SetupConverter() {

//            bool isUts2Installed = CheckUTS2isInstalled();
//            bool isUts2SupportedVersion = CheckUTS2VersionError();

            int materialCount = 0;

            for (int ii = 0; ii < m_materialGuids.Length; ii++)
            {
                var guid = m_materialGuids[ii];

                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                var shaderName = material.shader.ToString();
#if false
                if (!shaderName.StartsWith("Hidden/InternalErrorShader"))
                {
                    continue;
                }
#endif
                string content = File.ReadAllText(path);
                string[] lines = content.Split(lineSeparators, StringSplitOptions.None);
                // always two spaces before m_Shader?
                var targetLine = Array.Find<string>(lines, line => line.StartsWith("  m_Shader:"));
                if (targetLine == null)
                {
                    continue; // todo. prefab?
                }
                var shaderMetadata = targetLine.Split(targetSepeartors, StringSplitOptions.None);
                if (shaderMetadata.Length < 4)
                {
                    Error(path);
                    continue;
                }
                var shaderGUID = shaderMetadata[4];
                while (shaderGUID.StartsWith(" "))
                {
                    shaderGUID = shaderGUID.TrimStart(' ');
                }
                var foundUTS2GUID = FindUTS2GUID(shaderGUID);
                if (foundUTS2GUID == null)
                {
                    continue;
                }

                var targetLine2 = Array.Find<string>(lines, line => line.StartsWith("    - _utsVersion"));
                if (targetLine2 == null)
                {
                    Error(path);
                    continue;
                }
                string[] lines2 = targetLine2.Split(targetSepeartors2, StringSplitOptions.None);
                if (lines2 == null || lines2.Length < 2)
                {
                    Error(path);
                    m_versionErrorCount++;
                    continue;
                }
                var utsVersionString = lines2[1];
                while (utsVersionString.StartsWith(" "))
                {
                    utsVersionString = utsVersionString.TrimStart(' ');
                }
                float utsVersion = float.Parse(utsVersionString);
                if (utsVersion < 2.07f)
                {
                    m_versionErrorCount++;
                    continue;
                }
                m_ConvertingMaterials.Add(material);
                if (!m_Material2GUID_Dictionary.ContainsKey(material))
                {
                    m_Material2GUID_Dictionary.Add(material, shaderGUID);
                }
                if (!m_GuidToUTSID_Dictionary.ContainsKey(shaderGUID))
                {
                    m_GuidToUTSID_Dictionary.Add(shaderGUID, foundUTS2GUID);
                }
                materialCount++;

                AddMaterialToScrollview(material);
            }

        }
        public override void Convert() { }
        public override void PostConverting() { }


        const string legacyShaderPrefix = "UnityChanToonShader/";

        bool CheckUTS2VersionError()
        {
            
            int materialCount = 0;

            for (int ii = 0; ii < m_materialGuids.Length; ii++)
            {
                var guid = m_materialGuids[ii];


                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

                var shaderName = material.shader.ToString();
                if (!shaderName.StartsWith(legacyShaderPrefix))
                {
                    continue;

                }

                if (material.HasProperty(utsVersionProp))
                {
                    float utsVersion = material.GetFloat(utsVersionProp);
                    if (utsVersion < 2.07)
                    {
                        m_versionErrorCount++;
                        continue;
                    }
                }
                else
                {
                    m_versionErrorCount++;
                    continue;
                }
                materialCount++;
            }
            m_materialCount = materialCount;
            if (m_versionErrorCount > 0)
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

        public override int CountErrors(bool addToScrollView) 
        {
            Debug.Assert(UTS3Converter.scrollView != null);

            m_versionErrorCount = 0;

            for (int ii = 0; ii < m_materialGuids.Length; ii++)
            {

                var guid = m_materialGuids[ii];

                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                var shaderName = material.shader.ToString();
#if false
                if (!shaderName.StartsWith("Hidden/InternalErrorShader"))
                {
                    continue;
                }
#endif
                string content = File.ReadAllText(path);
                string[] lines = content.Split(lineSeparators, StringSplitOptions.None);
                // always two spaces before m_Shader?
                var targetLine = Array.Find<string>(lines, line => line.StartsWith("  m_Shader:"));
                if (targetLine == null)
                {
                    continue; // todo. prefab?
                }
                var shaderMetadata = targetLine.Split(targetSepeartors, StringSplitOptions.None);
                if (shaderMetadata == null)
                {
                    continue;
                }
                if (shaderMetadata.Length < 4)
                {
                    m_versionErrorCount++;
                    Error(path);
                    continue;
                }
                var shaderGUID = shaderMetadata[4];
                while (shaderGUID.StartsWith(" "))
                {
                    shaderGUID = shaderGUID.TrimStart(' ');
                }
                var foundUTS2GUID = FindUTS2GUID(shaderGUID);
                if (foundUTS2GUID == null)
                {
                    continue;       // Not Unity-chan Toon Shader Ver 2.
                }

                var targetLine2 = Array.Find<string>(lines, line => line.StartsWith("    - _utsVersion"));
                if (targetLine2 == null)
                {
                    m_versionErrorCount++;
                    if (addToScrollView)
                        AddMaterialToScrollview(material);
                    continue;
                }
                string[] lines2 = targetLine2.Split(targetSepeartors2, StringSplitOptions.None);
                if (lines2 == null || lines2.Length < 2)
                {
                    m_versionErrorCount++;
                    if (addToScrollView)
                        AddMaterialToScrollview(material);
                    continue;
                }
                var utsVersionString = lines2[1];
                while (utsVersionString.StartsWith(" "))
                {
                    utsVersionString = utsVersionString.TrimStart(' ');
                }
                float utsVersion = float.Parse(utsVersionString);
                if (utsVersion < 2.07f)
                {
                    m_versionErrorCount++;
                    if (addToScrollView)
                        AddMaterialToScrollview(material);
                    continue;
                }

            }
            return m_versionErrorCount;
        }
        public override InstalledStatus CheckSourceShaderInstalled() { return InstalledStatus.NotInstalled; }
    }
}