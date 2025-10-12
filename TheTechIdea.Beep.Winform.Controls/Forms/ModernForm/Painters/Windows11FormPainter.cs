using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Windows 11 rounded corners with mica
    /// </summary>
    internal sealed class Windows11FormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Windows11, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // Windows11: Base with subtle noise for Mica material
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillPath(brush, path);
            }
            
            // Mica texture - subtle noise pattern (UNIQUE to Windows11!)
            var random = new Random(42); // Fixed seed for consistent pattern
            for (int i = 0; i < 200; i++)
            {
                int x = random.Next(owner.ClientRectangle.Width);
                int y = random.Next(owner.ClientRectangle.Height);
                int size = random.Next(1, 3);
                var noiseAlpha = random.Next(3, 8);
                using (var noiseBrush = new SolidBrush(Color.FromArgb(noiseAlpha, 255, 255, 255)))
                {
                    g.FillRectangle(noiseBrush, x, y, size, size);
                }
            }
            
            // Acrylic layer - very subtle
            using (var acrylicBrush = new SolidBrush(Color.FromArgb(5, 255, 255, 255)))
            {
                g.FillPath(acrylicBrush, path);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            
            // Windows11: Acrylic caption with Mica texture
            var radius = GetCornerRadius(owner);
            var captionPath = new GraphicsPath();
            int tl = radius.TopLeft;
            int tr = radius.TopRight;
            
            if (tl > 0) captionPath.AddArc(captionRect.X, captionRect.Y, tl * 2, tl * 2, 180, 90);
            else captionPath.AddLine(captionRect.X, captionRect.Y, captionRect.X, captionRect.Y);
            
            if (tr > 0) captionPath.AddArc(captionRect.Right - tr * 2, captionRect.Y, tr * 2, tr * 2, 270, 90);
            else captionPath.AddLine(captionRect.Right, captionRect.Y, captionRect.Right, captionRect.Y);
            
            captionPath.AddLine(captionRect.Right, captionRect.Bottom, captionRect.X, captionRect.Bottom);
            captionPath.CloseFigure();
            
            using var capBrush = new SolidBrush(metrics.CaptionColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(capBrush, captionPath);
            
            captionPath.Dispose();

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Windows11: Square icon-style buttons (DISTINCT from iOS circles!)
            PaintWindows11Buttons(g, owner, captionRect, metrics);
        }
        
        private void PaintWindows11Buttons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            // Windows11: Square/rectangular buttons with hover states (UNIQUE!)
            int buttonWidth = 46;
            int buttonHeight = captionRect.Height;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Close button - square with red on hover (draw subtle background)
            var closeRect = new Rectangle(buttonX, 0, buttonWidth, buttonHeight);
            using (var closeBg = new SolidBrush(Color.FromArgb(10, 255, 0, 0)))
            {
                g.FillRectangle(closeBg, closeRect);
            }
            
            // Draw X icon
            using (var iconPen = new Pen(metrics.CaptionTextColor, 1))
            {
                int iconSize = 10;
                int iconX = closeRect.X + (closeRect.Width - iconSize) / 2;
                int iconY = closeRect.Y + (closeRect.Height - iconSize) / 2;
                g.DrawLine(iconPen, iconX, iconY, iconX + iconSize, iconY + iconSize);
                g.DrawLine(iconPen, iconX + iconSize, iconY, iconX, iconY + iconSize);
            }
            
            buttonX -= buttonWidth;
            
            // Maximize button - square
            var maxRect = new Rectangle(buttonX, 0, buttonWidth, buttonHeight);
            using (var iconPen = new Pen(metrics.CaptionTextColor, 1))
            {
                int iconSize = 10;
                int iconX = maxRect.X + (maxRect.Width - iconSize) / 2;
                int iconY = maxRect.Y + (maxRect.Height - iconSize) / 2;
                g.DrawRectangle(iconPen, iconX, iconY, iconSize, iconSize);
            }
            
            buttonX -= buttonWidth;
            
            // Minimize button - square
            var minRect = new Rectangle(buttonX, 0, buttonWidth, buttonHeight);
            using (var iconPen = new Pen(metrics.CaptionTextColor, 1))
            {
                int lineWidth = 10;
                int iconX = minRect.X + (minRect.Width - lineWidth) / 2;
                int iconY = minRect.Y + minRect.Height / 2;
                g.DrawLine(iconPen, iconX, iconY, iconX + lineWidth, iconY);
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // Windows11: Very thin, semi-transparent border
            using var pen = new Pen(Color.FromArgb(30, metrics.BorderColor), 1);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Windows11: Large soft shadow for floating effect
            return new ShadowEffect
            {
                Color = Color.FromArgb(18, 0, 0, 0),
                Blur = 18, // Large blur
                OffsetX = 0,
                OffsetY = 8,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(8); // Windows 11 signature rounding
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
            
            int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));
            owner._hits.Clear();
            
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
            
            int buttonWidth = metrics.ButtonWidth;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            
            layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("close", layout.CloseButtonRect, HitAreaType.Button);
            buttonX -= buttonWidth;
            
            layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
            buttonX -= buttonWidth;
            
            layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
            
            int iconX = metrics.IconLeftPadding;
            int iconY = (captionHeight - metrics.IconSize) / 2;
            layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
            }
            
            int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
            int titleWidth = layout.MinimizeButtonRect.Left - metrics.ButtonSpacing - titleX;
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
