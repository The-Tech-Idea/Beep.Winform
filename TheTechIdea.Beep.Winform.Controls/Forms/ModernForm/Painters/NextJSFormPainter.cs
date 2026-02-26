using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Next.js/Vercel form painter with modern, gradient-accented design.
    /// 
    /// Features:
    /// - Modern, Vercel-inspired aesthetic
    /// - Gradient accents
    /// - Subtle gradient overlay on caption
    /// - Rounded buttons with gradient hover effects
    /// - Thin borders with gradient accents
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class NextJSFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultForCached(FormStyle.NextJS, owner);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Next.js clean white background
            using (var brush = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Subtle texture gradient
            FormPainterRenderHelper.PaintGradientBackground(g, owner.ClientRectangle,
                Color.FromArgb(5, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                LinearGradientMode.Vertical);
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Subtle gradient overlay on caption (Vercel style)
            FormPainterRenderHelper.PaintGradientBackground(g, captionRect,
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(250, 250, 250),
                LinearGradientMode.Vertical);

            // Gradient accent border at bottom
            using (var gradientBrush = new LinearGradientBrush(
                new Rectangle(captionRect.Left, captionRect.Bottom - 2, captionRect.Width, 2),
                Color.FromArgb(100, 0, 112, 243), // Vercel blue
                Color.FromArgb(50, 139, 92, 246), // Purple accent
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(gradientBrush, captionRect.Left, captionRect.Bottom - 2, captionRect.Width, 2);
            }

            // Paint Next.js gradient buttons
            PaintNextJSButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            // Draw title text with modern typography
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Paint icon only
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Next.js gradient buttons with rounded corners and gradient hover effects
        /// Features: rounded (8px), gradient hover effects, modern aesthetic
        /// </summary>
        private void PaintNextJSButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonSize = 20;
            int padding = (captionRect.Height - buttonSize) / 2;

            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;

            // Close button - gradient red
            PaintNextJSButton(g, closeRect, 
                Color.FromArgb(239, 68, 68), 
                Color.FromArgb(220, 38, 38), 
                padding, buttonSize, "close", closeHovered);

            // Maximize button - gradient green
            PaintNextJSButton(g, maxRect, 
                Color.FromArgb(34, 197, 94), 
                Color.FromArgb(22, 163, 74), 
                padding, buttonSize, "maximize", maxHovered);

            // Minimize button - gradient blue (Vercel blue)
            PaintNextJSButton(g, minRect, 
                Color.FromArgb(59, 130, 246), 
                Color.FromArgb(37, 99, 235), 
                padding, buttonSize, "minimize", minHovered);

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintNextJSButton(g, styleRect, 
                    Color.FromArgb(139, 92, 246), 
                    Color.FromArgb(124, 58, 237), 
                    padding, buttonSize, "Style", styleHovered);
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintNextJSButton(g, themeRect, 
                    Color.FromArgb(245, 158, 11), 
                    Color.FromArgb(234, 88, 12), 
                    padding, buttonSize, "theme", themeHovered);
            }
        }

        private void PaintNextJSButton(Graphics g, Rectangle buttonRect, Color color1, Color color2, int padding, int size, string buttonType, bool isHovered = false)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Hover: color tweak
            if (isHovered)
            {
                color1 = ControlPaint.Light(color1, 0.2f);
                color2 = ControlPaint.Light(color2, 0.2f);
            }

            // Button with rounded corners (8px) and gradient
            using (var buttonPath = CreateRoundedRectanglePath(rect, new CornerRadius(8)))
            {
                // Gradient background
                using (var gradient = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Vertical))
                {
                    g.FillPath(gradient, buttonPath);
                }

                // Thin border with gradient accent
                using (var borderPen = new Pen(Color.FromArgb(180, 180, 180), 1f))
                {
                    g.DrawPath(borderPen, buttonPath);
                }

                // Gradient hover glow effect
                if (isHovered)
                {
                    using (var glowGradient = new LinearGradientBrush(
                        new Rectangle(rect.X - 4, rect.Y - 4, rect.Width + 8, rect.Height + 8),
                        Color.FromArgb(60, color1),
                        Color.FromArgb(0, color2),
                        LinearGradientMode.Vertical))
                    {
                        using (var glowPath = CreateRoundedRectanglePath(
                            new Rectangle(rect.X - 4, rect.Y - 4, rect.Width + 8, rect.Height + 8), 
                            new CornerRadius(12)))
                        {
                            g.FillPath(glowGradient, glowPath);
                        }
                    }
                }
            }

            // Draw icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 6;
                int iconCenterX = rect.X + rect.Width / 2;
                int iconCenterY = rect.Y + rect.Height / 2;

                switch (buttonType)
                {
                    case "close":
                        g.DrawLine(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2,
                            iconCenterX + iconSize / 2, iconCenterY + iconSize / 2);
                        g.DrawLine(iconPen, iconCenterX + iconSize / 2, iconCenterY - iconSize / 2,
                            iconCenterX - iconSize / 2, iconCenterY + iconSize / 2);
                        break;
                    case "maximize":
                        g.DrawRectangle(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize);
                        break;
                    case "minimize":
                        g.DrawLine(iconPen, iconCenterX - iconSize / 2, iconCenterY, iconCenterX + iconSize / 2, iconCenterY);
                        break;
                    case "Style":
                        // Palette icon
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize);
                        break;
                    case "theme":
                        // Sun icon
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 3, iconCenterY - iconSize / 3, iconSize * 2 / 3, iconSize * 2 / 3);
                        break;
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var path = owner.BorderShape; // Do NOT dispose - path is cached and owned by BeepiFormPro
            // Thin border with subtle gradient accent
            using var pen = new Pen(Color.FromArgb(220, 220, 220), 1f);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(20, 0, 0, 0),
                Blur = 12,
                OffsetY = 3,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(10);
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

            var path = owner.BorderShape; // Do NOT dispose - path is cached and owned by BeepiFormPro
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
            
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(owner);
            int captionPadding = DpiScalingHelper.ScaleValue(17, dpiScale);
            var captionHeight = owner.Font.Height + captionPadding;
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            int buttonWidth = DpiScalingHelper.ScaleValue(32, dpiScale);
            var buttonSize = new Size(buttonWidth, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;
            
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("Style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            if (owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Search box (between title and buttons)
            int searchBoxWidth = DpiScalingHelper.ScaleValue(200, dpiScale);
            int searchBoxPadding = DpiScalingHelper.ScaleValue(8, dpiScale);
            if (owner.ShowSearchBox)
            {
                layout.SearchBoxRect = new Rectangle(buttonX - searchBoxWidth - searchBoxPadding, buttonY + searchBoxPadding / 2, 
                    searchBoxWidth, captionHeight - searchBoxPadding);
                owner._hits.RegisterHitArea("search", layout.SearchBoxRect, HitAreaType.TextBox);
                buttonX -= searchBoxWidth + searchBoxPadding;
            }
            else
            {
                layout.SearchBoxRect = Rectangle.Empty;
            }
            
            int iconSize    = DpiScalingHelper.ScaleValue(16, dpiScale);
            int iconPadding = DpiScalingHelper.ScaleValue(8, dpiScale);
            int iconY       = (captionHeight - iconSize) / 2;
            var cornerRadiusNJS = GetCornerRadius(owner);
            int safeIconXNJS = FormPainterMetrics.GetCaptionLeftSafeX(
                cornerRadiusNJS.TopLeft, iconY + iconSize / 2, iconSize / 2) + 2;
            int adjustedIconPadding = Math.Max(iconPadding, safeIconXNJS);

            layout.IconRect = new Rectangle(adjustedIconPadding, iconY, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            var titleX     = adjustedIconPadding + iconSize + iconPadding;
            var titleWidth = Math.Max(0, buttonX - titleX - iconPadding);
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);

            // Expose safe corner insets for child controls
            int bottomRadius = Math.Max(cornerRadiusNJS.BottomLeft, cornerRadiusNJS.BottomRight);
            layout.SafeContentInsets = new System.Windows.Forms.Padding(
                left:   bottomRadius > 0 ? FormPainterMetrics.GetCaptionLeftSafeX(cornerRadiusNJS.BottomLeft,  layout.ContentRect.Height, layout.ContentRect.Height / 2) : 0,
                top:    0,
                right:  bottomRadius > 0 ? FormPainterMetrics.GetCaptionLeftSafeX(cornerRadiusNJS.BottomRight, layout.ContentRect.Height, layout.ContentRect.Height / 2) : 0,
                bottom: bottomRadius
            );
            
            owner.CurrentLayout = layout;
        }

        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var radius = GetCornerRadius(owner);
            var outerRect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outerRect, radius);
            using var pen = new Pen(Color.FromArgb(220, 220, 220), Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
    }
}
