Shader "WOE/Fx/ReflectiveSpecularRim" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range(0.03,1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
        _Cube ("Cubemap", CUBE) = "black" {}
    }
    SubShader {
        LOD 400
        Tags { "RenderType"="Opaque" }

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
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform half _Shininess;
            uniform float4 _RimColor;
            uniform float _RimPower;
            uniform samplerCUBE _Cube;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 reflDir : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float3 ambientOrVertexLight : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                float3 worldViewDir = o.worldPos - _WorldSpaceCameraPos;
                o.reflDir = reflect(worldViewDir, o.worldNormal);

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
                float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed3 reflTex = texCUBE(_Cube, i.reflDir).rgb;
                float3 albedo = (mainTex.rgb * _Color.rgb) + reflTex;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                // Diffuse NdotL
                float NdotL = max(0.0, dot(worldNormal, lightDir));

                // Specular (Blinn-Phong)
                float3 halfDir = normalize(lightDir + worldViewDir);
                float NdotH = max(0.0, dot(worldNormal, halfDir));
                float spec = pow(NdotH, _Shininess * 128.0) * mainTex.a;

                float3 lightColor = _LightColor0.rgb * atten;
                float3 diffTerm = albedo * lightColor * NdotL;
                float3 specTerm = lightColor * _SpecColor.rgb * spec;

                // Ambient (SH / Vertex Lights)
                float3 ambientTerm = albedo * i.ambientOrVertexLight;

                // Rim Lighting
                float NdotV = saturate(dot(worldViewDir, worldNormal));
                float rimFactor = pow(1.0 - NdotV, _RimPower);
                float3 rimTerm = _RimColor.rgb * rimFactor;

                float4 finalColor;
                finalColor.rgb = diffTerm + specTerm + ambientTerm + rimTerm;
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
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform half _Shininess;
            uniform samplerCUBE _Cube;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 reflDir : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                LIGHTING_COORDS(4, 5)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                float3 worldViewDir = o.worldPos - _WorldSpaceCameraPos;
                o.reflDir = reflect(worldViewDir, o.worldNormal);

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed3 reflTex = texCUBE(_Cube, i.reflDir).rgb;
                float3 albedo = (mainTex.rgb * _Color.rgb) + reflTex;

                float NdotL = max(0.0, dot(worldNormal, lightDir));

                float3 halfDir = normalize(lightDir + worldViewDir);
                float NdotH = max(0.0, dot(worldNormal, halfDir));
                float spec = pow(NdotH, _Shininess * 128.0) * mainTex.a;

                float3 lightColor = _LightColor0.rgb * atten;
                float3 diffTerm = albedo * lightColor * NdotL;
                float3 specTerm = lightColor * _SpecColor.rgb * spec;

                return fixed4(diffTerm + specTerm, 1.0);
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
            #include "UnityCG.cginc"

            uniform half _Shininess;

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
                return fixed4(worldNormal * 0.5 + 0.5, _Shininess);
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
            #include "UnityCG.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float4 _RimColor;
            uniform float _RimPower;
            uniform samplerCUBE _Cube;
            uniform sampler2D _LightBuffer;
            uniform fixed4 _SpecColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 reflDir : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                float3 sh : TEXCOORD5;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                float3 worldViewDir = o.worldPos - _WorldSpaceCameraPos;
                o.reflDir = reflect(worldViewDir, o.worldNormal);

                o.screenPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(o.worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed3 reflTex = texCUBE(_Cube, i.reflDir).rgb;
                float3 albedo = (mainTex.rgb * _Color.rgb) + reflTex;

                float NdotV = saturate(dot(worldViewDir, worldNormal));
                float rimFactor = pow(1.0 - NdotV, _RimPower);
                float3 rimTerm = _RimColor.rgb * rimFactor;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));
                #ifndef UNITY_HDR_ON
                    light = -log2(max(light, 0.001));
                #else
                    light = max(light, 0.001);
                #endif

                light.rgb += i.sh;

                float spec = light.a * mainTex.a;
                float3 diffTerm = albedo * light.rgb;
                float3 specTerm = light.rgb * _SpecColor.rgb * spec;

                float4 finalColor;
                finalColor.rgb = diffTerm + specTerm + rimTerm;
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "Specular"
}