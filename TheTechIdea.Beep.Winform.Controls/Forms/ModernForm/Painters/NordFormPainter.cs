using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Nord theme with frost gradients and rounded triangle buttons (synced with NordTheme).
    /// 
    /// Nord Color Palette (synced with NordTheme):
    /// - Background: #2E3440 (46, 52, 64) - Dark blue-gray base (Polar Night)
    /// - Foreground: #ECEFF4 (236, 239, 244) - Light gray text (Snow Storm)
    /// - Border: #4C566A (76, 86, 106) - Medium gray border (Polar Night)
    /// - Hover: #3B4252 (59, 66, 82) - Lighter hover (Polar Night)
    /// - Selected: #5E81AC (94, 129, 172) - Frost blue selected
    /// 
    /// Features:
    /// - Frost gradient overlay (icy blue-white tint from top)
    /// - Subtle frost line at top
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class NordFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Nord, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;

            // Nord: frost gradient background
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Frost gradient overlay (subtle blue-white tint from top - using helper)
            var frostRect = new Rectangle(0, 0, owner.ClientRectangle.Width, owner.ClientRectangle.Height / 3);
            FormPainterRenderHelper.PaintGradientBackground(g, frostRect,
                Color.FromArgb(18, 200, 220, 240),  // Icy blue-white
                Color.FromArgb(0, 200, 220, 240),
                LinearGradientMode.Vertical);
            
            // Subtle frost line at top (icy blue)
            using (var linePen = new Pen(Color.FromArgb(40, 180, 200, 230), 1))
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
            
            // Nord: frost gradient caption (using helper)
            FormPainterRenderHelper.PaintGradientBackground(g, captionRect,
                Color.FromArgb(10, 200, 220, 240),
                Color.FromArgb(5, 200, 220, 240),
                LinearGradientMode.Vertical);
            
            // Base caption
            using (var baseBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(baseBrush, captionRect);
            }

            // Paint Nord rounded triangle buttons (UNIQUE SKIN)
            PaintNordTriangleButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Nord triangle buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Nord rounded triangle buttons (UNIQUE SKIN)
        /// Smooth rounded triangles with frost gradient fills
        /// </summary>
        private void PaintNordTriangleButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int triangleSize = 18;
            int triangleY = (captionRect.Height - triangleSize) / 2;
            
            // Close button: Red-frost triangle
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            int cx = closeRect.X + closeRect.Width / 2;
            int cy = triangleY + triangleSize / 2;
            
            using (var trianglePath = CreateRoundedTrianglePath(cx, cy, triangleSize))
            {
                // Frost gradient fill (red with icy tint) - brighter on hover
                int alpha = closeHovered ? 255 : 200;
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(cx - triangleSize/2, cy - triangleSize/2, triangleSize, triangleSize),
                    Color.FromArgb(alpha, 191, 97, 106),  // Nord red
                    Color.FromArgb(alpha, 180, 90, 100),  // Darker
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradBrush, trianglePath);
                }
                
                // Frost outline - brighter and thicker on hover
                int outlineAlpha = closeHovered ? 220 : 150;
                float outlineWidth = closeHovered ? 2f : 1.5f;
                using (var outlinePen = new Pen(Color.FromArgb(outlineAlpha, 220, 230, 240), outlineWidth))
                {
                    g.DrawPath(outlinePen, trianglePath);
                }
            }
            
            // X icon
            using (var iconPen = new Pen(Color.FromArgb(255, 236, 239, 244), 1.5f)) // Nord snow white
            {
                int iconSize = 6;
                g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
            }
            
            // Maximize button: Blue-frost triangle
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            int mx = maxRect.X + maxRect.Width / 2;
            int my = triangleY + triangleSize / 2;
            
            using (var trianglePath = CreateRoundedTrianglePath(mx, my, triangleSize))
            {
                // Frost gradient (blue) - brighter on hover
                int alpha = maxHovered ? 255 : 200;
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(mx - triangleSize/2, my - triangleSize/2, triangleSize, triangleSize),
                    Color.FromArgb(alpha, 129, 161, 193),  // Nord frost blue
                    Color.FromArgb(alpha, 120, 150, 180),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradBrush, trianglePath);
                }
                
                int outlineAlpha = maxHovered ? 220 : 150;
                float outlineWidth = maxHovered ? 2f : 1.5f;
                using (var outlinePen = new Pen(Color.FromArgb(outlineAlpha, 220, 230, 240), outlineWidth))
                {
                    g.DrawPath(outlinePen, trianglePath);
                }
            }
            
            // Square icon
            using (var iconPen = new Pen(Color.FromArgb(255, 236, 239, 244), 1.5f))
            {
                int sqSize = 6;
                g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Teal-frost triangle
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            int mnx = minRect.X + minRect.Width / 2;
            int mny = triangleY + triangleSize / 2;
            
            using (var trianglePath = CreateRoundedTrianglePath(mnx, mny, triangleSize))
            {
                // Frost gradient (teal) - brighter on hover
                int alpha = minHovered ? 255 : 200;
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(mnx - triangleSize/2, mny - triangleSize/2, triangleSize, triangleSize),
                    Color.FromArgb(alpha, 136, 192, 208),  // Nord aurora teal
                    Color.FromArgb(alpha, 125, 180, 195),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradBrush, trianglePath);
                }
                
                int outlineAlpha = minHovered ? 220 : 150;
                float outlineWidth = minHovered ? 2f : 1.5f;
                using (var outlinePen = new Pen(Color.FromArgb(outlineAlpha, 220, 230, 240), outlineWidth))
                {
                    g.DrawPath(outlinePen, trianglePath);
                }
            }
            
            // Line icon
            using (var iconPen = new Pen(Color.FromArgb(255, 236, 239, 244), 1.5f))
            {
                int lineSize = 7;
                g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
            }
            
            // Style button: Purple-frost triangle (if shown)
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                int sx = styleRect.X + styleRect.Width / 2;
                int sy = triangleY + triangleSize / 2;
                
                using (var trianglePath = CreateRoundedTrianglePath(sx, sy, triangleSize))
                {
                    // Frost gradient (purple)
                    using (var gradBrush = new LinearGradientBrush(
                        new Rectangle(sx - triangleSize/2, sy - triangleSize/2, triangleSize, triangleSize),
                        Color.FromArgb(200, 180, 142, 173),  // Nord aurora purple
                        Color.FromArgb(200, 170, 130, 160),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(gradBrush, trianglePath);
                    }
                    
                    using (var outlinePen = new Pen(Color.FromArgb(150, 220, 230, 240), 1.5f))
                    {
                        g.DrawPath(outlinePen, trianglePath);
                    }
                }
                
                // Tree/Style icon (Nordic nature)
                using (var iconPen = new Pen(Color.FromArgb(255, 236, 239, 244), 1.5f))
                {
                    int iconSize = 6;
                    // Trunk
                    g.DrawLine(iconPen, sx, sy - iconSize/2, sx, sy + iconSize/2);
                    // Branches
                    g.DrawLine(iconPen, sx - iconSize/3, sy - iconSize/4, sx + iconSize/3, sy - iconSize/4);
                    g.DrawLine(iconPen, sx - iconSize/4, sy, sx + iconSize/4, sy);
                }
            }
            
            // Theme button: Orange-frost triangle (if shown)
            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                int tx = themeRect.X + themeRect.Width / 2;
                int ty = triangleY + triangleSize / 2;
                
                using (var trianglePath = CreateRoundedTrianglePath(tx, ty, triangleSize))
                {
                    // Frost gradient (orange)
                    using (var gradBrush = new LinearGradientBrush(
                        new Rectangle(tx - triangleSize/2, ty - triangleSize/2, triangleSize, triangleSize),
                        Color.FromArgb(200, 208, 135, 112),  // Nord aurora orange
                        Color.FromArgb(200, 195, 125, 100),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(gradBrush, trianglePath);
                    }
                    
                    using (var outlinePen = new Pen(Color.FromArgb(150, 220, 230, 240), 1.5f))
                    {
                        g.DrawPath(outlinePen, trianglePath);
                    }
                }
                
                // Mountain/Theme icon (Nordic landscape)
                using (var iconPen = new Pen(Color.FromArgb(255, 236, 239, 244), 1.5f))
                {
                    int iconSize = 6;
                    var points = new PointF[] {
                        new PointF(tx - iconSize/2, ty + iconSize/3),
                        new PointF(tx, ty - iconSize/2),
                        new PointF(tx + iconSize/2, ty + iconSize/3)
                    };
                    g.DrawPolygon(iconPen, points);
                }
            }
        }
        
        /// <summary>
        /// Create rounded triangle path (3-sided with curved corners)
        /// </summary>
        private GraphicsPath CreateRoundedTrianglePath(int centerX, int centerY, int size)
        {
            var path = new GraphicsPath();
            int halfSize = size / 2;
            
            // Triangle points (pointing up)
            var p1 = new PointF(centerX, centerY - halfSize);         // Top
            var p2 = new PointF(centerX + halfSize, centerY + halfSize/2);   // Bottom right
            var p3 = new PointF(centerX - halfSize, centerY + halfSize/2);   // Bottom left
            
            // Create rounded triangle by adding curves between points
            path.AddCurve(new[] { p1, p2, p3, p1 }, 0.5f);
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
            
            int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));
            // NOTE: _hits.Clear() is handled by EnsureLayoutCalculated - do not call here
            
            // Check if caption bar should be hidden
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
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
