Shader "WOE/Fx/FxHologram" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTexture ("MainTexture", 2D) = "bump" {}
        _Mask ("Mask", 2D) = "white" {}
        [HideInInspector] _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _MainTexture;
            uniform float4 _MainTexture_ST;
            uniform sampler2D _Mask;
            uniform float4 _Mask_ST;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD2;
                float4 clipPos : TEXCOORD3;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.clipPos = o.pos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Screen-space UV computation (manual perspective divide)
                float2 screenUV = i.clipPos.xy / i.clipPos.w;
                float screenY = screenUV.y * _ProjectionParams.x;

                // Light direction (directional light)
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                // Sample main texture
                float2 mainUV = (i.uv * _MainTexture_ST.xy) + _MainTexture_ST.zw;
                float4 mainTexVar = tex2D(_MainTexture, mainUV);

                // Mask UV: aspect-corrected screen-space + time scrolling
                float2 maskScreenUV;
                maskScreenUV.x = screenUV.x * (_ScreenParams.x / _ScreenParams.y);
                maskScreenUV.y = screenY;

                float timeVal = (_Time + _TimeEditor).y;
                float2 maskUV = ((maskScreenUV + (timeVal * float2(0.0, 0.025))) * _Mask_ST.xy) + _Mask_ST.zw;
                float4 maskVar = tex2D(_Mask, maskUV);

                // Lighting: NdotL * lightColor + ambient*2, then multiply by (Color + MainTex) — additive color blend
                float3 normalDir = normalize(i.worldNormal);
                float NdotL = max(0.0, dot(normalDir, lightDir));
                float3 lighting = (NdotL * _LightColor0.xyz) + (UNITY_LIGHTMODEL_AMBIENT).xyz;
                float3 finalColor = lighting * (_Color.xyz + mainTexVar.xyz);

                float4 finalRGBA;
                finalRGBA.xyz = finalColor;
                finalRGBA.w = maskVar.w;
                return finalRGBA;
            }
            ENDCG
        }

        // ============================================================
        // FORWARD ADD PASS
        // ============================================================
        Pass {
            Name "FORWARD_DELTA"
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

            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _MainTexture;
            uniform float4 _MainTexture_ST;
            uniform sampler2D _Mask;
            uniform float4 _Mask_ST;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float4 clipPos : TEXCOORD3;
                LIGHTING_COORDS(4, 5)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.clipPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Screen-space UV computation (manual perspective divide)
                float2 screenUV = i.clipPos.xy / i.clipPos.w;
                float screenY = screenUV.y * _ProjectionParams.x;

                // Light direction: handles both directional (w=0) and point/spot (w=1)
                float3 lightDir = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.worldPos.xyz, _WorldSpaceLightPos0.www));

                // Attenuation
                float atten = LIGHT_ATTENUATION(i);

                // Sample main texture
                float2 mainUV = (i.uv * _MainTexture_ST.xy) + _MainTexture_ST.zw;
                float4 mainTexVar = tex2D(_MainTexture, mainUV);

                // Mask UV: aspect-corrected screen-space + time scrolling
                float2 maskScreenUV;
                maskScreenUV.x = screenUV.x * (_ScreenParams.x / _ScreenParams.y);
                maskScreenUV.y = screenY;

                float timeVal = (_Time + _TimeEditor).y;
                float2 maskUV = ((maskScreenUV + (timeVal * float2(0.0, 0.025))) * _Mask_ST.xy) + _Mask_ST.zw;
                float4 maskVar = tex2D(_Mask, maskUV);

                // Lighting: NdotL * (atten * lightColor), then multiply by (Color + MainTex), then multiply by mask alpha
                float3 normalDir = normalize(i.worldNormal);
                float NdotL = max(0.0, dot(normalDir, lightDir));
                float3 lighting = NdotL * (atten * _LightColor0.xyz);
                float3 finalColor = (lighting * (_Color.xyz + mainTexVar.xyz)) * maskVar.w;

                float4 finalRGBA;
                finalRGBA.xyz = finalColor;
                finalRGBA.w = 0.0;
                return finalRGBA;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}