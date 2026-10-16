Shader "Character/CharShader-bumped-lambertwrap"
{
    Properties
    {
        _MainTex   ("Texture", 2D) = "white" {}
        _MaskTex   ("MaskTex (RGBA)", 2D) = "black" {}
        _BumpMap   ("Bumpmap", 2D) = "bump" {}
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
            sampler2D _BumpMap;
            float4    _BumpMap_ST;
            fixed4    _EyeColor;
            fixed4    _SkinColor;
            fixed4    _HairColor;

            struct appdata
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float4 uv       : TEXCOORD0;
                half3  tbnRow0  : TEXCOORD1;
                half3  tbnRow1  : TEXCOORD2;
                half3  tbnRow2  : TEXCOORD3;
                half3  shLight  : TEXCOORD4;
                LIGHTING_COORDS(5, 6)
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                half3 wNormal   = UnityObjectToWorldNormal(v.normal);
                half3 wTangent  = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
                half  tSign     = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBinormal = cross(wNormal, wTangent) * tSign;

                o.tbnRow0 = half3(wTangent.x, wBinormal.x, wNormal.x);
                o.tbnRow1 = half3(wTangent.y, wBinormal.y, wNormal.y);
                o.tbnRow2 = half3(wTangent.z, wBinormal.z, wNormal.z);

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

                o.shLight = sh;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 basecol = tex2D(_MainTex, i.uv.xy);
                fixed4 maskcol = tex2D(_MaskTex, i.uv.xy);

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

                fixed3 tn = tex2D(_BumpMap, i.uv.zw).xyz * 2.0 - 1.0;

                half3 worldN;
                worldN.x = dot(i.tbnRow0, tn);
                worldN.y = dot(i.tbnRow1, tn);
                worldN.z = dot(i.tbnRow2, tn);

                half3 lightDir = _WorldSpaceLightPos0.xyz;
                fixed atten    = LIGHT_ATTENUATION(i);
                fixed halfLamb = dot(worldN, lightDir) * 0.5 + 0.5;

                fixed3 lit = basecol.rgb * _LightColor0.rgb * halfLamb * atten * 2.0;
                fixed3 amb = basecol.rgb * i.shLight;

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
            sampler2D _BumpMap;
            float4    _BumpMap_ST;
            fixed4    _EyeColor;
            fixed4    _SkinColor;
            fixed4    _HairColor;

            struct appdata
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float4 uv       : TEXCOORD0;
                half3  tbnRow0  : TEXCOORD1;
                half3  tbnRow1  : TEXCOORD2;
                half3  tbnRow2  : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
                LIGHTING_COORDS(5, 6)
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                half3 wNormal   = UnityObjectToWorldNormal(v.normal);
                half3 wTangent  = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
                half  tSign     = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBinormal = cross(wNormal, wTangent) * tSign;

                o.tbnRow0 = half3(wTangent.x, wBinormal.x, wNormal.x);
                o.tbnRow1 = half3(wTangent.y, wBinormal.y, wNormal.y);
                o.tbnRow2 = half3(wTangent.z, wBinormal.z, wNormal.z);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 basecol = tex2D(_MainTex, i.uv.xy);
                fixed4 maskcol = tex2D(_MaskTex, i.uv.xy);

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

                fixed3 tn = tex2D(_BumpMap, i.uv.zw).xyz * 2.0 - 1.0;

                half3 worldN;
                worldN.x = dot(i.tbnRow0, tn);
                worldN.y = dot(i.tbnRow1, tn);
                worldN.z = dot(i.tbnRow2, tn);

                #ifndef USING_DIRECTIONAL_LIGHT
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    half3 lightDir = _WorldSpaceLightPos0.xyz;
                #endif

                fixed atten    = LIGHT_ATTENUATION(i);
                fixed halfLamb = dot(worldN, lightDir) * 0.5 + 0.5;

                fixed3 lit = basecol.rgb * _LightColor0.rgb * halfLamb * atten * 2.0;
                return fixed4(lit, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}