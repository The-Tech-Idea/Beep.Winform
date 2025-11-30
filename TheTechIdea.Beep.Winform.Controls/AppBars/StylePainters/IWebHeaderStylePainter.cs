using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Interface for WebHeader style painters
    /// Each painter handles rendering of a complete header design variant
    /// </summary>
    public interface IWebHeaderStylePainter
    {
        /// <summary>Gets the style this painter implements</summary>
        WebHeaderStyle Style { get; }

        /// <summary>
        /// Paints the complete header with all components
        /// </summary>
        /// <param name="skipBackground">If true, do not draw background (for transparent background support)</param>
        void PaintHeader(
            Graphics g,
            Rectangle bounds,
            IBeepTheme theme,
            WebHeaderColors colors,
            List<WebHeaderTab> tabs,
            List<WebHeaderActionButton> buttons,
            int selectedTabIndex,
            string logoImagePath,
            string logoText,
            bool showLogo,
            bool showSearchBox,
            string searchText,
            Font tabFont,
            Font buttonFont,
            bool skipBackground = false);

        /// <summary>
        /// Gets tab bounds after layout
        /// </summary>
        Rectangle GetTabBounds(int tabIndex, Rectangle headerBounds, List<WebHeaderTab> tabs);

        /// <summary>
        /// Gets button bounds after layout
        /// </summary>
        Rectangle GetButtonBounds(int buttonIndex, Rectangle headerBounds, List<WebHeaderActionButton> buttons);

        /// <summary>
        /// Gets the tab or button at the given point, returns tab index (positive) or button index (negative)
        /// </summary>
        int GetHitElement(Point pt, Rectangle headerBounds, List<WebHeaderTab> tabs, List<WebHeaderActionButton> buttons);
    }

    /// <summary>
    /// Base class for style painters providing common functionality
    /// </summary>
    public abstract class WebHeaderStylePainterBase : IWebHeaderStylePainter
    {
        public abstract WebHeaderStyle Style { get; }

        public abstract void PaintHeader(
            Graphics g,
            Rectangle bounds,
            IBeepTheme theme,
            WebHeaderColors colors,
            List<WebHeaderTab> tabs,
            List<WebHeaderActionButton> buttons,
            int selectedTabIndex,
            string logoImagePath,
            string logoText,
            bool showLogo,
            bool showSearchBox,
            string searchText,
            Font tabFont,
            Font buttonFont,
            bool skipBackground = false);

        public abstract Rectangle GetTabBounds(int tabIndex, Rectangle headerBounds, List<WebHeaderTab> tabs);

        public abstract Rectangle GetButtonBounds(int buttonIndex, Rectangle headerBounds, List<WebHeaderActionButton> buttons);

        public abstract int GetHitElement(Point pt, Rectangle headerBounds, List<WebHeaderTab> tabs, List<WebHeaderActionButton> buttons);

        /// <summary>
        /// Helper: Draw logo at position using StyledImagePainter
        /// </summary>
        protected void DrawLogo(Graphics g, Rectangle logoBounds, string logoPath, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(logoPath) || logoBounds.Width <= 0 || logoBounds.Height <= 0)
                return;

            try
            {
                // Use StyledImagePainter to render SVG/image with style support
                StyledImagePainter.Paint(g, logoBounds, logoPath);
            }
            catch 
            { 
                // Silently handle image loading errors - draw placeholder
                using (var brush = new SolidBrush(theme?.BackColor ?? Color.White))
                {
                    g.FillRectangle(brush, logoBounds);
                }
                using (var pen = new Pen(theme?.ForeColor ?? Color.Black, 1))
                {
                    g.DrawRectangle(pen, logoBounds);
                }
            }
        }

        /// <summary>
        /// Draw logo in circle shape
        /// </summary>
        protected void DrawLogoCircle(Graphics g, Rectangle logoBounds, string logoPath, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(logoPath) || logoBounds.Width <= 0 || logoBounds.Height <= 0)
                return;

            try
            {
                float cx = logoBounds.Left + logoBounds.Width / 2f;
                float cy = logoBounds.Top + logoBounds.Height / 2f;
                float radius = Math.Min(logoBounds.Width, logoBounds.Height) / 2f;
                StyledImagePainter.PaintInCircle(g, cx, cy, radius, logoPath);
            }
            catch
            {
                DrawLogo(g, logoBounds, logoPath, theme);
            }
        }

        /// <summary>
        /// Draw logo inside a pill/capsule shape
        /// </summary>
        protected void DrawLogoPill(Graphics g, Rectangle logoBounds, string logoPath, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(logoPath) || logoBounds.Width <= 0 || logoBounds.Height <= 0)
                return;

            try
            {
                StyledImagePainter.PaintInPill(g, logoBounds.X, logoBounds.Y, logoBounds.Width, logoBounds.Height, logoPath);
            }
            catch
            {
                DrawLogo(g, logoBounds, logoPath, theme);
            }
        }

        /// <summary>
        /// Draw logo inside a small circular badge
        /// </summary>
        protected void DrawLogoBadge(Graphics g, Rectangle logoBounds, string logoPath, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(logoPath) || logoBounds.Width <= 0 || logoBounds.Height <= 0)
                return;

            try
            {
                float cx = logoBounds.Left + logoBounds.Width / 2f;
                float cy = logoBounds.Top + logoBounds.Height / 2f;
                float r = Math.Min(logoBounds.Width, logoBounds.Height) / 2f;
                StyledImagePainter.PaintInBadge(g, cx, cy, r, logoPath);
            }
            catch
            {
                DrawLogo(g, logoBounds, logoPath, theme);
            }
        }

        /// <summary>
        /// Draws a simple rounded search box with optional text
        /// </summary>
        protected void DrawSearchBox(Graphics g, Rectangle bounds, string text, IBeepTheme theme)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            Color back = Color.FromArgb(240, 240, 245);
            Color fore = Color.FromArgb(80, 80, 100);
            using (var brush = new SolidBrush(back))
            using (var pen = new Pen(Color.FromArgb(220, 220, 225), 1))
            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, 12))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            var textRect = new Rectangle(bounds.Left + 12, bounds.Top, bounds.Width - 36, bounds.Height);
            using (var sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near })
            using (var brush = new SolidBrush(fore))
            {
                g.DrawString(string.IsNullOrEmpty(text) ? "Search" : text, SystemFonts.DefaultFont, brush, textRect, sf);
            }

            // Draw magnifier icon at right using StyledImagePainter (tint to fore color)
            var iconRect = new Rectangle(bounds.Right - 28, bounds.Top + (bounds.Height - 18) / 2, 18, 18);
            try
            {
                // Use themed tint to color the icon
                StyledImagePainter.PaintWithTint(g, iconRect, SvgsUI.Search, fore, 1f, cornerRadius: 2);
            }
            catch
            {
                // Fallback to primitive icon if SVG resource is not available
                using (var pen = new Pen(fore, 2))
                {
                    g.DrawEllipse(pen, iconRect);
                    // draw handle
                    g.DrawLine(pen, iconRect.Right - 4, iconRect.Bottom - 4, iconRect.Right + 4, iconRect.Bottom + 4);
                }
            }
        }

        /// <summary>
        /// Helper: Draw tab with state
        /// </summary>
        protected void DrawTab(Graphics g, Rectangle tabBounds, WebHeaderTab tab, bool isActive, bool isHovered, Color textColor, Font font)
        {
            if (tabBounds.Width <= 0 || tabBounds.Height <= 0)
                return;

            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(tab.Text, font, brush, tabBounds, sf);
            }
        }

        /// <summary>
        /// Helper: Draw button with state
        /// </summary>
        protected void DrawButton(Graphics g, Rectangle btnBounds, WebHeaderActionButton btn, bool isHovered, Color foreColor, Color backColor, Font font)
        {
            if (btnBounds.Width <= 0 || btnBounds.Height <= 0)
                return;

            // Draw button background
            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, btnBounds);
            }

            // Draw button border
            using (var pen = new Pen(foreColor, 1))
            {
                g.DrawRectangle(pen, btnBounds);
            }

            // Draw button text and optional icon
            using (var brush = new SolidBrush(foreColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                if (!string.IsNullOrEmpty(btn.ImagePath))
                {
                    // draw icon at left inside button
                    var iconRect = new Rectangle(btnBounds.Left + 6, btnBounds.Top + (btnBounds.Height - 20) / 2, 20, 20);
                    try
                    {
                        DrawLogoCircle(g, iconRect, btn.ImagePath, null);
                    }
                    catch
                    {
                        // fallback to no icon
                    }

                    var textRect = new Rectangle(btnBounds.Left + 30, btnBounds.Top, btnBounds.Width - 36, btnBounds.Height);
                    g.DrawString(btn.Text, font, brush, textRect, sf);
                }
                else
                {
                    g.DrawString(btn.Text, font, brush, btnBounds, sf);
                }
            }
        }

        /// <summary>
        /// Helper: Hit test rectangle
        /// </summary>
        protected bool HitTestRect(Point pt, Rectangle rect)
        {
            return rect.Contains(pt);
        }
    }
}
