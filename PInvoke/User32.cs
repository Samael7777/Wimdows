using System.Runtime.InteropServices;

namespace Windows.PInvoke;

internal static class User32
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern SafeWndHandle CreateWindowEx(
        int dwExStyle, 
        SafeWndClassHandle? lpClassNameAtom,
        string lpWindowName, 
        uint dwStyle, 
        int x, int y, int nWidth, int nHeight,
        IntPtr hWndParent, 
        IntPtr hMenu, 
        IntPtr hInstance, 
        IntPtr lpParam);

    
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern SafeWndClassHandle RegisterClassEx(ref WindowClassEx windowClass);

    
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool UnregisterClass(ushort lpClassNameAtom, IntPtr hInstance);

    
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TranslateMessage(ref Win32Message lpMsg);

    
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int DispatchMessage(ref Win32Message lpMsg);

    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr DefWindowProc(
        IntPtr hWnd, 
        uint msg, 
        IntPtr wParam, 
        IntPtr lParam);

    
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetMessage(
        [Out]out Win32Message lpMsg, 
        IntPtr hWnd,
        int wMsgFilterMin, 
        int wMsgFilterMax);

    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int SendMessage(
        IntPtr hWnd, 
        uint msg, 
        IntPtr wParam, 
        IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return:MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(
        IntPtr hWnd, 
        uint msg, 
        IntPtr wParam, 
        IntPtr lParam);

    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern void PostQuitMessage(int errorCode);
    
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool DestroyWindow(IntPtr hWnd);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern uint RegisterWindowMessage([In][MarshalAs(UnmanagedType.LPWStr)] string lpString);
}