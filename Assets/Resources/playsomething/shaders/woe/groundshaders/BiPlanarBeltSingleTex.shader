Shader "WOE/Ground/BiPlanarBeltSingleTex" {
    Properties {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _Tiling ("Tiling", Float) = 1
        _TexBase1 ("Texture1 (Base)", 2D) = "white" {}
    }
    SubShader {
        LOD 400
        Tags { "RenderType"="Opaque" }

        // Helper function for biplanar projection logic
        CGINCLUDE
        #include "UnityCG.cginc"

        uniform fixed4 _Color;
        uniform float _Tiling;
        uniform sampler2D _TexBase1;

        inline fixed3 SampleBiplanarAlbedo(float3 objPos, float3 objNormal) {
            float scale = _Tiling / 10.0;
            float2 tex0 = float2(scale * objPos.x, -scale * objPos.z);
            float2 tex1 = float2(scale * objPos.y, -scale * objPos.z);

            fixed4 color0 = tex2D(_TexBase1, tex0);
            fixed4 color1 = tex2D(_TexBase1, tex1);

            // Blending factor based on object-space normal X-axis alignment
            float normX = abs(normalize(objNormal).x) * 1.5;
            float blendFactor = saturate(pow(normX, 50.0));

            fixed4 blendedTex = lerp(color0, color1, blendFactor);
            return blendedTex.rgb * _Color.rgb;
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
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 objPos : TEXCOORD2;
                float3 objNormal : TEXCOORD3;
                float3 ambientOrVertexLight : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.objPos = v.vertex.xyz;
                o.objNormal = v.normal;

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

                fixed3 albedo = SampleBiplanarAlbedo(i.objPos, i.objNormal);

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
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 objPos : TEXCOORD2;
                float3 objNormal : TEXCOORD3;
                LIGHTING_COORDS(4, 5)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.objPos = v.vertex.xyz;
                o.objNormal = v.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed3 albedo = SampleBiplanarAlbedo(i.objPos, i.objNormal);

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
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD1;
                float3 objNormal : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 sh : TEXCOORD5;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.objPos = v.vertex.xyz;
                o.objNormal = v.normal;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.screenPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed3 albedo = SampleBiplanarAlbedo(i.objPos, i.objNormal);

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
    Fallback "Diffuse"
}