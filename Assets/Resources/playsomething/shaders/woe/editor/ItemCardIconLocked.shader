Shader "WOE/Editor/ItemCardIconLocked" {
    Properties {
        _TintColor ("Color", Color) = (0,0,0,1)
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader { 
        Tags { 
            "Queue"="Transparent+500" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Pass {
            Tags { 
                "Queue"="Transparent+500" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 texcol = tex2D(_MainTex, i.uv);

                // Weighted luminance dot product calculation with _TintColor
                half lum = dot(texcol.rgb, _TintColor.rgb);

                // Replaces texture RGB with greyscale result, preserving texture alpha
                texcol.rgb = lum;

                return texcol;
            }
            ENDCG
        }
    }
    Fallback "WOE/ColorUnlit"
}