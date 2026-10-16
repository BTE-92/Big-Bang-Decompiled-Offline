Shader "Vortex Game Studios/OLD TV Filter 2/Screen Filter" {
    Properties {
        _ScreenHorizontal ("Screen Horizontal Orientation", Float) = 1
        _Mobile ("Is on mobile", Float) = 0
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Saturation ("Saturation", Range(0,1)) = 0
        _ChromaticAberrationTex ("Chromatic Aberration (RGB)", 2D) = "black" {}
        _ChromaticAberrationMagnetude ("Chromatic Aberration Magnetude", Range(-1,1)) = 0
        _NoiseTex ("Noise (RGB)", 2D) = "black" {}
        _NoiseMagnetude ("Noise Magnetude", Range(-1,1)) = 0.5
        _StaticTex ("Static (RGB)", 2D) = "black" {}
        _StaticMask ("Static Mask (RGB)", 2D) = "white" {}
        _StaticMagnetude ("Static Magnetude", Range(-1,1)) = 0.5
    }
    SubShader {
        LOD 200
        Tags { "IGNOREPROJECTOR"="true" "RenderType"="Opaque" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }

        Pass {
            Tags { "IGNOREPROJECTOR"="true" "RenderType"="Opaque" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float _Saturation;
            uniform sampler2D _ChromaticAberrationTex;
            uniform float4 _ChromaticAberrationTex_ST;
            uniform float _ChromaticAberrationMagnetude;
            uniform sampler2D _NoiseTex;
            uniform float4 _NoiseTex_ST;
            uniform float _NoiseMagnetude;
            uniform sampler2D _StaticTex;
            uniform float4 _StaticTex_ST;
            uniform sampler2D _StaticMask;
            uniform float4 _StaticMask_ST;
            uniform float _StaticMagnetude;

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvMain : TEXCOORD0;
                float2 uvChrom : TEXCOORD1;
                float2 uvNoise : TEXCOORD2;
                float2 uvStatic : TEXCOORD3;
                float2 uvStaticMask : TEXCOORD4;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvChrom = TRANSFORM_TEX(v.texcoord, _ChromaticAberrationTex);
                o.uvNoise = TRANSFORM_TEX(v.texcoord, _NoiseTex);
                o.uvStatic = TRANSFORM_TEX(v.texcoord, _StaticTex);
                o.uvStaticMask = TRANSFORM_TEX(v.texcoord, _StaticMask);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Sample Chromatic Aberration offset vector
                float3 chromOffset = tex2D(_ChromaticAberrationTex, i.uvChrom).rgb * _ChromaticAberrationMagnetude;

                // CRT Static / Displacement Calculation
                float2 staticUV = float2(i.uvStatic.y, 0.0);
                float2 staticOffset = float2(tex2D(_StaticTex, staticUV).g - 0.5, 0.0);
                float staticMaskVal = tex2D(_StaticMask, -i.uvStaticMask).g;

                float2 outUV = i.uvMain + (staticOffset * (_StaticMagnetude * staticMaskVal));

                // Chromatic Aberration per-channel UV offsets
                fixed3 color;
                color.r = tex2D(_MainTex, outUV - float2(chromOffset.x, 0.0)).r;
                color.g = tex2D(_MainTex, outUV).g;
                color.b = tex2D(_MainTex, outUV + float2(chromOffset.z, 0.0)).b;

                // Add Signal Noise
                color += tex2D(_NoiseTex, i.uvNoise).rgb * _NoiseMagnetude;

                // Desaturation / Rec. 709 Luminance Lerp
                float lum = dot(float3(0.2126, 0.7152, 0.0722), color);
                color = lerp(color, float3(lum, lum, lum), _Saturation);

                fixed4 col;
                col.rgb = color;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}