Shader "Character/Ghost Shader"
{
    Properties
    {
        _MainTex    ("Texture", 2D) = "white" {}
        _BumpMap    ("Bumpmap", 2D) = "bump" {}
        _RimColor   ("Rim Color", Color) = (0.46,0,1,0)
        _RimPower   ("Rim Power", Range(0.2,2)) = 0.5
        _Brightness ("Brightness", Range(0,3)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" }

        Pass
        {
            Tags { "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" }
            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                fixed4 color : COLOR0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = saturate(v.color);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
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
            sampler2D _BumpMap;
            float4    _BumpMap_ST;
            fixed4    _RimColor;
            half      _RimPower;
            half      _Brightness;

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
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 basecol = tex2D(_MainTex, i.uv.xy);
                fixed  grayscale = dot(basecol.rgb, fixed3(0.3, 0.59, 0.11));

                half3 tn = tex2D(_BumpMap, i.uv.zw).xyz * 2.0 - 1.0;

                half3 worldN;
                worldN.x = dot(i.tbnRow0, tn);
                worldN.y = dot(i.tbnRow1, tn);
                worldN.z = dot(i.tbnRow2, tn);

                half3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                half3 tangentViewDir = half3(
                    dot(half3(i.tbnRow0.x, i.tbnRow1.x, i.tbnRow2.x), worldViewDir),
                    dot(half3(i.tbnRow0.y, i.tbnRow1.y, i.tbnRow2.y), worldViewDir),
                    dot(half3(i.tbnRow0.z, i.tbnRow1.z, i.tbnRow2.z), worldViewDir)
                );

                half  ndv = clamp(dot(normalize(tangentViewDir), tn), 0.0, 1.0);
                half  rimFactor = pow(1.0 - ndv, _RimPower);
                half3 rim = _RimColor.rgb * rimFactor * _Brightness;
                half  rimAlpha = (rim.r + rim.g + rim.b) / 3.0;

                half3 lightDir = _WorldSpaceLightPos0.xyz;
                fixed diff = max((fixed)0.0, dot(worldN, lightDir));

                fixed3 lit = grayscale * _LightColor0.rgb * diff;
                return fixed4(lit + rim, rimAlpha);
            }
            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha One
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _BumpMap;
            float4    _BumpMap_ST;
            fixed4    _RimColor;
            half      _RimPower;
            half      _Brightness;

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
                fixed  grayscale = dot(basecol.rgb, fixed3(0.3, 0.59, 0.11));

                half3 tn = tex2D(_BumpMap, i.uv.zw).xyz * 2.0 - 1.0;

                half3 worldN;
                worldN.x = dot(i.tbnRow0, tn);
                worldN.y = dot(i.tbnRow1, tn);
                worldN.z = dot(i.tbnRow2, tn);

                half3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                half3 tangentViewDir = half3(
                    dot(half3(i.tbnRow0.x, i.tbnRow1.x, i.tbnRow2.x), worldViewDir),
                    dot(half3(i.tbnRow0.y, i.tbnRow1.y, i.tbnRow2.y), worldViewDir),
                    dot(half3(i.tbnRow0.z, i.tbnRow1.z, i.tbnRow2.z), worldViewDir)
                );

                half  ndv = clamp(dot(normalize(tangentViewDir), tn), 0.0, 1.0);
                half  rimFactor = pow(1.0 - ndv, _RimPower);
                half3 rim = _RimColor.rgb * rimFactor * _Brightness;
                half  rimAlpha = (rim.r + rim.g + rim.b) / 3.0;

                #ifndef USING_DIRECTIONAL_LIGHT
                    half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    half3 lightDir = _WorldSpaceLightPos0.xyz;
                #endif

                fixed atten = LIGHT_ATTENUATION(i);
                fixed diff  = max((fixed)0.0, dot(worldN, lightDir));

                fixed3 lit = grayscale * (_LightColor0.rgb * atten) * diff;
                return fixed4(lit, rimAlpha);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}