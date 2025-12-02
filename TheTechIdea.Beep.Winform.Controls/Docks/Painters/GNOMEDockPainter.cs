using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// GNOME Shell dash-style dock painter
    /// Follows GNOME HIG (Human Interface Guidelines) with Ubuntu/Dash to Dock styling
    /// </summary>
    public class GNOMEDockPainter : DockPainterBase
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // GNOME uses a semi-transparent dark background
            var bgColor = GetColor(
                config.BackgroundColor, 
                theme?.BackColor ?? Color.FromArgb(40, 40, 45), 
                config.BackgroundOpacity
            );

            // Adjust radius based on position
            int radius = config.Position == DockPosition.Left || config.Position == DockPosition.Right 
                ? 8 
                : config.CornerRadius;

            // Only round certain corners based on position
            using (var path = CreateDockPath(bounds, radius, config.Position))
            {
                // Dark semi-transparent background
                using (var bgBrush = new SolidBrush(Color.FromArgb((int)(config.BackgroundOpacity * 255), 40, 40, 45)))
                {
                    g.FillPath(bgBrush, path);
                }

                // Subtle inner glow for depth
                using (var innerGlow = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(30, 255, 255, 255),
                    Color.FromArgb(5, 255, 255, 255),
                    config.Orientation == DockOrientation.Horizontal ? 90f : 0f))
                {
                    using (var glowPath = CreateDockPath(bounds, radius, config.Position))
                    {
                        g.FillPath(innerGlow, glowPath);
                    }
                }

                // Border
                if (config.ShowBorder)
                {
                    var borderColor = Color.FromArgb(60, 255, 255, 255);
                    using (var pen = new Pen(borderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Shadow
            if (config.ShowShadow)
            {
                PaintGNOMEShadow(g, bounds, config);
            }
        }

        private GraphicsPath CreateDockPath(Rectangle bounds, int radius, DockPosition position)
        {
            var path = new GraphicsPath();

            // Round only the corners that are away from the screen edge
            switch (position)
            {
                case DockPosition.Left:
                    // Round right side only
                    path.AddLine(bounds.Left, bounds.Top, bounds.Right - radius, bounds.Top);
                    path.AddArc(bounds.Right - radius * 2, bounds.Top, radius * 2, radius * 2, 270, 90);
                    path.AddLine(bounds.Right, bounds.Top + radius, bounds.Right, bounds.Bottom - radius);
                    path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                    path.AddLine(bounds.Right - radius, bounds.Bottom, bounds.Left, bounds.Bottom);
                    path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
                    break;

                case DockPosition.Right:
                    // Round left side only
                    path.AddLine(bounds.Right, bounds.Top, bounds.Left + radius, bounds.Top);
                    path.AddArc(bounds.Left, bounds.Top, radius * 2, radius * 2, 270, -90);
                    path.AddLine(bounds.Left, bounds.Top + radius, bounds.Left, bounds.Bottom - radius);
                    path.AddArc(bounds.Left, bounds.Bottom - radius * 2, radius * 2, radius * 2, 180, -90);
                    path.AddLine(bounds.Left + radius, bounds.Bottom, bounds.Right, bounds.Bottom);
                    path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Top);
                    break;

                case DockPosition.Top:
                    // Round bottom only
                    path.AddLine(bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - radius);
                    path.AddArc(bounds.Left, bounds.Bottom - radius * 2, radius * 2, radius * 2, 180, -90);
                    path.AddLine(bounds.Left + radius, bounds.Bottom, bounds.Right - radius, bounds.Bottom);
                    path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, -90);
                    path.AddLine(bounds.Right, bounds.Bottom - radius, bounds.Right, bounds.Top);
                    path.AddLine(bounds.Right, bounds.Top, bounds.Left, bounds.Top);
                    break;

                case DockPosition.Bottom:
                default:
                    // Round top only
                    path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top + radius);
                    path.AddArc(bounds.Left, bounds.Top, radius * 2, radius * 2, 180, 90);
                    path.AddLine(bounds.Left + radius, bounds.Top, bounds.Right - radius, bounds.Top);
                    path.AddArc(bounds.Right - radius * 2, bounds.Top, radius * 2, radius * 2, 270, 90);
                    path.AddLine(bounds.Right, bounds.Top + radius, bounds.Right, bounds.Bottom);
                    path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                    break;
            }

            path.CloseFigure();
            return path;
        }

        private void PaintGNOMEShadow(Graphics g, Rectangle bounds, DockConfig config)
        {
            int shadowSize = 12;
            Rectangle shadowBounds;

            switch (config.Position)
            {
                case DockPosition.Left:
                    shadowBounds = new Rectangle(bounds.Right, bounds.Top + 4, shadowSize, bounds.Height - 8);
                    using (var brush = new LinearGradientBrush(
                        shadowBounds,
                        Color.FromArgb(60, 0, 0, 0),
                        Color.FromArgb(0, 0, 0, 0),
                        0f))
                    {
                        g.FillRectangle(brush, shadowBounds);
                    }
                    break;

                case DockPosition.Right:
                    shadowBounds = new Rectangle(bounds.Left - shadowSize, bounds.Top + 4, shadowSize, bounds.Height - 8);
                    using (var brush = new LinearGradientBrush(
                        shadowBounds,
                        Color.FromArgb(0, 0, 0, 0),
                        Color.FromArgb(60, 0, 0, 0),
                        0f))
                    {
                        g.FillRectangle(brush, shadowBounds);
                    }
                    break;

                case DockPosition.Top:
                    shadowBounds = new Rectangle(bounds.Left + 4, bounds.Bottom, bounds.Width - 8, shadowSize);
                    using (var brush = new LinearGradientBrush(
                        shadowBounds,
                        Color.FromArgb(60, 0, 0, 0),
                        Color.FromArgb(0, 0, 0, 0),
                        90f))
                    {
                        g.FillRectangle(brush, shadowBounds);
                    }
                    break;

                case DockPosition.Bottom:
                default:
                    shadowBounds = new Rectangle(bounds.Left + 4, bounds.Top - shadowSize, bounds.Width - 8, shadowSize);
                    using (var brush = new LinearGradientBrush(
                        shadowBounds,
                        Color.FromArgb(0, 0, 0, 0),
                        Color.FromArgb(60, 0, 0, 0),
                        90f))
                    {
                        g.FillRectangle(brush, shadowBounds);
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

            // GNOME uses subtle square-ish icons with small radius
            int iconRadius = 8;

            // Hover background (GNOME style)
            if (itemState.IsHovered)
            {
                using (var hoverPath = CreateRoundedPath(bounds, iconRadius))
                {
                    var hoverColor = Color.FromArgb(40, 255, 255, 255);
                    using (var brush = new SolidBrush(hoverColor))
                    {
                        g.FillPath(brush, hoverPath);
                    }
                }
            }

            // Selection/running background
            if (itemState.IsSelected || itemState.IsRunning)
            {
                var selectBounds = bounds;
                selectBounds.Inflate(-2, -2);
                
                using (var selectPath = CreateRoundedPath(selectBounds, iconRadius - 1))
                {
                    var selectColor = itemState.IsSelected 
                        ? Color.FromArgb(60, 255, 255, 255)
                        : Color.FromArgb(30, 255, 255, 255);
                    
                    using (var brush = new SolidBrush(selectColor))
                    {
                        g.FillPath(brush, selectPath);
                    }
                }
            }

            // Paint icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                var iconBounds = bounds;
                iconBounds.Inflate(-4, -4);

                using (var iconPath = CreateRoundedPath(iconBounds, iconRadius))
                {
                    StyledImagePainter.Paint(g, iconPath, itemState.Item.ImagePath);
                }
            }

            // Badge for notifications
            if (config.ShowBadges && itemState.BadgeCount > 0)
            {
                PaintGNOMEBadge(g, bounds, itemState.BadgeCount);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState?.Item == null || !itemState.IsRunning)
                return;

            var bounds = itemState.Bounds;
            var indicatorColor = theme?.AccentColor ?? Color.FromArgb(53, 132, 228); // GNOME blue

            // GNOME uses a line indicator on the side
            int lineThickness = 3;
            int lineLength = Math.Min(bounds.Width / 2, 24);

            using (var pen = new Pen(indicatorColor, lineThickness) 
                { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                if (config.Orientation == DockOrientation.Horizontal)
                {
                    // Indicator below icon
                    int y = bounds.Bottom + 4;
                    int x1 = bounds.X + (bounds.Width - lineLength) / 2;
                    int x2 = x1 + lineLength;
                    g.DrawLine(pen, x1, y, x2, y);
                }
                else
                {
                    // Indicator to the side of icon
                    int x = config.Position == DockPosition.Left ? bounds.Right + 4 : bounds.Left - 4;
                    int y1 = bounds.Y + (bounds.Height - lineLength) / 2;
                    int y2 = y1 + lineLength;
                    g.DrawLine(pen, x, y1, x, y2);
                }
            }
        }

        private void PaintGNOMEBadge(Graphics g, Rectangle iconBounds, int count)
        {
            string text = count > 99 ? "99+" : count.ToString();
            
            int badgeSize = 22;
            using (var font = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                var badgeBounds = new Rectangle(
                    iconBounds.Right - badgeSize + 4,
                    iconBounds.Top - 4,
                    badgeSize,
                    badgeSize
                );

                // GNOME uses orange/red for notifications
                using (var bgBrush = new SolidBrush(Color.FromArgb(230, 97, 53)))
                {
                    g.FillEllipse(bgBrush, badgeBounds);
                }

                // White border
                using (var pen = new Pen(Color.FromArgb(40, 40, 45), 2f))
                {
                    g.DrawEllipse(pen, badgeBounds);
                }

                // Text
                using (var textBrush = new SolidBrush(Color.White))
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
            var metrics = DockPainterMetrics.DefaultFor(DockStyle.GNOMEDock, theme, useThemeColors);
            
            // GNOME specific styling
            metrics.ItemSize = 48;
            metrics.ItemSpacing = 8;
            metrics.CornerRadius = 12;
            metrics.ItemCornerRadius = 8;
            metrics.BackgroundOpacity = 0.85f;
            metrics.ShowReflection = false;
            metrics.ShowShadow = true;
            metrics.ShowGlow = false;
            metrics.MaxScale = 1.4f;
            metrics.BackgroundColor = Color.FromArgb(40, 40, 45);
            metrics.BorderColor = Color.FromArgb(60, 255, 255, 255);

            return metrics;
        }
    }
}

