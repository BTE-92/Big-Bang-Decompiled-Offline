Shader "WOE/Fx/VehicleColor" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _BaseColor ("BaseColor", Color) = (1,0,0,1)
        _BaseColorMasks ("BaseColorMasks", 2D) = "white" {}
        _PatternColor ("PatternColor", Color) = (1,0.6,0,1)
        _PatternColorMask ("PatternColorMask", 2D) = "white" {}
        _RimExponent ("RimExponent", Float) = 2
        _RimIntensity ("RimIntensity", Float) = 1
        _Reflection ("Reflection", CUBE) = "black" {}
        _ReflectionIntensity ("ReflectionIntensity", Float) = 0
    }
    SubShader {
        Tags { "RenderType"="WOEUnit" }

        Pass {
            Name "FORWARDBASE"
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="WOEUnit" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float4 _BaseColor;
            uniform sampler2D _BaseColorMasks;
            uniform float4 _BaseColorMasks_ST;
            uniform float4 _PatternColor;
            uniform sampler2D _PatternColorMask;
            uniform float4 _PatternColorMask_ST;
            uniform float _RimExponent;
            uniform float _RimIntensity;
            uniform samplerCUBE _Reflection;
            uniform float _ReflectionIntensity;

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
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

                // Reflection vector lookup: reflect(-viewDir, normal)
                float3 reflDir = reflect(-viewDir, worldNormal);
                float4 reflSample = texCUBE(_Reflection, reflDir);

                // Sample textures with individual UV scaling & offsets
                float4 baseColorMasksVar = tex2D(_BaseColorMasks, (i.uv * _BaseColorMasks_ST.xy) + _BaseColorMasks_ST.zw);
                float4 mainTexVar = tex2D(_MainTex, (i.uv * _MainTex_ST.xy) + _MainTex_ST.zw);
                float4 patternColorMaskVar = tex2D(_PatternColorMask, (i.uv * _PatternColorMask_ST.xy) + _PatternColorMask_ST.zw);

                // Reflection Intensity math: refl + (intensity * refl)
                float3 reflectionColor = reflSample.rgb + (_ReflectionIntensity * reflSample.rgb);
                
                // Mask reflection using BaseColorMasks (1.0 - (1.0 - mask) = mask)
                float3 maskedReflection = reflectionColor * baseColorMasksVar.rgb;

                // Base Color Layer: lerp between mainTex and (BaseColor * mainTex) using BaseColorMasks.r
                float3 baseLayer = lerp(mainTexVar.rgb, _BaseColor.rgb * mainTexVar.rgb, baseColorMasksVar.r);

                // Pattern Color Layer: lerp base layer with (mainTex * PatternColor) using PatternColorMask.rgb
                float3 patternLayer = lerp(baseLayer, mainTexVar.rgb * _PatternColor.rgb, patternColorMaskVar.rgb);

                // Combine reflection + pattern layer and clamp [0, 1]
                float3 combinedAlbedo = saturate(maskedReflection + patternLayer);

                // Lighting calculation: (NdotL * 2.0 * LightColor) + (Ambient * 4.0)
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0.0, dot(worldNormal, lightDir));
                float3 diffLighting = (NdotL * (2.0 * _LightColor0.rgb)) + (UNITY_LIGHTMODEL_AMBIENT.rgb * 2.0);

                // Rim Lighting calculation: RimIntensity * (1.0 - NdotV)^RimExponent
                float NdotV = max(0.0, dot(worldNormal, viewDir));
                float rimFactor = _RimIntensity * pow(1.0 - NdotV, _RimExponent);

                // Final color output: (Lighting * Albedo) + Rim
                float4 finalRGBA;
                finalRGBA.rgb = (diffLighting * combinedAlbedo) + rimFactor;
                finalRGBA.a = 1.0;

                return finalRGBA;
            }
            ENDCG
        }
    }
}