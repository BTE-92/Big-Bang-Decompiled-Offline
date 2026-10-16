Shader "WOE/Ground/BiPlanarBeltDoubleTex" {
    Properties {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _Emission ("Emissive Color", Color) = (0,0,0,0)
        _Tiling ("Tiling", Float) = 1
        _TexBase1 ("Texture1 (Base)", 2D) = "white" { }
        _TexBase2 ("Texture2 (Base)", 2D) = "white" { }
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }

        // --- PASS 1: FORWARD BASE (Main Directional Light, SH Ambient, Shadows, & BiPlanar Material) ---
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }

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
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                half3 sh : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            sampler2D _TexBase1;
            sampler2D _TexBase2;
            float _Tiling;
            fixed4 _Color;
            fixed4 _Emission;

            // Shared BiPlanar helper function matching GLSL fragment logic
            void CalculateBiPlanarMaterial(float3 worldPos, float3 worldNormal, out half3 albedo, out half3 emission) {
                // Blend factor calculation: saturate(pow(abs(normalize(worldNormal.xy).x), 6.0))
                half blend = saturate(pow(abs(normalize(worldNormal.xy).x), 6.0));

                // World-space Z offset from original shader math
                float3 pos = worldPos;
                pos.z -= 215.0;

                float2 uv_xz = _Tiling * pos.xz;
                float2 uv_yz = _Tiling * pos.yz;

                half3 ca, cb;
                if (worldNormal.y >= 0.0) {
                    ca = tex2D(_TexBase1, uv_xz).rgb;
                    cb = tex2D(_TexBase2, uv_yz).rgb;
                } else {
                    ca = tex2D(_TexBase2, uv_xz).rgb;
                    cb = tex2D(_TexBase2, uv_yz).rgb;
                }

                half3 texColor = lerp(ca, cb, blend);
                albedo = texColor * _Color.rgb;
                emission = texColor * _Emission.rgb;
            }

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = worldPos;
                o.worldNormal = worldNormal;

                #ifdef VERTEXLIGHT_ON
                    o.sh = Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, worldPos, worldNormal);
                    o.sh += ShadeSH9(half4(worldNormal, 1.0));
                #else
                    o.sh = ShadeSH9(half4(worldNormal, 1.0));
                #endif

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 albedo, emission;
                CalculateBiPlanarMaterial(i.worldPos, i.worldNormal, albedo, emission);

                half3 worldNormal = normalize(i.worldNormal);
                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                half diff = max(0.0, dot(worldNormal, lightDir));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                half3 lightColor = _LightColor0.rgb * atten;

                fixed4 col;
                col.rgb = (albedo * lightColor * diff) + (albedo * i.sh) + emission;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }

        // --- PASS 2: FORWARD ADD (Additional Dynamic Lights) ---
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "RenderType"="Opaque" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                LIGHTING_COORDS(2, 3)
            };

            sampler2D _TexBase1;
            sampler2D _TexBase2;
            float _Tiling;
            fixed4 _Color;
            fixed4 _Emission;

            void CalculateBiPlanarMaterial(float3 worldPos, float3 worldNormal, out half3 albedo, out half3 emission) {
                half blend = saturate(pow(abs(normalize(worldNormal.xy).x), 6.0));

                float3 pos = worldPos;
                pos.z -= 215.0;

                float2 uv_xz = _Tiling * pos.xz;
                float2 uv_yz = _Tiling * pos.yz;

                half3 ca, cb;
                if (worldNormal.y >= 0.0) {
                    ca = tex2D(_TexBase1, uv_xz).rgb;
                    cb = tex2D(_TexBase2, uv_yz).rgb;
                } else {
                    ca = tex2D(_TexBase2, uv_xz).rgb;
                    cb = tex2D(_TexBase2, uv_yz).rgb;
                }

                half3 texColor = lerp(ca, cb, blend);
                albedo = texColor * _Color.rgb;
                emission = texColor * _Emission.rgb;
            }

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = worldPos;
                o.worldNormal = worldNormal;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 albedo, emission;
                CalculateBiPlanarMaterial(i.worldPos, i.worldNormal, albedo, emission);

                half3 worldNormal = normalize(i.worldNormal);

                #if defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                #endif

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                half3 lightColor = _LightColor0.rgb * atten;

                half diff = max(0.0, dot(worldNormal, lightDir));

                fixed4 col;
                col.rgb = (albedo * lightColor) * diff;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }

        // --- PASS 3: PREPASS BASE (G-Buffer Normal Target) ---
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassBase" "RenderType"="Opaque" }

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
                half3 normal = normalize(i.worldNormal);
                fixed4 res;
                res.xyz = (normal * 0.5) + 0.5;
                res.w = 0.0;
                return res;
            }
            ENDCG
        }

        // --- PASS 4: PREPASS FINAL (Deferred Final Lighting Composition) ---
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassFinal" "RenderType"="Opaque" }
            ZWrite Off

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
                float3 worldPos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                half3 sh : TEXCOORD4;
            };

            sampler2D _TexBase1;
            sampler2D _TexBase2;
            float _Tiling;
            fixed4 _Color;
            fixed4 _Emission;
            sampler2D _LightBuffer;

            void CalculateBiPlanarMaterial(float3 worldPos, float3 worldNormal, out half3 albedo, out half3 emission) {
                half blend = saturate(pow(abs(normalize(worldNormal.xy).x), 6.0));

                float3 pos = worldPos;
                pos.z -= 215.0;

                float2 uv_xz = _Tiling * pos.xz;
                float2 uv_yz = _Tiling * pos.yz;

                half3 ca, cb;
                if (worldNormal.y >= 0.0) {
                    ca = tex2D(_TexBase1, uv_xz).rgb;
                    cb = tex2D(_TexBase2, uv_yz).rgb;
                } else {
                    ca = tex2D(_TexBase2, uv_xz).rgb;
                    cb = tex2D(_TexBase2, uv_yz).rgb;
                }

                half3 texColor = lerp(ca, cb, blend);
                albedo = texColor * _Color.rgb;
                emission = texColor * _Emission.rgb;
            }

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = worldPos;
                o.worldNormal = worldNormal;
                o.screenPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 albedo, emission;
                CalculateBiPlanarMaterial(i.worldPos, i.worldNormal, albedo, emission);

                half4 light = tex2Dproj(_LightBuffer, i.screenPos);

                #ifndef UNITY_HDR_ON
                    light.rgb = -log2(max(light.rgb, 0.001));
                #else
                    light.rgb = max(light.rgb, 0.001);
                #endif

                light.rgb += i.sh;

                fixed4 col;
                col.rgb = (albedo * light.rgb) + emission;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }
}