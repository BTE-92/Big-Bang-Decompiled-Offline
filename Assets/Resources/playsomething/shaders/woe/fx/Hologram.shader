Shader "WOE/Fx/Hologram" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (0.26,0.7,1,0)
        _RimPower ("Rim Power", Range(0.1,8)) = 3
        _Brightness ("Brightness", Range(0,3)) = 1.5
        _DiffuseAmount ("Diffuse Amount", Range(0,1)) = 0
    }
    SubShader {
        Tags { "QUEUE"="Transparent+1000" "IGNOREPROJECTOR"="true" "RenderType"="Opaque" }

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent+1000" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
            ZTest Always
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

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float4 _RimColor;
            uniform float _RimPower;
            uniform float _Brightness;
            uniform float _DiffuseAmount;

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
                SHADOW_COORDS(3)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                fixed4 basecol = tex2D(_MainTex, i.uv);

                // Rim lighting term lerped with base texture color
                float NdotV = saturate(dot(worldViewDir, worldNormal));
                float rimFactor = pow(1.0 - NdotV, _RimPower);
                float3 rimColorScaled = (_RimColor.rgb * rimFactor) * _Brightness;
                float3 rimTerm = lerp(rimColorScaled, basecol.rgb, _DiffuseAmount);

                // Direct light diffuse term with shadow attenuation
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 diffuse = (basecol.rgb * _LightColor0.rgb * atten) * NdotL;

                fixed4 col;
                col.rgb = diffuse + rimTerm;
                col.a = rimTerm.r;
                return col;
            }
            ENDCG
        }

        // ============================================================
        // FORWARD ADD PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent+1000" "IGNOREPROJECTOR"="true" "RenderType"="Opaque" }
            ZTest Always
            ZWrite Off
            Blend SrcAlpha One
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float4 _RimColor;
            uniform float _RimPower;
            uniform float _Brightness;
            uniform float _DiffuseAmount;

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
                float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed4 basecol = tex2D(_MainTex, i.uv);

                // Rim lighting calculation for alpha determination
                float NdotV = saturate(dot(worldViewDir, worldNormal));
                float rimFactor = pow(1.0 - NdotV, _RimPower);
                float3 rimColorScaled = (_RimColor.rgb * rimFactor) * _Brightness;
                float3 rimTerm = lerp(rimColorScaled, basecol.rgb, _DiffuseAmount);

                // Additional light attenuation and diffuse computation
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 diffuse = (basecol.rgb * _LightColor0.rgb * atten) * NdotL;

                fixed4 col;
                col.rgb = diffuse;
                col.a = rimTerm.r;
                return col;
            }
            ENDCG
        }

        // ============================================================
        // DEFERRED PREPASS FINAL PASS
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LIGHTMODE"="PrePassFinal" "QUEUE"="Transparent+1000" "IGNOREPROJECTOR"="true" "RenderType"="Opaque" }
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_HDR_ON
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float4 _RimColor;
            uniform float _RimPower;
            uniform float _Brightness;
            uniform float _DiffuseAmount;
            uniform sampler2D _LightBuffer;

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
                float4 screenPos : TEXCOORD3;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

                fixed4 basecol = tex2D(_MainTex, i.uv);

                float NdotV = saturate(dot(worldViewDir, worldNormal));
                float rimFactor = pow(1.0 - NdotV, _RimPower);
                float3 rimColorScaled = (_RimColor.rgb * rimFactor) * _Brightness;
                float3 rimTerm = lerp(rimColorScaled, basecol.rgb, _DiffuseAmount);

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));
                #ifndef UNITY_HDR_ON
                    light = -log2(max(light, 0.001));
                #else
                    light = max(light, 0.001);
                #endif

                fixed4 col;
                col.rgb = (basecol.rgb * light.rgb) + rimTerm;
                col.a = rimTerm.r;
                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}