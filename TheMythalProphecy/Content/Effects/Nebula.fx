//-----------------------------------------------------------------------------
// Nebula.fx - Purple and golden mist/nebula effect
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
float Time;
float Intensity;

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

// Smooth noise
float Noise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    f = f * f * (3.0 - 2.0 * f);

    float a = Hash(i);
    float b = Hash(i + float2(1.0, 0.0));
    float c = Hash(i + float2(0.0, 1.0));
    float d = Hash(i + float2(1.0, 1.0));

    return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
}

// Fractal Brownian Motion for cloud-like patterns
float FBM(float2 p)
{
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;

    for (int i = 0; i < 5; i++)
    {
        value += amplitude * Noise(p * frequency);
        amplitude *= 0.5;
        frequency *= 2.0;
    }
    return value;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TexCoord;
    float drift = Time * 0.015;

    // Purple nebula layer - large sweeping clouds
    float2 purpleUV = uv * 2.5;
    float purple1 = FBM(purpleUV + float2(drift, drift * 0.5));
    float purple2 = FBM(purpleUV * 0.6 + float2(-drift * 0.7, drift * 0.3) + 3.0);
    float purpleNoise = (purple1 + purple2 * 0.7) * 0.6;

    // Golden mist layer - different movement pattern
    float2 goldUV = uv * 2.0;
    float gold1 = FBM(goldUV + float2(drift * 0.4, -drift * 0.6) + 7.0);
    float gold2 = FBM(goldUV * 0.5 + float2(drift * 0.5, drift * 0.2) + 12.0);
    float goldNoise = (gold1 + gold2 * 0.6) * 0.55;

    // Define colors - very dark purple
    float3 basePurple = float3(0.03, 0.01, 0.06);    // Nearly black purple base
    float3 deepPurple = float3(0.06, 0.015, 0.12);   // Very deep purple
    float3 midPurple = float3(0.12, 0.03, 0.22);     // Dark purple for highlights
    float3 gold = float3(0.7, 0.5, 0.2);
    float3 darkGold = float3(0.35, 0.25, 0.08);

    // Create variation in purple intensity
    float purpleVariation = smoothstep(0.3, 0.7, purpleNoise);
    float3 purpleColor = lerp(deepPurple, midPurple, purpleVariation);

    // Gold appears in wisps
    float goldVariation = smoothstep(0.4, 0.8, goldNoise);
    float3 goldColor = lerp(darkGold, gold, gold2) * goldVariation * 0.5;

    // Base layer covers everything - no holes
    float3 nebula = basePurple;

    // Add purple cloud variation on top
    nebula = lerp(nebula, purpleColor, purpleNoise * 0.8 + 0.2);

    // Add golden wisps
    nebula += goldColor;

    // Apply intensity
    nebula *= Intensity;

    // Lower opacity
    float alpha = 0.6 * Intensity;

    return float4(nebula, alpha);
}

technique BasicEffect
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
