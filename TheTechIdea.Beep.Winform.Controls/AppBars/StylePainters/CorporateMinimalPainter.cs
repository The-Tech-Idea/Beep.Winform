using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Corporate Minimal Painter
    /// DISTINCT STYLE: Professional enterprise design with subtle gray tones and
    /// understated elegance. Suitable for B2B and corporate sites.
    /// Features: Muted colors, thin borders, professional typography, subtle shadows
    /// </summary>
    public class CorporateMinimalPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 20;
        private const int TAB_SPACING = 24;
        private const int LOGO_SIZE = 34;
        private const int BUTTON_HEIGHT = 36;
        private const int BUTTON_MIN_WIDTH = 100;

        // Style-specific colors (corporate gray palette)
        private static readonly Color AccentSlate = Color.FromArgb(71, 85, 105);
        private static readonly Color AccentSlateHover = Color.FromArgb(51, 65, 85);
        private static readonly Color TextPrimary = Color.FromArgb(30, 41, 59);
        private static readonly Color TextSecondary = Color.FromArgb(100, 116, 139);
        private static readonly Color BorderColor = Color.FromArgb(226, 232, 240);
        private static readonly Color BgHover = Color.FromArgb(248, 250, 252);

        public override WebHeaderStyle Style => WebHeaderStyle.CorporateMinimal;

        public override void PaintHeader(
            Graphics g,
            Rectangle bounds,
            IBeepTheme theme,
            WebHeaderColors colors,
            List<WebHeaderTab> tabs,
            List<WebHeaderActionButton> buttons,
            int selectedTabIndex,
            string logoImagePath,
            string logoText,
            bool showLogo,
            bool showSearchBox,
            string searchText,
            Font tabFont,
            Font buttonFont,
            bool skipBackground = false)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clean white background (skip if transparent background)
            Color bgColor = colors?.BackgroundColor ?? Color.White;
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO ===
            if (showLogo)
            {
                // Accent line before logo
                using (var pen = new Pen(AccentSlate, 2))
                {
                    g.DrawLine(pen, bounds.Left + 8, bounds.Top + 14, bounds.Left + 8, bounds.Bottom - 14);
                }

                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    DrawLogoPill(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 12;
                }

                // Company name - professional typography
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "CORP";
                using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
                {
                    // Letter spacing simulation
                    float spacing = 2f;
                    float currentX = x;
                    foreach (char c in brandText.ToUpper())
                    {
                        string ch = c.ToString();
                        var size = g.MeasureString(ch, font);
                        using (var brush = new SolidBrush(TextPrimary))
                        {
                            g.DrawString(ch, font, brush, currentX, centerY - size.Height / 2);
                        }
                        currentX += size.Width - 4 + spacing;
                    }
                    x = (int)currentX + 30;
                }
            }

            // === TABS (centered, professional) ===
            if (tabs != null && tabs.Count > 0)
            {
                // Calculate total width for centering
                int totalTabsWidth = 0;
                foreach (var tab in tabs)
                {
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    totalTabsWidth += (int)textSize.Width + 20 + TAB_SPACING;
                }
                totalTabsWidth -= TAB_SPACING;

                // Center tabs
                int tabStartX = bounds.Left + (bounds.Width - totalTabsWidth) / 2;
                tabStartX = Math.Max(tabStartX, x);

                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 20;
                    var tabBounds = new Rectangle(tabStartX, bounds.Top + 10, tabWidth, bounds.Height - 20);
                    tab.Bounds = tabBounds;

                    // Hover background
                    if (tab.IsHovered && !tab.IsActive)
                    {
                        using (var brush = new SolidBrush(BgHover))
                        {
                            g.FillRectangle(brush, tabBounds);
                        }
                    }

                    // Tab text
                    Color textColor = tab.IsActive ? AccentSlate : (tab.IsHovered ? TextPrimary : TextSecondary);
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, tabFont, brush, tabBounds, sf);
                    }

                    // Thin underline for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(AccentSlate, 1.5f))
                        {
                            g.DrawLine(pen, tabBounds.Left + 4, tabBounds.Bottom - 4, tabBounds.Right - 4, tabBounds.Bottom - 4);
                        }
                    }

                    tabStartX += tabWidth + TAB_SPACING;
                }
            }

            // === BUTTONS (right side) ===
            int rightX = bounds.Right - PADDING;
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    int btnWidth = Math.Max(BUTTON_MIN_WIDTH, btn.Width > 0 ? btn.Width : BUTTON_MIN_WIDTH);
                    rightX -= btnWidth;
                    var btnBounds = new Rectangle(rightX, centerY - BUTTON_HEIGHT / 2, btnWidth, BUTTON_HEIGHT);
                    btn.Bounds = btnBounds;

                    bool isCta = btn.Style == WebHeaderButtonStyle.Solid || i == buttons.Count - 1;

                    if (isCta)
                    {
                        // Solid corporate button
                        Color bgCol = btn.IsHovered ? AccentSlateHover : AccentSlate;
                        using (var brush = new SolidBrush(bgCol))
                        {
                            g.FillRectangle(brush, btnBounds);
                        }
                        using (var brush = new SolidBrush(Color.White))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }
                    else
                    {
                        // Outline button
                        if (btn.IsHovered)
                        {
                            using (var brush = new SolidBrush(BgHover))
                            {
                                g.FillRectangle(brush, btnBounds);
                            }
                        }

                        using (var pen = new Pen(BorderColor, 1))
                        {
                            g.DrawRectangle(pen, btnBounds);
                        }

                        Color textCol = btn.IsHovered ? TextPrimary : TextSecondary;
                        using (var brush = new SolidBrush(textCol))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    rightX -= 12;
                }
            }

            // Bottom border
            using (var pen = new Pen(BorderColor, 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
        }

        public override Rectangle GetTabBounds(int tabIndex, Rectangle headerBounds, List<WebHeaderTab> tabs)
        {
            if (tabIndex >= 0 && tabIndex < tabs?.Count)
                return tabs[tabIndex].Bounds;
            return Rectangle.Empty;
        }

        public override Rectangle GetButtonBounds(int buttonIndex, Rectangle headerBounds, List<WebHeaderActionButton> buttons)
        {
            if (buttonIndex >= 0 && buttonIndex < buttons?.Count)
                return buttons[buttonIndex].Bounds;
            return Rectangle.Empty;
        }

        public override int GetHitElement(Point pt, Rectangle headerBounds, List<WebHeaderTab> tabs, List<WebHeaderActionButton> buttons)
        {
            if (tabs != null)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (tabs[i].Bounds.Contains(pt))
                        return i;
                }
            }

            if (buttons != null)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (buttons[i].Bounds.Contains(pt))
                        return -(i + 1);
                }
            }

            return -1;
        }
    }
}
