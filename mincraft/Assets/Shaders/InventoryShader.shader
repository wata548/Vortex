Shader "Hidden/InventoryShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,0)
        _Size ("Size", Vector) = (0,0,0,0)
        _Pos("Pos", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
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
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _Size;
            float4 _Color;
            float4 _Pos;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.x = (_Pos.x + uv.x) / _Size.x;
                uv.y = (_Size.y - 1 - _Pos.y + uv.y) / _Size.y;
                
                fixed4 col = tex2D(_MainTex, uv);
                col.rgb = col.rgb * _Color;
                return col;
            }
            ENDCG
        }
    }
}
