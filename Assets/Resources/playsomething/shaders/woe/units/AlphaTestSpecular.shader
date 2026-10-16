Shader "WOE/Units/AlphaTestSpecular"
{
    Properties
    {
        _Shininess ("Shininess", Range(0.03,1)) = 0.078125
        _MainTex   ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _AlphaTex  ("Trans. (Alpha)", 2D) = "white" {}
        _Cutoff    ("Alpha cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="true" "RenderType"="WOEUnit" }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _AlphaTex;
            half      _Shininess;
            fixed     _Cutoff;

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half3  worldNormal : TEXCOORD1;
                float3 worldPos    : TEXCOORD2;
                half3  shAmbient   : TEXCOORD3;
                LIGHTING_COORDS(4, 5)
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);

                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                o.worldNormal = wNormal;
                o.worldPos    = mul(unity_ObjectToWorld, v.vertex).xyz;

                half3 sh = ShadeSH9(half4(wNormal, 1.0));
                sh = max(1.055 * pow(max(sh, (half3)0.0), (half3)0.4166667) - 0.055,
                         (half3)0.0);

                #ifdef VERTEXLIGHT_ON
                    sh += Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb,
                        unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0,
                        o.worldPos,
                        wNormal);
                #endif

                o.shAmbient = sh;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed  alpha = tex2D(_AlphaTex, i.uv).a;
                clip(alpha - _Cutoff);

                half3 lightDir = _WorldSpaceLightPos0.xyz;
                half3 viewDir  = normalize(_WorldSpaceCameraPos - i.worldPos);
                half3 halfDir  = normalize(viewDir + lightDir);

                fixed atten = LIGHT_ATTENUATION(i);

                fixed  nDotL = max((fixed)0.0, dot(i.worldNormal, lightDir));
                fixed  nDotH = max((fixed)0.0, dot(i.worldNormal, halfDir));
                half   spec  = pow(nDotH, _Shininess * 128.0) * tex.a;

                fixed4 c;
                c.rgb = tex.rgb * i.shAmbient;
                c.a   = 0.0;

                fixed3 lit = ((tex.rgb * _LightColor0.rgb) * nDotL
                            + _LightColor0.rgb * spec) * (atten * 2.0);

                c.rgb += lit;
                return c;
            }
            ENDCG
        }
    }
}