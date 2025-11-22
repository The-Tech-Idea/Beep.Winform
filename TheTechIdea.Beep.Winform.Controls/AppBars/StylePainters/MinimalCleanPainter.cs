using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Minimal Clean Painter
    /// Ultra-minimal design with text-only navigation for content-focused sites
    /// Features: No icons, text-only tabs, maximum simplicity, light gray palette
    /// </summary>
    public class MinimalCleanPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 6;
        // Much wider tab spacing for minimal clean header
        private const int TAB_SPACING = 40;
        private const int LOGO_WIDTH = 30;
        private const int BUTTON_WIDTH = 80;

        public override WebHeaderStyle Style => WebHeaderStyle.MinimalClean;

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
            // Draw ultra-minimal background (off-white)
            using (var brush = new SolidBrush(Color.FromArgb(252, 252, 254)))
            {
                g.FillRectangle(brush, bounds);
            }

            int x = bounds.Left + PADDING;

            // Draw Logo (very small)
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogo(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }

            // Draw Tabs - text only, spaced for maximum clarity
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        x,
                        bounds.Top + 10,
                        (int)textSize.Width + 4,
                        bounds.Height - 20);

                    tab.Bounds = tabBounds;

                    // Draw tab text only (no background)
                    Color textColor = tab.IsActive ? Color.FromArgb(30, 30, 40) : Color.FromArgb(140, 140, 150);
                    DrawTab(g, tabBounds, tab, tab.IsActive, tab.IsHovered, textColor, tabFont);

                    x += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw Buttons (Right side) - minimal text buttons
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = bounds.Right - PADDING;
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    buttonX -= BUTTON_WIDTH;
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 28) / 2, BUTTON_WIDTH, 28);
                    btn.Bounds = btnBounds;

                    // Draw minimal button - text only or outline depending on style
                    if (btn.Style == WebHeaderButtonStyle.Outline)
                    {
                        using (var pen = new Pen(Color.FromArgb(220, 220, 230), 1))
                        {
                            g.DrawRectangle(pen, btnBounds);
                        }
                    }
                    else if (btn.IsHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(245, 245, 248)))
                        {
                            g.FillRectangle(brush, btnBounds);
                        }
                    }

                    using (var brush = new SolidBrush(btn.IsHovered ? Color.FromArgb(30, 30, 40) : Color.FromArgb(100, 100, 120)))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                    }

                    buttonX -= 4;
                }
            }

            // No borders - ultra minimal
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
