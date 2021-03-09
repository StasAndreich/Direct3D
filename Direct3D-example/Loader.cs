using System;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.WIC;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace Para_1
{
    class Loader : IDisposable
    {
        private DirectX3DGraphics _directX3DGraphics;
        private ImagingFactory _imagingFactory;

        private SampleDescription _sampleDescription = new SampleDescription(1, 0);

        public Loader(DirectX3DGraphics directX3DGraphics)
        {
            _directX3DGraphics = directX3DGraphics;
            _imagingFactory = new ImagingFactory();
        }

        public MeshObject MakeMesh(Vector4 position, Material material)
        {
            Renderer.VertexDataStruct[] vertices = new Renderer.VertexDataStruct[441];

            int index = 0;

            for (float i = -10; i < 11; i++)
            {
                for (float j = -10; j < 11; j++)
                {
                    vertices[index++] = new Renderer.VertexDataStruct
                    {
                        position = new Vector4(i, 0.0f, j, 1.0f),
                        normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        color = new Vector4(0.9f, 0.9f, 0.9f, 1.0f)
                    };
                }
            }

            uint[] indices = new uint[400 * 6];

            uint pos = 0;

            index = 0;

            for (uint i = 0; i < 20; i++)
            {
                for (uint j = 0; j < 20; j++)
                {
                    pos = i + j * 21;
                    indices[index++] = pos;
                    indices[index++] = pos + 21;
                    indices[index++] = pos + 1;
                    indices[index++] = pos + 1;
                    indices[index++] = pos + 21;
                    indices[index++] = pos + 22;
                }
            }

            return new MeshObject(_directX3DGraphics, position,
                0, 0, 0, vertices, indices, PrimitiveTopology.TriangleList, material, true, ObjectType.Plato, false);
        }

        public MeshObject MakeCylinder(Vector4 position, Material material)
        {
            Renderer.VertexDataStruct[] vertices = new Renderer.VertexDataStruct[444];

            double x = 2;
            double y = 0;

            for (int i = 0; i < 111; i += 3)
            {
                vertices[i] = new Renderer.VertexDataStruct
                {
                    position = new Vector4(0.0f, 0.0f, 2.0f, 1.0f),
                    texCoord = new Vector2(0.5f, 0.25f)
                };
                vertices[i + 2] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, 2.0f, 1.0f),
                    texCoord = new Vector2(((float)x + 2) / 4, ((float)y + 2) / 8)
                };
                Rotation(ref x, ref y, 0.1745533);
                vertices[i + 1] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, 2.0f, 1.0f),
                    texCoord = new Vector2(((float)x + 2) / 4, ((float)y + 2) / 8)
                };
            }

            x = 2;
            y = 0;
            for (int i = 111; i < 333; i += 6)
            {
                vertices[i] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, 2.0f, 1.0f),
                    texCoord = new Vector2((float)(i - 111) / (6 * 37), 0.5f)
                };
                vertices[i + 2] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, -2.0f, 1.0f),
                    texCoord = new Vector2((float)(i - 111) / (6 * 37), 1.0f)
                };
                Rotation(ref x, ref y, 0.1745533);
                vertices[i + 1] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, -2.0f, 1.0f),
                    texCoord = new Vector2((float)(i - 105) / (6 * 37), 1.0f)
                };

                vertices[i + 3] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, -2.0f, 1.0f),
                    texCoord = new Vector2((float)(i - 105) / (6 * 37), 1.0f)
                };
                vertices[i + 5] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, 2.0f, 1.0f),
                    texCoord = new Vector2((float)(i - 105) / (6 * 37), 0.5f)
                };
                Rotation(ref x, ref y, -0.1745533);
                vertices[i + 4] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, 2.0f, 1.0f),
                    texCoord = new Vector2((float)(i - 111) / (6 * 37), 0.5f)
                };

                Rotation(ref x, ref y, 0.1745533);
            }



            x = 2;
            y = 0;
            for (int i = 333; i < 444; i += 3)
            {
                vertices[i] = new Renderer.VertexDataStruct
                {
                    position = new Vector4(0.0f, 0.0f, -2.0f, 1.0f),
                    texCoord = new Vector2(0.5f, 0.25f)
                };
                vertices[i + 1] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, -2.0f, 1.0f),
                    texCoord = new Vector2(((float)x + 2) / 4, ((float)y + 2) / 8)
                };
                Rotation(ref x, ref y, 0.1745533);
                vertices[i + 2] = new Renderer.VertexDataStruct
                {
                    position = new Vector4((float)x, (float)y, -2.0f, 1.0f),
                    texCoord = new Vector2(((float)x + 2) / 4, ((float)y + 2) / 8)
                };
            }

            uint[] indices = new uint[444];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = (UInt32)i;
            }

            return new MeshObject(_directX3DGraphics, position,
                0, 0, 0, vertices, indices, PrimitiveTopology.TriangleList, material, true, ObjectType.Normal);
        }

        private void Rotation(ref double x, ref double y, double angle)
        {
            double newX = x * Math.Cos(angle) - y * Math.Sin(angle);
            double newY = x * Math.Sin(angle) + y * Math.Cos(angle);
            x = newX;
            y = newY;
        }


        public Texture LoadTextureFromFile(string fileName, SamplerState samplerState, bool generateMips, int mipLevels = -1)
        {
            BitmapDecoder decoder = new BitmapDecoder(_imagingFactory,
                fileName, DecodeOptions.CacheOnDemand);
            BitmapFrameDecode bitmapFirstFrame = decoder.GetFrame(0);

            Utilities.Dispose(ref decoder);

            FormatConverter formatConverter = new FormatConverter(_imagingFactory);
            formatConverter.Initialize(bitmapFirstFrame, PixelFormat.Format32bppRGBA,
                BitmapDitherType.None, null, 0.0f, BitmapPaletteType.Custom);

            int stride = formatConverter.Size.Width * 4;
            DataStream buffer = new DataStream(
                formatConverter.Size.Height * stride, true, true);
            formatConverter.CopyPixels(stride, buffer);

            int width = formatConverter.Size.Width;
            int height = formatConverter.Size.Height;

            Texture2DDescription texture2DDescription = new Texture2DDescription()
            {
                Width = width,
                Height = height,
                MipLevels = (generateMips ? 0 : 1),
                ArraySize = 1,
                Format = Format.R8G8B8A8_UNorm,
                SampleDescription = _sampleDescription,
                Usage = ResourceUsage.Default,
                BindFlags = (
                    generateMips ?
                    BindFlags.ShaderResource | BindFlags.RenderTarget :
                    BindFlags.ShaderResource
                    ),
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = (
                   generateMips ?
                   ResourceOptionFlags.GenerateMipMaps:
                   ResourceOptionFlags.None
                   )
            };

            Texture2D textureObject;
                
            if(generateMips)
                textureObject = new Texture2D(_directX3DGraphics.Device, texture2DDescription);
            else
            {
                DataRectangle dataRectangle = new DataRectangle(buffer.DataPointer, stride);
                textureObject = new Texture2D(_directX3DGraphics.Device, texture2DDescription, dataRectangle);
            }

            ShaderResourceViewDescription shaderResourceViewDescription =
                new ShaderResourceViewDescription()
                {
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Format = Format.R8G8B8A8_UNorm,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource
                    {
                        MostDetailedMip = 0,
                        MipLevels = (generateMips ? mipLevels : 1)
                    }
                };
            ShaderResourceView shaderResourceView =
                new ShaderResourceView(_directX3DGraphics.Device, textureObject, shaderResourceViewDescription);

            if(generateMips)
            {
                DataBox dataBox = new DataBox(buffer.DataPointer, stride, 1);
                _directX3DGraphics.DeviceContext.UpdateSubresource(dataBox, textureObject, 0);
                _directX3DGraphics.DeviceContext.GenerateMips(shaderResourceView);
            }

            Utilities.Dispose(ref formatConverter);

            return new Texture(textureObject, shaderResourceView, width, height, samplerState);
        }

        public MeshObject MakeAxes(Material material, bool visible)
        {
            Renderer.VertexDataStruct[] vertices =
                new Renderer.VertexDataStruct[6]
                {
                    new Renderer.VertexDataStruct // x
                    {
                        position = new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.0f)
                    },
                    new Renderer.VertexDataStruct
                    {
                        position = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.0f)
                    },
                    new Renderer.VertexDataStruct // y
                    {
                        position = new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.0f)
                    },
                    new Renderer.VertexDataStruct
                    {
                        position = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.0f)
                    },
                    new Renderer.VertexDataStruct // z
                    {
                        position = new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.0f)
                    },
                    new Renderer.VertexDataStruct
                    {
                        position = new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        color = new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.0f)
                    }
                };
            uint[] indices = new uint[6];
            for (uint i = 0; i <= 6 - 1; ++i) indices[i] = i;

            return new MeshObject(_directX3DGraphics, Vector4.Zero,
                0.0f, 0.0f, 0.0f, vertices, indices, PrimitiveTopology.LineList,
                material, visible, ObjectType.Normal);
        }

        public MeshObject MakeRectangle(float sizeX, float sizeZ, Vector4 position, float yaw, float pitch, float roll,
            Vector4 color, Material material, bool visible)
        {
            Renderer.VertexDataStruct[] vertices =
                new Renderer.VertexDataStruct[4]
                {
                    new Renderer.VertexDataStruct
                    {
                        position = new Vector4(-sizeX, 0.0f, -sizeZ, 1.0f),
                        color = color,
                        texCoord = new Vector2(0.0f, 1.0f)
                    },
                    new Renderer.VertexDataStruct
                    {
                        position = new Vector4(sizeX, 0.0f, -sizeZ, 1.0f),
                        color = color,
                        texCoord = new Vector2(1.0f, 1.0f)
                    },
                    new Renderer.VertexDataStruct
                    {
                        position = new Vector4(sizeX, 0.0f, sizeZ, 1.0f),
                        color = color,
                        texCoord = new Vector2(1.0f, 0.0f)
                    },
                    new Renderer.VertexDataStruct
                    {
                        position = new Vector4(-sizeX, 0.0f, sizeZ, 1.0f),
                        color = color,
                        texCoord = new Vector2(0.0f, 0.0f)
                    }
                };
            uint[] indices = new uint[6]
            {
                0, 1, 2,
                2, 3, 0
            };

            return new MeshObject(_directX3DGraphics, position,
                yaw, pitch, roll, vertices, indices, PrimitiveTopology.TriangleList,
                material, visible, ObjectType.Normal);
        }

        public MeshObject MakeCube(Vector4 position, float yaw, float pitch, float roll, Material material, bool visible)
        {
            Renderer.VertexDataStruct[] vertices =
                new Renderer.VertexDataStruct[24]
                {
                    new Renderer.VertexDataStruct // front 0
                    {
                        position = new Vector4(-1.0f, 1.0f, -1.0f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.25f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // front 1
                    {
                        position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.25f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // front 2
                    {
                        position = new Vector4(1.0f, -1.0f, -1.0f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.5f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // front 3
                    {
                        position = new Vector4(1.0f, 1.0f, -1.0f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.5f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // right 4
                    {
                        position = new Vector4(1.0f, 1.0f, -1.0f, 1.0f),
                        normal = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.5f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // right 5
                    {
                        position = new Vector4(1.0f, -1.0f, -1.0f, 1.0f),
                        normal = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.5f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // right 6
                    {
                        position = new Vector4(1.0f, -1.0f, 1.0f, 1.0f),
                        normal = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.75f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // right 7
                    {
                        position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        normal = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.75f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // back 8
                    {
                        position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.75f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // back 9
                    {
                        position = new Vector4(1.0f, -1.0f, 1.0f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.75f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // back 10
                    {
                        position = new Vector4(-1.0f, -1.0f, 1.0f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(1.0f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // back 11
                    {
                        position = new Vector4(-1.0f, 1.0f, 1.0f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(1.0f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // left 12
                    {
                        position = new Vector4(-1.0f, 1.0f, 1.0f, 1.0f),
                        normal = new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // left 13
                    {
                        position = new Vector4(-1.0f, -1.0f, 1.0f, 1.0f),
                        normal = new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // left 14
                    {
                        position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f),
                        normal = new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.25f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // left 15
                    {
                        position = new Vector4(-1.0f, 1.0f, -1.0f, 1.0f),
                        normal = new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.25f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // top 16
                    {
                        position = new Vector4(-1.0f, 1.0f, 1.0f, 1.0f),
                        normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.25f, 0.0f)
                    },
                    new Renderer.VertexDataStruct // top 17
                    {
                        position = new Vector4(-1.0f, 1.0f, -1.0f, 1.0f),
                        normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.25f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // top 18
                    {
                        position = new Vector4(1.0f, 1.0f, -1.0f, 1.0f),
                        normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.5f, 0.25f)
                    },
                    new Renderer.VertexDataStruct // top 19
                    {
                        position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.5f, 0.0f)
                    },
                    new Renderer.VertexDataStruct // bottom 20
                    {
                        position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f),
                        normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.25f, 0.5f)
                    },
                    new Renderer.VertexDataStruct // bottom 21
                    {
                        position = new Vector4(-1.0f, -1.0f, 1.0f, 1.0f),
                        normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.25f, 0.75f)
                    },
                    new Renderer.VertexDataStruct // bottom 22
                    {
                        position = new Vector4(1.0f, -1.0f, 1.0f, 1.0f),
                        normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.5f, 0.75f)
                    },
                    new Renderer.VertexDataStruct // bottom 23
                    {
                        position = new Vector4(1.0f, -1.0f, -1.0f, 1.0f),
                        normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.5f, 0.5f)
                    }
                };


            uint[] indices = new uint[36]
            {
                8, 9, 10,       10, 11, 8,
                12, 13, 14,     14, 15, 12,
                20, 21, 22,     22, 23, 20,
                0, 1, 2,        2, 3, 0,
                4, 5, 6,        6, 7, 4,
                16, 17, 18,     18, 19, 16
            };

            return new MeshObject(_directX3DGraphics, position,
                yaw, pitch, roll, vertices, indices, PrimitiveTopology.TriangleList,
                material, visible, ObjectType.Normal);
        }

        public MeshObject MakeLine(Vector4 from, Vector4 direction, float length, 
            Vector4 color, Material material, bool visible)
        {
            Renderer.VertexDataStruct[] vertices =
                new Renderer.VertexDataStruct[2]
                {
                    new Renderer.VertexDataStruct
                    {
                        position = from,
                        normal = new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.0f)
                    },
                    new Renderer.VertexDataStruct // front 0
                    {
                        position = new Vector4(10f, 10f, 10f, 1.0f),
                        normal = new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        texCoord = new Vector2(0.0f, 0.0f)
                    },
                };
            uint[] indices = new uint[2] {0, 1};

            return new MeshObject(_directX3DGraphics, 
                new Vector4(0.0f), 0.0f, 0.0f, 0.0f, vertices, indices, PrimitiveTopology.LineList, material, visible, ObjectType.Normal);
        }

        public void Dispose()
        {
            Utilities.Dispose(ref _imagingFactory);
        }
    }
}
