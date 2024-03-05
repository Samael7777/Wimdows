namespace Windows;

public class WindowsMessageEventArgs : EventArgs
{
    public Message Message { get; }

    public WindowsMessageEventArgs(Message windowsMessage)
    {
        Message = windowsMessage;
    }
}