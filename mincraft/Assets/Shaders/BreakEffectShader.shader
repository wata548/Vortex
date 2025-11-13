Shader "Custom/BreakEffectShader"
{
    Properties
    {
        _MainTex ("TileMap", 2D) = "white" {}
        _Size("Size", Vector) = (0,0,0,0)
        _Pos("Pos", Vector) = (0,0,0,0)
        _Color ("Color", Color) = (1,1,1,1)
        
        _Center("Center", Vector) = (0,0,0,0)
        _Scale("Scale", Float) = 0.2
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
            float4 _Color;
            float4 _Size;
            float4 _Pos;
            float4 _Center;
            float _Scale;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed2 uv = i.uv;
                uv.x = (_Pos.x + _Center.x + uv.x * _Scale - _Scale / 2) / _Size.x;
                uv.y = (_Size.y - _Pos.y - 1 + _Center + uv.y * _Scale - _Scale / 2) / _Size.y;
                
                fixed4 o = tex2D(_MainTex, uv) * _Color;
                return o;
            }
            ENDCG
        }
    }
}