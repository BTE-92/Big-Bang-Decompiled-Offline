Shader "SpaceUnity/PlanetSelfIllum" {
    Properties {
        _MainTex ("_MainTex", 2D) = "black" {}
        _Normals ("_Normals", 2D) = "black" {}
        _Illum ("Illumin (A)", 2D) = "white" {}
        _Lights ("_Lights", 2D) = "black" {}
        _LightScale ("_LightScale", Float) = 1
        _AtmosNear ("_AtmosNear", Color) = (0.168628,0.737255,1,1)
        _AtmosFar ("_AtmosFar", Color) = (0.455781,0.518704,0.985075,1)
        _AtmosFalloff ("_AtmosFalloff", Float) = 3
    }
    SubShader {
        Tags { "QUEUE"="Geometry" "IGNOREPROJECTOR"="False" "RenderType"="Opaque" }

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "IGNOREPROJECTOR"="False" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _Normals;
            uniform float4 _Normals_ST;
            uniform sampler2D _Illum;
            uniform float4 _Illum_ST;
            uniform sampler2D _Lights;
            uniform float4 _Lights_ST;
            uniform float _LightScale;
            uniform float4 _AtmosNear;
            uniform float4 _AtmosFar;
            uniform float _AtmosFalloff;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uvMainNorm : TEXCOORD0;     // xy: MainTex, zw: Normals
                float4 uvIllumLights : TEXCOORD1;  // xy: Illum, zw: Lights
                float4 tangentWorldPos : TEXCOORD2;
                float4 binormalWorldPos : TEXCOORD3;
                float4 normalWorldPos : TEXCOORD4;
                float3 ambientOrVertexLight : TEXCOORD5;
                SHADOW_COORDS(6)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMainNorm.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvMainNorm.zw = TRANSFORM_TEX(v.texcoord, _Normals);
                o.uvIllumLights.xy = TRANSFORM_TEX(v.texcoord, _Illum);
                o.uvIllumLights.zw = TRANSFORM_TEX(v.texcoord, _Lights);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

                o.tangentWorldPos = float4(worldTangent, worldPos.x);
                o.binormalWorldPos = float4(worldBinormal, worldPos.y);
                o.normalWorldPos = float4(worldNormal, worldPos.z);

                #ifdef VERTEXLIGHT_ON
                    o.ambientOrVertexLight = Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, worldPos, worldNormal
                    );
                #else
                    o.ambientOrVertexLight = float3(0.0, 0.0, 0.0);
                #endif

                o.ambientOrVertexLight += ShadeSH9(half4(worldNormal, 1.0));
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldPos = float3(i.tangentWorldPos.w, i.binormalWorldPos.w, i.normalWorldPos.w);
                float3 worldTangent = i.tangentWorldPos.xyz;
                float3 worldBinormal = i.binormalWorldPos.xyz;
                float3 worldNormal = i.normalWorldPos.xyz;

                float3 worldViewDir = normalize(_WorldSpaceCameraPos - worldPos);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                // Transform ViewDir to Tangent Space
                float3 tangentViewDir = float3(
                    dot(worldTangent, worldViewDir),
                    dot(worldBinormal, worldViewDir),
                    dot(worldNormal, worldViewDir)
                );

                // Atmosphere calculation
                float atmosFactor = saturate(pow(saturate(1.0 - normalize(tangentViewDir).z), _AtmosFalloff));
                float4 mainTex = tex2D(_MainTex, i.uvMainNorm.xy);
                float3 surfaceColor = (lerp(_AtmosNear, _AtmosFar, atmosFactor) * atmosFactor).rgb + mainTex.rgb;

                // Tangent Space Normal Unpacking
                float4 packedNormal = tex2D(_Normals, i.uvMainNorm.zw);
                float3 tangentNormal = normalize((packedNormal.rgb * 2.0) - 1.0);

                // Reconstruct World Normal Map
                float3 worldNormalMap = normalize(
                    worldTangent * tangentNormal.x +
                    worldBinormal * tangentNormal.y +
                    worldNormal * tangentNormal.z
                );

                // Self-Illumination and Night Lights
                float3 illumVal = mainTex.rgb * tex2D(_Illum, i.uvIllumLights.xy).a;
                float nightLights = tex2D(_Lights, i.uvIllumLights.zw).r * _LightScale;

                UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

                float NdotL = max(0.0, dot(lightDir, worldNormalMap));
                float3 diffLight = _LightColor0.rgb * NdotL * (atten * 2.0);

                float darknessFactor = 1.0 - saturate(16.0 * NdotL);

                float3 litColor = surfaceColor * diffLight;
                litColor.y -= 0.01 * darknessFactor;
                litColor.x -= 0.03 * darknessFactor;
                litColor.xy += min(nightLights, darknessFactor);
                litColor.z += 0.75 * min(nightLights, darknessFactor);
                litColor.z = saturate(litColor.z + darknessFactor * 0.02);

                fixed4 finalColor;
                finalColor.rgb = (surfaceColor * i.ambientOrVertexLight) + litColor + illumVal;
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
            Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Geometry" "IGNOREPROJECTOR"="False" "RenderType"="Opaque" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _Normals;
            uniform float4 _Normals_ST;
            uniform sampler2D _Illum;
            uniform float4 _Illum_ST;
            uniform sampler2D _Lights;
            uniform float4 _Lights_ST;
            uniform float _LightScale;
            uniform float4 _AtmosNear;
            uniform float4 _AtmosFar;
            uniform float _AtmosFalloff;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uvMainNorm : TEXCOORD0;
                float4 uvIllumLights : TEXCOORD1;
                float3 worldTangent : TEXCOORD2;
                float3 worldBinormal : TEXCOORD3;
                float3 worldNormal : TEXCOORD4;
                float3 worldPos : TEXCOORD5;
                LIGHTING_COORDS(6, 7)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMainNorm.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvMainNorm.zw = TRANSFORM_TEX(v.texcoord, _Normals);
                o.uvIllumLights.xy = TRANSFORM_TEX(v.texcoord, _Illum);
                o.uvIllumLights.zw = TRANSFORM_TEX(v.texcoord, _Lights);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * tangentSign;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                float3 tangentViewDir = float3(
                    dot(i.worldTangent, worldViewDir),
                    dot(i.worldBinormal, worldViewDir),
                    dot(i.worldNormal, worldViewDir)
                );

                float atmosFactor = saturate(pow(saturate(1.0 - normalize(tangentViewDir).z), _AtmosFalloff));
                float4 mainTex = tex2D(_MainTex, i.uvMainNorm.xy);
                float3 surfaceColor = (lerp(_AtmosNear, _AtmosFar, atmosFactor) * atmosFactor).rgb + mainTex.rgb;

                float4 packedNormal = tex2D(_Normals, i.uvMainNorm.zw);
                float3 tangentNormal = normalize((packedNormal.rgb * 2.0) - 1.0);

                float3 worldNormalMap = normalize(
                    i.worldTangent * tangentNormal.x +
                    i.worldBinormal * tangentNormal.y +
                    i.worldNormal * tangentNormal.z
                );

                float3 illumVal = mainTex.rgb * tex2D(_Illum, i.uvIllumLights.xy).a;
                float nightLights = tex2D(_Lights, i.uvIllumLights.zw).r * _LightScale;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                float NdotL = max(0.0, dot(lightDir, worldNormalMap));
                float3 diffLight = _LightColor0.rgb * NdotL * (atten * 2.0);

                float darknessFactor = 1.0 - saturate(16.0 * NdotL);

                float3 litColor = surfaceColor * diffLight;
                litColor.y -= 0.01 * darknessFactor;
                litColor.x -= 0.03 * darknessFactor;
                litColor.xy += min(nightLights, darknessFactor);
                litColor.z += 0.75 * min(nightLights, darknessFactor);
                litColor.z = saturate(litColor.z + darknessFactor * 0.02);

                return fixed4(litColor + illumVal, 1.0);
            }
            ENDCG
        }

        // ============================================================
        // DEFERRED PREPASS BASE PASS
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LIGHTMODE"="PrePassBase" "QUEUE"="Geometry" "IGNOREPROJECTOR"="False" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _Normals;
            uniform float4 _Normals_ST;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvNormals : TEXCOORD0;
                float3 worldTangent : TEXCOORD1;
                float3 worldBinormal : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvNormals = TRANSFORM_TEX(v.texcoord, _Normals);

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * tangentSign;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 packedNormal = tex2D(_Normals, i.uvNormals);
                float3 tangentNormal = normalize((packedNormal.rgb * 2.0) - 1.0);

                float3 worldNormalMap = normalize(
                    i.worldTangent * tangentNormal.x +
                    i.worldBinormal * tangentNormal.y +
                    i.worldNormal * tangentNormal.z
                );

                return fixed4(worldNormalMap * 0.5 + 0.5, 0.0);
            }
            ENDCG
        }

        // ============================================================
        // DEFERRED PREPASS FINAL PASS
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LIGHTMODE"="PrePassFinal" "QUEUE"="Geometry" "IGNOREPROJECTOR"="False" "RenderType"="Opaque" }
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_HDR_ON
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _Illum;
            uniform float4 _Illum_ST;
            uniform sampler2D _Lights;
            uniform float4 _Lights_ST;
            uniform float _LightScale;
            uniform float4 _AtmosNear;
            uniform float4 _AtmosFar;
            uniform float _AtmosFalloff;
            uniform sampler2D _LightBuffer;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uvMainIllum : TEXCOORD0;  // xy: MainTex, zw: Illum
                float2 uvLights : TEXCOORD1;
                float3 tangentViewDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 sh : TEXCOORD5;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMainIllum.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvMainIllum.zw = TRANSFORM_TEX(v.texcoord, _Illum);
                o.uvLights = TRANSFORM_TEX(v.texcoord, _Lights);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

                float3 worldViewDir = _WorldSpaceCameraPos - worldPos;
                o.tangentViewDir = float3(
                    dot(worldViewDir, worldTangent),
                    dot(worldViewDir, worldBinormal),
                    dot(worldViewDir, worldNormal)
                );

                o.screenPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 tangentViewDir = normalize(i.tangentViewDir);

                float atmosFactor = saturate(pow(saturate(1.0 - tangentViewDir.z), _AtmosFalloff));
                float4 mainTex = tex2D(_MainTex, i.uvMainIllum.xy);
                float3 surfaceColor = (lerp(_AtmosNear, _AtmosFar, atmosFactor) * atmosFactor).rgb + mainTex.rgb;

                float3 illumVal = mainTex.rgb * tex2D(_Illum, i.uvMainIllum.zw).a;
                float nightLights = tex2D(_Lights, i.uvLights).r * _LightScale;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));
                #ifndef UNITY_HDR_ON
                    light = -log2(max(light, 0.001));
                #else
                    light = max(light, 0.001);
                #endif

                light.rgb += i.sh;

                float3 litColor = surfaceColor * light.rgb;
                litColor.y -= 0.01;
                litColor.x -= 0.03;
                litColor.xy += min(nightLights, 1.0);
                litColor.z += 0.75 * min(nightLights, 1.0);
                litColor.z = saturate(litColor.z + 0.02);

                fixed4 finalColor;
                finalColor.rgb = litColor + illumVal;
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}