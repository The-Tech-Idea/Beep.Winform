using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Shoppy Store Style 2 Painter
    /// E-commerce centered layout with emphasis on search
    /// Features: Centered tabs, prominent search box, buttons at far right, minimal logo
    /// </summary>
    public class ShoppyStore2Painter : WebHeaderStylePainterBase
    {
        private const int PADDING = 10;
        private const int TAB_SPACING = 20;
        private const int LOGO_WIDTH = 35;
        private const int SEARCH_BOX_WIDTH = 280;
        private const int BUTTON_WIDTH = 85;

        public override WebHeaderStyle Style => WebHeaderStyle.ShoppyStore2;

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

            int x = bounds.Left + PADDING;

            // Draw Logo (small, left side)
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogo(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }

            // Calculate center position for tabs
            int centerX = (bounds.Left + bounds.Right) / 2;
            int tabsTotalWidth = 0;

            if (tabs != null && tabs.Count > 0)
            {
                // Measure total width needed for tabs
                foreach (var tab in tabs)
                {
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    tabsTotalWidth += (int)textSize.Width + 20 + TAB_SPACING;
                }

                int tabStartX = centerX - tabsTotalWidth / 2;

                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        tabStartX,
                        bounds.Top + 10,
                        (int)textSize.Width + 20,
                        bounds.Height - 20);

                    tab.Bounds = tabBounds;

                    // Draw tab with underline for active
                    DrawTab(g, tabBounds, tab, tab.IsActive, tab.IsHovered,
                        tab.IsActive ? (theme?.ForeColor ?? Color.Black) : Color.FromArgb(80, 80, 80),
                        tabFont);

                    // Draw underline for active tab
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(theme?.ForeColor ?? Color.Black, 2))
                        {
                            g.DrawLine(pen, tabBounds.Left, tabBounds.Bottom - 2, tabBounds.Right, tabBounds.Bottom - 2);
                        }
                    }

                    tabStartX += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw central search box (if enabled)
            if (showSearchBox)
            {
                int searchW = SEARCH_BOX_WIDTH;
                var searchRect = new Rectangle(centerX - searchW/2, bounds.Top + (bounds.Height - 32) / 2, searchW, 32);
                DrawSearchBox(g, searchRect, searchText, theme);
            }

            // Draw Buttons (Far right)
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = bounds.Right - PADDING;
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    int width = Math.Min(BUTTON_WIDTH, btn.Width > 0 ? btn.Width : BUTTON_WIDTH);
                    buttonX -= width;
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 36) / 2, width, 36);
                    btn.Bounds = btnBounds;

                    // Draw button with gradient effect for hover
                    Color bgColor = btn.IsHovered ? Color.FromArgb(50, 100, 200) : Color.FromArgb(245, 245, 245);
                    Color textColor = btn.IsHovered ? Color.White : (theme?.ForeColor ?? Color.Black);

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(brush, btnBounds);
                    }

                    using (var pen = new Pen(bgColor, 1))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        if (!string.IsNullOrEmpty(btn.ImagePath))
                        {
                            var iconRect = new Rectangle(btnBounds.Left + 6, btnBounds.Top + (btnBounds.Height - 20) / 2, 20, 20);
                            StyledImagePainter.PaintInCircle(g, iconRect.Left + iconRect.Width/2f, iconRect.Top + iconRect.Height/2f, Math.Min(iconRect.Width, iconRect.Height)/2f, btn.ImagePath);
                            var textRect = new Rectangle(btnBounds.Left + 30, btnBounds.Top, btnBounds.Width - 36, btnBounds.Height);
                            g.DrawString(btn.Text, buttonFont, brush, textRect, sf);
                        }
                        else
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    buttonX -= 5;
                }
            }

            // Draw bottom border
            using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
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
