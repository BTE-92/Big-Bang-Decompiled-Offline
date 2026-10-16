Shader "WOE/Fx/UnlitAlphaRimPlanet" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
        _RimColor ("RimColor", Color) = (1,1,1,1)
        _RimPower ("RimPower", Range(0,5)) = 3.59223
        _RimMultiplier ("RimMultiplier", Float) = 1
        _FogColor ("FogColor", Color) = (0.301471,0.710953,1,1)
        _FogPower ("FogPower", Range(0,5)) = 2.84466
        _FogMultiplier ("FogMultiplier", Float) = 3
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Name "FORWARDBASE"
            Tags { "LightMode"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float rawUVY : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _RimColor;
            float _RimPower;
            float _RimMultiplier;
            fixed4 _FogColor;
            float _FogPower;
            float _FogMultiplier;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.rawUVY = v.uv.y; // Preserves unscaled UV.y for fog calculation
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);

                half3 worldNormal = normalize(i.worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                // Rim calculation: pow(1.0 - max(0.0, dot(N, V)), _RimPower) * _RimMultiplier
                half nDotV = max(0.0, dot(worldNormal, viewDir));
                half rim = saturate(pow(1.0 - nDotV, _RimPower) * _RimMultiplier);
                half3 basePlusRim = (_RimColor.rgb * rim) + tex.rgb;

                // Fog gradient calculation based on vertical UV coordinate
                half fogFactor = saturate(pow(1.0 - i.rawUVY, _FogPower) * _FogMultiplier);

                // Mix (lerp) base+rim with fog color
                fixed4 col;
                col.rgb = lerp(basePlusRim, _FogColor.rgb, fogFactor);
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}