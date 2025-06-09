using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.TestTools.Graphics
{
    public static class GenerateShaderVariantList
    {
        public static readonly string k_CompiledShaderString = "Uploaded shader variant to the GPU driver";
        private const string k_RegexTail = "pass: (?<passName>[^,]*), stage: (?<stage>[^,]*), keywords (?<keywords>.*)(, time: (?<time>[^,]*) ms)?";
        public static readonly Regex s_CompiledShaderRegex = new Regex(@$"({k_CompiledShaderString}|Compiled shader): (?<shaderName>[^,]*)( \(instance (?<instanceID>[^,]*)\))?, {k_RegexTail}");
        public static readonly Regex s_CompiledComputeShaderRegex = new Regex("Compiled compute shader: (?<computeName>[^,]*), kernel: (?<kernelName>[^,]*), keywords (?<keywords>.*)");
        public static readonly Regex s_ShaderVariantNotFoundRegex = new Regex("Shader (?<shaderName>[^,]*), subshader (?<subShaderIndex>\\d+), pass (?<passIndex>\\d+), stage (?<stage>[^,]*): variant (?<keywords>.*) not found.");
        public static readonly string s_NoKeywordText = "<no keywords>";
        public static readonly string k_UseGraphicsTestStripperEnv = "USE_GFX_TEST_STRIPPER";
        public static readonly string k_UseFastShaderVariantListGeneration = "FAST_SHADER_TRACE_GENERATION";

    #if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            GraphicsSettings.logWhenShaderIsCompiled = true;
            GraphicsTestLogger.Log(LogType.Log, "Register log file processing");
            Application.quitting += ConvertShaderErrorsToLog;
        }
    #endif

        static void ConvertShaderErrorsToLog()
        {
            string logFilePath = Application.consoleLogPath;

            StringBuilder finalList;
            // Read log while the handle is still controlled by Unity
            using (var logFile = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(logFile, Encoding.Default))
                    AppendAllShaderLines(out finalList, reader.ReadToEnd(), true);
            }
            GraphicsTestLogger.Log(LogType.Log, "The following list of Compiled Shaders are directly converted from shader not found errors. You can ignore them as it's only used for the Graphics Test Shader Variants Stripper system.");

            GraphicsTestLogger.Log(LogType.Log, finalList.ToString());
        }

        public static void AppendAllShaderLines(out StringBuilder finalFile, string playerLogContent, bool ignoreValidShadersAndCompute = false)
        {
            var lines = new SortedSet<string>(StringComparer.Ordinal);
            AppendAllShaderLines(out finalFile, playerLogContent, lines, ignoreValidShadersAndCompute);
        }

        public static void AppendAllShaderLines(out StringBuilder finalFile, string playerLogContent, SortedSet<string> existingFileContent, bool ignoreValidShadersAndCompute = false)
        {
            var notFoundMatchSet = new HashSet<string>();

            foreach (var line in playerLogContent.Split('\n'))
            {
                var lineTrimmed = line.Trim();

                if (!ignoreValidShadersAndCompute)
                {
                    var compiledShaderMatch = s_CompiledShaderRegex.Match(lineTrimmed);
                    if (compiledShaderMatch.Success)
                    {
                        var sanitizedLine = compiledShaderMatch.Value;
                        var allStageLine = s_CompiledShaderRegex.Replace(sanitizedLine, $"{k_CompiledShaderString}: $1, pass: $2, stage: all, keywords $4");

                        if (existingFileContent.Contains(allStageLine))
                            continue;

                        // Replace fragment by pixel to avoid duplication in the file
                        if (compiledShaderMatch.Groups["stage"].Value == "fragment")
                            sanitizedLine = s_CompiledShaderRegex.Replace(sanitizedLine, $"{k_CompiledShaderString}: $1, pass: $2, stage: pixel, keywords $4");

                        if (existingFileContent.Contains(sanitizedLine))
                            continue;

                        existingFileContent.Add(sanitizedLine);
                    }

                    var computeShaderMatch = s_CompiledComputeShaderRegex.Match(lineTrimmed);
                    if (computeShaderMatch.Success)
                    {
                        var sanitizedLine = computeShaderMatch.Value;

                        if (existingFileContent.Contains(sanitizedLine))
                            continue;

                        existingFileContent.Add(sanitizedLine);
                    }
                }
#if !UNITY_EDITOR
                // Shader not found error can be spammed quite a bit in the log, causing this process to stall with 10000s of calls
                if (notFoundMatchSet.Contains(lineTrimmed))
                    continue;

                var notFoundMatch = s_ShaderVariantNotFoundRegex.Match(lineTrimmed);
                if (notFoundMatch.Success)
                {
                    notFoundMatchSet.Add(lineTrimmed);
                    // Convert not found shader using the available data in the build
                    var shaderName = notFoundMatch.Groups["shaderName"].Value;
                    var shader = Shader.Find(shaderName);
                    if (shader == null)
                    {
                        GraphicsTestLogger.Log(LogType.Error, $"Could not find shader {shaderName}");
                        continue;
                    }
                    var dummyMaterial = new Material(shader);
                    int.TryParse(notFoundMatch.Groups["passIndex"].Value, out int passIndex);
                    existingFileContent.Add($"{k_CompiledShaderString}: {shaderName}, pass: {dummyMaterial.GetPassName(passIndex)}, stage: {notFoundMatch.Groups["stage"]}, keywords {notFoundMatch.Groups["keywords"]}");
                    Object.DestroyImmediate(dummyMaterial);
                }
#endif
            }

            finalFile = new StringBuilder();
            foreach (var line in existingFileContent)
                finalFile.AppendLine(line);
        }
    }
}
