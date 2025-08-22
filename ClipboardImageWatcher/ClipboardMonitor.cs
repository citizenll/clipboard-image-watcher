using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ClipboardImageWatcher
{
    public class ClipboardMonitor : IDisposable
    {
        private HwndSource _hwndSource;
        public event EventHandler? ClipboardChanged;

        public ClipboardMonitor()
        {
            _hwndSource = new HwndSource(new HwndSourceParameters());
            _hwndSource.AddHook(WndProc);
            NativeMethods.AddClipboardFormatListener(_hwndSource.Handle);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                ClipboardChanged?.Invoke(this, EventArgs.Empty);
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            NativeMethods.RemoveClipboardFormatListener(_hwndSource.Handle);
            _hwndSource.RemoveHook(WndProc);
            _hwndSource.Dispose();
        }

        private static class NativeMethods
        {
            internal const int WM_CLIPBOARDUPDATE = 0x031D;

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool AddClipboardFormatListener(IntPtr hwnd);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        }
    }
}