//-----------------------------------------------------------------------------
// LogoShimmer.fx - Diagonal shimmer effect for logo
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
float Time;           // Current time for animation
float ShimmerPhase;   // 0-1 position of shimmer across texture

sampler2D TextureSampler : register(s0);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 texColor = tex2D(TextureSampler, input.TexCoord);

    // Vertical position (left to right)
    float diag = input.TexCoord.x;

    // Shimmer band position (-0.2 to 1.2 range for smooth entry/exit)
    float shimmerPos = ShimmerPhase * 1.4 - 0.2;

    // Distance from shimmer center
    float dist = abs(diag - shimmerPos);

    // Soft falloff (anti-aliased edges)
    float shimmerWidth = 0.08;
    float shimmer = 1.0 - smoothstep(0.0, shimmerWidth, dist);

    // Gold color overlay
    float3 goldTint = float3(0.7, 0.5, 0.2);

    // Add shimmer to original color (only where texture has alpha)
    float3 finalColor = texColor.rgb + goldTint * shimmer * 0.6 * texColor.a;

    return float4(finalColor, texColor.a) * input.Color;
}

technique BasicEffect
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
