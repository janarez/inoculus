using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.Graphics.Gdi;
using static Windows.Win32.PInvoke;

namespace InOculus.Utilities
{
    class ThumbnailPreview : IDisposable
    {
        private const int WmDwmSendIconThumbnail = 0x0323;
        private const int WmDwmSendIconicLivePreviewBitmap = 0x0326;

        private readonly HWND hwnd;
        private readonly FrameworkElement element;
        private readonly HwndSource hwndSource;
        private readonly HwndSourceHook hook;
        private double scale = 1.0;

        public ThumbnailPreview(IntPtr windowHandle, FrameworkElement element)
        {
            hwnd = new HWND(windowHandle);
            this.element = element;
            hwndSource = HwndSource.FromHwnd(windowHandle);
            hook = HandleWindowsEvent;
            hwndSource.AddHook(hook);
        }

        public bool IsEnabled { get; private set; }

        public void Dispose()
        {
            hwndSource.RemoveHook(hook);
            hwndSource.Dispose();
        }

        public unsafe void Enable(bool enable)
        {
            if (IsEnabled != enable)
            {
                IsEnabled = enable;
                const int size = 4;
                IntPtr value = Marshal.AllocHGlobal(size);
                Marshal.WriteInt32(value, enable ? 1 : 0);
                try
                {
                    DwmSetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_FORCE_ICONIC_REPRESENTATION, value.ToPointer(), size).ThrowOnFailure();
                    DwmSetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_HAS_ICONIC_BITMAP, value.ToPointer(), size).ThrowOnFailure();
                }
                finally
                {
                    Marshal.FreeHGlobal(value);
                }
            }
        }

        public void Invalidate()
        {
            if (IsEnabled)
            {
                DwmInvalidateIconicBitmaps(hwnd).ThrowOnFailure();
            }
        }

        private IntPtr HandleWindowsEvent(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (!IsEnabled)
            {
                return IntPtr.Zero;
            }

            Debug.Assert(hwnd == this.hwnd);

            switch (msg)
            {
                case WmDwmSendIconThumbnail:
                    {
                        var (maxWidth, maxHeight) = (lParam.LoWord(), lParam.HiWord());
                        while (true)
                        {
                            var maxSize = new Size(maxWidth * scale, maxHeight * scale);
                            var image = element.TakeScreenshot();
                            var imageScale = Math.Min(maxSize.Width / image.Width, maxSize.Height / image.Height);
                            var scaledImage = new TransformedBitmap(image, new ScaleTransform(imageScale, imageScale));
                            var hbitmap = scaledImage.ToHbitmap();
                            try
                            {
                                var hresult = DwmSetIconicThumbnail(this.hwnd, hbitmap, 0);
                                if (hresult.Succeeded)
                                {
                                    break;
                                }

                                // Scale the image down until successful.
                                // Needed probably due to a Windows bug.
                                if (scale < 0.2) break;
                                scale -= 0.1;
                                Debug.WriteLine($"Scaled: {scale}");
                                continue;
                            }
                            finally
                            {
                                DeleteObject(new HGDIOBJ(hbitmap));
                            }
                        }
                        handled = true;
                        break;
                    }
                case WmDwmSendIconicLivePreviewBitmap:
                    unsafe
                    {
                        var image = element.TakeScreenshot();
                        var dpi = this.hwnd.GetDpi();
                        var dpiScale = dpi / 96.0;
                        var scaledImage = new TransformedBitmap(image, new ScaleTransform(dpiScale, dpiScale));
                        var hbitmap = scaledImage.ToHbitmap();
                        try
                        {
                            DwmSetIconicLivePreviewBitmap(this.hwnd, hbitmap, null, 0).ThrowOnFailure();
                        }
                        finally
                        {
                            DeleteObject(new HGDIOBJ(hbitmap));
                        }
                        handled = true;
                        break;
                    }
            }

            return IntPtr.Zero;
        }
    }
}
