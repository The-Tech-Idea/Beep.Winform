using System;
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

            // GNOME-Style gradient headerbar
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

            // Paint GNOME Adwaita pill buttons (ENHANCED UNIQUE SKIN)
            PaintAdwaitaPillButtons(g, owner, captionRect, metrics);

            // Draw title text (centered for GNOME Style)
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom GNOME pill buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint GNOME Adwaita pill buttons (ENHANCED UNIQUE SKIN)
        /// Features: pill-shaped buttons, gradient mesh, GNOME design language
        /// </summary>
        private void PaintAdwaitaPillButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonHeight = 22;
            int buttonWidth = 28;
            int padding = (captionRect.Height - buttonHeight) / 2;

            // Close button: Red pill
            PaintPillButton(g, closeRect, Color.FromArgb(246, 97, 81), padding, buttonWidth, buttonHeight, "close");

            // Maximize button: Green pill
            PaintPillButton(g, maxRect, Color.FromArgb(51, 209, 122), padding, buttonWidth, buttonHeight, "maximize");

            // Minimize button: Blue pill
            PaintPillButton(g, minRect, Color.FromArgb(53, 132, 228), padding, buttonWidth, buttonHeight, "minimize");

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintPillButton(g, styleRect, Color.FromArgb(145, 65, 172), padding, buttonWidth, buttonHeight, "Style");
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintPillButton(g, themeRect, Color.FromArgb(230, 97, 0), padding, buttonWidth, buttonHeight, "theme");
            }
        }

        private void PaintPillButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int width, int height, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - width / 2, centerY - height / 2, width, height);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Gradient mesh overlay (subtle GNOME effect)
            DrawGradientMesh(g, rect);

            // Create pill shape (fully rounded ends)
            using (var pillPath = CreatePillPath(rect))
            {
                // GNOME Adwaita gradient fill (vertical, subtle)
                using (var gradientBrush = new LinearGradientBrush(rect,
                    ControlPaint.Light(baseColor, 0.10f),
                    baseColor,
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradientBrush, pillPath);
                }

                // Soft shadow inside (top edge)
                using (var shadowPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    g.DrawLine(shadowPen, rect.X + height / 2, rect.Y + 1, 
                        rect.Right - height / 2, rect.Y + 1);
                }

                // Highlight on bottom edge (GNOME signature)
                using (var highlightPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
                {
                    g.DrawLine(highlightPen, rect.X + height / 2, rect.Bottom - 2,
                        rect.Right - height / 2, rect.Bottom - 2);
                }

                // Pill border (subtle, 1px)
                using (var borderPen = new Pen(ControlPaint.Dark(baseColor, 0.2f), 1))
                {
                    g.DrawPath(borderPen, pillPath);
                }
            }

            // Draw icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 8;
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
                        // Grid icon (GNOME Style)
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                int x = iconCenterX - iconSize / 3 + i * iconSize / 2;
                                int y = iconCenterY - iconSize / 3 + j * iconSize / 2;
                                g.FillRectangle(Brushes.White, x, y, 3, 3);
                            }
                        }
                        break;
                    case "theme":
                        // Contrast icon (GNOME accessibility)
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize);
                        using (var fillBrush = new SolidBrush(Color.White))
                        {
                            g.FillPie(fillBrush, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, 
                                iconSize, iconSize, 270, 180);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Draw gradient mesh overlay (GNOME Adwaita effect)
        /// </summary>
        private void DrawGradientMesh(Graphics g, Rectangle rect)
        {
            // Subtle diagonal gradient mesh
            using (var meshBrush = new LinearGradientBrush(
                new Rectangle(rect.X - 10, rect.Y - 10, rect.Width + 20, rect.Height + 20),
                Color.FromArgb(15, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                45f))
            {
                g.FillRectangle(meshBrush, rect);
            }
        }

        /// <summary>
        /// Create pill shape path (fully rounded ends)
        /// </summary>
        private GraphicsPath CreatePillPath(Rectangle rect)
        {
            var path = new GraphicsPath();
            int radius = rect.Height / 2;

            // Left semicircle
            path.AddArc(rect.X, rect.Y, rect.Height, rect.Height, 90, 180);
            
            // Top line
            path.AddLine(rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            
            // Right semicircle
            path.AddArc(rect.Right - rect.Height, rect.Y, rect.Height, rect.Height, 270, 180);
            
            // Bottom line
            path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius, rect.Bottom);
            
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var cornerRadius = GetCornerRadius(owner);
            
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth))
            {
                Alignment = PenAlignment.Inset
            };
            
             using var path = owner.BorderShape;
            
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
            
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            var captionHeight = owner.Font.Height + 20;
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            var buttonSize = new Size(44, captionHeight);
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
