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
    /// Trend Modern Painter
    /// DISTINCT STYLE: Bold dark gradient with vibrant coral/pink accents.
    /// Pill-shaped tabs, bold typography, neon-style hover effects.
    /// Features: Dark gradient background, coral accent pills, bold white text, glowing hover
    /// </summary>
    public class TrendModernPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 20;
        private const int TAB_SPACING = 10;
        private const int LOGO_SIZE = 40;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_MIN_WIDTH = 120;

        // Style-specific colors (dark modern palette)
        private static readonly Color GradientStart = Color.FromArgb(20, 20, 35);
        private static readonly Color GradientEnd = Color.FromArgb(40, 35, 55);
        private static readonly Color AccentCoral = Color.FromArgb(255, 107, 129);
        private static readonly Color AccentCoralHover = Color.FromArgb(255, 140, 160);
        private static readonly Color TextLight = Color.FromArgb(255, 255, 255);
        private static readonly Color TextMuted = Color.FromArgb(180, 180, 195);
        private static readonly Color GlowColor = Color.FromArgb(40, 255, 107, 129);

        public override WebHeaderStyle Style => WebHeaderStyle.TrendModern;

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
                using (var brush = new LinearGradientBrush(bounds, GradientStart, GradientEnd, 45f))
                {
                    g.FillRectangle(brush, bounds);
                }

                // Subtle noise/texture effect via horizontal lines
                using (var pen = new Pen(Color.FromArgb(8, 255, 255, 255), 1))
                {
                    for (int y = bounds.Top + 10; y < bounds.Bottom; y += 4)
                    {
                        g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
                    }
                }
            }

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    // Glow effect behind logo
                    using (var glowBrush = new SolidBrush(Color.FromArgb(30, AccentCoral)))
                    {
                        g.FillEllipse(glowBrush, logoBounds.X - 4, logoBounds.Y - 4, logoBounds.Width + 8, logoBounds.Height + 8);
                    }
                    DrawLogoPill(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 16;
                }

                // Brand text with gradient effect
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "TREND";
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    var size = g.MeasureString(brandText, font);
                    var textRect = new RectangleF(x, centerY - size.Height / 2, size.Width, size.Height);
                    
                    // Text gradient
                    using (var brush = new LinearGradientBrush(textRect, TextLight, AccentCoral, 0f))
                    {
                        g.DrawString(brandText, font, brush, textRect.Location);
                    }
                    x += (int)size.Width + 30;
                }
            }

            // === TABS (pill style) ===
            if (tabs != null && tabs.Count > 0)
            {
                using (var boldFont = new Font(tabFont.FontFamily, tabFont.Size + 1, FontStyle.Bold))
                {
                    for (int i = 0; i < tabs.Count; i++)
                    {
                        var tab = tabs[i];
                        var textSize = g.MeasureString(tab.Text, boldFont);
                        int tabWidth = (int)textSize.Width + 32;
                        int tabHeight = 38;
                        var tabBounds = new Rectangle(x, centerY - tabHeight / 2, tabWidth, tabHeight);
                        tab.Bounds = tabBounds;

                        // Pill background with glow
                        if (tab.IsActive || tab.IsHovered)
                        {
                            Color pillColor = tab.IsActive ? AccentCoral : Color.FromArgb(60, 60, 80);
                            
                            // Glow effect
                            if (tab.IsActive)
                            {
                                using (var glowPath = GraphicsExtensions.GetRoundedRectPath(
                                    new Rectangle(tabBounds.X - 3, tabBounds.Y - 3, tabBounds.Width + 6, tabBounds.Height + 6), tabHeight / 2 + 3))
                                using (var glowBrush = new SolidBrush(GlowColor))
                                {
                                    g.FillPath(glowBrush, glowPath);
                                }
                            }

                            using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, tabHeight / 2))
                            using (var brush = new SolidBrush(pillColor))
                            {
                                g.FillPath(brush, path);
                            }
                        }

                        // Tab text
                        using (var brush = new SolidBrush(TextLight))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(tab.Text, boldFont, brush, tabBounds, sf);
                        }

                        x += tabWidth + TAB_SPACING;
                    }
                }
            }

            // === BUTTONS (right side, bold style) ===
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
                    
                    // Button background
                    Color bgColor = isCta 
                        ? (btn.IsHovered ? AccentCoralHover : AccentCoral)
                        : (btn.IsHovered ? Color.FromArgb(70, 70, 95) : Color.FromArgb(50, 50, 70));
                    Color borderColor = isCta ? Color.Transparent : Color.FromArgb(100, 100, 130);

                    // Glow for CTA
                    if (isCta && btn.IsHovered)
                    {
                        using (var glowPath = GraphicsExtensions.GetRoundedRectPath(
                            new Rectangle(btnBounds.X - 4, btnBounds.Y - 4, btnBounds.Width + 8, btnBounds.Height + 8), 12))
                        using (var glowBrush = new SolidBrush(Color.FromArgb(50, AccentCoral)))
                        {
                            g.FillPath(glowBrush, glowPath);
                        }
                    }

                    using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, 8))
                    {
                        using (var brush = new SolidBrush(bgColor))
                        {
                            g.FillPath(brush, path);
                        }
                        if (borderColor != Color.Transparent)
                        {
                            using (var pen = new Pen(borderColor, 1.5f))
                            {
                                g.DrawPath(pen, path);
                            }
                        }
                    }

                    // Button text
                    using (var boldFont = new Font(buttonFont.FontFamily, buttonFont.Size, isCta ? FontStyle.Bold : FontStyle.Regular))
                    using (var brush = new SolidBrush(TextLight))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(btn.Text, boldFont, brush, btnBounds, sf);
                    }

                    rightX -= 12;
                }
            }

            // Bottom accent line with gradient
            using (var brush = new LinearGradientBrush(
                new Rectangle(bounds.Left, bounds.Bottom - 3, bounds.Width, 3),
                AccentCoral, Color.FromArgb(180, 100, 255), 0f))
            {
                g.FillRectangle(brush, bounds.Left, bounds.Bottom - 3, bounds.Width, 3);
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
