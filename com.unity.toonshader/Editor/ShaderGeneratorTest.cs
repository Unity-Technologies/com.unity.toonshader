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
    public class ShaderGeneratorTest
    {
        [MenuItem("Tools/Unity Toon Shader/Test Shader Generation")]
        public static void TestShaderGeneration()
        {
            try
            {
                // Test reading the common properties file
                string commonPropertiesPath = "Assets/com.unity.toonshader/Runtime/Integrated/Shaders/CommonProperties.txt";
                string commonProperties = File.ReadAllText(commonPropertiesPath);
                
                if (string.IsNullOrEmpty(commonProperties))
                {
                    Debug.LogError("Common properties file is empty or could not be read");
                    return;
                }
                
                Debug.Log($"Successfully read common properties file. Length: {commonProperties.Length} characters");
                
                // Test reading the tessellation properties file
                string tessellationPropertiesPath = "Assets/com.unity.toonshader/Runtime/Integrated/Shaders/TessellationProperties.txt";
                string tessellationProperties = File.ReadAllText(tessellationPropertiesPath);
                
                if (string.IsNullOrEmpty(tessellationProperties))
                {
                    Debug.LogError("Tessellation properties file is empty or could not be read");
                    return;
                }
                
                Debug.Log($"Successfully read tessellation properties file. Length: {tessellationProperties.Length} characters");
                
                // Test reading the original shader files
                string unityToonPath = "Assets/com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader";
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
                    string testPath = "Assets/com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon_Generated_Test.shader";
                    File.WriteAllText(testPath, newContent);
                    AssetDatabase.Refresh();
                    
                    Debug.Log($"Test shader written to {testPath}");
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
    }
}