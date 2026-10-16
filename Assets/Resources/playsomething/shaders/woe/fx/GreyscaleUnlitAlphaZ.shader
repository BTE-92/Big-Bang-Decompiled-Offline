Shader "WOE/Fx/GreyscaleUnlitAlphaZ" {
Properties {
    _TintColor ("Color", Color) = (0.3,0.59,0.11,1)
    _TintAmount ("Amount", Range(0,1)) = 1
    _MainTex ("Texture", 2D) = "white" { }
}
SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    Pass {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_ST;
        fixed4 _TintColor;
        fixed _TintAmount;

        struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        v2f vert (appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 texcol = tex2D(_MainTex, i.uv);
            float grey = dot(texcol.rgb, _TintColor.rgb);
            texcol.rgb = texcol.rgb * (1.0 - _TintAmount) + grey * _TintAmount;
            return texcol;
        }
        ENDCG
    }
}
Fallback "WOE/ColorUnlit"
}