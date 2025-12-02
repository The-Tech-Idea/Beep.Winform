using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Nord theme inspired cool-toned dock painter
    /// Uses the popular Nord color palette
    /// </summary>
    public class NordDockPainter : DockPainterBase
    {
        // Nord color palette
        private static class NordColors
        {
            // Polar Night (dark backgrounds)
            public static readonly Color Nord0 = Color.FromArgb(46, 52, 64);     // #2E3440
            public static readonly Color Nord1 = Color.FromArgb(59, 66, 82);     // #3B4252
            public static readonly Color Nord2 = Color.FromArgb(67, 76, 94);     // #434C5E
            public static readonly Color Nord3 = Color.FromArgb(76, 86, 106);    // #4C566A

            // Snow Storm (light foregrounds)
            public static readonly Color Nord4 = Color.FromArgb(216, 222, 233);  // #D8DEE9
            public static readonly Color Nord5 = Color.FromArgb(229, 233, 240);  // #E5E9F0
            public static readonly Color Nord6 = Color.FromArgb(236, 239, 244);  // #ECEFF4

            // Frost (bright blues/cyans)
            public static readonly Color Nord7 = Color.FromArgb(143, 188, 187);  // #8FBCBB
            public static readonly Color Nord8 = Color.FromArgb(136, 192, 208);  // #88C0D0
            public static readonly Color Nord9 = Color.FromArgb(129, 161, 193);  // #81A1C1
            public static readonly Color Nord10 = Color.FromArgb(94, 129, 172);  // #5E81AC

            // Aurora (accent colors)
            public static readonly Color Nord11 = Color.FromArgb(191, 97, 106);  // #BF616A - Red
            public static readonly Color Nord12 = Color.FromArgb(208, 135, 112); // #D08770 - Orange
            public static readonly Color Nord13 = Color.FromArgb(235, 203, 139); // #EBCB8B - Yellow
            public static readonly Color Nord14 = Color.FromArgb(163, 190, 140); // #A3BE8C - Green
            public static readonly Color Nord15 = Color.FromArgb(180, 142, 173); // #B48EAD - Purple
        }

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Nord uses Polar Night colors for backgrounds
            var bgColor1 = Color.FromArgb((int)(config.BackgroundOpacity * 255), NordColors.Nord1.R, NordColors.Nord1.G, NordColors.Nord1.B);
            var bgColor2 = Color.FromArgb((int)(config.BackgroundOpacity * 255), NordColors.Nord0.R, NordColors.Nord0.G, NordColors.Nord0.B);

            using (var path = CreateRoundedPath(bounds, config.CornerRadius))
            {
                // Subtle gradient
                using (var bgBrush = new LinearGradientBrush(
                    bounds,
                    bgColor1,
                    bgColor2,
                    config.Orientation == DockOrientation.Horizontal ? 90f : 0f))
                {
                    g.FillPath(bgBrush, path);
                }

                // Nord frost accent line at edge
                PaintNordAccentLine(g, bounds, config);

                // Border with Nord3 color
                if (config.ShowBorder)
                {
                    using (var pen = new Pen(Color.FromArgb(120, NordColors.Nord3), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Subtle shadow
            if (config.ShowShadow)
            {
                PaintNordShadow(g, bounds, config);
            }
        }

        private void PaintNordAccentLine(Graphics g, Rectangle bounds, DockConfig config)
        {
            // Thin frost-colored accent line
            var accentColor = Color.FromArgb(100, NordColors.Nord8);
            
            using (var pen = new Pen(accentColor, 2f))
            {
                if (config.Orientation == DockOrientation.Horizontal)
                {
                    int y = config.Position == DockPosition.Bottom ? bounds.Top + 1 : bounds.Bottom - 1;
                    g.DrawLine(pen,
                        bounds.Left + config.CornerRadius, y,
                        bounds.Right - config.CornerRadius, y);
                }
                else
                {
                    int x = config.Position == DockPosition.Right ? bounds.Left + 1 : bounds.Right - 1;
                    g.DrawLine(pen,
                        x, bounds.Top + config.CornerRadius,
                        x, bounds.Bottom - config.CornerRadius);
                }
            }
        }

        private void PaintNordShadow(Graphics g, Rectangle bounds, DockConfig config)
        {
            int shadowSize = 16;
            Rectangle shadowRect;

            // Nord uses softer shadows
            switch (config.Position)
            {
                case DockPosition.Bottom:
                    shadowRect = new Rectangle(bounds.X + 4, bounds.Y - shadowSize, bounds.Width - 8, shadowSize);
                    using (var brush = new LinearGradientBrush(
                        shadowRect,
                        Color.FromArgb(0, NordColors.Nord0),
                        Color.FromArgb(60, NordColors.Nord0),
                        90f))
                    {
                        g.FillRectangle(brush, shadowRect);
                    }
                    break;

                case DockPosition.Top:
                    shadowRect = new Rectangle(bounds.X + 4, bounds.Bottom, bounds.Width - 8, shadowSize);
                    using (var brush = new LinearGradientBrush(
                        shadowRect,
                        Color.FromArgb(60, NordColors.Nord0),
                        Color.FromArgb(0, NordColors.Nord0),
                        90f))
                    {
                        g.FillRectangle(brush, shadowRect);
                    }
                    break;
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState?.Item == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var bounds = itemState.Bounds;
            int iconRadius = 10;

            // Nord-styled hover/selection with frost colors
            if (itemState.IsHovered || itemState.IsSelected)
            {
                var highlightBounds = bounds;
                highlightBounds.Inflate(2, 2);

                using (var path = CreateRoundedPath(highlightBounds, iconRadius))
                {
                    Color highlightColor;
                    if (itemState.IsSelected)
                    {
                        // Use Nord frost blue for selection
                        highlightColor = Color.FromArgb(80, NordColors.Nord9);
                    }
                    else
                    {
                        // Subtle Nord2 for hover
                        highlightColor = Color.FromArgb(60, NordColors.Nord2);
                    }

                    using (var brush = new SolidBrush(highlightColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Border with frost color
                    if (itemState.IsSelected)
                    {
                        using (var pen = new Pen(Color.FromArgb(150, NordColors.Nord8), 2f))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }

            // Icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                var iconBounds = bounds;
                iconBounds.Inflate(-4, -4);

                using (var iconPath = CreateRoundedPath(iconBounds, iconRadius))
                {
                    // Apply subtle tint for Nord aesthetic
                    if (config.ApplyThemeToIcons)
                    {
                        StyledImagePainter.PaintWithTint(g, iconPath, itemState.Item.ImagePath, 
                            NordColors.Nord4, 0.9f, iconRadius);
                    }
                    else
                    {
                        StyledImagePainter.Paint(g, iconPath, itemState.Item.ImagePath);
                    }
                }
            }

            // Nord-styled badge
            if (config.ShowBadges && itemState.BadgeCount > 0)
            {
                PaintNordBadge(g, bounds, itemState.BadgeCount);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState?.Item == null || !itemState.IsRunning)
                return;

            var bounds = itemState.Bounds;
            
            // Use Nord frost colors for indicators
            var indicatorColor = itemState.IsSelected ? NordColors.Nord9 : NordColors.Nord8;

            // Nord uses circular indicators
            int dotSize = 5;
            Rectangle dotRect;

            if (config.Orientation == DockOrientation.Horizontal)
            {
                int offset = config.Position == DockPosition.Bottom ? 6 : -6;
                dotRect = new Rectangle(
                    bounds.X + (bounds.Width - dotSize) / 2,
                    config.Position == DockPosition.Bottom ? bounds.Bottom + offset : bounds.Top - offset - dotSize,
                    dotSize,
                    dotSize
                );
            }
            else
            {
                int offset = config.Position == DockPosition.Right ? 6 : -6;
                dotRect = new Rectangle(
                    config.Position == DockPosition.Right ? bounds.Right + offset : bounds.Left - offset - dotSize,
                    bounds.Y + (bounds.Height - dotSize) / 2,
                    dotSize,
                    dotSize
                );
            }

            // Subtle glow
            var glowRect = dotRect;
            glowRect.Inflate(3, 3);
            using (var glowBrush = new SolidBrush(Color.FromArgb(40, indicatorColor)))
            {
                g.FillEllipse(glowBrush, glowRect);
            }

            // Main dot
            using (var brush = new SolidBrush(indicatorColor))
            {
                g.FillEllipse(brush, dotRect);
            }
        }

        private void PaintNordBadge(Graphics g, Rectangle iconBounds, int count)
        {
            string text = count > 99 ? "99+" : count.ToString();
            
            int badgeSize = 20;
            using (var font = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            {
                var badgeBounds = new Rectangle(
                    iconBounds.Right - badgeSize / 2,
                    iconBounds.Top - badgeSize / 2,
                    badgeSize,
                    badgeSize
                );

                // Use Nord11 (red) for badges
                using (var bgBrush = new SolidBrush(NordColors.Nord11))
                {
                    g.FillEllipse(bgBrush, badgeBounds);
                }

                // Border with Polar Night color
                using (var pen = new Pen(NordColors.Nord0, 2f))
                {
                    g.DrawEllipse(pen, badgeBounds);
                }

                // Text with Snow Storm color
                using (var textBrush = new SolidBrush(NordColors.Nord6))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, font, textBrush, badgeBounds, format);
                }
            }
        }

        public override DockPainterMetrics GetMetrics(DockConfig config, IBeepTheme theme, bool useThemeColors)
        {
            var metrics = DockPainterMetrics.DefaultFor(DockStyle.NordDock, theme, useThemeColors);
            
            // Nord specific styling
            metrics.ItemSize = 50;
            metrics.ItemSpacing = 10;
            metrics.CornerRadius = 12;
            metrics.ItemCornerRadius = 10;
            metrics.BackgroundOpacity = 0.92f;
            metrics.ShowReflection = false;
            metrics.ShowShadow = true;
            metrics.ShowGlow = false;
            metrics.MaxScale = 1.35f;
            metrics.BackgroundColor = NordColors.Nord1;
            metrics.BorderColor = NordColors.Nord3;
            metrics.ItemHoverColor = NordColors.Nord2;
            metrics.ItemSelectedColor = NordColors.Nord9;
            metrics.IndicatorColor = NordColors.Nord8;
            metrics.AccentColor = NordColors.Nord10;

            return metrics;
        }
    }
}

