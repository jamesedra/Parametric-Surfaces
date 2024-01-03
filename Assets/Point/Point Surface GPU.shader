Shader "Custom/Point Surface GPU"
{
    Properties
    {
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        // sync compilation makes the shader compile everytime
        #pragma editor_sync_compilation 
        #pragma target 4.5

        #include "PointGPU.hlsl"

        struct Input
        {
            float3 worldPos;
        };

        float _Smoothness;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo.rgb = saturate(IN.worldPos.xyz * 0.5 + 0.5);
            o.Smoothness = _Smoothness;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
