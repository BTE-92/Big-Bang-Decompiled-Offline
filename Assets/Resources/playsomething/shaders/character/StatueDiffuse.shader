Shader "Character/StatueDiffuse" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Diffuse (RGB)", 2D) = "white" {}
        _GrungeTex ("Grunge (RGB)", 2D) = "white" {}
        _BaseTex ("Base (RGB)", 2D) = "white" {}
        _Scale ("Scale", Float) = 1
        _Tighten ("Tighten", Range(0.1,0.45)) = 0.3
        _DiffuseAmount ("Diffuse Amount", Range(0,1)) = 0.2
        _GrungeAmount ("Grunge Amount", Range(0.01,2)) = 1
    }
    SubShader {
        LOD 200
        Tags { "RenderType"="Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"

        uniform fixed4 _Color;
        uniform sampler2D _MainTex;
        uniform float4 _MainTex_ST;
        uniform sampler2D _GrungeTex;
        uniform sampler2D _BaseTex;
        uniform float _Scale;
        uniform float _Tighten;
        uniform float _DiffuseAmount;
        uniform float _GrungeAmount;

        // Triplanar blend with MainTex lerp
        inline float3 SampleStatueAlbedo(float2 uv, float3 worldPos, float3 worldNormal) {
            float3 norm = abs(worldNormal);
            float3 weights = max(norm - _Tighten, 0.0);
            weights /= (weights.x + weights.y + weights.z);

            float2 uv_xz = worldPos.xz / _Scale;
            float2 uv_xy = worldPos.xy / _Scale;
            float2 uv_zy = worldPos.zy / _Scale;

            float4 d = tex2D(_MainTex, uv);
            float4 grunge = tex2D(_GrungeTex, uv_xz);
            float4 base_xz = tex2D(_BaseTex, uv_xz);
            float4 base_xy = tex2D(_BaseTex, uv_xy); // Z-axis projection
            float4 base_zy = tex2D(_BaseTex, uv_zy); // X-axis projection

            float4 yProj = lerp(base_xz, grunge, _GrungeAmount);
            float4 triplanar = base_zy * weights.x + yProj * weights.y + base_xy * weights.z;

            return lerp(triplanar, d, _DiffuseAmount).rgb * _Color.rgb;
        }
        ENDCG

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

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

                fixed3 albedo = SampleStatueAlbedo(i.uv, i.worldPos, worldNormal);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 diffuse = albedo * _LightColor0.rgb * atten * NdotL;
                float3 ambient = albedo * i.ambientOrVertexLight;

                fixed4 finalColor;
                finalColor.rgb = diffuse + ambient;
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
            Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

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

                fixed3 albedo = SampleStatueAlbedo(i.uv, i.worldPos, worldNormal);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 diffuse = albedo * _LightColor0.rgb * atten * NdotL;

                return fixed4(diffuse, 1.0);
            }
            ENDCG
        }

        // ============================================================
        // DEFERRED PREPASS BASE PASS
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LIGHTMODE"="PrePassBase" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            Tags { "LIGHTMODE"="PrePassFinal" "RenderType"="Opaque" }
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_HDR_ON

            uniform sampler2D _LightBuffer;

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
                float4 screenPos : TEXCOORD3;
                float3 sh : TEXCOORD5;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                o.screenPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(o.worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                fixed3 albedo = SampleStatueAlbedo(i.uv, i.worldPos, worldNormal);

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));
                #ifndef UNITY_HDR_ON
                    light = -log2(max(light, 0.001));
                #else
                    light = max(light, 0.001);
                #endif

                light.rgb += i.sh;

                fixed4 finalColor;
                finalColor.rgb = albedo * light.rgb;
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}