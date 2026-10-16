Shader "WOE/Editor/UnitMaskReplaceShader" {
    Properties {
        // No properties declared in the original compiled GLSL
    }
    SubShader { 
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="WOEUnit" 
        }

        // --- PASS 1: FORWARD BASE (Directional Light, Ambient SH, Shadows & Rim) ---
        Pass {
            Name "FORWARD"
            Tags { 
                "LightMode"="ForwardBase" 
                "Queue"="Transparent" 
                "RenderType"="WOEUnit" 
            }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

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

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                
                o.worldPos = worldPos;
                o.worldNormal = worldNormal;
                
                // Evaluates Ambient Spherical Harmonics (SH9)
                o.sh = ShadeSH9(half4(worldNormal, 1.0));
                
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 worldNormal = normalize(i.worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                // Rim light calculation: pow(1.0 - saturate(dot(view, normal)), 0.2)
                half rimFactor = pow(1.0 - saturate(dot(viewDir, worldNormal)), 0.2);
                half3 rimColor = half3(0.6, 0.8, 1.0) * rimFactor;
                
                // Light direction & diffuse attenuation
                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                half diff = max(0.0, dot(worldNormal, lightDir));
                
                // Shadow & Light attenuation
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                half3 lightColor = _LightColor0.rgb * atten;
                
                fixed4 col;
                // Matches the original logic exactly: RGB = light * diff + rim, A = rim.r
                col.rgb = (lightColor * diff) + rimColor;
                col.a = rimColor.r; 
                
                return col;
            }
            ENDCG
        }

        // --- PASS 2: FORWARD ADD (Pixel Lights) ---
        Pass {
            Name "FORWARD"
            Tags { 
                "LightMode"="ForwardAdd" 
                "Queue"="Transparent" 
                "RenderType"="WOEUnit" 
            }
            ZWrite Off
            Blend SrcAlpha One // Matches original ForwardAdd alpha blending exactly
            ColorMask RGB

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

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 worldNormal = normalize(i.worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                // Rim light calculation
                half rimFactor = pow(1.0 - saturate(dot(viewDir, worldNormal)), 0.2);
                half3 rimColor = half3(0.6, 0.8, 1.0) * rimFactor;
                
                #if defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                #endif
                
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                half3 lightColor = _LightColor0.rgb * atten;
                
                half diff = max(0.0, dot(worldNormal, lightDir));
                
                fixed4 col;
                // Note: ForwardAdd only outputs Diffuse light to RGB, keeping rim.r strictly to Alpha!
                col.rgb = lightColor * diff;
                col.a = rimColor.r;
                
                return col;
            }
            ENDCG
        }

        // --- PASS 3: PREPASS FINAL (Deferred Light Buffer Decoder) ---
        Pass {
            Name "PREPASS"
            Tags { 
                "LightMode"="PrePassFinal" 
                "Queue"="Transparent" 
                "RenderType"="WOEUnit" 
            }
            ZWrite Off
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
                float3 worldViewDir : TEXCOORD1;
                float4 projPos : TEXCOORD3;
            };

            sampler2D _LightBuffer;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldViewDir = _WorldSpaceCameraPos - worldPos;
                o.projPos = ComputeScreenPos(o.pos);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 worldNormal = normalize(i.worldNormal);
                half3 viewDir = normalize(i.worldViewDir);
                
                // Rim light calculation
                half rimFactor = pow(1.0 - saturate(dot(viewDir, worldNormal)), 0.2);
                half3 rimColor = half3(0.6, 0.8, 1.0) * rimFactor;
                
                // G-Buffer light buffer lookup
                half4 light = tex2Dproj(_LightBuffer, i.projPos);
                
                #ifndef UNITY_HDR_ON
                    light.rgb = -log2(max(light.rgb, 0.001));
                #else
                    light.rgb = max(light.rgb, 0.001);
                #endif
                
                fixed4 col;
                // Matches exact PrePass decoding layout: RGB = light + rim, A = rim.r
                col.rgb = light.rgb + rimColor;
                col.a = rimColor.r;
                
                return col;
            }
            ENDCG
        }
    }
}