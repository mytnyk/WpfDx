using System;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D10.Device;

namespace OlivecDx.Tex
{
  public class TrianglesLayout : IDisposable
  {
    private readonly ShaderSignature _inputSignature;
    public VertexShader VertexShader { get; }
    public PixelShader PixelShader { get; }
    public InputLayout Layout { get; }

    public TrianglesLayout(Device device)
    {
      // Compile Vertex and Pixel shaders
      using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile("Tex/triangles.fx", "VertSh", "vs_4_0"))
      {
        VertexShader = new VertexShader(device, vertexShaderByteCode);
        _inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
      }
      using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile("Tex/triangles.fx", "PixSh", "ps_4_0"))
      {
        PixelShader = new PixelShader(device, pixelShaderByteCode);
      }
      // Layout from VertexShader input signature
      Layout = new InputLayout(
          device,
          _inputSignature,
          new[]
              {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
              });
    }
    public void Dispose()
    {
      _inputSignature.Dispose();
      VertexShader.Dispose();
      PixelShader.Dispose();
      Layout.Dispose();
    }
  }
}
