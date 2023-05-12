Shader "Custom/WindowGridShader" {
    Properties {
        _LitWindowColor ("Lit Window Color", Color) = (1, 1, 1, 1)
        _UnlitWindowColor ("Unlit Window Color", Color) = (0, 0, 0, 1)
        _WindowSize ("Window Size", Range(0, 1)) = 0.1
        _WindowSpacing ("Window Spacing", Range(0, .5)) = 0.02
        _LitWindowProbability ("Lit Window Probability", Range(0, 1)) = 0.2
        _WindowAspectRatio ("Window Aspect Ratio", Range(0, 1)) = 0.5
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1.0
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        float _WindowSize;
        float _WindowSpacing;
        float _LitWindowProbability;
        float _WindowAspectRatio;
        float4 _LitWindowColor;
        float4 _UnlitWindowColor;
        float _EmissionIntensity;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // Simple pseudo-random number generator
        float rand(float3 co) {
            return frac(sin(dot(co, float3(12.9898, 78.233, 37.113))) * 43758.5453);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            // Calculate the cell coordinates
            float3 cellCoord = floor(IN.worldPos / (float3(_WindowSize * _WindowAspectRatio, _WindowSize, _WindowSize * _WindowAspectRatio) + _WindowSpacing));

            // Use the cell coordinates for window lighting randomness
            float windowRand = rand(cellCoord);
            float litWindow = step(1 - _LitWindowProbability, windowRand);

            // Set the color for lit and unlit windows
            float3 windowColor = litWindow * _LitWindowColor.rgb + (1 - litWindow) * _UnlitWindowColor.rgb;

            o.Albedo = windowColor;

            // If the window is lit, use the lit window color for emission
            if (litWindow > 0.5) {
                o.Emission = _LitWindowColor.rgb * _EmissionIntensity;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
