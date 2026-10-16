Shader "WOE/Fx/UnlitReflectiveRim" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" { }
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
        _Cube ("Cubemap", CUBE) = "black" { }
    }
    SubShader { 
        LOD 400
        Tags { "RenderType"="Opaque" }

        // --- PASS 1: FORWARD BASE (Base + Cubemap Reflection + Rim + Vertex Lighting) ---
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }

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
                float3 worldRefl : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                half3 vertexLight : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            samplerCUBE _Cube;
            fixed4 _Color;
            fixed4 _RimColor;
            float _RimPower;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = worldPos;
                o.worldNormal = worldNormal;

                // Reflection vector: reflect(WorldPos - CameraPos, WorldNormal)
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                o.worldRefl = reflect(viewDir, worldNormal);

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
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 cube = texCUBE(_Cube, i.worldRefl);

                // Combine albedo texture with cubemap reflection
                half3 basePlusReflection = (tex.rgb * _Color.rgb) + cube.rgb;

                // Rim light calculation
                half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                half rim = pow(1.0 - saturate(dot(viewDir, i.worldNormal)), _RimPower);
                half3 rimColor = _RimColor.rgb * rim;

                fixed4 col;
                // Matches GLSL assembly: (basePlusReflection * vertexLight) + basePlusReflection + rimColor
                col.rgb = (basePlusReflection * i.vertexLight) + basePlusReflection + rimColor;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }

        // --- PASS 2: FORWARD ADD (Dynamic Additive Pass) ---
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "RenderType"="Opaque" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
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

                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                o.worldRefl = reflect(viewDir, worldNormal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 cube = texCUBE(_Cube, i.worldRefl);

                fixed4 col;
                col.rgb = (tex.rgb * _Color.rgb) + cube.rgb;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }
    Fallback "Specular"
}