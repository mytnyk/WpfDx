cbuffer TriangleConstants : register(b0)
{
    matrix View;
    matrix Projection;
    matrix Position;
    float4 Color;
}

struct VS_IN
{
    float4 pos : POSITION;
    float4 normal : NORMAL;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 col : COLOR;
};

PS_IN VertSh(VS_IN input)
{
    PS_IN output;

    output.pos = mul(Projection, mul(View, mul(Position, input.pos)));

    float3 normal = normalize(mul(View, mul(Position, input.normal)));
    float4 direction = float4(0, 0, -1, 0);
    float coef = abs(dot(normal, direction));
    output.col = float4(Color.xyz * coef, Color.w);

    return output;
}

float4 PixSh(PS_IN input) : SV_Target
{
    return input.col;
}

