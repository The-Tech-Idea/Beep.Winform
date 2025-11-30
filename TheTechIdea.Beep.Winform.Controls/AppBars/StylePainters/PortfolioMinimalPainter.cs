using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Portfolio Minimal Painter
    /// DISTINCT STYLE: Personal portfolio/agency with bold typography, wide spacing,
    /// diamond accent, and arrow CTA button. Elegant and artistic.
    /// Features: Bold serif logo, wide tab spacing, diamond icon, arrow in CTA
    /// </summary>
    public class PortfolioMinimalPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 32;
        private const int TAB_SPACING = 44;
        private const int BUTTON_HEIGHT = 44;
        private const int BUTTON_RADIUS = 4;

        // Style-specific colors (portfolio elegant palette)
        private static readonly Color AccentEmerald = Color.FromArgb(16, 185, 129);
        private static readonly Color AccentEmeraldHover = Color.FromArgb(5, 150, 105);
        private static readonly Color TextDark = Color.FromArgb(17, 24, 39);
        private static readonly Color TextMuted = Color.FromArgb(75, 85, 99);

        public override WebHeaderStyle Style => WebHeaderStyle.PortfolioMinimal;

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

            // Background (skip if transparent background)
            Color bgColor = colors?.BackgroundColor ?? Color.FromArgb(250, 250, 250);
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Colors
            Color accent = colors?.AccentColor ?? AccentEmerald;
            Color accentHover = colors?.AccentHoverColor ?? AccentEmeraldHover;
            Color fgColor = colors?.ForegroundColor ?? TextDark;

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO (bold with diamond) ===
            if (showLogo)
            {
                // Diamond accent
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var iconBounds = new Rectangle(x, centerY - 10, 20, 20);
                    DrawLogo(g, iconBounds, logoImagePath, theme);
                    x += 28;
                }
                else
                {
                    // Draw diamond shape
                    using (var brush = new SolidBrush(accent))
                    {
                        var points = new PointF[]
                        {
                            new PointF(x + 7, centerY - 7),
                            new PointF(x + 14, centerY),
                            new PointF(x + 7, centerY + 7),
                            new PointF(x, centerY)
                        };
                        g.FillPolygon(brush, points);
                    }
                    x += 22;
                }

                // Bold serif logo text
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "Portfolio";
                using (var font = new Font("Georgia", 16, FontStyle.Bold))
                {
                    var size = g.MeasureString(brandText, font);
                    using (var brush = new SolidBrush(fgColor))
                    {
                        g.DrawString(brandText, font, brush, x, centerY - size.Height / 2);
                    }
                    x += (int)size.Width + 48;
                }
            }

            // === TABS (wide spacing) ===
            if (tabs != null && tabs.Count > 0)
            {
                // Calculate total width
                int totalTabsWidth = 0;
                var tabWidths = new List<int>();
                foreach (var tab in tabs)
                {
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 8;
                    tabWidths.Add(tabWidth);
                    totalTabsWidth += tabWidth + TAB_SPACING;
                }
                totalTabsWidth -= TAB_SPACING;

                // Center tabs
                int buttonsWidth = CalculateButtonsWidth(buttons, buttonFont);
                int availableWidth = bounds.Width - x - buttonsWidth - PADDING;
                int tabStartX = x + Math.Max(0, (availableWidth - totalTabsWidth) / 2);

                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var tabBounds = new Rectangle(tabStartX, bounds.Top + 12, tabWidths[i], bounds.Height - 24);
                    tab.Bounds = tabBounds;

                    // Tab text - bold for active
                    Color textColor = tab.IsActive ? fgColor : (tab.IsHovered ? fgColor : TextMuted);
                    var font = tab.IsActive ? new Font(tabFont.FontFamily, tabFont.Size, FontStyle.Bold) : tabFont;

                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, font, brush, tabBounds, sf);
                    }

                    if (tab.IsActive && font != tabFont)
                        font.Dispose();

                    tabStartX += tabWidths[i] + TAB_SPACING;
                }
            }

            // === BUTTONS (arrow CTA) ===
            int rightX = bounds.Right - PADDING;
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    var textSize = g.MeasureString(btn.Text, buttonFont);
                    int btnWidth = Math.Max((int)textSize.Width + 48, 130); // Extra space for arrow
                    rightX -= btnWidth;
                    var btnBounds = new Rectangle(rightX, centerY - BUTTON_HEIGHT / 2, btnWidth, BUTTON_HEIGHT);
                    btn.Bounds = btnBounds;

                    bool isCta = btn.Style == WebHeaderButtonStyle.Solid || i == buttons.Count - 1;

                    if (isCta)
                    {
                        // Solid dark/accent with arrow
                        Color bgCol = btn.IsHovered ? accent : fgColor;
                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, BUTTON_RADIUS))
                        using (var brush = new SolidBrush(bgCol))
                        {
                            g.FillPath(brush, path);
                        }

                        // Text
                        using (var brush = new SolidBrush(Color.White))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            var textRect = new Rectangle(btnBounds.X, btnBounds.Y, btnBounds.Width - 20, btnBounds.Height);
                            g.DrawString(btn.Text, buttonFont, brush, textRect, sf);
                        }

                        // Arrow icon
                        int arrowX = btnBounds.Right - 26;
                        int arrowY = centerY;
                        using (var pen = new Pen(Color.White, 2))
                        {
                            pen.EndCap = LineCap.Round;
                            g.DrawLine(pen, arrowX - 4, arrowY - 4, arrowX + 3, arrowY);
                            g.DrawLine(pen, arrowX + 3, arrowY, arrowX - 4, arrowY + 4);
                        }
                    }
                    else
                    {
                        // Text button with hover underline
                        Color textCol = btn.IsHovered ? fgColor : TextMuted;

                        using (var brush = new SolidBrush(textCol))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }

                        // Hover underline
                        if (btn.IsHovered)
                        {
                            var underlineRect = new Rectangle(
                                btnBounds.X + 10,
                                btnBounds.Bottom - 8,
                                btnBounds.Width - 20,
                                2);
                            using (var brush = new SolidBrush(fgColor))
                            {
                                g.FillRectangle(brush, underlineRect);
                            }
                        }
                    }

                    rightX -= 16;
                }
            }

            // Bottom border
            using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
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
                    width += Math.Max((int)textSize.Width + 48, 130) + 16;
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
