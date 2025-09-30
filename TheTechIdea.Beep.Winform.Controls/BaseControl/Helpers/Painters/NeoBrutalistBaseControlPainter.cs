using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Neo-Brutalist painter: provides bold, high-contrast styling with thick borders and stark shadows
    /// while leaving inner content (DrawingRect) for inheriting controls to handle.
    /// Uses current theme colors with brutalist modifications.
    /// </summary>
    internal sealed class NeoBrutalistBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _drawingRect;
        private Rectangle _borderRect;
        private Rectangle _contentRect;

        // Neo-Brutalist design constants
        private const int BORDER_WIDTH = 4;
        private const int SHADOW_OFFSET = 8;
        private const int CONTENT_PADDING = 16;

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

            // Calculate rects with brutalist spacing
            int totalPadding = BORDER_WIDTH + CONTENT_PADDING;
            int shadowSpace = SHADOW_OFFSET;

            // Drawing rect with content padding
            _drawingRect = new Rectangle(
                totalPadding,
                totalPadding,
                Math.Max(0, owner.Width - (totalPadding * 2) - shadowSpace),
                Math.Max(0, owner.Height - (totalPadding * 2) - shadowSpace)
            );

            // Border rect for the main border
            _borderRect = new Rectangle(
                BORDER_WIDTH / 2,
                BORDER_WIDTH / 2,
                Math.Max(0, owner.Width - BORDER_WIDTH - shadowSpace),
                Math.Max(0, owner.Height - BORDER_WIDTH - shadowSpace)
            );

            // Content rect is same as drawing rect for brutalist style
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

            // Disable anti-aliasing for sharp, brutal edges
            var oldSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            try
            {
                // 1. Draw stark shadow
                DrawBrutalistShadow(g, owner);

                // 2. Draw bold background
                DrawBrutalistBackground(g, owner);

                // 3. Draw thick border
                DrawBrutalistBorder(g, owner);

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
            }
        }

        private void DrawBrutalistShadow(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            
            Rectangle shadowRect = new Rectangle(
                _borderRect.X + SHADOW_OFFSET,
                _borderRect.Y + SHADOW_OFFSET,
                _borderRect.Width,
                _borderRect.Height
            );

            // Use theme shadow color or stark black
            Color shadowColor = theme?.ShadowColor ?? Color.Black;
            // Make shadow more opaque for brutalist style
            if (shadowColor.A < 200)
            {
                shadowColor = Color.FromArgb(200, shadowColor.R, shadowColor.G, shadowColor.B);
            }

            using var shadowBrush = new SolidBrush(shadowColor);
            g.FillRectangle(shadowBrush, shadowRect);
        }

        private void DrawBrutalistBackground(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            
            // Get background color from theme or owner
            Color backgroundColor = GetBrutalistBackgroundColor(owner, theme);
            
            using var backgroundBrush = new SolidBrush(backgroundColor);
            g.FillRectangle(backgroundBrush, _borderRect);
        }

        private void DrawBrutalistBorder(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            
            // Get border color from theme or use high contrast
            Color borderColor = GetBrutalistBorderColor(owner, theme);
            
            using var borderPen = new Pen(borderColor, BORDER_WIDTH);
            g.DrawRectangle(borderPen, _borderRect);

            // Add inner accent line for more brutal effect
            if (owner.IsFocused || owner.IsHovered)
            {
                Color accentColor = owner.IsFocused ? 
                    (theme?.FocusIndicatorColor ?? Color.Yellow) : 
                    (theme?.BorderColor ?? Color.White);
                    
                Rectangle innerRect = Rectangle.Inflate(_borderRect, -BORDER_WIDTH, -BORDER_WIDTH);
                using var accentPen = new Pen(accentColor, 2);
                g.DrawRectangle(accentPen, innerRect);
            }
        }

        private Color GetBrutalistBackgroundColor(Base.BaseControl owner, TheTechIdea.Beep.Vis.Modules.IBeepTheme theme)
        {
            // Neo-brutalist uses high contrast colors
            if (!owner.Enabled)
                return theme?.DisabledBackColor ?? Color.LightGray;
            if (owner.IsPressed)
                return theme?.ButtonPressedBackColor ?? Color.Red;
            if (owner.IsHovered)
                return theme?.ButtonHoverBackColor ?? Color.Yellow;
            if (owner.IsSelected)
                return theme?.ButtonSelectedBackColor ?? Color.Lime;
            
            // Use owner's BackColor or theme default, ensure high contrast
            Color baseColor = owner.BackColor != Color.Transparent && owner.BackColor != SystemColors.Control ? 
                owner.BackColor : (theme?.BackColor ?? Color.White);
                
            // Ensure the color is vibrant for brutalist style
            if (baseColor == Color.White || baseColor == Color.Empty || baseColor == Color.Transparent)
            {
                baseColor = theme?.ButtonBackColor ?? Color.Cyan;
            }
            
            return baseColor;
        }

        private Color GetBrutalistBorderColor(Base.BaseControl owner, TheTechIdea.Beep.Vis.Modules.IBeepTheme theme)
        {
            // Neo-brutalist always uses stark, high-contrast borders
            if (owner.HasError)
                return owner.ErrorColor;
            if (!owner.Enabled)
                return theme?.DisabledBorderColor ?? Color.Gray;
            if (owner.IsFocused)
                return theme?.FocusIndicatorColor ?? Color.Yellow;
            if (owner.IsPressed)
                return theme?.ButtonPressedBorderColor ?? Color.DarkRed;
            if (owner.IsHovered)
                return theme?.ButtonHoverBorderColor ?? Color.DarkBlue;
            
            // Default to black for maximum contrast
            return owner.BorderColor != Color.Empty ? owner.BorderColor : 
                   (theme?.BorderColor ?? Color.Black);
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
            if (!lead.IsEmpty && owner.LeadingIconClickable) register("NeoLeadingIcon", lead, owner.TriggerLeadingIconClick);
            if (!trail.IsEmpty && owner.TrailingIconClickable) register("NeoTrailingIcon", trail, owner.TriggerTrailingIconClick);
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            // Neo-brutalist needs extra space for thick borders and shadows
            int totalPadding = (BORDER_WIDTH * 2) + (CONTENT_PADDING * 2) + SHADOW_OFFSET;
            
            int minWidth = 100 + totalPadding;
            int minHeight = owner.Font.Height + totalPadding;

            return new Size(
                Math.Max(minWidth, proposedSize.Width),
                Math.Max(minHeight, proposedSize.Height)
            );
        }
    }
}
