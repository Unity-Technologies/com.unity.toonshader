namespace Unity.ToonShader.GraphicsTest
{
public class UTSGraphicsTestConstants {
#if UTS_TEST_USE_HDRP
    internal const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/HDRP";
#elif UTS_TEST_USE_URP
    internal const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/URP";
#else
    internal const string ReferenceImagePath = "Packages/com.unity.toon-reference-images/Built-In";
#endif

}

} //end namespace
