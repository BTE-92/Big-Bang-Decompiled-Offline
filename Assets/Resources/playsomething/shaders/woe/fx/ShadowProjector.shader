// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "WOE/Fx/ShadowProjector" {
    Properties {
        _ShadowTex ("Cookie", 2D) = "gray" { }
        _FalloffTex ("FallOff", 2D) = "white" { }
    }
    SubShader { 
        Tags { "Queue"="Transparent" }
        Pass {
            Tags { "Queue"="Transparent" }
            ZWrite Off
            Blend DstColor Zero
            ColorMask RGB
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
            };

            sampler2D _ShadowTex;
            sampler2D _FalloffTex;
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Projector matrices set automatically by Unity's Projector component
                o.uvShadow = mul(unity_Projector, v.vertex);
                o.uvFalloff = mul(unity_ProjectorClip, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Sample cookie and falloff textures using projected UV coordinates
                fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
                fixed4 texS_1 = fixed4(texS.rgb, 1.0 - texS.a);

                fixed falloff = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff)).a;

                // Linearly interpolate between unshadowed white (1,1,1,0) and cookie color based on falloff
                fixed4 col = lerp(fixed4(1.0, 1.0, 1.0, 0.0), texS_1, falloff);
                return col;
            }
            ENDCG
        }
    }
}