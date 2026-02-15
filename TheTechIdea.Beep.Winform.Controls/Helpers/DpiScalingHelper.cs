using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// DPI scaling helper with GDI-safe font management and .NET 8/9/10 PerMonitorV2 awareness.
    /// Critical fixes: Prevents GDI handle leaks, handles font units correctly (Point vs Pixel),
    /// and integrates with WinForms' modern scaling pipeline without double-scaling.
    /// </summary>
    public static class DpiScalingHelper
    {
        private const float StandardDpi = 96.0f;
        private const float MinScale = 0.1f;
        private const float Epsilon = 0.01f;

        #region Private Enums (Required before P/Invoke)

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

        #region P/Invoke for Modern DPI Awareness (Windows 10+)

        [DllImport("user32.dll")]
        private static extern int GetDpiForWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDpiAwareness awareness);

        [DllImport("user32.dll")]
        private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

        // DPI_AWARENESS_CONTEXT constants for Windows 10 1703+
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_UNAWARE = new IntPtr(-1);
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = new IntPtr(-2);
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = new IntPtr(-3);
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

        #endregion

        #region DPI Detection (Prioritize Modern APIs)

        /// <summary>
        /// Gets DPI scale factor using .NET's DeviceDpi (most reliable in .NET Core 3.0+ / .NET 5+).
        /// Falls back to GetDpiForWindow for older runtimes.
        /// </summary>
        public static float GetDpiScaleFactor(Control control)
        {
            if (control == null || !control.IsHandleCreated)
                return 1.0f;

            // Modern .NET: DeviceDpi is authoritative (updated on WM_DPICHANGED)
            if (control.DeviceDpi > 0)
                return control.DeviceDpi / StandardDpi;

            // Fallback for older frameworks
            try
            {
                int dpi = GetDpiForWindow(control.Handle);
                return dpi > 0 ? dpi / StandardDpi : 1.0f;
            }
            catch
            {
                return 1.0f;
            }
        }

        /// <summary>
        /// Gets DPI scale factor from a Graphics object.
        /// WARNING: Graphics.DpiX can return incorrect values (e.g., printer DPI during print preview).
        /// Per Microsoft guidance: Prefer GetDpiScaleFactor(Control) when available.
        /// Only use this overload when Control reference is not available (e.g., in IBaseControlPainter.Draw).
        /// </summary>
        /// <param name="g">Graphics object (typically from PaintEventArgs)</param>
        /// <returns>DPI scale factor (1.0 = 96 DPI, 1.5 = 144 DPI, etc.)</returns>
        public static float GetDpiScaleFactor(Graphics g)
        {
            if (g == null) return 1.0f;
            return g.DpiX / StandardDpi;
        }

        /// <summary>
        /// Gets system-wide DPI scale factor at application startup.
        /// Use this when no Control instance is available (e.g., during static initialization).
        /// Per Microsoft guidance: Prefer GetDpiScaleFactor(Control) for per-monitor DPI scenarios.
        /// </summary>
        /// <returns>System DPI scale factor (1.0 = 96 DPI, 1.5 = 144 DPI, etc.)</returns>
        public static float GetSystemDpiScaleFactor()
        {
            try
            {
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    return g.DpiX / StandardDpi;
                }
            }
            catch
            {
                return 1.0f;
            }
        }

        /// <summary>
        /// Gets current DPI value (96 = 100%, 144 = 150%, etc.)
        /// </summary>
        public static int GetCurrentDpi(Control control)
        {
            if (control == null || !control.IsHandleCreated)
                return 96;

            return control.DeviceDpi > 0 ? control.DeviceDpi : 96;
        }

        /// <summary>
        /// Initializes PerMonitorV2 DPI awareness at application startup (call BEFORE any UI creation).
        /// Per Microsoft guidance: Prefer app.manifest over runtime calls where possible.
        /// </summary>
        public static bool InitializeDpiAwareness()
        {
            try
            {
                // Windows 10 1703+ supports PerMonitorV2 context
                if (Environment.OSVersion.Version >= new Version(10, 0, 15063))
                {
                    var prevContext = SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                    return prevContext != IntPtr.Zero && prevContext != new IntPtr(-1); // -1 = failure
                }

                // Windows 8.1+: Per-monitor awareness (no V2 improvements)
                if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 3)
                {
                    return SetProcessDpiAwareness(ProcessDpiAwareness.ProcessPerMonitorDpiAware) == 0;
                }

                // Windows Vista+: System DPI awareness
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
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return value;
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

        /// <summary>
        /// Scales a float value using control's DPI
        /// </summary>
        public static float ScaleValue(float value, Control control)
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
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return size;
            return new Size(
                ScaleValue(size.Width, dpiScaleFactor),
                ScaleValue(size.Height, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a SizeF based on DPI scale factor
        /// </summary>
        public static SizeF ScaleSize(SizeF size, float dpiScaleFactor)
        {
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return size;
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
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return point;
            return new Point(
                ScaleValue(point.X, dpiScaleFactor),
                ScaleValue(point.Y, dpiScaleFactor));
        }

        /// <summary>
        /// Scales a PointF based on DPI scale factor
        /// </summary>
        public static PointF ScalePoint(PointF point, float dpiScaleFactor)
        {
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return point;
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
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return rect;
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
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return rect;
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
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return padding;
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

        #region Safe Font Scaling (GDI Leak Prevention)

        /// <summary>
        /// Safely scales a font for DPI changes with proper GraphicsUnit handling per Microsoft guidance.
        /// 
        /// MICROSOFT GUIDANCE (https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support):
        /// - GraphicsUnit.Point: Physical unit (1/72 inch) that WinForms auto-scales. Recommended for standard controls.
        /// - GraphicsUnit.Pixel: Device-dependent unit requiring manual DPI scaling. Use for custom painters.
        /// 
        /// WHEN TO USE:
        /// - Point fonts: Standard WinForms controls (Button, Label, etc.) - framework handles DPI automatically
        /// - Pixel fonts: Custom-painted controls (OnPaint overrides) - you control scaling manually
        /// 
        /// CRITICAL FIXES:
        /// - Handles GraphicsUnit.Point conversion (96/72 ratio) like in BaseControl font caching
        /// - Multi-level fallback for custom font families (icon fonts) that throw ArgumentException 
        /// - Prevents GDI leaks with try-catch on font creation
        /// </summary>
        /// <param name="font">Font to scale (must not be null)</param>
        /// <param name="scaleFactor">DPI scale factor (e.g., 1.5 for 150%, 2.0 for 200%)</param>
        /// <returns>New scaled Font instance. Caller MUST dispose old font if replacing.</returns>
        public static Font ScaleFont(Font font, float scaleFactor)
        {
            if (font == null || scaleFactor <= Epsilon || Math.Abs(scaleFactor - 1.0f) < Epsilon)
                return font;

            try
            {                // Calculate target size based on font unit
                float targetSize;
                GraphicsUnit targetUnit;

                if (font.Unit == GraphicsUnit.Point)
                {
                    // CRITICAL: GraphicsUnit.Point is DPI-independent (1/72 inch)
                    // Per Microsoft: Convert Point → Pixel for scaling → back to Point
                    // This prevents double-scaling when WinForms auto-scales Point fonts
                    float sizeInPixels = font.Size * scaleFactor * (StandardDpi / 72.0f);
                    targetSize = sizeInPixels * (72.0f / StandardDpi) / scaleFactor;
                    targetUnit = GraphicsUnit.Point;
                }
                else if (font.Unit == GraphicsUnit.Pixel)
                {
                    // Pixel fonts: Direct scaling (manual DPI control)
                    targetSize = font.Size * scaleFactor;
                    targetUnit = GraphicsUnit.Pixel;
                }
                else
                {
                    // Other units (World, Inch, etc.): Scale size, preserve unit
                    targetSize = font.Size * scaleFactor;
                    targetUnit = font.Unit;
                }

                // Enforce minimum readable size
                targetSize = Math.Max(targetSize, 6.0f);

                // Try creating font with original font family
                try
                {
                    return new Font(font.FontFamily, targetSize, font.Style, targetUnit, 
                        font.GdiCharSet, font.GdiVerticalFont);
                }
                catch (ArgumentException)
                {
                    // FALLBACK 1: Icon fonts and some custom families throw ArgumentException at certain sizes
                    // Try with GenericSansSerif family but original style
                    try
                    {
                        return new Font(FontFamily.GenericSansSerif, targetSize, font.Style, targetUnit);
                    }
                    catch
                    {
                        // FALLBACK 2: Try with font name as string
                        try
                        {
                            return new Font(font.Name, targetSize, font.Style, targetUnit);
                        }
                        catch
                        {
                            // FALLBACK 3: Return original font (better than crash)
                            return font;
                        }
                    }
                }
            }
            catch
            {
                // Final fallback: return original font
                return font;
            }
        }

        /// <summary>
        /// Converts point size to pixel size for a given DPI scale factor.
        /// Per Microsoft: 1 point = 1/72 inch. At 96 DPI (100%): 1 point = 96/72 = 1.333 pixels.
        /// At 144 DPI (150%): The scale factor is 1.5, so calculation accounts for both DPI and point→pixel conversion.
        /// </summary>
        /// <param name="points">Size in points (typography unit, device-independent)</param>
        /// <param name="dpiScaleFactor">DPI scale factor (1.0 = 96 DPI, 1.5 = 144 DPI, etc.)</param>
        /// <returns>Size in pixels (device-dependent)</returns>
        public static float PointsToPixels(float points, float dpiScaleFactor)
        {
            // Point to Pixel conversion: points * (DPI / 72)
            // At 96 DPI (1.0 scale): points * (96/72) = points * 1.333
            // At 144 DPI (1.5 scale): points * (144/72) = points * 2.0
            return points * dpiScaleFactor * (StandardDpi / 72.0f);
        }

        /// <summary>
        /// Converts pixel size to point size for a given DPI scale factor.
        /// Inverse of PointsToPixels - use when converting physical pixels back to typography units.
        /// </summary>
        /// <param name="pixels">Size in pixels (device-dependent)</param>
        /// <param name="dpiScaleFactor">DPI scale factor (1.0 = 96 DPI, 1.5 = 144 DPI, etc.)</param>
        /// <returns>Size in points (typography unit, device-independent)</returns>
        public static float PixelsToPoints(float pixels, float dpiScaleFactor)
        {
            // Pixel to Point conversion: pixels * (72 / DPI)
            return pixels / (dpiScaleFactor * (StandardDpi / 72.0f));
        }

        /// <summary>
        /// Safely replaces a control's font with a scaled version while preventing GDI handle leaks.
        /// DISPOSES the old font ONLY if it was explicitly set on this control (not inherited).
        /// </summary>
        /// <returns>True if font was replaced; false if skipped (inherited font or no change)</returns>
        public static bool TryReplaceFontSafely(Control control, Font newFont, bool disposeOld = true)
        {
            if (control == null || newFont == null || control.IsDisposed || !control.IsHandleCreated)
                return false;

            Font oldFont = control.Font;
            if (oldFont == null || ReferenceEquals(oldFont, newFont))
                return false;

            // Skip if font is inherited from parent (prevents double-scaling)
            if (IsFontInherited(control, oldFont))
                return false;

            try
            {
                control.Font = newFont;

                if (disposeOld && 
                    oldFont != Control.DefaultFont && 
                    oldFont != SystemFonts.DefaultFont &&
                    !IsSystemFont(oldFont))
                {
                    oldFont.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if a font is inherited from parent container (vs explicitly set).
        /// Prevents double-scaling when parent containers also scale fonts.
        /// </summary>
        public static bool IsFontInherited(Control control, Font font)
        {
            if (control?.Parent == null || font == null)
                return false;

            // Heuristic: Compare with parent's current font
            return control.Parent.Font != null && font.Equals(control.Parent.Font);
        }

        private static bool IsSystemFont(Font font)
        {
            return font.Equals(SystemFonts.DefaultFont) ||
                   font.Equals(SystemFonts.MessageBoxFont) ||
                   font.Equals(Control.DefaultFont);
        }

        #endregion

        #region Coordinate Conversion (Logical ↔ Physical)

        /// <summary>
        /// Converts logical coordinates (design-time 96 DPI) to physical coordinates (current DPI).
        /// Use when painting custom graphics that were designed at 100% DPI.
        /// </summary>
        public static int LogicalToPhysical(int logicalValue, float dpiScaleFactor)
        {
            return ScaleValue(logicalValue, dpiScaleFactor);
        }

        /// <summary>
        /// Converts physical coordinates (current DPI) to logical coordinates (96 DPI baseline).
        /// Use when saving/restoring layout positions across DPI changes.
        /// </summary>
        public static int PhysicalToLogical(int physicalValue, float dpiScaleFactor)
        {
            if (Math.Abs(dpiScaleFactor - 1.0f) < Epsilon) return physicalValue;
            return (int)Math.Round(physicalValue / dpiScaleFactor, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region Per-Monitor DPI Change Handling (Modern .NET)

        /// <summary>
        /// Refreshes DPI scale factors using DeviceDpi (authoritative in .NET Core 3.0+).
        /// Call in OnDpiChangedAfterParent override after base.OnDpiChangedAfterParent().
        /// </summary>
        public static void RefreshScaleFactors(Control control, ref float scaleX, ref float scaleY)
        {
            if (control == null || !control.IsHandleCreated)
            {
                scaleX = scaleY = 1.0f;
                return;
            }

            try
            {
                // Modern .NET: DeviceDpi is updated automatically on WM_DPICHANGED
                if (control.DeviceDpi > 0)
                {
                    float scale = Math.Max(control.DeviceDpi / StandardDpi, MinScale);
                    scaleX = scaleY = scale;
                    return;
                }

                // Fallback for older runtimes
                using (var g = control.CreateGraphics())
                {
                    scaleX = Math.Max(g.DpiX / StandardDpi, MinScale);
                    scaleY = Math.Max(g.DpiY / StandardDpi, MinScale);
                }
            }
            catch
            {
                scaleX = scaleY = 1.0f;
            }
        }

        /// <summary>
        /// Extracts old/new DPI scales from DpiChangedEventArgs with safe fallbacks.
        /// </summary>
        public static void GetScalesFromDpiChangedEvent(
            DpiChangedEventArgs e,
            float fallbackOldScaleX,
            float fallbackOldScaleY,
            out float oldScaleX,
            out float oldScaleY,
            out float newScaleX,
            out float newScaleY)
        {
            oldScaleX = fallbackOldScaleX > 0 ? fallbackOldScaleX : 1f;
            oldScaleY = fallbackOldScaleY > 0 ? fallbackOldScaleY : 1f;
            newScaleX = oldScaleX;
            newScaleY = oldScaleY;

            if (e == null)
                return;

            try
            {
                oldScaleX = Math.Max(e.DeviceDpiOld / StandardDpi, MinScale);
                oldScaleY = oldScaleX;
                newScaleX = Math.Max(e.DeviceDpiNew / StandardDpi, MinScale);
                newScaleY = newScaleX;
                return;
            }
            catch
            {
                // Fall through to reflection-based compatibility path.
            }

            try
            {
                var dpiScaleProperty = e.GetType().GetProperty("DpiScale");
                var dpiScale = dpiScaleProperty?.GetValue(e);
                if (dpiScale != null)
                {
                    var xProp = dpiScale.GetType().GetProperty("X");
                    var yProp = dpiScale.GetType().GetProperty("Y");
                    if (xProp?.GetValue(dpiScale) is float sx && yProp?.GetValue(dpiScale) is float sy)
                    {
                        newScaleX = Math.Max(sx, MinScale);
                        newScaleY = Math.Max(sy, MinScale);
                    }
                }
            }
            catch
            {
                // Keep fallback values.
            }
        }

        /// <summary>
        /// Scales child controls during DPI changes WITHOUT scaling fonts by default (prevents double-scaling).
        /// </summary>
        public static void ScaleControlTreeForDpiChange(
            Control root,
            float oldScaleX,
            float oldScaleY,
            float newScaleX,
            float newScaleY,
            bool scaleFont = false, // DEFAULT FALSE to avoid double-scaling with font replacement
            bool scalePaddingAndMargin = true)
        {
            if (root == null) return;

            float ratioX = oldScaleX > Epsilon ? newScaleX / oldScaleX : 1.0f;
            float ratioY = oldScaleY > Epsilon ? newScaleY / oldScaleY : 1.0f;

            if (Math.Abs(ratioX - 1.0f) < Epsilon && Math.Abs(ratioY - 1.0f) < Epsilon)
                return;

            root.SuspendLayout();
            try
            {
                ScaleControlChildrenRecursive(root, ratioX, ratioY, scaleFont, scalePaddingAndMargin);
            }
            finally
            {
                root.ResumeLayout(true);
            }
        }

        private static void ScaleControlChildrenRecursive(
            Control parent,
            float ratioX,
            float ratioY,
            bool scaleFont,
            bool scalePaddingAndMargin)
        {
            foreach (Control child in parent.Controls)
            {
                if (scalePaddingAndMargin)
                {
                    child.Padding = ScalePaddingXY(child.Padding, ratioX, ratioY);
                    child.Margin = ScalePaddingXY(child.Margin, ratioX, ratioY);
                }

                // CRITICAL: Only scale font if EXPLICITLY requested (default false to prevent double-scaling)
                // Per Microsoft: ScaleFont now handles GraphicsUnit.Point vs Pixel correctly
                if (scaleFont && child.Font != null && !IsFontInherited(child, child.Font))
                {
                    float scaleFactor = (ratioX + ratioY) * 0.5f;
                    Font scaled = ScaleFont(child.Font, scaleFactor);
                    TryReplaceFontSafely(child, scaled, disposeOld: true);
                }

                if (ShouldScaleBounds(child))
                {
                    var bounds = child.Bounds;
                    child.Bounds = new Rectangle(
                        ScaleValue(bounds.X, ratioX),
                        ScaleValue(bounds.Y, ratioY),
                        ScaleValue(bounds.Width, ratioX),
                        ScaleValue(bounds.Height, ratioY));
                }

                if (child.Controls.Count > 0)
                {
                    ScaleControlChildrenRecursive(child, ratioX, ratioY, scaleFont, scalePaddingAndMargin);
                }

                child.Invalidate();
            }
        }

        private static bool ShouldScaleBounds(Control control)
        {
            if (control == null) return false;
            if (control.Dock != DockStyle.None) return false;

            // Only scale controls with simple top-left anchoring
            var anchor = control.Anchor;
            return anchor == AnchorStyles.None ||
                   anchor == AnchorStyles.Top ||
                   anchor == AnchorStyles.Left ||
                   anchor == (AnchorStyles.Top | AnchorStyles.Left);
        }

        private static Padding ScalePaddingXY(Padding padding, float scaleX, float scaleY)
        {
            return new Padding(
                ScaleValue(padding.Left, scaleX),
                ScaleValue(padding.Top, scaleY),
                ScaleValue(padding.Right, scaleX),
                ScaleValue(padding.Bottom, scaleY));
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
            return Math.Abs(scale1 - scale2) < Epsilon;
        }

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