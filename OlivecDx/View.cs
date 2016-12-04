using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using FillMode = SharpDX.Direct3D10.FillMode;
using Format = SharpDX.DXGI.Format;

namespace OlivecDx
{
  public class View : IDisposable
  {
    private readonly SharpDX.Direct3D10.Device1 _device10;
    private readonly RenderTargetView _renderView;
    private readonly DepthStencilView _depthStencilView;
    private readonly SharpDX.Direct3D9.Texture _backBufferTexture;

    [DllImport("user32.dll", SetLastError = false)]
    static extern IntPtr GetDesktopWindow();

    private Color4 _bgColor = Color.LightBlue;
    private readonly Matrix _projectionTransform;
    private Matrix _viewTransform;

    public View(int width, int height)
    {
      var presentparams = new SharpDX.Direct3D9.PresentParameters
      {
        Windowed = true,
        SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard,
        DeviceWindowHandle = GetDesktopWindow(),
        PresentationInterval = SharpDX.Direct3D9.PresentInterval.Default,
      };

      var device9 = new SharpDX.Direct3D9.DeviceEx(
        new SharpDX.Direct3D9.Direct3DEx(), 0, SharpDX.Direct3D9.DeviceType.Hardware, IntPtr.Zero,
        SharpDX.Direct3D9.CreateFlags.HardwareVertexProcessing | SharpDX.Direct3D9.CreateFlags.Multithreaded | SharpDX.Direct3D9.CreateFlags.FpuPreserve,
        presentparams);

      _device10 = new SharpDX.Direct3D10.Device1(
        DriverType.Hardware, DeviceCreationFlags.BgraSupport,
        FeatureLevel.Level_10_0);

      var texture2DDescription = new Texture2DDescription
      {
        ArraySize = 1,
        BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
        CpuAccessFlags = CpuAccessFlags.None,
        Format = Format.B8G8R8A8_UNorm,
        Width = width,
        Height = height,
        MipLevels = 1,
        OptionFlags = ResourceOptionFlags.Shared,
        SampleDescription = new SampleDescription(1, 0),
        Usage = ResourceUsage.Default
      };

       var backBuffer = new Texture2D(_device10, texture2DDescription);
      _renderView = new RenderTargetView(_device10, backBuffer);

      var format = TranslateFormat(backBuffer);
      if (format == SharpDX.Direct3D9.Format.Unknown)
        throw new ArgumentException("texture format is not compatible with OpenSharedResource");
      var handle = GetSharedHandle(backBuffer);
      if (handle == IntPtr.Zero)
        throw new ArgumentNullException("Handle");

      _backBufferTexture = new SharpDX.Direct3D9.Texture(device9, backBuffer.Description.Width,
        backBuffer.Description.Height, 1, SharpDX.Direct3D9.Usage.RenderTarget, format, SharpDX.Direct3D9.Pool.Default, ref handle);

      _projectionTransform = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, width / (float)height, 0.1f, 100);

      var rasterizerStateDesc = new RasterizerStateDescription()
      {
        CullMode = CullMode.None,
        FillMode = FillMode.Solid,
        IsFrontCounterClockwise = false,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = 0,
        IsDepthClipEnabled = true,
        IsScissorEnabled = false,
        IsMultisampleEnabled = true,
        IsAntialiasedLineEnabled = true
      };
      _device10.Rasterizer.State = new RasterizerState(_device10, rasterizerStateDesc);

      _device10.Rasterizer.SetViewports(new Viewport(0, 0,
          backBuffer.Description.Width, backBuffer.Description.Height, 0.0f, 1.0f));

      var texture2DDescriptionDepth = new Texture2DDescription
      {
        ArraySize = 1,
        BindFlags = BindFlags.DepthStencil,
        CpuAccessFlags = CpuAccessFlags.None,
        Format = Format.R32_Typeless,
        Width = width,
        Height = height,
        MipLevels = 1,
        OptionFlags = ResourceOptionFlags.Shared,
        SampleDescription = new SampleDescription(1, 0),
        Usage = ResourceUsage.Default
      };
      var depthTexture = new Texture2D(_device10, texture2DDescriptionDepth);
      var depthViewDesc = new DepthStencilViewDescription
      {
        Dimension = DepthStencilViewDimension.Texture2D,
        Format = Format.D32_Float,
      };

