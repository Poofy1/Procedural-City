Shader "Custom/WindowGridShader" {
    Properties {
        _LitWindowColor ("Lit Window Color", Color) = (1, 1, 1, 1)
        _UnlitWindowColor ("Unlit Window Color", Color) = (0, 0, 0, 1)
        _WindowSize ("Window Size", Range(0, 1)) = 0.1
        _WindowSpacing ("Window Spacing", Range(0, .5)) = 0.02
        _LitWindowProbability ("Lit Window Probability", Range(0, 1)) = 0.2
        _AntialiasingAmount ("Antialiasing Amount", Range(0, 20)) = 1
        _WindowAspectRatio ("Window Aspect Ratio", Range(0, 1)) = 0.5
        _EmissionIntensity ("Emission Intensity", Range(0, 1)) = 1.0
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
        float _AntialiasingAmount;
        float _EmissionIntensity;

        struct Input {
            float2 uv_MainTex;
        };

        // Simple pseudo-random number generator
        float rand(float2 co) {
            return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            float2 uv = IN.uv_MainTex;
            float2 gridPos = fmod(uv, float2(_WindowSize * _WindowAspectRatio, _WindowSize) + _WindowSpacing);

            // Calculate antialiasing width based on distance
            float aaWidth = fwidth(uv) * _WindowSpacing * _AntialiasingAmount * 0.5;

            // Apply antialiasing using smoothstep function
            float maskX = smoothstep(_WindowSpacing - aaWidth, _WindowSpacing, gridPos.x) *
                          (1.0 - smoothstep(_WindowSize * _WindowAspectRatio, _WindowSize * _WindowAspectRatio + aaWidth, gridPos.x));
            float maskY = smoothstep(_WindowSpacing - aaWidth, _WindowSpacing, gridPos.y) *
                          (1.0 - smoothstep(_WindowSize, _WindowSize + aaWidth, gridPos.y));
            float mask = maskX * maskY;

            // Calculate the cell coordinates
            float2 cellCoord = floor(uv / (float2(_WindowSize * _WindowAspectRatio, _WindowSize) + _WindowSpacing));

            // Use the cell coordinates for window lighting randomness
            float windowRand = rand(cellCoord);
            float litWindow = step(1 - _LitWindowProbability, windowRand);

            // Set the color for lit and unlit windows
            float3 windowColor = mask * (litWindow * _LitWindowColor.rgb + (1 - litWindow) * _UnlitWindowColor.rgb);

            o.Albedo = windowColor;
            o.Alpha = mask;

            // If the window is lit, use the lit window color for emission
            if (litWindow > 0.5) {
                o.Emission = _LitWindowColor.rgb * _EmissionIntensity;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
