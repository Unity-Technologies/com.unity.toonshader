using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.Rendering.Toon
{
    /// <summary>
    /// Test script to verify the shader generator works correctly
    /// </summary>
    public class UnityToonShaderGeneratorTest
    {
        [MenuItem("Toon Shader/Test Shader Generation")]
        public static void TestShaderGeneration()
        {
            try
            {
                // Test reading the common properties file
                string commonPropertiesPath = "Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/CommonProperties.shaderblock";
                string commonPropertiesShader = File.ReadAllText(commonPropertiesPath);
                
                if (string.IsNullOrEmpty(commonPropertiesShader))
                {
                    Debug.LogError("Common properties file is empty or could not be read");
                    return;
                }
                string commonProperties = ExtractProperties(commonPropertiesShader);
                if (string.IsNullOrEmpty(commonProperties))
                {
                    Debug.LogError("Failed to extract properties from CommonProperties.shaderblock");
                    return;
                }
                
                Debug.Log($"Successfully extracted common properties. Length: {commonProperties.Length} characters");
                
                // Test reading the tessellation properties file
                string tessellationPropertiesPath = "Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/TessellationProperties.shaderblock";
                string tessellationPropertiesShader = File.ReadAllText(tessellationPropertiesPath);
                
                if (string.IsNullOrEmpty(tessellationPropertiesShader))
                {
                    Debug.LogError("Tessellation properties file is empty or could not be read");
                    return;
                }
                string tessellationProperties = ExtractProperties(tessellationPropertiesShader);
                if (string.IsNullOrEmpty(tessellationProperties))
                {
                    Debug.LogError("Failed to extract properties from TessellationProperties.shaderblock");
                    return;
                }
                
                Debug.Log($"Successfully extracted tessellation properties. Length: {tessellationProperties.Length} characters");
                
                // Test reading the original shader files
                string unityToonPath = "Packages/com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader";
                string unityToonContent = File.ReadAllText(unityToonPath);
                
                if (string.IsNullOrEmpty(unityToonContent))
                {
                    Debug.LogError("UnityToon.shader file is empty or could not be read");
                    return;
                }
                
                Debug.Log($"Successfully read UnityToon.shader. Length: {unityToonContent.Length} characters");
                
                // Test the Properties block replacement
                string propertiesPattern = @"Properties\s*\{[^}]*\}";
                Match match = Regex.Match(unityToonContent, propertiesPattern, RegexOptions.Singleline);
                
                if (match.Success)
                {
                    Debug.Log($"Found Properties block at position {match.Index}, length {match.Length}");
                    
                    // Test building new Properties block
                    StringBuilder newProperties = new StringBuilder();
                    newProperties.AppendLine("    Properties {");
                    
                    // Add common properties
                    string[] commonLines = commonProperties.Split('\n');
                    int propertyCount = 0;
                    foreach (string line in commonLines)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("//"))
                        {
                            newProperties.AppendLine($"        {line.Trim()}");
                            propertyCount++;
                        }
                    }
                    
                    newProperties.AppendLine("    }");
                    
                    Debug.Log($"Generated new Properties block with {propertyCount} properties. Length: {newProperties.Length} characters");
                    
                    // Test the replacement
                    string newContent = unityToonContent.Substring(0, match.Index) + newProperties.ToString() + unityToonContent.Substring(match.Index + match.Length);
                    
                    Debug.Log($"Generated new shader content. Original length: {unityToonContent.Length}, New length: {newContent.Length}");
                    
                    // Write test file
                    string testPath = "Packages/com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon_Generated_Test.shader";
                    File.WriteAllText(testPath, newContent);
                    AssetDatabase.Refresh();
                    
                    Debug.Log($"Test shader written to {testPath}");

                    if (File.Exists(testPath))
                    {
                        File.Delete(testPath);
                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    Debug.LogError("Could not find Properties block in UnityToon.shader");
                }
                
                Debug.Log("Shader generation test completed successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during shader generation test: {e.Message}\n{e.StackTrace}");
            }
        }
        
        private static string ExtractProperties(string shaderContent)
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
    }
}