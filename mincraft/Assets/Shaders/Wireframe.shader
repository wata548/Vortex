Shader "Custom/Wireframe"
{
    Properties
    {
        _MainTex ("OutLineTexture", 2D) = "white" {}
        _Color1 ("Texture", Color) = (1,1,1,1)
        
        _Break ("BreakTexture", 2D) = "white" {}
        _Color2 ("Texture", Color) = (1,1,1,1)
        _Thickness("Thickness", float) = 0.01
        _BreakProcess("Break", float) = 0.01
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags
        {
            "Queue"="Overlay"
            "RenderType"="Transparent"
            "IgnoreProjector" = "True"
        }
        LOD 100
        Pass
        {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Break;
            float4 _Color1;
            float4 _Color2;
            float4 _MainTex_ST;
            float _Thickness;
            float _BreakProcess;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 o = tex2D(_MainTex, i.uv) * _Color1;

                if (i.uv.x <= _Thickness || i.uv.x >= 1 - _Thickness || i.uv.y <= _Thickness || i.uv.y >= 1 - _Thickness)
                    return o;
                
                if (i.uv.x <= 0.5f + _BreakProcess / 2 && i.uv.x >= 0.5f - _BreakProcess / 2 && i.uv.y <= 0.5f + _BreakProcess / 2 && i.uv.y >= 0.5f - _BreakProcess / 2) {
                    o = tex2D(_Break, i.uv) * _Color2;
                }
                else
                    o.w = 0;

                return o;
            }
            ENDCG
        }
    }
}