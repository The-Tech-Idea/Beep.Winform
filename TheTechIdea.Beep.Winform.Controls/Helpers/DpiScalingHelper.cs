using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace  TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Static helper class for DPI scaling operations that can be used by both BeepControl and BeepiForm
    /// </summary>
    public static class DpiScalingHelper
    {
        private const float StandardDpi = 96.0f;
        // P/Invoke (fallback when DeviceDpi is not reliable or for non-Control contexts)
        [DllImport("User32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hWnd);

        public static float GetDpiScaleFactor(Control control)
        {
            if (control == null || !control.IsHandleCreated)
                return 1f;

            // Prefer DeviceDpi when available (WinForms exposes this per-control)
            if (control.DeviceDpi > 0)
                return control.DeviceDpi / StandardDpi;

            // Fallback to GetDpiForWindow
            try
            {
                var dpi = GetDpiForWindow(control.Handle);
                return dpi > 0 ? dpi / StandardDpi : 1f;
            }
            catch
            {
                return 1f;
            }
        }
        /// <summary>
        /// Gets the current DPI scale factor from a Graphics object
        /// </summary>
        public static float GetDpiScaleFactor(Graphics g)
        {
            if (g == null) return 1.0f;
            return g.DpiX / StandardDpi;
        }

        public static float GetSystemDpiScaleFactor()
        {
            // For process/system DPI awareness contexts (not per-monitor precise)
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
                return g.DpiX / StandardDpi;
        }

        public static int ScaleInt(int value, float scale) =>
            (int)Math.Round(value * scale, MidpointRounding.AwayFromZero);

        public static Size Scale(Size size, float scale) =>
            new Size(ScaleInt(size.Width, scale), ScaleInt(size.Height, scale));

        public static Point Scale(Point pt, float scale) =>
            new Point(ScaleInt(pt.X, scale), ScaleInt(pt.Y, scale));

        public static Rectangle Scale(Rectangle r, float scale) =>
            new Rectangle(ScaleInt(r.X, scale), ScaleInt(r.Y, scale),
                          ScaleInt(r.Width, scale), ScaleInt(r.Height, scale));

        public static Font ScaleFont(Font font, float scale)
        {
            if (font == null) return SystemFonts.DefaultFont;
            // Keep points; avoid double-scaling when AutoScaleMode=Font
            return new Font(font.FontFamily, font.SizeInPoints * scale, font.Style,
                            GraphicsUnit.Point, font.GdiCharSet, font.GdiVerticalFont);
        }
    

        /// <summary>
        /// Scales an integer value based on DPI
        /// </summary>
        public static int ScaleValue(int value, float dpiScaleFactor)
        {
            return (int)(value * dpiScaleFactor);
        }

        /// <summary>
        /// Scales a Size based on DPI
        /// </summary>
        public static Size ScaleSize(Size size, float dpiScaleFactor)
        {
            return new Size(
                ScaleValue(size.Width, dpiScaleFactor),
                ScaleValue(size.Height, dpiScaleFactor));
        }

       
        /// <summary>
        /// Scales a Rectangle based on DPI
        /// </summary>
        public static Rectangle ScaleRectangle(Rectangle rect, float dpiScaleFactor)
        {
            return new Rectangle(
                ScaleValue(rect.X, dpiScaleFactor),
                ScaleValue(rect.Y, dpiScaleFactor),
                ScaleValue(rect.Width, dpiScaleFactor),
                ScaleValue(rect.Height, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a Point based on DPI
        /// </summary>
        public static Point ScalePoint(Point point, float dpiScaleFactor)
        {
            return new Point(
                ScaleValue(point.X, dpiScaleFactor),
                ScaleValue(point.Y, dpiScaleFactor));
        }

      
    }
}