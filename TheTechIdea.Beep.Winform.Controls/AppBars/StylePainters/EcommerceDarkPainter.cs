using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// E-commerce Dark Painter
    /// Sleek dark design with product categories and cart functionality
    /// Features: Dark background, vibrant accent colors, emphasis on cart/account buttons
    /// </summary>
    public class EcommerceDarkPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 12;
        private const int TAB_SPACING = 12;
        private const int LOGO_WIDTH = 40;
        private const int BUTTON_WIDTH = 95;

        public override WebHeaderStyle Style => WebHeaderStyle.EcommerceDark;

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
            // Draw dark background
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(bounds, Color.FromArgb(22, 22, 34), Color.FromArgb(14, 14, 24), 90f))
            {
                g.FillRectangle(brush, bounds);
            }

            int x = bounds.Left + PADDING;

            // Draw Logo as badge
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogoBadge(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }

            // Draw vertical separator
            using (var pen = new Pen(Color.FromArgb(60, 60, 75), 1))
            {
                g.DrawLine(pen, x, bounds.Top + 8, x, bounds.Bottom - 8);
            }
            x += PADDING;

            // Draw Tabs (Categories)
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        x,
                        bounds.Top + 8,
                        (int)textSize.Width + 14,
                        bounds.Height - 16);

                    tab.Bounds = tabBounds;

                    // Draw tab background with subtle hover/active highlights
                    if (tab.IsActive)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(255, 140, 0)))
                        {
                            g.FillRectangle(brush, tabBounds);
                        }
                    }
                    else if (tab.IsHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(60, 60, 75)))
                        {
                            g.FillRectangle(brush, tabBounds);
                        }
                    }

                    // Draw tab text
                    Color textColor = tab.IsActive ? Color.White : Color.FromArgb(180, 180, 190);
                    DrawTab(g, tabBounds, tab, tab.IsActive, tab.IsHovered, textColor, tabFont);

                    x += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw Buttons (Right side) - Shopping related
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = bounds.Right - PADDING;
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    // Render cart/account as a pill/circle for emphasis if it has an ImagePath
                    int width = btn.Width > 0 ? btn.Width : BUTTON_WIDTH;
                    buttonX -= width;
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 34) / 2, width, 34);
                    btn.Bounds = btnBounds;

                    // Draw button - vibrant for cart/account
                    Color bgColor = btn.IsHovered ? Color.FromArgb(255, 140, 0) : Color.FromArgb(50, 50, 65);
                    Color textColor = btn.IsHovered ? Color.White : Color.FromArgb(200, 200, 210);

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(brush, btnBounds);
                    }

                    using (var pen = new Pen(Color.FromArgb(100, 100, 120), 1))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        // If this button has an image, render it as a circular avatar/badge on right
                        if (!string.IsNullOrEmpty(btn.ImagePath))
                        {
                            var avatarRect = new Rectangle(btnBounds.Right - 30, btnBounds.Top + 2, 28, 28);
                            DrawLogoBadge(g, avatarRect, btn.ImagePath, theme);
                        }
                        else
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    buttonX -= 5;
                }
            }

            // Draw bottom highlight line
            using (var pen = new Pen(Color.FromArgb(255, 140, 0), 2))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 2, bounds.Right, bounds.Bottom - 2);
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
