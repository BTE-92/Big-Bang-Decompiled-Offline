Shader "Projector/Light" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ShadowTex ("Cookie", 2D) = "gray" {}
        _FalloffTex ("FallOff", 2D) = "white" {}
    }
    SubShader {
        Pass {
            ZWrite Off
            Blend DstColor One
            ColorMask RGB
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _ShadowTex;
            uniform float4 _ShadowTex_ST;
            uniform sampler2D _FalloffTex;
            uniform float4 _FalloffTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR0;
                float2 uvShadow : TEXCOORD0;
                float2 uvFalloff : TEXCOORD1;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = saturate(_Color);
                o.uvShadow = TRANSFORM_TEX(v.texcoord, _ShadowTex);
                o.uvFalloff = TRANSFORM_TEX(v.texcoord, _FalloffTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 texShadow = tex2D(_ShadowTex, i.uvShadow);
                fixed4 texFalloff = tex2D(_FalloffTex, i.uvFalloff);

                fixed4 col;
                col.rgb = texShadow.rgb * i.color.rgb;
                col.a = 1.0 - texShadow.a;

                col *= texFalloff.a;

                return col;
            }
            ENDCG
        }
    }
}