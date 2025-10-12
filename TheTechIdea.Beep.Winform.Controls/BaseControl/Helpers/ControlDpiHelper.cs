using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    internal class ControlDpiHelper
    {
        private readonly BaseControl _owner;
        private float _dpiScaleFactor = 1.0f;
        private int _currentDpi = 96; // Standard DPI
        private bool _isDpiAware = false;

        public ControlDpiHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            InitializeDpiAwareness();
        }

        #region DPI Awareness Initialization

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDpiAwareness awareness);

        [DllImport("user32.dll")]
        private static extern int GetDpiForWindow(IntPtr hWnd);

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

        private void InitializeDpiAwareness()
        {
            try
            {
                // Try to set per-monitor DPI awareness for .NET 8
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    var result = SetProcessDpiAwareness(ProcessDpiAwareness.ProcessPerMonitorDpiAware);
                    _isDpiAware = result == 0; // S_OK
                }

                // Fallback to system DPI aware
                if (!_isDpiAware)
                {
                    _isDpiAware = SetProcessDPIAware();
                }

                // Initialize with current DPI
                UpdateDpiFromControl();
            }
            catch (Exception)
            {
                // If DPI awareness setup fails, continue with default scaling
                _isDpiAware = false;
                _dpiScaleFactor = 1.0f;
            }
        }

        #endregion

        #region DPI Scaling Properties

        /// <summary>
        /// Current DPI scale factor (1.0 = 96 DPI, 1.25 = 120 DPI, 2.0 = 192 DPI, etc.)
        /// </summary>
        public float DpiScaleFactor
        {
            get => _dpiScaleFactor;
            private set
            {
                if (Math.Abs(_dpiScaleFactor - value) > 0.01f)
                {
                    _dpiScaleFactor = value;
                    OnDpiChanged();
                }
            }
        }

        /// <summary>
        /// Current DPI value (96, 120, 144, 192, etc.)
        /// </summary>
        public int CurrentDpi => _currentDpi;

        /// <summary>
        /// Whether the application is DPI aware
        /// </summary>
        public bool IsDpiAware => _isDpiAware;

        #endregion

        #region DPI Detection and Updates

        public void UpdateDpiScaling(Graphics g)
        {
            if (g == null) return;

            try
            {
                // Get DPI from Graphics object
                float dpiX = g.DpiX;
                float dpiY = g.DpiY;
                
                // Use the higher of the two for scaling
                float newDpi = Math.Max(dpiX, dpiY);
                _currentDpi = (int)Math.Round(newDpi);
                DpiScaleFactor = newDpi / 96.0f;
            }
            catch (Exception)
            {
                // Fallback to control-based detection
                UpdateDpiFromControl();
            }
        }

        /// <summary>
        /// Update DPI values from current control state
        /// Called when DPI change is detected or control is created
        /// Per Microsoft docs: Use DeviceDpi property and DPI change events
        /// </summary>
        public void UpdateDpi()
        {
            if (_owner?.Handle == IntPtr.Zero) return;

            try
            {
                // PREFERRED: Use DeviceDpi property (available in .NET Framework 4.7+/.NET Core)
                // This automatically reflects current monitor DPI for Per-Monitor V2 awareness
                if (_owner.DeviceDpi > 0)
                {
                    _currentDpi = _owner.DeviceDpi;
                    DpiScaleFactor = _owner.DeviceDpi / 96.0f;
                    return;
                }
            }
            catch { }

            try
            {
                // Fallback: Try to get DPI from window handle (Windows 10 1607+)
                int windowDpi = GetDpiForWindow(_owner.Handle);
                if (windowDpi > 0)
                {
                    _currentDpi = windowDpi;
                    DpiScaleFactor = windowDpi / 96.0f;
                    return;
                }
            }
            catch { }

            // Final fallback: use graphics from control
            try
            {
                using (var g = _owner.CreateGraphics())
                {
                    UpdateDpiScaling(g);
                }
            }
            catch
            {
                // Use default values if all else fails
                _currentDpi = 96;
                DpiScaleFactor = 1.0f;
            }
        }

        /// <summary>
        /// Legacy method - prefer UpdateDpi() which uses DeviceDpi property
        /// </summary>
        [Obsolete("Use UpdateDpi() instead - it uses the DeviceDpi property recommended by Microsoft")]
        public void UpdateDpiFromControl()
        {
            UpdateDpi();
        }

        private void OnDpiChanged()
        {
            // React to DPI change by refreshing owner layout and visuals
            try
            {
                // Update any internal layout rectangles
                _owner.UpdateDrawingRect();
                // Trigger a layout pass and redraw
                _owner.PerformLayout();
                _owner.Invalidate();
                // Notify listeners and derived controls
                _owner.OnDpiChangedInternal();
            }
            catch { /* best-effort */ }
        }

        #endregion

        #region Scaling Methods

        public int ScaleValue(int value)
        {
            if (_dpiScaleFactor == 1.0f) return value;
            return (int)Math.Round(value * _dpiScaleFactor);
        }

        public float ScaleValue(float value)
        {
            return value * _dpiScaleFactor;
        }

        public Size ScaleSize(Size size)
        {
            if (_dpiScaleFactor == 1.0f) return size;
            return new Size(
                ScaleValue(size.Width),
                ScaleValue(size.Height)
            );
        }

        public SizeF ScaleSize(SizeF size)
        {
            if (_dpiScaleFactor == 1.0f) return size;
            return new SizeF(
                ScaleValue(size.Width),
                ScaleValue(size.Height)
            );
        }

        public Rectangle ScaleRectangle(Rectangle rect)
        {
            if (_dpiScaleFactor == 1.0f) return rect;
            return new Rectangle(
                ScaleValue(rect.X),
                ScaleValue(rect.Y),
                ScaleValue(rect.Width),
                ScaleValue(rect.Height)
            );
        }

        public Padding ScalePadding(Padding padding)
        {
            if (_dpiScaleFactor == 1.0f) return padding;
            return new Padding(
                ScaleValue(padding.Left),
                ScaleValue(padding.Top),
                ScaleValue(padding.Right),
                ScaleValue(padding.Bottom)
            );
        }

        public Font ScaleFont(Font font)
        {
            if (font == null || _dpiScaleFactor == 1.0f) return font;
            
            try
            {
                // In .NET 8, font scaling should be more careful
                float newSize = font.Size * _dpiScaleFactor;
                
                // Ensure minimum readable font size
                newSize = Math.Max(newSize, 6.0f);
                
                return new Font(font.FontFamily, newSize, font.Style, font.Unit);
            }
            catch (Exception)
            {
                return font; // Return original font if scaling fails
            }
        }

        public void SafeApplyFont(Font newFont, bool preserveLocation = true)
        {
            if (newFont == null) return;

            Point originalLocation = _owner.Location;
            Size originalSize = _owner.Size;

            _owner.SuspendLayout();
            try
            {
                // In .NET 8, be more conservative about font scaling
                // The framework handles much of this automatically
                Font fontToApply = newFont;
                
                // Only scale if we're not at standard DPI and the font wasn't already scaled
                if (_dpiScaleFactor != 1.0f && !IsFontAlreadyScaled(newFont))
                {
                    fontToApply = ScaleFont(newFont);
                }

                _owner.Font = fontToApply;

                if (preserveLocation)
                {
                    _owner.Location = originalLocation;
                    _owner.Size = originalSize;
                }
            }
            finally
            {
                _owner.ResumeLayout(false);
            }
        }

        private bool IsFontAlreadyScaled(Font font)
        {
            // Simple heuristic: if font size is significantly larger than typical
            // it might already be DPI scaled
            return font.Size > 12.0f * _dpiScaleFactor;
        }

        #endregion

        #region Font Scaling Helper Methods

        public Font GetScaledFont(Graphics g, string text, Size maxSize, Font originalFont = null)
        {
            originalFont ??= _owner.Font;
            if (originalFont == null) return null;

            if (Fits(g, text, originalFont, maxSize))
                return originalFont;

            float minSize = 6.0f;
            float maxSizeToTest = originalFont.Size;
            float bestSize = minSize;

            // Binary search for the best fitting font size
            while ((maxSizeToTest - minSize) > 0.5f)
            {
                float midSize = (minSize + maxSizeToTest) / 2f;
                using (var testFont = new Font(originalFont.FontFamily, midSize, originalFont.Style))
                {
                    if (Fits(g, text, testFont, maxSize))
                    {
                        bestSize = midSize;
                        minSize = midSize;
                    }
                    else
                    {
                        maxSizeToTest = midSize;
                    }
                }
            }

            try
            {
                return new Font(originalFont.FontFamily, bestSize, originalFont.Style);
            }
            catch (Exception)
            {
                return originalFont;
            }
        }

        private bool Fits(Graphics g, string text, Font font, Size maxSize)
        {
            try
            {
                var measured = TextRenderer.MeasureText(g, text, font, maxSize, TextFormatFlags.WordBreak);
                return measured.Width <= maxSize.Width && measured.Height <= maxSize.Height;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Image Scaling

        public float GetScaleFactor(SizeF imageSize, Size targetSize, ImageScaleMode scaleMode)
        {
            if (imageSize.Width <= 0 || imageSize.Height <= 0 || targetSize.Width <= 0 || targetSize.Height <= 0)
                return 1.0f;

            float scaleX = targetSize.Width / imageSize.Width;
            float scaleY = targetSize.Height / imageSize.Height;

            return scaleMode switch
            {
                ImageScaleMode.Stretch => 1.0f, // No scaling needed, stretch to fill
                ImageScaleMode.KeepAspectRatioByWidth => scaleX,
                ImageScaleMode.KeepAspectRatioByHeight => scaleY,
                ImageScaleMode.KeepAspectRatio => Math.Min(scaleX, scaleY),
                _ => Math.Min(scaleX, scaleY)
            };
        }

        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect, ImageScaleMode scaleMode)
        {
            float scale = GetScaleFactor(imageSize, targetRect.Size, scaleMode);

            if (scale <= 0) return RectangleF.Empty;

            float newWidth, newHeight;

            if (scaleMode == ImageScaleMode.Stretch)
            {
                newWidth = targetRect.Width;
                newHeight = targetRect.Height;
            }
            else
            {
                newWidth = imageSize.Width * scale;
                newHeight = imageSize.Height * scale;
            }

            float xOffset = targetRect.X + (targetRect.Width - newWidth) / 2;
            float yOffset = targetRect.Y + (targetRect.Height - newHeight) / 2;

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }

        public RectangleF GetScaledBounds(SizeF imageSize, ImageScaleMode scaleMode)
        {
            var clientSize = _owner.ClientSize;
            return GetScaledBounds(imageSize, new Rectangle(0, 0, clientSize.Width, clientSize.Height), scaleMode);
        }

        public Size GetSuitableSizeForTextAndImage(Size imageSize, Size maxImageSize, TextImageRelation textImageRelation)
        {
            if (_owner.Font == null) return Size.Empty;

            Size textSize = TextRenderer.MeasureText(_owner.Text ?? "", _owner.Font);

            // Scale the image size if it exceeds maximum
            if (imageSize.Width > maxImageSize.Width || imageSize.Height > maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)maxImageSize.Width / imageSize.Width,
                    (float)maxImageSize.Height / imageSize.Height);

                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            // Apply DPI scaling to ensure proper sizing
            imageSize = ScaleSize(imageSize);
            textSize = ScaleSize(textSize);

            int spacing = ScaleValue(4); // 4px spacing between text and image
            var padding = ScalePadding(_owner.Padding);

            int width, height;

            switch (textImageRelation)
            {
                case TextImageRelation.ImageBeforeText:
                case TextImageRelation.TextBeforeImage:
                    width = imageSize.Width + textSize.Width + spacing + padding.Horizontal;
                    height = Math.Max(imageSize.Height, textSize.Height) + padding.Vertical;
                    break;

                case TextImageRelation.ImageAboveText:
                case TextImageRelation.TextAboveImage:
                    width = Math.Max(imageSize.Width, textSize.Width) + padding.Horizontal;
                    height = imageSize.Height + textSize.Height + spacing + padding.Vertical;
                    break;

                case TextImageRelation.Overlay:
                default:
                    width = Math.Max(imageSize.Width, textSize.Width) + padding.Horizontal;
                    height = Math.Max(imageSize.Height, textSize.Height) + padding.Vertical;
                    break;
            }

            return new Size(width, height);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Checks if the current system is using high DPI (>96 DPI)
        /// </summary>
        public bool IsHighDpi => _currentDpi > 96;

        /// <summary>
        /// Gets a DPI-appropriate margin/padding value
        /// </summary>
        public int GetStandardMargin() => ScaleValue(8);

        /// <summary>
        /// Gets a DPI-appropriate border width
        /// </summary>
        public int GetStandardBorderWidth() => Math.Max(1, ScaleValue(1));

        /// <summary>
        /// Gets DPI scale factor as a percentage (100%, 125%, 150%, 200%, etc.)
        /// </summary>
        public int GetScalePercentage() => (int)Math.Round(_dpiScaleFactor * 100);

        #endregion
    }
}