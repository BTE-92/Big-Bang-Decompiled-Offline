Shader "Character/StatueBmp" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Diffuse (RGB)", 2D) = "white" {}
        _GrungeTex ("Grunge (RGB)", 2D) = "white" {}
        _BumpMap ("Bump (RGB) Illumin (A)", 2D) = "bump" {}
        _BaseTex ("Base (RGB)", 2D) = "white" {}
        _Scale ("Scale", Float) = 1
        _Tighten ("Tighten", Range(0.1,0.45)) = 0.3
        _DiffuseAmount ("Diffuse Amount", Range(0,1)) = 0.2
        _GrungeAmount ("Grunge Amount", Range(0,2)) = 1
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
        uniform sampler2D _BumpMap;
        uniform float4 _BumpMap_ST;
        uniform sampler2D _BaseTex;
        uniform float _Scale;
        uniform float _Tighten;
        uniform float _DiffuseAmount;
        uniform float _GrungeAmount;

        // Triplanar blending + Normal map reconstruction
        inline void CalculateStatueBmp(
            float2 uvMain, float2 uvBump, float3 worldPos, 
            float3 worldTangent, float3 worldBinormal, float3 worldNormal,
            out float3 outAlbedo, out float3 outBumpedWorldNormal) 
        {
            // Unpack normal map in tangent space
            float4 packedNormal = tex2D(_BumpMap, uvBump);
            float3 tangentNormal = UnpackNormal(packedNormal);

            // Reconstruct bumped world normal
            outBumpedWorldNormal = normalize(
                worldTangent * tangentNormal.x +
                worldBinormal * tangentNormal.y +
                worldNormal * tangentNormal.z
            );

            // Triplanar blend weights calculated from the bumped world normal
            float3 norm = abs(outBumpedWorldNormal);
            float3 weights = max(norm - _Tighten, 0.0);
            weights /= max(weights.x + weights.y + weights.z, 0.00001);

            float2 uv_xz = worldPos.xz / _Scale;
            float2 uv_xy = worldPos.xy / _Scale;
            float2 uv_zy = worldPos.zy / _Scale;

            float4 d = tex2D(_MainTex, uvMain);
            float4 grunge = tex2D(_GrungeTex, uv_xz);
            float4 base_xz = tex2D(_BaseTex, uv_xz);
            float4 base_xy = tex2D(_BaseTex, uv_xy); // Z-axis projection
            float4 base_zy = tex2D(_BaseTex, uv_zy); // X-axis projection

            float4 yProj = lerp(base_xz, grunge, _GrungeAmount);
            float4 triplanar = base_zy * weights.x + yProj * weights.y + base_xy * weights.z;

            outAlbedo = lerp(triplanar, d, _DiffuseAmount).rgb * _Color.rgb;
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
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uvMainBump : TEXCOORD0;       // xy: MainTex, zw: BumpMap
                float4 tangentWorldPos : TEXCOORD1;  // xyz: Tangent, w: pos.x
                float4 binormalWorldPos : TEXCOORD2; // xyz: Binormal, w: pos.y
                float4 normalWorldPos : TEXCOORD3;   // xyz: Normal, w: pos.z
                float3 ambientOrVertexLight : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMainBump.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvMainBump.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

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

                float3 albedo, bumpedWorldNormal;
                CalculateStatueBmp(i.uvMainBump.xy, i.uvMainBump.zw, worldPos,
                                   worldTangent, worldBinormal, worldNormal,
                                   albedo, bumpedWorldNormal);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

                float NdotL = max(0.0, dot(bumpedWorldNormal, lightDir));
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
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uvMainBump : TEXCOORD0;
                float3 worldTangent : TEXCOORD1;
                float3 worldBinormal : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
                LIGHTING_COORDS(5, 6)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMainBump.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvMainBump.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * tangentSign;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                float3 albedo, bumpedWorldNormal;
                CalculateStatueBmp(i.uvMainBump.xy, i.uvMainBump.zw, i.worldPos,
                                   i.worldTangent, i.worldBinormal, i.worldNormal,
                                   albedo, bumpedWorldNormal);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                float NdotL = max(0.0, dot(bumpedWorldNormal, lightDir));
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
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvBump : TEXCOORD0;
                float3 worldTangent : TEXCOORD1;
                float3 worldBinormal : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvBump = TRANSFORM_TEX(v.texcoord, _BumpMap);

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * tangentSign;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 packedNormal = tex2D(_BumpMap, i.uvBump);
                float3 tangentNormal = UnpackNormal(packedNormal);

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
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uvMainBump : TEXCOORD0;
                float4 tangentWorldPos : TEXCOORD1;
                float4 binormalWorldPos : TEXCOORD2;
                float4 normalWorldPos : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                float3 sh : TEXCOORD6;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMainBump.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvMainBump.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

                o.tangentWorldPos = float4(worldTangent, worldPos.x);
                o.binormalWorldPos = float4(worldBinormal, worldPos.y);
                o.normalWorldPos = float4(worldNormal, worldPos.z);

                o.screenPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldPos = float3(i.tangentWorldPos.w, i.binormalWorldPos.w, i.normalWorldPos.w);
                float3 worldTangent = i.tangentWorldPos.xyz;
                float3 worldBinormal = i.binormalWorldPos.xyz;
                float3 worldNormal = i.normalWorldPos.xyz;

                float3 albedo, bumpedWorldNormal;
                CalculateStatueBmp(i.uvMainBump.xy, i.uvMainBump.zw, worldPos,
                                   worldTangent, worldBinormal, worldNormal,
                                   albedo, bumpedWorldNormal);

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