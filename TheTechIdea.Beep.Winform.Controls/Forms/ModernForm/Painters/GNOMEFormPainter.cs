using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// GNOME (Adwaita) form painter inspired by modern Linux desktop design.
    /// Features soft rounded headerbar, subtle shadows, and clean minimalist aesthetic.
    /// </summary>
    internal sealed class GNOMEFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            var metrics = FormPainterMetrics.DefaultFor(FormStyle.GNOME, owner.UseThemeColors ? owner.CurrentTheme : null);
            // GNOME prefers subtle rounded corners
            return metrics;
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var cornerRadius = GetCornerRadius(owner);
            
            using (var path = CreateRoundedPath(owner.ClientRectangle, cornerRadius))
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(brush, path);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // GNOME-style gradient headerbar
            using (var brush = new LinearGradientBrush(
                captionRect,
                metrics.CaptionTextColorActive,
                Color.FromArgb(250, metrics.CaptionTextColor.R, metrics.CaptionTextColor.G, metrics.CaptionTextColor.B),
                LinearGradientMode.Vertical))
            {
                var cornerRadius = GetCornerRadius(owner);
                using var path = CreateRoundedPath(new Rectangle(0, 0, owner.ClientSize.Width, captionRect.Height + cornerRadius.TopLeft), cornerRadius);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(brush, path);
            }

            // Subtle bottom separator
            using (var separatorPen = new Pen(Color.FromArgb(20, 0, 0, 0), 1f))
            {
                g.DrawLine(separatorPen, 0, captionRect.Bottom - 1, owner.ClientSize.Width, captionRect.Bottom - 1);
            }

            // Draw title text (centered for GNOME style)
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var cornerRadius = GetCornerRadius(owner);
            
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth))
            {
                Alignment = PenAlignment.Inset
            };
            
            var rect = new Rectangle(0, 0, owner.ClientSize.Width - 1, owner.ClientSize.Height - 1);
            using var path = CreateRoundedPath(rect, cornerRadius);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(30, 0, 0, 0),
                Blur = 12,
                OffsetY = 4,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(8); // GNOME uses subtle rounded corners
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High;
        }

        public bool SupportsAnimations => true;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            var shadow = GetShadowEffect(owner);
            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow);
            }

            PaintBackground(g, owner);
            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }
        }

        private void DrawShadow(Graphics g, Rectangle rect, ShadowEffect shadow)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            var shadowRect = new Rectangle(
                rect.X + shadow.OffsetX - shadow.Blur,
                rect.Y + shadow.OffsetY - shadow.Blur,
                rect.Width + shadow.Blur * 2,
                rect.Height + shadow.Blur * 2);
            if (shadowRect.Width <= 0 || shadowRect.Height <= 0) return;
            
            var cornerRadius = GetCornerRadius(null);
            using var path = CreateRoundedPath(shadowRect, cornerRadius);
            using var brush = new SolidBrush(shadow.Color);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(brush, path);
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            var captionHeight = owner.Font.Height + 20;
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            var buttonSize = new Size(44, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;
            
            // GNOME typically has close button on the right
            layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Icon on left (optional in GNOME)
            var iconSize = 18;
            var iconPadding = 12;
            layout.IconRect = new Rectangle(iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            // Title centered in available space
            var titleX = iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            owner.CurrentLayout = layout;
        }

        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var cornerRadius = GetCornerRadius(owner);
            
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            
            var rect = new Rectangle(0, 0, owner.Width - 1, owner.Height - 1);
            using var path = CreateRoundedPath(rect, cornerRadius);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);

            // Subtle top highlight
            using var highlightPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1f);
            using var highlightPath = CreateRoundedPath(
                new Rectangle(rect.X + borderThickness, rect.Y + borderThickness, rect.Width - borderThickness * 2, rect.Height / 3),
                new CornerRadius(cornerRadius.TopLeft - borderThickness, cornerRadius.TopRight - borderThickness, 0, 0));
            g.DrawPath(highlightPen, highlightPath);
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, CornerRadius cornerRadius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            var tl = Math.Min(cornerRadius.TopLeft, Math.Min(rect.Width / 2, rect.Height / 2));
            var tr = Math.Min(cornerRadius.TopRight, Math.Min(rect.Width / 2, rect.Height / 2));
            var br = Math.Min(cornerRadius.BottomRight, Math.Min(rect.Width / 2, rect.Height / 2));
            var bl = Math.Min(cornerRadius.BottomLeft, Math.Min(rect.Width / 2, rect.Height / 2));

            if (tl > 0) path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90);
            else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);

            if (tr > 0) path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90);
            else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);

            if (br > 0) path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90);
            else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);

            if (bl > 0) path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90);
            else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);

            path.CloseFigure();
            return path;
        }
    }
}
