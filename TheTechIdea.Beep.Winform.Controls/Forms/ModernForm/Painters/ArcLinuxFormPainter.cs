using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Arc Linux dark theme style (synced with ArcLinuxTheme).
    /// 
    /// Arc Linux Color Palette (synced with ArcLinuxTheme):
    /// - Background: #383C4A (56, 60, 74) - Dark blue-gray base
    /// - Foreground: #D3DAE3 (211, 218, 227) - Light gray text
    /// - Border: #2F343F (47, 52, 63) - Darker blue-gray border
    /// - Hover: #404552 (64, 69, 82) - Medium blue-gray hover
    /// - Selected: #5294E2 (82, 148, 226) - Arc blue selected
    /// 
    /// Features:
    /// - Flat design (no gradients)
    /// - Material design elevation line at top
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class ArcLinuxFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.ArcLinux, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Arc Linux flat design - solid fill only (no gradients)
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Arc material design: subtle elevation line at top (1px, alpha 60)
            using (var linePen = new Pen(Color.FromArgb(60, 255, 255, 255), 1))
            {
                g.DrawLine(linePen, 0, 0, owner.ClientRectangle.Width, 0);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Arc flat caption (no gradient)
            using (var capBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(capBrush, captionRect);
            }

            // Paint Arc hexagonal buttons (UNIQUE)
            PaintArcHexButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Arc hexagon buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Arc Linux hexagonal buttons (UNIQUE)
        /// </summary>
        /// <summary>
        /// Paint Arc Linux hexagonal buttons (UNIQUE)
        /// </summary>
        private void PaintArcHexButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int hexSize = 20; // Hexagon size
            int hexY = (captionRect.Height - hexSize) / 2;
            
            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;
            
            // Close button: Red hexagon with X
            int cx = closeRect.X + closeRect.Width / 2;
            using (var hexPath = CreateHexagonPath(cx, hexY + hexSize/2, hexSize/2))
            {
                // Hover: brighter fill
                Color closeColor = closeHovered ? Color.FromArgb(230, 80, 80) : Color.FromArgb(200, 60, 60);
                using (var hexBrush = new SolidBrush(closeColor))
                {
                    g.FillPath(hexBrush, hexPath);
                }
                
                // Hover: glow outline
                if (closeHovered)
                {
                    using (var glowPen = new Pen(Color.FromArgb(100, 255, 255, 255), 2f))
                    {
                        g.DrawPath(glowPen, hexPath);
                    }
                }
                
                // X icon (7px, white)
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int iconSize = 7;
                    g.DrawLine(iconPen, cx - iconSize/2, hexY + hexSize/2 - iconSize/2, 
                                       cx + iconSize/2, hexY + hexSize/2 + iconSize/2);
                    g.DrawLine(iconPen, cx + iconSize/2, hexY + hexSize/2 - iconSize/2, 
                                       cx - iconSize/2, hexY + hexSize/2 + iconSize/2);
                }
            }
            
            // Maximize button: Dark hexagon with square
            int mx = maxRect.X + maxRect.Width / 2;
            using (var hexPath = CreateHexagonPath(mx, hexY + hexSize/2, hexSize/2))
            {
                // Hover: lighter gray fill
                Color maxColor = maxHovered ? Color.FromArgb(110, 110, 110) : Color.FromArgb(80, 80, 80);
                using (var hexBrush = new SolidBrush(maxColor))
                {
                    g.FillPath(hexBrush, hexPath);
                }
                
                if (maxHovered)
                {
                    using (var glowPen = new Pen(Color.FromArgb(60, 255, 255, 255), 2f))
                    {
                        g.DrawPath(glowPen, hexPath);
                    }
                }
                
                // Square icon (6px)
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int sqSize = 6;
                    g.DrawRectangle(iconPen, mx - sqSize/2, hexY + hexSize/2 - sqSize/2, sqSize, sqSize);
                }
            }
            
            // Minimize button: Dark hexagon with line
            int mnx = minRect.X + minRect.Width / 2;
            using (var hexPath = CreateHexagonPath(mnx, hexY + hexSize/2, hexSize/2))
            {
                // Hover: lighter gray fill
                Color minColor = minHovered ? Color.FromArgb(110, 110, 110) : Color.FromArgb(80, 80, 80);
                using (var hexBrush = new SolidBrush(minColor))
                {
                    g.FillPath(hexBrush, hexPath);
                }
                
                if (minHovered)
                {
                    using (var glowPen = new Pen(Color.FromArgb(60, 255, 255, 255), 2f))
                    {
                        g.DrawPath(glowPen, hexPath);
                    }
                }
                
                // Line icon (7px)
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int lineSize = 7;
                    g.DrawLine(iconPen, mnx - lineSize/2, hexY + hexSize/2, 
                                       mnx + lineSize/2, hexY + hexSize/2);
                }
            }

            // Theme button (if shown): Arc blue hexagon
            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                int tx = themeRect.X + themeRect.Width / 2;
                using (var hexPath = CreateHexagonPath(tx, hexY + hexSize/2, hexSize/2))
                {
                    // Hover: brighter Arc blue
                    Color themeColor = themeHovered ? Color.FromArgb(115, 149, 177) : Color.FromArgb(95, 129, 157);
                    using (var hexBrush = new SolidBrush(themeColor)) 
                    {
                        g.FillPath(hexBrush, hexPath);
                    }
                    
                    if (themeHovered)
                    {
                        using (var glowPen = new Pen(Color.FromArgb(80, 255, 255, 255), 2f))
                        {
                            g.DrawPath(glowPen, hexPath);
                        }
                    }
                    
                    // Palette icon
                    using (var iconPen = new Pen(Color.White, 1.5f))
                    {
                        g.DrawEllipse(iconPen, tx - 3, hexY + hexSize/2 - 3, 6, 6);
                    }
                    g.FillEllipse(Brushes.White, tx - 1, hexY + hexSize/2 + 1, 2, 2);
                }
            }

            // Style button (if shown): Arc blue hexagon
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                int sx = styleRect.X + styleRect.Width / 2;
                using (var hexPath = CreateHexagonPath(sx, hexY + hexSize/2, hexSize/2))
                {
                    // Hover: brighter Arc blue
                    Color styleColor = styleHovered ? Color.FromArgb(115, 149, 177) : Color.FromArgb(95, 129, 157);
                    using (var hexBrush = new SolidBrush(styleColor))
                    {
                        g.FillPath(hexBrush, hexPath);
                    }
                    
                    if (styleHovered)
                    {
                        using (var glowPen = new Pen(Color.FromArgb(80, 255, 255, 255), 2f))
                        {
                            g.DrawPath(glowPen, hexPath);
                        }
                    }
                    
                    // Brush icon
                    using (var iconPen = new Pen(Color.White, 1.5f))
                    {
                        g.DrawLine(iconPen, sx - 2, hexY + hexSize/2 - 2, sx - 2, hexY + hexSize/2 + 2);
                        g.DrawLine(iconPen, sx - 2, hexY + hexSize/2, sx + 2, hexY + hexSize/2 + 2);
                    }
                }
            }
        }
        
        /// <summary>
        /// Create hexagon path (6-sided polygon)
        /// </summary>
        private GraphicsPath CreateHexagonPath(int centerX, int centerY, int radius)
        {
            var path = new GraphicsPath();
            var points = new PointF[6];
            
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i - Math.PI / 6; // Start from top
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
            // Arc thin border (1px)
            using var pen = new Pen(Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 50), metrics.BorderColor), 1);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Arc Linux material flat shadow (minimal blur)
            return new ShadowEffect
            {
                Color = Color.FromArgb(30, 0, 0, 0), // Slightly darker for flat material
                Blur = 6, // Minimal blur for flat aesthetic
                OffsetY = 3, // Small offset
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(4); // Arc small corner radius
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
