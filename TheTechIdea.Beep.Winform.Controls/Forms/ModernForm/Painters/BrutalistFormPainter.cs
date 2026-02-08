using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Bold geometric high-contrast brutalist design
    /// </summary>
    internal sealed class BrutalistFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Brutalist, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Brutalist: Flat, solid fill - no gradients
            g.SmoothingMode = SmoothingMode.None; // Sharp pixels
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Optional: Grid lines for geometric brutalist aesthetic
            using (var gridPen = new Pen(Color.FromArgb(30, metrics.BorderColor), 1))
            {
                // Vertical grid lines every 40px
                for (int x = 40; x < owner.ClientRectangle.Width; x += 40)
                {
                    g.DrawLine(gridPen, x, 0, x, owner.ClientRectangle.Height);
                }
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            
            // Brutalist: Flat solid caption, no rounding
            g.SmoothingMode = SmoothingMode.None;
            using var capBrush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(capBrush, captionRect);
            
            // Bold horizontal divider line
            using (var dividerPen = new Pen(metrics.BorderColor, 3))
            {
                g.DrawLine(dividerPen, 0, captionRect.Bottom - 1, captionRect.Width, captionRect.Bottom - 1);
            }

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Paint all built-in caption elements (icon, minimize, maximize, close, theme, style buttons)
            // Paint custom Brutalist buttons (Bold, high contrast, hard edges)
            PaintBrutalistButtons(g, owner, captionRect, metrics);
        }

        /// <summary>
        /// Paint Brutalist buttons (Bold, high contrast, hard edges, no smoothing)
        /// </summary>
        private void PaintBrutalistButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int buttonSize = 20;
            int padding = (captionRect.Height - buttonSize) / 2;
            
            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;
            
            // Shared pen for borders
            using (var borderPen = new Pen(metrics.BorderColor, 2))
            {
                // Close button: Red block on hover, X icon
                PaintBrutalistButton(g, closeRect, Color.FromArgb(255, 60, 60), borderPen, "close", closeHovered);
                
                // Maximize button: Block on hover, Square icon
                PaintBrutalistButton(g, maxRect, metrics.BorderColor, borderPen, "maximize", maxHovered);
                
                // Minimize button: Block on hover, Line icon
                PaintBrutalistButton(g, minRect, metrics.BorderColor, borderPen, "minimize", minHovered);
                
                // Theme/Style buttons
                if (owner.ShowStyleButton)
                {
                    PaintBrutalistButton(g, owner.CurrentLayout.StyleButtonRect, metrics.BorderColor, borderPen, "Style", styleHovered);
                }
                
                if (owner.ShowThemeButton)
                {
                    PaintBrutalistButton(g, owner.CurrentLayout.ThemeButtonRect, metrics.BorderColor, borderPen, "theme", themeHovered);
                }
            }
        }
        
        private void PaintBrutalistButton(Graphics g, Rectangle rect, Color hoverColor, Pen borderPen, string type, bool isHovered)
        {
            // Brutalist: No smoothing
            g.SmoothingMode = SmoothingMode.None;
            
            // Draw background only on hover (Brutalist interaction style: interaction reveals form)
            if (isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    // Draw box with hard shadow offset
                    g.FillRectangle(brush, rect);
                    
                    // Offset border for "pressed" or "active" feel
                    g.DrawRectangle(borderPen, rect);
                    
                    // Hard shadow
                    using (var shadowBrush = new SolidBrush(Color.Black))
                    {
                        g.FillRectangle(shadowBrush, rect.Right, rect.Bottom, 4, 4);
                    }
                }
            }
            
            // Draw Icon
            // Inverse color on hover for contrast
            Color iconColor = isHovered ? Color.White : borderPen.Color;
            
            using (var iconPen = new Pen(iconColor, 2))
            {
                int cx = rect.X + rect.Width / 2;
                int cy = rect.Y + rect.Height / 2;
                int size = 10;
                
                switch (type)
                {
                    case "close":
                        g.DrawLine(iconPen, cx - size/2, cy - size/2, cx + size/2, cy + size/2);
                        g.DrawLine(iconPen, cx + size/2, cy - size/2, cx - size/2, cy + size/2);
                        break;
                    case "maximize":
                        g.DrawRectangle(iconPen, cx - size/2, cy - size/2, size, size);
                        g.FillRectangle(isHovered ? Brushes.White : Brushes.Black, cx - size/2, cy - size/2, size, 2); // Header bar
                        break;
                    case "minimize":
                        g.DrawLine(iconPen, cx - size/2, cy + size/2, cx + size/2, cy + size/2);
                        break;
                    case "Style": // Brush/Palette
                         g.FillRectangle(isHovered ? Brushes.White : new SolidBrush(iconColor), cx - size/2, cy - size/2, size, size);
                        break;
                    case "theme": // Layers
                         g.DrawRectangle(iconPen, cx - size/2, cy - size/2, size, size);
                         g.DrawLine(iconPen, cx - size/2 + 2, cy - size/2 + size/2, cx + size/2 - 2, cy - size/2 + size/2);
                        break;
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using var path = owner.BorderShape;
            // Brutalist: Thick, sharp borders - no anti-aliasing
            g.SmoothingMode = SmoothingMode.None;
            using var pen = new Pen(metrics.BorderColor, 5); // Thick 5px border
            
            // Draw outer border using path
            g.DrawPath(pen, path);
            
            // Additional accent lines for brutalist geometric Style
            using (var accentPen = new Pen(metrics.BorderColor, 2))
            {
                // Inner rectangle using path
                var innerRect = new Rectangle(8, 8, owner.ClientRectangle.Width - 17, owner.ClientRectangle.Height - 17);
                using var innerPath = CreateRoundedRectanglePath(innerRect, new CornerRadius(0));
                g.DrawPath(accentPen, innerPath);
            }
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Brutalist: Hard shadow with NO blur
            return new ShadowEffect
            {
                Color = Color.FromArgb(60, 0, 0, 0), // Stronger but hard-edged
                Blur = 0, // No blur for sharp edges
                OffsetX = 6,
                OffsetY = 6,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(0);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.None;
        }

        public bool SupportsAnimations => false;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            var originalClip = g.Clip;
            var shadow = GetShadowEffect(owner);
            var radius = GetCornerRadius(owner);

            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow, radius);
            }

            PaintBackground(g, owner);

            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            g.Clip = new Region(path);
            g.Clip = originalClip;

            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }

            g.Clip = originalClip;
        }

        private void DrawShadow(Graphics g, Rectangle rect, ShadowEffect shadow, CornerRadius radius)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            var shadowRect = new Rectangle(
                rect.X + shadow.OffsetX - shadow.Blur,
                rect.Y + shadow.OffsetY - shadow.Blur,
                rect.Width + shadow.Blur * 2,
                rect.Height + shadow.Blur * 2);
            if (shadowRect.Width <= 0 || shadowRect.Height <= 0) return;
            using var brush = new SolidBrush(shadow.Color);
            using var path = CreateRoundedRectanglePath(shadowRect, radius);
            g.FillPath(brush, path);
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(new Rectangle(rect.X, rect.Y, Math.Max(1, rect.Width), Math.Max(1, rect.Height)));
                return path;
            }
            int maxRadius = Math.Min(rect.Width, rect.Height) / 2;
            int tl = Math.Max(0, Math.Min(radius.TopLeft, maxRadius));
            int tr = Math.Max(0, Math.Min(radius.TopRight, maxRadius));
            int br = Math.Max(0, Math.Min(radius.BottomRight, maxRadius));
            int bl = Math.Max(0, Math.Min(radius.BottomLeft, maxRadius));
            if (tl == 0 && tr == 0 && br == 0 && bl == 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            if (tl > 0) path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90); else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            if (tr > 0) path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90); else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            if (br > 0) path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90); else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            if (bl > 0) path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90); else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);
            path.CloseFigure();
            return path;
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            var metrics = GetMetrics(owner);
            
            // NOTE: _hits.Clear() is handled by EnsureLayoutCalculated - do not call here
            
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));
            
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
            
            int buttonWidth = metrics.ButtonWidth;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("Style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Custom action button (only if ShowCustomActionButton is true)
            if (owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Search box (between title and buttons)
            int searchBoxWidth = 200;
            int searchBoxPadding = 8;
            if (owner.ShowSearchBox)
            {
                layout.SearchBoxRect = new Rectangle(buttonX - searchBoxWidth - searchBoxPadding, searchBoxPadding / 2, 
                    searchBoxWidth, captionHeight - searchBoxPadding);
                owner._hits.RegisterHitArea("search", layout.SearchBoxRect, HitAreaType.TextBox);
                buttonX -= searchBoxWidth + searchBoxPadding;
            }
            else
            {
                layout.SearchBoxRect = Rectangle.Empty;
            }
            
            int iconX = metrics.IconLeftPadding;
            int iconY = (captionHeight - metrics.IconSize) / 2;
            layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
            }
            
            int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
            int titleWidth = buttonX - titleX - metrics.ButtonSpacing;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            
            owner.CurrentLayout = layout;
        }

        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var outer = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outer, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
    }
}
