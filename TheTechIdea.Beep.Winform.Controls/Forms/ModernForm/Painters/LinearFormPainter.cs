using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Linear form painter with sleek, minimal chrome design.
    /// 
    /// Features:
    /// - Ultra-minimal design
    /// - Dark mode friendly
    /// - Minimal chrome
    /// - Subtle dividers
    /// - Clean typography
    /// - Very thin borders
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class LinearFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultForCached(FormStyle.Linear, owner);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Linear light gray background (#F5F5F5) or dark (#1A1A1A) based on theme
            Color bgColor = Color.FromArgb(245, 245, 245); // Light mode default
            if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
            {
                bgColor = Color.FromArgb(26, 26, 26); // Dark mode
            }
            
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Minimal elevation (almost none)
            FormPainterRenderHelper.PaintGradientBackground(g, owner.ClientRectangle,
                Color.FromArgb(3, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                LinearGradientMode.Vertical);
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Ultra-minimal caption background
            Color captionBg = Color.FromArgb(255, 255, 255);
            if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
            {
                captionBg = Color.FromArgb(30, 30, 30);
            }
            
            using (var brush = new SolidBrush(captionBg))
            {
                g.FillRectangle(brush, captionRect);
            }

            // Very subtle divider (0.5-1px)
            Color dividerColor = Color.FromArgb(230, 230, 230);
            if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
            {
                dividerColor = Color.FromArgb(60, 60, 60);
            }
            
            using var dividerPen = new Pen(dividerColor, 0.5f);
            g.DrawLine(dividerPen, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);

            // Paint Linear minimal buttons
            PaintLinearButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            // Draw title text with clean typography
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Paint icon only
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Linear minimal buttons with icon-only style
        /// Features: very minimal, icon-only style, subtle hover, ultra-thin borders
        /// </summary>
        /// <summary>
        /// Paint Linear minimal buttons with icon-only style
        /// Features: very minimal, icon-only style, subtle hover, ultra-thin borders
        /// </summary>
        private void PaintLinearButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonSize = 16;
            int padding = (captionRect.Height - buttonSize) / 2;

            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;

            // Close button - minimal red
            PaintLinearButton(g, closeRect, Color.FromArgb(239, 68, 68), padding, buttonSize, "close", owner, closeHovered);

            // Maximize button - minimal green
            PaintLinearButton(g, maxRect, Color.FromArgb(34, 197, 94), padding, buttonSize, "maximize", owner, maxHovered);

            // Minimize button - minimal blue
            PaintLinearButton(g, minRect, Color.FromArgb(59, 130, 246), padding, buttonSize, "minimize", owner, minHovered);

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintLinearButton(g, styleRect, Color.FromArgb(139, 92, 246), padding, buttonSize, "Style", owner, styleHovered);
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintLinearButton(g, themeRect, Color.FromArgb(245, 158, 11), padding, buttonSize, "theme", owner, themeHovered);
            }
        }

        private void PaintLinearButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int size, string buttonType, BeepiFormPro owner, bool isHovered = false)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Ultra-minimal button with very small rounded corners (4px)
            using (var buttonPath = CreateRoundedRectanglePath(rect, new CornerRadius(4)))
            {
                // Almost transparent background
                Color bgColor = Color.FromArgb(10, 255, 255, 255);
                if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
                {
                    bgColor = Color.FromArgb(10, 0, 0, 0);
                }
                
                // Hover: Darker/lighter background depending on theme
                if (isHovered)
                {
                     if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
                     {
                         bgColor = Color.FromArgb(40, 255, 255, 255); // Visible light grey on dark
                     }
                     else
                     {
                         bgColor = Color.FromArgb(20, 0, 0, 0); // Visible dark grey on light
                     }
                }

                using (var bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, buttonPath);
                }

                // Very thin border (0.5-1px)
                Color borderColor = Color.FromArgb(200, 200, 200);
                if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
                {
                    borderColor = Color.FromArgb(80, 80, 80);
                }
                
                // Hover: Colored border matching the button type
                if (isHovered)
                {
                    borderColor = baseColor;
                }
                
                using (var borderPen = new Pen(borderColor, 0.5f))
                {
                    g.DrawPath(borderPen, buttonPath);
                }

                // Subtle hover effect - Fill with base color at low alpha
                if (isHovered)
                {
                    using (var hoverBrush = new SolidBrush(Color.FromArgb(25, baseColor)))
                    {
                        g.FillPath(hoverBrush, buttonPath);
                    }
                }
            }

            // Draw icon (minimal, thin lines)
            Color iconColor = Color.FromArgb(120, 120, 120);
            if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
            {
                iconColor = Color.FromArgb(180, 180, 180);
            }
            
            // Hover: Colored icon
            if (isHovered)
            {
                iconColor = baseColor;
            }
            
            using (var iconPen = new Pen(iconColor, 1f))
            {
                int iconSize = 5;
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
                        // Minimal palette icon
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize);
                        break;
                    case "theme":
                        // Minimal sun icon
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 3, iconCenterY - iconSize / 3, iconSize * 2 / 3, iconSize * 2 / 3);
                        break;
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var path = owner.BorderShape; // Do NOT dispose - path is cached and owned by BeepiFormPro
            
            // Very thin border (0.5-1px)
            Color borderColor = Color.FromArgb(230, 230, 230);
            if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
            {
                borderColor = Color.FromArgb(60, 60, 60);
            }
            
            using var pen = new Pen(borderColor, 0.5f);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(15, 0, 0, 0),
                Blur = 6,
                OffsetY = 1,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(4);
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
            int captionPadding = DpiScalingHelper.ScaleValue(14, dpiScale);
            var captionHeight = owner.Font.Height + captionPadding;
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            int buttonWidth = DpiScalingHelper.ScaleValue(28, dpiScale);
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
            
            int iconSize = DpiScalingHelper.ScaleValue(14, dpiScale);
            int iconPadding = DpiScalingHelper.ScaleValue(6, dpiScale);
            layout.IconRect = new Rectangle(iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            var titleX = iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            owner.CurrentLayout = layout;
        }

        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var radius = GetCornerRadius(owner);
            var outerRect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outerRect, radius);
            
            Color borderColor = Color.FromArgb(230, 230, 230);
            if (owner.UseThemeColors && owner.CurrentTheme != null && owner.CurrentTheme.IsDarkTheme)
            {
                borderColor = Color.FromArgb(60, 60, 60);
            }
            
            using var pen = new Pen(borderColor, Math.Max(0.5f, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
    }
}
