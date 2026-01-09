using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Solarized light color scheme (synced with SolarizedTheme).
    /// 
    /// Solarized Color Palette (synced with SolarizedTheme):
    /// - Background: #FDF6E3 (253, 246, 227) - Light warm base
    /// - Foreground: #657B83 (101, 123, 131) - Blue-gray text
    /// - Border: #EEE8D5 (238, 232, 213) - Light warm border
    /// - Hover: #EEE8D5 (238, 232, 213) - Light warm hover
    /// - Selected: #268BD2 (38, 139, 210) - Bright blue selected
    /// 
    /// Features:
    /// - Flat design with pure solid fill
    /// - Minimal transparency (only separator line in caption)
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class SolarizedFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Solarized, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Solarized flat design - pure solid fill
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Flat caption
            using (var capBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Solarized subtle separator line at bottom of caption
            using (var sepPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
            {
                g.DrawLine(sepPen, captionRect.Left, captionRect.Bottom - 1, 
                                  captionRect.Right, captionRect.Bottom - 1);
            }

            // Paint Solarized diamond buttons (UNIQUE)
            PaintSolarizedDiamondButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Solarized diamond buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Solarized diamond-shaped buttons (UNIQUE - rotated squares)
        /// </summary>
        private void PaintSolarizedDiamondButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            var themeRect = owner.CurrentLayout.ThemeButtonRect;
            var styleRect = owner.CurrentLayout.StyleButtonRect;
            
            int diamondSize = 16;
            int diamondY = (captionRect.Height - diamondSize) / 2;
            
            // Close button: Red diamond with X
            int cx = closeRect.X + closeRect.Width / 2;
            using (var diamondPath = CreateDiamondPath(cx, diamondY + diamondSize/2, diamondSize/2))
            {
                using (var diamondBrush = new SolidBrush(Color.FromArgb(220, 50, 47)))
                {
                    g.FillPath(diamondBrush, diamondPath);
                }
                
                // X icon (6px)
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int iconSize = 6;
                    int cy = diamondY + diamondSize/2;
                    g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                    g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
                }
            }
            
            // Maximize button: Outline diamond with square
            int mx = maxRect.X + maxRect.Width / 2;
            using (var diamondPath = CreateDiamondPath(mx, diamondY + diamondSize/2, diamondSize/2))
            {
                using (var outlinePen = new Pen(metrics.CaptionTextColor, 1.5f))
                {
                    g.DrawPath(outlinePen, diamondPath);
                }
                
                // Square icon (5px)
                using (var iconPen = new Pen(metrics.CaptionTextColor, 1.2f))
                {
                    int sqSize = 5;
                    int my = diamondY + diamondSize/2;
                    g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
                }
            }
            
            // Minimize button: Outline diamond with line
            int mnx = minRect.X + minRect.Width / 2;
            using (var diamondPath = CreateDiamondPath(mnx, diamondY + diamondSize/2, diamondSize/2))
            {
                using (var outlinePen = new Pen(metrics.CaptionTextColor, 1.5f))
                {
                    g.DrawPath(outlinePen, diamondPath);
                }
                
                // Line icon (6px)
                using (var iconPen = new Pen(metrics.CaptionTextColor, 1.2f))
                {
                    int lineSize = 6;
                    int mny = diamondY + diamondSize/2;
                    g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
                }
            }
            
            // Theme button: Blue diamond with palette icon (Solarized blue #268bd2)
            if (!themeRect.IsEmpty)
            {
                int tx = themeRect.X + themeRect.Width / 2;
                using (var diamondPath = CreateDiamondPath(tx, diamondY + diamondSize/2, diamondSize/2))
                {
                    using (var diamondBrush = new SolidBrush(Color.FromArgb(38, 139, 210)))
                    {
                        g.FillPath(diamondBrush, diamondPath);
                    }
                    
                    // Palette icon (circles)
                    using (var iconBrush = new SolidBrush(Color.White))
                    {
                        int ty = diamondY + diamondSize/2;
                        g.FillEllipse(iconBrush, tx - 3, ty - 2, 3, 3);
                        g.FillEllipse(iconBrush, tx + 1, ty - 2, 3, 3);
                        g.FillEllipse(iconBrush, tx - 1, ty + 2, 3, 3);
                    }
                }
            }
            
            // Style button: Magenta diamond with brush icon (Solarized magenta #d33682)
            if (!styleRect.IsEmpty)
            {
                int sx = styleRect.X + styleRect.Width / 2;
                using (var diamondPath = CreateDiamondPath(sx, diamondY + diamondSize/2, diamondSize/2))
                {
                    using (var diamondBrush = new SolidBrush(Color.FromArgb(211, 54, 130)))
                    {
                        g.FillPath(diamondBrush, diamondPath);
                    }
                    
                    // Brush icon (triangle + line)
                    using (var iconPen = new Pen(Color.White, 1.2f))
                    {
                        int sy = diamondY + diamondSize/2;
                        // Brush bristles
                        g.DrawLine(iconPen, sx - 2, sy - 2, sx - 2, sy + 2);
                        g.DrawLine(iconPen, sx, sy - 2, sx, sy + 2);
                        g.DrawLine(iconPen, sx + 2, sy - 2, sx + 2, sy + 2);
                    }
                }
            }
        }
        
        /// <summary>
        /// Create diamond path (45° rotated square)
        /// </summary>
        private GraphicsPath CreateDiamondPath(int centerX, int centerY, int radius)
        {
            var path = new GraphicsPath();
            path.AddPolygon(new PointF[]
            {
                new PointF(centerX, centerY - radius),        // Top
                new PointF(centerX + radius, centerY),        // Right
                new PointF(centerX, centerY + radius),        // Bottom
                new PointF(centerX - radius, centerY)         // Left
            });
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = owner.BorderShape;
            using var pen = new Pen(Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 60), metrics.BorderColor), 1);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Solarized very subtle shadow
            return new ShadowEffect
            {
                Color = Color.FromArgb(18, 0, 0, 0),
                Blur = 7,
                OffsetY = 3,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(5); // Small corner radius
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High;
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

            using var path = GraphicsExtensions.CreateRoundedRectanglePath(owner.ClientRectangle, radius);
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
            using var path = GraphicsExtensions.CreateRoundedRectanglePath(shadowRect, radius);
            g.FillPath(brush, path);
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
            var outer = owner.BorderShape.GetBounds();
            using var path = GraphicsExtensions.CreateRoundedRectanglePath(outer, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
    }
}
