float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

float4 xNorthColor;
float4 xSouthColor;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float Polarity  : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float Polarity  : TEXCOORD0;
};





VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, xWorld);
    float4 viewPosition = mul(worldPosition, xView);
    output.Position = mul(viewPosition, xProjection);
	output.Polarity = input.Polarity;
    return output;
}

static const float PI = 3.14159265f;
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float interpValue = pow(sin(input.Polarity*(PI/2.0)), 0.5) * sign(input.Polarity);
	interpValue = interpValue/2.0 + 0.5; 
    return lerp(xSouthColor, xNorthColor, interpValue);
}





technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
