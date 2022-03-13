using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace UnityEditor.Rendering.Toon
{
    internal class UnitychanToonShader2UnityToonShader : EditorWindow
    {
        public enum _UTS_Technique
        {
            DoubleShadeWithFeather, ShadingGradeMap
        }
        public enum _UTS_ClippingMode
        {
            Off, On, TransClippingMode
        }

        public enum _UTS_TransClippingMode
        {
            Off, On,
        }
        public enum _UTS_Transparent
        {
            Off, On,
        }
        public enum _UTS_StencilMode
        {
            Off, StencilOut, StencilMask
        }

        public enum _StencilOperation
        {
            //https://docs.unity3d.com/Manual/SL-Stencil.html
            Keep, //    Keep the current contents of the buffer.
            Zero, //    Write 0 into the buffer.
            Replace, // Write the reference value into the buffer.
            IncrSat, // Increment the current value in the buffer. If the value is 255 already, it stays at 255.
            DecrSat, // Decrement the current value in the buffer. If the value is 0 already, it stays at 0.
            Invert, //  Negate all the bits.
            IncrWrap, //    Increment the current value in the buffer. If the value is 255 already, it becomes 0.
            DecrWrap, //    Decrement the current value in the buffer. If the value is 0 already, it becomes 255.
        }

        public enum _StencilCompFunction
        {

            Disabled,//    Depth or stencil test is disabled.
            Never,   //   Never pass depth or stencil test.
            Less,   //   Pass depth or stencil test when new value is less than old one.
            Equal,  //  Pass depth or stencil test when values are equal.
            LessEqual, // Pass depth or stencil test when new value is less or equal than old one.
            Greater, // Pass depth or stencil test when new value is greater than old one.
            NotEqual, //    Pass depth or stencil test when values are different.
            GreaterEqual, // Pass depth or stencil test when new value is greater or equal than old one.
            Always,//  Always pass depth or stencil test.
        }

        struct UTS2GUID
        {
            public UTS2GUID(string guid, string shaderName)
            {
                m_ShaderName = shaderName;
                m_Guid = shaderName;
            }
            string m_ShaderName;
            string m_Guid;
        }
        UTS2GUID[] stdShaders =
        {
            new UTS2GUID(  "96d4d9f975e6c8849bd1a5c06acfae84", "ToonColor_DoubleShadeWithFeather"),
            new UTS2GUID(  "ccd13b7f8710b264ea8bd3bc4f51f9e4", "ToonColor_DoubleShadeWithFeather_Clipping"),
            new UTS2GUID(  "9c3978743d5db18448a8b945c723a6eb", "ToonColor_DoubleShadeWithFeather_Clipping_StencilMask"),
            new UTS2GUID(  "d7da29588857e774bb0650f1fae494c6", "ToonColor_DoubleShadeWithFeather_Clipping_StencilOut"),
            new UTS2GUID(  "315897103223dab42a0746aa65ec251a", "ToonColor_DoubleShadeWithFeather_StencilMask"),
            new UTS2GUID(  "2e5cc2da6af713844956264245e092e4", "ToonColor_DoubleShadeWithFeather_StencilOut"),
            new UTS2GUID(  "369d674ae1ba36249bb00e2f73b0cd10", "ToonColor_DoubleShadeWithFeather_TransClipping"),
            new UTS2GUID(  "8600b2bec3ae31145afa80084df20c61", "ToonColor_DoubleShadeWithFeather_TransClipping_StencilMask"),
            new UTS2GUID(  "43d0eeb4c46f52841b0941e99ac9b16b", "ToonColor_DoubleShadeWithFeather_TransClipping_StencilOut"),
            new UTS2GUID(  "97b7edb5fc0f5744c9b264c2224a0b1e", "ToonColor_DoubleShadeWithFeather_Transparent"),
            new UTS2GUID(  "3b20fc0febd34f94baf0304bf47841d8", "ToonColor_DoubleShadeWithFeather_Transparent_StencilOut"),
            new UTS2GUID(  "af8454e09b3a41448a4140e792059446", "ToonColor_ShadingGradeMap"),
            new UTS2GUID(  "295fec4a7029edd4eb9522bef07f41ce", "ToonColor_ShadingGradeMap_AngelRing"),
            new UTS2GUID(  "e32270aa38f4b664b90f04cc475fdb81", "ToonColor_ShadingGradeMap_AngelRing_StencilOut"),
            new UTS2GUID(  "29a860a3f3c4cec43ab821338e28eac8", "ToonColor_ShadingGradeMap_AngelRing_TransClipping"),
            new UTS2GUID(  "d5d9c1f4718235248ad37448b0c74c68", "ToonColor_ShadingGradeMap_AngelRing_TransClipping_StencilOut"),
            new UTS2GUID(  "6439813c08a1f8947bb0ca6599499dd7", "ToonColor_ShadingGradeMap_StencilMask"),
            new UTS2GUID(  "b39692f1382224b4cbe21c12ae51c639", "ToonColor_ShadingGradeMap_StencilOut"),
            new UTS2GUID(  "cd7e85b59edbb7740841003baeb510b5", "ToonColor_ShadingGradeMap_TransClipping"),
            new UTS2GUID(  "6b4b6d07944415f44b1fc2f0fc24535f", "ToonColor_ShadingGradeMap_TransClipping_StencilMask"),
            new UTS2GUID(  "31c75b34739dfc64fb57bf49005e942d", "ToonColor_ShadingGradeMap_TransClipping_StencilOut"),
            new UTS2GUID(  "7737ca8c4e3939f4086a6e08f93c2ebd", "ToonColor_ShadingGradeMap_Transparent"),
            new UTS2GUID(  "be27d4be45de7dd4ab2e69c992876edb", "ToonColor_ShadingGradeMap_Transparent_StencilOut"),
            new UTS2GUID(  "9baf30ce95c751649b14d96da3a4b4d5", "Toon_DoubleShadeWithFeather"),
            new UTS2GUID(  "345def18d0906d544b7d12b050937392", "Toon_DoubleShadeWithFeather_Clipping"),
            new UTS2GUID(  "7a735f9b121d96349b6da0a077299424", "Toon_DoubleShadeWithFeather_Clipping_StencilMask"),
            new UTS2GUID(  "ed7fba947f3bccb4cbc78f55d7a56a70", "Toon_DoubleShadeWithFeather_Clipping_StencilOut"),
//            new UTS2GUID(  "1d10c7840eb6ba74c889a27f14ba6081", "Toon_DoubleShadeWithFeather_Mobile"),
//            new UTS2GUID(  "88791c14394118d42a5e176b433af322", "Toon_DoubleShadeWithFeather_Mobile_Clipping"),
//            new UTS2GUID(  "41f4ee183cb66ad40bc74a9f8f944974", "Toon_DoubleShadeWithFeather_Mobile_Clipping_StencilMask"),
//            new UTS2GUID(  "dec01cbdbc5b8da4ca8671815cda1557", "Toon_DoubleShadeWithFeather_Mobile_StencilMask"),
//            new UTS2GUID(  "55e8b9eeaaff205469365133fe7bc744", "Toon_DoubleShadeWithFeather_Mobile_StencilOut"),
//            new UTS2GUID(  "d4c592285a93c3844aafdaafffc07ec7", "Toon_DoubleShadeWithFeather_Mobile_TransClipping"),
//            new UTS2GUID(  "100d373b596f44d49ac9bb944d671d32", "Toon_DoubleShadeWithFeather_Mobile_TransClipping_StencilMask"),
            new UTS2GUID(  "036bc90bfe3475b4c9fadb85d0520621", "Toon_DoubleShadeWithFeather_StencilMask"),
            new UTS2GUID(  "0a1e4c9dcc0e9ea4db38ae9cb5059608", "Toon_DoubleShadeWithFeather_StencilOut"),
            new UTS2GUID(  "e8e7d781c3155254b9ea8956c5bd1218", "Toon_DoubleShadeWithFeather_TransClipping"),
            new UTS2GUID(  "79add09e32e5c4541980118f6c4045b6", "Toon_DoubleShadeWithFeather_TransClipping_StencilMask"),
            new UTS2GUID(  "fb47be5a840097b45bac228446468ef3", "Toon_DoubleShadeWithFeather_TransClipping_StencilOut"),
            new UTS2GUID(  "42a47eda2ed77084c9136507eadb8641", "Toon_OutlineObject"),
            new UTS2GUID(  "2e2edd12fbf6bcb4ea1f34c17ee42df5", "Toon_OutlineObject_StencilOut"),
            new UTS2GUID(  "ca035891872022e4f80c952b3916e450", "Toon_ShadingGradeMap"),
            new UTS2GUID(  "9aadc53d7cdc63f4898ea042aa9d853b", "Toon_ShadingGradeMap_AngelRing"),
            new UTS2GUID(  "23e399973d807464fb195291a44a614c", "Toon_ShadingGradeMap_AngelRing_Mobile"),
            new UTS2GUID(  "8d33e4e4084e5af449f3e762fecce3c9", "Toon_ShadingGradeMap_AngelRing_Mobile_StencilOut"),
            new UTS2GUID(  "415f07ab6fd766048ac6f8c2f2b406a9", "Toon_ShadingGradeMap_AngelRing_StencilOut"),
            new UTS2GUID(  "b2a70923168ea0c40a3051a013c93a8a", "Toon_ShadingGradeMap_AngelRing_TransClipping"),
            new UTS2GUID(  "d1e11a558d143f14c864edf263332764", "Toon_ShadingGradeMap_AngelRing_TransClipping_StencilOut"),
            new UTS2GUID(  "f90e11a40dcf4f745ae6b21b857943fa", "Toon_ShadingGradeMap_Mobile"),
            new UTS2GUID(  "206c554c8b0c60041a9d242385f543d3", "Toon_ShadingGradeMap_Mobile_StencilMask"),
            new UTS2GUID(  "cfc201757f2519c4bb6ef9265a046582", "Toon_ShadingGradeMap_Mobile_StencilOut"),
            new UTS2GUID(  "cce1da34c52aff745adf0222f56a356c", "Toon_ShadingGradeMap_Mobile_TransClipping"),
            new UTS2GUID(  "e88039bab21b7894e918126e8fce5d1b", "Toon_ShadingGradeMap_Mobile_TransClipping_StencilMask"),
            new UTS2GUID(  "aa2e05ed58ca15441bd0989f008da78b", "Toon_ShadingGradeMap_StencilMask"),
            new UTS2GUID(  "923058fda1b61544b93d91eeee772086", "Toon_ShadingGradeMap_StencilOut"),
            new UTS2GUID(  "aebd33b74ef849a4882b4a8d55f0f0c9", "Toon_ShadingGradeMap_TransClipping"),
            new UTS2GUID(  "0a05dd221bacbb448afac3d63e6bd833", "Toon_ShadingGradeMap_TransClipping_StencilMask"),
            new UTS2GUID(  "67212ac11ff43b04a833d3986b997a9f", "Toon_ShadingGradeMap_TransClipping_StencilOut"),

        };
        UTS2GUID[] tessShaders =
        {
            new UTS2GUID(  "5b8a1502578ed764c9880a7be65c9672", "ToonColor_DoubleShadeWithFeather_Clipping_Tess"),
            new UTS2GUID(  "682e6e6cf60a51040ade19437a3f53e2", "ToonColor_DoubleShadeWithFeather_Clipping_Tess_StencilMask"),
            new UTS2GUID(  "148d1eca2cf299e4eb949d15c4cf95ee", "ToonColor_DoubleShadeWithFeather_Clipping_Tess_StencilOut"),
            new UTS2GUID(  "e987cf9cca0941042aa68d1dd51ee20f", "ToonColor_DoubleShadeWithFeather_Tess"),
            new UTS2GUID(  "97df86a7afe06ef41b2a2c242b10593e", "ToonColor_DoubleShadeWithFeather_Tess_StencilMask"),
            new UTS2GUID(  "b179fb8a87955a347b5f594a18b43475", "ToonColor_DoubleShadeWithFeather_Tess_StencilOut"),
            new UTS2GUID(  "60fe384b76fb67d40bc7e38411073dd6", "ToonColor_DoubleShadeWithFeather_TransClipping_Tess"),
            new UTS2GUID(  "4a20b66d106d3f5409f759b5193ecdc2", "ToonColor_DoubleShadeWithFeather_TransClipping_Tess_StencilMask"),
            new UTS2GUID(  "a7842aa9522c7584cae2169b8e1ddb86", "ToonColor_DoubleShadeWithFeather_TransClipping_Tess_StencilOut"),
            new UTS2GUID(  "0cb6c9e6216a91e4a9d38cd2acb4ccb6", "ToonColor_DoubleShadeWithFeather_Transparent_Tess"),
            new UTS2GUID(  "f28bba8b2f259bb40b697d91849c8794", "ToonColor_DoubleShadeWithFeather_Transparent_Tess_StencilOut"),
            new UTS2GUID(  "4876871966ca2344793e439d7391d7b0", "ToonColor_ShadingGradeMap_AngelRing_Tess"),
            new UTS2GUID(  "7c48bdc9fed28c14b8ad0748673b1369", "ToonColor_ShadingGradeMap_AngelRing_Tess_StencilOut"),
            new UTS2GUID(  "d3fb22770ec830b43bdb5ccb973e6f76", "ToonColor_ShadingGradeMap_AngelRing_Tess_TransClipping"),
            new UTS2GUID(  "11e8f1e181e558a47a387492d3ecdb88", "ToonColor_ShadingGradeMap_AngelRing_TransClipping_Tess_StencilOut"),
            new UTS2GUID(  "01494e58d87212f44ab51d29caea84e4", "ToonColor_ShadingGradeMap_Tess"),
            new UTS2GUID(  "24c20b8ed5be113499b40f4e3b6b03e6", "ToonColor_ShadingGradeMap_Tess_StencilMask"),
            new UTS2GUID(  "9cf7e8eb46e9128438d50adf7a841de6", "ToonColor_ShadingGradeMap_Tess_StencilOut"),
            new UTS2GUID(  "3c39a77fda28b5043a7a17c7877cf7b2", "ToonColor_ShadingGradeMap_TransClipping_Tess"),
            new UTS2GUID(  "bf840a439c33c8b4a99d52e6c3d8511f", "ToonColor_ShadingGradeMap_TransClipping_Tess_StencilMask"),
            new UTS2GUID(  "8eff803eae89c994fae3acf2f686fafa", "ToonColor_ShadingGradeMap_TransClipping_Tess_StencilOut"),
            new UTS2GUID(  "0959cb8822a344c4da890457e668fdc9", "ToonColor_ShadingGradeMap_Transparent_Tess"),
            new UTS2GUID(  "6d115cf94d14d1842a56dfff76b57f42", "ToonColor_ShadingGradeMap_Transparent_Tess_StencilOut"),
            new UTS2GUID(  "f0b2fc9b8a189134da9c7d24f361caf4", "Toon_DoubleShadeWithFeather_Clipping_Tess"),
            new UTS2GUID(  "8c94ee3046ef0574f87f6b658b4e4691", "Toon_DoubleShadeWithFeather_Clipping_Tess_StencilMask"),
            new UTS2GUID(  "c4aed8662ca0f194284f3ab649e66d23", "Toon_DoubleShadeWithFeather_Clipping_Tess_StencilOut"),
            new UTS2GUID(  "1f248db3b28fc5f44aabd7aca618bd1e", "Toon_DoubleShadeWithFeather_Tess"),
            new UTS2GUID(  "a3214384442742648aa664ef0039d397", "Toon_DoubleShadeWithFeather_Tess_Light"),
            new UTS2GUID(  "3073cd2564e4cde45a19c05e0012d22a", "Toon_DoubleShadeWithFeather_Tess_Light_StencilMask"),
            new UTS2GUID(  "7e7690a767a07da4f943439680e70db8", "Toon_DoubleShadeWithFeather_Tess_Light_StencilOut"),
            new UTS2GUID(  "08c65988dc25d9f44b791fcc18fb543a", "Toon_DoubleShadeWithFeather_Tess_StencilMask"),
            new UTS2GUID(  "f937ea4ce96dfbe448afc0fb671198e5", "Toon_DoubleShadeWithFeather_Tess_StencilOut"),
            new UTS2GUID(  "3fb99ac3775edeb4aa9530db5a614c92", "Toon_DoubleShadeWithFeather_TransClipping_Tess"),
            new UTS2GUID(  "9855f226cd8152d4e99085272aceede6", "Toon_DoubleShadeWithFeather_TransClipping_Tess_StencilMask"),
            new UTS2GUID(  "2a0d4af863770404faee6488b86fe3c9", "Toon_DoubleShadeWithFeather_TransClipping_Tess_StencilOut"),
            new UTS2GUID(  "1847c44f729b68e49ba63610abdf9132", "Toon_OutlineObject_Tess"),
            new UTS2GUID(  "06cae78b869a3234bab02eeb52197e1c", "Toon_OutlineObject_Tess_StencilOut"),
            new UTS2GUID(  "3a1af221400a61a4b94bae19aa79da2b", "Toon_ShadingGradeMap_AngelRing_Tess"),
            new UTS2GUID(  "a1449ab672051624ca3160737b630f5e", "Toon_ShadingGradeMap_AngelRing_Tess_Light"),
            new UTS2GUID(  "79d3dc54c32b69b42be17c48d33575f2", "Toon_ShadingGradeMap_AngelRing_Tess_Light_StencilOut"),
            new UTS2GUID(  "18c9172cdf36a344f9aca9bbc0e7002d", "Toon_ShadingGradeMap_AngelRing_Tess_StencilOut"),
            new UTS2GUID(  "54a94f776a43a074c8c2d205bb934005", "Toon_ShadingGradeMap_AngelRing_TransClipping_Tess"),
            new UTS2GUID(  "d496a1c70c797ad43836d5bfff575b5f", "Toon_ShadingGradeMap_AngelRing_TransClipping_Tess_StencilOut"),
            new UTS2GUID(  "183ea557143786346b1bfc862ad22636", "Toon_ShadingGradeMap_Tess"),
            new UTS2GUID(  "356dd5af8f0d40e41b647d3d0a0555c1", "Toon_ShadingGradeMap_Tess_Light"),
            new UTS2GUID(  "ffadecfbd9e31f840ba4109fea0f0436", "Toon_ShadingGradeMap_Tess_Light_StencilMask"),
            new UTS2GUID(  "98ac5d198a471494da681b7b8d1e1727", "Toon_ShadingGradeMap_Tess_Light_StencilOut"),
            new UTS2GUID(  "0d799eb857c0e2c45bbdfb2c033d33e6", "Toon_ShadingGradeMap_Tess_StencilMask"),
            new UTS2GUID(  "e667137c8b6fd3d4390fc364b2e5c70b", "Toon_ShadingGradeMap_Tess_StencilOut"),
            new UTS2GUID(  "feba437d8ff93f745a78828529e9a272", "Toon_ShadingGradeMap_TransClipping_Tess"),
            new UTS2GUID(  "8d1395a9f4bfad44d8fddd0f2af19b1e", "Toon_ShadingGradeMap_TransClipping_Tess_StencilMask"),
            new UTS2GUID(  "08c6bb334aed21c4198cf46b71ebca2d", "Toon_ShadingGradeMap_TransClipping_Tess_StencilOut"),

        };
        const string ShaderDefineSHADINGGRADEMAP = "_SHADINGGRADEMAP";
        const string ShaderDefineANGELRING_ON = "_IS_ANGELRING_ON";
        const string ShaderDefineANGELRING_OFF = "_IS_ANGELRING_OFF";

        const string ShaderDefineIS_TRANSCLIPPING_OFF = "_IS_TRANSCLIPPING_OFF";
        const string ShaderDefineIS_TRANSCLIPPING_ON = "_IS_TRANSCLIPPING_ON";

        const string ShaderDefineIS_CLIPPING_OFF = "_IS_CLIPPING_OFF";
        const string ShaderDefineIS_CLIPPING_MODE = "_IS_CLIPPING_MODE";
        const string ShaderDefineIS_CLIPPING_TRANSMODE = "_IS_CLIPPING_TRANSMODE";
        const string ShaderDefineIS_OUTLINE_CLIPPING_NO = "_IS_OUTLINE_CLIPPING_NO";
        const string ShaderDefineIS_OUTLINE_CLIPPING_YES = "_IS_OUTLINE_CLIPPING_YES";

        const string ShaderPropAngelRing = "_AngelRing";

        const string ShaderProp1st_ShadeColor_Step = "_1st_ShadeColor_Step";
        const string ShaderPropBaseColor_Step = "_BaseColor_Step";
        const string ShaderProp1st_ShadeColor_Feather = "_1st_ShadeColor_Feather";
        const string ShaderPropBaseShade_Feather = "_BaseShade_Feather";
        const string ShaderProp2nd_ShadeColor_Step = "_2nd_ShadeColor_Step";
        const string ShaderPropShadeColor_Step = "_ShadeColor_Step";
        const string ShaderProp2nd_ShadeColor_Feather = "_2nd_ShadeColor_Feather";
        const string ShaderProp1st2nd_Shades_Feather = "_1st2nd_Shades_Feather";
        const string ShaderPropIs_NormalMapForMatCap = "_Is_NormalMapForMatCap";
        const string ShaderPropIs_UseTweakMatCapOnShadow = "_Is_UseTweakMatCapOnShadow";
        const string ShaderPropIs_ViewCoord_Scroll = "_Is_ViewCoord_Scroll";
        const string ShaderPropIs_PingPong_Base = "_Is_PingPong_Base";

        const string ShaderPropMatCap = "_MatCap";
        const string ShaderPropClippingMode = "_ClippingMode";

        const string ShaderPropOutline = "_OUTLINE";
        const string ShaderPropIs_Ortho = "_Is_Ortho";
        const string ShaderPropGI_Intensity = "_GI_Intensity";
        const string ShaderPropCameraRolling_Stabilizer = "_CameraRolling_Stabilizer";
        const string ShaderPropIs_Filter_LightColor = "_Is_Filter_LightColor";
        const string ShaderPropUnlit_Intensity = "_Unlit_Intensity";
        const string ShaderPropStencilMode = "_StencilMode";
        const string ShaderPropStencilNo = "_StencilNo";
        const string ShaderPropTransparentEnabled = "_TransparentEnabled";
        const string ShaderPropStencilComp = "_StencilComp";
        const string ShaderPropStencilOpPass = "_StencilOpPass";
        const string ShaderPropStencilOpFail = "_StencilOpFail";

        const string ShaderPropIsLightColor_Base = "_Is_LightColor_Base";
        const string ShaderPropIs_LightColor_1st_Shade = "_Is_LightColor_1st_Shade";
        const string ShaderPropIs_LightColor_2nd_Shade = "_Is_LightColor_2nd_Shade";
        const string ShaderPropIs_LightColor_HighColor = "_Is_LightColor_HighColor";
        const string ShaderPropIs_LightColor_RimLight = "_Is_LightColor_RimLight";
        const string ShaderPropIs_LightColor_Ap_RimLight = "_Is_LightColor_Ap_RimLight";
        const string ShaderPropIs_LightColor_MatCap = "_Is_LightColor_MatCap";
        const string ShaderPropIs_LightColor_AR = "_Is_LightColor_AR";
        const string ShaderPropIs_LightColor_Outline = "_Is_LightColor_Outline";
        const string ShaderPropSetSystemShadowsToBase = "_Set_SystemShadowsToBase";
        const string ShaderPropIsFilterHiCutPointLightColor = "_Is_Filter_HiCutPointLightColor";


        const string ShaderPropAutoRenderQueue = "_AutoRenderQueue";
        const string ShaderPropUtsTechniqe = "_utsTechnique";



        public int _autoRenderQueue = 1;
        public int _renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        static _UTS_Transparent _Transparent_Setting;
        static int _StencilNo_Setting;
        //        static bool _OriginalInspector = false;
        //        static bool _SimpleUI = false;

        // for converter
        Vector2 m_scrollPos;
        bool m_initialzed;
        string[] guids;
        const string legacyShaderPrefix = "UnityChanToonShader/";
        readonly string[] m_RendderPipelineNames = { "Legacy", "Universal", "HDRP" };
        int m_selectedRenderPipeline;
        int m_materialCount = 0;
        [MenuItem("Window/Toon Shader/Unitychan Toon Shader Material Converter", false, 9999)]
        static private void OpenWindow()
        {
            var window = GetWindow<UnitychanToonShader2UnityToonShader>(true, "Unitychan Toon Shader Material Converter");
            window.Show();
        }

        private void OnGUI()
        {

            if (!m_initialzed)
            {
                guids = AssetDatabase.FindAssets("t:Material", null);
            }
            m_initialzed = true;
            int labelHeight = 40;
            int buttonHeight = 20;
            Rect rect = new Rect(0, labelHeight, position.width, position.height - buttonHeight ); // GUILayoutUtility.GetRect(position.width, position.height - buttonHeight);
            Rect rect2 = new Rect(2, labelHeight, position.width - 4, position.height - 4 - buttonHeight );
            // scroll view background
            EditorGUI.DrawRect(rect, Color.gray);
            EditorGUI.DrawRect(rect2, new Color(0.3f, 0.3f, 0.3f));
            EditorGUILayout.LabelField("Make sure that Unity Toon Shader is not installed in the project. ");
            using (new EditorGUI.DisabledScope(m_materialCount == 0))
            {

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Convert to ");
                m_selectedRenderPipeline = EditorGUILayout.Popup(m_selectedRenderPipeline, m_RendderPipelineNames);
                EditorGUILayout.EndHorizontal();
            }


            // scroll view 
            m_scrollPos =
                 EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(position.width - 4));
            EditorGUILayout.BeginVertical();


            int materialCount = 0;
            int versionErrorCount = 0;
            for (int ii = 0; ii < guids.Length; ii++)
            {
                var guid = guids[ii];


                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                
                var shaderName = material.shader.ToString();
                if (!shaderName.StartsWith(legacyShaderPrefix))
                {
                    continue;

                }
                const string utsVersionProp = "_utsVersion";
                if (material.HasProperty(utsVersionProp))
                {
                    float utsVersion = material.GetFloat(utsVersionProp);
                    if (utsVersion < 2.07)
                    {
                        versionErrorCount++;
                        continue;
                    }
                }
                else
                {
                    versionErrorCount++;
                    continue;
                }
                materialCount++;
                Debug.Log(shaderName);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(16);
                string str = "" + materialCount + ":";

                EditorGUILayout.LabelField(str, GUILayout.Width(40));
                EditorGUILayout.LabelField(path, GUILayout.Width(Screen.width - 130));
                GUILayout.Space(1);
                EditorGUILayout.EndHorizontal();
            }
            m_materialCount = materialCount;
            if (m_materialCount == 0)
            {
                GUILayout.Space(16);
                if (versionErrorCount > 0 )
                {
                    EditorGUILayout.LabelField("   Error: Unitychan Toon Shader version must be newer than 2.0.7");
                }
                else
                {
                    EditorGUILayout.LabelField("   No Unitychan Toon Shader material was found.");
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();


            // buttons 
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(m_materialCount == 0))
            {
                if (GUILayout.Button(new GUIContent("Convert")))
                {
                    ConvertMaterials(m_selectedRenderPipeline, guids);
                }
            }
            if ( GUILayout.Button(new GUIContent("Close")) )
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();


        }

        void ConvertMaterials(int renderPipelineIndex, string[] guids)
        {


 


            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material  = AssetDatabase.LoadAssetAtPath<Material>(path) ;
                var shaderName = material.shader.ToString();
                if (!shaderName.StartsWith(legacyShaderPrefix))
                {
                    continue;

                }
                Debug.Log(shaderName);
                switch (renderPipelineIndex)
                {
                    case 0: // built in
                        material.shader = Shader.Find("Toon (Built-in)"); 
                        break;
                    case 1: // Universal
                        material.shader = Shader.Find("Universal Render Pipeline/Toon");
                        break;
                    case 2: // HDRP
                        material.shader = Shader.Find("HDRP/Toon");
                        break;
                }

               
                _Transparent_Setting = (_UTS_Transparent)material.GetInt(ShaderPropTransparentEnabled);
                _StencilNo_Setting = material.GetInt(ShaderPropStencilNo);
                _autoRenderQueue = material.GetInt(ShaderPropAutoRenderQueue);
                _renderQueue = material.renderQueue;
                _UTS_Technique technique = (_UTS_Technique)material.GetInt(ShaderPropUtsTechniqe);

                switch (technique)
                {
                    case _UTS_Technique.DoubleShadeWithFeather:
                        material.DisableKeyword(ShaderDefineSHADINGGRADEMAP);
                        break;
                    case _UTS_Technique.ShadingGradeMap:
                        material.EnableKeyword(ShaderDefineSHADINGGRADEMAP);
                        break;
                }
                BasicLookdevs(material);
                SetGameRecommendation(material);
                ApplyClippingMode(material);
                ApplyStencilMode(material);
                ApplyAngelRing(material);
                ApplyMatCapMode(material);
                ApplyQueueAndRenderType(technique, material);


            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }
        void BasicLookdevs(Material material)
        {
            if (material.HasProperty(ShaderPropUtsTechniqe))//DoubleWithFeather or ShadingGradeMap
            {
                if (material.GetInt(ShaderPropUtsTechniqe) == (int)_UTS_Technique.DoubleShadeWithFeather)   //DWF
                {

                    //Sharing variables with ShadingGradeMap method.

                    material.SetFloat(ShaderProp1st_ShadeColor_Step, material.GetFloat(ShaderPropBaseColor_Step));
                    material.SetFloat(ShaderProp1st_ShadeColor_Feather, material.GetFloat(ShaderPropBaseShade_Feather));
                    material.SetFloat(ShaderProp2nd_ShadeColor_Step, material.GetFloat(ShaderPropShadeColor_Step));
                    material.SetFloat(ShaderProp2nd_ShadeColor_Feather, material.GetFloat(ShaderProp1st2nd_Shades_Feather));
                }
                else if (material.GetInt(ShaderPropUtsTechniqe) == (int)_UTS_Technique.ShadingGradeMap)
                {    //SGM

                    //Share variables with DoubleWithFeather method.
                    material.SetFloat(ShaderPropBaseColor_Step, material.GetFloat(ShaderProp1st_ShadeColor_Step));
                    material.SetFloat(ShaderPropBaseShade_Feather, material.GetFloat(ShaderProp1st_ShadeColor_Feather));
                    material.SetFloat(ShaderPropShadeColor_Step, material.GetFloat(ShaderProp2nd_ShadeColor_Step));
                    material.SetFloat(ShaderProp1st2nd_Shades_Feather, material.GetFloat(ShaderProp2nd_ShadeColor_Feather));
                }
                else
                {
                    // OutlineObj.
                    return;
                }
            }
            EditorGUILayout.Space();
        }
        private bool IsShadingGrademap(Material material)
        {
            return material.GetInt(ShaderPropUtsTechniqe) == (int)_UTS_Technique.ShadingGradeMap;
        }

        void ApplyQueueAndRenderType(_UTS_Technique technique, Material material)
        {
            var stencilMode = (_UTS_StencilMode)material.GetInt(ShaderPropStencilMode);
            if (_autoRenderQueue == 1)
            {
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }

            const string OPAQUE = "Opaque";
            const string TRANSPARENTCUTOUT = "TransparentCutOut";
            const string TRANSPARENT = "Transparent";
            const string RENDERTYPE = "RenderType";
            const string IGNOREPROJECTION = "IgnoreProjection";
            const string DO_IGNOREPROJECTION = "True";
            const string DONT_IGNOREPROJECTION = "False";
            var renderType = OPAQUE;
            var ignoreProjection = DONT_IGNOREPROJECTION;

            if (_Transparent_Setting == _UTS_Transparent.On)
            {
                renderType = TRANSPARENT;
                ignoreProjection = DO_IGNOREPROJECTION;
            }
            else
            {
                switch (technique)
                {
                    case _UTS_Technique.DoubleShadeWithFeather:
                        {
                            _UTS_ClippingMode clippingMode = (_UTS_ClippingMode)material.GetInt(ShaderPropClippingMode);
                            if (clippingMode == _UTS_ClippingMode.Off)
                            {

                            }
                            else
                            {
                                renderType = TRANSPARENTCUTOUT;

                            }

                            break;
                        }
                    case _UTS_Technique.ShadingGradeMap:
                        {
                            _UTS_TransClippingMode transClippingMode = (_UTS_TransClippingMode)material.GetInt(ShaderPropClippingMode);
                            if (transClippingMode == _UTS_TransClippingMode.Off)
                            {
                            }
                            else
                            {
                                renderType = TRANSPARENTCUTOUT;

                            }

                            break;
                        }
                }

            }
            if (_autoRenderQueue == 1)
            {
                if (_Transparent_Setting == _UTS_Transparent.On)
                {
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
                else if (stencilMode == _UTS_StencilMode.StencilMask)
                {
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest - 1;
                }
                else if (stencilMode == _UTS_StencilMode.StencilOut)
                {
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                }
            }
            else
            {
                material.renderQueue = _renderQueue;
            }

            material.SetOverrideTag(RENDERTYPE, renderType);
            material.SetOverrideTag(IGNOREPROJECTION, ignoreProjection);
        }
        void ApplyMatCapMode(Material material)
        {
            if (material.GetInt(ShaderPropClippingMode) == 0)
            {
                if (material.GetFloat(ShaderPropMatCap) == 1)
                    material.EnableKeyword(ShaderPropMatCap);
                else
                    material.DisableKeyword(ShaderPropMatCap);
            }
            else
            {
                material.DisableKeyword(ShaderPropMatCap);
            }
        }

        void ApplyAngelRing(Material material)
        {
            int angelRingEnabled = material.GetInt(ShaderPropAngelRing);
            if (angelRingEnabled == 0)
            {
                material.DisableKeyword(ShaderDefineANGELRING_ON);
                material.EnableKeyword(ShaderDefineANGELRING_OFF);
            }
            else
            {
                material.EnableKeyword(ShaderDefineANGELRING_ON);
                material.DisableKeyword(ShaderDefineANGELRING_OFF);

            }
        }

        void ApplyStencilMode(Material material)
        {
            _UTS_StencilMode mode = (_UTS_StencilMode)(material.GetInt(ShaderPropStencilMode));
            switch (mode)
            {
                case _UTS_StencilMode.Off:
                    //    material.SetInt(ShaderPropStencilNo,0);
                    material.SetInt(ShaderPropStencilComp, (int)_StencilCompFunction.Disabled);
                    material.SetInt(ShaderPropStencilOpPass, (int)_StencilOperation.Keep);
                    material.SetInt(ShaderPropStencilOpFail, (int)_StencilOperation.Keep);
                    break;
                case _UTS_StencilMode.StencilMask:
                    //    material.SetInt(ShaderPropStencilNo,0);
                    material.SetInt(ShaderPropStencilComp, (int)_StencilCompFunction.Always);
                    material.SetInt(ShaderPropStencilOpPass, (int)_StencilOperation.Replace);
                    material.SetInt(ShaderPropStencilOpFail, (int)_StencilOperation.Replace);
                    break;
                case _UTS_StencilMode.StencilOut:
                    //    material.SetInt(ShaderPropStencilNo,0);
                    material.SetInt(ShaderPropStencilComp, (int)_StencilCompFunction.NotEqual);
                    material.SetInt(ShaderPropStencilOpPass, (int)_StencilOperation.Keep);
                    material.SetInt(ShaderPropStencilOpFail, (int)_StencilOperation.Keep);

                    break;
            }



        }
        void ApplyClippingMode(Material material)
        {

            if (!IsShadingGrademap(material))
            {


                material.DisableKeyword(ShaderDefineIS_TRANSCLIPPING_OFF);
                material.DisableKeyword(ShaderDefineIS_TRANSCLIPPING_ON);

                switch (material.GetInt(ShaderPropClippingMode))
                {
                    case 0:
                        material.EnableKeyword(ShaderDefineIS_CLIPPING_OFF);
                        material.DisableKeyword(ShaderDefineIS_CLIPPING_MODE);
                        material.DisableKeyword(ShaderDefineIS_CLIPPING_TRANSMODE);
                        material.EnableKeyword(ShaderDefineIS_OUTLINE_CLIPPING_NO);
                        material.DisableKeyword(ShaderDefineIS_OUTLINE_CLIPPING_YES);
                        break;
                    case 1:
                        material.DisableKeyword(ShaderDefineIS_CLIPPING_OFF);
                        material.EnableKeyword(ShaderDefineIS_CLIPPING_MODE);
                        material.DisableKeyword(ShaderDefineIS_CLIPPING_TRANSMODE);
                        material.DisableKeyword(ShaderDefineIS_OUTLINE_CLIPPING_NO);
                        material.EnableKeyword(ShaderDefineIS_OUTLINE_CLIPPING_YES);
                        break;
                    default:
                        material.DisableKeyword(ShaderDefineIS_CLIPPING_OFF);
                        material.DisableKeyword(ShaderDefineIS_CLIPPING_MODE);
                        material.EnableKeyword(ShaderDefineIS_CLIPPING_TRANSMODE);
                        material.DisableKeyword(ShaderDefineIS_OUTLINE_CLIPPING_NO);
                        material.EnableKeyword(ShaderDefineIS_OUTLINE_CLIPPING_YES);
                        break;
                }
            }
            else
            {


                material.DisableKeyword(ShaderDefineIS_CLIPPING_OFF);
                material.DisableKeyword(ShaderDefineIS_CLIPPING_MODE);
                material.DisableKeyword(ShaderDefineIS_CLIPPING_TRANSMODE);
                switch (material.GetInt(ShaderPropClippingMode))
                {
                    case 0:
                        material.EnableKeyword(ShaderDefineIS_TRANSCLIPPING_OFF);
                        material.DisableKeyword(ShaderDefineIS_TRANSCLIPPING_ON);
                        break;
                    default:
                        material.DisableKeyword(ShaderDefineIS_TRANSCLIPPING_OFF);
                        material.EnableKeyword(ShaderDefineIS_TRANSCLIPPING_ON);
                        break;

                }

            }

        }
        void SetGameRecommendation(Material material)
        {


            material.SetFloat(ShaderPropIsLightColor_Base, 1);
            material.SetFloat(ShaderPropIs_LightColor_1st_Shade, 1);
            material.SetFloat(ShaderPropIs_LightColor_2nd_Shade, 1);
            material.SetFloat(ShaderPropIs_LightColor_HighColor, 1);
            material.SetFloat(ShaderPropIs_LightColor_RimLight, 1);
            material.SetFloat(ShaderPropIs_LightColor_Ap_RimLight, 1);
            material.SetFloat(ShaderPropIs_LightColor_MatCap, 1);
            if (material.HasProperty(ShaderPropAngelRing))
            {//When AngelRing is available
                material.SetFloat(ShaderPropIs_LightColor_AR, 1);
            }
            if (material.HasProperty(ShaderPropOutline))//OUTLINEがある場合.
            {
                material.SetFloat(ShaderPropIs_LightColor_Outline, 1);
            }
            material.SetFloat(ShaderPropSetSystemShadowsToBase, 1);
            material.SetFloat(ShaderPropIsFilterHiCutPointLightColor, 1);
            material.SetFloat(ShaderPropCameraRolling_Stabilizer, 1);
            material.SetFloat(ShaderPropIs_Ortho, 0);
            material.SetFloat(ShaderPropGI_Intensity, 0);
            material.SetFloat(ShaderPropUnlit_Intensity, 1);
            material.SetFloat(ShaderPropIs_Filter_LightColor, 1);
        }
    }
}