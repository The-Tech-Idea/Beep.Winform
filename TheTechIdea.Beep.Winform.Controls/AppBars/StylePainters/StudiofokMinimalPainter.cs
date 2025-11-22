using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Studiofok Minimal Painter
    /// Clean professional design with left-aligned navigation
    /// Features: Logo + text, minimal tabs, clean typography, subtle styling
    /// </summary>
    public class StudiofokMinimalPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 8;
        private const int TAB_SPACING = 25;
        private const int LOGO_WIDTH = 32;
        private const int BUTTON_WIDTH = 95;

        public override WebHeaderStyle Style => WebHeaderStyle.StudiofokMinimal;

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
            // Draw clean white background
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, bounds);
            }

            int x = bounds.Left + PADDING;
            int y = bounds.Top + (bounds.Height - LOGO_WIDTH) / 2;

            // Draw Logo
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, y, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogo(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING + 8;
            }

            // Draw brand text next to logo
            using (var brush = new SolidBrush(Color.FromArgb(30, 30, 30)))
            using (var font = new Font("Segoe UI", 10, FontStyle.SemiBold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("STUDIOFOK", font, brush, new Rectangle(x, bounds.Top, 80, bounds.Height), sf);
            }

            x += 85;

            // Draw separator line
            using (var pen = new Pen(Color.FromArgb(240, 240, 240), 1))
            {
                g.DrawLine(pen, x + 5, bounds.Top + 8, x + 5, bounds.Bottom - 8);
            }

            x += 15;

            // Draw Tabs (with optional icons)
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int extraForIcon = !string.IsNullOrEmpty(tab.ImagePath) ? 22 : 0;
                    var tabBounds = new Rectangle(
                        x,
                        bounds.Top + 8,
                        (int)textSize.Width + 12 + extraForIcon,
                        bounds.Height - 16);

                    tab.Bounds = tabBounds;

                    // Draw optional icon and text
                    if (!string.IsNullOrEmpty(tab.ImagePath))
                    {
                        var iconRect = new Rectangle(tabBounds.Left + 6, tabBounds.Top + (tabBounds.Height - 18) / 2, 18, 18);
                        DrawLogoCircle(g, iconRect, tab.ImagePath, theme);
                    }

                    Color textColor = tab.IsActive ? Color.FromArgb(30, 30, 30) : Color.FromArgb(120, 120, 120);
                    var textRect = new Rectangle(tabBounds.Left + (string.IsNullOrEmpty(tab.ImagePath) ? 0 : 26), tabBounds.Top, tabBounds.Width - (string.IsNullOrEmpty(tab.ImagePath) ? 0 : 26), tabBounds.Height);
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, tabFont, brush, textRect, sf);
                    }

                    // Draw thin underline for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(Color.FromArgb(30, 30, 30), 1.5f))
                        {
                            g.DrawLine(pen, tabBounds.Left, tabBounds.Bottom + 2, tabBounds.Right, tabBounds.Bottom + 2);
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
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 32) / 2, BUTTON_WIDTH, 32);
                    btn.Bounds = btnBounds;

                    // Draw minimal button
                    if (btn.IsHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
                        {
                            g.FillRectangle(brush, btnBounds);
                        }
                    }

                    using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    using (var brush = new SolidBrush(Color.FromArgb(80, 80, 80)))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                    }

                    buttonX -= 5;
                }
            }

            // Draw subtle top border and a left accent on brand
            using (var pen = new Pen(Color.FromArgb(240, 240, 240), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }

            // Draw subtle bottom border
            using (var pen = new Pen(Color.FromArgb(245, 245, 245), 1))
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
