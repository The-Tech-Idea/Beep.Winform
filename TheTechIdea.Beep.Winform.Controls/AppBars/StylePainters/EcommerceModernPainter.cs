using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// E-commerce Modern Painter
    /// DISTINCT STYLE: Feature-rich e-commerce with prominent search, icon buttons,
    /// and pill-shaped CTA. Clean and functional shopping experience.
    /// Features: Prominent search bar, icon buttons (cart/heart/profile), pill CTA
    /// </summary>
    public class EcommerceModernPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 20;
        private const int TAB_SPACING = 24;
        private const int SEARCH_WIDTH = 280;
        private const int SEARCH_HEIGHT = 42;
        private const int ICON_SIZE = 22;
        private const int BUTTON_HEIGHT = 42;
        private const int BUTTON_RADIUS = 21;

        // Style-specific colors (modern e-commerce palette)
        private static readonly Color AccentRed = Color.FromArgb(239, 68, 68);
        private static readonly Color AccentRedHover = Color.FromArgb(220, 50, 50);
        private static readonly Color TextDark = Color.FromArgb(17, 24, 39);
        private static readonly Color TextMuted = Color.FromArgb(107, 114, 128);
        private static readonly Color BorderColor = Color.FromArgb(229, 231, 235);
        private static readonly Color SearchBg = Color.FromArgb(243, 244, 246);

        public override WebHeaderStyle Style => WebHeaderStyle.EcommerceModern;

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
            Color bgColor = colors?.BackgroundColor ?? Color.White;
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Colors
            Color accent = colors?.AccentColor ?? AccentRed;
            Color accentHover = colors?.AccentHoverColor ?? AccentRedHover;
            Color fgColor = colors?.ForegroundColor ?? TextDark;

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - 16, 32, 32);
                    DrawLogo(g, logoBounds, logoImagePath, theme);
                    x += 40;
                }

                // Logo text
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "Store";
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    var size = g.MeasureString(brandText, font);
                    using (var brush = new SolidBrush(fgColor))
                    {
                        g.DrawString(brandText, font, brush, x, centerY - size.Height / 2);
                    }
                    x += (int)size.Width + 24;
                }
            }

            // === TABS ===
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 16;
                    var tabBounds = new Rectangle(x, bounds.Top + 8, tabWidth, bounds.Height - 16);
                    tab.Bounds = tabBounds;

                    // Tab styling
                    Color tabTextColor = tab.IsActive ? fgColor : (tab.IsHovered ? fgColor : TextMuted);

                    // Hover/active background
                    if (tab.IsHovered || tab.IsActive)
                    {
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 6))
                        using (var brush = new SolidBrush(Color.FromArgb(tab.IsActive ? 20 : 10, fgColor)))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Text
                    var font = tab.IsActive ? new Font(tabFont.FontFamily, tabFont.Size, FontStyle.Bold) : tabFont;
                    using (var brush = new SolidBrush(tabTextColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, font, brush, tabBounds, sf);
                    }
                    if (tab.IsActive && font != tabFont) font.Dispose();

                    x += tabWidth + TAB_SPACING;
                }
            }

            // === SEARCH BOX ===
            if (showSearchBox)
            {
                int searchX = x + 16;
                var searchBounds = new Rectangle(searchX, centerY - SEARCH_HEIGHT / 2, SEARCH_WIDTH, SEARCH_HEIGHT);

                // Search background - pill shape
                using (var path = GraphicsExtensions.GetRoundedRectPath(searchBounds, SEARCH_HEIGHT / 2))
                using (var brush = new SolidBrush(SearchBg))
                {
                    g.FillPath(brush, path);
                }

                // Search icon
                var iconRect = new Rectangle(searchBounds.X + 14, centerY - 9, 18, 18);
                try
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, SvgsUI.Search, TextMuted, 1f, 2);
                }
                catch
                {
                    using (var pen = new Pen(TextMuted, 1.5f))
                    {
                        g.DrawEllipse(pen, iconRect.X, iconRect.Y, 12, 12);
                        g.DrawLine(pen, iconRect.X + 10, iconRect.Y + 10, iconRect.X + 14, iconRect.Y + 14);
                    }
                }

                // Search text
                string displayText = string.IsNullOrEmpty(searchText) ? "Search products..." : searchText;
                Color textCol = string.IsNullOrEmpty(searchText) ? TextMuted : fgColor;
                using (var font = new Font("Segoe UI", 10))
                using (var brush = new SolidBrush(textCol))
                using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                {
                    var textRect = new Rectangle(searchBounds.X + 40, searchBounds.Y, searchBounds.Width - 52, searchBounds.Height);
                    g.DrawString(displayText, font, brush, textRect, sf);
                }
            }

            // === RIGHT SIDE: Icon buttons + CTA ===
            int rightX = bounds.Right - PADDING;

            if (buttons != null && buttons.Count > 0)
            {
                // CTA button (last)
                var ctaBtn = buttons[buttons.Count - 1];
                var ctaTextSize = g.MeasureString(ctaBtn.Text, buttonFont);
                int ctaWidth = Math.Max((int)ctaTextSize.Width + 32, 100);
                rightX -= ctaWidth;
                var ctaBounds = new Rectangle(rightX, centerY - BUTTON_HEIGHT / 2, ctaWidth, BUTTON_HEIGHT);
                ctaBtn.Bounds = ctaBounds;

                // CTA - pill shape
                Color ctaBg = ctaBtn.IsHovered ? accentHover : accent;
                using (var path = GraphicsExtensions.GetRoundedRectPath(ctaBounds, BUTTON_RADIUS))
                using (var brush = new SolidBrush(ctaBg))
                {
                    g.FillPath(brush, path);
                }
                using (var brush = new SolidBrush(Color.White))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(ctaBtn.Text, buttonFont, brush, ctaBounds, sf);
                }

                rightX -= 16;

                // Icon buttons
                for (int i = buttons.Count - 2; i >= 0; i--)
                {
                    var btn = buttons[i];
                    rightX -= 44;
                    var iconBounds = new Rectangle(rightX, centerY - 22, 44, 44);
                    btn.Bounds = iconBounds;

                    // Hover circle
                    if (btn.IsHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(15, fgColor)))
                        {
                            g.FillEllipse(brush, iconBounds);
                        }
                    }

                    // Icon
                    if (!string.IsNullOrEmpty(btn.ImagePath))
                    {
                        var innerIconRect = new Rectangle(iconBounds.X + 11, iconBounds.Y + 11, ICON_SIZE, ICON_SIZE);
                        try
                        {
                            StyledImagePainter.PaintWithTint(g, innerIconRect, btn.ImagePath, fgColor, 1f, 2);
                        }
                        catch
                        {
                            DrawIconByName(g, innerIconRect, btn.Text, fgColor);
                        }
                    }
                    else
                    {
                        var innerIconRect = new Rectangle(iconBounds.X + 11, iconBounds.Y + 11, ICON_SIZE, ICON_SIZE);
                        DrawIconByName(g, innerIconRect, btn.Text, fgColor);
                    }

                    // Badge
                    if (btn.BadgeCount > 0)
                    {
                        DrawBadge(g, iconBounds.Right - 10, iconBounds.Top + 6, btn.BadgeCount, accent);
                    }

                    rightX -= 4;
                }
            }

            // Bottom border
            using (var pen = new Pen(BorderColor, 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
        }

        private void DrawIconByName(Graphics g, Rectangle rect, string name, Color color)
        {
            string lowerName = name?.ToLowerInvariant() ?? "";

            using (var pen = new Pen(color, 1.5f))
            {
                if (lowerName.Contains("cart") || lowerName.Contains("bag"))
                {
                    g.DrawRectangle(pen, rect.X + 4, rect.Y + 8, rect.Width - 8, rect.Height - 10);
                    g.DrawArc(pen, rect.X + 6, rect.Y + 2, rect.Width - 12, 12, 180, 180);
                }
                else if (lowerName.Contains("heart") || lowerName.Contains("wish"))
                {
                    using (var path = new GraphicsPath())
                    {
                        path.AddArc(rect.X + 2, rect.Y + 4, 10, 10, 135, 225);
                        path.AddArc(rect.X + 12, rect.Y + 4, 10, 10, 180, 225);
                        path.AddLine(rect.X + rect.Width - 2, rect.Y + 10, rect.X + rect.Width / 2, rect.Bottom - 4);
                        path.CloseFigure();
                        g.DrawPath(pen, path);
                    }
                }
                else if (lowerName.Contains("user") || lowerName.Contains("profile") || lowerName.Contains("account"))
                {
                    g.DrawEllipse(pen, rect.X + 7, rect.Y + 3, 10, 10);
                    g.DrawArc(pen, rect.X + 2, rect.Y + 14, 20, 14, 180, 180);
                }
                else
                {
                    g.DrawEllipse(pen, rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
                }
            }
        }

        private void DrawBadge(Graphics g, int x, int y, int count, Color color)
        {
            string text = count > 9 ? "9+" : count.ToString();
            int size = 16;
            var rect = new Rectangle(x - size / 2, y, size, size);

            using (var brush = new SolidBrush(color))
            {
                g.FillEllipse(brush, rect);
            }
            using (var font = new Font("Segoe UI", 7, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, font, brush, rect, sf);
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
