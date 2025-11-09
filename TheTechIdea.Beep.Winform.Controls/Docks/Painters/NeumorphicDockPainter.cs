using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Neumorphic (Soft UI) dock painter
    /// Features:
    /// - Soft embossed/debossed appearance
    /// - Dual shadows (light and dark)
    /// - Same-color background and elements
    /// - Subtle depth with shadows
    /// - Tactile, soft aesthetic
    /// </summary>
    public class NeumorphicDockPainter : DockPainterBase
    {
        private const int ShadowDistance = 8;
        private const int ShadowBlur = 12;

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Neumorphic base color (typically light gray)
            var baseColor = GetColor(
                config.BackgroundColor,
                theme?.BackgroundColor ?? Color.FromArgb(230, 230, 235),
                1f
            );

            // Paint neumorphic container with dual shadows
            using (var path = CreateRoundedPath(bounds, config.CornerRadius))
            {
                // Dark shadow (bottom-right)
                PaintNeumorphicShadow(g, bounds, config.CornerRadius, true);

                // Light shadow (top-left)
                PaintNeumorphicShadow(g, bounds, config.CornerRadius, false);

                // Main background
                using (var brush = new SolidBrush(baseColor))
                {
                    g.FillPath(brush, path);
                }

                // Subtle inner shadow for depth
                PaintInnerShadow(g, path, bounds, config);
            }
        }

        private void PaintNeumorphicShadow(Graphics g, Rectangle bounds, int cornerRadius, bool isDark)
        {
            var shadowBounds = bounds;
            shadowBounds.Inflate(ShadowBlur, ShadowBlur);

            if (isDark)
            {
                // Dark shadow (bottom-right)
                shadowBounds.Offset(ShadowDistance, ShadowDistance);
            }
            else
            {
                // Light shadow (top-left)
                shadowBounds.Offset(-ShadowDistance, -ShadowDistance);
            }

            using (var shadowPath = CreateRoundedPath(shadowBounds, cornerRadius))
            {
                for (int i = ShadowBlur; i > 0; i--)
                {
                    int alpha = (int)(60 * (i / (float)ShadowBlur));
                    Color shadowColor = isDark
                        ? Color.FromArgb(alpha, 0, 0, 0)          // Dark shadow
                        : Color.FromArgb(alpha, 255, 255, 255);   // Light shadow

                    using (var pen = new Pen(shadowColor, i / 3f))
                    {
                        g.DrawPath(pen, shadowPath);
                    }
                }
            }
        }

        private void PaintInnerShadow(Graphics g, GraphicsPath path, Rectangle bounds, DockConfig config)
        {
            // Subtle inner shadow for depth
            var innerBounds = bounds;
            innerBounds.Inflate(-2, -2);

            using (var innerPath = CreateRoundedPath(innerBounds, config.CornerRadius - 2))
            using (var brush = new LinearGradientBrush(
                bounds,
                Color.FromArgb(10, Color.Black),
                Color.FromArgb(5, Color.White),
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(brush, innerPath);
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = itemState.Bounds;

            // Neumorphic effect on hover/selection
            if (itemState.IsHovered || itemState.IsSelected)
            {
                PaintNeumorphicItem(g, bounds, itemState, config, theme);
            }

            // Paint icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, itemState.CurrentOpacity);
            }
        }

        private void PaintNeumorphicItem(Graphics g, Rectangle bounds, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            var itemBounds = bounds;
            itemBounds.Inflate(6, 6);
            int itemRadius = 16;

            var baseColor = GetColor(
                config.BackgroundColor,
                theme?.BackgroundColor ?? Color.FromArgb(230, 230, 235),
                1f
            );

            if (itemState.IsDragging)
            {
                // Pressed - inset (embossed) effect
                PaintPressedNeumorphic(g, itemBounds, itemRadius, baseColor);
            }
            else
            {
                // Hovered/Selected - raised (debossed) effect
                PaintRaisedNeumorphic(g, itemBounds, itemRadius, baseColor);
            }
        }

        private void PaintPressedNeumorphic(Graphics g, Rectangle bounds, int radius, Color baseColor)
        {
            using (var path = CreateRoundedPath(bounds, radius))
            {
                // Inner shadow effect for pressed state
                using (var brush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(20, Color.Black),
                    Color.FromArgb(5, Color.White),
                    LinearGradientMode.ForwardDiagonal))
                {
                    g.FillPath(brush, path);
                }

                // Slightly darker fill
                var darkerColor = Color.FromArgb(
                    Math.Max(0, baseColor.R - 10),
                    Math.Max(0, baseColor.G - 10),
                    Math.Max(0, baseColor.B - 10)
                );

                using (var brush = new SolidBrush(darkerColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void PaintRaisedNeumorphic(Graphics g, Rectangle bounds, int radius, Color baseColor)
        {
            // Mini dual shadows for item
            var shadowOffset = 3;

            // Dark shadow
            var darkShadowBounds = bounds;
            darkShadowBounds.Offset(shadowOffset, shadowOffset);
            using (var path = CreateRoundedPath(darkShadowBounds, radius))
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                {
                    g.FillPath(brush, path);
                }
            }

            // Light shadow
            var lightShadowBounds = bounds;
            lightShadowBounds.Offset(-shadowOffset, -shadowOffset);
            using (var path = CreateRoundedPath(lightShadowBounds, radius))
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, Color.White)))
                {
                    g.FillPath(brush, path);
                }
            }

            // Item background
            using (var path = CreateRoundedPath(bounds, radius))
            using (var brush = new SolidBrush(baseColor))
            {
                g.FillPath(brush, path);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (!itemState.IsRunning && !itemState.IsSelected)
            {
                return;
            }

            var bounds = itemState.Bounds;

            // Neumorphic dot with subtle shadow
            int dotSize = 6;
            var dotRect = new Rectangle(
                bounds.X + bounds.Width / 2 - dotSize / 2,
                bounds.Bottom + 10,
                dotSize,
                dotSize
            );

            // Shadow for dot
            var shadowRect = dotRect;
            shadowRect.Offset(1, 1);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
            {
                g.FillEllipse(shadowBrush, shadowRect);
            }

            // Dot
            var dotColor = theme?.AccentColor ?? Color.FromArgb(150, 150, 160);
            using (var brush = new SolidBrush(dotColor))
            {
                g.FillEllipse(brush, dotRect);
            }
        }
    }
}
