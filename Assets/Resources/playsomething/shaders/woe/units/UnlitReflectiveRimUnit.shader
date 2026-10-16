Shader "WOE/Units/UnlitReflectiveRimUnit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
        _Cube ("Cubemap", CUBE) = "black" {}
    }
    SubShader {
        LOD 400
        Tags { "RenderType"="WOEUnit" }

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="WOEUnit" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed3 _RimColor;
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
                float3 vertexLight : TEXCOORD4;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                // Reflection vector from camera to vertex reflected across normal
                float3 worldViewDir = o.worldPos - _WorldSpaceCameraPos;
                o.reflDir = reflect(worldViewDir, o.worldNormal);

                #ifdef VERTEXLIGHT_ON
                    o.vertexLight = Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, o.worldPos, o.worldNormal
                    );
                #else
                    o.vertexLight = float3(0.0, 0.0, 0.0);
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                // Sample texture and add cubemap reflection
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed3 reflTex = texCUBE(_Cube, i.reflDir).rgb;
                fixed3 baseColor = (mainTex.rgb * _Color.rgb) + reflTex;

                // Rim lighting computation
                float NdotV = saturate(dot(viewDir, worldNormal));
                float rimFactor = pow(1.0 - NdotV, _RimPower);
                fixed3 rimColor = _RimColor * rimFactor;

                // Final color composition
                fixed3 finalColor = (baseColor * i.vertexLight) + baseColor + rimColor;

                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }

        // ============================================================
        // FORWARD ADD PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="WOEUnit" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            #include "UnityCG.cginc"

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
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
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldViewDir = worldPos - _WorldSpaceCameraPos;
                o.reflDir = reflect(worldViewDir, worldNormal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed3 reflTex = texCUBE(_Cube, i.reflDir).rgb;
                fixed3 baseColor = (mainTex.rgb * _Color.rgb) + reflTex;

                return fixed4(baseColor, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}