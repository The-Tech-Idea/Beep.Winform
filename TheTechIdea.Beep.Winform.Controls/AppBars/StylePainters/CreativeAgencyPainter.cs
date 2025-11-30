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
    /// Creative Agency Painter
    /// DISTINCT STYLE: Bold artistic design with gradient accents and expressive typography.
    /// Portfolio/agency aesthetic with asymmetric elements and vibrant colors.
    /// Features: Large bold logo, artistic underlines, gradient CTA, wide spacing
    /// </summary>
    public class CreativeAgencyPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 24;
        private const int TAB_SPACING = 36;
        private const int LOGO_SIZE = 44;
        private const int BUTTON_HEIGHT = 42;
        private const int BUTTON_MIN_WIDTH = 130;

        // Style-specific colors (creative vibrant palette)
        private static readonly Color AccentCoral = Color.FromArgb(255, 99, 99);
        private static readonly Color AccentCoralHover = Color.FromArgb(255, 130, 130);
        private static readonly Color AccentPurple = Color.FromArgb(147, 51, 234);
        private static readonly Color TextDark = Color.FromArgb(17, 17, 17);
        private static readonly Color TextMuted = Color.FromArgb(115, 115, 115);
        private static readonly Color BgCream = Color.FromArgb(255, 253, 250);

        public override WebHeaderStyle Style => WebHeaderStyle.CreativeAgency;

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

            // Subtle gradient background (skip if transparent background)
            if (!skipBackground)
            {
                using (var brush = new LinearGradientBrush(bounds, Color.White, BgCream, 90f))
                {
                    g.FillRectangle(brush, bounds);
                }

                // Artistic top accent - asymmetric line
                using (var pen = new Pen(AccentCoral, 3))
                {
                    g.DrawLine(pen, bounds.Left, bounds.Top + 4, bounds.Left + 100, bounds.Top + 4);
                }
            }

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO (large, artistic) ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    // Tinted logo
                    try
                    {
                        StyledImagePainter.PaintWithTint(g, logoBounds, logoImagePath, AccentCoral, 0.7f, 8);
                    }
                    catch
                    {
                        DrawLogoPill(g, logoBounds, logoImagePath, theme);
                    }
                    x += LOGO_SIZE + 16;
                }

                // Bold agency name
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText.ToUpper() : "CREATIVE";
                using (var font = new Font("Segoe UI", 15, FontStyle.Bold))
                {
                    var size = g.MeasureString(brandText, font);
                    using (var brush = new SolidBrush(TextDark))
                    {
                        g.DrawString(brandText, font, brush, x, centerY - size.Height / 2);
                    }
                    x += (int)size.Width + 40;
                }
            }

            // === TABS (bold with artistic underline) ===
            if (tabs != null && tabs.Count > 0)
            {
                using (var boldFont = new Font(tabFont.FontFamily, tabFont.Size, FontStyle.Bold))
                {
                    for (int i = 0; i < tabs.Count; i++)
                    {
                        var tab = tabs[i];
                        var textSize = g.MeasureString(tab.Text, boldFont);
                        int tabWidth = (int)textSize.Width + 12;
                        var tabBounds = new Rectangle(x, bounds.Top + 16, tabWidth, bounds.Height - 32);
                        tab.Bounds = tabBounds;

                        // Tab text
                        Color textColor = tab.IsActive ? AccentCoral : (tab.IsHovered ? TextDark : TextMuted);
                        using (var brush = new SolidBrush(textColor))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(tab.Text, boldFont, brush, tabBounds, sf);
                        }

                        // Artistic underline for active - thick with rounded ends
                        if (tab.IsActive)
                        {
                            int underlineY = tabBounds.Bottom + 4;
                            var underlineRect = new Rectangle(tabBounds.Left, underlineY, tabBounds.Width, 4);
                            using (var path = GraphicsExtensions.GetRoundedRectPath(underlineRect, 2))
                            using (var brush = new SolidBrush(AccentCoral))
                            {
                                g.FillPath(brush, path);
                            }
                        }

                        // Hover dot indicator
                        if (tab.IsHovered && !tab.IsActive)
                        {
                            using (var brush = new SolidBrush(AccentCoral))
                            {
                                g.FillEllipse(brush, tabBounds.Left + tabBounds.Width / 2 - 2, tabBounds.Bottom + 4, 4, 4);
                            }
                        }

                        x += tabWidth + TAB_SPACING;
                    }
                }
            }

            // === BUTTONS (right side, gradient CTA) ===
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
                        // Gradient CTA button
                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, 4))
                        {
                            Color startColor = btn.IsHovered ? AccentCoralHover : AccentCoral;
                            Color endColor = btn.IsHovered ? AccentCoral : AccentPurple;
                            using (var brush = new LinearGradientBrush(btnBounds, startColor, endColor, 45f))
                            {
                                g.FillPath(brush, path);
                            }
                        }

                        using (var boldFont = new Font(buttonFont.FontFamily, buttonFont.Size, FontStyle.Bold))
                        using (var brush = new SolidBrush(Color.White))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, boldFont, brush, btnBounds, sf);
                        }
                    }
                    else
                    {
                        // Outline button
                        Color borderCol = btn.IsHovered ? AccentCoral : TextMuted;
                        Color textCol = btn.IsHovered ? AccentCoral : TextMuted;

                        if (btn.IsHovered)
                        {
                            using (var brush = new SolidBrush(Color.FromArgb(255, 245, 245)))
                            {
                                g.FillRectangle(brush, btnBounds);
                            }
                        }

                        using (var pen = new Pen(borderCol, 2))
                        {
                            g.DrawRectangle(pen, btnBounds);
                        }

                        using (var boldFont = new Font(buttonFont.FontFamily, buttonFont.Size, FontStyle.Bold))
                        using (var brush = new SolidBrush(textCol))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, boldFont, brush, btnBounds, sf);
                        }
                    }

                    rightX -= 16;
                }
            }

            // Bottom border - subtle
            using (var pen = new Pen(Color.FromArgb(240, 240, 240), 1))
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

