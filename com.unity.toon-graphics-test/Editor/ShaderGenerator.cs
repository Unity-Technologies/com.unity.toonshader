using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.Rendering.Toon
{
    /// <summary>
    /// Shader generator for Unity Toon Shader that creates shader files from common properties.
    /// This helps maintain consistency between UnityToon.shader and UnityToonTessellation.shader
    /// by using a single source of truth for shared properties.
    /// </summary>
    public class ShaderGenerator : EditorWindow
    {
        private static readonly Regex PropertyNameRegex = new Regex(@"([A-Za-z_][A-Za-z0-9_]*)\s*\(", RegexOptions.Compiled);
        private const string COMMON_PROPERTIES_PATH = "Assets/com.unity.toonshader/Runtime/Integrated/Shaders/CommonPropertiesPart.shader";
        private const string TESSELATION_PROPERTIES_PATH = "Assets/com.unity.toonshader/Runtime/Integrated/Shaders/TessellationPropertiesPart.shader";
        private const string UNITY_TOON_SHADER_PATH = "Assets/com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader";
        private const string UNITY_TOON_TESSELATION_SHADER_PATH = "Assets/com.unity.toonshader/Runtime/Integrated/Shaders/UnityToonTessellation.shader";
        
        [MenuItem("Unity Toon Shader/Generate Shader Files")]
        public static void ShowWindow()
        {
            GetWindow<ShaderGenerator>("Shader Generator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Unity Toon Shader Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("This tool generates UnityToon.shader and UnityToonTessellation.shader from common property files.", EditorStyles.helpBox);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Generate Shader Files", GUILayout.Height(30)))
            {
                GenerateShaderFiles();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Open Common Properties File"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Shader>(COMMON_PROPERTIES_PATH);
                if (asset != null)
                {
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                }
            }
            
            if (GUILayout.Button("Open Tessellation Properties File"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Shader>(TESSELATION_PROPERTIES_PATH);
                if (asset != null)
                {
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                }
            }
        }
        
        private void GenerateShaderFiles()
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
        
        private void GenerateUnityToonShader(string commonProperties, string autoCommentLine)
        {
            // Read the original shader file to preserve the rest of the content
            string originalContent = ReadFile(UNITY_TOON_SHADER_PATH);
            if (string.IsNullOrEmpty(originalContent))
            {
                Debug.LogError($"Failed to read original shader from {UNITY_TOON_SHADER_PATH}");
                return;
            }
            // Replace the Properties block
            string newContent = ReplacePropertiesBlock(originalContent, commonProperties, string.Empty, autoCommentLine);
            WriteFile(UNITY_TOON_SHADER_PATH, newContent);
        }
        
        private void GenerateUnityToonTessellationShader(string commonProperties, string tessellationProperties, string autoCommentLine)
        {
            // Read the original shader file to preserve the rest of the content
            string originalContent = ReadFile(UNITY_TOON_TESSELATION_SHADER_PATH);
            if (string.IsNullOrEmpty(originalContent))
            {
                Debug.LogError($"Failed to read original tessellation shader from {UNITY_TOON_TESSELATION_SHADER_PATH}");
                return;
            }
            // Replace the Properties block
            string newContent = ReplacePropertiesBlock(originalContent, commonProperties, tessellationProperties, autoCommentLine);
            WriteFile(UNITY_TOON_TESSELATION_SHADER_PATH, newContent);
        }
        
        private string ReplacePropertiesBlock(string originalContent, string commonProperties, string tessellationProperties, string autoCommentLine)
        {
            // Find the Properties block using a more robust regex that handles nested braces
            string propertiesPattern = @"Properties\s*\{";
            Match startMatch = Regex.Match(originalContent, propertiesPattern);
            
            if (!startMatch.Success)
            {
                Debug.LogError("Could not find Properties block start in shader file");
                return originalContent;
            }
            
            // Find the matching closing brace
            int startIndex = startMatch.Index;
            int braceCount = 0;
            int endIndex = startIndex;
            bool foundStart = false;
            
            for (int i = startIndex; i < originalContent.Length; i++)
            {
                if (originalContent[i] == '{')
                {
                    braceCount++;
                    foundStart = true;
                }
                else if (originalContent[i] == '}')
                {
                    braceCount--;
                    if (foundStart && braceCount == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }
            }
            
            if (braceCount != 0)
            {
                Debug.LogError("Could not find matching closing brace for Properties block");
                return originalContent;
            }
            
            int lineStartIndex = originalContent.LastIndexOf('\n', startIndex - 1);
            int baseIndentStart = lineStartIndex == -1 ? 0 : lineStartIndex + 1;
            string baseIndent = "    ";
            string blockIndent = baseIndent + "    ";

            // Build new Properties block
            List<string> newProperties = new List<string>();
            Dictionary<string, int> propertyLineIndices = new Dictionary<string, int>(StringComparer.Ordinal);
            int propertyCount = 0;

            newProperties.Add($"{baseIndent}Properties {{");

            // Add common properties
            string[] commonLines = commonProperties.Split('\n');
            foreach (string line in commonLines)
            {
                string trimmed = line.Trim();
                if (trimmed.Length == 0)
                {
                    newProperties.Add(string.Empty);
                    continue;
                }

                string propertyLine = $"{blockIndent}{trimmed}";
                string propertyName = GetPropertyName(trimmed);

                if (propertyName != null)
                {
                    if (propertyLineIndices.TryGetValue(propertyName, out int existingIndex))
                    {
                        newProperties[existingIndex] = propertyLine;
                    }
                    else
                    {
                        propertyLineIndices[propertyName] = newProperties.Count;
                        propertyCount++;
                        newProperties.Add(propertyLine);
                    }
                }
                else
                {
                    newProperties.Add(propertyLine);
                }
            }

            // Add tessellation properties if provided
            if (!string.IsNullOrEmpty(tessellationProperties))
            {
                newProperties.Add(string.Empty);
                newProperties.Add($"{blockIndent}// Tessellation-specific properties");
                string[] tessellationLines = tessellationProperties.Split('\n');
                foreach (string line in tessellationLines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.Length == 0)
                    {
                        newProperties.Add(string.Empty);
                        continue;
                    }

                    string propertyLine = $"{blockIndent}{trimmed}";
                    string propertyName = GetPropertyName(trimmed);

                    if (propertyName != null)
                    {
                        if (propertyLineIndices.TryGetValue(propertyName, out int existingIndex))
                        {
                            newProperties[existingIndex] = propertyLine;
                        }
                        else
                        {
                            propertyLineIndices[propertyName] = newProperties.Count;
                            propertyCount++;
                            newProperties.Add(propertyLine);
                        }
                    }
                    else
                    {
                        newProperties.Add(propertyLine);
                    }
                }
            }

            newProperties.Add($"{baseIndent}}");

            Debug.Log($"Generated Properties block with {propertyCount} properties");

            string newPropertiesText = string.Join("\n", newProperties);

            string updatedContent = originalContent.Substring(0, baseIndentStart) + newPropertiesText + originalContent.Substring(endIndex + 1);
            return ApplyAutoGeneratedComment(updatedContent, autoCommentLine);
        }
        
        private string ExtractPropertiesBlockContent(string shaderContent)
        {
            if (string.IsNullOrEmpty(shaderContent))
            {
                return null;
            }

            string propertiesPattern = @"Properties\s*\{";
            Match startMatch = Regex.Match(shaderContent, propertiesPattern);
            if (!startMatch.Success)
            {
                return null;
            }

            int openBraceIndex = shaderContent.IndexOf('{', startMatch.Index);
            if (openBraceIndex == -1)
            {
                return null;
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
                return null;
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

        private string GetPropertyName(string line)
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

        private string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return null;
        }
        
        private void WriteFile(string path, string content)
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(path, content);
        }

        private string ApplyAutoGeneratedComment(string content, string commentLine)
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
    }
}