      _depthStencilView = new DepthStencilView(_device10, depthTexture, depthViewDesc);

      _device10.OutputMerger.SetTargets(_depthStencilView, _renderView);
      /*
      var blend_desc = new BlendStateDescription();
      blend_desc.IsBlendEnabled[0] = true;
      blend_desc.SourceBlend = BlendOption.SourceAlpha;
      blend_desc.DestinationBlend = BlendOption.InverseSourceAlpha;
      blend_desc.BlendOperation = SharpDX.Direct3D10.BlendOperation.Add;
      blend_desc.SourceAlphaBlend = BlendOption.Zero;
      blend_desc.DestinationAlphaBlend = BlendOption.Zero;
      blend_desc.AlphaBlendOperation = SharpDX.Direct3D10.BlendOperation.Add;
      blend_desc.RenderTargetWriteMask[0] = ColorWriteMaskFlags.All;
      //blend_desc.AlphaToCoverageEnable = false;
      var _blend_transparency_state = new BlendState(_device10, blend_desc);
      _device10.OutputMerger.SetBlendState(_blend_transparency_state, Color4.White, 0);*/
    }

    public void TestRotate()
    {
      Direction += new System.Numerics.Vector3(0, 0.001f, 0);// Matrix.RotationZ(0.001f) * Direction;
    }

    private static IntPtr GetSharedHandle(Texture2D texture)
    {
      using (var resource = texture.QueryInterface<SharpDX.DXGI.Resource>())
      {
        return resource.SharedHandle;
      }
    }

    public Scene Scene { get; set; }

    public void InitBuffers()
    {
      foreach (var sceneObject in Scene.ListOfObjects)
      {
        sceneObject.InitBuffers(_device10);
      }
    }

    private static SharpDX.Direct3D9.Format TranslateFormat(Texture2D texture)
    {
      switch (texture.Description.Format)
      {
        case Format.R10G10B10A2_UNorm:
          return SharpDX.Direct3D9.Format.A2B10G10R10;

        case Format.R16G16B16A16_Float:
          return SharpDX.Direct3D9.Format.A16B16G16R16F;

        case Format.B8G8R8A8_UNorm:
          return SharpDX.Direct3D9.Format.A8R8G8B8;

        case Format.R8G8B8A8_UNorm:
          return SharpDX.Direct3D9.Format.A8R8G8B8;
        default:
          return SharpDX.Direct3D9.Format.Unknown;
      }
    }
    public IntPtr GetBackBufferPtr()
    {
      var surface = _backBufferTexture.GetSurfaceLevel(0);
      return surface.NativePointer;
    }

    public void Render()
    {
      var position = new Vector3(Position.X, Position.Y, Position.Z);
      var direction = new Vector3(Direction.X, Direction.Y, Direction.Z);
      _viewTransform = Matrix.LookAtLH(position, position + direction, Vector3.UnitZ);
      _device10.ClearRenderTargetView(_renderView, _bgColor);
      _device10.ClearDepthStencilView(_depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
      foreach (var sceneObject in Scene.ListOfObjects)
      {
        sceneObject.Render(_device10, _viewTransform, _projectionTransform);
      }

      _device10.Flush();
    }

    public void SetNewBgColor()
    {
      _bgColor = _bgColor != Color.LightBlue ? Color.LightBlue : Color.Green;
    }

    public System.Numerics.Vector3 Position { get; set; }
    public System.Numerics.Vector3 Direction { get; set; }
    public void Dispose()
    {
      _device10.Dispose();
      _depthStencilView.Dispose();
      _renderView.Dispose();
      _backBufferTexture.Dispose();
    }
  }
}
