Shader "Framework/VertexColorUnlit" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader { 
        Tags { 
            "Queue"="Transparent-200" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Pass {
            Tags { 
                "Queue"="Transparent-200" 
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

            v2f vert (appdata v) {
                v2f o;
                // Forces w component to 1.0, matching 'tmpvar_3' in original vertex GLSL
                float4 pos = float4(v.vertex.xyz, 1.0);
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = clamp(v.color, 0.0, 1.0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Multiplies sampled texture by clamped vertex color (matching GLSL fragment logic)
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}