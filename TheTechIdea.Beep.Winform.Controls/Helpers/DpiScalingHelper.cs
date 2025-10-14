﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Static helper class for DPI scaling operations that can be used by both controls and forms.
    /// Provides comprehensive DPI-aware scaling methods similar to ControlDpiHelper.
    /// </summary>
    public static class DpiScalingHelper
    {
        private const float StandardDpi = 96.0f;

        #region P/Invoke for DPI Detection

        [DllImport("user32.dll")]
        private static extern int GetDpiForWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDpiAwareness awareness);

        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);

        private enum ProcessDpiAwareness
        {
            ProcessDpiUnaware = 0,
            ProcessSystemDpiAware = 1,
            ProcessPerMonitorDpiAware = 2
        }

        private enum MonitorDpiType
        {
            EffectiveDpi = 0,
            AngularDpi = 1,
            RawDpi = 2
        }

        #endregion

        #region DPI Detection Methods

        /// <summary>
        /// Gets the DPI scale factor from a Control (preferred method)
        /// </summary>
        public static float GetDpiScaleFactor(Control control)
        {
            if (control == null || !control.IsHandleCreated)
                return 1f;

            // Try DeviceDpi property first (most reliable in .NET)
            if (control.DeviceDpi > 0)
                return control.DeviceDpi / StandardDpi;

            // Fallback to GetDpiForWindow
            try
            {
                int dpi = GetDpiForWindow(control.Handle);
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
            return Math.Max(g.DpiX, g.DpiY) / StandardDpi;
        }

        /// <summary>
        /// Gets the system-wide DPI scale factor
        /// </summary>
        public static float GetSystemDpiScaleFactor()
        {
            try
            {
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                    return g.DpiX / StandardDpi;
            }
            catch
            {
                return 1.0f;
            }
        }

        /// <summary>
        /// Gets the current DPI value from a Control
        /// </summary>
        public static int GetCurrentDpi(Control control)
        {
            if (control == null || !control.IsHandleCreated)
                return 96;

            if (control.DeviceDpi > 0)
                return control.DeviceDpi;

            try
            {
                int dpi = GetDpiForWindow(control.Handle);
                return dpi > 0 ? dpi : 96;
            }
            catch
            {
                return 96;
            }
        }

        /// <summary>
        /// Initializes DPI awareness for the application (call early in app startup)
        /// </summary>
        public static bool InitializeDpiAwareness()
        {
            try
            {
                // Try to set per-monitor DPI awareness (Windows 8.1+)
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    var result = SetProcessDpiAwareness(ProcessDpiAwareness.ProcessPerMonitorDpiAware);
                    if (result == 0) return true; // S_OK
                }

                // Fallback to system DPI aware
                return SetProcessDPIAware();
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Integer and Float Scaling

        /// <summary>
        /// Scales an integer value based on DPI scale factor
        /// </summary>
        public static int ScaleValue(int value, float dpiScaleFactor)
        {
            if (dpiScaleFactor == 1.0f) return value;
            return (int)Math.Round(value * dpiScaleFactor, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Scales a float value based on DPI scale factor
        /// </summary>
        public static float ScaleValue(float value, float dpiScaleFactor)
        {
            return value * dpiScaleFactor;
        }

        /// <summary>
        /// Scales an integer value using control's DPI
        /// </summary>
        public static int ScaleValue(int value, Control control)
        {
            return ScaleValue(value, GetDpiScaleFactor(control));
        }

        #endregion

        #region Size Scaling

        /// <summary>
        /// Scales a Size based on DPI scale factor
        /// </summary>
        public static Size ScaleSize(Size size, float dpiScaleFactor)
        {
            if (dpiScaleFactor == 1.0f) return size;
            return new Size(
                ScaleValue(size.Width, dpiScaleFactor),
                ScaleValue(size.Height, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a SizeF based on DPI scale factor
        /// </summary>
        public static SizeF ScaleSize(SizeF size, float dpiScaleFactor)
        {
            if (dpiScaleFactor == 1.0f) return size;
            return new SizeF(
                ScaleValue(size.Width, dpiScaleFactor),
                ScaleValue(size.Height, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a Size using control's DPI
        /// </summary>
        public static Size ScaleSize(Size size, Control control)
        {
            return ScaleSize(size, GetDpiScaleFactor(control));
        }

        #endregion

        #region Point Scaling

        /// <summary>
        /// Scales a Point based on DPI scale factor
        /// </summary>
        public static Point ScalePoint(Point point, float dpiScaleFactor)
        {
            if (dpiScaleFactor == 1.0f) return point;
            return new Point(
                ScaleValue(point.X, dpiScaleFactor),
                ScaleValue(point.Y, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a PointF based on DPI scale factor
        /// </summary>
        public static PointF ScalePoint(PointF point, float dpiScaleFactor)
        {
            if (dpiScaleFactor == 1.0f) return point;
            return new PointF(
                ScaleValue(point.X, dpiScaleFactor),
                ScaleValue(point.Y, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a Point using control's DPI
        /// </summary>
        public static Point ScalePoint(Point point, Control control)
        {
            return ScalePoint(point, GetDpiScaleFactor(control));
        }

        #endregion

        #region Rectangle Scaling

        /// <summary>
        /// Scales a Rectangle based on DPI scale factor
        /// </summary>
        public static Rectangle ScaleRectangle(Rectangle rect, float dpiScaleFactor)
        {
            if (dpiScaleFactor == 1.0f) return rect;
            return new Rectangle(
                ScaleValue(rect.X, dpiScaleFactor),
                ScaleValue(rect.Y, dpiScaleFactor),
                ScaleValue(rect.Width, dpiScaleFactor),
                ScaleValue(rect.Height, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a RectangleF based on DPI scale factor
        /// </summary>
        public static RectangleF ScaleRectangle(RectangleF rect, float dpiScaleFactor)
        {
            if (dpiScaleFactor == 1.0f) return rect;
            return new RectangleF(
                ScaleValue(rect.X, dpiScaleFactor),
                ScaleValue(rect.Y, dpiScaleFactor),
                ScaleValue(rect.Width, dpiScaleFactor),
                ScaleValue(rect.Height, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a Rectangle using control's DPI
        /// </summary>
        public static Rectangle ScaleRectangle(Rectangle rect, Control control)
        {
            return ScaleRectangle(rect, GetDpiScaleFactor(control));
        }

        #endregion

        #region Padding Scaling

        /// <summary>
        /// Scales Padding based on DPI scale factor
        /// </summary>
        public static Padding ScalePadding(Padding padding, float dpiScaleFactor)
        {
            if (dpiScaleFactor == 1.0f) return padding;
            return new Padding(
                ScaleValue(padding.Left, dpiScaleFactor),
                ScaleValue(padding.Top, dpiScaleFactor),
                ScaleValue(padding.Right, dpiScaleFactor),
                ScaleValue(padding.Bottom, dpiScaleFactor));
        }

        /// <summary>
        /// Scales Padding using control's DPI
        /// </summary>
        public static Padding ScalePadding(Padding padding, Control control)
        {
            return ScalePadding(padding, GetDpiScaleFactor(control));
        }

        #endregion

        #region Font Scaling

        /// <summary>
        /// Scales a Font based on DPI scale factor
        /// </summary>
        public static Font ScaleFont(Font font, float dpiScaleFactor)
        {
            if (font == null || dpiScaleFactor == 1.0f) return font;

            try
            {
                float newSize = font.Size * dpiScaleFactor;
                // Ensure minimum readable font size
                newSize = Math.Max(newSize, 6.0f);
                return new Font(font.FontFamily, newSize, font.Style, font.Unit);
            }
            catch
            {
                return font; // Return original font if scaling fails
            }
        }

        /// <summary>
        /// Scales a Font using control's DPI
        /// </summary>
        public static Font ScaleFont(Font font, Control control)
        {
            return ScaleFont(font, GetDpiScaleFactor(control));
        }

        /// <summary>
        /// Creates a scaled font from Graphics DPI
        /// </summary>
        public static Font ScaleFont(Font font, Graphics g)
        {
            return ScaleFont(font, GetDpiScaleFactor(g));
        }

        #endregion

        #region Convenience Methods

        /// <summary>
        /// Converts DPI value to scale factor
        /// </summary>
        public static float DpiToScaleFactor(int dpi)
        {
            return dpi / StandardDpi;
        }

        /// <summary>
        /// Converts scale factor to DPI value
        /// </summary>
        public static int ScaleFactorToDpi(float scaleFactor)
        {
            return (int)Math.Round(scaleFactor * StandardDpi);
        }

        /// <summary>
        /// Checks if two scale factors are effectively equal
        /// </summary>
        public static bool AreScaleFactorsEqual(float scale1, float scale2)
        {
            return Math.Abs(scale1 - scale2) < 0.01f;
        }

        #endregion
        #region Text utils
      
        #endregion
        #region Legacy Compatibility (old method names)

        /// <summary>
        /// Legacy method: Scales an integer (same as ScaleValue)
        /// </summary>
        public static int ScaleInt(int value, float scale) => ScaleValue(value, scale);

        /// <summary>
        /// Legacy method: Scales a Size (same as ScaleSize)
        /// </summary>
        public static Size Scale(Size size, float scale) => ScaleSize(size, scale);

        /// <summary>
        /// Legacy method: Scales a Point (same as ScalePoint)
        /// </summary>
        public static Point Scale(Point pt, float scale) => ScalePoint(pt, scale);

        /// <summary>
        /// Legacy method: Scales a Rectangle (same as ScaleRectangle)
        /// </summary>
        public static Rectangle Scale(Rectangle r, float scale) => ScaleRectangle(r, scale);

        #endregion
    }
}
