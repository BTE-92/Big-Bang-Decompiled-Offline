// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "WOE/Unlit/ColorUnlitTransparent" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader { 
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="true" 
            "RenderType"="Transparent" 
        }
        Pass {
            Tags { 
                "Queue"="Transparent" 
                "IgnoreProjector"="true" 
                "RenderType"="Transparent" 
            }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;     // Maps to _glesVertex
                fixed4 color : COLOR;         // Maps to _glesColor
                float2 uv : TEXCOORD0;        // Maps to _glesMultiTexCoord0
            };

            struct v2f {
                float4 vertex : SV_POSITION;  // Maps to gl_Position
                fixed4 color : COLOR0;        // Maps to xlv_COLOR0
                float2 uv : TEXCOORD0;        // Maps to xlv_TEXCOORD0
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                
                // 1. Clamps vertex color and assigns to varying (exact GLSL logic)
                o.color = clamp(v.color, 0.0, 1.0);
                
                // 2. Forces w-component to 1.0 (exact GLSL logic: tmpvar_3)
                float4 tmpvar_3 = float4(v.vertex.xyz, 1.0);
                
                // 3. Transform using Unity 5 compatible MVP matrix
                o.vertex = UnityObjectToClipPos(tmpvar_3);
                
                // 4. Exact UV transform logic
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Sample Texture (tmpvar_2)
                fixed4 tmpvar_2 = tex2D(_MainTex, i.uv);
                
                // 2. Component-wise multiplication of RGB and Alpha (col_1)
                fixed4 col_1;
                col_1.xyz = (tmpvar_2 * _Color).xyz;
                col_1.w = (tmpvar_2.w * _Color.w);
                
                return col_1;
            }
            ENDCG
        }
    }
}