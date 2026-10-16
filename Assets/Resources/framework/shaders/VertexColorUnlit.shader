Shader "Framework/VertexColorUnlit"
{
    Properties
    {
        _Color   ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent-200" "IgnoreProjector"="true" "RenderType"="Transparent" }

        Pass
        {
            Tags { "Queue"="Transparent-200" "IgnoreProjector"="true" "RenderType"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;

            struct appdata
            {
                float4 vertex   : POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                fixed4 color : COLOR0;
                float2 uv    : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.color = saturate(v.color);
                o.uv    = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col;
                col.rgb = tex.rgb * i.color.rgb;
                col.a   = tex.a * i.color.a;
                return col;
            }
            ENDCG
        }
    }
}