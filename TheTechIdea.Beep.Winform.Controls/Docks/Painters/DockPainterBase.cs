using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Base class for dock painters providing common functionality
    /// </summary>
    public abstract class DockPainterBase : IDockPainter
    {
        #region Metrics

        /// <summary>
        /// Gets the metrics for this painter. Override to provide custom metrics.
        /// </summary>
        public virtual DockPainterMetrics GetMetrics(DockConfig config, IBeepTheme theme, bool useThemeColors)
        {
            return DockPainterMetrics.DefaultFor(config.Style, theme, useThemeColors);
        }

        #endregion

        #region Abstract Methods

        public abstract void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme);
        public abstract void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme);
        public abstract void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme);

        #endregion

        #region Separator Painting

        public virtual void PaintSeparator(Graphics g, Point position, DockConfig config, IBeepTheme theme)
        {
            if (config.SeparatorStyle == DockSeparatorStyle.None)
                return;

            var color = config.SeparatorColor;
            if (color == Color.Empty && theme != null)
                color = Color.FromArgb(100, theme.BorderColor);

            switch (config.SeparatorStyle)
            {
                case DockSeparatorStyle.Line:
                    PaintLineSeparator(g, position, config, color);
                    break;

                case DockSeparatorStyle.Dot:
                    PaintDotSeparator(g, position, config, color);
                    break;

                case DockSeparatorStyle.Space:
                    // Just spacing, no visual element
                    break;
            }
        }

        protected virtual void PaintLineSeparator(Graphics g, Point position, DockConfig config, Color color)
        {
            using (var pen = new Pen(color, 1))
            {
                if (config.Orientation == DockOrientation.Horizontal)
                {
                    int y1 = position.Y;
                    int y2 = position.Y + config.DockHeight - config.Padding * 2;
                    g.DrawLine(pen, position.X, y1, position.X, y2);
                }
                else
                {
                    int x1 = position.X;
                    int x2 = position.X + config.DockHeight - config.Padding * 2;
                    g.DrawLine(pen, x1, position.Y, x2, position.Y);
                }
            }
        }

        protected virtual void PaintDotSeparator(Graphics g, Point position, DockConfig config, Color color)
        {
            using (var brush = new SolidBrush(color))
            {
                int dotSize = 4;
                var dotRect = new Rectangle(
                    position.X - dotSize / 2,
                    position.Y - dotSize / 2,
                    dotSize,
                    dotSize
                );
                g.FillEllipse(brush, dotRect);
            }
        }

        #endregion

        #region Layout Calculations

        public virtual Rectangle CalculateDockBounds(List<DockItemState> itemStates, DockConfig config, Size containerSize)
        {
            int totalSize = CalculateTotalSize(itemStates, config);
            int dockSize = config.Orientation == DockOrientation.Horizontal ? config.DockHeight : config.DockHeight;

            Rectangle bounds;

            if (config.Orientation == DockOrientation.Horizontal)
            {
                int x = CalculateAlignedPosition(totalSize, containerSize.Width, config.Alignment);
                int y = GetDockY(config, containerSize.Height, dockSize);
                bounds = new Rectangle(x, y, totalSize, dockSize);
            }
            else
            {
                int x = GetDockX(config, containerSize.Width, dockSize);
                int y = CalculateAlignedPosition(totalSize, containerSize.Height, config.Alignment);
                bounds = new Rectangle(x, y, dockSize, totalSize);
            }

            return bounds;
        }

        public virtual Rectangle CalculateItemBounds(int index, List<DockItemState> allStates, DockConfig config, Rectangle dockBounds)
        {
            if (index < 0 || index >= allStates.Count)
                return Rectangle.Empty;

            var state = allStates[index];
            int currentPos = config.Padding;

            // Calculate position by summing previous items
            for (int i = 0; i < index; i++)
            {
                int itemSize = (int)(config.ItemSize * allStates[i].CurrentScale);
                currentPos += itemSize + config.Spacing;
            }

            int scaledSize = (int)(config.ItemSize * state.CurrentScale);
            int hoverOffset = state.IsHovered ? config.HoverOffset : 0;

            Rectangle itemRect;

            if (config.Orientation == DockOrientation.Horizontal)
            {
                int x = dockBounds.X + currentPos;
                int y = dockBounds.Y + (dockBounds.Height - scaledSize) / 2 - hoverOffset;
                itemRect = new Rectangle(x, y, scaledSize, scaledSize);
            }
            else
            {
                int x = dockBounds.X + (dockBounds.Width - scaledSize) / 2 - hoverOffset;
                int y = dockBounds.Y + currentPos;
                itemRect = new Rectangle(x, y, scaledSize, scaledSize);
            }

            return itemRect;
        }

        protected virtual int CalculateTotalSize(List<DockItemState> itemStates, DockConfig config)
        {
            int total = config.Padding;

            foreach (var state in itemStates)
            {
                int itemSize = (int)(config.ItemSize * state.CurrentScale);
                total += itemSize + config.Spacing;
            }

            total += config.Padding - config.Spacing; // Remove last spacing, add end padding
            return total;
        }

        protected virtual int GetDockY(DockConfig config, int containerHeight, int dockHeight)
        {
            return config.Position switch
            {
                DockPosition.Top => 0,
                DockPosition.Bottom => containerHeight - dockHeight,
                DockPosition.Center => (containerHeight - dockHeight) / 2,
                _ => containerHeight - dockHeight
            };
        }

        protected virtual int GetDockX(DockConfig config, int containerWidth, int dockWidth)
        {
            return config.Position switch
            {
                DockPosition.Left => 0,
                DockPosition.Right => containerWidth - dockWidth,
                DockPosition.Center => (containerWidth - dockWidth) / 2,
                _ => 0
            };
        }

        protected virtual int CalculateAlignedPosition(int totalSize, int containerSize, DockAlignment alignment)
        {
            return alignment switch
            {
                DockAlignment.Start => 0,
                DockAlignment.Center => (containerSize - totalSize) / 2,
                DockAlignment.End => containerSize - totalSize,
                _ => (containerSize - totalSize) / 2
            };
        }

        #endregion

        #region Hit Testing

        public virtual DockItemState HitTest(Point location, List<DockItemState> itemStates, DockConfig config)
        {
            // Test in reverse order (top items first)
            for (int i = itemStates.Count - 1; i >= 0; i--)
            {
                var state = itemStates[i];
                if (state.HitBounds.Contains(location))
                {
                    return state;
                }
            }

            return null;
        }

        #endregion

        #region Helper Methods

        protected GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            // Top-left
            path.AddArc(arc, 180, 90);

            // Top-right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom-right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom-left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        protected Color GetColor(Color? customColor, Color themeColor, float opacity = 1f)
        {
            var baseColor = customColor ?? themeColor;
            if (opacity < 1f)
            {
                return Color.FromArgb((int)(255 * opacity), baseColor);
            }
            return baseColor;
        }

        protected void PaintItemIcon(Graphics g, Rectangle bounds, string imagePath, DockConfig config, IBeepTheme theme, float opacity = 1f)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            using (var path = CreateRoundedPath(bounds, config.CornerRadius / 2))
            {
                if (config.ApplyThemeToIcons && theme != null)
                {
                    var tintColor = GetColor(config.ForegroundColor, theme.ForeColor);
                    StyledImagePainter.PaintWithTint(g, path, imagePath, tintColor, opacity, config.CornerRadius / 2);
                }
                else
                {
                    StyledImagePainter.Paint(g, path, imagePath);
                }
            }
        }

        protected void PaintShadow(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            if (!config.ShowShadow)
                return;

            // Use simple shadow for performance
            int shadowSize = 10;
            int shadowOffset = 3;

            using (var shadowPath = CreateRoundedPath(
                new Rectangle(
                    bounds.X + shadowOffset,
                    bounds.Y + shadowOffset,
                    bounds.Width,
                    bounds.Height
                ),
                config.CornerRadius
            ))
            {
                for (int i = shadowSize; i > 0; i--)
                {
                    int alpha = (int)(30 * (i / (float)shadowSize));
                    using (var brush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                    using (var pen = new Pen(brush, 1))
                    {
                        g.DrawPath(pen, shadowPath);
                    }
                }
            }
        }

        protected void PaintGlow(Graphics g, Rectangle bounds, Color glowColor, int glowSize = 10)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(bounds);

                using (var pgb = new PathGradientBrush(path))
                {
                    pgb.CenterColor = Color.FromArgb(100, glowColor);
                    pgb.SurroundColors = new[] { Color.FromArgb(0, glowColor) };

                    var inflated = bounds;
                    inflated.Inflate(glowSize, glowSize);
                    g.FillEllipse(pgb, inflated);
                }
            }
        }

        #endregion
    }
}
