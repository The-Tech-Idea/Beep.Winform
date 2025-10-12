using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Minimal form painter with clean lines and subtle accent borders.
    /// Features a simple underline on the caption bar with primary color.
    /// </summary>
    internal sealed class MinimalFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Minimal, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            // Base fill
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Minimal style: whisper-light vertical highlight
            using (var grad = new System.Drawing.Drawing2D.LinearGradientBrush(
                owner.ClientRectangle,
                Color.FromArgb(12, 255, 255, 255), // slight top highlight
                Color.FromArgb(0, 255, 255, 255),
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillRectangle(grad, owner.ClientRectangle);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Draw primary color underline at bottom of caption
            using var line = new Pen(metrics.BorderColor, 2f);
            g.DrawLine(line, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);

            // Paint Minimal Japanese Zen circle buttons (ENHANCED UNIQUE SKIN)
            PaintZenCircleButtons(g, owner, captionRect, metrics);

            // Draw title text with 8px padding
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Zen circle buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Minimal Japanese Zen circle buttons (ENHANCED UNIQUE SKIN)
        /// Features: Japanese aesthetic circles, negative space emphasis, ultra-thin lines
        /// </summary>
        private void PaintZenCircleButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonSize = 18;
            int padding = (captionRect.Height - buttonSize) / 2;

            // Close button: Red Zen circle
            PaintZenButton(g, closeRect, Color.FromArgb(200, 80, 80), metrics.BorderColor, padding, buttonSize, "close");

            // Maximize button: Green Zen circle
            PaintZenButton(g, maxRect, Color.FromArgb(100, 160, 100), metrics.BorderColor, padding, buttonSize, "maximize");

            // Minimize button: Blue Zen circle
            PaintZenButton(g, minRect, Color.FromArgb(100, 140, 180), metrics.BorderColor, padding, buttonSize, "minimize");

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintZenButton(g, styleRect, Color.FromArgb(140, 120, 160), metrics.BorderColor, padding, buttonSize, "style");
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintZenButton(g, themeRect, Color.FromArgb(180, 140, 100), metrics.BorderColor, padding, buttonSize, "theme");
            }
        }

        private void PaintZenButton(Graphics g, Rectangle buttonRect, Color baseColor, Color accentColor, int padding, int size, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            int radius = size / 2;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Negative space: draw only the outer circle outline (Zen minimalism)
            using (var zenPen = new Pen(Color.FromArgb(40, baseColor), 1))
            {
                g.DrawEllipse(zenPen, centerX - radius, centerY - radius, size, size);
            }

            // Ultra-thin inner circle (Japanese calligraphy aesthetic)
            using (var innerPen = new Pen(Color.FromArgb(80, baseColor), 0.5f))
            {
                int innerRadius = radius - 2;
                g.DrawEllipse(innerPen, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
            }

            // Accent point (enso circle - incomplete circle, Zen philosophy)
            DrawEnsoAccent(g, centerX, centerY, radius, accentColor);

            // Calm monochromatic fill (very subtle, almost transparent)
            using (var fillBrush = new SolidBrush(Color.FromArgb(15, baseColor)))
            {
                g.FillEllipse(fillBrush, centerX - radius, centerY - radius, size, size);
            }

            // Draw icon (minimalist, thin lines)
            using (var iconPen = new Pen(Color.FromArgb(180, baseColor), 1f))
            {
                int iconSize = 6;

                switch (buttonType)
                {
                    case "close":
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY - iconSize / 2,
                            centerX + iconSize / 2, centerY + iconSize / 2);
                        g.DrawLine(iconPen, centerX + iconSize / 2, centerY - iconSize / 2,
                            centerX - iconSize / 2, centerY + iconSize / 2);
                        break;
                    case "maximize":
                        g.DrawRectangle(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        break;
                    case "minimize":
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY, centerX + iconSize / 2, centerY);
                        break;
                    case "style":
                        // Brush icon (Japanese calligraphy)
                        g.DrawLine(iconPen, centerX, centerY - iconSize / 2, centerX, centerY + iconSize / 2);
                        g.DrawLine(iconPen, centerX - iconSize / 3, centerY - iconSize / 4,
                            centerX + iconSize / 3, centerY + iconSize / 4);
                        break;
                    case "theme":
                        // Yin-yang icon (balance, Zen)
                        g.DrawEllipse(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        g.DrawArc(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize / 2, iconSize, 90, 180);
                        g.DrawArc(iconPen, centerX, centerY - iconSize / 2, iconSize / 2, iconSize, 270, 180);
                        break;
                }
            }
        }

        /// <summary>
        /// Draw Enso accent (incomplete circle - Zen Buddhist symbol)
        /// Represents elegance, strength through imperfection
        /// </summary>
        private void DrawEnsoAccent(Graphics g, int centerX, int centerY, int radius, Color accentColor)
        {
            using (var ensoPen = new Pen(Color.FromArgb(60, accentColor), 1))
            {
                // Draw 300° arc (leaving 60° gap for Zen imperfection)
                g.DrawArc(ensoPen, centerX - radius, centerY - radius, radius * 2, radius * 2, 30, 300);
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(20, 0, 0, 0),
                Blur = 8,
                OffsetY = 2,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(6);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High;
        }

        public bool SupportsAnimations => false;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // Orchestrated: base background, clipped effects (none), borders, caption
            var originalClip = g.Clip;
            var shadow = GetShadowEffect(owner);
            var radius = GetCornerRadius(owner);

            // 1) Shadow
            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow, radius);
            }

            // 2) Base background (entire form)
            PaintBackground(g, owner);

            // 3) Background effects with clipping (Minimal has none but keep structure)
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            // Always paint over entire form background since this runs in OnPaintBackground
            g.Clip = new Region(path);

            PaintBackgroundEffects(g, owner, rect);

            // 4) Reset clip
            g.Clip = originalClip;

            // 5) Borders and caption without clipping
            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }

            g.Clip = originalClip;
        }

        // Minimal has no special background effects, keep for pattern consistency
        private void PaintBackgroundEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // No-op for Minimal style
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
            
            // Calculate caption height based on font and padding
            var captionHeight = owner.Font.Height + 16; // 8px padding top and bottom
            
            // Set caption rectangle
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            
            // Set content rectangle (below caption)
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            // Calculate button positions (right-aligned in caption)
            var buttonSize = new Size(32, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;
            
            // Close button
            layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            // Maximize button
            layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            // Minimize button
            layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Custom action button (if theme/style not shown)
            if (!owner.ShowThemeButton && !owner.ShowStyleButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
            }
            
            // Icon and title areas (left side of caption)
            var iconSize = 16;
            var iconPadding = 8;
            layout.IconRect = new Rectangle(iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            var titleX = iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            owner.CurrentLayout = layout;
        }

        // Painter-owned non-client border rendering for Minimal style
        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);

            // Minimal border: crisp stroke along the outer rounded shape, thickness as provided
            var outerRect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outerRect, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
    }
}
