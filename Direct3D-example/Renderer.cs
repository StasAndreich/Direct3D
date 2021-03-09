using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using Buffer11 = SharpDX.Direct3D11.Buffer;
using Device11 = SharpDX.Direct3D11.Device;

namespace Para_1
{
    partial class Renderer : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct VertexDataStruct
        {
            public Vector4 position;
            public Vector4 normal;
            public Vector4 color;
            public Vector2 texCoord;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PerFrameConstantBuffer
        {
            public float time;
            public Vector3 _padding;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PerObjectConstantBuffer
        {
            public Matrix worldViewProjectionMatrix;
            public Matrix worldMatrix;
            public Matrix inverseTransposeWorldMatrix;
            public int timeScaling;
            public Vector3 _padding;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MaterialProperties
        {
            public Vector3 emmisiveK;
            public float _padding0;
            public Vector3 ambientK;
            public float _padding1;
            public Vector3 diffuseK;
            public float _padding2;
            public Vector3 specularK;
            public float specularPower;
        }

        private DirectX3DGraphics _directX3DGraphics;
        private Device11 _device;
        private DeviceContext _deviceContext;

        private VertexShader _vertexShader;
        private PixelShader _pixelShader;
        private ShaderSignature _shaderSignature;
        private InputLayout _inputLayout;

        private PerFrameConstantBuffer _perFrameConstantBuffer;
        private Buffer11 _perFrameConstantBufferObject;

        private PerObjectConstantBuffer _perObjectConstantBuffer;
        private Buffer11 _perObjectConstantBufferObject;

        private Buffer11 _materialPropertiesConstantBufferObject;

        private Material _currentMaterial = null;
        private Texture _currentTexture = null;


        private SamplerState _anisotropicSampler;
        public SamplerState AnisotropicSampler { get => _anisotropicSampler; }

        private SamplerState _linearSampler;
        public SamplerState LinearSampler { get => _linearSampler; }

        private SamplerState _pointSampler;
        public SamplerState PointSampler { get => _pointSampler; }

        private ClassLinkage _pixelShaderClassLinkage;
        private ClassLinkage _vertexShaderClassLinkage;

        private RasterizerState _solidRasterizerState;
        private RasterizerState _wireframeRasterizerState;



        public Renderer(DirectX3DGraphics directX3DGraphics)
        {
            _directX3DGraphics = directX3DGraphics;
            _device = _directX3DGraphics.Device;
            _deviceContext = _directX3DGraphics.DeviceContext;

            _pixelShaderClassLinkage = new ClassLinkage(_device);
            _vertexShaderClassLinkage = new ClassLinkage(_device);

            CompilationResult vertexShaderByteCode =
                ShaderBytecode.CompileFromFile("vertex.hlsl", "vertexShader", "vs_5_0", ShaderFlags.None, EffectFlags.None, null, new IncludeHandler());

            _vertexShader = new VertexShader(_device, vertexShaderByteCode, _vertexShaderClassLinkage);
            Utilities.Dispose(ref _vertexShaderClassLinkage);

            CompilationResult pixelShaderByteCode =
                ShaderBytecode.CompileFromFile("pixel.hlsl", "pixelShader", "ps_5_0", ShaderFlags.EnableStrictness | ShaderFlags.SkipOptimization | ShaderFlags.Debug, EffectFlags.None, null, new IncludeHandler());

            _pixelShader = new PixelShader(_device, pixelShaderByteCode, _pixelShaderClassLinkage);

            InitializeIllumination(pixelShaderByteCode);


            InputElement[] inputElements = new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 48, 0)
            };

            _shaderSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);

            _inputLayout = new InputLayout(_device, _shaderSignature, inputElements);

            Utilities.Dispose(ref vertexShaderByteCode);
            Utilities.Dispose(ref pixelShaderByteCode);

            _deviceContext.InputAssembler.InputLayout = _inputLayout;

            RasterizerStateDescription _rasterizerStateDescription = new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid, // Какркас или солид
                CullMode = CullMode.Back, // Какую часть отбрасывать
                IsFrontCounterClockwise = true,
                IsMultisampleEnabled = true,
                IsAntialiasedLineEnabled = true,
                IsDepthClipEnabled = true

            };

            _solidRasterizerState = new RasterizerState(_device, _rasterizerStateDescription);

