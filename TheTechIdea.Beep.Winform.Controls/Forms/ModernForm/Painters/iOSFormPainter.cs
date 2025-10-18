using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Apple iOS modern rounded style
    /// </summary>
    internal sealed class iOSFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.iOS, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // iOS: Smooth vertical gradient with vibrancy overlay
            var topColor = ControlPaint.Light(metrics.BackgroundColor, 0.08f);
            using (var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                topColor,
                metrics.BackgroundColor,
                LinearGradientMode.Vertical))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(gradBrush, path);
            }
            
            // Vibrancy effect - subtle white overlay
            var vibrancyColor = Color.FromArgb(12, 255, 255, 255);
            var topThird = new Rectangle(owner.ClientRectangle.X, owner.ClientRectangle.Y,
                owner.ClientRectangle.Width, owner.ClientRectangle.Height / 3);
            var topPath = CreateRoundedRectanglePath(topThird, new CornerRadius(radius.TopLeft, radius.TopRight, 0, 0));
            using (var vibrancyBrush = new SolidBrush(vibrancyColor))
            {
                g.FillPath(vibrancyBrush, topPath);
            }
            topPath.Dispose();
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            
            // iOS: Semi-transparent blurred caption bar
            var captionPath = new GraphicsPath();
            int tl = radius.TopLeft;
            int tr = radius.TopRight;
            
            if (tl > 0) captionPath.AddArc(captionRect.X, captionRect.Y, tl * 2, tl * 2, 180, 90);
            else captionPath.AddLine(captionRect.X, captionRect.Y, captionRect.X, captionRect.Y);
            
            if (tr > 0) captionPath.AddArc(captionRect.Right - tr * 2, captionRect.Y, tr * 2, tr * 2, 270, 90);
            else captionPath.AddLine(captionRect.Right, captionRect.Y, captionRect.Right, captionRect.Y);
            
            captionPath.AddLine(captionRect.Right, captionRect.Bottom, captionRect.X, captionRect.Bottom);
            captionPath.CloseFigure();
            
            // Translucent caption with blur effect
            var transCaptionColor = Color.FromArgb(230, metrics.CaptionColor);
            using (var capBrush = new SolidBrush(transCaptionColor))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(capBrush, captionPath);
            }
            
            // Frosted effect overlay
            using (var hatchBrush = new HatchBrush(HatchStyle.DottedDiamond,
                Color.FromArgb(8, 255, 255, 255),
                Color.Transparent))
            {
                g.FillPath(hatchBrush, captionPath);
            }
            
            // iOS-style separator line (1px hairline)
            using (var separatorPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
            {
                g.DrawLine(separatorPen, 0, captionRect.Bottom - 1, captionRect.Width, captionRect.Bottom - 1);
            }
            
            captionPath.Dispose();

            // iOS: Centered title (different from others!)
            var textRect = owner.CurrentLayout.TitleRect;
            var textSize = TextUtils.MeasureText(g,owner.Text ?? string.Empty, owner.Font);
            var centeredX = (owner.ClientSize.Width - (int)textSize.Width) / 2;
            var centeredRect = new Rectangle(centeredX, textRect.Y, (int)textSize.Width, textRect.Height);
            
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, centeredRect, metrics.CaptionTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // iOS: Paint circular traffic light buttons (distinct!)
            PaintiOSButtons(g, owner, captionRect);

            // Paint icon, theme/style buttons (iOS only paints traffic lights, not system buttons)
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
            
            if (owner.ShowThemeButton)
                owner._themeButton?.OnPaint?.Invoke(g, owner.CurrentLayout.ThemeButtonRect);
            
            if (owner.ShowStyleButton)
                owner._styleButton?.OnPaint?.Invoke(g, owner.CurrentLayout.StyleButtonRect);
            
            if (!owner.ShowThemeButton && !owner.ShowStyleButton)
                owner._customActionButton?.OnPaint?.Invoke(g, owner.CurrentLayout.CustomActionButtonRect);
        }
        
        private void PaintiOSButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            // iOS: Circular colored buttons (red, yellow, green) - UNIQUE to iOS!
            int buttonSize = 12;
            int buttonY = (captionRect.Height - buttonSize) / 2;
            int buttonSpacing = 8;
            int startX = 10;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Close button (red circle)
            var closeRect = new Rectangle(startX, buttonY, buttonSize, buttonSize);
            using (var closeBrush = new SolidBrush(Color.FromArgb(255, 95, 86)))
            {
                g.FillEllipse(closeBrush, closeRect);
            }
            
            // Minimize button (yellow circle)
            var minRect = new Rectangle(startX + buttonSize + buttonSpacing, buttonY, buttonSize, buttonSize);
            using (var minBrush = new SolidBrush(Color.FromArgb(255, 189, 46)))
            {
                g.FillEllipse(minBrush, minRect);
            }
            
            // Maximize button (green circle)
            var maxRect = new Rectangle(startX + (buttonSize + buttonSpacing) * 2, buttonY, buttonSize, buttonSize);
            using (var maxBrush = new SolidBrush(Color.FromArgb(39, 201, 63)))
            {
                g.FillEllipse(maxBrush, maxRect);
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
             using var path = owner.BorderShape;
            // iOS: Thin or borderless with ultra smooth edges
            using var pen = new Pen(Color.FromArgb(40, metrics.BorderColor), 1);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // iOS: Soft, floating shadow
            return new ShadowEffect
            {
                Color = Color.FromArgb(20, 0, 0, 0),
                Blur = 14, // Soft blur
                OffsetX = 0,
                OffsetY = 6,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(14); // Signature iOS rounding
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
            
            owner._hits.Clear();
            
            // If caption bar is hidden, skip button layout
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
            
            // iOS: Traffic light buttons on LEFT (red, yellow, green circles)
            int buttonSize = 12; // Small circular buttons
            int buttonY = (captionHeight - buttonSize) / 2;
            int buttonSpacing = 8;
            int leftX = 10;
            
            // Close button (red circle, leftmost)
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(leftX, buttonY, buttonSize, buttonSize);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                leftX += buttonSize + buttonSpacing;
            }
            
            // Minimize/Maximize buttons (yellow/green circles)
            if (owner.ShowMinMaxButtons)
            {
                layout.MinimizeButtonRect = new Rectangle(leftX, buttonY, buttonSize, buttonSize);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                leftX += buttonSize + buttonSpacing;
                
                layout.MaximizeButtonRect = new Rectangle(leftX, buttonY, buttonSize, buttonSize);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                leftX += buttonSize + buttonSpacing;
            }
            
            // RIGHT side: Theme/Style buttons (standard Windows-style placement)
            int buttonWidth = 32; // Larger for theme/style buttons
            int rightX = owner.ClientSize.Width - buttonWidth;
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(rightX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
                rightX -= buttonWidth;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(rightX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                rightX -= buttonWidth;
            }
            
            // Custom action button (fallback if theme/style not shown)
            if (!owner.ShowThemeButton && !owner.ShowStyleButton)
            {
                layout.CustomActionButtonRect = new Rectangle(rightX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                rightX -= buttonWidth;
            }
            
            // Icon positioning (after traffic lights)
            int iconX = leftX + 8;
            int iconY = (captionHeight - metrics.IconSize) / 2;
            layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
            }
            
            // Title area: between icon and right-side buttons (centered for iOS style)
            int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
            int titleWidth = rightX - titleX - metrics.ButtonSpacing;
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
