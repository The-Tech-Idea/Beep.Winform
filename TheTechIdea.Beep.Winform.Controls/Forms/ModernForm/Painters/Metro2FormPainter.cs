using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Metro2 (updated Windows 10/11 Metro) form painter with accent colors and modern improvements.
    /// Features colored accent stripes, flat design with subtle depth, and modern fluent touches.
    /// </summary>
    internal sealed class Metro2FormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Metro2, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Subtle diagonal accent pattern
            using var accentPen = new Pen(Color.FromArgb(5, metrics.BorderColor), 1f);
            for (int i = -owner.ClientRectangle.Height; i < owner.ClientRectangle.Width; i += 40)
            {
                g.DrawLine(accentPen, i, 0, i + owner.ClientRectangle.Height, owner.ClientRectangle.Height);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Modern gradient caption
            using var brush = new LinearGradientBrush(
                captionRect,
                metrics.BorderColor,
                Color.FromArgb(240, metrics.BorderColor.R, metrics.BorderColor.G, metrics.BorderColor.B),
                LinearGradientMode.Horizontal);
            g.FillRectangle(brush, captionRect);

            // Colored accent stripe at top
            using var accentBrush = new SolidBrush(metrics.BorderColor);
            g.FillRectangle(accentBrush, new Rectangle(0, 0, owner.ClientSize.Width, 3));

            // Draw title text
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            g.DrawRectangle(pen, new Rectangle(0, 0, owner.ClientSize.Width - 1, owner.ClientSize.Height - 1));
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(20, 0, 0, 0),
                Blur = 10,
                OffsetY = 3,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(0); // Metro2 keeps flat edges
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Low;
        }

        public bool SupportsAnimations => false;

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
            using var brush = new SolidBrush(shadow.Color);
            g.FillRectangle(brush, shadowRect);
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            var captionHeight = owner.Font.Height + 20;
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            var buttonSize = new Size(48, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;
            
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
            
            var iconSize = 18;
            var iconPadding = 12;
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
            // Accent stripe at top
            using var accentBrush = new SolidBrush(metrics.BorderColor);
            g.FillRectangle(accentBrush, new Rectangle(0, 0, owner.Width, Math.Max(2, borderThickness)));
            
            // Side and bottom borders
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            var rect = new Rectangle(0, 0, owner.Width, owner.Height);
            g.DrawRectangle(pen, rect);
        }
    }
}
