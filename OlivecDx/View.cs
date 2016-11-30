using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows;
using OlivecDx.Render;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;
using System.Threading;

namespace OlivecDx
{
  public class View
  {
    private readonly IntPtr _hwnd;
    private readonly int _width;
    private readonly int _height;
    private DeviceContext _context;

    public View(IntPtr hwnd, int width, int height)
    {
      _hwnd = hwnd;
      _width = width;
      _height = height;
    }

    public Scene Scene { get; set; }
    public Matrix4x4 ViewMatrix { get; set; }

    public void Run()
    {
      var swap_chain_description = new SwapChainDescription
      {
        BufferCount = 1,
        ModeDescription = new ModeDescription(_width, _height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
        IsWindowed = true,
        OutputHandle = _hwnd,
        SampleDescription = new SampleDescription(1, 0),
        SwapEffect = SwapEffect.Discard,
        Usage = Usage.RenderTargetOutput
      };

      // create device and swap chain
      Device device;
      SwapChain swap_chain;
      Device.CreateWithSwapChain(DriverType.Hardware, 
        DeviceCreationFlags.BgraSupport, swap_chain_description, out device, out swap_chain);
      _context = device.ImmediateContext;

      // Ignore all windows events (necessary?)
      //var factory = swap_chain.GetParent<Factory>();
      //factory.MakeWindowAssociation(_hwnd, WindowAssociationFlags.IgnoreAll);

      // New RenderTargetView from the backbuffer
      var back_buffer = Resource.FromSwapChain<Texture2D>(swap_chain, 0);
      var render_view = new RenderTargetView(device, back_buffer);

      var triangles_layout = new TrianglesLayout(device);
            //_shell.InitBuffers(device, layout);

        //using (var renderLoop = new RenderLoop())
        //{
            while (true)
            {
                Thread.Sleep(100);
                    _context.ClearRenderTargetView(render_view, Colors.LightBlue);
                    //_context.ClearDepthStencilView(depth_stencil_view, DepthStencilClearFlags.Depth, 1.0f, 0);
                    swap_chain.Present(0, PresentFlags.None);
                }
        //}
        /*
            RenderLoop.Run(new ApplicationContext(), () =>
      {
        _context.ClearRenderTargetView(render_view, Colors.LightBlue);
        //_context.ClearDepthStencilView(depth_stencil_view, DepthStencilClearFlags.Depth, 1.0f, 0);
        swap_chain.Present(0, PresentFlags.None);
      });*/
    }
  }
}
