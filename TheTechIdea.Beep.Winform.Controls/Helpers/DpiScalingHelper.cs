using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Static helper class for DPI scaling operations that can be used by both BeepControl and BeepiForm
    /// </summary>
    public static class DpiScalingHelper
    {
        private const float StandardDpi = 96.0f;

        /// <summary>
        /// Gets the current DPI scale factor from a Graphics object
        /// </summary>
        public static float GetDpiScaleFactor(Graphics g)
        {
            if (g == null) return 1.0f;
            return g.DpiX / StandardDpi;
        }

        /// <summary>
        /// Gets the current DPI scale factor from a Control
        /// </summary>
        public static float GetDpiScaleFactor(Control control)
        {
            if (control == null || !control.IsHandleCreated)
                return 1.0f;

            using (Graphics g = control.CreateGraphics())
            {
                return GetDpiScaleFactor(g);
            }
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
        /// Scales a Font based on DPI
        /// </summary>
        public static Font ScaleFont(Font font, float dpiScaleFactor)
        {
            if (font == null) return SystemFonts.DefaultFont;
            return new Font(font.FontFamily, font.Size * dpiScaleFactor, font.Style);
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

        /// <summary>
        /// Gets the system DPI scale factor (Windows 10/11 method)
        /// </summary>
        public static float GetSystemDpiScaleFactor()
        {
            try
            {
                // Try to get system DPI awareness
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    return g.DpiX / StandardDpi;
                }
            }
            catch
            {
                return 1.0f; // Fallback
            }
        }
    }
}