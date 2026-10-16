Shader "SpaceUnity/Planet Rings" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    SubShader {
        LOD 200
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha

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

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 diffuse = mainTex.rgb * _LightColor0.rgb * atten * NdotL;
                float3 ambient = mainTex.rgb * i.ambientOrVertexLight;

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
            Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
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
            Tags { "LIGHTMODE"="PrePassBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha

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
            Tags { "LIGHTMODE"="PrePassFinal" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_HDR_ON
            #include "UnityCG.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
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

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));
                #ifndef UNITY_HDR_ON
                    light = -log2(max(light, 0.001));
                #else
                    light = max(light, 0.001);
                #endif

                light.rgb += i.sh;

                fixed4 finalColor;
                finalColor.rgb = mainTex.rgb * light.rgb;
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDCG
        }

        // ============================================================
        // SHADOW CASTER PASS
        // ============================================================
        Pass {
            Name "SHADOWCASTER"
            Tags { "LIGHTMODE"="SHADOWCASTER" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            v2f vert (appdata_base v) {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    Fallback "Transparent/VertexLit"
}