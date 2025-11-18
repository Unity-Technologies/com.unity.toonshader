#include <UnityCG.cginc>

struct appdata {
    float4 vertex : POSITION;
};

struct v2f {
    float4 vertex : SV_POSITION;
};

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
