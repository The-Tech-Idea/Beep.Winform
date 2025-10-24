using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Neon Style with vibrant multi-color glow and star-shaped buttons
    /// </summary>
    internal sealed class NeonFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Neon, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // Neon: deep dark background
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Multi-color neon glow effect (RGB gradient layers)
            // Pink glow from top
            using (var pinkGlow = new LinearGradientBrush(
                new Rectangle(0, 0, owner.ClientRectangle.Width, 80),
                Color.FromArgb(40, 255, 0, 150),
                Color.FromArgb(0, 255, 0, 150),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(pinkGlow, 0, 0, owner.ClientRectangle.Width, 80);
            }
            
            // Cyan glow from left
            using (var cyanGlow = new LinearGradientBrush(
                new Rectangle(0, 0, 100, owner.ClientRectangle.Height),
                Color.FromArgb(30, 0, 255, 255),
                Color.FromArgb(0, 0, 255, 255),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(cyanGlow, 0, 0, 100, owner.ClientRectangle.Height);
            }
            
            // Neon outline lines (bright vibrant)
            using (var neonPinkPen = new Pen(Color.FromArgb(150, 255, 0, 200), 2))
            {
                g.DrawLine(neonPinkPen, 0, 0, owner.ClientRectangle.Width, 0);
            }
            using (var neonCyanPen = new Pen(Color.FromArgb(150, 0, 255, 255), 2))
            {
                g.DrawLine(neonCyanPen, 0, 0, 0, owner.ClientRectangle.Height);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Neon: vibrant caption with multi-color glow
            using (var capBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Neon glow line at bottom (RGB cycling effect)
            using (var neonBrush = new LinearGradientBrush(captionRect,
                Color.FromArgb(180, 255, 0, 150),  // Pink
                Color.FromArgb(180, 0, 255, 255),  // Cyan
                LinearGradientMode.Horizontal))
            {
                using (var neonPen = new Pen(neonBrush, 3))
                {
                    g.DrawLine(neonPen, 0, captionRect.Bottom - 2, captionRect.Width, captionRect.Bottom - 2);
                }
            }

            // Paint Neon star-shaped buttons (UNIQUE SKIN)
            PaintNeonStarButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Neon star buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Neon star-shaped buttons (UNIQUE SKIN)
        /// 5-pointed stars with intense multi-color neon glow
        /// </summary>
        private void PaintNeonStarButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            var themeRect = owner.CurrentLayout.ThemeButtonRect;
            var styleRect = owner.CurrentLayout.StyleButtonRect;
            
            int starSize = 16;
            int starY = (captionRect.Height - starSize) / 2;
            
            // Close button: Pink neon star with intense glow
            int cx = closeRect.X + closeRect.Width / 2;
            int cy = starY + starSize / 2;
            
            // Draw glow layers (multiple passes for intense neon effect)
            for (int i = 5; i > 0; i--)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(15 * i, 255, 0, 150)))
                using (var starPath = CreateStarPath(cx, cy, starSize + i * 2))
                {
                    g.FillPath(glowBrush, starPath);
                }
            }
            
            // Solid star
            using (var starBrush = new SolidBrush(Color.FromArgb(255, 255, 50, 180)))
            using (var starPath = CreateStarPath(cx, cy, starSize))
            {
                g.FillPath(starBrush, starPath);
            }
            
            // Bright outline
            using (var outlinePen = new Pen(Color.FromArgb(255, 255, 200, 255), 1.5f))
            using (var starPath = CreateStarPath(cx, cy, starSize))
            {
                g.DrawPath(outlinePen, starPath);
            }
            
            // X icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 5;
                g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
            }
            
            // Maximize button: Cyan neon star with glow
            int mx = maxRect.X + maxRect.Width / 2;
            int my = starY + starSize / 2;
            
            // Cyan glow layers
            for (int i = 5; i > 0; i--)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(15 * i, 0, 255, 255)))
                using (var starPath = CreateStarPath(mx, my, starSize + i * 2))
                {
                    g.FillPath(glowBrush, starPath);
                }
            }
            
            // Solid star
            using (var starBrush = new SolidBrush(Color.FromArgb(255, 50, 255, 255)))
            using (var starPath = CreateStarPath(mx, my, starSize))
            {
                g.FillPath(starBrush, starPath);
            }
            
            // Outline
            using (var outlinePen = new Pen(Color.FromArgb(255, 200, 255, 255), 1.5f))
            using (var starPath = CreateStarPath(mx, my, starSize))
            {
                g.DrawPath(outlinePen, starPath);
            }
            
            // Square icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int sqSize = 5;
                g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Green neon star with glow
            int mnx = minRect.X + minRect.Width / 2;
            int mny = starY + starSize / 2;
            
            // Green glow layers
            for (int i = 5; i > 0; i--)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(15 * i, 0, 255, 100)))
                using (var starPath = CreateStarPath(mnx, mny, starSize + i * 2))
                {
                    g.FillPath(glowBrush, starPath);
                }
            }
            
            // Solid star
            using (var starBrush = new SolidBrush(Color.FromArgb(255, 100, 255, 100)))
            using (var starPath = CreateStarPath(mnx, mny, starSize))
            {
                g.FillPath(starBrush, starPath);
            }
            
            // Outline
            using (var outlinePen = new Pen(Color.FromArgb(255, 200, 255, 200), 1.5f))
            using (var starPath = CreateStarPath(mnx, mny, starSize))
            {
                g.DrawPath(outlinePen, starPath);
            }
            
            // Line icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int lineSize = 6;
                g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
            }
            
            // Theme button: Purple neon star with glow and palette icon
            if (!themeRect.IsEmpty)
            {
                int tx = themeRect.X + themeRect.Width / 2;
                int ty = starY + starSize / 2;
                
                // Purple glow layers
                for (int i = 5; i > 0; i--)
                {
                    using (var glowBrush = new SolidBrush(Color.FromArgb(15 * i, 150, 0, 255)))
                    using (var starPath = CreateStarPath(tx, ty, starSize + i * 2))
                    {
                        g.FillPath(glowBrush, starPath);
                    }
                }
                
                // Solid star
                using (var starBrush = new SolidBrush(Color.FromArgb(255, 180, 100, 255)))
                using (var starPath = CreateStarPath(tx, ty, starSize))
                {
                    g.FillPath(starBrush, starPath);
                }
                
                // Outline
                using (var outlinePen = new Pen(Color.FromArgb(255, 220, 180, 255), 1.5f))
                using (var starPath = CreateStarPath(tx, ty, starSize))
                {
                    g.DrawPath(outlinePen, starPath);
                }
                
                // Palette icon
                using (var iconBrush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(iconBrush, tx - 3, ty - 2, 3, 3);
                    g.FillEllipse(iconBrush, tx + 1, ty - 2, 3, 3);
                    g.FillEllipse(iconBrush, tx - 1, ty + 2, 3, 3);
                }
            }
            
            // Style button: Yellow neon star with glow and brush icon
            if (!styleRect.IsEmpty)
            {
                int sx = styleRect.X + styleRect.Width / 2;
                int sy = starY + starSize / 2;
                
                // Yellow glow layers
                for (int i = 5; i > 0; i--)
                {
                    using (var glowBrush = new SolidBrush(Color.FromArgb(15 * i, 255, 255, 0)))
                    using (var starPath = CreateStarPath(sx, sy, starSize + i * 2))
                    {
                        g.FillPath(glowBrush, starPath);
                    }
                }
                
                // Solid star
                using (var starBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 100)))
                using (var starPath = CreateStarPath(sx, sy, starSize))
                {
                    g.FillPath(starBrush, starPath);
                }
                
                // Outline
                using (var outlinePen = new Pen(Color.FromArgb(255, 255, 255, 200), 1.5f))
                using (var starPath = CreateStarPath(sx, sy, starSize))
                {
                    g.DrawPath(outlinePen, starPath);
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
        
        /// <summary>
        /// Create 5-pointed star path
        /// </summary>
        private GraphicsPath CreateStarPath(int centerX, int centerY, int size)
        {
            var path = new GraphicsPath();
            var points = new PointF[10]; // 5 outer + 5 inner points
            
            float outerRadius = size / 2f;
            float innerRadius = outerRadius * 0.4f; // Inner points closer to center
            
            for (int i = 0; i < 10; i++)
            {
                double angle = (Math.PI * 2 / 10 * i) - Math.PI / 2; // Start from top
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                
                points[i] = new PointF(
                    centerX + (float)(radius * Math.Cos(angle)),
                    centerY + (float)(radius * Math.Sin(angle))
                );
            }
            
            path.AddPolygon(points);
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = owner.BorderShape;  
            // Neon: vibrant RGB border with multi-color gradient
            using (var neonBrush = new LinearGradientBrush(owner.ClientRectangle,
                Color.FromArgb(200, 255, 0, 200),  // Magenta
                Color.FromArgb(200, 0, 255, 200),  // Cyan
                LinearGradientMode.Horizontal))
            {
                var blend = new ColorBlend(3);
                blend.Colors = new[] {
                    Color.FromArgb(200, 255, 0, 150),   // Pink
                    Color.FromArgb(200, 0, 255, 255),   // Cyan
                    Color.FromArgb(200, 100, 255, 100)  // Green
                };
                blend.Positions = new[] { 0f, 0.5f, 1f };
                neonBrush.InterpolationColors = blend;
                
                using var pen = new Pen(neonBrush, 3);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Neon: vibrant multi-color shadow with RGB tint
            return new ShadowEffect
            {
                Color = Color.FromArgb(70, 100, 50, 150), // Purple-pink tint
                Blur = 18, // Very wide blur for intense neon glow
                OffsetY = 9, // Large offset for dramatic neon effect
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(4); // Neon sharp corners for electric aesthetic
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
