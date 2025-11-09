using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Windows 11 style dock painter
    /// Features:
    /// - Centered layout with rounded corners
    /// - Acrylic/mica background effect
    /// - Subtle elevation shadows
    /// - Modern Fluent Design principles
    /// </summary>
    public class Windows11DockPainter : DockPainterBase
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Acrylic background
            using (var path = CreateRoundedPath(bounds, config.CornerRadius))
            {
                // Base acrylic layer
                var acrylicColor = GetColor(
                    config.BackgroundColor,
                    theme?.SurfaceColor ?? Color.FromArgb(245, 245, 245),
                    config.BackgroundOpacity
                );

                using (var brush = new SolidBrush(acrylicColor))
                {
                    g.FillPath(brush, path);
                }

                // Noise texture simulation (subtle)
                PaintNoiseTexture(g, bounds, path);

                // Subtle top highlight
                PaintTopHighlight(g, bounds, config);

                // Border
                if (config.ShowBorder)
                {
                    var borderColor = GetColor(
                        config.BorderColor,
                        theme?.BorderColor ?? Color.FromArgb(220, 220, 220),
                        0.5f
                    );

                    using (var pen = new Pen(borderColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Shadow
            if (config.ShowShadow)
            {
                PaintElevationShadow(g, bounds, config);
            }
        }

        private void PaintNoiseTexture(Graphics g, Rectangle bounds, GraphicsPath clipPath)
        {
            // Simulate subtle noise/grain for acrylic effect
            g.SetClip(clipPath);

            var random = new Random(42); // Fixed seed for consistency
            using (var noiseBrush = new SolidBrush(Color.FromArgb(5, Color.White)))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = bounds.X + random.Next(bounds.Width);
                    int y = bounds.Y + random.Next(bounds.Height);
                    g.FillRectangle(noiseBrush, x, y, 1, 1);
                }
            }

            g.ResetClip();
        }

        private void PaintTopHighlight(Graphics g, Rectangle bounds, DockConfig config)
        {
            var highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, 2);
            using (var highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(30, Color.White),
                Color.FromArgb(0, Color.White),
                90f))
            {
                g.FillRectangle(highlightBrush, highlightRect);
            }
        }

        private void PaintElevationShadow(Graphics g, Rectangle bounds, DockConfig config)
        {
            var shadowBounds = bounds;
            shadowBounds.Inflate(3, 3);
            shadowBounds.Offset(0, 2);

            using (var shadowPath = CreateRoundedPath(shadowBounds, config.CornerRadius))
            {
                // Soft, diffuse shadow
                for (int i = 8; i > 0; i--)
                {
                    int alpha = (int)(25 * (i / 8f));
                    using (var pen = new Pen(Color.FromArgb(alpha, Color.Black), i / 2f))
                    {
                        g.DrawPath(pen, shadowPath);
                    }
                }
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = itemState.Bounds;

            // Hover/selection background
            if (itemState.IsHovered || itemState.IsSelected)
            {
                PaintItemBackground(g, bounds, itemState, config, theme);
            }

            // Icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, itemState.CurrentOpacity);
            }

            // Badge
            if (config.ShowBadges && itemState.BadgeCount > 0)
            {
                PaintBadge(g, bounds, itemState.BadgeCount, theme);
            }
        }

        private void PaintItemBackground(Graphics g, Rectangle bounds, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            var bgBounds = bounds;
            bgBounds.Inflate(4, 4);

            using (var path = CreateRoundedPath(bgBounds, 8))
            {
                Color bgColor;

                if (itemState.IsSelected)
                {
                    bgColor = GetColor(
                        config.SelectedColor,
                        theme?.AccentColor ?? Color.FromArgb(0, 120, 212),
                        0.2f
                    );
                }
                else
                {
                    bgColor = GetColor(
                        config.HoverColor,
                        theme?.ButtonHoverBackColor ?? Color.FromArgb(240, 240, 240),
                        0.5f
                    );
                }

                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void PaintBadge(Graphics g, Rectangle itemBounds, int count, IBeepTheme theme)
        {
            string text = count > 99 ? "99+" : count.ToString();
            int badgeSize = 18;

            var badgeRect = new Rectangle(
                itemBounds.Right - badgeSize + 2,
                itemBounds.Top - 2,
                badgeSize,
                badgeSize
            );

            // Badge background
            using (var bgBrush = new SolidBrush(theme?.ErrorColor ?? Color.FromArgb(196, 43, 28)))
            {
                g.FillEllipse(bgBrush, badgeRect);
            }

            // Badge text
            using (var font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, font, textBrush, badgeRect, sf);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (config.IndicatorStyle == DockIndicatorStyle.None)
                return;

            var bounds = itemState.Bounds;

            if (!itemState.IsRunning && !itemState.IsSelected)
                return;

            // Windows 11 uses small line indicator below icons
            int lineWidth = 4;
            int lineHeight = 3;
            int lineY = bounds.Bottom + 6;

            var lineRect = new Rectangle(
                bounds.X + bounds.Width / 2 - lineWidth / 2,
                lineY,
                lineWidth,
                lineHeight
            );

            var color = itemState.IsSelected
                ? (theme?.AccentColor ?? Color.FromArgb(0, 120, 212))
                : Color.FromArgb(150, 150, 150);

            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, lineRect);
            }
        }
    }
}
