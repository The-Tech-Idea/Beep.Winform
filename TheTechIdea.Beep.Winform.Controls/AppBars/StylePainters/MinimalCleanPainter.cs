using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Minimal Clean Painter
    /// DISTINCT STYLE: Ultra-minimal text-only design for content-focused sites.
    /// Maximum whitespace, no decorations, typography-driven.
    /// Features: No icons, text-only tabs, extreme simplicity, serif typography option
    /// </summary>
    public class MinimalCleanPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 32;
        private const int TAB_SPACING = 48;
        private const int LOGO_SIZE = 24;
        private const int BUTTON_MIN_WIDTH = 80;
        private const int BUTTON_HEIGHT = 32;

        // Style-specific colors (ultra minimal)
        private static readonly Color TextPrimary = Color.FromArgb(17, 17, 17);
        private static readonly Color TextSecondary = Color.FromArgb(153, 153, 153);
        private static readonly Color TextHover = Color.FromArgb(68, 68, 68);
        private static readonly Color BgColor = Color.FromArgb(254, 254, 254);

        public override WebHeaderStyle Style => WebHeaderStyle.MinimalClean;

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

            // Ultra-minimal background (skip if transparent background)
            Color bg = colors?.BackgroundColor ?? BgColor;
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bg))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO (minimal text only) ===
            if (showLogo)
            {
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "minimal";
                using (var font = new Font("Georgia", 13, FontStyle.Italic))
                {
                    var size = g.MeasureString(brandText, font);
                    using (var brush = new SolidBrush(TextPrimary))
                    {
                        g.DrawString(brandText, font, brush, x, centerY - size.Height / 2);
                    }
                    x += (int)size.Width + 60;
                }
            }

            // === TABS (text only, wide spacing) ===
            if (tabs != null && tabs.Count > 0)
            {
                using (var normalFont = new Font(tabFont.FontFamily, tabFont.Size - 1, FontStyle.Regular))
                {
                    for (int i = 0; i < tabs.Count; i++)
                    {
                        var tab = tabs[i];
                        var textSize = g.MeasureString(tab.Text, normalFont);
                        int tabWidth = (int)textSize.Width;
                        var tabBounds = new Rectangle(x, bounds.Top + 8, tabWidth + 8, bounds.Height - 16);
                        tab.Bounds = tabBounds;

                        // Text only - color change for state
                        Color textColor = tab.IsActive ? TextPrimary : (tab.IsHovered ? TextHover : TextSecondary);
                        using (var brush = new SolidBrush(textColor))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(tab.Text, normalFont, brush, tabBounds, sf);
                        }

                        // Minimal dot for active
                        if (tab.IsActive)
                        {
                            using (var brush = new SolidBrush(TextPrimary))
                            {
                                g.FillEllipse(brush, tabBounds.X + tabBounds.Width / 2 - 2, tabBounds.Bottom - 8, 4, 4);
                            }
                        }

                        x += tabWidth + TAB_SPACING;
                    }
                }
            }

            // === BUTTONS (minimal, right side) ===
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
                        // Minimal solid button - just black
                        Color bgCol = btn.IsHovered ? Color.FromArgb(40, 40, 40) : TextPrimary;
                        using (var brush = new SolidBrush(bgCol))
                        {
                            g.FillRectangle(brush, btnBounds);
                        }
                        using (var font = new Font(buttonFont.FontFamily, buttonFont.Size - 1, FontStyle.Regular))
                        using (var brush = new SolidBrush(Color.White))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, font, brush, btnBounds, sf);
                        }
                    }
                    else
                    {
                        // Text only button
                        Color textColor = btn.IsHovered ? TextPrimary : TextSecondary;
                        using (var font = new Font(buttonFont.FontFamily, buttonFont.Size - 1, FontStyle.Regular))
                        using (var brush = new SolidBrush(textColor))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, font, brush, btnBounds, sf);
                        }
                    }

                    rightX -= 32;
                }
            }

            // No borders - true minimal
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
