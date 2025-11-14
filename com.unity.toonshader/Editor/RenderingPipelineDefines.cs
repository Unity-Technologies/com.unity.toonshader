// RenderingPipelineDefines.cs
// https://gist.github.com/cjaube/944b0d5221808c2a761d616f29deaf49
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine.Rendering;

[InitializeOnLoad]
internal class RenderingPipelineDefines
{
    enum PipelineType
    {
        Unsupported,
        BuiltInPipeline,
        UniversalPipeline,
        HDPipeline
    }

    static RenderingPipelineDefines()
    {
        UpdateDefines();
    }

    /// <summary>
    /// Update the unity pipeline defines for URP
    /// </summary>
    static void UpdateDefines()
    {
        var pipeline = GetPipeline();

        if (pipeline == PipelineType.UniversalPipeline)
        {
            AddDefine("UNITY_PIPELINE_URP");
        }
        else
        {
            RemoveDefine("UNITY_PIPELINE_URP");
        }
        if (pipeline == PipelineType.HDPipeline)
        {
            AddDefine("UNITY_PIPELINE_HDRP");
        }
        else
        {
            RemoveDefine("UNITY_PIPELINE_HDRP");
        }
    }


    /// <summary>
    /// Returns the type of renderpipeline that is currently running
    /// </summary>
    /// <returns></returns>
    static PipelineType GetPipeline()
    {
#if UNITY_2019_1_OR_NEWER
        if (GraphicsSettings.defaultRenderPipeline != null)
        {
            // SRP
            var srpType = GraphicsSettings.defaultRenderPipeline.GetType().ToString();
            if (srpType.Contains("HDRenderPipelineAsset"))
            {
                return PipelineType.HDPipeline;
            }
            else if (srpType.Contains("UniversalRenderPipelineAsset") || srpType.Contains("LightweightRenderPipelineAsset"))
            {
                return PipelineType.UniversalPipeline;
            }
            else return PipelineType.Unsupported;
        }
#elif UNITY_2017_1_OR_NEWER
        if (GraphicsSettings.renderPipelineAsset != null) {
            // SRP not supported before 2019
            return PipelineType.Unsupported;
        }
#endif
        // no SRP
        return PipelineType.BuiltInPipeline;
    }

    /// <summary>
    /// Add a custom define
    /// </summary>
    /// <param name="define"></param>
    /// <param name="buildTargetGroup"></param>
    static void AddDefine(string define)
    {
        var definesList = GetDefines();
        if (!definesList.Contains(define))
        {
            definesList.Add(define);
            SetDefines(definesList);
        }
    }

    /// <summary>
    /// Remove a custom define
    /// </summary>
    /// <param name="define"></param>
    public static void RemoveDefine(string define)
    {
        var definesList = GetDefines();
        if (definesList.Contains(define))
        {
            definesList.Remove(define);
            SetDefines(definesList);
        }
    }

    static List<string> GetDefines() {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);

        string defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
        return defines.Split(';').ToList();
    }

    static void SetDefines(List<string> definesList) {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
        string defines = string.Join(";", definesList.ToArray());
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines);
    }
}