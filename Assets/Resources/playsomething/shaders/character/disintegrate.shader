Shader "Character/Disintegrate Diffuse"
{
    Properties
    {
        _MainTex            ("Texture (RGB)", 2D) = "white" {}
        _NoiseTex           ("Effect Map (RGB)", 2D) = "white" {}
        _DisintegrateAmount ("Effect Amount", Range(0,1.01)) = 0
        _DissolveColor      ("Edge Color", Color) = (1,0.5,0.2,0)
        _EdgeEmission       ("Edge Emission", Color) = (0,0,0,0)
        _DissolveEdge       ("Edge Range", Range(0,0.1)) = 0.01
        _TileFactor         ("Tile Factor", Range(0,4)) = 1
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
            sampler2D _NoiseTex;
            half      _DisintegrateAmount;
            fixed4    _DissolveColor;
            fixed4    _EdgeEmission;
            half      _DissolveEdge;
            half      _TileFactor;

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
                half noiseVal = tex2D(_NoiseTex, i.uv * _TileFactor).r - _DisintegrateAmount;
                clip(noiseVal);

                fixed3 basecol;
                fixed3 emission = (fixed3)0.0;

                if (noiseVal < _DissolveEdge && _DisintegrateAmount > 0.0)
                {
                    basecol = _DissolveColor.rgb;
                    emission = _EdgeEmission.rgb;
                }
                else
                {
                    basecol = tex2D(_MainTex, i.uv).rgb;
                }

                half3 lightDir = _WorldSpaceLightPos0.xyz;
                fixed diff     = max((fixed)0.0, dot(i.worldNormal, lightDir));
                fixed atten    = LIGHT_ATTENUATION(i);

                fixed3 lit = basecol * _LightColor0.rgb * diff * atten;
                fixed3 amb = basecol * i.shAmbient;

                return fixed4(lit + amb + emission, 1.0);
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
            sampler2D _NoiseTex;
            half      _DisintegrateAmount;
            fixed4    _DissolveColor;
            fixed4    _EdgeEmission;
            half      _DissolveEdge;
            half      _TileFactor;

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
                half noiseVal = tex2D(_NoiseTex, i.uv * _TileFactor).r - _DisintegrateAmount;
                clip(noiseVal);

                fixed3 basecol;

                if (noiseVal < _DissolveEdge && _DisintegrateAmount > 0.0)
                    basecol = _DissolveColor.rgb;
                else
                    basecol = tex2D(_MainTex, i.uv).rgb;

                #ifndef USING_DIRECTIONAL_LIGHT
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    half3 lightDir = _WorldSpaceLightPos0.xyz;
                #endif

                fixed diff  = max((fixed)0.0, dot(i.worldNormal, lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                fixed3 lit = basecol * _LightColor0.rgb * diff * atten;
                return fixed4(lit, 1.0);
            }
            ENDCG
        }

        Pass
        {
            Name "PREPASS"
            Tags { "LightMode"="PrePassBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _NoiseTex;
            half      _DisintegrateAmount;
            fixed4    _DissolveColor;
            fixed4    _EdgeEmission;
            half      _DissolveEdge;
            half      _TileFactor;

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half3  worldNormal : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos         = UnityObjectToClipPos(v.vertex);
                o.uv          = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half noiseVal = tex2D(_NoiseTex, i.uv * _TileFactor).r - _DisintegrateAmount;
                clip(noiseVal);

                fixed4 res;
                res.rgb = i.worldNormal * 0.5 + 0.5;
                res.a   = 0.0;
                return res;
            }
            ENDCG
        }

        Pass
        {
            Name "PREPASS"
            Tags { "LightMode"="PrePassFinal" }
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ UNITY_HDR_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _NoiseTex;
            sampler2D _LightBuffer;
            half      _DisintegrateAmount;
            fixed4    _DissolveColor;
            fixed4    _EdgeEmission;
            half      _DissolveEdge;
            half      _TileFactor;

            struct v2f
            {
                float4 pos       : SV_POSITION;
                float2 uv        : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                half3  shAmbient : TEXCOORD2;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos       = UnityObjectToClipPos(v.vertex);
                o.uv        = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);

                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 sh = ShadeSH9(half4(wNormal, 1.0));
                sh = max(1.055 * pow(max(sh, (half3)0.0), (half3)0.4166667) - 0.055,
                         (half3)0.0);
                o.shAmbient = sh;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half noiseVal = tex2D(_NoiseTex, i.uv * _TileFactor).r - _DisintegrateAmount;
                clip(noiseVal);

                fixed3 basecol;
                fixed3 emission = (fixed3)0.0;

                if (noiseVal < _DissolveEdge && _DisintegrateAmount > 0.0)
                {
                    basecol = _DissolveColor.rgb;
                    emission = _EdgeEmission.rgb;
                }
                else
                {
                    basecol = tex2D(_MainTex, i.uv).rgb;
                }

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));

                #ifdef UNITY_HDR_ON
                    light = max(light, (half4)0.001);
                    light.rgb += i.shAmbient;
                #else
                    light = -log2(max(light, (half4)0.001));
                    light.rgb += i.shAmbient;
                #endif

                fixed3 lit = basecol * light.rgb + emission;
                return fixed4(lit, 1.0);
            }
            ENDCG
        }

        Pass
        {
            Name "SHADOWCASTER"
            Tags { "LightMode"="ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _NoiseTex;
            half      _DisintegrateAmount;
            fixed4    _DissolveColor;
            fixed4    _EdgeEmission;
            half      _DissolveEdge;
            half      _TileFactor;

            struct v2f
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half noiseVal = tex2D(_NoiseTex, i.uv * _TileFactor).r - _DisintegrateAmount;
                clip(noiseVal);

                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}