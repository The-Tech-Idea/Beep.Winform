using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Trend Modern Painter
    /// Bold vibrant design with emphasis on action buttons and typography
    /// Features: Minimal logo, bold tabs with pill background, prominent buttons, dark background
    /// </summary>
    public class TrendModernPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 15;
        private const int TAB_SPACING = 8;
        private const int LOGO_WIDTH = 45;
        private const int BUTTON_WIDTH = 110;

        public override WebHeaderStyle Style => WebHeaderStyle.TrendModern;

        public override void PaintHeader(
            Graphics g,
            Rectangle bounds,
            IBeepTheme theme,
            List<WebHeaderTab> tabs,
            List<WebHeaderActionButton> buttons,
            int selectedTabIndex,
            string logoImagePath,
            bool showLogo,
            bool showSearchBox,
            string searchText,
            Font tabFont,
            Font buttonFont)
        {
            // Draw dark background gradient
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(bounds, Color.FromArgb(18, 25, 40), Color.FromArgb(35, 40, 60), 90f))
            {
                g.FillRectangle(brush, bounds);
            }

            int x = bounds.Left + PADDING;

            // Draw Logo (pill-shaped logo)
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogoPill(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }
                        using (var boldTabFont = new Font(tabFont.FontFamily, tabFont.Size + 1, FontStyle.Bold))
            // Draw Tabs with pill background
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var boldTabFont = new Font(tabFont.FontFamily, tabFont.Size + 1, FontStyle.Bold);
                    var tabBounds = new Rectangle(
                        x,
                        bounds.Top + (bounds.Height - 34) / 2,
                        (int)textSize.Width + 28,
                        34);

                    tab.Bounds = tabBounds;

                    // Draw pill background for active tab
                    if (tab.IsActive || tab.IsHovered)
                    {
                        Color pillColor = tab.IsActive ? Color.FromArgb(255, 107, 107) : Color.FromArgb(100, 100, 120);
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, tabBounds.Height/2))
                        using (var brush = new SolidBrush(pillColor))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Draw tab text in white with bold style
                    using (var brush = new SolidBrush(Color.White))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, boldTabFont, brush, tabBounds, sf);
                    }

                    x += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw Buttons (Right side)
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = bounds.Right - PADDING;
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    buttonX -= BUTTON_WIDTH;
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 38) / 2, BUTTON_WIDTH, 38);
                    btn.Bounds = btnBounds;

                    // Draw button with vibrant color on hover
                    Color bgColor = btn.IsHovered ? Color.FromArgb(255, 107, 107) : Color.FromArgb(60, 60, 80);
                    Color borderColor = btn.IsHovered ? Color.FromArgb(255, 130, 130) : Color.FromArgb(100, 100, 130);

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(brush, btnBounds);
                    }

                    using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, 6))
                    using (var pen = new Pen(borderColor, 2))
                    {
                        g.DrawPath(pen, path);
                    }

                    using (var brush = new SolidBrush(Color.White))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        // Bold the primary CTA at far right (first drawn in reverse loop)
                        if (i == buttons.Count - 1)
                        {
                            using (var boldFont = new Font(buttonFont.FontFamily, buttonFont.Size, FontStyle.Bold))
                            {
                                g.DrawString(btn.Text, boldFont, brush, btnBounds, sf);
                            }
                        }
                        else
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    buttonX -= TAB_SPACING;
                }
            }
        }

        public override Rectangle GetTabBounds(int tabIndex, Rectangle headerBounds, List<WebHeaderTab> tabs)
        {
            if (tabIndex >= 0 && tabIndex < tabs.Count)
                return tabs[tabIndex].Bounds;
            return Rectangle.Empty;
        }

        public override Rectangle GetButtonBounds(int buttonIndex, Rectangle headerBounds, List<WebHeaderActionButton> buttons)
        {
            if (buttonIndex >= 0 && buttonIndex < buttons.Count)
                return buttons[buttonIndex].Bounds;
            return Rectangle.Empty;
        }

        public override int GetHitElement(Point pt, Rectangle headerBounds, List<WebHeaderTab> tabs, List<WebHeaderActionButton> buttons)
        {
            if (tabs != null)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (HitTestRect(pt, tabs[i].Bounds))
                        return i;
                }
            }

            if (buttons != null)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (HitTestRect(pt, buttons[i].Bounds))
                        return -(i + 1);
                }
            }

            return -1;
        }
    }
}
