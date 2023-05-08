Shader "Custom/WindowGridShader" {
    Properties {
        _LitWindowColor ("Lit Window Color", Color) = (1, 1, 1, 1)
        _UnlitWindowColor ("Unlit Window Color", Color) = (0, 0, 0, 1)
        _WindowSize ("Window Size", Range(0, 1)) = 0.1
        _WindowSpacing ("Window Spacing", Range(0, .1)) = 0.02
        _LitWindowProbability ("Lit Window Probability", Range(0, 1)) = 0.2
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _WindowSize;
        float _WindowSpacing;
        float _LitWindowProbability;
        float4 _LitWindowColor;
        float4 _UnlitWindowColor;

        struct Input {
            float2 uv_MainTex;
        };

        // Simple pseudo-random number generator
        float rand(float2 co) {
            return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            float2 uv = IN.uv_MainTex;
            float2 gridPos = fmod(uv, _WindowSize + _WindowSpacing);
            float mask = step(_WindowSpacing, gridPos.x) * step(_WindowSpacing, gridPos.y);

            // Calculate the cell coordinates
            float2 cellCoord = floor(uv / (_WindowSize + _WindowSpacing));

            // Use the cell coordinates for window lighting randomness
            float windowRand = rand(cellCoord);
            float litWindow = step(1 - _LitWindowProbability, windowRand);

            // Set the color for lit and unlit windows
            o.Albedo = mask * (litWindow * _LitWindowColor.rgb + (1 - litWindow) * _UnlitWindowColor.rgb);
            o.Alpha = mask;
        }
        ENDCG
    }
    FallBack "Diffuse"
}