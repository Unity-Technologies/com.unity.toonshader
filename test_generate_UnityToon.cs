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
            // Test reading the common properties shader
            string commonPropertiesPath = "com.unity.toonshader/Runtime/Shaders/Common/Parts/CommonProperties.shaderblock";
            string commonPropertiesShader = File.ReadAllText(commonPropertiesPath);

            if (string.IsNullOrEmpty(commonPropertiesShader))
            {
                Console.WriteLine("ERROR: Common properties shader file is empty or could not be read");
                return;
            }
            string commonProperties = ExtractProperties(commonPropertiesShader);
            if (string.IsNullOrEmpty(commonProperties))
            {
                Console.WriteLine("ERROR: Failed to extract properties from CommonProperties.shaderblock");
                return;
            }

            Console.WriteLine($"Successfully extracted common properties. Length: {commonProperties.Length} characters");

            // Test reading the tessellation properties shader
            string tessellationPropertiesPath = "com.unity.toonshader/Runtime/Shaders/Common/Parts/TessellationProperties.shaderblock";
            string tessellationPropertiesShader = File.ReadAllText(tessellationPropertiesPath);

            if (string.IsNullOrEmpty(tessellationPropertiesShader))
            {
                Console.WriteLine("ERROR: Tessellation properties shader file is empty or could not be read");
                return;
            }
            string tessellationProperties = ExtractProperties(tessellationPropertiesShader);
            if (string.IsNullOrEmpty(tessellationProperties))
            {
                Console.WriteLine("ERROR: Failed to extract properties from TessellationProperties.shaderblock");
                return;
            }

            Console.WriteLine($"Successfully extracted tessellation properties. Length: {tessellationProperties.Length} characters");

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

            // Build new Properties block
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
            newContent = ApplyAutoGeneratedComment(newContent, "//Auto-generated (test output)");

            Console.WriteLine($"Generated new shader content. Original length: {unityToonContent.Length}, New length: {newContent.Length}");

            // Write test file
            string testPath = "UnityToon_Generated_Test.shader";
            File.WriteAllText(testPath, newContent);

            Console.WriteLine($"Test shader written to {testPath}");
            Console.WriteLine("Shader generation test completed successfully!");

            try
            {
                File.Delete(testPath);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            Console.WriteLine(e.StackTrace);
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
            string[] updatedLines = new string[lines.Length + 1];
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
