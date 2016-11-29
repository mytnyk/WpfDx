using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using System.Numerics;

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
        public Matrix4x4 View;
        public Matrix4x4 Projection;
        public Matrix4x4 Position;
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
        public Vector4 Color { get; set; }

        public Triangles(
            IEnumerable<Vector3> vertices, IEnumerable<Vector3> normals,
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

        private void InitBuffers(Device device, TrianglesLayout layout)
        {
            _layout = layout;
            _vertices_buffer = Buffer.Create(device, BindFlags.VertexBuffer, _data);
            _vertex_binding = new VertexBufferBinding(_vertices_buffer, Utilities.SizeOf<TrianglesVertexShaderStruct>(), 0);
            _indices_buffer = Buffer.Create(device, BindFlags.IndexBuffer, _faces);

            _triangles_constants_buffer = new Buffer(device, Utilities.SizeOf<TrianglesConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
        public void Render(DeviceContext context, 
            Matrix4x4 view, Matrix4x4 projection, Matrix4x4 position)
        {
            context.VertexShader.SetConstantBuffer(0, _triangles_constants_buffer);
            _triangles_constants.View = view;
            _triangles_constants.Projection = projection;
            _triangles_constants.Position = position;
            _triangles_constants.Color = Color;
            context.UpdateSubresource(ref _triangles_constants, _triangles_constants_buffer);

            context.InputAssembler.InputLayout = _layout.Layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.VertexShader.Set(_layout.VertexShader);
            context.PixelShader.Set(_layout.PixelShader);
            context.InputAssembler.SetVertexBuffers(0, _vertex_binding);
            context.InputAssembler.SetIndexBuffer(_indices_buffer, Format.R32_UInt, 0);
            context.DrawIndexed(_faces.Length, 0, 0);
        }
        public void Dispose()
        {
            _vertices_buffer.Dispose();
            _indices_buffer.Dispose();
            _triangles_constants_buffer.Dispose();
        }
    }
}
