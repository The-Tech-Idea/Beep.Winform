using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Shoppy Store E-commerce Style 1 Painter
    /// E-commerce minimal style with category dropdown
    /// Features: Logo left, centered category tabs, search center, cart/account right
    /// </summary>
    public class ShoppyStore1Painter : WebHeaderStylePainterBase
    {
        private const int PADDING = 12;
        private const int TAB_SPACING = 15;
        private const int LOGO_WIDTH = 40;
        private const int SEARCH_BOX_WIDTH = 200;
        private const int BUTTON_WIDTH = 90;

        public override WebHeaderStyle Style => WebHeaderStyle.ShoppyStore1;

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
            // Draw background
            using (var brush = new SolidBrush(theme?.BackColor ?? Color.White))
            {
                g.FillRectangle(brush, bounds);
            }

            // Draw border
            using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }

            int x = bounds.Left + PADDING;

            // Draw Logo (pill shape)
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogoPill(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }

            // Draw Tabs (Categories) - centered under logo area
            if (tabs != null && tabs.Count > 0)
            {
                // measure total width and center tabs area
                int tabsTotalWidth = 0;
                for (int i = 0; i < tabs.Count; i++)
                {
                    var textSizeTmp = g.MeasureString(tabs[i].Text, tabFont);
                    tabsTotalWidth += (int)textSizeTmp.Width + 16 + TAB_SPACING;
                }
                tabsTotalWidth -= TAB_SPACING;
                int tabStartX = bounds.Left + (bounds.Width - tabsTotalWidth) / 2;
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        (int)tabStartX,
                        bounds.Top + 5,
                        (int)textSize.Width + 16,
                        bounds.Height - 10);

                    tab.Bounds = tabBounds;

                    // Draw tab background if active
                    if (tab.IsActive)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                        {
                            g.FillRectangle(brush, tabBounds);
                        }
                    }

                    // Draw tab text with color and active pill style
                    DrawTab(g, tabBounds, tab, tab.IsActive, tab.IsHovered,
                        tab.IsActive ? Color.FromArgb(255, 140, 0) : Color.FromArgb(100, 100, 100),
                        tabFont);

                    tabStartX += tabBounds.Width + TAB_SPACING;
                }

                x = (int)tabStartX + PADDING;
            }

            // Draw Search Box (center-right) if enabled
            if (showSearchBox)
            {
                int searchW = SEARCH_BOX_WIDTH;
                var searchRect = new Rectangle(bounds.Right - PADDING - BUTTON_WIDTH - PADDING - searchW, bounds.Top + (bounds.Height - 28) / 2, searchW, 28);
                DrawSearchBox(g, searchRect, searchText, theme);
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

                    // Draw button
                    // Draw call-to-action as colored fill if styled as solid
                    if (btn.Style == WebHeaderButtonStyle.Solid)
                    {
                        DrawButton(g, btnBounds, btn, btn.IsHovered, Color.White, Color.FromArgb(255, 140, 0), buttonFont);
                    }
                    else
                    {
                        DrawButton(g, btnBounds, btn, btn.IsHovered, theme?.ForeColor ?? Color.Black, btn.IsHovered ? Color.FromArgb(240, 240, 240) : Color.White, buttonFont);
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
            // Check tabs (return positive index)
            if (tabs != null)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (HitTestRect(pt, tabs[i].Bounds))
                        return i;
                }
            }

            // Check buttons (return negative index - 1)
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
