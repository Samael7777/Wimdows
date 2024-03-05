
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Windows.PInvoke;

internal static class WinApiHelper
{
    public static IntPtr GetModuleHandle(string? moduleName = null)
    {
        var handle = Kernel32.GetModuleHandle(moduleName);
        if(handle == IntPtr.Zero) ThrowLastWin32Error("Can't get module handle.");
        return handle;
    }
    
    public static SafeWndHandle CreateWindowEx(
        int dwExStyle, SafeWndClassHandle? lpClassNameAtom, string lpWindowName,
        uint dwStyle, int x, int y, int nWidth, int nHeight,
        IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam)
    {
        var handle = User32.CreateWindowEx(dwExStyle, lpClassNameAtom, lpWindowName, dwStyle, 
            x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);

        if (handle.IsInvalid) ThrowLastWin32Error("Can't create window.");

        return handle;
    }

    public static SafeWndClassHandle RegisterClassEx(ref WindowClassEx windowClass)
    {
        var handle = User32.RegisterClassEx(ref windowClass);
        if (handle.IsInvalid) ThrowLastWin32Error("Can't register window class.");

        return handle;
    }
    
    private static void ThrowLastWin32Error(string message)
    {
        var error = Marshal.GetLastWin32Error();
        throw new Win32Exception(error, message);
    }
}