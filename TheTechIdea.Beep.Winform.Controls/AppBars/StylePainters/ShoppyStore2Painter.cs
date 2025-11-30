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
    /// Shoppy Store Style 2 Painter
    /// DISTINCT STYLE: E-commerce with prominent centered search, category dropdown,
    /// and floating cart badge. More compact than Store1.
    /// Features: Compact logo, mega-menu style tabs, large search bar, cart with badge
    /// </summary>
    public class ShoppyStore2Painter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 12;
        private const int TAB_SPACING = 4;
        private const int LOGO_SIZE = 32;
        private const int SEARCH_WIDTH = 320;
        private const int BUTTON_HEIGHT = 38;
        private const int BUTTON_MIN_WIDTH = 42;

        // Style-specific colors (teal e-commerce palette)
        private static readonly Color AccentTeal = Color.FromArgb(0, 150, 136);
        private static readonly Color AccentTealHover = Color.FromArgb(0, 170, 156);
        private static readonly Color TextDark = Color.FromArgb(33, 33, 33);
        private static readonly Color TextMuted = Color.FromArgb(97, 97, 97);
        private static readonly Color BorderColor = Color.FromArgb(224, 224, 224);
        private static readonly Color SearchBg = Color.FromArgb(245, 245, 245);

        public override WebHeaderStyle Style => WebHeaderStyle.ShoppyStore2;

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

            // Use theme colors if available
            Color bgColor = colors?.BackgroundColor ?? Color.White;
            Color fgColor = colors?.ForegroundColor ?? TextDark;
            Color accent = colors?.AccentColor ?? AccentTeal;
            Color accentHover = colors?.AccentHoverColor ?? AccentTealHover;

            // Background (skip if transparent background)
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === LOGO (compact) ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    DrawLogoCircle(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 8;
                }
                
                // Brand text
                string brandText = !string.IsNullOrEmpty(logoText) ? logoText : "Store";
                using (var brush = new SolidBrush(accent))
                using (var font = new Font("Segoe UI", 13, FontStyle.Bold))
                {
                    var size = g.MeasureString(brandText, font);
                    g.DrawString(brandText, font, brush, x, centerY - size.Height / 2);
                    x += (int)size.Width + 16;
                }
            }

            // === TABS (dropdown style with chevron) ===
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + (tab.HasChildren ? 32 : 20);
                    var tabBounds = new Rectangle(x, bounds.Top + 6, tabWidth, bounds.Height - 12);
                    tab.Bounds = tabBounds;

                    // Hover/Active background
                    if (tab.IsActive || tab.IsHovered)
                    {
                        Color tabBg = tab.IsActive ? Color.FromArgb(20, accent) : Color.FromArgb(245, 245, 245);
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 4))
                        using (var brush = new SolidBrush(tabBg))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Tab text
                    Color textColor = tab.IsActive ? accent : (tab.IsHovered ? fgColor : TextMuted);
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                    {
                        var textRect = new Rectangle(tabBounds.X + 10, tabBounds.Y, tabBounds.Width - 20, tabBounds.Height);
                        g.DrawString(tab.Text, tabFont, brush, textRect, sf);
                    }

                    // Dropdown chevron
                    if (tab.HasChildren)
                    {
                        int chevronX = tabBounds.Right - 16;
                        int chevronY = centerY;
                        using (var pen = new Pen(textColor, 1.5f))
                        {
                            g.DrawLine(pen, chevronX - 3, chevronY - 2, chevronX, chevronY + 1);
                            g.DrawLine(pen, chevronX, chevronY + 1, chevronX + 3, chevronY - 2);
                        }
                    }

                    // Bottom indicator for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(accent, 2))
                        {
                            g.DrawLine(pen, tabBounds.Left + 8, tabBounds.Bottom - 2, tabBounds.Right - 8, tabBounds.Bottom - 2);
                        }
                    }

                    x += tabWidth + TAB_SPACING;
                }
            }

            // === SEARCH BOX (prominent, centered) ===
            if (showSearchBox)
            {
                int searchX = bounds.Left + (bounds.Width - SEARCH_WIDTH) / 2;
                // Ensure search doesn't overlap tabs
                searchX = Math.Max(searchX, x + 20);
                var searchRect = new Rectangle(searchX, centerY - 20, SEARCH_WIDTH, 40);
                DrawSearchBoxWithCategory(g, searchRect, searchText, accent, colors);
            }

            // === BUTTONS (icon-style cart/wishlist/account) ===
            int rightX = bounds.Right - PADDING;
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    int btnWidth = btn.Width > 0 ? btn.Width : BUTTON_MIN_WIDTH;
                    bool isIconOnly = string.IsNullOrEmpty(btn.Text) || !string.IsNullOrEmpty(btn.ImagePath);
                    if (!isIconOnly) btnWidth = Math.Max(btnWidth, 80);
                    
                    rightX -= btnWidth;
                    var btnBounds = new Rectangle(rightX, centerY - BUTTON_HEIGHT / 2, btnWidth, BUTTON_HEIGHT);
                    btn.Bounds = btnBounds;

                    // Icon button style
                    if (isIconOnly && !string.IsNullOrEmpty(btn.ImagePath))
                    {
                        // Hover circle
                        if (btn.IsHovered)
                        {
                            using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
                            {
                                g.FillEllipse(brush, btnBounds);
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
                                btn.IsHovered ? accent : TextMuted, 1f, 2);
                        }
                        catch { }

                        // Badge for cart
                        if (btn.BadgeCount > 0)
                        {
                            DrawBadge(g, btnBounds.Right - 10, btnBounds.Top + 4, btn.BadgeCount, accent);
                        }
                    }
                    else
                    {
                        // Text button (CTA style)
                        bool isCta = btn.Style == WebHeaderButtonStyle.Solid;
                        DrawButtonStyled(g, btnBounds, btn, isCta, accent, accentHover, colors, buttonFont);
                    }

                    rightX -= 8;
                }
            }

            // Bottom border
            using (var pen = new Pen(BorderColor, 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
        }

        private void DrawSearchBoxWithCategory(Graphics g, Rectangle bounds, string text, Color accent, WebHeaderColors colors)
        {
            // Category dropdown on left
            int categoryWidth = 100;
            var categoryRect = new Rectangle(bounds.Left, bounds.Top, categoryWidth, bounds.Height);
            var searchInputRect = new Rectangle(bounds.Left + categoryWidth, bounds.Top, bounds.Width - categoryWidth, bounds.Height);

            // Full rounded border
            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, 8))
            {
                using (var brush = new SolidBrush(SearchBg))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(BorderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Category text
            using (var brush = new SolidBrush(TextMuted))
            using (var font = new Font("Segoe UI", 9))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("All ▾", font, brush, categoryRect, sf);
            }

            // Divider
            using (var pen = new Pen(BorderColor, 1))
            {
                g.DrawLine(pen, categoryRect.Right, bounds.Top + 8, categoryRect.Right, bounds.Bottom - 8);
            }

            // Search text
            string displayText = string.IsNullOrEmpty(text) ? "Search for products..." : text;
            Color textColor = string.IsNullOrEmpty(text) ? TextMuted : TextDark;
            using (var brush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI", 10))
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                var textRect = new Rectangle(searchInputRect.Left + 12, searchInputRect.Top, searchInputRect.Width - 50, searchInputRect.Height);
                g.DrawString(displayText, font, brush, textRect, sf);
            }

            // Search button
            int btnSize = bounds.Height - 8;
            var searchBtnRect = new Rectangle(bounds.Right - btnSize - 4, bounds.Top + 4, btnSize, btnSize);
            using (var path = GraphicsExtensions.GetRoundedRectPath(searchBtnRect, 6))
            using (var brush = new SolidBrush(accent))
            {
                g.FillPath(brush, path);
            }

            // Search icon
            try
            {
                var iconRect = new Rectangle(searchBtnRect.X + 6, searchBtnRect.Y + 6, btnSize - 12, btnSize - 12);
                StyledImagePainter.PaintWithTint(g, iconRect, TheTechIdea.Beep.Icons.SvgsUI.Search, Color.White, 1f, 2);
            }
            catch { }
        }

        private void DrawBadge(Graphics g, int x, int y, int count, Color color)
        {
            string text = count > 99 ? "99+" : count.ToString();
            using (var font = new Font("Segoe UI", 7, FontStyle.Bold))
            {
                var size = g.MeasureString(text, font);
                int width = Math.Max(16, (int)size.Width + 6);
                var rect = new Rectangle(x - width / 2, y, width, 16);
                
                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, rect);
                }
                using (var brush = new SolidBrush(Color.White))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(text, font, brush, rect, sf);
                }
            }
        }

        private void DrawButtonStyled(Graphics g, Rectangle bounds, WebHeaderActionButton btn, bool isCta,
            Color accent, Color accentHover, WebHeaderColors colors, Font font)
        {
            Color bgColor = isCta ? (btn.IsHovered ? accentHover : accent) : (btn.IsHovered ? Color.FromArgb(245, 245, 245) : Color.Transparent);
            Color textColor = isCta ? Color.White : (btn.IsHovered ? TextDark : TextMuted);

            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, 6))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
                if (!isCta)
                {
                    using (var pen = new Pen(BorderColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(btn.Text, font, brush, bounds, sf);
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
