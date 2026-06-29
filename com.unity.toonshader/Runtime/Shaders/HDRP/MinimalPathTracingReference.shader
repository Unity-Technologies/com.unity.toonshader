// Minimal Path Tracing Reference Shader for HDRP
// This shader ONLY works with path tracing - it will appear black in normal rendering
// Created as a reference to understand path tracing shader structure

Shader "Reference/MinimalPathTracing"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap("Base Map", 2D) = "white" {}
        _Roughness("Roughness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "10.5.0"
        }

        Tags { "RenderPipeline" = "HDRenderPipeline" }

        Pass
        {
            Name "PathTracingDXR"
            Tags { "LightMode" = "PathTracingDXR" }

            HLSLPROGRAM

            // Required pragmas for path tracing
            #pragma only_renderers d3d11 xboxseries ps5
            #pragma raytracing surface_shader

            // Decals support (required by Material.hlsl)
            #pragma multi_compile DECALS_OFF DECALS_3RT DECALS_4RT

            // Optional features
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ALPHATEST_ON

            // Define the shader pass
            #define SHADERPASS SHADERPASS_PATH_TRACING

            // Shadow quality (required but not actually used in path tracing)
            #define SHADOW_LOW

            // Disable tile and cluster for path tracing
            #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER

            // Include required HDRP headers in correct order (matching Lit.shader exactly)
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingMacros.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracing.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"

            // Include material share pass
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/ShaderPass/LitSharePass.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingIntersection.hlsl"

            // Light loop for path tracing
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
            #define HAS_LIGHTLOOP
            #define PATH_TRACING_CLUSTERED_DECALS
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracingLightLoop.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RayTracingLightCluster.hlsl"

            // RayTracingCommon.hlsl includes RaytracingFragInputs.hlsl - this defines FragInputs
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RayTracingCommon.hlsl"

            // Include Lit material system (provides BSDFData structure)
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitData.hlsl"

            // Include our custom path tracing implementation
            #include "MinimalPathTracingImpl.hlsl"

            // Include Unity's path tracing integrator (does the actual ray tracing)
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassPathTracing.hlsl"

            ENDHLSL
        }
    }

    FallBack Off
}
