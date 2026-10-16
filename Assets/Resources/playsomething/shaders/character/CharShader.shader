Shader "Character/CharShader-lambertwrap"
{

    Properties
    {
        _MainTex   ("Texture", 2D) = "white" {}
        _MaskTex   ("MaskTex (RGBA)", 2D) = "black" {}
        _EyeColor  ("Eye Color", Color) = (0.5,0.5,0.5,1)
        _SkinColor ("Skin Color", Color) = (0.5,0.5,0.5,1)
        _HairColor ("Hair Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _MaskTex;
            fixed4    _EyeColor;
            fixed4    _SkinColor;
            fixed4    _HairColor;

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half3  worldNormal : TEXCOORD1;
                half3  shAmbient   : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);

                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                o.worldNormal = wNormal;

                half3 sh = ShadeSH9(half4(wNormal, 1.0));
                sh = max(1.055 * pow(max(sh, (half3)0.0), (half3)0.4166667) - 0.055,
                         (half3)0.0);

                #ifdef VERTEXLIGHT_ON
                    sh += Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb,
                        unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0,
                        mul(unity_ObjectToWorld, v.vertex).xyz,
                        wNormal);
                #endif

                o.shAmbient = sh;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 basecol = tex2D(_MainTex, i.uv);
                fixed4 maskcol = tex2D(_MaskTex, i.uv);

                if (maskcol.r > 0.0)
                {
                    fixed lum = dot(basecol.rgb, fixed3(0.3, 0.59, 0.11));
                    fixed3 newcol = lum * 2.0 * _EyeColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, newcol, maskcol.r);
                }
                if (maskcol.g > 0.0)
                {
                    fixed3 newcol = basecol.rgb * 2.0 * _SkinColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, newcol, maskcol.g);
                }
                if (maskcol.b > 0.0)
                {
                    fixed3 newcol = basecol.rgb * 2.0 * _HairColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, newcol, maskcol.b);
                }

                half3 lightDir = _WorldSpaceLightPos0.xyz;
                fixed atten    = LIGHT_ATTENUATION(i);
                fixed halfLamb = dot(i.worldNormal, lightDir) * 0.5 + 0.5;

                fixed3 lit = basecol.rgb * _LightColor0.rgb * halfLamb * atten * 2.0;
                fixed3 amb = basecol.rgb * i.shAmbient;

                return fixed4(amb + lit, 1.0);
            }
            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _MaskTex;
            fixed4    _EyeColor;
            fixed4    _SkinColor;
            fixed4    _HairColor;

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half3  worldNormal : TEXCOORD1;
                float3 worldPos    : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos         = UnityObjectToClipPos(v.vertex);
                o.uv          = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos    = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 basecol = tex2D(_MainTex, i.uv);
                fixed4 maskcol = tex2D(_MaskTex, i.uv);

                if (maskcol.r > 0.0)
                {
                    fixed lum = dot(basecol.rgb, fixed3(0.3, 0.59, 0.11));
                    fixed3 newcol = lum * 2.0 * _EyeColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, newcol, maskcol.r);
                }
                if (maskcol.g > 0.0)
                {
                    fixed3 newcol = basecol.rgb * 2.0 * _SkinColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, newcol, maskcol.g);
                }
                if (maskcol.b > 0.0)
                {
                    fixed3 newcol = basecol.rgb * 2.0 * _HairColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, newcol, maskcol.b);
                }

                #ifndef USING_DIRECTIONAL_LIGHT
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    half3 lightDir = _WorldSpaceLightPos0.xyz;
                #endif

                fixed atten    = LIGHT_ATTENUATION(i);
                fixed halfLamb = dot(i.worldNormal, lightDir) * 0.5 + 0.5;

                fixed3 lit = basecol.rgb * _LightColor0.rgb * halfLamb * atten * 2.0;
                return fixed4(lit, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}