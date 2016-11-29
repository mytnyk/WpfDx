using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlivecDx
{
  public class View
  {
    private IntPtr hwnd;
    private int v1;
    private int v2;

    public View(IntPtr hwnd, int v1, int v2)
    {
      this.hwnd = hwnd;
      this.v1 = v1;
      this.v2 = v2;
    }
  }
}
