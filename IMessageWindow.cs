namespace Windows;

public interface IMessageWindow : IDisposable
{
    event EventHandler<WindowsMessageEventArgs>? MessageCaptured; 
    IntPtr WndHandle { get; }
    int WndThreadId { get; }
    void Invoke(Action action);
    T? Invoke<T>(Func<T> func);
}