using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Windows.PInvoke;

[DebuggerDisplay("{handle}")]
public class SafeWndClassHandle : SafeHandle
{

    /// <summary>
    ///     Default constructor
    /// </summary>
    /// <remarks>This constructor is for P/Invoke.</remarks>
    public SafeWndClassHandle() : base(IntPtr.Zero, true)
    { }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        return IsInvalid || User32.UnregisterClass((ushort)handle.ToInt32(), IntPtr.Zero);
    }
}