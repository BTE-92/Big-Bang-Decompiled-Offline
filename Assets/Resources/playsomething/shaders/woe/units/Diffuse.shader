// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'glstate_matrix_invtrans_modelview0' with 'UNITY_MATRIX_IT_MV'
// Upgrade NOTE: replaced 'glstate_matrix_modelview0' with 'UNITY_MATRIX_MV'
// Upgrade NOTE: replaced 'glstate_matrix_mvp' with 'UNITY_MATRIX_MVP'

Shader "WOE/Units/Diffuse" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader { 
        Tags { 
            "LightMode"="Vertex" 
            "Queue"="Geometry" 
            "RenderType"="WOEUnit" 
        }
        Pass {
            Tags { 
                "LightMode"="Vertex" 
                "Queue"="Geometry" 
                "RenderType"="WOEUnit" 
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ POINT SPOT
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                
                // Position clip transform (matches glstate_matrix_mvp)
                float4 pos = float4(v.vertex.xyz, 1.0);
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                // Transform normal to view-space (Vector-left multiplication ensures correct D3D/OpenGL assembly)
                float3 eyeNormal = normalize(mul(v.normal, (float3x3)UNITY_MATRIX_IT_MV));
                
                // Position transformed to view-space (glstate_matrix_modelview0 * pos)
                float3 eyePos = mul(UNITY_MATRIX_MV, pos).xyz;

                // Base ambient contribution
                float3 lcolor = _Color.rgb * glstate_lightmodel_ambient.rgb;

                #if defined(POINT) || defined(SPOT)
                    for (int i = 0; i < 8; i++) {
                        float3 dirToLight = unity_LightPosition[i].xyz - (eyePos * unity_LightPosition[i].w);
                        float distSq = dot(dirToLight, dirToLight);
                        
                        // Distance attenuation
                        float att = 1.0 / (1.0 + (unity_LightAtten[i].z * distSq));
                        if (unity_LightPosition[i].w != 0.0 && distSq > unity_LightAtten[i].w) {
                            att = 0.0;
                        }
                        
                        dirToLight *= rsqrt(distSq);

                        #if defined(SPOT)
                            float rho = max(dot(dirToLight, unity_SpotDirection[i].xyz), 0.0);
                            att *= saturate((rho - unity_LightAtten[i].x) * unity_LightAtten[i].y);
                        #endif

                        att *= 0.5;

                        float diff = max(dot(eyeNormal, dirToLight), 0.0);
                        lcolor += min(((diff * _Color.rgb) * unity_LightColor[i].rgb) * att, float3(1.0, 1.0, 1.0));
                    }
                #else
                    // Unrolled directional loop (Matches default directional subprogram)
                    for (int i = 0; i < 8; i++) {
                        float3 dirToLight = unity_LightPosition[i].xyz;
                        float diff = max(dot(eyeNormal, dirToLight), 0.0);
                        lcolor += min(((diff * _Color.rgb) * unity_LightColor[i].rgb) * 0.5, float3(1.0, 1.0, 1.0));
                    }
                #endif

                o.color = saturate(float4(lcolor, _Color.a));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                fixed4 col;
                // Matches exact 2.0x overbrightening calculation in original fragment GLSL
                col.rgb = (tex.rgb * i.color.rgb) * 2.0;
                col.a = tex.a * i.color.a;
                
                return col;
            }
            ENDCG
        }
    }
}