            _rasterizerStateDescription = new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid, // Какркас или солид
                CullMode = CullMode.None, // Какую часть отбрасывать
                IsFrontCounterClockwise = true,
                IsMultisampleEnabled = true,
                IsAntialiasedLineEnabled = true,
                IsDepthClipEnabled = true

            };

            _wireframeRasterizerState = new RasterizerState(_device, _rasterizerStateDescription);

            SamplerStateDescription samplerStateDescription;

            samplerStateDescription =
                new SamplerStateDescription
                {
                    Filter = Filter.Anisotropic,
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp,
                    AddressW = TextureAddressMode.Clamp,
                    MipLodBias = 0,
                    MaximumAnisotropy = 16,
                    ComparisonFunction = Comparison.Never,
                    BorderColor = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 1.0f, 1.0f, 1.0f),
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                };

            _anisotropicSampler = new SamplerState(_directX3DGraphics.Device, samplerStateDescription);

            samplerStateDescription.Filter = Filter.MinMagMipLinear;
            _linearSampler = new SamplerState(_directX3DGraphics.Device, samplerStateDescription);

            samplerStateDescription.Filter = Filter.MinMagMipLinear;
            _pointSampler = new SamplerState(_directX3DGraphics.Device, samplerStateDescription);
        }

        public void CreateConstantBuffers()
        {
            _perFrameConstantBufferObject = new Buffer11(
                _device,
                Utilities.SizeOf<PerFrameConstantBuffer>(),
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);

            _perObjectConstantBufferObject = new Buffer11(
                _device,
                Utilities.SizeOf<PerObjectConstantBuffer>(),
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);

            _materialPropertiesConstantBufferObject = new Buffer11(
                _device,
                Utilities.SizeOf<MaterialProperties>(),
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);

            CreateIlluminationConstantBuffer();
        }

        public void BeginRender()
        {
            _directX3DGraphics.ClearBuffers(Color.Black);
        }

        public void UpdatePerFrameConstantBuffers(float time)
        {
            _perFrameConstantBuffer.time = time;

            DataStream dataStream;
            _deviceContext.MapSubresource(
                _perFrameConstantBufferObject,
                MapMode.WriteDiscard,
                SharpDX.Direct3D11.MapFlags.None,
                out dataStream);
            dataStream.Write(_perFrameConstantBuffer);
            _deviceContext.UnmapSubresource(_perFrameConstantBufferObject, 0);
            _deviceContext.VertexShader.SetConstantBuffer(0, _perFrameConstantBufferObject);
        }

        public void UpdatePerObjectConstantBuffers(Matrix world, Matrix view, Matrix projection, int timeScaling, ObjectType type)
        {

            _perObjectConstantBuffer.worldViewProjectionMatrix = Matrix.Multiply(Matrix.Multiply(world, view), projection);
            _perObjectConstantBuffer.worldViewProjectionMatrix.Transpose();
            _perObjectConstantBuffer.worldMatrix = world;
            _perObjectConstantBuffer.worldMatrix.Transpose();

            _perObjectConstantBuffer.inverseTransposeWorldMatrix = Matrix.Invert(world);

            _perObjectConstantBuffer.timeScaling = timeScaling;

            _perObjectConstantBuffer.worldMatrix = world;


            DataStream dataStream;
            _deviceContext.MapSubresource(
                _perObjectConstantBufferObject,
                MapMode.WriteDiscard,
                SharpDX.Direct3D11.MapFlags.None,
                out dataStream);
            dataStream.Write(_perObjectConstantBuffer);
            _deviceContext.UnmapSubresource(_perObjectConstantBufferObject, 0);
            _deviceContext.VertexShader.SetConstantBuffer(1, _perObjectConstantBufferObject);

        }

        public void SetTexture(Texture texture)
        {
            if(_currentTexture != texture && texture != null)
            {
                _deviceContext.PixelShader.SetShaderResource(0, texture.ShaderResourceView);
                _deviceContext.PixelShader.SetSampler(0, texture.SamplerState);

                _currentTexture = texture;
            }
        }

        public void SetMaterial(Material material)
        {
            if (_currentMaterial != material)
            {
                SetTexture(material.Texture);

                DataStream dataStream;
                _deviceContext.MapSubresource(
                    _materialPropertiesConstantBufferObject,
                    MapMode.WriteDiscard,
                    SharpDX.Direct3D11.MapFlags.None,
                    out dataStream);
                dataStream.Write(material.MaterialProperties);
                _deviceContext.UnmapSubresource(_materialPropertiesConstantBufferObject, 0);
                _deviceContext.PixelShader.SetConstantBuffer(0, _materialPropertiesConstantBufferObject);

                _currentMaterial = material;
            }
        }

        public void RenderMeshObject(MeshObject meshObject)
        {
            SetMaterial(meshObject.Material);

            _deviceContext.InputAssembler.PrimitiveTopology = meshObject.PrimitiveTopology;

            if(meshObject.FillModeSolid)
                _deviceContext.Rasterizer.State = _solidRasterizerState;
            else
                _deviceContext.Rasterizer.State = _wireframeRasterizerState;

            _deviceContext.InputAssembler.SetVertexBuffers(0, meshObject.VertexBufferBinding);
            _deviceContext.InputAssembler.SetIndexBuffer(meshObject.IndicesBufferObject, Format.R32_UInt, 0);

            _deviceContext.VertexShader.Set(_vertexShader);
            _deviceContext.PixelShader.Set(_pixelShader, _lightInterfaces);

            _deviceContext.DrawIndexed(meshObject.IndicesCount, 0, 0);
        }


        public void EndRender()
        {
            _directX3DGraphics.SwapChain.Present(1, PresentFlags.Restart);
        }


        public void Dispose()
        {
            Utilities.Dispose(ref _materialPropertiesConstantBufferObject);
            Utilities.Dispose(ref _perObjectConstantBufferObject);
            Utilities.Dispose(ref _perFrameConstantBufferObject);
            Utilities.Dispose(ref _linearSampler);
            Utilities.Dispose(ref _anisotropicSampler);
            Utilities.Dispose(ref _inputLayout);
            Utilities.Dispose(ref _shaderSignature);
            DisposeIllumination();
            Utilities.Dispose(ref _pixelShader);
            Utilities.Dispose(ref _vertexShader);
            Utilities.Dispose(ref _pixelShaderClassLinkage);
        }
    }
}
