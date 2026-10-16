Shader "WOE/Units/UnlitRimUnit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" { }
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
    }
    SubShader { 
        LOD 400
        Tags { "RenderType"="WOEUnit" }

        // --- PASS 1: FORWARD BASE (Unlit Base + Vertex Lights + Rim) ---
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="WOEUnit" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                half3 vertexLight : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
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

                // Accumulate 4-vertex lights if VERTEXLIGHT_ON keyword is active
                #ifdef VERTEXLIGHT_ON
                    o.vertexLight = Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, worldPos, worldNormal);
                #else
                    o.vertexLight = half3(0.0, 0.0, 0.0);
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed3 baseColor = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

                half3 worldNormal = normalize(i.worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                // Rim light calculation
                half rim = pow(1.0 - saturate(dot(viewDir, worldNormal)), _RimPower);
                half3 rimColor = _RimColor * rim;

                fixed4 col;
                // Matches exact GLSL fragment assembly: (baseColor * vertexLight) + baseColor + rimColor
                col.rgb = (baseColor * i.vertexLight) + baseColor + rimColor;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }

        // --- PASS 2: FORWARD ADD ---
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "RenderType"="WOEUnit" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col;
                col.rgb = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                col.a = 1.0;
                return col;
            }
            ENDCG
        }
    }
}