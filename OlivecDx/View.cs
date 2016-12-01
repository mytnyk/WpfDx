using System;
using System.Numerics;
using System.Runtime.InteropServices;
using OlivecDx.Render;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using SharpDX.Direct3D9;
using Format = SharpDX.DXGI.Format;
using Usage = SharpDX.DXGI.Usage;

namespace OlivecDx
{
  public class View
  {
    private readonly IntPtr _hwnd;
    private readonly int _width;
    private readonly int _height;
    //private DeviceContext _context;
    private readonly Texture2D _back_buffer;
    private readonly SharpDX.Direct3D9.DeviceEx _device9;
    private readonly SharpDX.Direct3D10.Device1 _device10;
    private readonly RenderTargetView _render_view;
    private Texture _texture;
    [DllImport("user32.dll", SetLastError = false)]
    static extern IntPtr GetDesktopWindow();

    private Color4 _bg_color = Colors.Green;

    public View(IntPtr hwnd, int width, int height)
    {
      var direct3D = new Direct3DEx();
      PresentParameters presentparams = new PresentParameters();
      presentparams.Windowed = true;
      presentparams.SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard;
      presentparams.DeviceWindowHandle = GetDesktopWindow();
      presentparams.PresentationInterval = PresentInterval.Default;

      _device9 = new SharpDX.Direct3D9.DeviceEx(
        direct3D, 0, DeviceType.Hardware, IntPtr.Zero, 
        CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve,
        presentparams);

      _hwnd = hwnd;
      _width = width;
      _height = height;

      _device10 = new SharpDX.Direct3D10.Device1(
        SharpDX.Direct3D10.DriverType.Hardware, DeviceCreationFlags.BgraSupport,
        SharpDX.Direct3D10.FeatureLevel.Level_10_0);
      //var device11 = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
      //var device11 = DeviceUtil.Create11(DeviceCreationFlags.BgraSupport);
      //_context = device11.ImmediateContext;

      var texture2_d_description = new Texture2DDescription()
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

      _back_buffer = new Texture2D(_device10, texture2_d_description);
      _render_view = new RenderTargetView(_device10, _back_buffer);

      SharpDX.Direct3D9.Format format = TranslateFormat(_back_buffer);
      if (format == SharpDX.Direct3D9.Format.Unknown)
        throw new ArgumentException("Texture format is not compatible with OpenSharedResource");
      IntPtr handle = GetSharedHandle(_back_buffer);
      if (handle == IntPtr.Zero)
        throw new ArgumentNullException("Handle");

      _texture = new Texture(_device9, _back_buffer.Description.Width, 
        _back_buffer.Description.Height, 1, SharpDX.Direct3D9.Usage.RenderTarget, format, Pool.Default, ref handle);

      //_context.ClearRenderTargetView(render_view, Colors.LightBlue);
    }
    private IntPtr GetSharedHandle(SharpDX.Direct3D10.Texture2D Texture)
    {
      SharpDX.DXGI.Resource resource = Texture.QueryInterface<SharpDX.DXGI.Resource>();
      IntPtr result = resource.SharedHandle;
      resource.Dispose();
      return result;
    }

    public Scene Scene { get; set; }
    public Matrix4x4 ViewMatrix { get; set; }

    private static SharpDX.Direct3D9.Format TranslateFormat(SharpDX.Direct3D10.Texture2D Texture)
    {
      switch (Texture.Description.Format)
      {
        case SharpDX.DXGI.Format.R10G10B10A2_UNorm:
          return SharpDX.Direct3D9.Format.A2B10G10R10;

        case SharpDX.DXGI.Format.R16G16B16A16_Float:
          return SharpDX.Direct3D9.Format.A16B16G16R16F;

        case SharpDX.DXGI.Format.B8G8R8A8_UNorm:
          return SharpDX.Direct3D9.Format.A8R8G8B8;

        default:
          return SharpDX.Direct3D9.Format.Unknown;
      }
    }
    public IntPtr GetBackBufferPtr()
    {
      SharpDX.Direct3D9.Surface surface = _texture.GetSurfaceLevel(0);
      return surface.NativePointer;
    }

    public void Render()
    {
      _device10.ClearRenderTargetView(_render_view, _bg_color);
      _device10.Flush();
    }

    public void SetNewBgColor()
    {
      _bg_color = _bg_color != Colors.LightBlue ? Colors.LightBlue : Colors.Green;
    }
  }
}
