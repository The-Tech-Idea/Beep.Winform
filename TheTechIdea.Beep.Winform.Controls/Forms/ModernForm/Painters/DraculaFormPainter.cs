using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Popular Dracula dark theme with purple accents (synced with DraculaTheme).
    /// 
    /// Dracula Color Palette (synced with DraculaTheme):
    /// - Background: #282A36 (40, 42, 54) - Dark purple-gray base
    /// - Foreground: #F8F8F2 (248, 248, 242) - Off-white text
    /// - Border: #6272A4 (98, 114, 164) - Blue-gray border
    /// - Hover: #44475A (68, 71, 90) - Lighter purple-gray hover
    /// - Selected: #6272A4 (98, 114, 164) - Blue-gray selected
    /// 
    /// Features:
    /// - Vignette effect with darker edges
    /// - Purple accent highlights
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class DraculaFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultForCached(FormStyle.Dracula, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Base fill
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Dracula vignette effect: darker edges (alpha 25, 30px inset)
            using (var vignettePath = new GraphicsPath())
            {
                var vignetteRect = new Rectangle(30, 30, 
                    owner.ClientRectangle.Width - 60, 
                    owner.ClientRectangle.Height - 60);
                
                if (vignetteRect.Width > 0 && vignetteRect.Height > 0)
                {
                    using (var pgb = new PathGradientBrush(new[] {
                        new Point(vignetteRect.Left, vignetteRect.Top),
                        new Point(vignetteRect.Right, vignetteRect.Top),
                        new Point(vignetteRect.Right, vignetteRect.Bottom),
                        new Point(vignetteRect.Left, vignetteRect.Bottom)
                    }))
                    {
                        pgb.CenterPoint = new PointF(
                            vignetteRect.X + vignetteRect.Width / 2f,
                            vignetteRect.Y + vignetteRect.Height / 2f);
                        pgb.CenterColor = Color.FromArgb(0, 0, 0, 0);
                        pgb.SurroundColors = new[] { Color.FromArgb(25, 0, 0, 0) };
                        
                        g.FillRectangle(pgb, owner.ClientRectangle);
                    }
                }
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Caption base
            using (var capBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Dracula subtle halo glow below caption (purple tint, 8px)
            using (var haloBrush = new SolidBrush(Color.FromArgb(15, 150, 100, 200))) // Purple tint
            {
                var haloRect = new Rectangle(captionRect.X, captionRect.Bottom, 
                    captionRect.Width, 8);
                g.FillRectangle(haloBrush, haloRect);
            }

            // Paint Dracula fang-shaped buttons (UNIQUE)
            PaintDraculaFangButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Dracula fang buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Dracula fang-shaped buttons (UNIQUE - vampire theme)
        /// </summary>
        private void PaintDraculaFangButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            var themeRect = owner.CurrentLayout.ThemeButtonRect;
            var styleRect = owner.CurrentLayout.StyleButtonRect;
            
            int fangHeight = 18; // Fang triangle height
            int fangY = (captionRect.Height - fangHeight) / 2;
            
            // Close button: Red fang with X
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            int cx = closeRect.X + closeRect.Width / 2;
            using (var fangPath = CreateFangPath(cx, fangY, fangHeight))
            {
                int alpha = closeHovered ? 255 : 200;
                using (var fangBrush = new SolidBrush(Color.FromArgb(alpha, 80, 80)))
                {
                    g.FillPath(fangBrush, fangPath);
                }
                
                // X icon (7px, white)
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int iconSize = 7;
                    int cy = fangY + fangHeight / 2;
                    g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, 
                                       cx + iconSize/2, cy + iconSize/2);
                    g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, 
                                       cx - iconSize/2, cy + iconSize/2);
                }
            }
            
            // Maximize button: Dark fang with square
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            int mx = maxRect.X + maxRect.Width / 2;
            using (var fangPath = CreateFangPath(mx, fangY, fangHeight))
            {
                int grayVal = maxHovered ? 120 : 90;
                using (var fangBrush = new SolidBrush(Color.FromArgb(grayVal, grayVal, grayVal)))
                {
                    g.FillPath(fangBrush, fangPath);
                }
                
                // Square icon (6px)
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int sqSize = 6;
                    int my = fangY + fangHeight / 2;
                    g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
                }
            }
            
            // Minimize button: Dark fang with line
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            int mnx = minRect.X + minRect.Width / 2;
            using (var fangPath = CreateFangPath(mnx, fangY, fangHeight))
            {
                int grayVal = minHovered ? 120 : 90;
                using (var fangBrush = new SolidBrush(Color.FromArgb(grayVal, grayVal, grayVal)))
                {
                    g.FillPath(fangBrush, fangPath);
                }
                
                // Line icon (7px)
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int lineSize = 7;
                    int mny = fangY + fangHeight / 2;
                    g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
                }
            }
            
            // Theme button: Purple fang with palette icon (Dracula purple #bd93f9)
            if (!themeRect.IsEmpty)
            {
                bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
                int tx = themeRect.X + themeRect.Width / 2;
                using (var fangPath = CreateFangPath(tx, fangY, fangHeight))
                {
                    int alpha = themeHovered ? 255 : 189;
                    using (var fangBrush = new SolidBrush(Color.FromArgb(alpha, 147, 249)))
                    {
                        g.FillPath(fangBrush, fangPath);
                    }
                    
                    // Palette icon (circles)
                    using (var iconBrush = new SolidBrush(Color.White))
                    {
                        int ty = fangY + fangHeight / 2;
                        g.FillEllipse(iconBrush, tx - 3, ty - 2, 3, 3);
                        g.FillEllipse(iconBrush, tx + 1, ty - 2, 3, 3);
                        g.FillEllipse(iconBrush, tx - 1, ty + 2, 3, 3);
                    }
                }
            }
            
            // Style button: Pink fang with brush icon (Dracula pink #ff79c6)
            if (!styleRect.IsEmpty)
            {
                bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;
                int sx = styleRect.X + styleRect.Width / 2;
                using (var fangPath = CreateFangPath(sx, fangY, fangHeight))
                {
                    int alpha = styleHovered ? 255 : 121;
                    using (var fangBrush = new SolidBrush(Color.FromArgb(255, alpha, 198)))
                    {
                        g.FillPath(fangBrush, fangPath);
                    }
                    
                    // Brush icon (lines)
                    using (var iconPen = new Pen(Color.White, 1.2f))
                    {
                        int sy = fangY + fangHeight / 2;
                        g.DrawLine(iconPen, sx - 2, sy - 2, sx - 2, sy + 2);
                        g.DrawLine(iconPen, sx, sy - 2, sx, sy + 2);
                        g.DrawLine(iconPen, sx + 2, sy - 2, sx + 2, sy + 2);
                    }
                }
            }
        }
        
        /// <summary>
        /// Create fang-shaped path (rounded triangle pointing down - vampire fang)
        /// </summary>
        private GraphicsPath CreateFangPath(int centerX, int topY, int height)
        {
            var path = new GraphicsPath();
            int width = height; // Equal width and height for balanced fang
            
            // Triangle points with rounded tip at bottom
            var points = new PointF[]
            {
                new PointF(centerX, topY),                          // Top center
                new PointF(centerX + width/2, topY + height * 0.6f), // Right side
                new PointF(centerX, topY + height),                 // Bottom point (fang tip)
                new PointF(centerX - width/2, topY + height * 0.6f)  // Left side
            };
            
            path.AddCurve(points, 0.3f); // Tension 0.3 for slight rounding
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
             var path = owner.BorderShape; // Do NOT dispose - path is cached and owned by BeepiFormPro
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Dracula purple glow effect (3 layers: blur 12, blur 6, solid)
            using (var glowPen1 = new Pen(Color.FromArgb(20, 150, 100, 200), 8))
            {
                g.DrawPath(glowPen1, path);
            }
            using (var glowPen2 = new Pen(Color.FromArgb(30, 150, 100, 200), 4))
            {
                g.DrawPath(glowPen2, path);
            }
            
            // Main border (thin with reduced alpha)
            using var pen = new Pen(Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 30), metrics.BorderColor), 1);
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Dracula shadow with purple tint
            return new ShadowEffect
            {
                Color = Color.FromArgb(30, 20, 10, 30), // Purple-tinted shadow
                Blur = 14,
                OffsetY = 6,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(7); // Medium corner radius
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra; // Smooth rendering for glow effects
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
