namespace Windows;

public record Message(IntPtr HWnd, uint Msg, IntPtr WParam, IntPtr LParam);
