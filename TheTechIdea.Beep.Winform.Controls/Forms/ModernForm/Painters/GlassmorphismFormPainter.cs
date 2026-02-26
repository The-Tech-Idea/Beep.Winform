using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Frosted glass with translucent blur effects (synced with GlassTheme).
    /// 
    /// Glassmorphism Color Palette (synced with GlassTheme):
    /// - Background: #ECF4FF (236, 244, 255) - Light blue frosted glass
    /// - Foreground: #111827 (17, 24, 39) - Dark gray text
    /// - Border: #C8DCF0 (200, 220, 240) - Visible border
    /// - Hover: #D8EAF8 (216, 234, 250) - Light blue hover
    /// - Selected: #BEDCF5 (190, 220, 245) - Medium blue selected
    /// 
    /// Features:
    /// - Multi-layer translucent background with frosted texture
    /// - Subtle white overlay for glass sheen effect
    /// - Frosted texture using dotted grid hatch pattern
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class GlassmorphismFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultForCached(FormStyle.Glassmorphism, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Glassmorphism: Semi-transparent background with frosted effect
            // Layer 1: Base color with transparency
            var transBackground = Color.FromArgb(210, metrics.BackgroundColor);
            using (var brush = new SolidBrush(transBackground))
            {
                g.FillPath(brush, path);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Layer 2: Frosted texture using hatching
            using (var hatchBrush = new HatchBrush(HatchStyle.DottedGrid, 
                Color.FromArgb(15, 255, 255, 255), 
                Color.Transparent))
            {
                g.FillPath(hatchBrush, path);
            }
            
            // Layer 3: Subtle white overlay for glass sheen
            var glassOverlay = Color.FromArgb(25, 255, 255, 255);
            using (var overlayBrush = new SolidBrush(glassOverlay))
            {
                var topHalf = new Rectangle(owner.ClientRectangle.X, owner.ClientRectangle.Y, 
                    owner.ClientRectangle.Width, owner.ClientRectangle.Height / 3);
                var topPath = CreateRoundedRectanglePath(topHalf, new CornerRadius(radius.TopLeft, radius.TopRight, 0, 0));
                g.FillPath(overlayBrush, topPath);
                topPath.Dispose();
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            
            // CRITICAL: Set compositing mode for semi-transparent overlays
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Glassmorphism: Translucent caption with frosted effect
            var transCaptionColor = Color.FromArgb(200, metrics.CaptionColor);
            
            var captionPath = new GraphicsPath();
            int tl = radius.TopLeft;
            int tr = radius.TopRight;
            
            if (tl > 0) captionPath.AddArc(captionRect.X, captionRect.Y, tl * 2, tl * 2, 180, 90);
            else captionPath.AddLine(captionRect.X, captionRect.Y, captionRect.X, captionRect.Y);
            
            if (tr > 0) captionPath.AddArc(captionRect.Right - tr * 2, captionRect.Y, tr * 2, tr * 2, 270, 90);
            else captionPath.AddLine(captionRect.Right, captionRect.Y, captionRect.Right, captionRect.Y);
            
            captionPath.AddLine(captionRect.Right, captionRect.Bottom, captionRect.X, captionRect.Bottom);
            captionPath.CloseFigure();
            
            using (var capBrush = new SolidBrush(transCaptionColor))
            {
                g.FillPath(capBrush, captionPath);
            }
            
            // Frosted texture overlay
            using (var hatchBrush = new HatchBrush(HatchStyle.DottedGrid, 
                Color.FromArgb(20, 255, 255, 255), 
                Color.Transparent))
            {
                g.FillPath(hatchBrush, captionPath);
            }
            
            captionPath.Dispose();
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;

            // Paint Glassmorphism frosted translucent circle buttons (UNIQUE)
            PaintGlassmorphismButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Glassmorphism buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Glassmorphism frosted translucent circle buttons (UNIQUE)
        /// </summary>
        private void PaintGlassmorphismButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            var themeRect = owner.CurrentLayout.ThemeButtonRect;
            var styleRect = owner.CurrentLayout.StyleButtonRect;
            
            int circleSize = 22;

            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Close button: Frosted translucent red circle
            PaintGlassButton(g, closeRect, Color.FromArgb(220, 80, 80), circleSize, "close", closeHovered);
            
            // Maximize button: Frosted translucent circle
            PaintGlassButton(g, maxRect, Color.FromArgb(200, 200, 200), circleSize, "maximize", maxHovered);
            
            // Minimize button: Frosted translucent circle
            PaintGlassButton(g, minRect, Color.FromArgb(200, 200, 200), circleSize, "minimize", minHovered);
            
            // Theme button: Frosted cyan/blue circle with palette icon
            if (!themeRect.IsEmpty)
            {
                PaintGlassButton(g, themeRect, Color.FromArgb(100, 180, 255), circleSize, "theme", themeHovered);
            }
            
            // Style button: Frosted purple circle with brush icon
            if (!styleRect.IsEmpty)
            {
                PaintGlassButton(g, styleRect, Color.FromArgb(180, 100, 255), circleSize, "Style", styleHovered);
            }
        }

        private void PaintGlassButton(Graphics g, Rectangle rect, Color baseColor, int size, string type, bool isHovered)
        {
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            var circleRect = new Rectangle(cx - size/2, cy - size/2, size, size);
            
            // Hover effect: Increase opacity/brightness
            int alpha = isHovered ? 200 : 120; // More opaque on hover
            if (type == "close") alpha = isHovered ? 220 : 150; // Close button slightly more opaque
            
            Color fill = Color.FromArgb(alpha, baseColor);
            if (isHovered) fill = ControlPaint.Light(fill, 0.2f);

            // Translucent fill
            using (var circleBrush = new SolidBrush(fill))
            {
                g.FillEllipse(circleBrush, circleRect);
            }
            
            // Frosted texture
            using (var hatchBrush = new HatchBrush(HatchStyle.DottedGrid, 
                Color.FromArgb(30, 255, 255, 255), 
                Color.Transparent))
            {
                g.FillEllipse(hatchBrush, circleRect);
            }
            
            // Glass border
            // Hover effect: Brighter border
            Color borderColor = isHovered ? Color.FromArgb(120, 255, 255, 255) : Color.FromArgb(80, 255, 255, 255);
            using (var borderPen = new Pen(borderColor, 2))
            {
                g.DrawEllipse(borderPen, circleRect);
            }
            
            // Icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 7;
                
                switch (type)
                {
                    case "close":
                        g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                        g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
                        break;
                    case "maximize":
                        g.DrawRectangle(iconPen, cx - 3, cy - 3, 6, 6);
                        break;
                    case "minimize":
                        g.DrawLine(iconPen, cx - 3, cy + 3, cx + 3, cy + 3);  // Fixed vertical position for minimize
                        break;
                    case "theme": // Palette dots
                        using (var iconBrush = new SolidBrush(Color.White))
                        {
                            g.FillEllipse(iconBrush, cx - 3, cy - 2, 3, 3);
                            g.FillEllipse(iconBrush, cx + 1, cy - 2, 3, 3);
                            g.FillEllipse(iconBrush, cx - 1, cy + 2, 3, 3);
                        }
                        break;
                    case "Style": // Brush icon
                         g.DrawLine(iconPen, cx - 2, cy - 2, cx - 2, cy + 2);
                        g.DrawLine(iconPen, cx, cy - 2, cx, cy + 2);
                        g.DrawLine(iconPen, cx + 2, cy - 2, cx + 2, cy + 2);
                        break;
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var path = owner.BorderShape; // Do NOT dispose - path is cached and owned by BeepiFormPro
            // Glassmorphism: Subtle border with white translucent overlay
            using (var pen = new Pen(metrics.BorderColor, 1))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }
            
            // Light glass edge
            using (var glassPen = new Pen(Color.FromArgb(50, 255, 255, 255), 2))
            {
                g.DrawPath(glassPen, path);
            }
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Glassmorphism: Light, airy shadow
            return new ShadowEffect
            {
                Color = Color.FromArgb(20, 0, 0, 0),
                Blur = 15, // Medium blur for soft glow
                OffsetX = 0,
                OffsetY = 6,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(14); // Larger radius for glass aesthetic
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra;
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
