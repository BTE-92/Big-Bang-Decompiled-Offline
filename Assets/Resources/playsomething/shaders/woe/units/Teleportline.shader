Shader "WOE/Units/Teleportline" {
    Properties {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" { }
        _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
    }
    SubShader { 
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
        }
        Pass {
            Tags { 
                "Queue"="Transparent" 
                "IgnoreProjector"="True" 
            }
            ZTest Always
            ZWrite Off
            Blend SrcAlpha One
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile SOFTPARTICLES_OFF SOFTPARTICLES_ON
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
                float2 uv : TEXCOORD0;
                #if defined(SOFTPARTICLES_ON)
                    float4 projPos : TEXCOORD1;
                #endif
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;

            #if defined(SOFTPARTICLES_ON)
                sampler2D_float _CameraDepthTexture;
                float _InvFade;
            #endif

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                #if defined(SOFTPARTICLES_ON)
                    o.projPos = ComputeScreenPos(o.vertex);
                    COMPUTE_EYEDEPTH(o.projPos.z);
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                #if defined(SOFTPARTICLES_ON)
                    float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                    float partZ = i.projPos.z;
                    float fade = saturate(_InvFade * (sceneZ - partZ));
                    i.color.a *= fade;
                #endif

                // Exact 2.0x vertex color multiplication with tint and texture sample
                fixed4 col = (2.0 * i.color) * (_TintColor * tex2D(_MainTex, i.uv));
                return col;
            }
            ENDCG
        }
    }
}