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

            // Paint KDE Breeze plasma wave buttons (ENHANCED UNIQUE SKIN)
            PaintKDEPlasmaButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom KDE plasma buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint KDE Breeze plasma wave buttons (ENHANCED UNIQUE SKIN)
        /// Features: plasma wave pattern, Breeze rounded squares, smooth gradients
        /// </summary>
        private void PaintKDEPlasmaButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int buttonSize = 20;
            int padding = (captionRect.Height - buttonSize) / 2;
            
            // Close button: Red with plasma wave
            PaintPlasmaButton(g, closeRect, Color.FromArgb(237, 21, 21), padding, buttonSize, "close", metrics);
            
            // Maximize button: Green with plasma wave
            PaintPlasmaButton(g, maxRect, Color.FromArgb(24, 218, 24), padding, buttonSize, "maximize", metrics);
            
            // Minimize button: Blue with plasma wave
            PaintPlasmaButton(g, minRect, Color.FromArgb(61, 174, 233), padding, buttonSize, "minimize", metrics);
            
            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintPlasmaButton(g, styleRect, Color.FromArgb(147, 115, 203), padding, buttonSize, "style", metrics);
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintPlasmaButton(g, themeRect, Color.FromArgb(246, 116, 0), padding, buttonSize, "theme", metrics);
            }
        }

        private void PaintPlasmaButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int size, string buttonType, FormPainterMetrics metrics)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Plasma wave pattern background
            DrawPlasmaWave(g, rect, baseColor);

            // Breeze rounded square shape (4px corner radius)
            using (var path = CreateRoundedSquarePath(rect, 4))
            {
                // Breeze gradient fill (vertical, light to dark)
                using (var gradientBrush = new LinearGradientBrush(rect,
                    ControlPaint.Light(baseColor, 0.15f),
                    ControlPaint.Dark(baseColor, 0.05f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradientBrush, path);
                }

                // Smooth border (1px, slightly darker)
                using (var borderPen = new Pen(ControlPaint.Dark(baseColor, 0.2f), 1))
                {
                    g.DrawPath(borderPen, path);
                }

                // KDE Breeze top highlight (signature feature)
                using (var highlightPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1))
                {
                    g.DrawLine(highlightPen, rect.X + 3, rect.Y + 2, rect.Right - 3, rect.Y + 2);
                }
            }

            // Draw icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 7;
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
                    case "style":
                        // Palette icon
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize);
                        g.FillEllipse(Brushes.White, iconCenterX - 1, iconCenterY - 1, 2, 2);
                        break;
                    case "theme":
                        // Gear icon
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 3, iconCenterY - iconSize / 3, 
                            iconSize * 2 / 3, iconSize * 2 / 3);
                        for (int i = 0; i < 6; i++)
                        {
                            double angle = Math.PI * 2 * i / 6;
                            int x = iconCenterX + (int)(Math.Cos(angle) * iconSize / 2);
                            int y = iconCenterY + (int)(Math.Sin(angle) * iconSize / 2);
                            g.FillRectangle(Brushes.White, x - 1, y - 1, 2, 2);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Draw KDE Plasma wave pattern (animated energy effect)
        /// </summary>
        private void DrawPlasmaWave(Graphics g, Rectangle rect, Color baseColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw 3 sine wave layers for plasma effect
            using (var plasmaPen = new Pen(Color.FromArgb(30, baseColor), 1))
            {
                for (int i = 0; i < 3; i++)
                {
                    var points = new PointF[15];
                    for (int j = 0; j < 15; j++)
                    {
                        float x = rect.X + (rect.Width * j / 14f);
                        float y = rect.Y + rect.Height / 2 + (float)(Math.Sin(j * 0.9 + i * 1.2) * 4);
                        points[j] = new PointF(x, y);
                    }
                    g.DrawCurve(plasmaPen, points, 0.5f);
                }
            }
        }

        /// <summary>
        /// Create rounded square path (KDE Breeze button shape)
        /// </summary>
        private GraphicsPath CreateRoundedSquarePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0 || rect.Width <= radius * 2 || rect.Height <= radius * 2)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            
            return path;
        }
        
        /// <summary>
        /// Paint KDE Breeze-style minimal icon buttons (ORIGINAL - KEPT FOR COMPATIBILITY)
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
            
            owner._hits.Clear();
            
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
                owner._hits.Register("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.Register("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.Register("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Custom action button (fallback)
            if (!owner.ShowThemeButton && !owner.ShowStyleButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
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
