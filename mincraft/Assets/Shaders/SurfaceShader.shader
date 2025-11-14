Shader "Custom/SurfaceTiling" {
    
    Properties
    {
        _Size ("Size", Vector) = (0,0,0,0)
        _Color ("Color", Color) = (0,0,0,0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        
        CGPROGRAM
        #pragma vertex vert
        #pragma surface surf Standard fullforwardshadows alpha:clip

        #pragma target 3.0

        sampler2D _MainTex;
        float2 _Size;
        float4 _Color;

        struct Input
        {
            float4 posAndUv;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.posAndUv = v.texcoord;
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed2 uvPos = IN.posAndUv.xy;
            fixed2 uvIdx = IN.posAndUv.zw;
            fixed2 atlasUv;

            atlasUv.x = (uvIdx.x + frac(uvPos.x)) / _Size.x;
            atlasUv.y = (_Size.y - 1 - uvIdx.y + frac(uvPos.y)) / _Size.y;

            fixed4 c = tex2Dgrad(_MainTex, atlasUv, ddx(atlasUv * _Size), ddy(atlasUv * _Size)) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            clip(o.Alpha - 0.5);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
