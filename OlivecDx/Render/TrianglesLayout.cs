using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace OlivecDx.Render
{
    internal class TrianglesLayout
    {
        private readonly ShaderSignature _input_signature;
        public VertexShader VertexShader { get; }
        public PixelShader PixelShader { get; }
        public InputLayout Layout { get; }

        public TrianglesLayout(Device device)
        {
            // Compile Vertex and Pixel shaders
            using (var vertex_shader_byte_code = ShaderBytecode.CompileFromFile("Render/triangles.fx", "VertSh", "vs_4_0"))
            {
                VertexShader = new VertexShader(device, vertex_shader_byte_code);
                _input_signature = ShaderSignature.GetInputSignature(vertex_shader_byte_code);
            }
            using (var pixel_shader_byte_code = ShaderBytecode.CompileFromFile("Render/triangles.fx", "PixSh", "ps_4_0"))
            {
                PixelShader = new PixelShader(device, pixel_shader_byte_code);
            }
            // Layout from VertexShader input signature
            Layout = new InputLayout(
                device,
                _input_signature,
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0)
                    });
        }
        public void Dispose()
        {
            _input_signature.Dispose();
            VertexShader.Dispose();
            PixelShader.Dispose();
            Layout.Dispose();
        }
    }
}
