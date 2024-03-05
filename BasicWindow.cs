namespace Windows;

public class BasicWindow: BaseMessageWindow, IMessageWindow
{
    private const string WndClass = "BasicWndClass_2171A7AF";
    
    public event EventHandler<WindowsMessageEventArgs>? MessageCaptured; 

    public BasicWindow() : base(WndClass, false)
    {
        Task.Run(CaptureTask);
    }

    public string WindowClassName => WndClass;

    protected override void WndProc(IntPtr wnd, uint message, IntPtr wParam, IntPtr lParam)
    {
        var msg = new Message(wnd, message, wParam, lParam);
        MessageCaptured?.Invoke(this, new WindowsMessageEventArgs(msg));
    }
}