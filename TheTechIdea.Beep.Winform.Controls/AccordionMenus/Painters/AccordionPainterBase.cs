using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Painters
{
    /// <summary>
    /// Base class for accordion painters providing common functionality
    /// </summary>
    public abstract class AccordionPainterBase : IAccordionPainter
    {
        #region Abstract Methods

        public abstract void PaintAccordionBackground(Graphics g, Rectangle bounds, AccordionRenderOptions options);
        public abstract void PaintHeader(Graphics g, Rectangle bounds, string title, AccordionRenderOptions options);
        public abstract void PaintItem(Graphics g, Rectangle bounds, SimpleItem item, AccordionItemState state, AccordionRenderOptions options);
        public abstract void PaintChildItem(Graphics g, Rectangle bounds, SimpleItem item, AccordionItemState state, AccordionRenderOptions options);
        public abstract void PaintExpanderIcon(Graphics g, Rectangle bounds, bool isExpanded, AccordionRenderOptions options);
        public abstract void PaintConnectorLine(Graphics g, Point start, Point end, AccordionRenderOptions options);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets colors for an item using theme helpers
        /// </summary>
        protected (Color background, Color foreground, Color border, Color highlight, Color expander) GetItemColors(
            AccordionItemState state,
            AccordionRenderOptions options)
        {
            return AccordionThemeHelpers.GetItemColors(
                options.Theme,
                options.UseThemeColors,
                state.IsHovered,
                state.IsSelected,
                state.IsExpanded);
        }

        /// <summary>
        /// Creates a rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            if (diameter > bounds.Width || diameter > bounds.Height)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Paints an icon using icon helpers
        /// </summary>
        protected void PaintItemIcon(
            Graphics g,
            Rectangle bounds,
            string iconPath,
            AccordionItemState state,
            AccordionRenderOptions options)
        {
            if (string.IsNullOrEmpty(iconPath) || bounds.IsEmpty)
                return;

            Color iconColor = AccordionIconHelpers.GetIconColor(
                options.Theme,
                options.UseThemeColors,
                state.IsSelected,
                state.IsHovered);

            AccordionIconHelpers.PaintIcon(
                g,
                bounds,
                iconPath,
                iconColor,
                options.Theme,
                options.UseThemeColors,
                state.IsSelected,
                state.IsHovered,
                options.ControlStyle);
        }

        /// <summary>
        /// Paints text using font helpers
        /// </summary>
        protected void PaintItemText(
            Graphics g,
            Rectangle bounds,
            string text,
            AccordionItemState state,
            AccordionRenderOptions options,
            bool isChild = false)
        {
            if (string.IsNullOrEmpty(text) || bounds.IsEmpty)
                return;

            Font font = isChild
                ? AccordionFontHelpers.GetChildItemFont(options.ControlStyle)
                : AccordionFontHelpers.GetItemFont(options.ControlStyle, state.IsSelected);

            Color textColor = AccordionThemeHelpers.GetItemForegroundColor(
                options.Theme,
                options.UseThemeColors,
                state.IsSelected);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                g.DrawString(text, font, brush, bounds, format);
            }
        }

        /// <summary>
        /// Paints the highlight bar (left border indicator)
        /// </summary>
        protected void PaintHighlight(
            Graphics g,
            Rectangle itemBounds,
            AccordionItemState state,
            AccordionRenderOptions options)
        {
            if (!state.IsHovered && !state.IsSelected)
                return;

            Color highlightColor = AccordionThemeHelpers.GetHighlightColor(
                options.Theme,
                options.UseThemeColors,
                state.IsHovered,
                state.IsSelected);

            Rectangle highlightRect = new Rectangle(
                itemBounds.Left,
                itemBounds.Top,
                options.HighlightWidth,
                itemBounds.Height);

            using (var brush = new SolidBrush(highlightColor))
            {
                g.FillRectangle(brush, highlightRect);
            }
        }

        #endregion
    }
}
