using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Tokyo Night theme with neon accents, night city glow, and cross-shaped buttons
    /// </summary>
    internal sealed class TokyoFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Tokyo, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Tokyo Night: deep background with subtle gradient
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Night city glow effect (vertical gradient from top)
            using (var glowBrush = new LinearGradientBrush(
                new Rectangle(0, 0, owner.ClientRectangle.Width, 100),
                Color.FromArgb(30, 125, 207, 255), // Cyan glow
                Color.FromArgb(0, 125, 207, 255),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(glowBrush, 0, 0, owner.ClientRectangle.Width, 100);
            }
            
            // Neon accent line at top (vibrant cyan)
            using (var neonPen = new Pen(Color.FromArgb(120, 125, 207, 255), 2))
            {
                g.DrawLine(neonPen, 0, 0, owner.ClientRectangle.Width, 0);
            }
            
            // Add subtle scan line effect (cyberpunk feel)
            using (var scanPen = new Pen(Color.FromArgb(3, 255, 255, 255), 1))
            {
                for (int y = 0; y < owner.ClientRectangle.Height; y += 4)
                {
                    g.DrawLine(scanPen, 0, y, owner.ClientRectangle.Width, y);
                }
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Tokyo Night: vibrant caption with neon glow
            using (var capBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Neon accent line at bottom of caption (cyan glow)
            using (var neonPen = new Pen(Color.FromArgb(100, 125, 207, 255), 2))
            {
                g.DrawLine(neonPen, 0, captionRect.Bottom - 1, captionRect.Width, captionRect.Bottom - 1);
            }

            // Paint Tokyo Night cross/plus shaped buttons (UNIQUE SKIN)
            PaintTokyoCrossButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Tokyo cross buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Tokyo Night cross/plus shaped buttons (UNIQUE SKIN)
        /// Two intersecting rectangles forming a + shape with neon glow
        /// </summary>
        private void PaintTokyoCrossButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            var themeRect = owner.CurrentLayout.ThemeButtonRect;
            var styleRect = owner.CurrentLayout.StyleButtonRect;
            
            int crossSize = 18; // Cross/plus size
            int crossY = (captionRect.Height - crossSize) / 2;
            
            // Close button: Red cross with neon glow
            int cx = closeRect.X + closeRect.Width / 2;
            int cy = crossY + crossSize / 2;
            
            // Draw glow first (outer to inner for layering)
            for (int i = 3; i > 0; i--)
            {
                using (var glowPen = new Pen(Color.FromArgb(20 * i, 255, 100, 120), 2 * i))
                {
                    // Horizontal bar of cross
                    g.DrawLine(glowPen, cx - crossSize/2, cy, cx + crossSize/2, cy);
                    // Vertical bar of cross
                    g.DrawLine(glowPen, cx, cy - crossSize/2, cx, cy + crossSize/2);
                }
            }
            
            // Draw solid cross (+ shape)
            using (var crossPen = new Pen(Color.FromArgb(255, 180, 100), 3))
            {
                // Horizontal bar
                g.DrawLine(crossPen, cx - crossSize/2, cy, cx + crossSize/2, cy);
                // Vertical bar
                g.DrawLine(crossPen, cx, cy - crossSize/2, cx, cy + crossSize/2);
            }
            
            // X icon on cross center
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 6;
                g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
            }
            
            // Maximize button: Cyan cross with glow
            int mx = maxRect.X + maxRect.Width / 2;
            int my = crossY + crossSize / 2;
            
            // Draw cyan glow
            for (int i = 3; i > 0; i--)
            {
                using (var glowPen = new Pen(Color.FromArgb(20 * i, 125, 207, 255), 2 * i))
                {
                    g.DrawLine(glowPen, mx - crossSize/2, my, mx + crossSize/2, my);
                    g.DrawLine(glowPen, mx, my - crossSize/2, mx, my + crossSize/2);
                }
            }
            
            // Draw solid cross
            using (var crossPen = new Pen(Color.FromArgb(150, 200, 255), 3))
            {
                g.DrawLine(crossPen, mx - crossSize/2, my, mx + crossSize/2, my);
                g.DrawLine(crossPen, mx, my - crossSize/2, mx, my + crossSize/2);
            }
            
            // Square icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int sqSize = 6;
                g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Purple cross with glow
            int mnx = minRect.X + minRect.Width / 2;
            int mny = crossY + crossSize / 2;
            
            // Draw purple glow
            for (int i = 3; i > 0; i--)
            {
                using (var glowPen = new Pen(Color.FromArgb(20 * i, 187, 154, 247), 2 * i))
                {
                    g.DrawLine(glowPen, mnx - crossSize/2, mny, mnx + crossSize/2, mny);
                    g.DrawLine(glowPen, mnx, mny - crossSize/2, mnx, mny + crossSize/2);
                }
            }
            
            // Draw solid cross
            using (var crossPen = new Pen(Color.FromArgb(200, 180, 255), 3))
            {
                g.DrawLine(crossPen, mnx - crossSize/2, mny, mnx + crossSize/2, mny);
                g.DrawLine(crossPen, mnx, mny - crossSize/2, mnx, mny + crossSize/2);
            }
            
            // Line icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int lineSize = 7;
                g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
            }
            
            // Theme button: Green cross with glow and palette icon (Tokyo Night green #9ece6a)
            if (!themeRect.IsEmpty)
            {
                int tx = themeRect.X + themeRect.Width / 2;
                int ty = crossY + crossSize / 2;
                
                // Draw green glow
                for (int i = 3; i > 0; i--)
                {
                    using (var glowPen = new Pen(Color.FromArgb(20 * i, 158, 206, 106), 2 * i))
                    {
                        g.DrawLine(glowPen, tx - crossSize/2, ty, tx + crossSize/2, ty);
                        g.DrawLine(glowPen, tx, ty - crossSize/2, tx, ty + crossSize/2);
                    }
                }
                
                // Draw solid cross
                using (var crossPen = new Pen(Color.FromArgb(158, 206, 106), 3))
                {
                    g.DrawLine(crossPen, tx - crossSize/2, ty, tx + crossSize/2, ty);
                    g.DrawLine(crossPen, tx, ty - crossSize/2, tx, ty + crossSize/2);
                }
                
                // Palette icon
                using (var iconBrush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(iconBrush, tx - 3, ty - 2, 3, 3);
                    g.FillEllipse(iconBrush, tx + 1, ty - 2, 3, 3);
                    g.FillEllipse(iconBrush, tx - 1, ty + 2, 3, 3);
                }
            }
            
            // Style button: Orange cross with glow and brush icon (Tokyo Night orange #ff9e64)
            if (!styleRect.IsEmpty)
            {
                int sx = styleRect.X + styleRect.Width / 2;
                int sy = crossY + crossSize / 2;
                
                // Draw orange glow
                for (int i = 3; i > 0; i--)
                {
                    using (var glowPen = new Pen(Color.FromArgb(20 * i, 255, 158, 100), 2 * i))
                    {
                        g.DrawLine(glowPen, sx - crossSize/2, sy, sx + crossSize/2, sy);
                        g.DrawLine(glowPen, sx, sy - crossSize/2, sx, sy + crossSize/2);
                    }
                }
                
                // Draw solid cross
                using (var crossPen = new Pen(Color.FromArgb(255, 158, 100), 3))
                {
                    g.DrawLine(crossPen, sx - crossSize/2, sy, sx + crossSize/2, sy);
                    g.DrawLine(crossPen, sx, sy - crossSize/2, sx, sy + crossSize/2);
                }
                
                // Brush icon
                using (var iconPen = new Pen(Color.White, 1.2f))
                {
                    g.DrawLine(iconPen, sx - 2, sy - 2, sx - 2, sy + 2);
                    g.DrawLine(iconPen, sx, sy - 2, sx, sy + 2);
                    g.DrawLine(iconPen, sx + 2, sy - 2, sx + 2, sy + 2);
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = owner.BorderShape;
            // Tokyo Night: vibrant neon border with cyan glow
            using var pen = new Pen(Color.FromArgb(100, 125, 207, 255), 2);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Tokyo Night: colored shadow with cyan tint (night city glow)
            return new ShadowEffect
            {
                Color = Color.FromArgb(50, 30, 60, 90), // Cyan-tinted shadow
                Blur = 14, // Wide blur for neon glow effect
                OffsetY = 7, // Larger offset for dramatic depth
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(8); // Tokyo Night smooth modern corners
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High; // Crisp but smooth
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
            if (!owner.ShowThemeButton && !owner.ShowStyleButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
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
