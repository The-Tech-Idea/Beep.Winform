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
    /// Multi-Row Compact Painter
    /// DISTINCT STYLE: Two-row header for complex navigation with categories and subcategories.
    /// Top row: Main navigation. Bottom row: Secondary/contextual navigation.
    /// Features: Two-row layout, category icons, breadcrumb-style secondary nav
    /// </summary>
    public class MultiRowCompactPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 16;
        private const int TAB_SPACING = 8;
        private const int LOGO_SIZE = 32;
        private const int BUTTON_HEIGHT = 32;
        private const int BUTTON_MIN_WIDTH = 90;

        // Style-specific colors
        private static readonly Color AccentIndigo = Color.FromArgb(79, 70, 229);
        private static readonly Color AccentIndigoHover = Color.FromArgb(99, 90, 249);
        private static readonly Color TextDark = Color.FromArgb(17, 24, 39);
        private static readonly Color TextMuted = Color.FromArgb(107, 114, 128);
        private static readonly Color BorderColor = Color.FromArgb(229, 231, 235);
        private static readonly Color SecondaryBg = Color.FromArgb(249, 250, 251);

        public override WebHeaderStyle Style => WebHeaderStyle.MultiRowCompact;

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

            // Split into two rows
            int topRowHeight = (int)(bounds.Height * 0.6);
            int bottomRowHeight = bounds.Height - topRowHeight;
            var topRow = new Rectangle(bounds.Left, bounds.Top, bounds.Width, topRowHeight);
            var bottomRow = new Rectangle(bounds.Left, bounds.Top + topRowHeight, bounds.Width, bottomRowHeight);

            // Background (skip if transparent background)
            if (!skipBackground)
            {
                // === TOP ROW: White background ===
                using (var brush = new SolidBrush(colors?.BackgroundColor ?? Color.White))
                {
                    g.FillRectangle(brush, topRow);
                }

                // === BOTTOM ROW: Subtle gray background ===
                using (var brush = new SolidBrush(SecondaryBg))
                {
                    g.FillRectangle(brush, bottomRow);
                }

                // Divider between rows
                using (var pen = new Pen(BorderColor, 1))
                {
                    g.DrawLine(pen, topRow.Left, topRow.Bottom, topRow.Right, topRow.Bottom);
                }
            }

            int topCenterY = topRow.Top + topRowHeight / 2;
            int x = topRow.Left + PADDING;

            // === TOP ROW: Logo + Main Tabs + Buttons ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, topCenterY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    DrawLogoPill(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 10;
                }

                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "Brand";
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                {
                    var size = g.MeasureString(brandText, font);
                    using (var brush = new SolidBrush(TextDark))
                    {
                        g.DrawString(brandText, font, brush, x, topCenterY - size.Height / 2);
                    }
                    x += (int)size.Width + 24;
                }
            }

            // Main tabs (first half)
            int mainTabCount = tabs != null ? (tabs.Count + 1) / 2 : 0;
            if (tabs != null && mainTabCount > 0)
            {
                for (int i = 0; i < mainTabCount; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 24;
                    var tabBounds = new Rectangle(x, topRow.Top + 6, tabWidth, topRowHeight - 12);
                    tab.Bounds = tabBounds;

                    // Active/Hover background
                    if (tab.IsActive)
                    {
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 6))
                        using (var brush = new SolidBrush(Color.FromArgb(238, 242, 255)))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    else if (tab.IsHovered)
                    {
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 6))
                        using (var brush = new SolidBrush(Color.FromArgb(249, 250, 251)))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Tab text
                    Color textColor = tab.IsActive ? AccentIndigo : (tab.IsHovered ? TextDark : TextMuted);
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, tabFont, brush, tabBounds, sf);
                    }

                    // Bottom indicator for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(AccentIndigo, 2))
                        {
                            g.DrawLine(pen, tabBounds.Left + 8, tabBounds.Bottom - 2, tabBounds.Right - 8, tabBounds.Bottom - 2);
                        }
                    }

                    x += tabWidth + TAB_SPACING;
                }
            }

            // Buttons (right side of top row)
            int rightX = topRow.Right - PADDING;
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    int btnWidth = Math.Max(BUTTON_MIN_WIDTH, btn.Width > 0 ? btn.Width : BUTTON_MIN_WIDTH);
                    rightX -= btnWidth;
                    var btnBounds = new Rectangle(rightX, topCenterY - BUTTON_HEIGHT / 2, btnWidth, BUTTON_HEIGHT);
                    btn.Bounds = btnBounds;

                    bool isCta = btn.Style == WebHeaderButtonStyle.Solid || i == buttons.Count - 1;

                    if (isCta)
                    {
                        Color bgCol = btn.IsHovered ? AccentIndigoHover : AccentIndigo;
                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, 6))
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
                        if (btn.IsHovered)
                        {
                            using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, 6))
                            using (var brush = new SolidBrush(Color.FromArgb(249, 250, 251)))
                            {
                                g.FillPath(brush, path);
                            }
                        }
                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, 6))
                        using (var pen = new Pen(BorderColor, 1))
                        {
                            g.DrawPath(pen, path);
                        }
                        Color textCol = btn.IsHovered ? TextDark : TextMuted;
                        using (var brush = new SolidBrush(textCol))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    rightX -= 8;
                }
            }

            // === BOTTOM ROW: Secondary tabs (second half) ===
            int bottomCenterY = bottomRow.Top + bottomRowHeight / 2;
            x = bottomRow.Left + PADDING;

            // Breadcrumb-style indicator
            using (var brush = new SolidBrush(TextMuted))
            using (var font = new Font("Segoe UI", 9))
            {
                g.DrawString("Categories:", font, brush, x, bottomCenterY - 7);
                x += 70;
            }

            if (tabs != null && tabs.Count > mainTabCount)
            {
                using (var smallFont = new Font(tabFont.FontFamily, tabFont.Size - 1, FontStyle.Regular))
                {
                    for (int i = mainTabCount; i < tabs.Count; i++)
                    {
                        var tab = tabs[i];
                        var textSize = g.MeasureString(tab.Text, smallFont);
                        int tabWidth = (int)textSize.Width + 16;
                        var tabBounds = new Rectangle(x, bottomRow.Top + 4, tabWidth, bottomRowHeight - 8);
                        tab.Bounds = tabBounds;

                        // Hover background
                        if (tab.IsHovered)
                        {
                            using (var brush = new SolidBrush(Color.FromArgb(243, 244, 246)))
                            {
                                g.FillRectangle(brush, tabBounds);
                            }
                        }

                        // Text
                        Color textColor = tab.IsActive ? AccentIndigo : (tab.IsHovered ? TextDark : TextMuted);
                        using (var brush = new SolidBrush(textColor))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(tab.Text, smallFont, brush, tabBounds, sf);
                        }

                        // Separator
                        if (i < tabs.Count - 1)
                        {
                            using (var pen = new Pen(BorderColor, 1))
                            {
                                g.DrawLine(pen, tabBounds.Right + 4, bottomRow.Top + 8, tabBounds.Right + 4, bottomRow.Bottom - 8);
                            }
                        }

                        x += tabWidth + 12;
                    }
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
