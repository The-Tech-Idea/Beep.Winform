using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Mobile-First Painter
    /// DISTINCT STYLE: Compact responsive header optimized for touch and smaller screens.
    /// Hamburger menu, icon-based navigation, large touch targets.
    /// Features: Hamburger menu, icon tabs, touch-friendly 44px targets, compact layout
    /// </summary>
    public class MobileFirstPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 12;
        private const int TAB_SPACING = 4;
        private const int LOGO_SIZE = 32;
        private const int TOUCH_TARGET = 44; // Minimum touch target size
        private const int ICON_SIZE = 22;

        // Style-specific colors (mobile-friendly high contrast)
        private static readonly Color AccentBlue = Color.FromArgb(0, 122, 255);
        private static readonly Color AccentBlueHover = Color.FromArgb(0, 142, 255);
        private static readonly Color TextDark = Color.FromArgb(28, 28, 30);
        private static readonly Color TextMuted = Color.FromArgb(142, 142, 147);
        private static readonly Color BorderColor = Color.FromArgb(229, 229, 234);
        private static readonly Color HoverBg = Color.FromArgb(242, 242, 247);

        public override WebHeaderStyle Style => WebHeaderStyle.MobileFirst;

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

            // Clean white background (skip if transparent background)
            Color bgColor = colors?.BackgroundColor ?? Color.White;
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            int centerY = bounds.Top + bounds.Height / 2;
            int x = bounds.Left + PADDING;

            // === HAMBURGER MENU ===
            var menuBounds = new Rectangle(x, centerY - TOUCH_TARGET / 2, TOUCH_TARGET, TOUCH_TARGET);
            DrawHamburgerIcon(g, menuBounds, TextDark);
            x += TOUCH_TARGET + 8;

            // === LOGO (compact) ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    DrawLogoCircle(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 8;
                }
                else if (!string.IsNullOrEmpty(logoText))
                {
                    using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                    {
                        var size = g.MeasureString(logoText, font);
                        using (var brush = new SolidBrush(TextDark))
                        {
                            g.DrawString(logoText, font, brush, x, centerY - size.Height / 2);
                        }
                        x += (int)size.Width + 16;
                    }
                }
            }

            // === TABS (icon-based for mobile) ===
            if (tabs != null && tabs.Count > 0)
            {
                // Show only first few tabs as icons
                int maxVisibleTabs = Math.Min(tabs.Count, 4);
                for (int i = 0; i < maxVisibleTabs; i++)
                {
                    var tab = tabs[i];
                    var tabBounds = new Rectangle(x, centerY - TOUCH_TARGET / 2, TOUCH_TARGET, TOUCH_TARGET);
                    tab.Bounds = tabBounds;

                    // Hover/Active background
                    if (tab.IsActive || tab.IsHovered)
                    {
                        using (var brush = new SolidBrush(tab.IsActive ? Color.FromArgb(230, 244, 255) : HoverBg))
                        {
                            g.FillEllipse(brush, tabBounds);
                        }
                    }

                    // Icon or first letter
                    if (!string.IsNullOrEmpty(tab.ImagePath))
                    {
                        var iconRect = new Rectangle(
                            tabBounds.X + (tabBounds.Width - ICON_SIZE) / 2,
                            tabBounds.Y + (tabBounds.Height - ICON_SIZE) / 2,
                            ICON_SIZE, ICON_SIZE);
                        try
                        {
                            Color iconColor = tab.IsActive ? AccentBlue : TextMuted;
                            StyledImagePainter.PaintWithTint(g, iconRect, tab.ImagePath, iconColor, 1f, 2);
                        }
                        catch { }
                    }
                    else
                    {
                        // First letter as icon
                        Color textColor = tab.IsActive ? AccentBlue : TextMuted;
                        using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
                        using (var brush = new SolidBrush(textColor))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            string initial = tab.Text.Length > 0 ? tab.Text[0].ToString().ToUpper() : "?";
                            g.DrawString(initial, font, brush, tabBounds, sf);
                        }
                    }

                    // Active indicator dot
                    if (tab.IsActive)
                    {
                        using (var brush = new SolidBrush(AccentBlue))
                        {
                            g.FillEllipse(brush, tabBounds.X + tabBounds.Width / 2 - 3, tabBounds.Bottom - 8, 6, 6);
                        }
                    }

                    x += TOUCH_TARGET + TAB_SPACING;
                }
            }

            // === BUTTONS (right side, icon-based) ===
            int rightX = bounds.Right - PADDING;
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    rightX -= TOUCH_TARGET;
                    var btnBounds = new Rectangle(rightX, centerY - TOUCH_TARGET / 2, TOUCH_TARGET, TOUCH_TARGET);
                    btn.Bounds = btnBounds;

                    // Hover background
                    if (btn.IsHovered)
                    {
                        using (var brush = new SolidBrush(HoverBg))
                        {
                            g.FillEllipse(brush, btnBounds);
                        }
                    }

                    // Icon or text
                    if (!string.IsNullOrEmpty(btn.ImagePath))
                    {
                        var iconRect = new Rectangle(
                            btnBounds.X + (btnBounds.Width - ICON_SIZE) / 2,
                            btnBounds.Y + (btnBounds.Height - ICON_SIZE) / 2,
                            ICON_SIZE, ICON_SIZE);
                        try
                        {
                            Color iconColor = btn.IsHovered ? AccentBlue : TextMuted;
                            StyledImagePainter.PaintWithTint(g, iconRect, btn.ImagePath, iconColor, 1f, 2);
                        }
                        catch { }

                        // Badge
                        if (btn.BadgeCount > 0)
                        {
                            DrawBadge(g, btnBounds.Right - 8, btnBounds.Top + 6, btn.BadgeCount);
                        }
                    }
                    else
                    {
                        // CTA button style for text buttons
                        bool isCta = btn.Style == WebHeaderButtonStyle.Solid;
                        if (isCta)
                        {
                            using (var brush = new SolidBrush(btn.IsHovered ? AccentBlueHover : AccentBlue))
                            using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, TOUCH_TARGET / 2))
                            {
                                g.FillPath(brush, path);
                            }
                            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
                            using (var brush = new SolidBrush(Color.White))
                            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                            {
                                g.DrawString(btn.Text, font, brush, btnBounds, sf);
                            }
                        }
                        else
                        {
                            Color textColor = btn.IsHovered ? AccentBlue : TextMuted;
                            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
                            using (var brush = new SolidBrush(textColor))
                            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                            {
                                string initial = btn.Text.Length > 0 ? btn.Text[0].ToString().ToUpper() : "?";
                                g.DrawString(initial, font, brush, btnBounds, sf);
                            }
                        }
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

        private void DrawHamburgerIcon(Graphics g, Rectangle bounds, Color color)
        {
            int lineWidth = 20;
            int lineHeight = 2;
            int spacing = 5;
            int startX = bounds.X + (bounds.Width - lineWidth) / 2;
            int startY = bounds.Y + (bounds.Height - (lineHeight * 3 + spacing * 2)) / 2;

            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, startX, startY, lineWidth, lineHeight);
                g.FillRectangle(brush, startX, startY + lineHeight + spacing, lineWidth, lineHeight);
                g.FillRectangle(brush, startX, startY + (lineHeight + spacing) * 2, lineWidth, lineHeight);
            }
        }

        private void DrawBadge(Graphics g, int x, int y, int count)
        {
            string text = count > 9 ? "9+" : count.ToString();
            int size = 16;
            var rect = new Rectangle(x - size / 2, y, size, size);

            using (var brush = new SolidBrush(Color.FromArgb(255, 59, 48)))
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
