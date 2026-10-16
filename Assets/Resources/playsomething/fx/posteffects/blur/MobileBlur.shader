Shader "Hidden/FastBlur" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Bloom ("Bloom (RGB)", 2D) = "black" { }
    }
    SubShader {
        // --- PASS 0: 4-Tap Box Filter (Downsample) ---
        Pass {
            ZTest False
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv + _MainTex_TexelSize.xy;
                o.uv1 = v.uv + (_MainTex_TexelSize.xy * float2(-0.5, -0.5));
                o.uv2 = v.uv + (_MainTex_TexelSize.xy * float2(0.5, -0.5));
                o.uv3 = v.uv + (_MainTex_TexelSize.xy * float2(-0.5, 0.5));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 col = tex2D(_MainTex, i.uv0);
                col += tex2D(_MainTex, i.uv1);
                col += tex2D(_MainTex, i.uv2);
                col += tex2D(_MainTex, i.uv3);
                return col * 0.25;
            }
            ENDCG
        }

        // --- PASS 1: Vertical 7-Tap Gaussian Blur (Sequential) ---
        Pass {
            ZTest Always
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 offset : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Parameter;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.offset = (_MainTex_TexelSize.xy * float2(0.0, 1.0)) * _Parameter.x;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 coords = i.uv - (i.offset * 3.0);
                
                half4 col = tex2D(_MainTex, coords) * half4(0.0205, 0.0205, 0.0205, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.0855, 0.0855, 0.0855, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.232, 0.232, 0.232, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.324, 0.324, 0.324, 1.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.232, 0.232, 0.232, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.0855, 0.0855, 0.0855, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.0205, 0.0205, 0.0205, 0.0);
                
                return col;
            }
            ENDCG
        }

        // --- PASS 2: Horizontal 7-Tap Gaussian Blur (Sequential) ---
        Pass {
            ZTest Always
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 offset : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Parameter;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.offset = (_MainTex_TexelSize.xy * float2(1.0, 0.0)) * _Parameter.x;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 coords = i.uv - (i.offset * 3.0);
                
                half4 col = tex2D(_MainTex, coords) * half4(0.0205, 0.0205, 0.0205, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.0855, 0.0855, 0.0855, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.232, 0.232, 0.232, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.324, 0.324, 0.324, 1.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.232, 0.232, 0.232, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.0855, 0.0855, 0.0855, 0.0);
                coords += i.offset;
                col += tex2D(_MainTex, coords) * half4(0.0205, 0.0205, 0.0205, 0.0);
                
                return col;
            }
            ENDCG
        }

        // --- PASS 3: Vertical 7-Tap Gaussian Blur (Optimized Paired Lookups) ---
        Pass {
            ZTest Always
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                float4 uv3 : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Parameter;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                float2 step = (_MainTex_TexelSize.xy * float2(0.0, 1.0)) * _Parameter.x;
                float4 offset3 = -step.xyxy * 3.0;
                float4 offset2 = offset3 + step.xyxy;
                float4 offset1 = offset2 + step.xyxy;

                o.uv1 = v.uv.xyxy + (offset3 * float4(1.0, 1.0, -1.0, -1.0));
                o.uv2 = v.uv.xyxy + (offset2 * float4(1.0, 1.0, -1.0, -1.0));
                o.uv3 = v.uv.xyxy + (offset1 * float4(1.0, 1.0, -1.0, -1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 col = tex2D(_MainTex, i.uv) * half4(0.324, 0.324, 0.324, 1.0);
                col += (tex2D(_MainTex, i.uv1.xy) + tex2D(_MainTex, i.uv1.zw)) * half4(0.0205, 0.0205, 0.0205, 0.0);
                col += (tex2D(_MainTex, i.uv2.xy) + tex2D(_MainTex, i.uv2.zw)) * half4(0.0855, 0.0855, 0.0855, 0.0);
                col += (tex2D(_MainTex, i.uv3.xy) + tex2D(_MainTex, i.uv3.zw)) * half4(0.232, 0.232, 0.232, 0.0);
                return col;
            }
            ENDCG
        }

        // --- PASS 4: Horizontal 7-Tap Gaussian Blur (Optimized Paired Lookups) ---
        Pass {
            ZTest Always
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                float4 uv3 : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Parameter;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                float2 step = (_MainTex_TexelSize.xy * float2(1.0, 0.0)) * _Parameter.x;
                float4 offset3 = -step.xyxy * 3.0;
                float4 offset2 = offset3 + step.xyxy;
                float4 offset1 = offset2 + step.xyxy;

                o.uv1 = v.uv.xyxy + (offset3 * float4(1.0, 1.0, -1.0, -1.0));
                o.uv2 = v.uv.xyxy + (offset2 * float4(1.0, 1.0, -1.0, -1.0));
                o.uv3 = v.uv.xyxy + (offset1 * float4(1.0, 1.0, -1.0, -1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 col = tex2D(_MainTex, i.uv) * half4(0.324, 0.324, 0.324, 1.0);
                col += (tex2D(_MainTex, i.uv1.xy) + tex2D(_MainTex, i.uv1.zw)) * half4(0.0205, 0.0205, 0.0205, 0.0);
                col += (tex2D(_MainTex, i.uv2.xy) + tex2D(_MainTex, i.uv2.zw)) * half4(0.0855, 0.0855, 0.0855, 0.0);
                col += (tex2D(_MainTex, i.uv3.xy) + tex2D(_MainTex, i.uv3.zw)) * half4(0.232, 0.232, 0.232, 0.0);
                return col;
            }
            ENDCG
        }
    }
    Fallback Off
}