namespace UnityEditor.Rendering.Toon
{
    internal sealed class HdrpUTS3toIntegratedUTS3Converter : RenderPipelineConverterContainer
    {
        static readonly internal UTSGUID kOrgShaderGUID = new UTSGUID("873188af6a7b5ca49aa69929a5d863c1", "HDRP/Toon");
        static readonly internal UTSGUID kOrgTessShaderGUID = new UTSGUID("6499b7b5ccaae6944ae5fe89b016c50b", "HDRP/ToonTessellation", true);

        internal override string name => "Unity Toon Shader(HDRP) 0.7.x or earlier";

        public override int priority => -9000;
        public override void SetupConverter()
        {
            SetupConverterCommon(kOrgShaderGUID, kOrgTessShaderGUID);
        }
        public override void Convert()
        {
            CommonConvert();
            SendAnalyticsEvent();
        }

        public override void PostConverting() { }


        public override int CountErrors(bool addToScrollView) { return 0; }

        public override InstalledStatus CheckSourceShaderInstalled() { return InstalledStatus.NotInstalled; }
    }
}