#include <UnityCG.cginc>


struct appdata {
    float4 vertex : POSITION;
};

struct v2f {
    float4 vertex : SV_POSITION;
};

//----------------------------------------------------------------------------------------------------------------------
struct DummyTessellationFactors
{
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

[UNITY_domain("tri")]
v2f ds_surf(DummyTessellationFactors factors, const OutputPatch<appdata, 3> patch, float3 bary : SV_DomainLocation) {
 
   v2f o;
    // Interpolate position using barycentric coordinates
    o.vertex = patch[0].vertex * bary.x +
            patch[1].vertex * bary.y +
            patch[2].vertex * bary.z;
    return o;
}

//----------------------------------------------------------------------------------------------------------------------

sampler2D _MainTex;
float4 _MainTex_ST;

v2f vert(appdata v) {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    return o;
}

fixed4 frag(v2f i) : SV_Target {
    return fixed4(1,0,1,1);
}
