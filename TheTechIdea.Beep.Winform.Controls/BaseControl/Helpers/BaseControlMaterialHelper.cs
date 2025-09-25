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

        // Material Design 3 color tokens (kept in sync with owner at draw time)
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

        public BaseControlMaterialHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _icons = new BaseControlIconsHelper(owner);
            UpdateColors();
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            // Get the current DrawingRect - don't try to modify it
            var drawingRect = new Rectangle(0, 0, _owner.Width, _owner.Height);
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            {
                _inputRect = Rectangle.Empty;
                _fieldRect = Rectangle.Empty;
                _contentRect = Rectangle.Empty;
                return;
            }

            // Default MD paddings
            int horizontalPadding = 16;
            int verticalPadding = 8;
            switch (_owner.MaterialVariant)
            {
                case MaterialTextFieldVariant.Outlined:
                    horizontalPadding = 16; verticalPadding = 8; break;
                case MaterialTextFieldVariant.Filled:
                    horizontalPadding = 16; verticalPadding = 12; break;
                case MaterialTextFieldVariant.Standard:
                    horizontalPadding = 0; verticalPadding = 0; break;
            }

            // For pill styles we want the outline to hug the control bounds (no inner inset)
            bool isPillPreset = _owner.StylePreset == MaterialTextFieldStylePreset.PillOutlined
                                || _owner.StylePreset == MaterialTextFieldStylePreset.PillFilled
                                || (_owner.MaterialBorderRadius >= Math.Min(_owner.Width, _owner.Height) / 2);

            try
            {
                if (!_owner.MaterialUseVariantPadding || isPillPreset)
                {
                    horizontalPadding = 0;
                    verticalPadding = 0;
                }

                if (_owner.MaterialCustomPadding != Padding.Empty)
                {
                    horizontalPadding = Math.Max(0, _owner.MaterialCustomPadding.Left);
                    verticalPadding = Math.Max(0, _owner.MaterialCustomPadding.Top);
                }
            }
            catch { }

            _inputRect = new Rectangle(0, 0, drawingRect.Width, drawingRect.Height);

            // Reserve vertical space for label (top) and supporting text (bottom)
            int labelReserve = 0;
            int bottomReserve = 0;
            try
            {
                using var g = _owner.CreateGraphics();
                if (!string.IsNullOrEmpty(_owner.LabelText))
                {
                    // approx floating label height
                    int lblH = TextRenderer.MeasureText(g, "Ag", _owner.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    labelReserve = Math.Max(10, (int)Math.Ceiling(lblH * 0.75)) + 2; // small offset
                }
                // Supporting text prefers error text over helper
                string supporting = !string.IsNullOrEmpty(_owner.ErrorText) ? _owner.ErrorText : _owner.HelperText;
                if (!string.IsNullOrEmpty(supporting))
                {
                    using var supportFont = new Font(_owner.Font.FontFamily, Math.Max(8f, _owner.Font.Size - 1f));
                    int supH = TextRenderer.MeasureText(g, "Ag", supportFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    bottomReserve = supH + 4; // leave distance from border
                }
            }
            catch { /* fallback to zeros on design-time errors */ }

            // Compose field rectangle inside the reserved areas
            int fieldX = _inputRect.X + horizontalPadding;
            int fieldY = _inputRect.Y + verticalPadding + labelReserve;
            int fieldW = Math.Max(0, _inputRect.Width - (horizontalPadding * 2));
            int fieldH = Math.Max(0, _inputRect.Height - (verticalPadding * 2) - labelReserve - bottomReserve);
            _fieldRect = new Rectangle(fieldX, fieldY, fieldW, fieldH);

            // Layout icons inside the field area
            _icons.UpdateLayout(_fieldRect);
            _contentRect = _icons.AdjustedContentRect;
            if (_contentRect.IsEmpty || _contentRect.Width <= 0 || _contentRect.Height <= 0)
            {
                _contentRect = _fieldRect;
            }
        }

        public void UpdateColors()
        {
            // Refresh tokens from owner every time to reflect live property changes
            // Surface (background) color: show fill only for Filled variant or when explicitly requested
            bool shouldFill = _owner.MaterialVariant == MaterialTextFieldVariant.Filled || _owner.MaterialShowFill;
            _surfaceColor = shouldFill ? _owner.MaterialFillColor : _owner.BackColor;

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

            // Always sync with latest owner properties
            UpdateColors();

            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 1. Draw elevation/shadow (behind everything)
            if (_useElevation && _elevationLevel > 0)
            {
                DrawElevation(g);
            }

            // 2. Draw field background (only visible for Filled or when MaterialShowFill=true)
            DrawFieldBackground(g);

            // 3. Draw state layer (hover/press effects)
            DrawStateLayer(g);

            // 4. Draw border/outline
            if (!_owner.IsFrameless)
            {
                DrawBorder(g);
            }

            // Draw icons after background/border so backgrounds embedded in SVGs are visible
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

            // Skip drawing a fill if variant is not Filled and MaterialShowFill is false
            bool shouldFill = _owner.MaterialVariant == MaterialTextFieldVariant.Filled || _owner.MaterialShowFill;
            if (!shouldFill) return;

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

            switch (_owner.MaterialVariant)
            {
                case MaterialTextFieldVariant.Standard:
                case MaterialTextFieldVariant.Filled:
                    // Text-button style underline
                    g.DrawLine(pen, _fieldRect.Left, _fieldRect.Bottom, _fieldRect.Right, _fieldRect.Bottom);
                    break;

                case MaterialTextFieldVariant.Outlined:
                    if (radius > 0)
                    {
                        using var path = CreateRoundPath(_fieldRect, radius);
                        g.DrawPath(pen, path);
                    }
                    else
                    {
                        g.DrawRectangle(pen, _fieldRect);
                    }
                    break;
            }

            // Maintain InnerShape for other helpers (shadow/ink etc.)
            _owner.InnerShape?.Dispose();
            _owner.InnerShape = new GraphicsPath();

            var innerRectOutlined = new Rectangle(
                _fieldRect.X + borderWidth,
                _fieldRect.Y + borderWidth,
                Math.Max(0, _fieldRect.Width - (borderWidth * 2)),
                Math.Max(0, _fieldRect.Height - (borderWidth * 2)));

            if (innerRectOutlined.Width > 0 && innerRectOutlined.Height > 0)
            {
                if (radius > 0)
                {
                    int innerRadius = Math.Max(0, radius - borderWidth);
                    _owner.InnerShape.AddPath(CreateRoundPath(innerRectOutlined, innerRadius), false);
                }
                else
                {
                    _owner.InnerShape.AddRectangle(innerRectOutlined);
                }
            }
        }

        private void DrawFocusIndicator(Graphics g)
        {
            // MD3 text fields do not use an outer focus ring. Focus is indicated by:
            // - Outlined: border color switches to Primary and width increases (handled in DrawBorder)
            // - Filled/Standard: bottom indicator becomes thicker (handled in DrawBorder)
            // Keep method as no-op to avoid extra halo around the field.
            return;
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
        public Rectangle GetFieldRect() => _fieldRect;
        public Rectangle GetInputRect() => _inputRect;

        /// <summary>
        /// Gets a reasonable rectangle for supporting text (helper/error) under the field.
        /// </summary>
        public Rectangle GetSupportingTextRect(int textHeight)
        {
            int y = Math.Min(_owner.Height - textHeight - 1, _fieldRect.Bottom + 4);
            return new Rectangle(Math.Max(4, _fieldRect.Left), y, Math.Max(10, _fieldRect.Width - 8), textHeight);
        }

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
