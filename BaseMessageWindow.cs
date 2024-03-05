using System.Runtime.InteropServices;
using Windows.PInvoke;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Windows;

//todo async execution
public abstract class BaseMessageWindow : IDisposable
{
    private const int HWND_MESSAGE = -3;
    private const string InvokeActionMessageName = "InvokeActionMessage{8FD8734C-9B9D-4866-944B-54B81B9E3D7C}";

    private static readonly uint InvokeActionMessage = User32.RegisterWindowMessage(InvokeActionMessageName);
    
    private delegate IntPtr WndProcDelegate(IntPtr wnd, uint message, IntPtr wParam, IntPtr lParam);
    
    private readonly Queue<InvocationResult> _invocationQueue;
    private readonly ManualResetEventSlim _waitInitEvent;
    private readonly WindowClassEx _windowClass;
    private readonly SafeWndClassHandle _wndClassHandle;
    private readonly bool _isMessageOnly;
    
    protected SafeWndHandle? _captureWndHandle;
    private int _captureThreadId;
    
    protected BaseMessageWindow(string wndClassName, bool isMessageOnly)
    {
        _invocationQueue = new Queue<InvocationResult>();
        _waitInitEvent = new ManualResetEventSlim(false);
        _isMessageOnly = isMessageOnly;
        _windowClass = BuildWindowClass(wndClassName);
        _wndClassHandle = WinApiHelper.RegisterClassEx(ref _windowClass);
    }

    public IntPtr WndHandle
    {
        get
        {
            _waitInitEvent.Wait(); //Locks property until capture window creating
            return _captureWndHandle?.DangerousGetHandle()
                   ?? throw new ArgumentNullException(nameof(WndHandle));
        }
    }

    public int WndThreadId
    {
        get
        {
            _waitInitEvent.Wait();
            return _captureThreadId;
        }
    }

    public void Invoke(Action action)
    {
        _ = InvokeInternal(action, null, true);
    }

    public T? Invoke<T>(Func<T> func)
    {
        return (T?)InvokeInternal(func, null, true);
    }

    protected object? InvokeInternal(Delegate method, object?[]? args, bool syncronus)
    {
        var currentThreadId = Kernel32.GetCurrentThreadId();
        if (currentThreadId == WndThreadId) return method.DynamicInvoke(args);
        
        var invocation = new InvocationResult(method, null, syncronus);
        lock (_invocationQueue)
        {
            _invocationQueue.Enqueue(invocation);
        }
        PostInvokeMessage();
        
        invocation.AsyncWaitHandle.WaitOne();
        
        if (invocation is { IsCompleted: false, Exception: not null }) 
            throw invocation.Exception;

        return invocation.Result;
    }

    protected abstract void WndProc(IntPtr wnd, uint message, IntPtr wParam, IntPtr lParam);

    protected void CaptureTask()
    {
        _captureThreadId = Kernel32.GetCurrentThreadId();
        _captureWndHandle = CreateWindow(_wndClassHandle, _windowClass);
        
        _waitInitEvent.Set(); //Capture window created. Unlock methods.

        int error;
        while ((error = User32.GetMessage(out var message, IntPtr.Zero, 0, 0)) != 0)
        {
            if (message.Msg == InvokeActionMessage)
            {
                InvokeProc();
                continue;
            }
            if (error == -1)
            {
                //Handle the error

            }
            else
            {
                User32.TranslateMessage(ref message);
                _ = User32.DispatchMessage(ref message);
                //_ = DefaultWndProc(message.wndHandle, message.Msg, message.wParam, message.lParam);
            }
        }

        _captureWndHandle.Dispose();
        _captureThreadId = 0;
        UnregisterCaptureWindowClass();
    }
    
    private IntPtr DefaultWndProc(IntPtr wnd, uint message, IntPtr wParam, IntPtr lParam)
    {
        var maskedMessage = message & 0xFFFF;
        switch (maskedMessage)
        {
            case (uint)WindowsMessage.WM_DESTROY:
            case (uint)WindowsMessage.WM_QUIT:
                User32.PostQuitMessage(0);
                break;
            default:
                WndProc(wnd, message, wParam, lParam);
                break;
        }
        return User32.DefWindowProc(wnd, message, wParam, lParam);
    }

    private void InvokeProc()
    {
        lock (_invocationQueue)
        {
            if (_invocationQueue.Count == 0) return;
            while (_invocationQueue.TryDequeue(out var current))
            {
                try
                {
                    var result = current.Method.DynamicInvoke(current.Args);
                    current.SetCompleted(result);
                }
                catch (Exception e)
                {
                    current.SetException(e);
                }
            }
        }
    }

    private void PostInvokeMessage()
    {
        User32.PostMessage(WndHandle, InvokeActionMessage, IntPtr.Zero, IntPtr.Zero);
    }

    private WindowClassEx BuildWindowClass(string wndClassName)
    {
        var currentModuleHandle = WinApiHelper.GetModuleHandle();
        var wndClass = new WindowClassEx
        {
            cbSize = Marshal.SizeOf(typeof(WindowClassEx)),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate<WndProcDelegate>(DefaultWndProc),
            lpszClassName = wndClassName,
            style = 0,
            hbrBackground = IntPtr.Zero,
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = currentModuleHandle,
            hIcon = IntPtr.Zero,
            hIconSm = IntPtr.Zero,
            hCursor = IntPtr.Zero,
            lpszMenuName = null
        };

        return wndClass;
    }

    private SafeWndHandle CreateWindow(SafeWndClassHandle wndClassHandle, WindowClassEx wndClass)
    {
        return WinApiHelper.CreateWindowEx(
            0,
            wndClassHandle,
            "",
            0,
            0, 0, 0, 0,
            // ReSharper disable once RedundantCast
            _isMessageOnly ? (IntPtr)HWND_MESSAGE : IntPtr.Zero,
            IntPtr.Zero,
            wndClass.hInstance,
            IntPtr.Zero);
    }

    private void UnregisterCaptureWindowClass()
    {
        _wndClassHandle.Dispose();
    }
    
    #region Dispose

    private bool _disposed;

    ~BaseMessageWindow()
    {
        Dispose(false);
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            //dispose managed state (managed objects)
            _captureWndHandle?.Dispose();
            _wndClassHandle.Dispose();
            _captureThreadId = -1;
        }

        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        _disposed = true;
    }

    #endregion
}