Shader "WOE/Ground/GroundFrontWithBandGame" {
    Properties {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (0,0,0,0)
        _EdgeTreshold ("Edge Treshold", Range(0.01,1)) = 0.35
        _MainTex ("Diffuse Base", 2D) = "white" { }
        _MaskTex ("EdgeMask", 2D) = "white" { }
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }

        // --- PASS 1: FORWARD BASE ---
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
                float2 uv0 : TEXCOORD0; // Maps to _MaskTex
                float2 uv1 : TEXCOORD1; // Maps to _MainTex
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv01 : TEXCOORD0; // xy = MaskTex UVs, zw = MainTex UVs
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                half3 sh : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            fixed4 _Color;
            fixed4 _EdgeColor;
            float _EdgeTreshold;

            // Helper function to evaluate the custom edge mask threshold logic
            void CalculateGroundMaterial(float4 uv01, out half3 albedo) {
                half iMask = 1.0 - tex2D(_MaskTex, uv01.xy).a;
                half3 mainColor = tex2D(_MainTex, uv01.zw).rgb;

                // Edge threshold & smooth interpolation
                half edgeFactor = max((iMask / _EdgeTreshold) - (_EdgeTreshold * 0.9), 0.0);
                half3 edgeMixed = lerp(mainColor, _EdgeColor.rgb, edgeFactor);

                half3 edgeResult;
                if (iMask > _EdgeTreshold) {
                    edgeResult = (iMask == 1.0) ? mainColor : _EdgeColor.rgb;
                } else {
                    edgeResult = edgeMixed;
                }

                albedo = edgeResult * _Color.rgb;
            }

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv01.xy = TRANSFORM_TEX(v.uv0, _MaskTex);
                o.uv01.zw = TRANSFORM_TEX(v.uv1, _MainTex);

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
                half3 albedo;
                CalculateGroundMaterial(i.uv01, albedo);

                half3 worldNormal = normalize(i.worldNormal);
                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                half diff = max(0.0, dot(worldNormal, lightDir));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                half3 lightColor = _LightColor0.rgb * atten;

                fixed4 col;
                col.rgb = (albedo * lightColor * diff) + (albedo * i.sh);
                col.a = 1.0;

                return col;
            }
            ENDCG
        }

        // --- PASS 2: FORWARD ADD ---
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
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv01 : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            fixed4 _Color;
            fixed4 _EdgeColor;
            float _EdgeTreshold;

            void CalculateGroundMaterial(float4 uv01, out half3 albedo) {
                half iMask = 1.0 - tex2D(_MaskTex, uv01.xy).a;
                half3 mainColor = tex2D(_MainTex, uv01.zw).rgb;

                half edgeFactor = max((iMask / _EdgeTreshold) - (_EdgeTreshold * 0.9), 0.0);
                half3 edgeMixed = lerp(mainColor, _EdgeColor.rgb, edgeFactor);

                half3 edgeResult;
                if (iMask > _EdgeTreshold) {
                    edgeResult = (iMask == 1.0) ? mainColor : _EdgeColor.rgb;
                } else {
                    edgeResult = edgeMixed;
                }

                albedo = edgeResult * _Color.rgb;
            }

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv01.xy = TRANSFORM_TEX(v.uv0, _MaskTex);
                o.uv01.zw = TRANSFORM_TEX(v.uv1, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = worldPos;
                o.worldNormal = worldNormal;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 albedo;
                CalculateGroundMaterial(i.uv01, albedo);

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

        // --- PASS 3: PREPASS BASE ---
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

        // --- PASS 4: PREPASS FINAL ---
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
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv01 : TEXCOORD0;
                float4 projPos : TEXCOORD2;
                half3 sh : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            sampler2D _LightBuffer;

            fixed4 _Color;
            fixed4 _EdgeColor;
            float _EdgeTreshold;

            void CalculateGroundMaterial(float4 uv01, out half3 albedo) {
                half iMask = 1.0 - tex2D(_MaskTex, uv01.xy).a;
                half3 mainColor = tex2D(_MainTex, uv01.zw).rgb;

                half edgeFactor = max((iMask / _EdgeTreshold) - (_EdgeTreshold * 0.9), 0.0);
                half3 edgeMixed = lerp(mainColor, _EdgeColor.rgb, edgeFactor);

                half3 edgeResult;
                if (iMask > _EdgeTreshold) {
                    edgeResult = (iMask == 1.0) ? mainColor : _EdgeColor.rgb;
                } else {
                    edgeResult = edgeMixed;
                }

                albedo = edgeResult * _Color.rgb;
            }

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv01.xy = TRANSFORM_TEX(v.uv0, _MaskTex);
                o.uv01.zw = TRANSFORM_TEX(v.uv1, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.projPos = ComputeScreenPos(o.pos);
                o.sh = ShadeSH9(half4(worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 albedo;
                CalculateGroundMaterial(i.uv01, albedo);

                half4 light = tex2Dproj(_LightBuffer, i.projPos);

                #ifndef UNITY_HDR_ON
                    light.rgb = -log2(max(light.rgb, 0.001));
                #else
                    light.rgb = max(light.rgb, 0.001);
                #endif

                light.rgb += i.sh;

                fixed4 col;
                col.rgb = albedo * light.rgb;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }
}