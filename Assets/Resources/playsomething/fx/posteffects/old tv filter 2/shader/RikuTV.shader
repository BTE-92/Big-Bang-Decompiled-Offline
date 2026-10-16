Shader "Vortex Game Studios/OLD TV Filter 2/RikuTV" {
    Properties {
        _ScreenHorizontal ("Screen Horizontal Orientation", Float) = 1
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Mask ("Mask (RGB)", 2D) = "white" {}
        _Distortion ("Distortion", Range(-1,1)) = 0.1
        _Saturation ("Saturation", Range(0,1)) = 0
        _NoiseTex ("Noise (RGB)", 2D) = "black" {}
        _NoiseMagnetude ("Noise Magnetude", Range(-1,1)) = 0.5
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
            uniform sampler2D _Mask;
            uniform float _Distortion;
            uniform sampler2D _NoiseTex;
            uniform float4 _NoiseTex_ST;
            uniform float _NoiseMagnetude;

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvMain : TEXCOORD0;
                float2 uvNoise : TEXCOORD1;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvNoise = TRANSFORM_TEX(v.texcoord, _NoiseTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uvMain;

                // Screen distortion calculation
                if (_Distortion != 0.0) {
                    float dFactor = _Distortion * 0.098;
                    uv = (uv * ((1.0 - dFactor) - dFactor)) + dFactor;
                    float2 centerOffset = uv - 0.5;
                    float distSq = dot(centerOffset, centerOffset) * _Distortion;
                    uv += (centerOffset * (1.0 + distSq)) * distSq;
                }

                fixed4 mainColor = tex2D(_MainTex, uv);
                fixed4 noiseColor = tex2D(_NoiseTex, i.uvNoise);
                fixed4 maskColor = tex2D(_Mask, uv);

                fixed4 col;
                col.w = mainColor.w;
                col.xyz = (mainColor.xyz + (noiseColor.xyz * _NoiseMagnetude * 0.7)) * maskColor.xyz;

                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}