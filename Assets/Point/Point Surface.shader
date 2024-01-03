Shader "Custom/Point Surface"
{
    Properties
    {
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
        };

        float _Smoothness;


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo.rg = IN.worldPos.xy * 0.5 + 0.5;
            o.Smoothness = _Smoothness;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
