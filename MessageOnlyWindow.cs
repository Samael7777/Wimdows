using System.Data;
using System.Diagnostics;


// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Windows;


[DebuggerDisplay("{_captureWndHandle.handle}")]
public class MessageOnlyWindow : BaseMessageWindow, IMessageWindow
{
    private const string WndClass = "CaptureWndClass_";

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly Task _wndTask;

    public event EventHandler<WindowsMessageEventArgs>? MessageCaptured; 

    public MessageOnlyWindow() : base(WndClass + Guid.NewGuid(), true)
    {
        _wndTask = new Task(CaptureTask, TaskCreationOptions.LongRunning
                                            | TaskCreationOptions.DenyChildAttach
                                            | TaskCreationOptions.HideScheduler);
        _wndTask.Start();
    }
    
    protected override void WndProc(IntPtr wnd, uint message, IntPtr wParam, IntPtr lParam)
    {
        var msg = new Message(wnd, message, wParam, lParam);
        MessageCaptured?.Invoke(this, new WindowsMessageEventArgs(msg));
    }
}