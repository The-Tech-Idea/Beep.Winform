using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Holographic style with rainbow iridescent effects and chevron-shaped buttons (synced with HolographicTheme).
    /// 
    /// Holographic Color Palette (synced with HolographicTheme):
    /// - Background: #0A0A14 (10, 10, 20) - Deep dark base for holographic effects
    /// - Foreground: #E0E0FF (224, 224, 255) - Light purple-tinted text
    /// - Border: #4040FF (64, 64, 255) - Electric blue border
    /// - Hover: #1A1A2E (26, 26, 46) - Slightly lighter dark hover
    /// - Selected: #2A2A4E (42, 42, 78) - Medium dark selected
    /// 
    /// Features:
    /// - Multi-color rainbow gradient overlay (iridescent effect)
    /// - Prismatic shine effect (diagonal rainbow line)
    /// - Iridescent caption gradient
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class HolographicFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Holographic, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;

            // Holographic: base layer
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;

            // Rainbow gradient overlay (iridescent effect)
            using (var rainbowBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                Color.FromArgb(15, 255, 0, 100),   // Magenta
                Color.FromArgb(15, 0, 200, 255),   // Cyan
                LinearGradientMode.Horizontal))
            {
                var blend = new ColorBlend(5);
                blend.Colors = new[] {
                    Color.FromArgb(15, 255, 0, 100),   // Magenta
                    Color.FromArgb(15, 255, 200, 0),   // Orange
                    Color.FromArgb(15, 0, 255, 100),   // Green
                    Color.FromArgb(15, 100, 150, 255), // Blue
                    Color.FromArgb(15, 255, 0, 200)    // Pink
                };
                blend.Positions = new[] { 0f, 0.25f, 0.5f, 0.75f, 1f };
                rainbowBrush.InterpolationColors = blend;
                
                g.FillRectangle(rainbowBrush, owner.ClientRectangle);
            }
            
            // Prismatic shine effect (diagonal rainbow line)
            using (var shinePen = new Pen(Color.FromArgb(40, 255, 255, 255), 3))
            {
                g.DrawLine(shinePen, 0, owner.ClientRectangle.Height / 3,
                           owner.ClientRectangle.Width, 0);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // CRITICAL: Set compositing mode for semi-transparent overlays
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Holographic: iridescent caption gradient
            using (var capBrush = new LinearGradientBrush(captionRect,
                Color.FromArgb(20, 255, 0, 200),  // Magenta
                Color.FromArgb(20, 0, 200, 255),  // Cyan
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Base caption
            using (var baseBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(baseBrush, captionRect);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;

            // Paint Holographic chevron/arrow buttons (UNIQUE SKIN)
            PaintHolographicChevronButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Holographic chevron buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Holographic chevron/arrow shaped buttons (UNIQUE SKIN)
        /// Right-pointing arrow/chevron shapes with rainbow gradient
        /// </summary>
        private void PaintHolographicChevronButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            var themeRect = owner.CurrentLayout.ThemeButtonRect;
            var styleRect = owner.CurrentLayout.StyleButtonRect;
            
            int chevronSize = 18;
            int chevronY = (captionRect.Height - chevronSize) / 2;
            
            // Close button: Red iridescent chevron
            int cx = closeRect.X + closeRect.Width / 2;
            int cy = chevronY + chevronSize / 2;
            
            using (var chevronPath = CreateChevronPath(cx, cy, chevronSize))
            {
                // Rainbow gradient fill
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(cx - chevronSize/2, cy - chevronSize/2, chevronSize, chevronSize),
                    Color.FromArgb(200, 255, 100, 150),  // Pink
                    Color.FromArgb(200, 255, 200, 100),  // Orange
                    LinearGradientMode.Horizontal))
                {
                    g.FillPath(gradBrush, chevronPath);
                }
                
                // Rainbow outline
                using (var outlinePen = new Pen(Color.FromArgb(255, 255, 255, 200), 1.5f))
                {
                    g.DrawPath(outlinePen, chevronPath);
                }
            }
            
            // X icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 6;
                g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
            }
            
            // Maximize button: Cyan iridescent chevron
            int mx = maxRect.X + maxRect.Width / 2;
            int my = chevronY + chevronSize / 2;
            
            using (var chevronPath = CreateChevronPath(mx, my, chevronSize))
            {
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(mx - chevronSize/2, my - chevronSize/2, chevronSize, chevronSize),
                    Color.FromArgb(200, 100, 200, 255),  // Cyan
                    Color.FromArgb(200, 100, 255, 200),  // Green
                    LinearGradientMode.Horizontal))
                {
                    g.FillPath(gradBrush, chevronPath);
                }
                
                using (var outlinePen = new Pen(Color.FromArgb(255, 200, 255, 255), 1.5f))
                {
                    g.DrawPath(outlinePen, chevronPath);
                }
            }
            
            // Square icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int sqSize = 6;
                g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Purple iridescent chevron
            int mnx = minRect.X + minRect.Width / 2;
            int mny = chevronY + chevronSize / 2;
            
            using (var chevronPath = CreateChevronPath(mnx, mny, chevronSize))
            {
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(mnx - chevronSize/2, mny - chevronSize/2, chevronSize, chevronSize),
                    Color.FromArgb(200, 200, 100, 255),  // Purple
                    Color.FromArgb(200, 255, 100, 200),  // Magenta
                    LinearGradientMode.Horizontal))
                {
                    g.FillPath(gradBrush, chevronPath);
                }
                
                using (var outlinePen = new Pen(Color.FromArgb(255, 255, 200, 255), 1.5f))
                {
                    g.DrawPath(outlinePen, chevronPath);
                }
            }
            
            // Line icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int lineSize = 7;
                g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
            }
            
            // Theme button: Blue-green iridescent chevron with palette icon
            if (!themeRect.IsEmpty)
            {
                int tx = themeRect.X + themeRect.Width / 2;
                int ty = chevronY + chevronSize / 2;
                
                using (var chevronPath = CreateChevronPath(tx, ty, chevronSize))
                {
                    using (var gradBrush = new LinearGradientBrush(
                        new Rectangle(tx - chevronSize/2, ty - chevronSize/2, chevronSize, chevronSize),
                        Color.FromArgb(200, 50, 150, 255),   // Blue
                        Color.FromArgb(200, 50, 255, 150),   // Cyan-green
                        LinearGradientMode.Horizontal))
                    {
                        g.FillPath(gradBrush, chevronPath);
                    }
                    
                    using (var outlinePen = new Pen(Color.FromArgb(255, 100, 200, 255), 1.5f))
                    {
                        g.DrawPath(outlinePen, chevronPath);
                    }
                }
                
                // Palette icon
                using (var iconBrush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(iconBrush, tx - 3, ty - 2, 3, 3);
                    g.FillEllipse(iconBrush, tx + 1, ty - 2, 3, 3);
                    g.FillEllipse(iconBrush, tx - 1, ty + 2, 3, 3);
                }
            }
            
            // Style button: Yellow-orange iridescent chevron with brush icon
            if (!styleRect.IsEmpty)
            {
                int sx = styleRect.X + styleRect.Width / 2;
                int sy = chevronY + chevronSize / 2;
                
                using (var chevronPath = CreateChevronPath(sx, sy, chevronSize))
                {
                    using (var gradBrush = new LinearGradientBrush(
                        new Rectangle(sx - chevronSize/2, sy - chevronSize/2, chevronSize, chevronSize),
                        Color.FromArgb(200, 255, 200, 50),   // Yellow
                        Color.FromArgb(200, 255, 100, 50),   // Orange
                        LinearGradientMode.Horizontal))
                    {
                        g.FillPath(gradBrush, chevronPath);
                    }
                    
                    using (var outlinePen = new Pen(Color.FromArgb(255, 255, 200, 100), 1.5f))
                    {
                        g.DrawPath(outlinePen, chevronPath);
                    }
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
        /// Create chevron/arrow path pointing right (5-point polygon)
        /// </summary>
        private GraphicsPath CreateChevronPath(int centerX, int centerY, int size)
        {
            var path = new GraphicsPath();
            int halfSize = size / 2;
            
            // Chevron points (right-pointing arrow)
            var points = new PointF[]
            {
                new PointF(centerX - halfSize, centerY - halfSize),     // Top left
                new PointF(centerX + halfSize/2, centerY),              // Right point
                new PointF(centerX - halfSize, centerY + halfSize),     // Bottom left
                new PointF(centerX - halfSize/3, centerY + halfSize/3), // Inner bottom
                new PointF(centerX - halfSize/3, centerY - halfSize/3)  // Inner top
            };
            
            path.AddPolygon(points);
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = owner.BorderShape;
            // Holographic: rainbow iridescent border
            using (var rainbowBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                Color.FromArgb(150, 255, 0, 200),
                Color.FromArgb(150, 0, 255, 200),
                LinearGradientMode.Horizontal))
            {
                var blend = new ColorBlend(4);
                blend.Colors = new[] {
                    Color.FromArgb(150, 255, 0, 150),   // Magenta
                    Color.FromArgb(150, 0, 200, 255),   // Cyan
                    Color.FromArgb(150, 200, 255, 0),   // Yellow
                    Color.FromArgb(150, 255, 0, 200)    // Magenta
                };
                blend.Positions = new[] { 0f, 0.33f, 0.66f, 1f };
                rainbowBrush.InterpolationColors = blend;
                
                using var pen = new Pen(rainbowBrush, 2);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Holographic: prismatic rainbow-tinted shadow
            return new ShadowEffect
            {
                Color = Color.FromArgb(60, 100, 50, 150), // Purple-ish tint
                Blur = 16, // Wide blur for iridescent glow
                OffsetY = 8, // Large offset for dramatic depth
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(12); // Holographic smooth flowing corners
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
