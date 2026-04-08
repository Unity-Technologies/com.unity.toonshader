using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Unity.Rendering.Toon;

namespace Unity.ToonShader.EditorTests {
internal class ShaderCompileTest
{
    [Test]
    public void CompileToonShaders() {
        string[] guids      = AssetDatabase.FindAssets("t:Shader", new[] { SHADERS_PATH});
        int      numShaders = guids.Length;
        Assert.Greater(numShaders,0);

            
        for (int i=0;i<numShaders;++i) {
            string curAssetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            string directory = Path.GetDirectoryName(curAssetPath).Replace('\\','/');
            
            // Exclude shaders in subfolders
            if (directory != SHADERS_PATH) {
                continue;
            }
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(curAssetPath);
            AssetDatabase.ImportAsset(curAssetPath); //Recompile the shader to make sure there are no compile errors
            
            Assert.True(shader.isSupported);
            bool shaderHasError = ShaderUtil.ShaderHasError(shader);
            Assert.False(shaderHasError, "[UTS] Shader Compile Error: " + shader.name);
        }
    }

//----------------------------------------------------------------------------------------------------------------------

    private static readonly string SHADERS_PATH = 
        Path.Combine("Packages", ToonConstants.PACKAGE_NAME,"Runtime/Shaders").Replace('\\','/');

}

} //end namespace
