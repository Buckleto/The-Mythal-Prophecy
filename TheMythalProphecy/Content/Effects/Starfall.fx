//-----------------------------------------------------------------------------
// Starfall.fx - 3D starfield effect for title screen
// Creates stars flying towards the camera like traveling through space
//-----------------------------------------------------------------------------

#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Parameters
float Time;           // Elapsed time in seconds
float2 Resolution;    // Screen resolution
float Intensity;      // Effect intensity (0-1)

// Texture sampler (dummy texture for full-screen quad)
sampler2D TextureSampler : register(s0);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

// Pseudo-random function
float Hash(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.x + p3.y) * p3.z);
}

// Disc-shaped star (elliptical appearance)
float DiscStar(float2 uv, float2 center, float size, float seed)
{
    float2 diff = uv - center;

    // Correct for aspect ratio
    diff.x *= Resolution.x / Resolution.y;

    // Rotate based on seed for variety
    float angle = seed * 6.28;
    float cosA = cos(angle);
    float sinA = sin(angle);
    float2 rotated = float2(
        diff.x * cosA - diff.y * sinA,
        diff.x * sinA + diff.y * cosA
    );

    // Stretch one axis to create disc/ellipse appearance
    float stretch = 2.0 + seed * 1.5; // Stretch factor varies by star
    rotated.x *= stretch;

    float dist = length(rotated);

    // Twinkle with time-based modulation
    float twinkle = sin(Time * 2.0 + seed * 6.28) * 0.25 + 0.75;

    // Disc brightness - sharper falloff than regular stars
    float radius = size * 0.015;
    float brightness = smoothstep(radius, radius * 0.1, dist) * twinkle;

    // Subtle outer glow
    float glowRadius = radius * 2.0;
    float glow = smoothstep(glowRadius, radius, dist) * 0.2;
    brightness += glow * twinkle;

    return brightness;
}

// Star rendering with variable size
float Star(float2 uv, float2 center, float size, float seed, float isDisc)
{
    // Use disc rendering for ~25% of stars
    if (isDisc < 0.25)
    {
        return DiscStar(uv, center, size, seed);
    }

    float2 diff = uv - center;

    // Correct for aspect ratio
    diff.x *= Resolution.x / Resolution.y;

    float dist = length(diff);

    // Twinkle with time-based modulation
    float twinkle = sin(Time * 2.0 + seed * 6.28) * 0.25 + 0.75;

    // Star brightness falls off with distance, size affects radius
    float radius = size * 0.012;
    float brightness = smoothstep(radius, radius * 0.2, dist) * twinkle;

    // Add subtle glow for larger stars
    if (size > 0.5)
    {
        float glowRadius = radius * 2.5;
        float glow = smoothstep(glowRadius, radius, dist) * 0.3;
        brightness += glow * twinkle;
    }

    return brightness;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TexCoord;
    float2 screenCenter = float2(0.5, 0.5);

    // Accumulate starlight
    float starlight = 0.0;

    // Create multiple layers of stars distributed across entire screen
    for (int layer = 0; layer < 3; layer++)
    {
        float layerSeed = layer * 2.718;
        float speed = 0.07 + layer * 0.03; // Gentle speeds

        // Grid of stars across the screen
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                // Generate unique seeds for this star
                float seed = Hash(float2(i + layerSeed, j + layerSeed));
                float seed2 = Hash(float2(j + layerSeed + 0.5, i + layerSeed));
                float seed3 = Hash(float2(i * 0.7 + layerSeed, j * 0.3 + seed));
                float discChance = Hash(float2(seed + 0.123, seed2 * 0.456)); // Determines if disc-shaped

                // Base position: distribute across entire screen with randomness
                float baseX = (i + seed * 0.9) / 10.0;
                float baseY = (j + seed2 * 0.9) / 10.0;
                float2 basePos = float2(baseX, baseY);

                // Z-depth cycles from 0 (far) to 1 (near)
                float cycleOffset = seed3;
                float z = frac(Time * speed * (0.7 + seed * 0.6) + cycleOffset);

                // Slight outward drift from center as stars approach (subtle parallax)
                float2 toCenter = basePos - screenCenter;
                float parallaxStrength = z * z * 0.15; // Very subtle
                float2 starPos = basePos + toCenter * parallaxStrength;

                // Star size grows as it approaches camera
                float size = 0.15 + z * z * 0.85;

                // Fade: appear gradually, fade out as they pass by
                float fadeIn = smoothstep(0.0, 0.3, z);
                float fadeOut = 1.0 - smoothstep(0.8, 1.0, z);
                float fade = fadeIn * fadeOut;

                // Render star (some will be disc-shaped based on discChance)
                float star = Star(uv, starPos, size, seed, discChance);

                // Brightness increases as star gets closer
                float brightness = 0.4 + z * 0.6;
                starlight += star * fade * brightness;
            }
        }
    }

    // Apply intensity and clamp
    starlight *= Intensity;
    starlight = saturate(starlight);

    // Star color - cool white with slight blue tint
    float3 starColor = float3(0.95, 0.97, 1.0);

    return float4(starColor * starlight, starlight);
}

technique BasicEffect
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
