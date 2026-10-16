Shader "Framework/FontShader" {
    Properties {
        _MainTex ("Font Texture", 2D) = "white" { }
        _Color ("Text Color", Color) = (1,1,1,1)
    }
    SubShader { 
        Tags { 
            "Queue"="Transparent+20" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Pass {
            Tags { 
                "Queue"="Transparent+20" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
            ZWrite Off
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
                fixed4 color : COLOR0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                // Forces w component to 1.0, matching 'tmpvar_3' in original vertex GLSL
                float4 pos = float4(v.vertex.xyz, 1.0);
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = saturate(_Color);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Preserves text RGB color and multiplies font texture alpha by color alpha
                fixed4 col;
                col.rgb = i.color.rgb;
                col.a = tex2D(_MainTex, i.uv).a * i.color.a;
                return col;
            }
            ENDCG
        }
    }
}