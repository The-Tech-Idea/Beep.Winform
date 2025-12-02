using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// iOS-style dock painter with app icon badges, bounce animations, and frosted glass
    /// Follows iOS/iPadOS Human Interface Guidelines
    /// </summary>
    public class iOSDockPainter : DockPainterBase
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // iOS-style frosted glass background with rounded corners
            var bgColor = GetColor(config.BackgroundColor, theme?.BackColor ?? Color.FromArgb(240, 245, 250), config.BackgroundOpacity);
            
            // Increase corner radius for iOS pill shape
            int radius = Math.Min(bounds.Height / 2, 28);

            using (var path = CreateRoundedPath(bounds, radius))
            {
                // Frosted glass effect with gradient
                using (var bgBrush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb((int)(config.BackgroundOpacity * 255), 250, 250, 255),
                    Color.FromArgb((int)(config.BackgroundOpacity * 255), 235, 240, 250),
                    90f))
                {
                    g.FillPath(bgBrush, path);
                }

                // Subtle border for glass effect
                if (config.ShowBorder)
                {
                    var borderColor = Color.FromArgb(80, 255, 255, 255);
                    using (var pen = new Pen(borderColor, 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // iOS-style shadow beneath dock
            if (config.ShowShadow)
            {
                var shadowRect = new Rectangle(
                    bounds.X + 4,
                    bounds.Bottom - 2,
                    bounds.Width - 8,
                    8
                );
                using (var shadowBrush = new LinearGradientBrush(
                    shadowRect,
                    Color.FromArgb(40, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0),
                    90f))
                {
                    g.FillRectangle(shadowBrush, shadowRect);
                }
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState?.Item == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var bounds = itemState.Bounds;
            
            // iOS icons have rounded square shape (squircle approximation)
            int iconRadius = Math.Max(bounds.Width / 5, 12);

            // Shadow for depth
            if (config.ShowShadow && !itemState.IsDragging)
            {
                var shadowBounds = bounds;
                shadowBounds.Offset(0, 2);
                using (var shadowPath = CreateRoundedPath(shadowBounds, iconRadius))
                {
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }

            // Item background (for hover/selection)
            if (itemState.IsHovered || itemState.IsSelected)
            {
                var highlightBounds = bounds;
                highlightBounds.Inflate(4, 4);
                
                using (var highlightPath = CreateRoundedPath(highlightBounds, iconRadius + 2))
                {
                    var highlightColor = itemState.IsSelected 
                        ? Color.FromArgb(40, 0, 122, 255) 
                        : Color.FromArgb(20, 255, 255, 255);
                    
                    using (var brush = new SolidBrush(highlightColor))
                    {
                        g.FillPath(brush, highlightPath);
                    }
                }
            }

            // Paint icon with iOS styling
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                using (var iconPath = CreateRoundedPath(bounds, iconRadius))
                {
                    // Clip to rounded rectangle
                    g.SetClip(iconPath);
                    StyledImagePainter.Paint(g, iconPath, itemState.Item.ImagePath);
                    g.ResetClip();

                    // iOS icon border
                    using (var borderPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1f))
                    {
                        g.DrawPath(borderPen, iconPath);
                    }
                }
            }

            // Badge notification (iOS style)
            if (config.ShowBadges && itemState.BadgeCount > 0)
            {
                PaintBadge(g, bounds, itemState.BadgeCount, config, theme);
            }

            // Running indicator (small dot beneath icon)
            if (config.ShowRunningIndicator && itemState.IsRunning)
            {
                PaintRunningIndicator(g, bounds, config, theme);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState?.Item == null || config.IndicatorStyle == DockIndicatorStyle.None)
                return;

            var bounds = itemState.Bounds;
            var indicatorColor = GetColor(null, theme?.AccentColor ?? config.IndicatorColor);

            switch (config.IndicatorStyle)
            {
                case DockIndicatorStyle.Dot:
                    PaintDotIndicator(g, bounds, indicatorColor, config.Orientation);
                    break;

                case DockIndicatorStyle.Line:
                    PaintLineIndicator(g, bounds, indicatorColor, config.Orientation);
                    break;

                case DockIndicatorStyle.Glow:
                    PaintGlow(g, bounds, indicatorColor, 8);
                    break;
            }
        }

        private void PaintBadge(Graphics g, Rectangle iconBounds, int count, DockConfig config, IBeepTheme theme)
        {
            // iOS-style badge in top-right corner
            string text = count > 99 ? "99+" : count.ToString();
            
            // Badge sizing
            int badgeHeight = Math.Max(20, iconBounds.Height / 4);
            int minWidth = badgeHeight;
            
            using (var font = new Font("Segoe UI", badgeHeight * 0.55f, FontStyle.Bold))
            {
                var textSize = g.MeasureString(text, font);
                int badgeWidth = Math.Max(minWidth, (int)textSize.Width + 8);
                
                var badgeBounds = new Rectangle(
                    iconBounds.Right - badgeWidth / 2,
                    iconBounds.Top - badgeHeight / 2,
                    badgeWidth,
                    badgeHeight
                );

                // Badge background with iOS red
                using (var path = CreateRoundedPath(badgeBounds, badgeHeight / 2))
                {
                    var badgeColor = theme?.ButtonPressedBackColor ?? Color.FromArgb(255, 69, 58);
                    using (var brush = new SolidBrush(badgeColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Badge border
                    using (var pen = new Pen(Color.FromArgb(255, 255, 255), 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Badge text
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var textFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, font, textBrush, badgeBounds, textFormat);
                }
            }
        }

        private void PaintRunningIndicator(Graphics g, Rectangle iconBounds, DockConfig config, IBeepTheme theme)
        {
            // Small dot beneath icon
            int dotSize = 4;
            var dotBounds = new Rectangle(
                iconBounds.X + (iconBounds.Width - dotSize) / 2,
                iconBounds.Bottom + 4,
                dotSize,
                dotSize
            );

            var indicatorColor = theme?.AccentColor ?? Color.FromArgb(0, 122, 255);
            using (var brush = new SolidBrush(indicatorColor))
            {
                g.FillEllipse(brush, dotBounds);
            }
        }

        private void PaintDotIndicator(Graphics g, Rectangle bounds, Color color, DockOrientation orientation)
        {
            int dotSize = 6;
            Rectangle dotRect;

            if (orientation == DockOrientation.Horizontal)
            {
                dotRect = new Rectangle(
                    bounds.X + (bounds.Width - dotSize) / 2,
                    bounds.Bottom + 6,
                    dotSize,
                    dotSize
                );
            }
            else
            {
                dotRect = new Rectangle(
                    bounds.Right + 6,
                    bounds.Y + (bounds.Height - dotSize) / 2,
                    dotSize,
                    dotSize
                );
            }

            using (var brush = new SolidBrush(color))
            {
                g.FillEllipse(brush, dotRect);
            }
        }

        private void PaintLineIndicator(Graphics g, Rectangle bounds, Color color, DockOrientation orientation)
        {
            int lineThickness = 3;
            int lineLength = bounds.Width / 2;

            using (var pen = new Pen(color, lineThickness) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                if (orientation == DockOrientation.Horizontal)
                {
                    int y = bounds.Bottom + 6;
                    int x1 = bounds.X + (bounds.Width - lineLength) / 2;
                    int x2 = x1 + lineLength;
                    g.DrawLine(pen, x1, y, x2, y);
                }
                else
                {
                    int x = bounds.Right + 6;
                    int y1 = bounds.Y + (bounds.Height - lineLength) / 2;
                    int y2 = y1 + lineLength;
                    g.DrawLine(pen, x, y1, x, y2);
                }
            }
        }

        public override DockPainterMetrics GetMetrics(DockConfig config, IBeepTheme theme, bool useThemeColors)
        {
            var metrics = DockPainterMetrics.DefaultFor(DockStyle.iOSDock, theme, useThemeColors);
            
            // iOS specific adjustments
            metrics.ItemSize = 60;
            metrics.ItemSpacing = 8;
            metrics.CornerRadius = 24;
            metrics.ItemCornerRadius = 12;
            metrics.BackgroundOpacity = 0.75f;
            metrics.ShowReflection = false;
            metrics.ShowShadow = true;
            metrics.ShowGlow = false;
            metrics.MaxScale = 1.5f;
            metrics.HoverScale = 1.3f;

            return metrics;
        }
    }
}

