using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Material Design 3 Painter
    /// Google Material Design 3 pattern with elevation and dynamic colors
    /// Features: Elevation effect, rounded elements, accent color, ripple-ready layout
    /// </summary>
    public class MaterialDesign3Painter : WebHeaderStylePainterBase
    {
        private const int PADDING = 12;
        private const int TAB_SPACING = 14;
        private const int LOGO_WIDTH = 40;
        private const int BUTTON_WIDTH = 105;

        public override WebHeaderStyle Style => WebHeaderStyle.MaterialDesign3;

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
            // Draw Material background color
            using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
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

            // Draw Tabs with Material styling and subtle elevation shadow
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        x,
                        bounds.Top + 12,
                        (int)textSize.Width + 16,
                        bounds.Height - 24);

                    tab.Bounds = tabBounds;

                    // Draw Material indicator (bottom accent)
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(Color.FromArgb(100, 150, 255), 3))
                        {
                            g.DrawLine(pen, tabBounds.Left, tabBounds.Bottom + 1, tabBounds.Right, tabBounds.Bottom + 1);
                        }
                    }

                    // Draw tab text
                    Color textColor = tab.IsActive ? Color.FromArgb(100, 150, 255) : Color.FromArgb(100, 100, 120);
                    DrawTab(g, tabBounds, tab, tab.IsActive, tab.IsHovered, textColor, tabFont);

                    x += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw Buttons (Right side) - Material elevated buttons
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = bounds.Right - PADDING;
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    buttonX -= BUTTON_WIDTH;
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 36) / 2, BUTTON_WIDTH, 36);
                    btn.Bounds = btnBounds;

                    // Draw Material button with elevation
                    Color bgColor = btn.IsHovered ? Color.FromArgb(100, 150, 255) : Color.White;
                    Color textColor = btn.IsHovered ? Color.White : Color.FromArgb(100, 150, 255);

                    // Draw shadow for elevation
                    using (var brush = new SolidBrush(Color.FromArgb(20, 100, 150, 255)))
                    {
                        g.FillRectangle(brush, new Rectangle(btnBounds.X + 1, btnBounds.Y + 1, btnBounds.Width, btnBounds.Height));
                    }

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(brush, btnBounds);
                    }

                    using (var pen = new Pen(Color.FromArgb(100, 150, 255), 1))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                    }

                    buttonX -= 6;
                }
            }

            // Draw Material elevation shadow
            using (var pen = new Pen(Color.FromArgb(230, 230, 235), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
            using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            {
                g.FillRectangle(shadowBrush, new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 2));
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
