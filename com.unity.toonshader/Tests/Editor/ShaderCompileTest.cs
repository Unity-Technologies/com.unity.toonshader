using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEditor.Rendering.Toon;

namespace Unity.ToonShader.EditorTests {
internal class ShaderCompileTest
{
    [Test]
    public void CompileToonShaders() {
        string[] shaderPaths = {
            ToonEditorConstants.TOON_SHADER_PATH,
            ToonEditorConstants.TOON_TESS_SHADER_PATH,
        };
        int      numShaders = shaderPaths.Length;
        Assert.Greater(numShaders,0);

            
        for (int i=0;i<numShaders;++i) {
            string curAssetPath = shaderPaths[i];
            
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(curAssetPath);
            Assert.IsNotNull(shader);
            
            AssetDatabase.ImportAsset(curAssetPath); //Recompile the shader to make sure there are no compile errors
            
            Assert.True(shader.isSupported);
            bool shaderHasError = ShaderUtil.ShaderHasError(shader);
            Assert.False(shaderHasError, "[UTS] Shader Compile Error: " + shader.name);
        }
    }

//----------------------------------------------------------------------------------------------------------------------


}

} //end namespace
