#ifndef UNIVERSAL_UTS_FALLBACK_2D_INCLUDED
#define UNIVERSAL_UTS_FALLBACK_2D_INCLUDED

float4 _MainTex_ST;
TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

struct Attributes
{
    float4 positionOS       : POSITION;
    float2 uv               : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv        : TEXCOORD0;
    float4 vertex : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};



Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.vertex = vertexInput.positionCS;
    output.uv = TRANSFORM_TEX(input.uv, _MainTex);

    return output;
}

half4 frag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    half2 uv = input.uv;
    half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(uv, _MainTex));
//    half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
    half3 color = texColor.rgb * _BaseColor.rgb;
    half alpha = texColor.a * _BaseColor.a;
    AlphaDiscard(alpha, _Cutoff);

#ifdef _ALPHAPREMULTIPLY_ON
    color *= alpha;
#endif
    return half4(color, alpha);
}

#endif
