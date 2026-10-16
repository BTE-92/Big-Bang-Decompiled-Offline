Shader "SpaceUnity/PlanetAtmosphere" { //Blah Blah Blah
    Properties {
        _AtmoColor ("Atmosphere Color", Color) = (0.5,0.5,1,1)
        _Size ("Size", Float) = 0.1
        _Falloff ("Falloff", Float) = 5
        _Transparency ("Transparency", Float) = 15
    }
    SubShader {
        Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }

        Pass {
            Name "ATMOSPHEREBASE"
            Tags { "LIGHTMODE"="Always" }
            Cull Front
            ZWrite Off
            Blend SrcAlpha One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform fixed4 _AtmoColor;
            uniform float _Size;
            uniform float _Falloff;
            uniform float _Transparency;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v) {
                v2f o;
                // Extrude vertex along normal by _Size
                float4 extrudedPos = float4(v.vertex.xyz + v.normal * _Size, v.vertex.w);
                o.pos = UnityObjectToClipPos(extrudedPos);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, extrudedPos).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);

                // Backface Rim Calculation
                float NdotV = saturate(dot(viewDir, worldNormal));
                float alpha = pow(NdotV, _Falloff) * _Transparency * _AtmoColor.a;

                return fixed4(_AtmoColor.rgb, saturate(alpha));
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}