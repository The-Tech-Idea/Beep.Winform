using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Button painter: provides clean button styling with border and shadow
    /// while leaving inner content (DrawingRect) for inheriting controls to handle.
    /// Based on the simple button design with border and subtle shadow.
    /// </summary>
    internal sealed class ButtonBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _buttonRect;
        private Rectangle _drawingRect;

        // Layout constants matching the design
        private const int BUTTON_PADDING = 4;
        private const int CONTENT_PADDING = 12;
        private const int BORDER_RADIUS = 4;
        private const int SHADOW_OFFSET = 2;
        private const int BORDER_WIDTH = 2;

        // Reserved label/helper space
        private int _reserveTop;
        private int _reserveBottom;

        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _buttonRect;
        public Rectangle ContentRect => _drawingRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            _reserveTop = 0;
            _reserveBottom = 0;

            if (owner == null || owner.Width <= 0 || owner.Height <= 0)
            {
                _buttonRect = Rectangle.Empty;
                _drawingRect = Rectangle.Empty;
                return;
            }

           

            // Main button rectangle with padding for shadow and reserved label/helper
            _buttonRect = new Rectangle(
                BUTTON_PADDING,
                BUTTON_PADDING + _reserveTop,
                owner.Width - (BUTTON_PADDING * 2) - SHADOW_OFFSET,
                owner.Height - (BUTTON_PADDING * 2) - SHADOW_OFFSET - _reserveTop - _reserveBottom
            );

            if (_buttonRect.Width <= 0 || _buttonRect.Height <= 0) 
            {
                _drawingRect = Rectangle.Empty;
                return;
            }

            // DrawingRect for inheriting controls (inside button with content padding)
            _drawingRect = new Rectangle(
                _buttonRect.X + CONTENT_PADDING + BORDER_WIDTH,
                _buttonRect.Y + CONTENT_PADDING + BORDER_WIDTH,
                _buttonRect.Width - (CONTENT_PADDING * 2) - (BORDER_WIDTH * 2),
                _buttonRect.Height - (CONTENT_PADDING * 2) - (BORDER_WIDTH * 2)
            );

            // Ensure DrawingRect is valid
            if (_drawingRect.Width <= 0 || _drawingRect.Height <= 0)
            {
                _drawingRect = Rectangle.Empty;
            }
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null || _buttonRect.IsEmpty) return;

            // Enable high-quality rendering
            var oldSmoothingMode = g.SmoothingMode;
            var oldInterpolationMode = g.InterpolationMode;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            try
            {
                // 1. Draw button shadow
                DrawButtonShadow(g, owner);

                // 2. Draw button background and border
                DrawButton(g, owner);

              
            }
            finally
            {
                // Restore graphics state
                g.SmoothingMode = oldSmoothingMode;
                g.InterpolationMode = oldInterpolationMode;
            }
        }

        private void DrawButtonShadow(Graphics g, Base.BaseControl owner)
        {
            Rectangle shadowRect = new Rectangle(
                _buttonRect.X + SHADOW_OFFSET, 
                _buttonRect.Y + SHADOW_OFFSET, 
                _buttonRect.Width, 
                _buttonRect.Height
            );

            Color shadowColor = owner._currentTheme?.ShadowColor ?? Color.FromArgb(40, Color.Black);
            
            using (var shadowBrush = new SolidBrush(shadowColor))
            {
                if (BORDER_RADIUS > 0)
                {
                    using (var shadowPath = CreateRoundedPath(shadowRect, BORDER_RADIUS))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
                else
                {
                    g.FillRectangle(shadowBrush, shadowRect);
                }
            }
        }

        private void DrawButton(Graphics g, Base.BaseControl owner)
        {
            // Determine button colors based on state and theme
            Color backgroundColor = GetButtonBackgroundColor(owner);
            Color borderColor = GetButtonBorderColor(owner);

            using (var backgroundBrush = new SolidBrush(backgroundColor))
            using (var borderPen = new Pen(borderColor, BORDER_WIDTH))
            {
                if (BORDER_RADIUS > 0)
                {
                    using (var buttonPath = CreateRoundedPath(_buttonRect, BORDER_RADIUS))
                    {
                        g.FillPath(backgroundBrush, buttonPath);
                        g.DrawPath(borderPen, buttonPath);
                    }
                }
                else
                {
                    g.FillRectangle(backgroundBrush, _buttonRect);
                    g.DrawRectangle(borderPen, _buttonRect);
                }
            }
        }

   

        private Color GetButtonBackgroundColor(Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            
            // Use owner's BackColor if explicitly set
            if (owner.BackColor != Color.Transparent && owner.BackColor != SystemColors.Control)
            {
                return ApplyStateModification(owner.BackColor, owner);
            }

            // Use theme colors based on state
            if (!owner.Enabled)
                return theme?.ButtonBackColor ?? Color.FromArgb(200, 200, 200);
            else if (owner.IsPressed)
                return theme?.ButtonPressedBackColor ?? Color.FromArgb(200, 60, 60);
            else if (owner.IsHovered)
                return theme?.ButtonHoverBackColor ?? Color.FromArgb(255, 100, 100);
            else
                return theme?.ButtonBackColor ?? Color.FromArgb(255, 87, 87);
        }

        private Color GetButtonBorderColor(Base.BaseControl owner)
        {
            var theme = owner._currentTheme;

            // Use owner's BorderColor if explicitly set
            if (owner.BorderColor != Color.Black && owner.BorderColor != Color.Empty)
            {
                return ApplyStateModification(owner.BorderColor, owner);
            }

            // Use theme colors based on state
            if (!owner.Enabled)
                return theme?.ButtonBorderColor ?? Color.Gray;
            else if (owner.IsPressed)
                return theme?.ButtonPressedBorderColor ?? Color.FromArgb(50, 50, 50);
            else if (owner.IsHovered)
                return theme?.ButtonHoverBorderColor ?? Color.Black;
            else if (owner.IsFocused)
                return theme?.FocusIndicatorColor ?? Color.FromArgb(50, 50, 50);
            else
                return theme?.ButtonBorderColor ?? Color.Black;
        }

        private Color ApplyStateModification(Color baseColor, Base.BaseControl owner)
        {
            if (!owner.Enabled)
                return Color.FromArgb(128, baseColor);
            else if (owner.IsPressed)
                return Color.FromArgb(220, baseColor.R, baseColor.G, baseColor.B);
            else if (owner.IsHovered)
                return Color.FromArgb(255, Math.Min(255, baseColor.R + 20), 
                    Math.Min(255, baseColor.G + 20), Math.Min(255, baseColor.B + 20));
            else
                return baseColor;
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;

            // Register entire button as clickable
            if (!_buttonRect.IsEmpty)
            {
                register("Button_Main", _buttonRect, () => 
                {
                    // Trigger the Click event
                    owner.TriggerClick();
                });
            }
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            // Minimum size to accommodate the button design
            int minWidth = 80;
            int minHeight = 32;

            // Add extra for label/helper only when not using external drawing
            int extraTop = 0, extraBottom = 0;
          

            return new Size(
                Math.Max(minWidth, proposedSize.Width),
                Math.Max(minHeight + extraTop + extraBottom, proposedSize.Height)
            );
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}