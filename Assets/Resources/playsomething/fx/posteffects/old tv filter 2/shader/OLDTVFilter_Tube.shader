Shader "Vortex Game Studios/OLD TV Filter 2/Tube Filter" {
    Properties {
        _ScreenHorizontal ("Screen Horizontal Orientation", Float) = 1
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Pixel ("Pixel (RGB)", 2D) = "white" {}
        _ScreenWidth ("Screen Width", Float) = 0
        _ScreenHeight ("Screen Height", Float) = 0
        _ScanLine ("ScanLine (RGB)", 2D) = "white" {}
        _ScanLineCount ("ScanLine Count (Resolution Size)", Float) = 100
        _ScanLineMin ("ScanLine Min", Float) = 0
        _ScanLineMax ("ScanLine Max", Float) = 1
        _Mask ("Mask (RGB)", 2D) = "white" {}
        _Reflex ("Reflex (RGB)", 2D) = "black" {}
        _ReflexMagnetude ("Reflex Magnetude", Range(0,1)) = 0.5
        _Distortion ("Distortion", Range(-1,1)) = 0.1
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

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;

                // CRT Tube Lens Distortion Calculation
                if (_Distortion != 0.0) {
                    float dFactor = _Distortion * 0.098;
                    uv = (uv * ((1.0 - dFactor) - dFactor)) + dFactor;
                    float2 centerOffset = uv - 0.5;
                    float distSq = dot(centerOffset, centerOffset) * _Distortion;
                    uv += (centerOffset * (1.0 + distSq)) * distSq;
                }

                fixed4 mainColor = tex2D(_MainTex, uv);
                fixed4 maskColor = tex2D(_Mask, uv);

                fixed4 col;
                col.rgb = mainColor.rgb * maskColor.rgb;
                col.a = mainColor.a;

                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}