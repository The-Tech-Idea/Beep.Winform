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
    /// Shoppy Store E-commerce Style 1 Painter
    /// DISTINCT STYLE: Clean e-commerce with warm orange accent, centered navigation,
    /// prominent search bar, and pill-shaped CTA buttons.
    /// Features: Logo left, centered category tabs with orange underline, search center-right, cart/account right
    /// </summary>
    public class ShoppyStore1Painter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 16;
        private const int TAB_SPACING = 8;
        private const int LOGO_SIZE = 36;
        private const int SEARCH_WIDTH = 220;
        private const int BUTTON_HEIGHT = 36;
        private const int BUTTON_MIN_WIDTH = 90;
        
        // Style-specific colors (warm e-commerce palette)
        private static readonly Color AccentOrange = Color.FromArgb(255, 107, 53);
        private static readonly Color AccentOrangeHover = Color.FromArgb(255, 130, 80);
        private static readonly Color TextDark = Color.FromArgb(38, 38, 38);
        private static readonly Color TextMuted = Color.FromArgb(117, 117, 117);
        private static readonly Color BorderLight = Color.FromArgb(238, 238, 238);
        private static readonly Color HoverBg = Color.FromArgb(250, 250, 250);

        public override WebHeaderStyle Style => WebHeaderStyle.ShoppyStore1;

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

            // Use theme colors if available, otherwise use style defaults
            Color bgColor = colors?.BackgroundColor ?? Color.White;
            Color fgColor = colors?.ForegroundColor ?? TextDark;
            Color accent = colors?.AccentColor ?? AccentOrange;
            Color accentHover = colors?.AccentHoverColor ?? AccentOrangeHover;

            // Background - clean white (skip if transparent background)
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }

                // Bottom border - subtle shadow line
                using (var pen = new Pen(BorderLight, 1))
                {
                    g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                }
            }

            int x = bounds.Left + PADDING;
            int centerY = bounds.Top + bounds.Height / 2;

            // === LOGO SECTION ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    DrawLogoPill(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 12;
                }
                
                // Draw brand text
                if (!string.IsNullOrEmpty(logoText))
                {
                    using (var brush = new SolidBrush(fgColor))
                    using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                    {
                        var textSize = g.MeasureString(logoText, font);
                        g.DrawString(logoText, font, brush, x, centerY - textSize.Height / 2);
                        x += (int)textSize.Width + 20;
                    }
                }
                else if (string.IsNullOrEmpty(logoImagePath))
                {
                    // Default logo text
                    using (var brush = new SolidBrush(accent))
                    using (var font = new Font("Segoe UI", 15, FontStyle.Bold))
                    {
                        g.DrawString("Shoppy", font, brush, x, centerY - 12);
                        x += 80;
                    }
                }
            }

            // === TABS SECTION (centered) ===
            if (tabs != null && tabs.Count > 0)
            {
                // Calculate total tabs width for centering
                int totalTabsWidth = 0;
                foreach (var tab in tabs)
                {
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    totalTabsWidth += (int)textSize.Width + 24 + TAB_SPACING;
                }
                totalTabsWidth -= TAB_SPACING;

                // Center tabs in available space (between logo and buttons)
                int buttonsWidth = (buttons?.Count ?? 0) * (BUTTON_MIN_WIDTH + 10) + (showSearchBox ? SEARCH_WIDTH + 20 : 0);
                int availableWidth = bounds.Width - x - buttonsWidth - PADDING * 2;
                int tabStartX = x + Math.Max(0, (availableWidth - totalTabsWidth) / 2);

                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 24;
                    var tabBounds = new Rectangle(tabStartX, bounds.Top + 8, tabWidth, bounds.Height - 16);
                    tab.Bounds = tabBounds;

                    // Hover background
                    if (tab.IsHovered && !tab.IsActive)
                    {
                        using (var path = GetRoundedRect(tabBounds, 6))
                        using (var brush = new SolidBrush(HoverBg))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Tab text
                    Color textColor = tab.IsActive ? accent : (tab.IsHovered ? fgColor : TextMuted);
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, tabFont, brush, tabBounds, sf);
                    }

                    // Active indicator - orange underline with rounded ends
                    if (tab.IsActive)
                    {
                        int indicatorY = tabBounds.Bottom - 4;
                        int indicatorWidth = Math.Min(tabWidth - 16, 40);
                        int indicatorX = tabBounds.X + (tabBounds.Width - indicatorWidth) / 2;
                        var indicatorRect = new Rectangle(indicatorX, indicatorY, indicatorWidth, 3);
                        using (var path = GetRoundedRect(indicatorRect, 2))
                        using (var brush = new SolidBrush(accent))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Dropdown indicator for tabs with children
                    if (tab.HasChildren)
                    {
                        DrawDropdownArrow(g, tabBounds.Right - 14, centerY - 2, tab.IsActive ? accent : TextMuted);
                    }

                    tabStartX += tabWidth + TAB_SPACING;
                }
            }

            // === SEARCH BOX ===
            int rightX = bounds.Right - PADDING;
            
            if (showSearchBox)
            {
                int searchX = rightX - SEARCH_WIDTH - ((buttons?.Count ?? 0) * (BUTTON_MIN_WIDTH + 10));
                var searchRect = new Rectangle(searchX, centerY - 18, SEARCH_WIDTH, 36);
                DrawSearchBoxStyled(g, searchRect, searchText, colors);
                rightX = searchX - 16;
            }

            // === BUTTONS (right side) ===
            if (buttons != null && buttons.Count > 0)
            {
                int buttonX = bounds.Right - PADDING;
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    int btnWidth = Math.Max(BUTTON_MIN_WIDTH, btn.Width > 0 ? btn.Width : BUTTON_MIN_WIDTH);
                    buttonX -= btnWidth;
                    var btnBounds = new Rectangle(buttonX, centerY - BUTTON_HEIGHT / 2, btnWidth, BUTTON_HEIGHT);
                    btn.Bounds = btnBounds;

                    bool isCta = btn.Style == WebHeaderButtonStyle.Solid || i == buttons.Count - 1;
                    DrawButtonStyled(g, btnBounds, btn, isCta, accent, accentHover, colors, buttonFont);

                    buttonX -= 10;
                }
            }
        }

        private void DrawSearchBoxStyled(Graphics g, Rectangle bounds, string text, WebHeaderColors colors)
        {
            Color bgColor = colors?.SearchBackgroundColor ?? Color.FromArgb(247, 247, 247);
            Color textColor = colors?.SearchTextColor ?? TextDark;
            Color placeholderColor = colors?.SearchPlaceholderColor ?? TextMuted;

            // Rounded search box
            using (var path = GetRoundedRect(bounds, bounds.Height / 2))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
            }

            // Search icon
            int iconSize = 18;
            var iconRect = new Rectangle(bounds.Left + 12, bounds.Top + (bounds.Height - iconSize) / 2, iconSize, iconSize);
            try
            {
                StyledImagePainter.PaintWithTint(g, iconRect, TheTechIdea.Beep.Icons.SvgsUI.Search, placeholderColor, 1f, 2);
            }
            catch
            {
                using (var pen = new Pen(placeholderColor, 1.5f))
                {
                    g.DrawEllipse(pen, iconRect.X, iconRect.Y, 12, 12);
                    g.DrawLine(pen, iconRect.X + 10, iconRect.Y + 10, iconRect.X + 14, iconRect.Y + 14);
                }
            }

            // Search text or placeholder
            var textRect = new Rectangle(bounds.Left + 36, bounds.Top, bounds.Width - 48, bounds.Height);
            string displayText = string.IsNullOrEmpty(text) ? "Search products..." : text;
            Color displayColor = string.IsNullOrEmpty(text) ? placeholderColor : textColor;
            using (var brush = new SolidBrush(displayColor))
            using (var font = new Font("Segoe UI", 10))
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(displayText, font, brush, textRect, sf);
            }
        }

        private void DrawButtonStyled(Graphics g, Rectangle bounds, WebHeaderActionButton btn, bool isCta, 
            Color accent, Color accentHover, WebHeaderColors colors, Font font)
        {
            Color bgColor, textColor, borderColor;

            if (isCta)
            {
                // CTA button - filled with accent
                bgColor = btn.IsHovered ? accentHover : accent;
                textColor = Color.White;
                borderColor = Color.Transparent;
            }
            else
            {
                // Secondary button - outline style
                bgColor = btn.IsHovered ? HoverBg : Color.Transparent;
                textColor = btn.IsHovered ? TextDark : TextMuted;
                borderColor = btn.IsHovered ? BorderLight : Color.Transparent;
            }

            // Draw button background
            using (var path = GetRoundedRect(bounds, isCta ? bounds.Height / 2 : 6))
            {
                if (bgColor != Color.Transparent)
                {
                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
                if (borderColor != Color.Transparent)
                {
                    using (var pen = new Pen(borderColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw icon if present
            int textOffsetX = 0;
            if (!string.IsNullOrEmpty(btn.ImagePath))
            {
                int iconSize = 18;
                var iconRect = new Rectangle(bounds.Left + 10, bounds.Top + (bounds.Height - iconSize) / 2, iconSize, iconSize);
                try
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, btn.ImagePath, textColor, 1f, 2);
                }
                catch { }
                textOffsetX = iconSize + 4;
            }

            // Draw text
            var textRect = new Rectangle(bounds.Left + 10 + textOffsetX, bounds.Top, bounds.Width - 20 - textOffsetX, bounds.Height);
            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(btn.Text, font, brush, textRect, sf);
            }
        }

        private void DrawDropdownArrow(Graphics g, int x, int y, Color color)
        {
            using (var pen = new Pen(color, 1.5f))
            {
                g.DrawLine(pen, x - 3, y, x, y + 3);
                g.DrawLine(pen, x, y + 3, x + 3, y);
            }
        }

        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            return GraphicsExtensions.GetRoundedRectPath(bounds, radius);
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
