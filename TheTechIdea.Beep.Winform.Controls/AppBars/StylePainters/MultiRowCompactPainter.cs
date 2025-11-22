using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Multi-Row Compact Painter
    /// Multiple rows with category and secondary navigation for complex sites
    /// Features: Two-row header, main categories top row, secondary nav bottom row, icons emphasized
    /// </summary>
    public class MultiRowCompactPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 10;
        private const int TAB_SPACING = 12;
        private const int LOGO_WIDTH = 35;
        private const int BUTTON_WIDTH = 90;

        public override WebHeaderStyle Style => WebHeaderStyle.MultiRowCompact;

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
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, bounds);
            }

            // Split header into rows
            int topRowHeight = bounds.Height / 2;
            var topRowBounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width, topRowHeight);
            var bottomRowBounds = new Rectangle(bounds.Left, bounds.Top + topRowHeight, bounds.Width, bounds.Height - topRowHeight);

            // --- TOP ROW: Logo + Main Categories ---
            int x = topRowBounds.Left + PADDING;

            // Draw Logo in top row (pill)
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, topRowBounds.Top + (topRowHeight - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogoPill(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }

            // Draw main category tabs in top row
            if (tabs != null && tabs.Count > 0)
            {
                int mainTabCount = Math.Min((tabs.Count + 1) / 2, tabs.Count);
                for (int i = 0; i < mainTabCount; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        x,
                        topRowBounds.Top + 4,
                        (int)textSize.Width + 12,
                        topRowHeight - 8);

                    tab.Bounds = tabBounds;

                    // Draw main category tab
                    if (tab.IsActive)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(240, 245, 250)))
                        {
                            g.FillRectangle(brush, tabBounds);
                        }
                    }

                    Color textColor = tab.IsActive ? Color.FromArgb(40, 100, 180) : Color.FromArgb(80, 80, 100);
                    DrawTab(g, tabBounds, tab, tab.IsActive, tab.IsHovered, textColor, tabFont);

                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(Color.FromArgb(40, 100, 180), 2))
                        {
                            g.DrawLine(pen, tabBounds.Left, tabBounds.Bottom, tabBounds.Right, tabBounds.Bottom);
                        }
                    }

                    x += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw action buttons in top right
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = topRowBounds.Right - PADDING;
                for (int i = Math.Min(2, buttons.Count) - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    buttonX -= BUTTON_WIDTH;
                    var btnBounds = new Rectangle(buttonX, topRowBounds.Top + (topRowHeight - 30) / 2, BUTTON_WIDTH, 30);
                    btn.Bounds = btnBounds;

                    Color bgColor = btn.IsHovered ? Color.FromArgb(40, 100, 180) : Color.FromArgb(245, 245, 250);
                    Color textColor = btn.IsHovered ? Color.White : Color.FromArgb(80, 100, 140);

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(brush, btnBounds);
                    }

                    using (var pen = new Pen(Color.FromArgb(200, 210, 230), 1))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                    }

                    buttonX -= 4;
                }
            }

            // Draw separator between rows
            using (var pen = new Pen(Color.FromArgb(230, 230, 240), 1))
            {
                g.DrawLine(pen, bottomRowBounds.Left, bottomRowBounds.Top, bottomRowBounds.Right, bottomRowBounds.Top);
            }

            // --- BOTTOM ROW: Secondary Navigation ---
            x = bottomRowBounds.Left + PADDING;

            // Draw secondary category tabs
            if (tabs != null && tabs.Count > 1)
            {
                int secondaryStartIndex = (tabs.Count + 1) / 2;
                for (int i = secondaryStartIndex; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        x,
                        bottomRowBounds.Top + 2,
                        (int)textSize.Width + 10,
                        bottomRowBounds.Height - 4);

                    // Adjust tab position to bottom row coordinates
                    tab.Bounds = tabBounds;

                    // Draw secondary tab (smaller font, subtle styling)
                    Color textColor = Color.FromArgb(100, 100, 120);
                    using (var brush = new SolidBrush(textColor))
                    using (var smallFont = new Font(tabFont.FontFamily, tabFont.Size - 1))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, smallFont, brush, tabBounds, sf);
                    }

                    x += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw bottom border and secondary row background
            using (var bottomBrush = new SolidBrush(Color.FromArgb(250, 250, 252)))
            {
                g.FillRectangle(bottomBrush, bottomRowBounds);
            }

            using (var pen = new Pen(Color.FromArgb(230, 230, 240), 1))
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
