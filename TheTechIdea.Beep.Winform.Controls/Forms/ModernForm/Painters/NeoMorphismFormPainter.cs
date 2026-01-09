using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Soft UI with extruded shadows and highlights (synced with NeoMorphismTheme).
    /// 
    /// NeoMorphism Color Palette (synced with NeoMorphismTheme):
    /// - Background: #E0E5EC (224, 229, 236) - Soft light gray base
    /// - Foreground: #4A5568 (74, 85, 104) - Dark gray text
    /// - Border: #D1D9E6 (209, 217, 230) - Subtle border
    /// - Hover: #EDF2F7 (237, 242, 247) - Lighter hover
    /// - Selected: #CBD5E0 (203, 213, 224) - Medium gray selected
    /// 
    /// Features:
    /// - Dual shadows (light and dark) for extruded 3D effect
    /// - Subtle embossed gradient
    /// - Monochromatic depth from shadows
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class NeoMorphismFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.NeoMorphism, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // NeoMorphism: Background with subtle embossed gradient
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // Base background
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Subtle gradient for embossed effect (5% lighter at top)
            var lightColor = ControlPaint.Light(metrics.BackgroundColor, 0.05f);
            using (var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle, 
                lightColor, 
                metrics.BackgroundColor, 
                LinearGradientMode.Vertical))
            {
                g.FillPath(gradBrush, path);
            }
            
            // Inner shadow for depth (top-left)
            DrawInnerShadow(g, owner.ClientRectangle, radius, true);
            
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
            
            // NeoMorphism: Caption uses SAME color as background (monochromatic depth from shadows)
            // Create rounded rectangle for top of form only
            var captionPath = new GraphicsPath();
            int tl = radius.TopLeft;
            int tr = radius.TopRight;
            
            if (tl > 0) captionPath.AddArc(captionRect.X, captionRect.Y, tl * 2, tl * 2, 180, 90);
            else captionPath.AddLine(captionRect.X, captionRect.Y, captionRect.X, captionRect.Y);
            
            if (tr > 0) captionPath.AddArc(captionRect.Right - tr * 2, captionRect.Y, tr * 2, tr * 2, 270, 90);
            else captionPath.AddLine(captionRect.Right, captionRect.Y, captionRect.Right, captionRect.Y);
            
            captionPath.AddLine(captionRect.Right, captionRect.Bottom, captionRect.X, captionRect.Bottom);
            captionPath.CloseFigure();
            
            // Use background color for caption (monochromatic)
            using (var capBrush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillPath(capBrush, captionPath);
            }
            
            // Subtle gradient for embossed caption
            var lightColor = ControlPaint.Light(metrics.BackgroundColor, 0.03f);
            using (var gradBrush = new LinearGradientBrush(
                captionRect, 
                lightColor, 
                metrics.BackgroundColor, 
                LinearGradientMode.Vertical))
            {
                g.FillPath(gradBrush, captionPath);
            }
            
            captionPath.Dispose();
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;

            // Paint NeoMorphismembossed soft buttons (UNIQUE)
            PaintNeoMorphismButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }
        
        /// <summary>
        /// Paint NeoMorphism embossed soft rectangle buttons (UNIQUE)
        /// </summary>
        private void PaintNeoMorphismButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int btnSize = 24;
            int btnY = (captionRect.Height - btnSize) / 2;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Close button: Embossed soft rectangle
            var closeBtn = new Rectangle(closeRect.X + (closeRect.Width - btnSize) / 2, btnY, btnSize, btnSize);
            PaintEmbossedButton(g, closeBtn, metrics.BackgroundColor, Color.FromArgb(200, 80, 80), "X");
            
            // Maximize button: Embossed soft rectangle
            var maxBtn = new Rectangle(maxRect.X + (maxRect.Width - btnSize) / 2, btnY, btnSize, btnSize);
            PaintEmbossedButton(g, maxBtn, metrics.BackgroundColor, metrics.CaptionTextColor, "□");
            
            // Minimize button: Embossed soft rectangle
            var minBtn = new Rectangle(minRect.X + (minRect.Width - btnSize) / 2, btnY, btnSize, btnSize);
            PaintEmbossedButton(g, minBtn, metrics.BackgroundColor, metrics.CaptionTextColor, "─");
        }
        
        /// <summary>
        /// Paint single embossed button with soft 3D depth
        /// </summary>
        private void PaintEmbossedButton(Graphics g, Rectangle rect, Color baseColor, Color iconColor, string iconType)
        {
            // Base fill (same as background for monochromatic)
            using (var baseBrush = new SolidBrush(baseColor))
            {
                g.FillRectangle(baseBrush, rect);
            }
            
            // Light shadow (top-left) for raised effect
            using (var lightPen = new Pen(Color.FromArgb(40, 255, 255, 255), 2))
            {
                g.DrawLine(lightPen, rect.Left, rect.Bottom - 2, rect.Left, rect.Top + 2);
                g.DrawLine(lightPen, rect.Left + 2, rect.Top, rect.Right - 2, rect.Top);
            }
            
            // Dark shadow (bottom-right) for depth
            using (var darkPen = new Pen(Color.FromArgb(40, 0, 0, 0), 2))
            {
                g.DrawLine(darkPen, rect.Right - 1, rect.Top + 2, rect.Right - 1, rect.Bottom - 1);
                g.DrawLine(darkPen, rect.Right - 2, rect.Bottom - 1, rect.Left + 2, rect.Bottom - 1);
            }
            
            // Icon
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            using (var iconPen = new Pen(iconColor, 1.5f))
            {
                int iconSize = 8;
                if (iconType == "X")
                {
                    g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                    g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
                }
                else if (iconType == "□")
                {
                    g.DrawRectangle(iconPen, cx - iconSize/2, cy - iconSize/2, iconSize, iconSize);
                }
                else if (iconType == "─")
                {
                    g.DrawLine(iconPen, cx - iconSize/2, cy, cx + iconSize/2, cy);
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
             using var path = owner.BorderShape;
            // NeoMorphism: Very thin border (1px) or nearly invisible
            using var pen = new Pen(metrics.BorderColor, 1);
            pen.Color = Color.FromArgb(30, metrics.BorderColor); // Very subtle
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // NeoMorphism: Large blur for soft shadow (bottom-right)
            return new ShadowEffect
            {
                Color = Color.FromArgb(30, 0, 0, 0), // Soft dark shadow
                Blur = 20, // Large blur for soft edges
                OffsetX = 8,
                OffsetY = 8,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(12);
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

            // NeoMorphism: Dual shadow system
            // 1. Dark shadow bottom-right
            DrawShadow(g, rect, shadow, radius);
            
            // 2. Light shadow top-left (opposite direction for embossed effect)
            var lightShadow = new ShadowEffect
            {
                Color = Color.FromArgb(40, 255, 255, 255), // Light highlight
                Blur = 20,
                OffsetX = -8,
                OffsetY = -8,
                Inner = false
            };
            DrawShadow(g, rect, lightShadow, radius);

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
        
        private void DrawInnerShadow(Graphics g, Rectangle rect, CornerRadius radius, bool isLight)
        {
            // NeoMorphism: Inner shadow for depth effect
            if (rect.Width <= 0 || rect.Height <= 0) return;
            
            var shadowColor = isLight 
                ? Color.FromArgb(20, 255, 255, 255) 
                : Color.FromArgb(20, 0, 0, 0);
            
            var innerRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
            using var path = CreateRoundedRectanglePath(innerRect, radius);
            using var pen = new Pen(shadowColor, 3);
            g.DrawPath(pen, path);
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
