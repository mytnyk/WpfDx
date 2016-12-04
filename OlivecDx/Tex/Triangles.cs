using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device;
using Vector4 = SharpDX.Vector4;

namespace OlivecDx.Tex
{
  [StructLayout(LayoutKind.Sequential)]
  internal struct TrianglesVertexShaderStruct
  {
    public Vector4 Vertex;
    public Vector2 TextureCoord;
  }
  [StructLayout(LayoutKind.Sequential)]
  internal struct TrianglesConstants
  {
    public Matrix View;
    public Matrix Projection;
    public Matrix Position;
  }
  public class Triangles : IDisposable
  {
    private readonly TrianglesVertexShaderStruct[] _data;
    private Buffer _verticesBuffer;
    private VertexBufferBinding _vertexBinding;
    private TrianglesLayout _layout;
    private Buffer _trianglesConstantsBuffer;
    private TrianglesConstants _trianglesConstants;
    private ShaderResourceView _textureView;
    private SamplerState _sampler;
    private readonly byte[] _textureBuffer;
    public Triangles(
        IEnumerable<System.Numerics.Vector3> vertices,
        IEnumerable<System.Numerics.Vector2> textureCoord,
        byte[] textureBuffer
        )
    {
      _data = vertices.Zip(textureCoord, (v, t) =>
      new TrianglesVertexShaderStruct
      {
        Vertex = new Vector4(v.X, v.Y, v.Z, 1.0f),
        TextureCoord = new Vector2(t.X, t.Y)
      }).ToArray();
      _textureBuffer = textureBuffer;
    }

    internal void InitBuffers(Device device)
    {
      _layout = new TrianglesLayout(device);
      _verticesBuffer = Buffer.Create(device, BindFlags.VertexBuffer, _data);
      _vertexBinding = new VertexBufferBinding(_verticesBuffer, Utilities.SizeOf<TrianglesVertexShaderStruct>(), 0);

      _trianglesConstantsBuffer = new Buffer(device,
        Utilities.SizeOf<TrianglesConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);

      // Load textureBuffer and create sampler
      var texture = Resource.FromMemory<Texture2D>(device, _textureBuffer);

      _textureView = new ShaderResourceView(device, texture);

      _sampler = new SamplerState(device, new SamplerStateDescription()
      {
        Filter = Filter.MinMagMipLinear,
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap,
        BorderColor = Color.Black,
        ComparisonFunction = Comparison.Never,
        MaximumAnisotropy = 16,
        MipLodBias = 0,
        MinimumLod = 0,
        MaximumLod = 16,
      });
    }

    internal void Render(Device device,
        Matrix view, Matrix projection, Matrix position)
    {
      device.VertexShader.SetConstantBuffer(0, _trianglesConstantsBuffer);
      _trianglesConstants.View = view;
      _trianglesConstants.Projection = projection;
      _trianglesConstants.Position = position;
      device.UpdateSubresource(ref _trianglesConstants, _trianglesConstantsBuffer);

      device.InputAssembler.InputLayout = _layout.Layout;
      device.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
      device.VertexShader.Set(_layout.VertexShader);
      device.PixelShader.Set(_layout.PixelShader);
      device.PixelShader.SetSampler(0, _sampler);
      device.PixelShader.SetShaderResource(0, _textureView);

      device.InputAssembler.SetVertexBuffers(0, _vertexBinding);
      device.Draw(_data.Length, 0);
    }
    public void Dispose()
    {
      _verticesBuffer.Dispose();
      _trianglesConstantsBuffer.Dispose();
      _layout.Dispose();
      _textureView.Dispose();
      _sampler.Dispose();
    }
  }
}
