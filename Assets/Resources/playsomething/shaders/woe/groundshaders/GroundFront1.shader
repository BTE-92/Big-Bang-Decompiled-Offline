Shader "WOE/Ground/GroundFront1" {
Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _Emission ("Emissive Color", Color) = (0,0,0,0)
    _Shininess ("Shininess", Range(0.01,1)) = 1
    _MainTex ("Base (RGB)", 2D) = "white" { }
}
SubShader {
    Tags { "RenderType"="Opaque" }
    Pass {
        Tags { "LIGHTMODE"="Vertex" "QUEUE"="Geometry" "RenderType"="Opaque" }

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        fixed4 _Color;
        fixed4 _Emission;
        half _Shininess;
        sampler2D _MainTex;
        float4 _MainTex_ST;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR0;
            float2 uv : TEXCOORD0;
        };

        v2f vert (appdata v)
        {
            v2f o;

            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

            float3 eyeNormal = normalize(mul(v.normal, (float3x3)UNITY_MATRIX_MV));
            float3 eyePos = mul(UNITY_MATRIX_MV, v.vertex).xyz;

            float3 lcolor = _Emission.rgb + (_Color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb);

            for (int il = 0; il < 8; il++)
            {
                float3 dirToLight = unity_LightPosition[il].xyz - (eyePos * unity_LightPosition[il].w);
                float distSq = dot(dirToLight, dirToLight);

                float att = 1.0 / (1.0 + unity_LightAtten[il].z * distSq);

                if (unity_LightPosition[il].w != 0.0 && distSq > unity_LightAtten[il].w)
                {
                    att = 0.0;
                }

                dirToLight *= rsqrt(distSq);

                float rho = max(dot(dirToLight, unity_SpotDirection[il].xyz), 0.0);
                att *= saturate((rho - unity_LightAtten[il].x) * unity_LightAtten[il].y);

                att *= 0.5;

                float NdotL = max(dot(eyeNormal, dirToLight), 0.0);
                float3 lightContrib = (NdotL * _Color.rgb) * unity_LightColor[il].rgb * att;
                lcolor += min(lightContrib, float3(1.0, 1.0, 1.0));
            }

            fixed4 c;
            c.rgb = lcolor;
            c.a = _Color.a;
            o.color = saturate(c);

            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 tex = tex2D(_MainTex, i.uv);
            fixed4 col;
            col.rgb = tex.rgb * i.color.rgb * 2.0;
            col.a = tex.a * _Color.a;
            return col;
        }
        ENDCG
    }
}
}