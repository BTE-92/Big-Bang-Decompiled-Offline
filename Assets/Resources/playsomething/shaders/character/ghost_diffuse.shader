Shader "Character/Ghost Shader" {
Properties {
    _MainTex ("Texture", 2D) = "white" { }
    _RimColor ("Rim Color", Color) = (0.46,0,1,0)
    _RimPower ("Rim Power", Range(0.2,2)) = 0.5
    _Brightness ("Brightness", Range(0,3)) = 1
}
SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

    // ---------------------------------------------------------------------
    // Pass 1: Depth / ColorMask 0 primer (writes depth only)
    // ---------------------------------------------------------------------
    Pass {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ColorMask 0

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct appdata {
            float4 vertex : POSITION;
            fixed4 color  : COLOR;
        };

        struct v2f {
            float4 pos   : SV_POSITION;
            fixed4 color : COLOR0;
        };

        v2f vert (appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.color = saturate(v.color);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            return i.color;
        }
        ENDCG
    }

    // ---------------------------------------------------------------------
    // Pass 2: ForwardBase — greyscale diffuse + SH ambient + rim alpha
    // ---------------------------------------------------------------------
    Pass {
        Name "FORWARD"
        Tags { "LightMode"="ForwardBase" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fwdbase
        #include "UnityCG.cginc"
        #include "Lighting.cginc"

        sampler2D _MainTex;
        float4 _MainTex_ST;
        fixed4 _RimColor;
        float _RimPower;
        float _Brightness;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
        };

        struct v2f {
            float4 pos      : SV_POSITION;
            float2 uv       : TEXCOORD0;
            float3 worldNormal : TEXCOORD1;
            float3 worldPos : TEXCOORD2;
            // xlv_TEXCOORD3 (SH ambient) was computed but never read in fragment -> stripped
        };

        v2f vert (appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            float3 lightDir = _WorldSpaceLightPos0.xyz; // directional in base pass

            // Greyscale conversion of albedo
            fixed4 basecol = tex2D(_MainTex, i.uv);
            fixed3 grey = fixed3(dot(basecol.rgb, fixed3(0.3, 0.59, 0.11)),
                                  dot(basecol.rgb, fixed3(0.3, 0.59, 0.11)),
                                  dot(basecol.rgb, fixed3(0.3, 0.59, 0.11)));

            // Rim lighting
            float rim = 1.0 - saturate(dot(normalize(worldViewDir), i.worldNormal));
            float3 rimCol = _RimColor.rgb * pow(rim, _RimPower) * _Brightness;

            // Lambert diffuse with greyscale base
            float diff = max(0.0, dot(i.worldNormal, lightDir));
            fixed3 diffuse = grey * _LightColor0.rgb * diff;

            fixed4 c;
            c.rgb = diffuse + rimCol;
            c.a = (rimCol.x + rimCol.y + rimCol.z) / 3.0;
            return c;
        }
        ENDCG
    }

    // ---------------------------------------------------------------------
    // Pass 3: ForwardAdd — additive per-light with cookie / attenuation
    // ---------------------------------------------------------------------
    Pass {
        Name "FORWARD"
        Tags { "LightMode"="ForwardAdd" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
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
        float4 _MainTex_ST;
        fixed4 _RimColor;
        float _RimPower;
        float _Brightness;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
        };

        struct v2f {
            float4 pos         : SV_POSITION;
            float2 uv          : TEXCOORD0;
            float3 worldNormal : TEXCOORD1;
            float3 worldPos    : TEXCOORD2;
            LIGHTING_COORDS(3,4)
        };

        v2f vert (appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            TRANSFER_VERTEX_TO_FRAGMENT(o);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

            // Correct lightDir for directional vs point/spot
            float3 lightDir = (_WorldSpaceLightPos0.w == 0.0)
                ? _WorldSpaceLightPos0.xyz
                : normalize(_WorldSpaceLightPos0.xyz - i.worldPos);

            // Greyscale albedo
            fixed4 basecol = tex2D(_MainTex, i.uv);
            fixed3 grey = fixed3(dot(basecol.rgb, fixed3(0.3, 0.59, 0.11)),
                                  dot(basecol.rgb, fixed3(0.3, 0.59, 0.11)),
                                  dot(basecol.rgb, fixed3(0.3, 0.59, 0.11)));

            // Rim
            float rim = 1.0 - saturate(dot(normalize(worldViewDir), i.worldNormal));
            float3 rimCol = _RimColor.rgb * pow(rim, _RimPower) * _Brightness;

            // Attenuation (handles cookie + range + spot cone + point cubemap)
            UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

            float diff = max(0.0, dot(i.worldNormal, lightDir));
            fixed3 lightCol = _LightColor0.rgb * atten;
            fixed3 diffuse = grey * lightCol * diff;

            fixed4 c;
            c.rgb = diffuse;
            c.a = (rimCol.x + rimCol.y + rimCol.z) / 3.0;
            return c;
        }
        ENDCG
    }
}
Fallback "Diffuse"
}