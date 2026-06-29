// using System.IO;
// using NUnit.Framework;
// using UnityEditor;
// using UnityEditor.Rendering.Toon;
// using UnityEngine;
// using Unity.Rendering.Toon;
//
// namespace Unity.ToonShader.EditorTests
// {
//     /// <summary>
//     /// Tests to ensure that the converter can find the integrated shaders.
//     /// These tests will fail if:
//     /// - The shaders are moved to a different location
//     /// - The shader GUIDs change
//     /// - The shader names change
//     /// This helps catch issues like #773 early.
//     /// </summary>
//     internal class ConverterShaderTest
//     {
//         private static readonly string EXPECTED_SHADER_PATH = Path.Combine("Packages", ToonConstants.PACKAGE_NAME, "Runtime/Integrated/Shaders");
//
//         [Test]
//         public void IntegratedShader_ExistsAtExpectedPath()
//         {
//             // Arrange
//             string expectedPath = Path.Combine(EXPECTED_SHADER_PATH, "UnityToon.shader");
//
//             // Act
//             string actualPath = AssetDatabase.GUIDToAssetPath(RenderPipelineConverterContainer.kIntegratedUTS3GUID);
//
//             // Assert
//             Assert.IsNotEmpty(actualPath, $"Shader with GUID {RenderPipelineConverterContainer.kIntegratedUTS3GUID} not found in project");
//             Assert.AreEqual(expectedPath, actualPath,
//                 $"Integrated shader has moved! Expected at: {expectedPath}, but found at: {actualPath}. " +
//                 $"Update the shader path or GUID in RenderPipelineConverterContainer.cs");
//         }
//
//         [Test]
//         public void IntegratedTessellationShader_ExistsAtExpectedPath()
//         {
//             // Arrange
//             string expectedPath = Path.Combine(EXPECTED_SHADER_PATH, "UnityToonTessellation.shader");
//
//             // Act
//             string actualPath = AssetDatabase.GUIDToAssetPath(RenderPipelineConverterContainer.kIntegratedTessllationUTS3GUID);
//
//             // Assert
//             Assert.IsNotEmpty(actualPath, $"Shader with GUID {RenderPipelineConverterContainer.kIntegratedTessllationUTS3GUID} not found in project");
//             Assert.AreEqual(expectedPath, actualPath,
//                 $"Integrated tessellation shader has moved! Expected at: {expectedPath}, but found at: {actualPath}. " +
//                 $"Update the shader path or GUID in RenderPipelineConverterContainer.cs");
//         }
//
//         [Test]
//         public void IntegratedShader_CanBeLoaded()
//         {
//             // Arrange & Act
//             string path = AssetDatabase.GUIDToAssetPath(RenderPipelineConverterContainer.kIntegratedUTS3GUID);
//             Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
//
//             // Assert
//             Assert.IsNotNull(shader, $"Could not load shader at path: {path}");
//             Assert.IsTrue(shader.isSupported, $"Shader {shader.name} is not supported on this platform");
//             Assert.IsFalse(ShaderUtil.ShaderHasError(shader), $"Shader {shader.name} has compile errors");
//         }
//
//         [Test]
//         public void IntegratedTessellationShader_CanBeLoaded()
//         {
//             // Arrange & Act
//             string path = AssetDatabase.GUIDToAssetPath(RenderPipelineConverterContainer.kIntegratedTessllationUTS3GUID);
//             Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
//
//             // Assert
//             Assert.IsNotNull(shader, $"Could not load shader at path: {path}");
//             Assert.IsTrue(shader.isSupported, $"Shader {shader.name} is not supported on this platform");
//             Assert.IsFalse(ShaderUtil.ShaderHasError(shader), $"Shader {shader.name} has compile errors");
//         }
//
//         [Test]
//         public void IntegratedShader_NameMatchesExpected()
//         {
//             // Arrange
//             const string expectedPattern = "Toon/Toon";
//
//             // Act
//             string actualName = RenderPipelineConverterContainer.GetIntegratedUTS3ShaderName();
//
//             // Assert
//             Assert.AreEqual(expectedPattern, actualName,
//                 $"Integrated shader name has changed! Expected: '{expectedPattern}', but got: '{actualName}'. " +
//                 $"This will break material conversion. Update the shader name in the shader file or update the expected pattern in this test.");
//         }
//
//         [Test]
//         public void IntegratedTessellationShader_NameMatchesExpected()
//         {
//             // Arrange
//             const string expectedPattern = "Toon/Toon (Tessellation)";
//
//             // Act
//             string actualName = RenderPipelineConverterContainer.GetIntegratedUTS3TessellationShaderName();
//
//             // Assert
//             Assert.AreEqual(expectedPattern, actualName,
//                 $"Integrated tessellation shader name has changed! Expected: '{expectedPattern}', but got: '{actualName}'. " +
//                 $"This will break material conversion. Update the shader name in the shader file or update the expected pattern in this test.");
//         }
//
//         [Test]
//         public void IntegratedShader_CanBeFoundByName()
//         {
//             // Arrange
//             string shaderName = RenderPipelineConverterContainer.GetIntegratedUTS3ShaderName();
//
//             // Act
//             Shader shader = Shader.Find(shaderName);
//
//             // Assert
//             Assert.IsNotNull(shader,
//                 $"Shader.Find() could not find shader with name: '{shaderName}'. " +
//                 $"This is exactly what causes issue #773. Check that the shader name in the shader file matches.");
//         }
//
//         [Test]
//         public void IntegratedTessellationShader_CanBeFoundByName()
//         {
//             // Arrange
//             string shaderName = RenderPipelineConverterContainer.GetIntegratedUTS3TessellationShaderName();
//
//             // Act
//             Shader shader = Shader.Find(shaderName);
//
//             // Assert
//             Assert.IsNotNull(shader,
//                 $"Shader.Find() could not find shader with name: '{shaderName}'. " +
//                 $"This is exactly what causes issue #773. Check that the shader name in the shader file matches.");
//         }
//     }
// }
