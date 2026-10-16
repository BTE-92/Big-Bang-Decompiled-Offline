Shader "WOE/Units/Ice" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0)
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range(0.01,1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="WOEUnit" }

        // ============================================================
        // PASS 1: BACKFACES (Cull Front)
        // ============================================================
        Pass {
            Tags { "LIGHTMODE"="Vertex" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="WOEUnit" }
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            uniform fixed4 _Color;
            uniform fixed4 _Emission;
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
                fixed4 diffClr : COLOR0;
                fixed3 specClr : COLOR1;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

                // Ambient + Emission lighting
                float3 lcolor = _Emission.rgb + (_Color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb);

                // Vertex lights (4 point lights)
                float3 vLight = Shade4PointLights(
                    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                    unity_4LightAtten0, worldPos, worldNormal
                );

                // Main light direction
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 mainLightDiff = _LightColor0.rgb * NdotL;

                // Total Diffuse
                float3 totalDiff = lcolor + (mainLightDiff + vLight) * _Color.rgb;
                o.diffClr = fixed4(saturate(totalDiff), _Color.a);

                // Blinn-Phong Specular
                float3 halfDir = normalize(lightDir + viewDir);
                float NdotH = max(0.0, dot(worldNormal, halfDir));
                float spec = pow(NdotH, _Shininess * 128.0) * (NdotL > 0.0 ? 1.0 : 0.0);
                o.specClr = saturate(_SpecColor.rgb * _LightColor0.rgb * spec * 0.5);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col;
                col.rgb = (i.diffClr.rgb * tex.rgb) + i.specClr;
                col.a = i.diffClr.a * tex.a;
                return col;
            }
            ENDCG
        }

        // ============================================================
        // PASS 2: FRONTFACES (Cull Back)
        // ============================================================
        Pass {
            Tags { "LIGHTMODE"="Vertex" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="WOEUnit" }
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            uniform fixed4 _Color;
            uniform fixed4 _Emission;
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
                fixed4 diffClr : COLOR0;
                fixed3 specClr : COLOR1;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

                // Ambient + Emission lighting
                float3 lcolor = _Emission.rgb + (_Color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb);

                // Vertex lights (4 point lights)
                float3 vLight = Shade4PointLights(
                    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                    unity_4LightAtten0, worldPos, worldNormal
                );

                // Main light direction
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 mainLightDiff = _LightColor0.rgb * NdotL;

                // Total Diffuse
                float3 totalDiff = lcolor + (mainLightDiff + vLight) * _Color.rgb;
                o.diffClr = fixed4(saturate(totalDiff), _Color.a);

                // Blinn-Phong Specular
                float3 halfDir = normalize(lightDir + viewDir);
                float NdotH = max(0.0, dot(worldNormal, halfDir));
                float spec = pow(NdotH, _Shininess * 128.0) * (NdotL > 0.0 ? 1.0 : 0.0);
                o.specClr = saturate(_SpecColor.rgb * _LightColor0.rgb * spec * 0.5);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col;
                col.rgb = (i.diffClr.rgb * tex.rgb) + i.specClr;
                col.a = i.diffClr.a * tex.a;
                return col;
            }
            ENDCG
        }
    }
}