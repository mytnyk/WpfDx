using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device;

using Vector3 = SharpDX.Vector3;
using Vector4 = SharpDX.Vector4;
using SharpDX.Direct3D10;

namespace OlivecDx.Render
{
  [StructLayout(LayoutKind.Sequential)]
  internal struct TrianglesVertexShaderStruct
  {
    public Vector4 Vertex;
    public Vector4 Normal;
  }
  [StructLayout(LayoutKind.Sequential)]
  internal struct TrianglesConstants
  {
    public Matrix View;
    public Matrix Projection;
    public Matrix Position;
    public Vector4 Color;
  }
  public class Triangles
  {
    private readonly int[] _faces;
    private readonly TrianglesVertexShaderStruct[] _data;
    private Buffer _vertices_buffer;
    private Buffer _indices_buffer;
    private VertexBufferBinding _vertex_binding;
    private TrianglesLayout _layout;
    private Buffer _triangles_constants_buffer;
    private TrianglesConstants _triangles_constants;
    public Vector4 Color { get; set; } = Colors.YellowTransparent;

    public Triangles(
        IEnumerable<System.Numerics.Vector3> vertices, IEnumerable<System.Numerics.Vector3> normals,
        int[] faces)
    {
      _faces = faces;
      _data = vertices.Zip(normals, (v, n) =>
      new TrianglesVertexShaderStruct
      {
        Vertex = new Vector4(v.X, v.Y, v.Z, 1.0f),
        Normal = new Vector4(n.X, n.Y, n.Z, 0.0f)
      }).ToArray();
    }

    public void InitBuffers(Device device, TrianglesLayout layout)
    {
      _layout = layout;
      _vertices_buffer = Buffer.Create(device, BindFlags.VertexBuffer, _data);
      _vertex_binding = new VertexBufferBinding(_vertices_buffer, Utilities.SizeOf<TrianglesVertexShaderStruct>(), 0);
      _indices_buffer = Buffer.Create(device, BindFlags.IndexBuffer, _faces);

      _triangles_constants_buffer = new Buffer(device,
        Utilities.SizeOf<TrianglesConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);
    }

    public void Render(Device device,
        Matrix view, Matrix projection, Matrix position)
    {
      device.VertexShader.SetConstantBuffer(0, _triangles_constants_buffer);
      _triangles_constants.View = view;
      _triangles_constants.Projection = projection;
      _triangles_constants.Position = position;
      _triangles_constants.Color = Color;
      device.UpdateSubresource(ref _triangles_constants, _triangles_constants_buffer);

      device.InputAssembler.InputLayout = _layout.Layout;
      device.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
      device.VertexShader.Set(_layout.VertexShader);
      device.PixelShader.Set(_layout.PixelShader);
      device.InputAssembler.SetVertexBuffers(0, _vertex_binding);
      device.InputAssembler.SetIndexBuffer(_indices_buffer, Format.R32_UInt, 0);
      device.DrawIndexed(_faces.Length, 0, 0);
    }
    public void Dispose()
    {
      _vertices_buffer.Dispose();
      _indices_buffer.Dispose();
      _triangles_constants_buffer.Dispose();
    }
  }
}
