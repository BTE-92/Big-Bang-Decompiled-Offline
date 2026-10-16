Shader "WOE/Fx/Alpha Additive Overdraw" {
    Properties {
        _MainTex ("Particle Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "QUEUE"="Geometry+1" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }

        Pass {
            Tags { "QUEUE"="Geometry+1" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
            ZTest Always
            ZWrite Off
            Cull Off
            Blend SrcAlpha One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR0;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = saturate(v.color);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}