using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Corporate Minimal Painter
    /// Clean white design with subtle styling for corporate sites
    /// Features: Understated branding, professional tabs, accent line, subtle shadows
    /// </summary>
    public class CorporateMinimalPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 10;
        private const int TAB_SPACING = 22;
        private const int LOGO_WIDTH = 38;
        private const int BUTTON_WIDTH = 98;

        public override WebHeaderStyle Style => WebHeaderStyle.CorporateMinimal;

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

            // Draw Logo (pill)
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogoPill(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }

            // Draw a thin accent line near logo
            using (var accent = new Pen(Color.FromArgb(100, 140, 220), 2))
            {
                g.DrawLine(accent, bounds.Left + (LOGO_WIDTH / 2), bounds.Top + 10, bounds.Left + (LOGO_WIDTH / 2), bounds.Bottom - 10);
            }

            // Draw company initials
            using (var brush = new SolidBrush(Color.FromArgb(50, 50, 80)))
            using (var font = new Font("Arial", 8, FontStyle.Regular))
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("CORP", font, brush, new Rectangle(x, bounds.Top, 70, bounds.Height), sf);
            }
            x += 75;

            // Draw Tabs centered
            if (tabs != null && tabs.Count > 0)
            {
                // measure total width
                int tabsTotalWidth = 0;
                for (int i = 0; i < tabs.Count; i++)
                {
                    var textSize = g.MeasureString(tabs[i].Text, tabFont);
                    tabsTotalWidth += (int)textSize.Width + 14 + TAB_SPACING;
                }
                tabsTotalWidth -= TAB_SPACING; // remove last spacing
                int tabStartX = bounds.Left + (bounds.Width - tabsTotalWidth) / 2;

                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        tabStartX,
                        bounds.Top + 10,
                        (int)textSize.Width + 14,
                        bounds.Height - 20);

                    tab.Bounds = tabBounds;

                    // Draw tab text
                    Color textColor = tab.IsActive ? Color.FromArgb(50, 100, 180) : Color.FromArgb(100, 100, 120);
                    DrawTab(g, tabBounds, tab, tab.IsActive, tab.IsHovered, textColor, tabFont);

                    // Draw simple underline for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(Color.FromArgb(50, 100, 180), 1.5f))
                        {
                            g.DrawLine(pen, tabBounds.Left, tabBounds.Bottom, tabBounds.Right, tabBounds.Bottom);
                        }
                    }

                    tabStartX += tabBounds.Width + TAB_SPACING;
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

                    // Draw corporate button
                    if (btn.IsHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(240, 245, 250)))
                        {
                            g.FillRectangle(brush, btnBounds);
                        }
                    }

                    using (var pen = new Pen(Color.FromArgb(200, 210, 230), 1))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    using (var brush = new SolidBrush(Color.FromArgb(70, 80, 110)))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                    }

                    buttonX -= 6;
                }
            }

            // Draw subtle bottom border
            using (var pen = new Pen(Color.FromArgb(230, 235, 245), 1))
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
