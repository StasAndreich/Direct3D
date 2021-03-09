interface IVertexShaderType
{
	float4 GetVertex(float4 vertex);
	float4 GetColor(float4 vertex, float4 color);
};

class  VertexShaderBase : IVertexShaderType
{
	float4 vertex;

	float4 GetVertex(float4 vertex)
	{
		return vertex;
	}

	float4 GetColor(float4 vertex, float4 color)
	{
		return color;
	}
};

class VertexShaderTypePlato : IVertexShaderType
{
	float4 vertex;

	float4 GetVertex(float4 vertex);

	float4 GetColor(float4 vertex, float4 color);
};