using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// SaaS Professional Painter
    /// Dashboard-style with user profile and notifications
    /// Features: Blue accent color, dashboard-focused layout, settings/profile buttons, notification indicator
    /// </summary>
    public class SaaSProfessionalPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 12;
        private const int TAB_SPACING = 18;
        private const int LOGO_WIDTH = 36;
        private const int BUTTON_WIDTH = 100;

        public override WebHeaderStyle Style => WebHeaderStyle.SaaSProfessional;

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
            // Draw professional light blue background
            using (var brush = new SolidBrush(Color.FromArgb(245, 248, 252)))
            {
                g.FillRectangle(brush, bounds);
            }

            int x = bounds.Left + PADDING;

            // Draw Logo as circle
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogoCircle(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }

            // Draw app name
            using (var brush = new SolidBrush(Color.FromArgb(40, 60, 100)))
            using (var font = new Font("Arial", 10, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("Dashboard", font, brush, new Rectangle(x, bounds.Top, 80, bounds.Height), sf);
            }
            x += 85;

            // Draw search box if requested
            if (showSearchBox)
            {
                var searchRect = new Rectangle(x, bounds.Top + (bounds.Height - 32) / 2, 220, 32);
                DrawSearchBox(g, searchRect, searchText, theme);
                x += 230;
            }

            // Draw Tabs with blue accent
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        x,
                        bounds.Top + 10,
                        (int)textSize.Width + 18,
                        bounds.Height - 20);

                    tab.Bounds = tabBounds;

                    // Draw tab background if active
                    if (tab.IsActive)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(230, 240, 255)))
                        {
                            g.FillRectangle(brush, tabBounds);
                        }
                    }

                    // Draw tab text
                    Color textColor = tab.IsActive ? Color.FromArgb(40, 120, 200) : Color.FromArgb(100, 100, 120);
                    DrawTab(g, tabBounds, tab, tab.IsActive, tab.IsHovered, textColor, tabFont);

                    // Draw blue underline for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(Color.FromArgb(40, 120, 200), 3))
                        {
                            g.DrawLine(pen, tabBounds.Left + 2, tabBounds.Bottom - 3, tabBounds.Right - 2, tabBounds.Bottom - 3);
                        }
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
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 34) / 2, BUTTON_WIDTH, 34);
                    btn.Bounds = btnBounds;

                    // Draw button with professional styling
                    Color bgColor = btn.IsHovered ? Color.FromArgb(40, 120, 200) : Color.White;
                    Color textColor = btn.IsHovered ? Color.White : Color.FromArgb(80, 100, 140);
                    Color borderColor = Color.FromArgb(200, 210, 230);

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(brush, btnBounds);
                    }

                    using (var pen = new Pen(borderColor, 1))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        if (!string.IsNullOrEmpty(btn.ImagePath))
                        {
                            var avatarRect = new Rectangle(btnBounds.Left + 6, btnBounds.Top + 3, 28, 28);
                            DrawLogoCircle(g, avatarRect, btn.ImagePath, theme);
                            // add notification badge to right-most avatar
                            if (i == buttons.Count - 1)
                            {
                                var badgeRect = new Rectangle(avatarRect.Right - 8, avatarRect.Top - 4, 12, 12);
                                using (var bBrush = new SolidBrush(Color.FromArgb(220, 40, 40)))
                                using (var bPen = new Pen(Color.White, 1))
                                {
                                    g.FillEllipse(bBrush, badgeRect);
                                    g.DrawEllipse(bPen, badgeRect);
                                }
                            }
                        }
                        else
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    buttonX -= 8;
                }
            }

            // Draw top blue accent line
            using (var pen = new Pen(Color.FromArgb(40, 120, 200), 3))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }

            // Draw subtle bottom border
            using (var pen = new Pen(Color.FromArgb(220, 225, 235), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
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
