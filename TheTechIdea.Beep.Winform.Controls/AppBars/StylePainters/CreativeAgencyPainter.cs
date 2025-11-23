using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Creative Agency Painter
    /// Bold typography with gradient backgrounds for creative/portfolio sites
    /// Features: Large logo area, bold typography, gradient accents, artistic spacing
    /// </summary>
    public class CreativeAgencyPainter : WebHeaderStylePainterBase
    {
        private const int PADDING = 16;
        private const int TAB_SPACING = 30;
        private const int LOGO_WIDTH = 50;
        private const int BUTTON_WIDTH = 115;

        public override WebHeaderStyle Style => WebHeaderStyle.CreativeAgency;

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
            // Draw subtle background gradient for creative flair
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(bounds, Color.FromArgb(255, 255, 255), Color.FromArgb(250, 245, 240), 90f))
            {
                g.FillRectangle(brush, bounds);
            }

            int x = bounds.Left + PADDING;

            // Draw Logo - larger for creative agencies (pill)
            if (showLogo && !string.IsNullOrEmpty(logoImagePath))
            {
                var logoBounds = new Rectangle(x, bounds.Top + (bounds.Height - LOGO_WIDTH) / 2, LOGO_WIDTH, LOGO_WIDTH);
                // Tinted pill logo for creative effect
                try
                {
                    var tint = Color.FromArgb(255, 105, 105);
                    StyledImagePainter.PaintWithTint(g, logoBounds, logoImagePath, tint, 0.65f, 8);
                }
                catch
                {
                    DrawLogoPill(g, logoBounds, logoImagePath, theme);
                }
                x += LOGO_WIDTH + PADDING;
            }

            // Draw agency name in larger bold font
            using (var brush = new SolidBrush(Color.FromArgb(10, 10, 20)))
            using (var font = new Font("Segoe UI", 13, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("CREATIVE", font, brush, new Rectangle(x, bounds.Top, 100, bounds.Height), sf);
            }
            x += 105;

            // Draw Tabs with artistic spacing
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    var tabBounds = new Rectangle(
                        x,
                        bounds.Top + 12,
                        (int)textSize.Width + 8,
                        bounds.Height - 24);

                    tab.Bounds = tabBounds;

                    // Draw bold tab text
                    Color textColor = tab.IsActive ? Color.FromArgb(255, 100, 100) : Color.FromArgb(80, 80, 100);
                    using (var brush = new SolidBrush(textColor))
                    using (var boldFont = new Font(tabFont.FontFamily, tabFont.Size, FontStyle.Bold))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, boldFont, brush, tabBounds, sf);
                    }

                    // Draw artistic underline for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(Color.FromArgb(255, 100, 100), 2.5f))
                        {
                            g.DrawLine(pen, tabBounds.Left, tabBounds.Bottom + 2, tabBounds.Right, tabBounds.Bottom + 2);
                        }
                    }

                    x += tabBounds.Width + TAB_SPACING;
                }
            }

            // Draw Buttons (Right side) - Large and prominent
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = bounds.Right - PADDING;
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    buttonX -= BUTTON_WIDTH;
                    var btnBounds = new Rectangle(buttonX, bounds.Top + (bounds.Height - 38) / 2, BUTTON_WIDTH, 38);
                    btn.Bounds = btnBounds;

                    // Draw creative button - gradient effect with bold text
                    Color bgColor = btn.IsHovered ? Color.FromArgb(255, 100, 100) : Color.White;
                    Color textColor = btn.IsHovered ? Color.White : Color.FromArgb(255, 100, 100);

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(brush, btnBounds);
                    }

                    using (var pen = new Pen(Color.FromArgb(255, 100, 100), 2))
                    {
                        g.DrawRectangle(pen, btnBounds);
                    }

                    using (var brush = new SolidBrush(textColor))
                    using (var boldFont = new Font(buttonFont.FontFamily, buttonFont.Size, FontStyle.Bold))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(btn.Text, boldFont, brush, btnBounds, sf);
                    }

                    buttonX -= 10;
                }
            }

            // Draw artistic top accent line
            using (var pen = new Pen(Color.FromArgb(255, 100, 100), 2))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top + 5, bounds.Left + 80, bounds.Top + 5);
            }

            // Draw subtle bottom border
            using (var pen = new Pen(Color.FromArgb(240, 240, 245), 1))
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
