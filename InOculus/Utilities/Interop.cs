using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using static Windows.Win32.PInvoke;

namespace InOculus.Utilities
{
    static class Interop
    {
        public static IntPtr GetWindowHandle(this Window window) => new WindowInteropHelper(window).EnsureHandle();
        public static int LoWord(this IntPtr ptr) => unchecked((int)((long)ptr >> 16));
        public static int HiWord(this IntPtr ptr) => unchecked((int)(((long)ptr) & (0xFFFF)));
        public static void ThrowIfFalse(this BOOL result)
        {
            if (!result)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
            }
        }

        /// <remarks>
        /// Inspired by <see href="https://stackoverflow.com/a/55499170"/>.
        /// </remarks>
        public static BitmapSource TakeScreenshot(this FrameworkElement element)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(element);
            var bitmap = new RenderTargetBitmap((int)(bounds.X + bounds.Width), (int)(bounds.Y + bounds.Height), 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                var brush = new VisualBrush(element);
                context.DrawRectangle(brush, null, bounds);
            }
            bitmap.Render(visual);
            return bitmap;
        }

        /// <remarks>
        /// Inspired by <see href="https://github.com/microsoft/Windows-classic-samples/blob/7cbd99ac1d2b4a0beffbaba29ea63d024ceff700/Samples/Win7Samples/winui/shell/appshellintegration/TabThumbnails/TabWnd.cpp#L265"/>
        /// and <see href="https://devblogs.microsoft.com/oldnewthing/20130225-00/?p=5153"/>.
        /// </remarks>
        public static unsafe HBITMAP ToHbitmap(this BitmapSource source)
        {
            var dcMem = CreateCompatibleDC(default(HDC));
            try
            {
                BITMAPINFO bmi;
                bmi.bmiHeader.biSize = (uint)sizeof(BITMAPINFOHEADER);
                bmi.bmiHeader.biWidth = source.PixelWidth;
                bmi.bmiHeader.biHeight = -source.PixelHeight; // minus for top-down bitmap
                bmi.bmiHeader.biPlanes = 1;
                bmi.bmiHeader.biBitCount = 32;
                bmi.bmiHeader.biCompression = BI_COMPRESSION.BI_RGB;
                byte* pbds;
                var hbitmap = CreateDIBSection(default, &bmi, DIB_USAGE.DIB_RGB_COLORS, (void**)&pbds, default, default);
                if (hbitmap.IsNull) throw new InvalidOperationException("Cannot create bitmap");

                source.CopyPixels(Int32Rect.Empty, (IntPtr)pbds, source.PixelWidth * source.PixelHeight * 4, source.PixelWidth * 4);

                return hbitmap;
            }
            finally
            {
                DeleteDC(dcMem);
            }
        }

        public static double GetDpi(this HWND hwnd)
        {
            var monitor = MonitorFromWindow(hwnd, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
            GetDpiForMonitor(monitor, Windows.Win32.UI.HiDpi.MONITOR_DPI_TYPE.MDT_RAW_DPI, out var dpi, out _).ThrowOnFailure();
            GetScaleFactorForMonitor(monitor, out var scaleFactor);
            return dpi * ((int)scaleFactor / 100.0);
        }
    }
}
