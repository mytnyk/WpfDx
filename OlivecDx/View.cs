using System;
using System.Numerics;
using System.Runtime.InteropServices;
using OlivecDx.Render;
using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using SharpDX.Direct3D9;

namespace OlivecDx
{
  public class View
  {
    private readonly int _width;
    private readonly int _height;

    private readonly Texture2D _back_buffer;
    private readonly SharpDX.Direct3D9.DeviceEx _device9;
    private readonly SharpDX.Direct3D10.Device1 _device10;
    private readonly RenderTargetView _render_view;
    private readonly Texture _texture;
    [DllImport("user32.dll", SetLastError = false)]
    static extern IntPtr GetDesktopWindow();

    private Color4 _bg_color = Colors.Green;

    public View(int width, int height)
    {
      var presentparams = new PresentParameters
      {
        Windowed = true,
        SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard,
        DeviceWindowHandle = GetDesktopWindow(),
        PresentationInterval = PresentInterval.Default,
      };

      _device9 = new SharpDX.Direct3D9.DeviceEx(
        new Direct3DEx(), 0, DeviceType.Hardware, IntPtr.Zero, 
        CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve,
        presentparams);

      _width = width;
      _height = height;

      _device10 = new SharpDX.Direct3D10.Device1(
        SharpDX.Direct3D10.DriverType.Hardware, DeviceCreationFlags.BgraSupport,
        SharpDX.Direct3D10.FeatureLevel.Level_10_0);

      var texture2_d_description = new Texture2DDescription
      {
        ArraySize = 1,
        BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
        CpuAccessFlags = CpuAccessFlags.None,
        Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
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
        throw new ArgumentException("texture format is not compatible with OpenSharedResource");
      IntPtr handle = GetSharedHandle(_back_buffer);
      if (handle == IntPtr.Zero)
        throw new ArgumentNullException("Handle");

      _texture = new Texture(_device9, _back_buffer.Description.Width, 
        _back_buffer.Description.Height, 1, SharpDX.Direct3D9.Usage.RenderTarget, format, Pool.Default, ref handle);

      var eye = new System.Numerics.Vector3(0, 0, -50);
      ViewTransform = Matrix4x4.CreateLookAt(eye, System.Numerics.Vector3.Zero, System.Numerics.Vector3.UnitY);
      ProjectionTransform = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, _width / (float)_height, 0.1f, 100);
    }
    private IntPtr GetSharedHandle(SharpDX.Direct3D10.Texture2D texture)
    {
      SharpDX.DXGI.Resource resource = texture.QueryInterface<SharpDX.DXGI.Resource>();
      IntPtr result = resource.SharedHandle;
      resource.Dispose();
      return result;
    }

    public Scene Scene { get; set; }
    //public Matrix4x4 ViewMatrix { get; set; }

    public void InitBuffers()
    {
      var layout = new TrianglesLayout(_device10);
      foreach (var scene_object in Scene.ListOfObjects)
      {
        scene_object.InitBuffers(_device10, layout);
      }
    }

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

        case SharpDX.DXGI.Format.R8G8B8A8_UNorm:
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

      foreach (var scene_object in Scene.ListOfObjects)
      {
        scene_object.Render(_device10, ViewTransform, ProjectionTransform);
      }

      _device10.Flush();
    }

    public void SetNewBgColor()
    {
      _bg_color = _bg_color != Colors.LightBlue ? Colors.LightBlue : Colors.Green;
    }

    public Matrix4x4 ViewTransform { get; set; }
    public Matrix4x4 ProjectionTransform { get; set; }
  }
}
