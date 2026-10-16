Shader "WOE/Fx/BlendAdd" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _ColorTint ("Color Tint", Color) = (0,0,0,1)
    }
    SubShader { 
        LOD 100
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Pass {
            Tags { 
                "Queue"="Transparent" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
            ZWrite Off
            Cull Off
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile SOFTPARTICLES_OFF SOFTPARTICLES_ON
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ColorTint;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Combined sample from MainTex, vertex color, and _ColorTint
                fixed4 c = tex2D(_MainTex, i.uv) * (i.color * _ColorTint);

                // Exact GLSL pre-multiplied alpha math
                c.rgb = c.rgb * c.a;
                c.a = c.a * c.a;

                return c;
            }
            ENDCG
        }
    }
}