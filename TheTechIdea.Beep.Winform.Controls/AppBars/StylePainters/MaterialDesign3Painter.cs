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
    /// Material Design 3 Painter
    /// DISTINCT STYLE: Google Material Design 3 (Material You) with dynamic colors,
    /// elevation shadows, and tonal surfaces. Modern Android/Google aesthetic.
    /// Features: Tonal surfaces, elevation shadows, rounded corners, dynamic color system
    /// </summary>
    public class MaterialDesign3Painter : WebHeaderStylePainterBase
    {
        // Layout constants
        private const int PADDING = 16;
        private const int TAB_SPACING = 8;
        private const int LOGO_SIZE = 40;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_MIN_WIDTH = 110;
        private const int CORNER_RADIUS = 20;

        // Style-specific colors (Material You palette)
        private static readonly Color Primary = Color.FromArgb(103, 80, 164);
        private static readonly Color PrimaryHover = Color.FromArgb(123, 100, 184);
        private static readonly Color OnPrimary = Color.White;
        private static readonly Color Surface = Color.FromArgb(255, 251, 254);
        private static readonly Color SurfaceVariant = Color.FromArgb(231, 224, 236);
        private static readonly Color OnSurface = Color.FromArgb(28, 27, 31);
        private static readonly Color OnSurfaceVariant = Color.FromArgb(73, 69, 79);
        private static readonly Color Outline = Color.FromArgb(121, 116, 126);
        private static readonly Color ShadowColor = Color.FromArgb(30, 0, 0, 0);

        public override WebHeaderStyle Style => WebHeaderStyle.MaterialDesign3;

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

            // Surface background (skip if transparent background)
            Color bgColor = colors?.BackgroundColor ?? Surface;
            if (!skipBackground)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
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
                    DrawLogoCircle(g, logoBounds, logoImagePath, theme);
                    x += LOGO_SIZE + 12;
                }

                // App name
                string appName = !string.IsNullOrEmpty(logoText) ? logoText : "Material";
                using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    var size = g.MeasureString(appName, font);
                    using (var brush = new SolidBrush(OnSurface))
                    {
                        g.DrawString(appName, font, brush, x, centerY - size.Height / 2);
                    }
                    x += (int)size.Width + 24;
                }
            }

            // === TABS (Material navigation rail style) ===
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    var textSize = g.MeasureString(tab.Text, tabFont);
                    int tabWidth = (int)textSize.Width + 32;
                    int tabHeight = 40;
                    var tabBounds = new Rectangle(x, centerY - tabHeight / 2, tabWidth, tabHeight);
                    tab.Bounds = tabBounds;

                    // Active/Hover state container
                    if (tab.IsActive)
                    {
                        // Active indicator - pill shape with primary color
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, tabHeight / 2))
                        using (var brush = new SolidBrush(Color.FromArgb(232, 222, 248))) // Secondary container
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    else if (tab.IsHovered)
                    {
                        // Hover state layer
                        using (var path = GraphicsExtensions.GetRoundedRectPath(tabBounds, tabHeight / 2))
                        using (var brush = new SolidBrush(Color.FromArgb(20, OnSurface)))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Tab text
                    Color textColor = tab.IsActive ? Primary : (tab.IsHovered ? OnSurface : OnSurfaceVariant);
                    using (var font = new Font(tabFont.FontFamily, tabFont.Size, tab.IsActive ? FontStyle.Bold : FontStyle.Regular))
                    using (var brush = new SolidBrush(textColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(tab.Text, font, brush, tabBounds, sf);
                    }

                    x += tabWidth + TAB_SPACING;
                }
            }

            // === BUTTONS (Material FAB/Filled style) ===
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
                        // Filled button (elevated)
                        // Shadow
                        var shadowRect = new Rectangle(btnBounds.X + 2, btnBounds.Y + 3, btnBounds.Width, btnBounds.Height);
                        using (var path = GraphicsExtensions.GetRoundedRectPath(shadowRect, CORNER_RADIUS))
                        using (var brush = new SolidBrush(ShadowColor))
                        {
                            g.FillPath(brush, path);
                        }

                        // Button
                        Color bgCol = btn.IsHovered ? PrimaryHover : Primary;
                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, CORNER_RADIUS))
                        using (var brush = new SolidBrush(bgCol))
                        {
                            g.FillPath(brush, path);
                        }

                        using (var font = new Font(buttonFont.FontFamily, buttonFont.Size, FontStyle.Bold))
                        using (var brush = new SolidBrush(OnPrimary))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, font, brush, btnBounds, sf);
                        }
                    }
                    else
                    {
                        // Outlined button
                        if (btn.IsHovered)
                        {
                            using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, CORNER_RADIUS))
                            using (var brush = new SolidBrush(Color.FromArgb(20, Primary)))
                            {
                                g.FillPath(brush, path);
                            }
                        }

                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnBounds, CORNER_RADIUS))
                        using (var pen = new Pen(Outline, 1))
                        {
                            g.DrawPath(pen, path);
                        }

                        Color textCol = btn.IsHovered ? Primary : OnSurfaceVariant;
                        using (var brush = new SolidBrush(textCol))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(btn.Text, buttonFont, brush, btnBounds, sf);
                        }
                    }

                    rightX -= 12;
                }
            }

            // Bottom elevation shadow
            using (var brush = new LinearGradientBrush(
                new Rectangle(bounds.Left, bounds.Bottom - 4, bounds.Width, 4),
                Color.FromArgb(15, 0, 0, 0), Color.Transparent, 90f))
            {
                g.FillRectangle(brush, bounds.Left, bounds.Bottom - 4, bounds.Width, 4);
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
