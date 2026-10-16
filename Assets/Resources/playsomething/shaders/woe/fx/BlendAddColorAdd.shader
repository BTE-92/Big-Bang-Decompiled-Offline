Shader "WOE/Fx/BlendAddColorAdd" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ColorTint ("Color Tint", Color) = (0,0,0,1)
    }
    SubShader {
        LOD 100
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }

        Pass {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ SOFTPARTICLES_ON
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed4 _ColorTint;

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Additive combination of vertex color + _ColorTint multiplied by texture
                fixed4 c = tex2D(_MainTex, i.uv) * (i.color + _ColorTint);
                
                // Premultiplied alpha calculation
                c.rgb *= c.a;
                c.a *= c.a;

                return c;
            }
            ENDCG
        }
    }
}