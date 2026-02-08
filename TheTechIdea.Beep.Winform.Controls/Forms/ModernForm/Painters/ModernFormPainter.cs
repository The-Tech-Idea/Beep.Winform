using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Modern form painter with clean, contemporary design.
    /// 
    /// Features:
    /// - Smooth rounded corners
    /// - Subtle gradient overlay for depth (semi-transparent white)
    /// - Refined aesthetics with subtle shadows
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class ModernFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Modern, owner);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Subtle gradient overlay for depth (using helper)
            FormPainterRenderHelper.PaintGradientBackground(g, owner.ClientRectangle,
                Color.FromArgb(8, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Vertical);
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Subtle caption background (using helper)
            FormPainterRenderHelper.PaintGradientBackground(g, captionRect,
                Color.FromArgb(250, 250, 250),
                Color.FromArgb(245, 245, 245),
                LinearGradientMode.Vertical);

            // Thin bottom divider
            using var divPen = new Pen(Color.FromArgb(230, 230, 230), 1f);
            g.DrawLine(divPen, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);

            // Paint Modern beveled buttons (UNIQUE SKIN)
            PaintModernBeveledButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            // Draw title text
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) here because we paint custom beveled buttons
            // Only paint the icon (we handle theme/Style/system buttons ourselves)
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Modern beveled buttons with subtle 3D effect (UNIQUE SKIN)
        /// Features: beveled edges, inner shadow, micro-gradient, highlight line
        /// </summary>
        private void PaintModernBeveledButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
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

            // Close button - red with beveled edge
            PaintBeveledButton(g, closeRect, Color.FromArgb(230, 90, 90), padding, buttonSize, "close", closeHovered);

            // Maximize button - green with beveled edge
            PaintBeveledButton(g, maxRect, Color.FromArgb(100, 200, 100), padding, buttonSize, "maximize", maxHovered);

            // Minimize button - blue with beveled edge
            PaintBeveledButton(g, minRect, Color.FromArgb(100, 150, 230), padding, buttonSize, "minimize", minHovered);

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintBeveledButton(g, styleRect, Color.FromArgb(180, 140, 200), padding, buttonSize, "Style", styleHovered);
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintBeveledButton(g, themeRect, Color.FromArgb(230, 180, 90), padding, buttonSize, "theme", themeHovered);
            }
        }

        private void PaintBeveledButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int size, string buttonType, bool isHovered = false)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Hover effect: lighter base color
            if (isHovered)
            {
                baseColor = ControlPaint.Light(baseColor, 0.2f);
            }

            // Bevel outer edge (subtle 3D effect)
            using (var bevelPath = CreateRoundedRectanglePath(new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Height + 2), new CornerRadius(4)))
            {
                using (var lightBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                {
                    g.FillPath(lightBrush, bevelPath);
                }
            }

            // Inner shadow for depth
            using (var shadowPath = CreateRoundedRectanglePath(new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2), new CornerRadius(3)))
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }

            // Micro-gradient fill (light to slightly darker)
            using (var buttonPath = CreateRoundedRectanglePath(rect, new CornerRadius(3)))
            {
                using (var gradient = new LinearGradientBrush(rect,
                    baseColor,
                    ControlPaint.Dark(baseColor, 0.1f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradient, buttonPath);
                }

                // Border - brighter on hover
                Color borderColor = isHovered ? Color.FromArgb(180, 255, 255, 255) : ControlPaint.Dark(baseColor, 0.2f);
                using (var borderPen = new Pen(borderColor, 1))
                {
                    g.DrawPath(borderPen, buttonPath);
                }
            }

            // Highlight line on top edge
            using (var highlightPen = new Pen(Color.FromArgb(60, 255, 255, 255), 1))
            {
                g.DrawLine(highlightPen, rect.X + 3, rect.Y + 2, rect.Right - 3, rect.Y + 2);
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
                        g.FillEllipse(Brushes.White, iconCenterX - 1, iconCenterY - 1, 2, 2);
                        break;
                    case "theme":
                        // Sun/moon icon
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 3, iconCenterY - iconSize / 3, iconSize * 2 / 3, iconSize * 2 / 3);
                        for (int i = 0; i < 8; i++)
                        {
                            double angle = Math.PI * 2 * i / 8;
                            int x1 = iconCenterX + (int)(Math.Cos(angle) * iconSize / 2);
                            int y1 = iconCenterY + (int)(Math.Sin(angle) * iconSize / 2);
                            int x2 = iconCenterX + (int)(Math.Cos(angle) * iconSize * 0.7);
                            int y2 = iconCenterY + (int)(Math.Sin(angle) * iconSize * 0.7);
                            g.DrawLine(iconPen, x1, y1, x2, y2);
                        }
                        break;
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            // Use BorderShape property - this returns the correct GraphicsPath for the form's shape
            using var path = owner.BorderShape;
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(25, 0, 0, 0),
                Blur = 10,
                OffsetY = 4,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(8);
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

            // Use BorderShape for clipping region
            using var path = owner.BorderShape;
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
            
            // If caption bar is hidden, skip button layout
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            // Get DPI scale factor for current display
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(owner);
            
            // Scale all sizes based on DPI
            int captionPadding = DpiScalingHelper.ScaleValue(16, dpiScale);
            var captionHeight = owner.Font.Height + captionPadding;
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            int buttonWidth = DpiScalingHelper.ScaleValue(32, dpiScale);
            var buttonSize = new Size(buttonWidth, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;
            
            // Close button
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Maximize/Minimize buttons
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
            
            // Custom action button (only if ShowCustomActionButton is true)
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
            
            int iconSize = DpiScalingHelper.ScaleValue(16, dpiScale);
            int iconPadding = DpiScalingHelper.ScaleValue(8, dpiScale);
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
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var outerRect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outerRect, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
    }
}
