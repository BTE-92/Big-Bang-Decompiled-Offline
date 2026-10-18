// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'glstate_matrix_invtrans_modelview0' with 'UNITY_MATRIX_IT_MV'
// Upgrade NOTE: replaced 'glstate_matrix_modelview0' with 'UNITY_MATRIX_MV'
// Upgrade NOTE: replaced 'glstate_matrix_mvp' with 'UNITY_MATRIX_MVP'

Shader "WOE/Diffuse" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader { 
        Tags { 
            "LightMode"="Vertex" 
            "Queue"="Geometry" 
            "RenderType"="Opaque" 
        }
        Pass {
            Tags { 
                "LightMode"="Vertex" 
                "Queue"="Geometry" 
                "RenderType"="Opaque" 
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
                
                // Matches cb2[0..3] (glstate_matrix_mvp * v.vertex)
                o.vertex = UnityObjectToClipPos(float4(v.vertex.xyz, 1.0));
                
                // Matches cb0[6] (_MainTex_ST)
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                // CRITICAL FIX: Vector MUST be on the left to generate D3D11 instructions 2-4:
                // v1.x * cb2[8] + v1.y * cb2[9] + v1.z * cb2[10]
                float3 eyeNormal = normalize(mul(v.normal, (float3x3)UNITY_MATRIX_IT_MV));
                
                // View-space position (glstate_matrix_modelview0 * v.vertex)
                float3 eyePos = mul(UNITY_MATRIX_MV, float4(v.vertex.xyz, 1.0)).xyz;

                // Base ambient contribution (cb0[7] * cb3[0])
                float3 lcolor = _Color.rgb * glstate_lightmodel_ambient.rgb;

                #if defined(POINT) || defined(SPOT)
                    for (int i = 0; i < 8; i++) {
                        float3 dirToLight = unity_LightPosition[i].xyz - (eyePos * unity_LightPosition[i].w);
                        float distSq = dot(dirToLight, dirToLight);
                        
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
                    // Unrolled directional loop (Matches default D3D11/D3D9 assembly 0-63)
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
                // Matches ps_4_0 instruction 3: add o0.xyz, r0.xyzx, r0.xyzx ((tex * color) * 2.0)
                col.rgb = (tex.rgb * i.color.rgb) * 2.0;
                col.a = tex.a * i.color.a;
                
                return col;
            }
            ENDCG
        }
    }
}