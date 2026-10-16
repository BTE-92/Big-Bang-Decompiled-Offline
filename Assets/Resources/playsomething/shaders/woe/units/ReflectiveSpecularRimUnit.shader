Shader "WOE/Units/ReflectiveSpecularRimUnit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range(0.03,1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" { }
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
        _Cube ("Cubemap", CUBE) = "" { }
    }
    SubShader { 
        LOD 400
        Tags { "RenderType"="WOEUnit" }

        // --- PASS 1: FORWARD BASE (Main Directional Light, SH Ambient, Shadows, & Rim) ---
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="WOEUnit" }

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
                float3 worldRefl : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                half3 shOrVertexLight : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            samplerCUBE _Cube;
            fixed4 _Color;
            fixed3 _RimColor;
            float _RimPower;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = worldPos;
                o.worldNormal = worldNormal;

                // Exact reflection vector math: reflect(worldPos - CameraPos, worldNormal)
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                o.worldRefl = reflect(viewDir, worldNormal);

                // Spherical Harmonics ambient + 4-Vertex Light accumulation
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
                fixed4 cube = texCUBE(_Cube, i.worldRefl);

                // Combine albedo texture with cubemap reflection (tmpvar_11 in GLSL)
                half3 basePlusReflection = (tex.rgb * _Color.rgb) + cube.rgb;

                // Rim light: pow(1.0 - saturate(dot(viewDir, worldNormal)), _RimPower) * _RimColor
                half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                half rim = pow(1.0 - saturate(dot(viewDir, i.worldNormal)), _RimPower);
                half3 rimColor = _RimColor * rim;

                // Direct Light Diffuse
                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                half diff = max(0.0, dot(i.worldNormal, lightDir));

                // Shadow & Light Attenuation
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                half3 lightColor = _LightColor0.rgb * atten;

                fixed4 col;
                // Final color assembly: (BaseRefl * Light * Diff) + (BaseRefl * Ambient) + Rim
                col.rgb = (basePlusReflection * lightColor * diff) + (basePlusReflection * i.shOrVertexLight) + rimColor;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }

        // --- PASS 2: FORWARD ADD (Dynamic Point / Spot Lights) ---
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "RenderType"="WOEUnit" }
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
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldRefl : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                LIGHTING_COORDS(4, 5)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            samplerCUBE _Cube;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = worldPos;
                o.worldNormal = worldNormal;

                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                o.worldRefl = reflect(viewDir, worldNormal);

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 cube = texCUBE(_Cube, i.worldRefl);

                half3 basePlusReflection = (tex.rgb * _Color.rgb) + cube.rgb;

                #if defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                #endif

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                half3 lightColor = _LightColor0.rgb * atten;

                half diff = max(0.0, dot(i.worldNormal, lightDir));

                fixed4 col;
                col.rgb = (basePlusReflection * lightColor) * diff;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }

        // --- PASS 3: PREPASS BASE (Writes Normals to RGB and _Shininess to Alpha) ---
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassBase" "RenderType"="WOEUnit" }

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

            float _Shininess;

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
                res.w = _Shininess;
                return res;
            }
            ENDCG
        }

        // --- PASS 4: PREPASS FINAL (Legacy Deferred Final Color Composition) ---
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassFinal" "RenderType"="WOEUnit" }
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldRefl : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldViewDir : TEXCOORD4;
                float4 projPos : TEXCOORD5;
                half3 sh : TEXCOORD7;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            samplerCUBE _Cube;
            fixed4 _Color;
            fixed3 _RimColor;
            float _RimPower;
            sampler2D _LightBuffer;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldNormal = worldNormal;
                o.worldViewDir = _WorldSpaceCameraPos - worldPos;

                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                o.worldRefl = reflect(viewDir, worldNormal);

                o.projPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 light = tex2Dproj(_LightBuffer, i.projPos);

                #ifndef UNITY_HDR_ON
                    light.rgb = -log2(max(light.rgb, 0.001));
                #else
                    light.rgb = max(light.rgb, 0.001);
                #endif

                light.rgb += i.sh;

                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 cube = texCUBE(_Cube, i.worldRefl);

                half3 basePlusReflection = (tex.rgb * _Color.rgb) + cube.rgb;

                half3 viewDir = normalize(i.worldViewDir);
                half rim = pow(1.0 - saturate(dot(viewDir, i.worldNormal)), _RimPower);
                half3 rimColor = _RimColor * rim;

                fixed4 col;
                col.rgb = (basePlusReflection * light.rgb) + rimColor;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }
}