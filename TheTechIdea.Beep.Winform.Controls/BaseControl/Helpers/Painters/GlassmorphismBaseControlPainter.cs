using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Glassmorphism painter: provides modern glass-like styling with blur effects and transparency
    /// while leaving inner content (DrawingRect) for inheriting controls to handle.
    /// Uses current theme colors with glassmorphism modifications.
    /// </summary>
    internal sealed class GlassmorphismBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _drawingRect;
        private Rectangle _borderRect;
        private Rectangle _contentRect;

        // Glassmorphism design constants
        private const int BORDER_RADIUS = 16;
        private const int BLUR_RADIUS = 20;
        private const int CONTENT_PADDING = 20;
        private const float GLASS_OPACITY = 0.15f;
        private const float BORDER_OPACITY = 0.3f;

        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _borderRect;
        public Rectangle ContentRect => _contentRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null)
            {
                _drawingRect = _borderRect = _contentRect = Rectangle.Empty;
                return;
            }

            // Calculate rects with glassmorphism spacing
            _borderRect = new Rectangle(
                2,
                2,
                Math.Max(0, owner.Width - 4),
                Math.Max(0, owner.Height - 4)
            );

            _drawingRect = new Rectangle(
                CONTENT_PADDING,
                CONTENT_PADDING,
                Math.Max(0, owner.Width - (CONTENT_PADDING * 2)),
                Math.Max(0, owner.Height - (CONTENT_PADDING * 2))
            );

            _contentRect = _drawingRect;

            // Adjust for icons if present
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (hasLeading || hasTrailing)
            {
                var icons = new BaseControlIconsHelper(owner);
                icons.UpdateLayout(_drawingRect);
                _contentRect = icons.AdjustedContentRect;
            }
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;

            var theme = owner._currentTheme;

            // Enable high-quality rendering for smooth glass effects
            var oldSmoothingMode = g.SmoothingMode;
            var oldInterpolationMode = g.InterpolationMode;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            try
            {
                // 1. Draw background blur effect (simulated)
                DrawGlassBackground(g, owner);

                // 2. Draw glass border
                DrawGlassBorder(g, owner);

                // 3. Draw subtle inner glow
                if (owner.IsFocused || owner.IsHovered)
                {
                    DrawInnerGlow(g, owner);
                }

                // 4. Draw icons if any
                bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
                bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
                if (hasLeading || hasTrailing)
                {
                    var icons = new BaseControlIconsHelper(owner);
                    icons.UpdateLayout(_drawingRect);
                    icons.Draw(g);
                }
            }
            finally
            {
                // Restore graphics state
                g.SmoothingMode = oldSmoothingMode;
                g.InterpolationMode = oldInterpolationMode;
            }
        }

        private void DrawGlassBackground(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            
            // Create glass-like background color
            Color baseColor = GetGlassBackgroundColor(owner, theme);
            Color glassColor = Color.FromArgb((int)(255 * GLASS_OPACITY), baseColor.R, baseColor.G, baseColor.B);
            
            using var glassBrush = new SolidBrush(glassColor);
            using var glassPath = CreateRoundedPath(_borderRect, BORDER_RADIUS);
            g.FillPath(glassBrush, glassPath);

            // Add subtle gradient for depth
            using var gradientBrush = new LinearGradientBrush(
                _borderRect, 
                Color.FromArgb(40, 255, 255, 255), 
                Color.FromArgb(10, 255, 255, 255), 
                LinearGradientMode.Vertical);
            g.FillPath(gradientBrush, glassPath);
        }

        private void DrawGlassBorder(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            
            // Create glass-like border
            Color borderColor = GetGlassBorderColor(owner, theme);
            Color glassBorderColor = Color.FromArgb((int)(255 * BORDER_OPACITY), borderColor.R, borderColor.G, borderColor.B);
            
            using var borderPen = new Pen(glassBorderColor, 1);
            using var borderPath = CreateRoundedPath(_borderRect, BORDER_RADIUS);
            g.DrawPath(borderPen, borderPath);

            // Add highlight on top edge for glass effect
            Color highlightColor = Color.FromArgb(60, 255, 255, 255);
            using var highlightPen = new Pen(highlightColor, 1);
            
            // Draw top curve highlight
            Rectangle highlightRect = new Rectangle(_borderRect.X + 1, _borderRect.Y + 1, _borderRect.Width - 2, BORDER_RADIUS);
            using var highlightPath = CreateRoundedPath(highlightRect, BORDER_RADIUS);
            g.DrawPath(highlightPen, highlightPath);
        }

        private void DrawInnerGlow(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            
            Color glowColor = owner.IsFocused ? 
                (theme?.FocusIndicatorColor ?? Color.CornflowerBlue) :
                (theme?.BorderColor ?? Color.White);
                
            // Create inner glow effect
            Rectangle glowRect = Rectangle.Inflate(_borderRect, -4, -4);
            Color innerGlowColor = Color.FromArgb(30, glowColor.R, glowColor.G, glowColor.B);
            
            using var glowBrush = new SolidBrush(innerGlowColor);
            using var glowPath = CreateRoundedPath(glowRect, BORDER_RADIUS - 4);
            g.FillPath(glowBrush, glowPath);
        }

        private Color GetGlassBackgroundColor(Base.BaseControl owner, TheTechIdea.Beep.Vis.Modules.IBeepTheme theme)
        {
            if (!owner.Enabled)
                return theme?.DisabledBackColor ?? Color.Gray;
            if (owner.IsPressed)
                return theme?.ButtonPressedBackColor ?? Color.RoyalBlue;
            if (owner.IsSelected)
                return theme?.ButtonSelectedBackColor ?? Color.DodgerBlue;
            
            // Use owner's BackColor or theme default
            Color baseColor = owner.BackColor != Color.Transparent && owner.BackColor != SystemColors.Control ? 
                owner.BackColor : (theme?.BackColor ?? Color.White);
                
            // Ensure some color for glass effect
            if (baseColor == Color.Transparent || baseColor.A < 50)
            {
                baseColor = theme?.ButtonBackColor ?? Color.CornflowerBlue;
            }
            
            return baseColor;
        }

        private Color GetGlassBorderColor(Base.BaseControl owner, TheTechIdea.Beep.Vis.Modules.IBeepTheme theme)
        {
            if (owner.HasError)
                return owner.ErrorColor;
            if (!owner.Enabled)
                return theme?.DisabledBorderColor ?? Color.Gray;
            if (owner.IsFocused)
                return theme?.FocusIndicatorColor ?? Color.CornflowerBlue;
            
            return owner.BorderColor != Color.Empty ? owner.BorderColor : 
                   (theme?.BorderColor ?? Color.White);
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;
            
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (!(hasLeading || hasTrailing)) return;

            var icons = new BaseControlIconsHelper(owner);
            icons.UpdateLayout(_drawingRect);
            var lead = icons.LeadingRect;
            var trail = icons.TrailingRect;
            if (!lead.IsEmpty && owner.LeadingIconClickable) register("GlassLeadingIcon", lead, owner.TriggerLeadingIconClick);
            if (!trail.IsEmpty && owner.TrailingIconClickable) register("GlassTrailingIcon", trail, owner.TriggerTrailingIconClick);
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            int totalPadding = CONTENT_PADDING * 2;
            int minWidth = 120 + totalPadding;
            int minHeight = owner.Font.Height + totalPadding;

            return new Size(
                Math.Max(minWidth, proposedSize.Width),
                Math.Max(minHeight, proposedSize.Height)
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