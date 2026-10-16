Shader "Separate Alpha Mask" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Alpha ("Alpha (A)", 2D) = "white" {}
    }
    SubShader {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }

        Pass {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _Alpha;
            uniform float4 _Alpha_ST;

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvMain : TEXCOORD0;
                float2 uvAlpha : TEXCOORD1;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvAlpha = TRANSFORM_TEX(v.texcoord, _Alpha);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col;
                col.rgb = tex2D(_MainTex, i.uvMain).rgb;
                col.a = tex2D(_Alpha, i.uvAlpha).a;
                return col;
            }
            ENDCG
        }
    }
}