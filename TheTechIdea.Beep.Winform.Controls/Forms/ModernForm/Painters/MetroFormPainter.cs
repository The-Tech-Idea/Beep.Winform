using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Metro (Windows 8/10) form painter with flat design and sharp edges.
    /// Features clean lines, flat colors, and modern minimalism.
    /// </summary>
    internal sealed class MetroFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Metro, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Flat caption with accent color
            using var brush = new SolidBrush(metrics.BorderColor);
            g.FillRectangle(brush, captionRect);

            // Paint Metro 3D tile buttons (ENHANCED UNIQUE SKIN)
            PaintMetro3DTileButtons(g, owner, captionRect, metrics);

            // Draw title text in white for contrast
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Metro 3D tile buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Metro 3D tile buttons (ENHANCED UNIQUE SKIN)
        /// Features: 3D tile perspective, bold borders, tile shadow depth
        /// </summary>
        private void PaintMetro3DTileButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonSize = 22;
            int padding = (captionRect.Height - buttonSize) / 2;

            // Close button: Red 3D tile
            Paint3DTileButton(g, closeRect, Color.FromArgb(232, 17, 35), padding, buttonSize, "close");

            // Maximize button: Green 3D tile
            Paint3DTileButton(g, maxRect, Color.FromArgb(16, 124, 16), padding, buttonSize, "maximize");

            // Minimize button: Blue 3D tile
            Paint3DTileButton(g, minRect, Color.FromArgb(0, 120, 215), padding, buttonSize, "minimize");

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                Paint3DTileButton(g, styleRect, Color.FromArgb(135, 100, 184), padding, buttonSize, "style");
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                Paint3DTileButton(g, themeRect, Color.FromArgb(247, 99, 12), padding, buttonSize, "theme");
            }
        }

        private void Paint3DTileButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int size, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);

            // NO anti-aliasing for sharp Metro edges
            g.SmoothingMode = SmoothingMode.None;

            // 3D perspective: draw back face (shadow layer)
            var backRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
            {
                g.FillRectangle(shadowBrush, backRect);
            }

            // 3D perspective: draw front face
            using (var tileBrush = new SolidBrush(baseColor))
            {
                g.FillRectangle(tileBrush, rect);
            }

            // Bold border (3px, Metro signature)
            using (var boldBorderPen = new Pen(ControlPaint.Dark(baseColor, 0.3f), 3))
            {
                g.DrawRectangle(boldBorderPen, rect);
            }

            // Inner highlight (top-left, 3D effect)
            using (var highlightPen = new Pen(Color.FromArgb(60, 255, 255, 255), 2))
            {
                g.DrawLine(highlightPen, rect.X + 2, rect.Y + 2, rect.Right - 2, rect.Y + 2);
                g.DrawLine(highlightPen, rect.X + 2, rect.Y + 2, rect.X + 2, rect.Bottom - 2);
            }

            // Inner shadow (bottom-right, 3D depth)
            using (var innerShadowPen = new Pen(Color.FromArgb(60, 0, 0, 0), 2))
            {
                g.DrawLine(innerShadowPen, rect.Right - 2, rect.Y + 4, rect.Right - 2, rect.Bottom - 2);
                g.DrawLine(innerShadowPen, rect.X + 4, rect.Bottom - 2, rect.Right - 2, rect.Bottom - 2);
            }

            // Draw icon (sharp, no anti-alias)
            using (var iconPen = new Pen(Color.White, 2f))
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
                        // Metro tile icon (4 squares)
                        int tileSize = 3;
                        g.FillRectangle(Brushes.White, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, tileSize, tileSize);
                        g.FillRectangle(Brushes.White, iconCenterX + 1, iconCenterY - iconSize / 2, tileSize, tileSize);
                        g.FillRectangle(Brushes.White, iconCenterX - iconSize / 2, iconCenterY + 1, tileSize, tileSize);
                        g.FillRectangle(Brushes.White, iconCenterX + 1, iconCenterY + 1, tileSize, tileSize);
                        break;
                    case "theme":
                        // Windows 8 logo icon
                        g.FillRectangle(Brushes.White, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize / 2 - 1, iconSize / 2 - 1);
                        g.FillRectangle(Brushes.White, iconCenterX + 1, iconCenterY - iconSize / 2, iconSize / 2 - 1, iconSize / 2 - 1);
                        g.FillRectangle(Brushes.White, iconCenterX - iconSize / 2, iconCenterY + 1, iconSize / 2 - 1, iconSize / 2 - 1);
                        g.FillRectangle(Brushes.White, iconCenterX + 1, iconCenterY + 1, iconSize / 2 - 1, iconSize / 2 - 1);
                        break;
                }
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
                Color = Color.FromArgb(30, 0, 0, 0),
                Blur = 5,
                OffsetY = 2,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(0); // Metro is flat/square
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.None; // Metro prefers sharp edges
        }

        public bool SupportsAnimations => false;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            PaintBackground(g, owner);
            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }
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
            
            var captionHeight = owner.Font.Height + 16;
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            var buttonSize = new Size(46, captionHeight);
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
            
            var iconSize = 16;
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
            var rect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.DrawRectangle(pen, rect);
        }
    }
}
