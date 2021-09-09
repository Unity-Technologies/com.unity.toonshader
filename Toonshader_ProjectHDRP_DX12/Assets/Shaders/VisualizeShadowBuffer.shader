Shader "UTJ/Raytraced Hard Shadow/Visualize Shadow Buffer"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma only_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct ia_out
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct vs_out
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Texture2D<float> _MainTex;
            float4 _MainTex_TexelSize;

            vs_out vert(ia_out v)
            {
                vs_out o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(vs_out i) : SV_Target
            {
                float2 uv = (1.0f.xx - i.uv) * _MainTex_TexelSize.zw;

                float r = _MainTex[uv].x;
                return float4(r, r, r, 1.0f);
            }
            ENDCG
        }
    }
}
