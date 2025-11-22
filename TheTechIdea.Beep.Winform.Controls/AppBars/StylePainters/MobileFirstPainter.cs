using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Mobile-First Painter
    /// Responsive header optimized for smaller screens with icon-focused buttons
    /// Features: Compact layout, icon emphasis, minimal text, touch-friendly sizes
    /// </summary>
    public class MobileFirstPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 8;
        private const int TAB_SPACING = 18;
        private const int LOGO_WIDTH = 36;
        private const int BUTTON_WIDTH = 44;

        public override WebHeaderStyle Style => WebHeaderStyle.MobileFirst;

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
            // Draw clean background
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, bounds);
            }

            int x = bounds.Left + PADDING;

            // Draw menu icon on far left (for mobile) and then logo
            var menuRect = new Rectangle(x, bounds.Top + (bounds.Height - 28) / 2, 28, 28);
            try
            {
                StyledImagePainter.PaintInPill(g, menuRect.X, menuRect.Y, menuRect.Width, menuRect.Height, SvgsUI.Menu);
            }
            catch { /* fallback not needed */ }
            x += menuRect.Width + PADDING;

            // Draw Logo (circle) for mobile compact header
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                DrawLogoCircle(g, logoBounds, logoImagePath, theme);
                x += LOGO_WIDTH + PADDING;
            }

            // Draw Tabs - minimal text, icon-based
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text.Substring(0, Math.Min(1, tab.Text.Length)), tabFont);
                    var tabBounds = new Rectangle(
                        x,
                        bounds.Top + 6,
                        36,
                        bounds.Height - 12);

                    tab.Bounds = tabBounds;

                    // Draw tab background if active
                    if (tab.IsActive || tab.IsHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(245, 245, 250)))
                        {
                            g.FillRectangle(brush, tabBounds);
                        }
                    }

                    // Draw tab icon/text (abbreviated)
                    Color textColor = tab.IsActive ? Color.FromArgb(50, 120, 200) : Color.FromArgb(120, 120, 140);
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text[0].ToString().ToUpper(), tabFont, brush, tabBounds, sf);
                    }

                    // Draw indicator for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(Color.FromArgb(50, 120, 200), 2))
                        {
                            g.DrawLine(pen, tabBounds.Left + 2, tabBounds.Bottom - 2, tabBounds.Right - 2, tabBounds.Bottom - 2);
                        }
                    }

                    x += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw Buttons (Right side) - Icon buttons, touch-friendly
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = bounds.Right - PADDING;
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    buttonX -= BUTTON_WIDTH;
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 36) / 2, BUTTON_WIDTH, 36);
                    btn.Bounds = btnBounds;

                    // Draw icon button
                    if (btn.IsHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(240, 245, 250)))
                        {
                            g.FillRectangle(brush, btnBounds);
                        }
                    }

                    using (var pen = new Pen(Color.FromArgb(220, 225, 235), 1))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    // Draw icon/initial
                    // Draw icon/initial or svg
                    Color iconColor = btn.IsHovered ? Color.FromArgb(50, 120, 200) : Color.FromArgb(100, 100, 130);
                    using (var brush = new SolidBrush(iconColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        if (!string.IsNullOrEmpty(btn.ImagePath))
                        {
                            var iconRect = new Rectangle(btnBounds.Left + 2, btnBounds.Top + 2, btnBounds.Width - 4, btnBounds.Height - 4);
                            DrawLogoCircle(g, iconRect, btn.ImagePath, theme);
                        }
                        else
                        {
                            g.DrawString(btn.Text[0].ToString().ToUpper(), buttonFont, brush, btnBounds, sf);
                        }
                    }

                    buttonX -= 3;
                }
            }

            // Draw bottom border
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
