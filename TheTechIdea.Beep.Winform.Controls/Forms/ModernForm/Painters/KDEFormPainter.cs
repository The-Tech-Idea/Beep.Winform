using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// KDE Plasma Breeze theme style
    /// </summary>
    internal sealed class KDEFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.KDE, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // KDE Breeze subtle gradient (5% lighter at top)
            using (var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                ControlPaint.Light(metrics.BackgroundColor, 0.05f),
                metrics.BackgroundColor,
                90f))
            {
                g.FillRectangle(gradBrush, owner.ClientRectangle);
            }
            
            // KDE layered depth: subtle overlay on top third (alpha 8)
            var overlayRect = new Rectangle(0, 0, owner.ClientRectangle.Width, owner.ClientRectangle.Height / 3);
            using (var overlayBrush = new SolidBrush(Color.FromArgb(8, 255, 255, 255)))
            {
                g.FillRectangle(overlayBrush, overlayRect);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // KDE Breeze caption gradient (4% lighter at top)
            using (var capBrush = new LinearGradientBrush(
                captionRect,
                ControlPaint.Light(metrics.CaptionColor, 0.04f),
                metrics.CaptionColor,
                90f))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // KDE Breeze highlight line at top edge (signature feature)
            using (var highlightPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
            {
                g.DrawLine(highlightPen, captionRect.Left, captionRect.Top + 1, captionRect.Right, captionRect.Top + 1);
            }

            // Paint KDE minimal icon buttons (UNIQUE)
            PaintKDEButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }
        
        /// <summary>
        /// Paint KDE Breeze-style minimal icon buttons (UNIQUE)
        /// </summary>
        private void PaintKDEButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int buttonSize = 22; // Small square button area
            int buttonY = (captionRect.Height - buttonSize) / 2;
            
            // Close button: Subtle rectangle with X icon
            var closeBtn = new Rectangle(
                closeRect.X + (closeRect.Width - buttonSize) / 2,
                buttonY,
                buttonSize,
                buttonSize);
            
            // Hover state background (very subtle)
            using (var btnBrush = new SolidBrush(Color.FromArgb(25, 255, 255, 255)))
            {
                g.FillRectangle(btnBrush, closeBtn);
            }
            
            // X icon (thin lines, 9px)
            using (var iconPen = new Pen(metrics.CaptionTextColor, 1.2f))
            {
                int iconSize = 9;
                int cx = closeBtn.X + closeBtn.Width / 2;
                int cy = closeBtn.Y + closeBtn.Height / 2;
                g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
            }
            
            // Maximize button: Subtle rectangle with square icon
            var maxBtn = new Rectangle(
                maxRect.X + (maxRect.Width - buttonSize) / 2,
                buttonY,
                buttonSize,
                buttonSize);
            
            using (var btnBrush = new SolidBrush(Color.FromArgb(25, 255, 255, 255)))
            {
                g.FillRectangle(btnBrush, maxBtn);
            }
            
            // Square icon (8px)
            using (var iconPen = new Pen(metrics.CaptionTextColor, 1.2f))
            {
                int sqSize = 8;
                int mx = maxBtn.X + maxBtn.Width / 2;
                int my = maxBtn.Y + maxBtn.Height / 2;
                g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Subtle rectangle with horizontal line icon
            var minBtn = new Rectangle(
                minRect.X + (minRect.Width - buttonSize) / 2,
                buttonY,
                buttonSize,
                buttonSize);
            
            using (var btnBrush = new SolidBrush(Color.FromArgb(25, 255, 255, 255)))
            {
                g.FillRectangle(btnBrush, minBtn);
            }
            
            // Line icon (9px)
            using (var iconPen = new Pen(metrics.CaptionTextColor, 1.2f))
            {
                int lineSize = 9;
                int mnx = minBtn.X + minBtn.Width / 2;
                int mny = minBtn.Y + minBtn.Height / 2;
                g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // KDE thin border (1px)
            using var pen = new Pen(Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 40), metrics.BorderColor), 1);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // KDE Breeze shadow with subtle cool blue tint
            return new ShadowEffect
            {
                Color = Color.FromArgb(25, 10, 15, 25), // Cool blue-tinted shadow
                Blur = 12,
                OffsetY = 5,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(6); // KDE Breeze signature 6px corner
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra; // KDE Breeze ultra-smooth rendering
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
