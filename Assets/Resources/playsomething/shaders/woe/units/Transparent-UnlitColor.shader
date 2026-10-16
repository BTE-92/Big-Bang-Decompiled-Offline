Shader "WOE/Units/Transparent-UnlitColor" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader { 
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="WOEUnit" 
        }

        // --- PASS 1: FORWARD BASE (Ambient SH, Vertex Lights, & Alpha Blending) ---
        Pass {
            Name "FORWARD"
            Tags { 
                "LightMode"="ForwardBase" 
                "Queue"="Transparent" 
                "SHADOWSUPPORT"="true" 
                "RenderType"="WOEUnit" 
            }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                half3 shOrVertexLight : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldNormal = worldNormal;

                // Ambient Spherical Harmonics + 4-Vertex Light Accumulation
                #ifdef VERTEXLIGHT_ON
                    o.shOrVertexLight = Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, worldPos, worldNormal);
                    o.shOrVertexLight += ShadeSH9(half4(worldNormal, 1.0));
                #else
                    o.shOrVertexLight = ShadeSH9(half4(worldNormal, 1.0));
                #endif

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed3 baseColor = tex.rgb * _Color.rgb;

                fixed4 col;
                // Matches exact GLSL assembly: (baseColor * shOrVertexLight) + baseColor
                col.rgb = (baseColor * i.shOrVertexLight) + baseColor;
                col.a = tex.a; // Transparency driven by texture alpha channel

                return col;
            }
            ENDCG
        }

        // --- PASS 2: FORWARD ADD (Dynamic Pixel Lights) ---
        Pass {
            Name "FORWARD"
            Tags { 
                "LightMode"="ForwardAdd" 
                "Queue"="Transparent" 
                "RenderType"="WOEUnit" 
            }
            ZWrite Off
            Blend SrcAlpha One
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                fixed4 col;
                col.rgb = tex.rgb * _Color.rgb;
                col.a = tex.a;
                
                return col;
            }
            ENDCG
        }
    }
}