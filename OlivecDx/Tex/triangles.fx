cbuffer TriangleConstants : register(b0)
{
    matrix View;
    matrix Projection;
    matrix Position;
}

struct VS_IN
{
    float4 pos : POSITION;
    float2 tex : TEXCOORD;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD;
};

Texture2D picture;
SamplerState pictureSampler;

PS_IN VertSh(VS_IN input)
{
    PS_IN output;

    output.pos = mul(Projection, mul(View, mul(Position, input.pos)));
    //float3 normal = normalize(mul(View, mul(Position, input.normal)));
    //float4 direction = float4(0, 0, -1, 0);
    //float coef = abs(dot(normal, direction));
    output.tex = input.tex;// *coef;

    return output;
}

float4 PixSh(PS_IN input) : SV_Target
{
  return picture.Sample(pictureSampler, input.tex);
}

