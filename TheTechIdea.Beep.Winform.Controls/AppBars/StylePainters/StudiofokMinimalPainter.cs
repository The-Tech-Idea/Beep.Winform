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
    /// Studiofok Minimal Painter
    /// DISTINCT STYLE: Ultra-clean Scandinavian design with maximum whitespace.
    /// No borders, subtle typography, thin underline indicators.
    /// Features: Minimal logo, wide letter-spacing, thin underline, ghost buttons
    /// </summary>
    public class StudiofokMinimalPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 24;
        private const int TAB_SPACING = 32;
        private const int LOGO_SIZE = 28;
        private const int BUTTON_HEIGHT = 36;
        private const int BUTTON_MIN_WIDTH = 100;

        // Style-specific colors (minimal monochrome)
        private static readonly Color TextPrimary = Color.FromArgb(20, 20, 20);
        private static readonly Color TextSecondary = Color.FromArgb(140, 140, 140);
        private static readonly Color TextHover = Color.FromArgb(80, 80, 80);
        private static readonly Color AccentBlack = Color.FromArgb(20, 20, 20);
        private static readonly Color BorderSubtle = Color.FromArgb(235, 235, 235);

        public override WebHeaderStyle Style => WebHeaderStyle.StudiofokMinimal;

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

            // Pure white background (skip if transparent background)
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

            // === LOGO (minimal) ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    DrawLogo(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 12;
                }

                // Brand text - uppercase, letter-spaced
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText.ToUpper() : "STUDIO";
                using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
                {
                    // Draw with letter spacing simulation
                    float letterSpacing = 3f;
                    float currentX = x;
                    foreach (char c in brandText)
                    {
                        string ch = c.ToString();
                        var size = g.MeasureString(ch, font);
                        using (var brush = new SolidBrush(TextPrimary))
                        {
                            g.DrawString(ch, font, brush, currentX, centerY - size.Height / 2);
                        }
                        currentX += size.Width - 4 + letterSpacing;
                    }
                    x = (int)currentX + 40;
                }
            }

            // Thin vertical divider
            using (var pen = new Pen(BorderSubtle, 1))
            {
                g.DrawLine(pen, x, bounds.Top + 16, x, bounds.Bottom - 16);
            }
            x += 40;

            // === TABS (minimal with thin underline) ===
            if (tabs != null && tabs.Count > 0)
            {
                using (var normalFont = new Font(tabFont.FontFamily, tabFont.Size - 1, FontStyle.Regular))
                {
                    for (int i = 0; i < tabs.Count; i++)
                    {
                        var tab = tabs[i];
                        var textSize = g.MeasureString(tab.Text, normalFont);
                        int tabWidth = (int)textSize.Width + 8;
                        var tabBounds = new Rectangle(x, bounds.Top + 8, tabWidth, bounds.Height - 16);
                        tab.Bounds = tabBounds;

                        // Tab text - subtle color change on hover
                        Color textColor = tab.IsActive ? TextPrimary : (tab.IsHovered ? TextHover : TextSecondary);
                        using (var brush = new SolidBrush(textColor))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(tab.Text, normalFont, brush, tabBounds, sf);
                        }

                        // Thin underline for active (1px)
                        if (tab.IsActive)
                        {
                            using (var pen = new Pen(AccentBlack, 1))
                            {
                                g.DrawLine(pen, tabBounds.Left, tabBounds.Bottom - 8, tabBounds.Right, tabBounds.Bottom - 8);
                            }
                        }

                        x += tabWidth + TAB_SPACING;
                    }
                }
            }

            // === BUTTONS (ghost style, right side) ===
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

                    // Minimal button styling
                    if (isCta)
                    {
                        // Solid black button
                        Color bgCol = btn.IsHovered ? Color.FromArgb(40, 40, 40) : AccentBlack;
                        using (var brush = new SolidBrush(bgCol))
                        {
                            g.FillRectangle(brush, btnBounds);
                        }
                        using (var brush = new SolidBrush(Color.White))
                        using (var font = new Font(buttonFont.FontFamily, buttonFont.Size - 1, FontStyle.Regular))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, font, brush, btnBounds, sf);
                        }
                    }
                    else
                    {
                        // Ghost button - text only, underline on hover
                        Color textColor = btn.IsHovered ? TextPrimary : TextSecondary;
                        using (var brush = new SolidBrush(textColor))
                        using (var font = new Font(buttonFont.FontFamily, buttonFont.Size - 1, FontStyle.Regular))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, font, brush, btnBounds, sf);
                        }
                        
                        // Underline on hover
                        if (btn.IsHovered)
                        {
                            var textSize = g.MeasureString(btn.Text, buttonFont);
                            int underlineX = btnBounds.X + (btnBounds.Width - (int)textSize.Width) / 2;
                            using (var pen = new Pen(TextPrimary, 1))
                            {
                                g.DrawLine(pen, underlineX, btnBounds.Bottom - 10, underlineX + (int)textSize.Width, btnBounds.Bottom - 10);
                            }
                        }
                    }

                    rightX -= 24;
                }
            }

            // No bottom border - true minimal
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
