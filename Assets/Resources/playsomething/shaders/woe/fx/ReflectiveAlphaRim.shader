Shader "WOE/Fx/ReflectiveAlphaRim" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range(0.03, 1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" { }
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _Cube ("Cubemap", CUBE) = "" { }
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB

        CGPROGRAM
        #pragma surface surf BlinnPhong alpha:blend
        #pragma target 2.0

        // ==========================================
        // EXPLICIT CG VARIABLE DEFINITIONS
        // ==========================================
        sampler2D _MainTex;
        samplerCUBE _Cube;
        fixed4 _Color;
        fixed4 _RimColor;
        float _RimPower;

        // These two MUST be explicitly defined here so the compiler 
        // links the ShaderLab properties to the CG/HLSL program:
        half _Shininess;     // Defines the specular exponent (surface smoothness)
        // ==========================================

        struct Input {
            float2 uv_MainTex;
            float3 worldRefl;   
            float3 viewDir;     
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 cube = texCUBE(_Cube, IN.worldRefl);
            
            o.Albedo = (tex.rgb * _Color.rgb) + cube.rgb;
            
            // Assigns defined variables to the surface output
            o.Gloss = tex.a;
            o.Specular = _Shininess; 
            
            half rim = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _RimPower);
            o.Emission = _RimColor.rgb * rim;
            o.Alpha = (rim + cube.r);
        }
        ENDCG
    }

    Fallback "Specular"
}