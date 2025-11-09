using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Ubuntu Unity desktop style (synced with UbuntuTheme).
    /// 
    /// Ubuntu Color Palette (synced with UbuntuTheme):
    /// - Background: #2C2C2C (44, 44, 44) - Dark gray base
    /// - Foreground: #FFFFFF (255, 255, 255) - White text
    /// - Border: #3C3C3C (60, 60, 60) - Medium gray border
    /// - Hover: #3C3C3C (60, 60, 60) - Medium gray hover
    /// - Selected: #E95420 (233, 84, 32) - Ubuntu orange selected
    /// 
    /// Features:
    /// - Warm gradient (6% lighter at top)
    /// - Unity launcher-inspired vertical accent line (left edge)
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class UbuntuFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Ubuntu, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Ubuntu warm gradient (subtle 6% lighter at top)
            using (var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                ControlPaint.Light(metrics.BackgroundColor, 0.06f),
                metrics.BackgroundColor,
                90f))
            {
                g.FillRectangle(gradBrush, owner.ClientRectangle);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Ubuntu Unity launcher-inspired vertical accent line (4px wide, left edge)
            var accentColor = Color.FromArgb(180, 233, 84, 32); // Ubuntu orange with transparency
            using (var accentBrush = new SolidBrush(accentColor))
            {
                g.FillRectangle(accentBrush, new Rectangle(0, 0, 4, owner.ClientRectangle.Height));
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Caption with subtle gradient
            using (var capBrush = new LinearGradientBrush(
                captionRect,
                ControlPaint.Light(metrics.CaptionColor, 0.04f),
                metrics.CaptionColor,
                90f))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Ubuntu orange accent line at top of caption
            using (var accentPen = new Pen(Color.FromArgb(120, 233, 84, 32), 2))
            {
                g.DrawLine(accentPen, captionRect.Left, captionRect.Top, captionRect.Right, captionRect.Top);
            }

            // Paint Ubuntu pill-shaped buttons (UNIQUE)
            PaintUbuntuButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Ubuntu pill buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Ubuntu Unity-Style circular buttons (REDESIGNED - cleaner look)
        /// </summary>
        private void PaintUbuntuButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Ubuntu clean circles: 14px diameter
            int buttonSize = 14;
            int buttonY = (captionRect.Height - buttonSize) / 2;
            
            // Close button: Ubuntu orange circle with X
            int closeX = closeRect.X + (closeRect.Width - buttonSize) / 2;
            var closeCircle = new Rectangle(closeX, buttonY, buttonSize, buttonSize);
            
            // Gradient for depth
            using (var orangeGrad = new LinearGradientBrush(
                closeCircle,
                Color.FromArgb(245, 94, 42),
                Color.FromArgb(223, 74, 22),
                LinearGradientMode.Vertical))
            {
                g.FillEllipse(orangeGrad, closeCircle);
            }
            
            // Subtle border
            using (var borderPen = new Pen(Color.FromArgb(200, 60, 20), 1f))
            {
                g.DrawEllipse(borderPen, closeCircle);
            }
            
            // White X symbol
            using (var xPen = new Pen(Color.White, 2f))
            {
                int xSize = 6;
                int cx = closeCircle.X + closeCircle.Width / 2;
                int cy = closeCircle.Y + closeCircle.Height / 2;
                g.DrawLine(xPen, cx - xSize/2, cy - xSize/2, cx + xSize/2, cy + xSize/2);
                g.DrawLine(xPen, cx + xSize/2, cy - xSize/2, cx - xSize/2, cy + xSize/2);
            }
            
            // Maximize button: Ubuntu orange circle with square
            int maxX = maxRect.X + (maxRect.Width - buttonSize) / 2;
            var maxCircle = new Rectangle(maxX, buttonY, buttonSize, buttonSize);
            
            using (var orangeGrad = new LinearGradientBrush(
                maxCircle,
                Color.FromArgb(245, 94, 42),
                Color.FromArgb(223, 74, 22),
                LinearGradientMode.Vertical))
            {
                g.FillEllipse(orangeGrad, maxCircle);
            }
            
            using (var borderPen = new Pen(Color.FromArgb(200, 60, 20), 1f))
            {
                g.DrawEllipse(borderPen, maxCircle);
            }
            
            // White square symbol
            using (var squarePen = new Pen(Color.White, 1.5f))
            {
                int sSize = 6;
                int mx = maxCircle.X + maxCircle.Width / 2;
                int my = maxCircle.Y + maxCircle.Height / 2;
                g.DrawRectangle(squarePen, mx - sSize/2, my - sSize/2, sSize, sSize);
            }
            
            // Minimize button: Ubuntu orange circle with line
            int minX = minRect.X + (minRect.Width - buttonSize) / 2;
            var minCircle = new Rectangle(minX, buttonY, buttonSize, buttonSize);
            
            using (var orangeGrad = new LinearGradientBrush(
                minCircle,
                Color.FromArgb(245, 94, 42),
                Color.FromArgb(223, 74, 22),
                LinearGradientMode.Vertical))
            {
                g.FillEllipse(orangeGrad, minCircle);
            }
            
            using (var borderPen = new Pen(Color.FromArgb(200, 60, 20), 1f))
            {
                g.DrawEllipse(borderPen, minCircle);
            }
            
            // White line symbol
            using (var linePen = new Pen(Color.White, 1.5f))
            {
                int lSize = 6;
                int mnx = minCircle.X + minCircle.Width / 2;
                int mny = minCircle.Y + minCircle.Height / 2;
                g.DrawLine(linePen, mnx - lSize/2, mny, mnx + lSize/2, mny);
            }

            // Theme button (if shown) - Ubuntu purple circle
            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                int themeX = themeRect.X + (themeRect.Width - buttonSize) / 2;
                var themeCircle = new Rectangle(themeX, buttonY, buttonSize, buttonSize);
                
                using (var purpleGrad = new LinearGradientBrush(
                    themeCircle,
                    Color.FromArgb(139, 61, 103),
                    Color.FromArgb(99, 21, 63),
                    LinearGradientMode.Vertical))
                {
                    g.FillEllipse(purpleGrad, themeCircle);
                }
                
                using (var borderPen = new Pen(Color.FromArgb(79, 11, 53), 1f))
                {
                    g.DrawEllipse(borderPen, themeCircle);
                }
                
                // White palette icon
                int tx = themeCircle.X + themeCircle.Width / 2;
                int ty = themeCircle.Y + themeCircle.Height / 2;
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    g.DrawEllipse(iconPen, tx - 3, ty - 3, 6, 6);
                }
                g.FillEllipse(Brushes.White, tx - 1, ty + 1, 2, 2);
            }

            // Style button (if shown) - Ubuntu purple circle
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                int styleX = styleRect.X + (styleRect.Width - buttonSize) / 2;
                var styleCircle = new Rectangle(styleX, buttonY, buttonSize, buttonSize);
                
                using (var purpleGrad = new LinearGradientBrush(
                    styleCircle,
                    Color.FromArgb(139, 61, 103),
                    Color.FromArgb(99, 21, 63),
                    LinearGradientMode.Vertical))
                {
                    g.FillEllipse(purpleGrad, styleCircle);
                }
                
                using (var borderPen = new Pen(Color.FromArgb(79, 11, 53), 1f))
                {
                    g.DrawEllipse(borderPen, styleCircle);
                }
                
                // White brush icon
                int sx = styleCircle.X + styleCircle.Width / 2;
                int sy = styleCircle.Y + styleCircle.Height / 2;
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    g.DrawLine(iconPen, sx - 2, sy - 2, sx - 2, sy + 2);
                    g.DrawLine(iconPen, sx - 2, sy, sx + 2, sy + 2);
                }
            }
        }
        
        /// <summary>
        /// Create pill-shaped path (rounded rectangle)
        /// </summary>
        private GraphicsPath CreatePillPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
          using var path = owner.BorderShape; 
            // Thin border with subtle Ubuntu orange tint
            var borderColor = Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 50), 
                Math.Min(255, metrics.BorderColor.R + 20),
                metrics.BorderColor.G,
                Math.Min(255, metrics.BorderColor.B - 10));
            using var pen = new Pen(borderColor, 1);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Warm shadow with subtle orange tint (Ubuntu characteristic)
            return new ShadowEffect
            {
                Color = Color.FromArgb(22, 30, 15, 10), // Warm shadow (less blue, more red)
                Blur = 12,
                OffsetY = 5,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(7); // Ubuntu's characteristic corner radius
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra; // Smooth Ubuntu aesthetic
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
            
            owner._hits.Clear();
            
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
                owner._hits.Register("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.Register("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.Register("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
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
            
            // Custom action button (fallback)
            // Custom action button (only if ShowCustomActionButton is true)
            if (owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
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
