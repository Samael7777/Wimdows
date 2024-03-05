using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Windows.PInvoke;

[DebuggerDisplay("{handle}")]
public class SafeWndHandle : SafeHandle
{
    /// <summary>
    ///     Default constructor
    /// </summary>
    /// <remarks>This constructor is for P/Invoke.</remarks>
    public SafeWndHandle() : base(IntPtr.Zero, true)
    { }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        return User32.DestroyWindow(handle);
    }
}

