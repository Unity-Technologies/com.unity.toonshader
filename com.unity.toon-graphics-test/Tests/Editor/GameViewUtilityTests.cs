using NUnit.Framework;

namespace Unity.ToonShader.GraphicsTest.EditorTests {
internal class GameViewUtilityTests {
    
    [Test]
    public void CheckInitialization() {
        Assert.IsTrue(GameViewUtility.IsInitialized());
        
        
    }
}
} //end namespace
