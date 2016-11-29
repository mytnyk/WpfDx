using SharpDX;
using SharpDX.Mathematics.Interop;

namespace OlivecDx.Render
{
    internal class Colors
    {
        public static RawColor4 LightRed => new RawColor4(1.0f, 0.0f, 0.0f, 1.0f);
        public static RawColor4 LightGreen => new RawColor4(0.0f, 1.0f, 0.0f, 1.0f);
        public static RawColor4 LightBlue => new RawColor4(0.0f, 0.0f, 1.0f, 1.0f);
        public static RawColor4 YellowTransparent => new RawColor4(0.5f, 0.5f, 0.0f, 0.5f);
        public static RawColor4 Red => new RawColor4(0.5f, 0.0f, 0.0f, 1.0f);
        public static RawColor4 Green => new RawColor4(0.0f, 0.5f, 0.0f, 1.0f);
    }
}