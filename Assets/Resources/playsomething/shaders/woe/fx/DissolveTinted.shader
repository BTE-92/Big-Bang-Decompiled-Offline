Shader "WOE/Fx/DissolveTinted" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" { }
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader { 
        Tags { 
            "Queue"="AlphaTest" 
            "RenderType"="TransparentCutout" 
        }
        Pass {
            Tags { 
                "Queue"="AlphaTest" 
                "RenderType"="TransparentCutout" 
            }
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);

                // Dissolve threshold calculation: ((tex.a * _TintColor.a) + vertex.a) * vertex.a
                half dissolveFactor = ((tex.a * _TintColor.a) + i.color.a) * i.color.a;

                // Cutoff discard logic (discard pixels below 0.5 threshold)
                clip(dissolveFactor - 0.5);

                // Edge glow calculation around the dissolve boundary
                half edgeGlow = saturate((dissolveFactor * 20.0) - 9.999998) * i.color.a;

                fixed4 col;
                // RGB = (MainTex * VertexColor * TintColor) + EdgeGlow
                col.rgb = (tex.rgb * i.color.rgb * _TintColor.rgb) + edgeGlow;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }
}