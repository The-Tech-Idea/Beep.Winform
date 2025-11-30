using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Startup Hero Painter
    /// DISTINCT STYLE: Modern startup/SaaS landing page with clean cream background,
    /// centered navigation, dot indicators, and rounded CTA buttons.
    /// Features: Cream/off-white bg, centered tabs, dot active indicator, pill CTA
    /// </summary>
    public class StartupHeroPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 24;
        private const int TAB_SPACING = 32;
        private const int LOGO_SIZE = 32;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_RADIUS = 20;

        // Style-specific colors (startup hero palette)
        private static readonly Color BgCream = Color.FromArgb(255, 253, 248);
        private static readonly Color AccentBlue = Color.FromArgb(37, 99, 235);
        private static readonly Color AccentBlueHover = Color.FromArgb(29, 78, 216);
        private static readonly Color TextDark = Color.FromArgb(31, 41, 55);
        private static readonly Color TextMuted = Color.FromArgb(107, 114, 128);

        public override WebHeaderStyle Style => WebHeaderStyle.StartupHero;

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

            // Background - cream/off-white (skip if transparent background)
            Color bgColor = colors?.BackgroundColor ?? BgCream;
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Use theme colors or defaults
            Color accent = colors?.AccentColor ?? AccentBlue;
            Color accentHover = colors?.AccentHoverColor ?? AccentBlueHover;
            Color fgColor = colors?.ForegroundColor ?? TextDark;

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    DrawLogo(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 10;
                }

                // Brand text
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "Brand";
                using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    var size = g.MeasureString(brandText, font);
                    using (var brush = new SolidBrush(fgColor))
                    {
                        g.DrawString(brandText, font, brush, x, centerY - size.Height / 2);
                    }
                    x += (int)size.Width + 40;
                }
            }

            // === TABS (centered) ===
            if (tabs != null && tabs.Count > 0)
            {
                // Calculate total width for centering
                int totalTabsWidth = 0;
                var tabWidths = new List<int>();
                foreach (var tab in tabs)
                {
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 16;
                    tabWidths.Add(tabWidth);
                    totalTabsWidth += tabWidth + TAB_SPACING;
                }
                totalTabsWidth -= TAB_SPACING;

                // Calculate buttons width
                int buttonsWidth = CalculateButtonsWidth(buttons, buttonFont);
                int availableWidth = bounds.Width - x - buttonsWidth - PADDING * 2;
                int tabStartX = x + Math.Max(0, (availableWidth - totalTabsWidth) / 2);

                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var tabBounds = new Rectangle(tabStartX, bounds.Top + 8, tabWidths[i], bounds.Height - 16);
                    tab.Bounds = tabBounds;

                    // Tab text
                    Color tabTextColor = tab.IsActive ? accent : (tab.IsHovered ? fgColor : TextMuted);

                    // Hover background
                    if (tab.IsHovered && !tab.IsActive)
                    {
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 6))
                        using (var brush = new SolidBrush(Color.FromArgb(15, accent)))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    using (var brush = new SolidBrush(tabTextColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, tabFont, brush, tabBounds, sf);
                    }

                    // Active indicator - dot
                    if (tab.IsActive)
                    {
                        int dotSize = 5;
                        int dotX = tabBounds.X + (tabBounds.Width - dotSize) / 2;
                        int dotY = tabBounds.Bottom - 10;
                        using (var brush = new SolidBrush(accent))
                        {
                            g.FillEllipse(brush, dotX, dotY, dotSize, dotSize);
                        }
                    }

                    tabStartX += tabWidths[i] + TAB_SPACING;
                }
            }

            // === BUTTONS (right side) ===
            int rightX = bounds.Right - PADDING;
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    var textSize = g.MeasureString(btn.Text, buttonFont);
                    int btnWidth = Math.Max((int)textSize.Width + 32, 100);
                    rightX -= btnWidth;
                    var btnBounds = new Rectangle(rightX, centerY - BUTTON_HEIGHT / 2, btnWidth, BUTTON_HEIGHT);
                    btn.Bounds = btnBounds;

                    bool isCta = btn.Style == WebHeaderButtonStyle.Solid || i == buttons.Count - 1;

                    if (isCta)
                    {
                        // Pill-shaped CTA
                        Color bgCol = btn.IsHovered ? accentHover : accent;
                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, BUTTON_RADIUS))
                        using (var brush = new SolidBrush(bgCol))
                        {
                            g.FillPath(brush, path);
                        }
                        using (var brush = new SolidBrush(Color.White))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }
                    else
                    {
                        // Text button
                        Color textCol = btn.IsHovered ? accent : fgColor;
                        
                        if (btn.Style == WebHeaderButtonStyle.Outline)
                        {
                            using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, BUTTON_RADIUS))
                            using (var pen = new Pen(btn.IsHovered ? accent : Color.FromArgb(209, 213, 219), 1.5f))
                            {
                                g.DrawPath(pen, path);
                            }
                        }
                        else if (btn.IsHovered)
                        {
                            using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, BUTTON_RADIUS))
                            using (var brush = new SolidBrush(Color.FromArgb(10, accent)))
                            {
                                g.FillPath(brush, path);
                            }
                        }

                        using (var brush = new SolidBrush(textCol))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    rightX -= 12;
                }
            }
        }

        private int CalculateButtonsWidth(List<WebHeaderActionButton> buttons, Font buttonFont)
        {
            if (buttons == null || buttons.Count == 0) return 0;

            int width = PADDING;
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                foreach (var btn in buttons)
                {
                    var textSize = g.MeasureString(btn.Text, buttonFont);
                    width += Math.Max((int)textSize.Width + 32, 100) + 12;
                }
            }
            return width;
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
