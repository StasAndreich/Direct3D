interface IMaterial
{
	float4 GetColor(float4 color, float2 texCoord);
};

class  CColoredMateral : IMaterial
{
	float4 color;

	float4 GetColor(float4 color, float2 texCoord)
	{
		return color;
	}
};

class CTexturedMaterial : IMaterial
{
	float4 color;

	float4 GetColor(float4 color, float2 texCoord);
};
