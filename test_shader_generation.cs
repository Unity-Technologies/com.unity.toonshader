using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        try
        {
            // Test reading the common properties file
            string commonPropertiesPath = "com.unity.toonshader/Runtime/Integrated/Shaders/CommonProperties.txt";
            string commonProperties = File.ReadAllText(commonPropertiesPath);
            
            if (string.IsNullOrEmpty(commonProperties))
            {
                Console.WriteLine("ERROR: Common properties file is empty or could not be read");
                return;
            }
            
            Console.WriteLine($"Successfully read common properties file. Length: {commonProperties.Length} characters");
            
            // Test reading the tessellation properties file
            string tessellationPropertiesPath = "com.unity.toonshader/Runtime/Integrated/Shaders/TessellationProperties.txt";
            string tessellationProperties = File.ReadAllText(tessellationPropertiesPath);
            
            if (string.IsNullOrEmpty(tessellationProperties))
            {
                Console.WriteLine("ERROR: Tessellation properties file is empty or could not be read");
                return;
            }
            
            Console.WriteLine($"Successfully read tessellation properties file. Length: {tessellationProperties.Length} characters");
            
            // Test reading the original shader files
            string unityToonPath = "com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader";
            string unityToonContent = File.ReadAllText(unityToonPath);
            
            if (string.IsNullOrEmpty(unityToonContent))
            {
                Console.WriteLine("ERROR: UnityToon.shader file is empty or could not be read");
                return;
            }
            
            Console.WriteLine($"Successfully read UnityToon.shader. Length: {unityToonContent.Length} characters");
            
            // Test the Properties block replacement
            string propertiesPattern = @"Properties\s*\{";
            Match startMatch = Regex.Match(unityToonContent, propertiesPattern);
            
            if (!startMatch.Success)
            {
                Console.WriteLine("ERROR: Could not find Properties block start in shader file");
                return;
            }
            
            Console.WriteLine($"Found Properties block start at position {startMatch.Index}");
            
            // Find the matching closing brace
            int startIndex = startMatch.Index;
            int braceCount = 0;
            int endIndex = startIndex;
            bool foundStart = false;
            
            for (int i = startIndex; i < unityToonContent.Length; i++)
            {
                if (unityToonContent[i] == '{')
                {
                    braceCount++;
                    foundStart = true;
                }
                else if (unityToonContent[i] == '}')
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
                Console.WriteLine("ERROR: Could not find matching closing brace for Properties block");
                return;
            }
            
            Console.WriteLine($"Found Properties block end at position {endIndex}");
            
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
            
            Console.WriteLine($"Generated new Properties block with {propertyCount} properties. Length: {newProperties.Length} characters");
            
            // Test the replacement
            string newContent = unityToonContent.Substring(0, startIndex) + newProperties.ToString() + unityToonContent.Substring(endIndex + 1);
            
            Console.WriteLine($"Generated new shader content. Original length: {unityToonContent.Length}, New length: {newContent.Length}");
            
            // Write test file
            string testPath = "UnityToon_Generated_Test.shader";
            File.WriteAllText(testPath, newContent);
            
            Console.WriteLine($"Test shader written to {testPath}");
            Console.WriteLine("Shader generation test completed successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            Console.WriteLine(e.StackTrace);
        }
    }
}