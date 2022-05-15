#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 WVP;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput InstancingVS(in VertexShaderInput input, float posX : POSITION1, float posY : POSITION2)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 pos = float4(input.Position) + float4(posX,posY,0,0);
    pos = mul(pos, WVP);

    output.Position = pos;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 InstancingPS(VertexShaderOutput input) : COLOR0
{
    return float4(1,0,0,1);
}

technique Instancing
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL InstancingVS();
        PixelShader = compile PS_SHADERMODEL InstancingPS();
    }
}