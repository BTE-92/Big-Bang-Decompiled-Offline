Shader "WOE/Units/AlphaTestDiffuse"
{
    Properties
    {
        _Color   ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AlphaTex("Trans. (Alpha)", 2D) = "white" {}
        _Cutoff  ("Alpha cutoff", Range(0,1)) = 0.5
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

            fixed4    _Color;
            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _AlphaTex;
            fixed     _Cutoff;

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
                fixed4 mainCol = tex2D(_MainTex, i.uv) * _Color;
                fixed  alpha   = tex2D(_AlphaTex, i.uv).a;
                clip(alpha - _Cutoff);

                half3 lightDir = _WorldSpaceLightPos0.xyz;
                fixed diff     = max((fixed)0.0, dot(i.worldNormal, lightDir));
                fixed atten    = LIGHT_ATTENUATION(i);

                fixed3 lit = mainCol.rgb * _LightColor0.rgb * diff * atten
                           + mainCol.rgb * i.shAmbient;

                return fixed4(lit, alpha);
            }
            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" }
            ZWrite Off
            Blend One One
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4    _Color;
            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _AlphaTex;
            fixed     _Cutoff;

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
                fixed alpha = tex2D(_AlphaTex, i.uv).a;
                clip(alpha - _Cutoff);

                fixed4 mainCol = tex2D(_MainTex, i.uv) * _Color;

                #ifndef USING_DIRECTIONAL_LIGHT
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    half3 lightDir = _WorldSpaceLightPos0.xyz;
                #endif

                fixed diff  = max((fixed)0.0, dot(i.worldNormal, lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                fixed3 lit = mainCol.rgb * _LightColor0.rgb * diff * atten;
                return fixed4(lit, alpha);
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

            sampler2D _AlphaTex;
            float4    _MainTex_ST;
            fixed     _Cutoff;

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
                fixed alpha = tex2D(_AlphaTex, i.uv).a;
                clip(alpha - _Cutoff);

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

            fixed4    _Color;
            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _AlphaTex;
            sampler2D _LightBuffer;
            fixed     _Cutoff;

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
                fixed alpha = tex2D(_AlphaTex, i.uv).a;
                clip(alpha - _Cutoff);

                fixed4 mainCol = tex2D(_MainTex, i.uv) * _Color;
                half4  light   = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));

                #ifdef UNITY_HDR_ON
                    light = max(light, (half4)0.001);
                    light.rgb += i.shAmbient;
                #else
                    light = -log2(max(light, (half4)0.001));
                    light.rgb += i.shAmbient;
                #endif

                return fixed4(mainCol.rgb * light.rgb, alpha);
            }
            ENDCG
        }
    }
}