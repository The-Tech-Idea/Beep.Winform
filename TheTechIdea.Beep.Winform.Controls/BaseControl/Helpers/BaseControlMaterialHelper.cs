using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    /// <summary>
    /// Enhanced helper that renders comprehensive Material Design 3 styling for BaseControl.
    /// Provides field backgrounds, borders, state layers, elevation effects, and proper icon layout.
    /// </summary>
    internal sealed class BaseControlMaterialHelper
    {
        private readonly BaseControl _owner;
        private readonly BaseControlIconsHelper _icons;

        private Rectangle _inputRect;
        private Rectangle _fieldRect;
        private Rectangle _contentRect;

        // Material Design 3 color tokens
        private Color _surfaceColor;
        private Color _onSurfaceColor;
        private Color _surfaceVariantColor;
        private Color _outlineColor;
        private Color _primaryColor;
        private Color _errorColor;
        private Color _shadowColor;

        // Elevation properties
        private int _elevationLevel = 0;
        private bool _useElevation = true;

        // Icon offset tracking to prevent accumulation
        private int _previousLeftIconOffset = 0;
        private int _previousRightIconOffset = 0;

        public BaseControlMaterialHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _icons = new BaseControlIconsHelper(owner);
            Console.WriteLine("BaseControlMaterialHelper initialized.");
            UpdateColors();
            Console.WriteLine("Material colors updated.");
            UpdateLayout();
            Console.WriteLine("Material layout updated.");
        }

        public void UpdateLayout()
        {
            Console.WriteLine("Updating Material Layout");
            // Get the current DrawingRect - don't try to modify it
            var drawingRect = new Rectangle(0,0,_owner.Width,_owner.Height);
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            {
                Console.WriteLine("Invalid drawing rectangle dimensions.");
                return;
            }
            // Calculate padding based on Material Design variant
            int horizontalPadding = 16; // Standard Material Design padding
            int verticalPadding = 8;
            Console.WriteLine($"Initial Padding - H: {horizontalPadding}, V: {verticalPadding}");
            // Adjust padding based on variant
            switch (_owner.MaterialVariant)
            {
                case MaterialTextFieldVariant.Outlined:
                    horizontalPadding = 16;
                    verticalPadding = 8;
                    break;
                case MaterialTextFieldVariant.Filled:
                    horizontalPadding = 16;
                    verticalPadding = 12;
                    break;
                case MaterialTextFieldVariant.Standard:
                    horizontalPadding = 0;
                    verticalPadding = 8;
                    break;
            }
            Console.WriteLine($"Adjusted Padding - H: {horizontalPadding}, V: {verticalPadding}");
            // Input area (the full drawable area for this control)
            _inputRect = new Rectangle(0, 0, drawingRect.Width, drawingRect.Height);
            Console.WriteLine($"Input Rect: {_inputRect}");
            // Field area (input area minus padding)
            _fieldRect = new Rectangle(
                _inputRect.X + horizontalPadding,
                _inputRect.Y + verticalPadding,
                Math.Max(0, _inputRect.Width - (horizontalPadding * 2)),
                Math.Max(0, _inputRect.Height - (verticalPadding * 2))
            );
            Console.WriteLine($"Field Rect: {_fieldRect}");
            // Update icons layout within the field area
            _icons.UpdateLayout(_fieldRect);
            Console.WriteLine($"Leading Icon Rect: {_icons.LeadingRect}, Trailing Icon Rect: {_icons.TrailingRect}");
            // Content rectangle is the adjusted area that excludes icon spaces
            _contentRect = _icons.AdjustedContentRect;
            Console.WriteLine($"Content Rect: {_contentRect}");
            // If no icons, use the full field area
            if (_contentRect.IsEmpty)
            {
                Console.WriteLine("No icons present, using full field area for content.");
                _contentRect = new Rectangle(
                    _fieldRect.X + 4,
                    _fieldRect.Y + 4,
                    Math.Max(0, _fieldRect.Width - 8),
                    Math.Max(0, _fieldRect.Height - 8)
                );
            }
            Console.WriteLine($"Final Content Rect: {_contentRect}");
        }

        public void UpdateColors()
        {
            // Material Design 3 color scheme
            _surfaceColor = _owner.MaterialFillColor;
            _onSurfaceColor = _owner.ForeColor;
            _surfaceVariantColor = Color.FromArgb(0xEE, 0xEA, 0xF0); // Surface Variant
            _outlineColor = _owner.MaterialOutlineColor;
            _primaryColor = _owner.MaterialPrimaryColor;
            _errorColor = Color.FromArgb(0xBA, 0x1A, 0x1A); // Material Error
            _shadowColor = Color.FromArgb(0x1F, 0x00, 0x00, 0x00); // Shadow color with alpha
        }

        /// <summary>
        /// Comprehensive material design drawing method with elevation and proper icon layout
        /// </summary>
        public void DrawAll(Graphics g)
        {
            if (!_owner.EnableMaterialStyle) return;

            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 1. Draw elevation/shadow (behind everything)
            if (_useElevation && _elevationLevel > 0)
            {
                DrawElevation(g);
            }

            // 2. Draw field background
            DrawFieldBackground(g);

            // 3. Draw state layer (hover/press effects)
            DrawStateLayer(g);

            // 4. Draw border/outline
            DrawBorder(g);

            // 5. Draw icons
            _icons.Draw(g);

            // 6. Draw focus indicator
            if (_owner.Focused)
            {
                DrawFocusIndicator(g);
            }
        }

        private void DrawFieldBackground(Graphics g)
        {
            if (_fieldRect.Width <= 0 || _fieldRect.Height <= 0) return;

            using var brush = new SolidBrush(_surfaceColor);
            int radius = _owner.MaterialBorderRadius;

            if (radius > 0)
            {
                using var path = CreateRoundPath(_fieldRect, radius);
                g.FillPath(brush, path);
            }
            else
            {
                g.FillRectangle(brush, _fieldRect);
            }
        }

        private void DrawStateLayer(Graphics g)
        {
            if (!_owner.IsHovered && !_owner.IsPressed) return;

            float opacity = _owner.IsPressed ? 0.12f : 0.08f;
            Color stateColor = Color.FromArgb((int)(opacity * 255), _primaryColor);

            using var brush = new SolidBrush(stateColor);
            int radius = _owner.MaterialBorderRadius;

            if (radius > 0)
            {
                using var path = CreateRoundPath(_fieldRect, radius);
                g.FillPath(brush, path);
            }
            else
            {
                g.FillRectangle(brush, _fieldRect);
            }
        }

        private void DrawBorder(Graphics g)
        {
            if (_fieldRect.Width <= 0 || _fieldRect.Height <= 0) return;

            // Material Design 3.0 border logic
            bool hasError = !string.IsNullOrEmpty(_owner.ErrorText);
            bool isFocused = _owner.Focused;
            bool isHovered = _owner.IsHovered;

            Color borderColor;
            int borderWidth;

            if (hasError)
            {
                borderColor = _errorColor;
                borderWidth = isFocused ? 2 : 1;
            }
            else if (isFocused)
            {
                borderColor = _primaryColor;
                borderWidth = 2;
            }
            else if (isHovered)
            {
                borderColor = Color.FromArgb(0xA8, _onSurfaceColor);
                borderWidth = 1;
            }
            else
            {
                borderColor = _outlineColor;
                borderWidth = 1;
            }

            using var pen = new Pen(borderColor, borderWidth);
            int radius = _owner.MaterialBorderRadius;

            if (radius > 0)
            {
                using var path = CreateRoundPath(_fieldRect, radius);
                g.DrawPath(pen, path);
            }
            else
            {
                g.DrawRectangle(pen, _fieldRect);
            }
        }

        private void DrawFocusIndicator(Graphics g)
        {
            if (_fieldRect.Width <= 0 || _fieldRect.Height <= 0) return;

            // Material Design 3.0 focus ring
            int focusRingWidth = 2;
            Rectangle focusRect = new Rectangle(
                _fieldRect.X - focusRingWidth,
                _fieldRect.Y - focusRingWidth,
                _fieldRect.Width + (focusRingWidth * 2),
                _fieldRect.Height + (focusRingWidth * 2)
            );

            using var pen = new Pen(_primaryColor, focusRingWidth);
            pen.DashStyle = DashStyle.Solid;

            int radius = _owner.MaterialBorderRadius + focusRingWidth;

            if (radius > 0)
            {
                using var path = CreateRoundPath(focusRect, radius);
                g.DrawPath(pen, path);
            }
            else
            {
                g.DrawRectangle(pen, focusRect);
            }
        }

        /// <summary>
        /// Draws Material Design 3.0 elevation shadow effect
        /// </summary>
        private void DrawElevation(Graphics g)
        {
            if (_elevationLevel <= 0) return;

            // Material Design 3.0 elevation levels
            int shadowOffset = Math.Min(_elevationLevel, 5);
            int shadowBlur = _elevationLevel * 2;

            // Create shadow rectangles with increasing opacity
            for (int i = shadowBlur; i > 0; i--)
            {
                float alpha = (0.1f / shadowBlur) * i;
                Color shadow = Color.FromArgb((int)(alpha * 255), _shadowColor);

                Rectangle shadowRect = new Rectangle(
                    _fieldRect.X - shadowOffset + i,
                    _fieldRect.Y - shadowOffset + i,
                    _fieldRect.Width + (shadowOffset * 2) - (i * 2),
                    _fieldRect.Height + (shadowOffset * 2) - (i * 2)
                );

                if (shadowRect.Width > 0 && shadowRect.Height > 0)
                {
                    using var brush = new SolidBrush(shadow);
                    int radius = _owner.MaterialBorderRadius;

                    if (radius > 0)
                    {
                        using var path = CreateRoundPath(shadowRect, radius);
                        g.FillPath(brush, path);
                    }
                    else
                    {
                        g.FillRectangle(brush, shadowRect);
                    }
                }
            }
        }

        public void DrawIconsOnly(Graphics g)
        {
            if (!_owner.EnableMaterialStyle) return;
            _icons.Draw(g);
        }

        public Rectangle GetLeadingIconRect() => _icons.LeadingRect;
        public Rectangle GetTrailingIconRect() => _icons.TrailingRect;

        public Rectangle GetContentRect() => _contentRect;

        /// <summary>
        /// Gets the adjusted content rectangle that excludes icon areas
        /// </summary>
        public Rectangle GetAdjustedContentRect() => _icons.AdjustedContentRect;

        /// <summary>
        /// Sets the elevation level for Material Design 3.0 shadow effects
        /// </summary>
        public void SetElevation(int level)
        {
            _elevationLevel = Math.Max(0, Math.Min(level, 5)); // MD3 supports levels 0-5
        }

        /// <summary>
        /// Enables or disables elevation effects
        /// </summary>
        public void SetElevationEnabled(bool enabled)
        {
            _useElevation = enabled;
        }

        private static GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            int d = r * 2;

            if (r > 0)
            {
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }
            return path;
        }
    }
}
