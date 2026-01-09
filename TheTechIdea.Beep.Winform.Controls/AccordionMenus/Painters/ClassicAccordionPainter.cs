using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Painters
{
    /// <summary>
    /// Classic bordered style accordion painter
    /// </summary>
    public class ClassicAccordionPainter : AccordionPainterBase
    {
        public override void PaintAccordionBackground(Graphics g, Rectangle bounds, AccordionRenderOptions options)
        {
            Color bgColor = AccordionThemeHelpers.GetAccordionBackgroundColor(
                options.Theme,
                options.UseThemeColors);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        public override void PaintHeader(Graphics g, Rectangle bounds, string title, AccordionRenderOptions options)
        {
            Color bgColor = AccordionThemeHelpers.GetHeaderBackgroundColor(
                options.Theme,
                options.UseThemeColors);

            Color fgColor = AccordionThemeHelpers.GetHeaderForegroundColor(
                options.Theme,
                options.UseThemeColors);

            // Paint background with border
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Paint border
            Color borderColor = AccordionThemeHelpers.GetItemBorderColor(
                options.Theme,
                options.UseThemeColors,
                false);

            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Paint title text
            if (!string.IsNullOrEmpty(title) && !options.IsCollapsed)
            {
                Font font = AccordionFontHelpers.GetHeaderFont(options.ControlStyle);
                using (var brush = new SolidBrush(fgColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    Rectangle textRect = new Rectangle(
                        bounds.Left + options.Padding,
                        bounds.Top,
                        bounds.Width - options.Padding * 2,
                        bounds.Height);

                    g.DrawString(title, font, brush, textRect, format);
                }
            }
        }

        public override void PaintItem(Graphics g, Rectangle bounds, SimpleItem item, AccordionItemState state, AccordionRenderOptions options)
        {
            var (bgColor, fgColor, borderColor, highlightColor, expanderColor) = GetItemColors(state, options);

            // Paint highlight bar
            PaintHighlight(g, bounds, state, options);

            // Paint background with border
            Rectangle contentRect = new Rectangle(
                bounds.Left + options.HighlightWidth,
                bounds.Top,
                bounds.Width - options.HighlightWidth,
                bounds.Height);

            using (var path = CreateRoundedPath(contentRect, 4))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Paint border if selected or hovered
                if (state.IsSelected || state.IsHovered)
                {
                    using (var pen = new Pen(borderColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Paint icon if available
            int currentX = contentRect.Left + options.Padding;
            if (!string.IsNullOrEmpty(item?.ImagePath) && File.Exists(item.ImagePath))
            {
                Size iconSize = AccordionIconHelpers.GetIconSize(options.ItemHeight, 0.6f);
                Rectangle iconRect = AccordionIconHelpers.CalculateItemIconBounds(
                    contentRect,
                    options.ItemHeight,
                    0.6f,
                    options.Padding);

                PaintItemIcon(g, iconRect, item.ImagePath, state, options);
                currentX = iconRect.Right + options.Padding;
            }
            else
            {
                currentX += 8;
            }

            // Paint text if not collapsed
            if (!options.IsCollapsed)
            {
                Rectangle textRect = new Rectangle(
                    currentX,
                    bounds.Top,
                    bounds.Right - currentX - 40,
                    bounds.Height);

                PaintItemText(g, textRect, item?.Text ?? item?.Name ?? "", state, options);
            }

            // Paint expander icon
            if (item?.Children != null && item.Children.Count > 0 && !options.IsCollapsed)
            {
                Rectangle expanderRect = new Rectangle(
                    bounds.Right - 32,
                    bounds.Top + (bounds.Height - 16) / 2,
                    16,
                    16);

                PaintExpanderIcon(g, expanderRect, state.IsExpanded, options);
            }
        }

        public override void PaintChildItem(Graphics g, Rectangle bounds, SimpleItem item, AccordionItemState state, AccordionRenderOptions options)
        {
            var (bgColor, fgColor, borderColor, highlightColor, expanderColor) = GetItemColors(state, options);

            // Paint background
            using (var path = CreateRoundedPath(bounds, 2))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
            }

            // Paint icon if available
            int currentX = bounds.Left + options.Padding;
            if (!string.IsNullOrEmpty(item?.ImagePath) && File.Exists(item.ImagePath))
            {
                Size iconSize = AccordionIconHelpers.GetIconSize(options.ChildItemHeight, 0.55f);
                Rectangle iconRect = AccordionIconHelpers.CalculateItemIconBounds(
                    bounds,
                    options.ChildItemHeight,
                    0.55f,
                    options.Padding);

                PaintItemIcon(g, iconRect, item.ImagePath, state, options);
                currentX = iconRect.Right + options.Padding;
            }
            else
            {
                currentX += 8;
            }

            // Paint text
            if (!options.IsCollapsed)
            {
                Rectangle textRect = new Rectangle(
                    currentX,
                    bounds.Top,
                    bounds.Width - currentX - options.Padding,
                    bounds.Height);

                PaintItemText(g, textRect, item?.Text ?? item?.Name ?? "", state, options, isChild: true);
            }
        }

        public override void PaintExpanderIcon(Graphics g, Rectangle bounds, bool isExpanded, AccordionRenderOptions options)
        {
            string iconPath = isExpanded
                ? AccordionIconHelpers.GetCollapseIconPath()
                : AccordionIconHelpers.GetExpandIconPath();

            Color iconColor = AccordionThemeHelpers.GetExpanderIconColor(
                options.Theme,
                options.UseThemeColors,
                isExpanded);

            AccordionIconHelpers.PaintIcon(
                g,
                bounds,
                iconPath,
                iconColor,
                options.Theme,
                options.UseThemeColors,
                false,
                false,
                options.ControlStyle);
        }

        public override void PaintConnectorLine(Graphics g, Point start, Point end, AccordionRenderOptions options)
        {
            Color lineColor = AccordionThemeHelpers.GetConnectorLineColor(
                options.Theme,
                options.UseThemeColors);

            using (var pen = new Pen(lineColor, 1))
            {
                g.DrawLine(pen, start.X, start.Y, start.X, end.Y);
                g.DrawLine(pen, start.X, end.Y, end.X, end.Y);
            }
        }
    }
}
