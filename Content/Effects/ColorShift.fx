sampler s0;

float3 TargetColor = float3(1, 0.4, 0.7); // Pink color (default)
float BlueThreshold = 0.8;   // Threshold to identify blue
float ColorShiftAmount = 1; // How much to shift the color (0-1)

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{
    // Get original color
    float4 color = tex2D(s0, texCoord);
    
    // Check if the blue channel is significantly stronger than red and green
    if (color.b > BlueThreshold && color.b > color.r && color.b > color.g)
    {
        // Shift blue areas to pink, maintaining overall intensity
        float intensity = color.b;
        color.rgb = lerp(color.rgb, TargetColor * intensity, ColorShiftAmount);
    }
    
    return color;
}

technique ColorShiftEffect
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
