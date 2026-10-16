Shader "WOE/Units/Emissive" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _EmissiveColor ("Emissive Color", Color) = (0,0,0,1)
        _Emission ("Emission", Range(0,1)) = 0
    }
    SubShader {
        Tags { "RenderType"="WOEUnit" }

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="WOEUnit" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed4 _EmissiveColor;
            uniform float _Emission;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 ambientOrVertexLight : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                #ifdef VERTEXLIGHT_ON
                    o.ambientOrVertexLight = Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, o.worldPos, o.worldNormal
                    );
                #else
                    o.ambientOrVertexLight = float3(0.0, 0.0, 0.0);
                #endif

                o.ambientOrVertexLight += ShadeSH9(half4(o.worldNormal, 1.0));
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                fixed4 mainTex = tex2D(_MainTex, i.uv) * _Color;
                float3 albedo = mainTex.rgb;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 diffuse = albedo * _LightColor0.rgb * atten * NdotL;

                float3 ambient = albedo * i.ambientOrVertexLight;
                float3 emission = _Emission * _EmissiveColor.rgb;

                fixed4 finalColor;
                finalColor.rgb = diffuse + ambient + emission;
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDCG
        }

        // ============================================================
        // FORWARD ADD PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="WOEUnit" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                fixed4 mainTex = tex2D(_MainTex, i.uv) * _Color;

                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 diffuse = mainTex.rgb * _LightColor0.rgb * atten * NdotL;

                return fixed4(diffuse, 1.0);
            }
            ENDCG
        }

        // ============================================================
        // DEFERRED PREPASS BASE PASS
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LIGHTMODE"="PrePassBase" "RenderType"="WOEUnit" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                return fixed4(worldNormal * 0.5 + 0.5, 0.0);
            }
            ENDCG
        }

        // ============================================================
        // DEFERRED PREPASS FINAL PASS
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LIGHTMODE"="PrePassFinal" "RenderType"="WOEUnit" }
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_HDR_ON
            #include "UnityCG.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float _Emission;
            uniform fixed4 _EmissiveColor;
            uniform sampler2D _LightBuffer;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float3 sh : TEXCOORD4;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.screenPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 mainTex = tex2D(_MainTex, i.uv) * _Color;
                float3 emission = _Emission * _EmissiveColor.rgb;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));
                #ifndef UNITY_HDR_ON
                    light = -log2(max(light, 0.001));
                #else
                    light = max(light, 0.001);
                #endif

                light.rgb += i.sh;

                fixed4 finalColor;
                finalColor.rgb = (mainTex.rgb * light.rgb) + emission;
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDCG
        }
    }
}