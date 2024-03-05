using System.Drawing;
using System.Runtime.InteropServices;

namespace Windows.PInvoke;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct Win32Message
{
    public IntPtr wndHandle;
    public uint Msg;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint Time;
    public Point Point;
    public uint Private;
}