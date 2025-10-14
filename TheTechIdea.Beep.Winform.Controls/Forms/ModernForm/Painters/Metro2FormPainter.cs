using System;
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

            // Paint Metro2 tile flip buttons (ENHANCED UNIQUE SKIN)
            PaintMetro2TileButtons(g, owner, captionRect, metrics);

            // Draw title text
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Metro2 tile buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Metro2 tile flip buttons (ENHANCED UNIQUE SKIN)
        /// Features: tile flip perspective, accent line highlights, modern Metro aesthetic
        /// </summary>
        private void PaintMetro2TileButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonSize = 20;
            int padding = (captionRect.Height - buttonSize) / 2;

            // Close button: Red tile
            PaintTileFlipButton(g, closeRect, Color.FromArgb(232, 17, 35), metrics.BorderColor, padding, buttonSize, "close");

            // Maximize button: Green tile
            PaintTileFlipButton(g, maxRect, Color.FromArgb(16, 124, 16), metrics.BorderColor, padding, buttonSize, "maximize");

            // Minimize button: Blue tile
            PaintTileFlipButton(g, minRect, Color.FromArgb(0, 120, 215), metrics.BorderColor, padding, buttonSize, "minimize");

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintTileFlipButton(g, styleRect, Color.FromArgb(135, 100, 184), metrics.BorderColor, padding, buttonSize, "style");
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintTileFlipButton(g, themeRect, Color.FromArgb(247, 99, 12), metrics.BorderColor, padding, buttonSize, "theme");
            }
        }

        private void PaintTileFlipButton(Graphics g, Rectangle buttonRect, Color baseColor, Color accentColor, int padding, int size, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Tile flip perspective effect (3D rotation illusion)
            DrawTileFlipPerspective(g, rect, baseColor);

            // Flat tile fill (Metro solid color)
            using (var tileBrush = new SolidBrush(baseColor))
            {
                g.FillRectangle(tileBrush, rect);
            }

            // Accent line highlights (Metro signature)
            using (var accentPen = new Pen(accentColor, 2))
            {
                // Top accent line
                g.DrawLine(accentPen, rect.X, rect.Y, rect.Right, rect.Y);
                
                // Left accent line
                g.DrawLine(accentPen, rect.X, rect.Y, rect.X, rect.Bottom);
            }

            // Subtle depth shadow on right/bottom
            using (var shadowPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
            {
                g.DrawLine(shadowPen, rect.Right, rect.Y + 1, rect.Right, rect.Bottom);
                g.DrawLine(shadowPen, rect.X + 1, rect.Bottom, rect.Right, rect.Bottom);
            }

            // Draw icon
            using (var iconPen = new Pen(Color.White, 1.8f))
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
                    case "style":
                        // Tile grid icon
                        g.DrawRectangle(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize / 2, iconSize / 2);
                        g.DrawRectangle(iconPen, iconCenterX, iconCenterY - iconSize / 2, iconSize / 2, iconSize / 2);
                        g.DrawRectangle(iconPen, iconCenterX - iconSize / 2, iconCenterY, iconSize / 2, iconSize / 2);
                        g.DrawRectangle(iconPen, iconCenterX, iconCenterY, iconSize / 2, iconSize / 2);
                        break;
                    case "theme":
                        // Flip icon (rotation arrows)
                        g.DrawArc(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize, 45, 270);
                        // Arrow head
                        g.DrawLine(iconPen, iconCenterX + iconSize / 3, iconCenterY - iconSize / 2,
                            iconCenterX + iconSize / 3 - 2, iconCenterY - iconSize / 2 + 3);
                        break;
                }
            }
        }

        /// <summary>
        /// Draw tile flip perspective effect (3D rotation illusion)
        /// </summary>
        private void DrawTileFlipPerspective(Graphics g, Rectangle rect, Color baseColor)
        {
            // Create perspective shadow/highlight layers
            var lightColor = ControlPaint.Light(baseColor, 0.3f);
            var darkColor = ControlPaint.Dark(baseColor, 0.2f);

            // Top-left highlight (front face)
            using (var highlightBrush = new LinearGradientBrush(
                new Rectangle(rect.X - 2, rect.Y - 2, 4, 4),
                Color.FromArgb(60, lightColor),
                Color.FromArgb(0, lightColor),
                45f))
            {
                g.FillRectangle(highlightBrush, rect.X - 2, rect.Y - 2, 3, 3);
            }

            // Bottom-right shadow (back face illusion)
            using (var shadowBrush = new LinearGradientBrush(
                new Rectangle(rect.Right - 2, rect.Bottom - 2, 4, 4),
                Color.FromArgb(0, darkColor),
                Color.FromArgb(40, darkColor),
                225f))
            {
                g.FillRectangle(shadowBrush, rect.Right, rect.Bottom, 2, 2);
            }
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
            
            // If caption bar is hidden, skip button layout
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
            
            var buttonSize = new Size(48, captionHeight);
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
