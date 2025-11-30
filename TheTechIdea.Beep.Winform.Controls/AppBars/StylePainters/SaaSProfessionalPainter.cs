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
    /// SaaS Professional Painter
    /// DISTINCT STYLE: Professional dashboard header with blue accent, notification system,
    /// and user profile integration. Clean enterprise aesthetic.
    /// Features: Blue accent, notification badge, user avatar, dashboard-style tabs
    /// </summary>
    public class SaaSProfessionalPainter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 16;
        private const int TAB_SPACING = 8;
        private const int LOGO_SIZE = 32;
        private const int BUTTON_HEIGHT = 36;
        private const int BUTTON_MIN_WIDTH = 100;
        private const int AVATAR_SIZE = 34;

        // Style-specific colors (professional blue palette)
        private static readonly Color BgLight = Color.FromArgb(250, 252, 255);
        private static readonly Color AccentBlue = Color.FromArgb(59, 130, 246);
        private static readonly Color AccentBlueHover = Color.FromArgb(79, 150, 255);
        private static readonly Color AccentBlueBg = Color.FromArgb(239, 246, 255);
        private static readonly Color TextDark = Color.FromArgb(30, 41, 59);
        private static readonly Color TextMuted = Color.FromArgb(100, 116, 139);
        private static readonly Color BorderColor = Color.FromArgb(226, 232, 240);
        private static readonly Color NotificationRed = Color.FromArgb(239, 68, 68);

        public override WebHeaderStyle Style => WebHeaderStyle.SaaSProfessional;

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

            // Light professional background (skip if transparent background)
            Color bgColor = colors?.BackgroundColor ?? BgLight;
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }

                // Top accent line
                using (var brush = new SolidBrush(AccentBlue))
                {
                    g.FillRectangle(brush, bounds.Left, bounds.Top, bounds.Width, 3);
                }
            }

            int centerY = bounds.Top + bounds.Height / 2 + 1; // Offset for top line
            int x = bounds.Left + PADDING;

            // === LOGO ===
            if (showLogo)
            {
                if (!string.IsNullOrEmpty(logoImagePath))
                {
                    var logoBounds = new Rectangle(x, centerY - LOGO_SIZE / 2, LOGO_SIZE, LOGO_SIZE);
                    DrawLogoCircle(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 10;
                }

                // App name
                string appName = !string.IsNullOrEmpty(logoText) ? logoText : "Dashboard";
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                {
                    var size = g.MeasureString(appName, font);
                    using (var brush = new SolidBrush(TextDark))
                    {
                        g.DrawString(appName, font, brush, x, centerY - size.Height / 2);
                    }
                    x += (int)size.Width + 24;
                }
            }

            // === SEARCH BOX ===
            if (showSearchBox)
            {
                var searchRect = new Rectangle(x, centerY - 18, 200, 36);
                DrawSearchBoxProfessional(g, searchRect, searchText, colors);
                x += 216;
            }

            // === TABS ===
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 24;
                    var tabBounds = new Rectangle(x, bounds.Top + 12, tabWidth, bounds.Height - 18);
                    tab.Bounds = tabBounds;

                    // Active/Hover background
                    if (tab.IsActive)
                    {
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 6))
                        using (var brush = new SolidBrush(AccentBlueBg))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    else if (tab.IsHovered)
                    {
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, 6))
                        using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Tab text
                    Color textColor = tab.IsActive ? AccentBlue : (tab.IsHovered ? TextDark : TextMuted);
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, tabFont, brush, tabBounds, sf);
                    }

                    // Bottom indicator for active
                    if (tab.IsActive)
                    {
                        using (var pen = new Pen(AccentBlue, 2.5f))
                        {
                            g.DrawLine(pen, tabBounds.Left + 8, tabBounds.Bottom - 2, tabBounds.Right - 8, tabBounds.Bottom - 2);
                        }
                    }

                    x += tabWidth + TAB_SPACING;
                }
            }

            // === RIGHT SIDE: Notifications, Settings, Avatar ===
            int rightX = bounds.Right - PADDING;

            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    var btn = buttons[i];
                    bool isAvatar = !string.IsNullOrEmpty(btn.ImagePath) && (btn.Text?.ToLower().Contains("profile") == true || btn.Text?.ToLower().Contains("account") == true || string.IsNullOrEmpty(btn.Text));
                    bool isIcon = !string.IsNullOrEmpty(btn.ImagePath) && !isAvatar;
                    
                    int btnWidth = isAvatar ? AVATAR_SIZE : (isIcon ? 40 : Math.Max(BUTTON_MIN_WIDTH, btn.Width > 0 ? btn.Width : BUTTON_MIN_WIDTH));
                    rightX -= btnWidth;
                    var btnBounds = new Rectangle(rightX, centerY - (isAvatar ? AVATAR_SIZE : BUTTON_HEIGHT) / 2, btnWidth, isAvatar ? AVATAR_SIZE : BUTTON_HEIGHT);
                    btn.Bounds = btnBounds;

                    if (isAvatar)
                    {
                        // User avatar with ring
                        using (var pen = new Pen(btn.IsHovered ? AccentBlue : BorderColor, 2))
                        {
                            g.DrawEllipse(pen, btnBounds);
                        }
                        var avatarInner = new Rectangle(btnBounds.X + 2, btnBounds.Y + 2, btnBounds.Width - 4, btnBounds.Height - 4);
                        try
                        {
                            StyledImagePainter.PaintInCircle(g, avatarInner.X + avatarInner.Width / 2f, avatarInner.Y + avatarInner.Height / 2f, avatarInner.Width / 2f, btn.ImagePath);
                        }
                        catch
                        {
                            // Fallback - draw initials
                            using (var brush = new SolidBrush(AccentBlueBg))
                            {
                                g.FillEllipse(brush, avatarInner);
                            }
                            using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                            using (var brush = new SolidBrush(AccentBlue))
                            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                            {
                                g.DrawString("U", font, brush, avatarInner, sf);
                            }
                        }
                    }
                    else if (isIcon)
                    {
                        // Icon button (notifications, settings)
                        if (btn.IsHovered)
                        {
                            using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                            {
                                g.FillEllipse(brush, btnBounds);
                            }
                        }

                        int iconSize = 20;
                        var iconRect = new Rectangle(btnBounds.X + (btnBounds.Width - iconSize) / 2, btnBounds.Y + (btnBounds.Height - iconSize) / 2, iconSize, iconSize);
                        try
                        {
                            StyledImagePainter.PaintWithTint(g, iconRect, btn.ImagePath, btn.IsHovered ? AccentBlue : TextMuted, 1f, 2);
                        }
                        catch { }

                        // Notification badge
                        if (btn.BadgeCount > 0)
                        {
                            DrawNotificationBadge(g, btnBounds.Right - 6, btnBounds.Top + 4, btn.BadgeCount);
                        }
                    }
                    else
                    {
                        // Regular button
                        bool isCta = btn.Style == WebHeaderButtonStyle.Solid;
                        Color bgCol = isCta ? (btn.IsHovered ? AccentBlueHover : AccentBlue) : (btn.IsHovered ? Color.FromArgb(248, 250, 252) : Color.Transparent);
                        Color textCol = isCta ? Color.White : (btn.IsHovered ? TextDark : TextMuted);

                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, 6))
                        {
                            using (var brush = new SolidBrush(bgCol))
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

                        using (var brush = new SolidBrush(textCol))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    rightX -= 10;
                }
            }

            // Bottom border
            using (var pen = new Pen(BorderColor, 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
        }

        private void DrawSearchBoxProfessional(Graphics g, Rectangle bounds, string text, WebHeaderColors colors)
        {
            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, 8))
            {
                using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(BorderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Search icon
            int iconSize = 16;
            var iconRect = new Rectangle(bounds.Left + 10, bounds.Top + (bounds.Height - iconSize) / 2, iconSize, iconSize);
            try
            {
                StyledImagePainter.PaintWithTint(g, iconRect, TheTechIdea.Beep.Icons.SvgsUI.Search, TextMuted, 1f, 2);
            }
            catch { }

            // Text
            string displayText = string.IsNullOrEmpty(text) ? "Search..." : text;
            Color textColor = string.IsNullOrEmpty(text) ? TextMuted : TextDark;
            using (var font = new Font("Segoe UI", 9))
            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                var textRect = new Rectangle(bounds.Left + 32, bounds.Top, bounds.Width - 40, bounds.Height);
                g.DrawString(displayText, font, brush, textRect, sf);
            }
        }

        private void DrawNotificationBadge(Graphics g, int x, int y, int count)
        {
            string text = count > 9 ? "9+" : count.ToString();
            int size = 16;
            var rect = new Rectangle(x - size / 2, y, size, size);

            using (var brush = new SolidBrush(NotificationRed))
            {
                g.FillEllipse(brush, rect);
            }
            using (var pen = new Pen(Color.White, 1.5f))
            {
                g.DrawEllipse(pen, rect);
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
