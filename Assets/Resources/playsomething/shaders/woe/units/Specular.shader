Shader "WOE/Units/Specular" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range(0.03,1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="WOEUnit" }

        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="WOEUnit" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform fixed4 _Color;
            uniform half _Shininess;
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
                float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDir = normalize(worldViewDir + lightDir);

                fixed4 mainTex = tex2D(_MainTex, i.uv);
                float3 albedo = mainTex.rgb;

                // Ambient / Vertex Light contribution
                float3 ambientTerm = albedo * i.ambientOrVertexLight;

                // Direct Light Attenuation / Shadows
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                // Specular and Diffuse
                float NdotH = max(0.0, dot(worldNormal, halfDir));
                float spec = pow(NdotH, _Shininess * 128.0) * mainTex.a;
                float NdotL = max(0.0, dot(worldNormal, lightDir));

                float3 diffLighting = albedo * _LightColor0.rgb * NdotL;
                float3 specLighting = _LightColor0.rgb * spec;

                // GLSL specifies exact 2.0x multiplier on direct lighting
                float3 directTerm = (diffLighting + specLighting) * (atten * 2.0);

                fixed4 finalColor;
                finalColor.rgb = ambientTerm + directTerm;
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDCG
        }
    }
}