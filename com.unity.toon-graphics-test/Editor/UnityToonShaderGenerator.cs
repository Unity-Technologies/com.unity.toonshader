using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.Rendering.Toon
{
    /// <summary>
    /// Shader generator for Unity Toon Shader that creates shader files from common properties.
    /// This helps maintain consistency between UnityToon.shader and UnityToonTessellation.shader
    /// by using a single source of truth for shared properties.
    /// </summary>
    public static class UnityToonShaderGenerator
    {
        private static readonly Regex PropertyNameRegex = new Regex(@"(?:\]\s*|^)([A-Za-z_][A-Za-z0-9_]*)\s*\(", RegexOptions.Compiled);
        private const string COMMON_PROPERTIES_PATH = "Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/CommonProperties.shaderblock";
        private const string TESSELATION_PROPERTIES_PATH = "Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/TessellationProperties.shaderblock";
        private const string UNITY_TOON_SHADER_PATH = "Packages/com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader";
        private const string UNITY_TOON_SHADER_TEMPLATE_PATH = "Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/UnityToon.shadertemplate";
        private const string UNITY_TOON_TESSELATION_SHADER_PATH = "Packages/com.unity.toonshader/Runtime/Integrated/Shaders/UnityToonTessellation.shader";
        private const string UNITY_TOON_TESSELATION_SHADER_TEMPLATE_PATH = "Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/UnityToonTessellation.shadertemplate";
        
        [MenuItem("Toon Shader/Generate Shader Files")]
        public static void GenerateShaderFilesMenu()
        {
            GenerateShaderFiles();
        }

        private static void GenerateShaderFiles()
        {
            GenerateShaderFilesInternal();
        }
        
        private static void GenerateShaderFilesInternal()
        {
            try
            {
                // Read common properties
                string commonPropertiesShader = ReadFile(COMMON_PROPERTIES_PATH);
                if (string.IsNullOrEmpty(commonPropertiesShader))
                {
                    Debug.LogError($"Failed to read common properties from {COMMON_PROPERTIES_PATH}");
                    return;
                }
                string commonProperties = ExtractPropertiesBlockContent(commonPropertiesShader);
                if (string.IsNullOrEmpty(commonProperties))
                {
                    Debug.LogError($"Failed to extract common properties block from {COMMON_PROPERTIES_PATH}");
                    return;
                }
                Debug.Log($"Extracted common properties block. Length: {commonProperties.Length} characters");
                
                // Read tessellation properties
                string tessellationPropertiesShader = ReadFile(TESSELATION_PROPERTIES_PATH);
                if (string.IsNullOrEmpty(tessellationPropertiesShader))
                {
                    Debug.LogError($"Failed to read tessellation properties from {TESSELATION_PROPERTIES_PATH}");
                    return;
                }
                string tessellationProperties = ExtractPropertiesBlockContent(tessellationPropertiesShader);
                if (string.IsNullOrEmpty(tessellationProperties))
                {
                    Debug.LogError($"Failed to extract tessellation properties block from {TESSELATION_PROPERTIES_PATH}");
                    return;
                }
                Debug.Log($"Extracted tessellation properties block. Length: {tessellationProperties.Length} characters");

                string timestamp = DateTime.UtcNow.ToString("ddd MMM dd HH:mm:ss 'UTC' yyyy", CultureInfo.InvariantCulture);
                string autoCommentLine = $"//Auto-generated on {timestamp}";

                // Generate UnityToon.shader
                GenerateUnityToonShader(commonProperties, autoCommentLine);

                // Generate UnityToonTessellation.shader
                GenerateUnityToonTessellationShader(commonProperties, tessellationProperties, autoCommentLine);
                
                AssetDatabase.Refresh();
                Debug.Log("Shader files generated successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating shader files: {e.Message}");
            }
        }
        
        private static void GenerateUnityToonShader(string commonProperties, string autoCommentLine)
        {
            GenerateShaderFromTemplate(UNITY_TOON_SHADER_TEMPLATE_PATH, UNITY_TOON_SHADER_PATH, commonProperties, string.Empty, autoCommentLine);
        }

        private static void GenerateUnityToonTessellationShader(string commonProperties, string tessellationProperties, string autoCommentLine)
        {
            GenerateShaderFromTemplate(UNITY_TOON_TESSELATION_SHADER_TEMPLATE_PATH, UNITY_TOON_TESSELATION_SHADER_PATH, commonProperties, tessellationProperties, autoCommentLine);
        }

        private static void GenerateShaderFromTemplate(string templatePath, string outputPath, string commonProperties, string tessellationProperties, string autoCommentLine)
        {
            string templateContent = ReadFile(templatePath);
            if (string.IsNullOrEmpty(templateContent))
            {
                templateContent = ReadFile(outputPath);
                if (string.IsNullOrEmpty(templateContent))
                {
                    Debug.LogError($"Failed to read template or existing shader for {outputPath}");
                    return;
                }
            }

            templateContent = templateContent.TrimStart('\uFEFF');

            PropertySections sections = BuildPropertySections(commonProperties, tessellationProperties, "        ");
            bool hasTessProperties = !string.IsNullOrEmpty(tessellationProperties);
            string commonReplacement = hasTessProperties && !string.IsNullOrEmpty(sections.CommonForTess)
                ? sections.CommonForTess
                : sections.Common;

            templateContent = ReplacePlaceholder(templateContent, "        [COMMON_PROPERTIES]", commonReplacement);
            templateContent = ReplacePlaceholder(templateContent, "[COMMON_PROPERTIES]", commonReplacement);

            templateContent = ReplacePlaceholder(templateContent, "        [TESSELLATION_PROPERTIES]", sections.Tess);
            templateContent = ReplacePlaceholder(templateContent, "[TESSELLATION_PROPERTIES]", sections.Tess);

            Debug.Log($"Generated properties with {sections.Count} entries for {outputPath}");

            templateContent = ApplyAutoGeneratedComment(templateContent, autoCommentLine);
            WriteFile(outputPath, templateContent);
        }
        
        private static string ExtractPropertiesBlockContent(string shaderContent)
        {
            if (string.IsNullOrEmpty(shaderContent))
            {
                return null;
            }

            string propertiesPattern = @"Properties\s*\{";
            Match startMatch = Regex.Match(shaderContent, propertiesPattern);
            if (!startMatch.Success)
            {
                return shaderContent.Trim();
            }

            int openBraceIndex = shaderContent.IndexOf('{', startMatch.Index);
            if (openBraceIndex == -1)
            {
                return shaderContent.Trim();
            }

            int braceCount = 1;
            int closeBraceIndex = -1;

            for (int i = openBraceIndex + 1; i < shaderContent.Length; i++)
            {
                char c = shaderContent[i];
                if (c == '{')
                {
                    braceCount++;
                }
                else if (c == '}')
                {
                    braceCount--;
                    if (braceCount == 0)
                    {
                        closeBraceIndex = i;
                        break;
                    }
                }
            }

            if (closeBraceIndex == -1)
            {
                return shaderContent.Trim();
            }

            string block = shaderContent.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);
            string[] rawLines = block.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            for (int i = 0; i < rawLines.Length; i++)
            {
                rawLines[i] = rawLines[i].Trim();
            }

            int start = 0;
            int end = rawLines.Length - 1;

            while (start <= end && string.IsNullOrEmpty(rawLines[start]))
            {
                start++;
            }

            while (end >= start && string.IsNullOrEmpty(rawLines[end]))
            {
                end--;
            }

            if (start > end)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();
            for (int i = start; i <= end; i++)
            {
                result.Append(rawLines[i]);
                if (i < end)
                {
                    result.Append('\n');
                }
            }

            return result.ToString();
        }

        private static string GetPropertyName(string line)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("//", StringComparison.Ordinal))
            {
                return null;
            }

            MatchCollection matches = PropertyNameRegex.Matches(line);
            if (matches.Count == 0)
            {
                return null;
            }

            string candidate = matches[matches.Count - 1].Groups[1].Value;
            if (string.IsNullOrEmpty(candidate) || candidate.StartsWith("[", StringComparison.Ordinal))
            {
                return null;
            }

            return candidate;
        }

        private static string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path).TrimStart('\uFEFF');
            }
            return null;
        }
        
        private static void WriteFile(string path, string content)
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            content = content.TrimStart('\uFEFF');
            content = content.Replace("\r\n", "\n").Replace("\r", "\n");
            if (!content.EndsWith("\n", StringComparison.Ordinal))
            {
                content += "\n";
            }

            File.WriteAllText(path, content);
        }

        private static string ApplyAutoGeneratedComment(string content, string commentLine)
        {
            const string autoPrefix = "//Auto-generated on ";
            string[] lines = content.Split(new[] { "\n" }, StringSplitOptions.None);

            if (lines.Length > 0 && lines[0].StartsWith(autoPrefix, StringComparison.Ordinal))
            {
                lines[0] = commentLine;
            }
            else
            {
                var updatedLines = new string[lines.Length + 1];
                updatedLines[0] = commentLine;
                Array.Copy(lines, 0, updatedLines, 1, lines.Length);
                lines = updatedLines;
            }

            string result = string.Join("\n", lines);
            if (!result.EndsWith("\n", StringComparison.Ordinal))
            {
                result += "\n";
            }

            return result;
        }

        private static string ReplacePlaceholder(string content, string placeholder, string replacement)
        {
            if (content.Contains(placeholder))
            {
                return content.Replace(placeholder, replacement ?? string.Empty);
            }

            return content;
        }

        private static PropertySections BuildPropertySections(string commonProperties, string tessellationProperties, string indent)
        {
            var entries = new List<PropertyEntry>();
            var nameToIndex = new Dictionary<string, Dictionary<string, int>>(StringComparer.Ordinal);

            Dictionary<string, int> GetSourceMap(string source)
            {
                if (!nameToIndex.TryGetValue(source, out var map))
                {
                    map = new Dictionary<string, int>(StringComparer.Ordinal);
                    nameToIndex[source] = map;
                }

                return map;
            }

            void AddLines(string rawText, string source)
            {
                if (string.IsNullOrEmpty(rawText))
                {
                    return;
                }

                var lines = rawText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                foreach (var rawLine in lines)
                {
                    var trimmed = rawLine.Trim();
                    var entry = new PropertyEntry
                    {
                        Line = trimmed,
                        Source = source,
                        Name = GetPropertyName(trimmed),
                        IsHidden = trimmed.IndexOf("[HideInInspector]", StringComparison.Ordinal) >= 0
                    };

                    if (trimmed.Length == 0)
                    {
                        entries.Add(entry);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(entry.Name))
                    {
                        var sourceMap = GetSourceMap(source);
                        if (sourceMap.TryGetValue(entry.Name, out int existingIndex))
                        {
                            var existingEntry = entries[existingIndex];
                            bool ShouldReplace(PropertyEntry existing, PropertyEntry candidate)
                            {
                                if (existing.IsHidden && !candidate.IsHidden)
                                {
                                    return true;
                                }

                                if (!existing.IsHidden && candidate.IsHidden)
                                {
                                    return false;
                                }

                                return true;
                            }

                            if (ShouldReplace(existingEntry, entry))
                            {
                                entries[existingIndex] = entry;
                            }
                        }
                        else
                        {
                            entries.Add(entry);
                            sourceMap[entry.Name] = entries.Count - 1;
                        }
                    }
                    else
                    {
                        entries.Add(entry);
                    }
                }
            }

            AddLines(commonProperties, "common");
            AddLines(tessellationProperties, "tess");

            int propertyCount = 0;
            var seenPairs = new HashSet<string>(StringComparer.Ordinal);
            var commonLines = new List<string>();
            var commonLinesForTess = new List<string>();
            var tessLines = new List<string>();

            var tessOverrides = new HashSet<string>(StringComparer.Ordinal);
            if (nameToIndex.TryGetValue("tess", out var tessMap))
            {
                foreach (var key in tessMap.Keys)
                {
                    tessOverrides.Add(key);
                }
            }

            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.Name))
                {
                    string key = entry.Source + "\u0001" + entry.Name;
                    if (seenPairs.Add(key))
                    {
                        propertyCount++;
                    }
                }

                entry.SkipForTess = entry.Source == "common" && !string.IsNullOrEmpty(entry.Name) && tessOverrides.Contains(entry.Name);

                var target = entry.Source == "tess" ? tessLines : commonLines;
                if (string.IsNullOrEmpty(entry.Line))
                {
                    target.Add(string.Empty);
                    if (entry.Source == "common" && !entry.SkipForTess)
                    {
                        commonLinesForTess.Add(string.Empty);
                    }
                }
                else
                {
                    var formattedLine = indent + entry.Line;
                    target.Add(formattedLine);
                    if (entry.Source == "common" && !entry.SkipForTess)
                    {
                        commonLinesForTess.Add(formattedLine);
                    }
                }
            }

            commonLines = CleanupLines(commonLines);
            commonLinesForTess = CleanupLines(commonLinesForTess);
            tessLines = CleanupLines(tessLines);

            return new PropertySections
            {
                Common = string.Join("\n", commonLines),
                CommonForTess = string.Join("\n", commonLinesForTess),
                Tess = string.Join("\n", tessLines),
                Count = propertyCount
            };
        }

        private static List<string> CleanupLines(List<string> lines)
        {
            var result = new List<string>(lines);

            while (result.Count > 0 && result[0].Length == 0)
            {
                result.RemoveAt(0);
            }

            while (result.Count > 0 && result[result.Count - 1].Length == 0)
            {
                result.RemoveAt(result.Count - 1);
            }

            var cleaned = new List<string>();
            foreach (var line in result)
            {
                if (line.Length == 0 && cleaned.Count > 0 && cleaned[cleaned.Count - 1].Length == 0)
                {
                    continue;
                }
                cleaned.Add(line);
            }

            return cleaned;
        }

        private struct PropertySections
        {
            public string Common;
            public string CommonForTess;
            public string Tess;
            public int Count;
        }

        private class PropertyEntry
        {
            public string Line;
            public string Source;
            public string Name;
            public bool IsHidden;
            public bool SkipForTess;
        }
    }
}