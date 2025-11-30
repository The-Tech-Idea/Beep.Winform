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
    /// E-commerce Dark Painter
    /// DISTINCT STYLE: Sleek dark theme with neon orange accents and glow effects.
    /// Gaming/tech e-commerce aesthetic with emphasis on cart and deals.
    /// Features: Dark gradient, neon glow tabs, glowing cart badge, cyber-style typography
    /// </summary>
    public class EcommerceDarkPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 16;
        private const int TAB_SPACING = 6;
        private const int LOGO_SIZE = 38;
        private const int BUTTON_HEIGHT = 38;
        private const int BUTTON_MIN_WIDTH = 100;

        // Style-specific colors (dark cyber palette)
        private static readonly Color BgDark = Color.FromArgb(18, 18, 24);
        private static readonly Color BgDarkGradient = Color.FromArgb(28, 28, 38);
        private static readonly Color AccentNeon = Color.FromArgb(255, 140, 0);
        private static readonly Color AccentNeonHover = Color.FromArgb(255, 165, 40);
        private static readonly Color TextLight = Color.FromArgb(245, 245, 250);
        private static readonly Color TextMuted = Color.FromArgb(160, 160, 175);
        private static readonly Color GlowOrange = Color.FromArgb(60, 255, 140, 0);
        private static readonly Color BorderDark = Color.FromArgb(50, 50, 65);

        public override WebHeaderStyle Style => WebHeaderStyle.EcommerceDark;

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

            // Dark gradient background (skip if transparent background)
            if (!skipBackground)
            {
                using (var brush = new LinearGradientBrush(bounds, BgDark, BgDarkGradient, 90f))
                {
                    g.FillRectangle(brush, bounds);
                }

                // Subtle grid pattern
                using (var pen = new Pen(Color.FromArgb(10, 255, 255, 255), 1))
                {
                    for (int i = bounds.Left; i < bounds.Right; i += 20)
                    {
                        g.DrawLine(pen, i, bounds.Top, i, bounds.Bottom);
                    }
                }
            }

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO with glow ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    // Glow behind logo
                    using (var glowBrush = new SolidBrush(GlowOrange))
                    {
                        g.FillEllipse(glowBrush, logoBounds.X - 6, logoBounds.Y - 6, logoBounds.Width + 12, logoBounds.Height + 12);
                    }
                    DrawLogoBadge(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 12;
                }

                // Brand text with glow
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "TECH";
                using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    var size = g.MeasureString(brandText, font);
                    // Glow effect
                    using (var glowBrush = new SolidBrush(Color.FromArgb(40, AccentNeon)))
                    {
                        g.DrawString(brandText, font, glowBrush, x - 1, centerY - size.Height / 2 - 1);
                        g.DrawString(brandText, font, glowBrush, x + 1, centerY - size.Height / 2 + 1);
                    }
                    using (var brush = new SolidBrush(AccentNeon))
                    {
                        g.DrawString(brandText, font, brush, x, centerY - size.Height / 2);
                    }
                    x += (int)size.Width + 20;
                }
            }

            // Vertical divider with glow
            using (var pen = new Pen(BorderDark, 1))
            {
                g.DrawLine(pen, x, bounds.Top + 12, x, bounds.Bottom - 12);
            }
            x += 16;

            // === TABS with neon effect ===
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 24;
                    var tabBounds = new Rectangle(x, bounds.Top + 8, tabWidth, bounds.Height - 16);
                    tab.Bounds = tabBounds;

                    // Active/Hover background with glow
                    if (tab.IsActive)
                    {
                        // Glow effect
                        using (var glowPath = GraphicsExtensions.GetRoundedRectPath(
                            new Rectangle(tabBounds.X - 2, tabBounds.Y - 2, tabBounds.Width + 4, tabBounds.Height + 4), 6))
                        using (var glowBrush = new SolidBrush(GlowOrange))
                        {
                            g.FillPath(glowBrush, glowPath);
                        }
                        
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 4))
                        using (var brush = new SolidBrush(AccentNeon))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    else if (tab.IsHovered)
                    {
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 4))
                        using (var brush = new SolidBrush(Color.FromArgb(45, 45, 60)))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Tab text
                    Color textColor = tab.IsActive ? BgDark : (tab.IsHovered ? TextLight : TextMuted);
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, tabFont, brush, tabBounds, sf);
                    }

                    x += tabWidth + TAB_SPACING;
                }
            }

            // === BUTTONS (right side) ===
            int rightX = bounds.Right - PADDING;
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    bool isIconButton = !string.IsNullOrEmpty(btn.ImagePath) && string.IsNullOrEmpty(btn.Text);
                    int btnWidth = isIconButton ? 44 : Math.Max(BUTTON_MIN_WIDTH, btn.Width > 0 ? btn.Width : BUTTON_MIN_WIDTH);
                    rightX -= btnWidth;
                    var btnBounds = new Rectangle(rightX, centerY - BUTTON_HEIGHT / 2, btnWidth, BUTTON_HEIGHT);
                    btn.Bounds = btnBounds;

                    if (isIconButton)
                    {
                        // Icon button with glow on hover
                        if (btn.IsHovered)
                        {
                            using (var glowBrush = new SolidBrush(Color.FromArgb(30, AccentNeon)))
                            {
                                g.FillEllipse(glowBrush, btnBounds);
                            }
                        }

                        // Icon
                        int iconSize = 22;
                        var iconRect = new Rectangle(
                            btnBounds.X + (btnBounds.Width - iconSize) / 2,
                            btnBounds.Y + (btnBounds.Height - iconSize) / 2,
                            iconSize, iconSize);
                        try
                        {
                            StyledImagePainter.PaintWithTint(g, iconRect, btn.ImagePath,
                                btn.IsHovered ? AccentNeon : TextMuted, 1f, 2);
                        }
                        catch { }

                        // Badge with glow
                        if (btn.BadgeCount > 0)
                        {
                            DrawGlowingBadge(g, btnBounds.Right - 8, btnBounds.Top + 2, btn.BadgeCount);
                        }
                    }
                    else
                    {
                        // Text button
                        bool isCta = btn.Style == WebHeaderButtonStyle.Solid || i == buttons.Count - 1;
                        Color bgColor = isCta
                            ? (btn.IsHovered ? AccentNeonHover : AccentNeon)
                            : (btn.IsHovered ? Color.FromArgb(55, 55, 70) : Color.FromArgb(40, 40, 55));

                        // Glow for CTA
                        if (isCta)
                        {
                            using (var glowPath = GraphicsExtensions.GetRoundedRectPath(
                                new Rectangle(btnBounds.X - 3, btnBounds.Y - 3, btnBounds.Width + 6, btnBounds.Height + 6), 8))
                            using (var glowBrush = new SolidBrush(GlowOrange))
                            {
                                g.FillPath(glowBrush, glowPath);
                            }
                        }

                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, 6))
                        {
                            using (var brush = new SolidBrush(bgColor))
                            {
                                g.FillPath(brush, path);
                            }
                            if (!isCta)
                            {
                                using (var pen = new Pen(BorderDark, 1))
                                {
                                    g.DrawPath(pen, path);
                                }
                            }
                        }

                        Color textColor = isCta ? BgDark : TextLight;
                        using (var brush = new SolidBrush(textColor))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    rightX -= 8;
                }
            }

            // Bottom neon line
            using (var brush = new LinearGradientBrush(
                new Rectangle(bounds.Left, bounds.Bottom - 2, bounds.Width, 2),
                AccentNeon, Color.FromArgb(255, 80, 0), 0f))
            {
                g.FillRectangle(brush, bounds.Left, bounds.Bottom - 2, bounds.Width, 2);
            }
        }

        private void DrawGlowingBadge(Graphics g, int x, int y, int count)
        {
            string text = count > 99 ? "99+" : count.ToString();
            using (var font = new Font("Segoe UI", 7, FontStyle.Bold))
            {
                var size = g.MeasureString(text, font);
                int width = Math.Max(16, (int)size.Width + 6);
                var rect = new Rectangle(x - width / 2, y, width, 16);

                // Glow
                using (var glowBrush = new SolidBrush(GlowOrange))
                {
                    g.FillEllipse(glowBrush, rect.X - 3, rect.Y - 3, rect.Width + 6, rect.Height + 6);
                }

                using (var brush = new SolidBrush(AccentNeon))
                {
                    g.FillEllipse(brush, rect);
                }
                using (var brush = new SolidBrush(BgDark))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(text, font, brush, rect, sf);
                }
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
