using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Spacing;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Helper class for calculating tooltip layout rectangles.
    /// Uses StyleTypography + StyleSpacing for style-aware sizing instead of hard-coded values.
    /// </summary>
    public static class ToolTipLayoutHelpers
    {
        /// <summary>
        /// Calculates layout rectangles for tooltip elements
        /// </summary>
        public static ToolTipLayoutMetrics CalculateLayout(
            Rectangle bounds,
            ToolTipConfig config,
            ToolTipPlacement placement,
            bool hasTitle,
            bool hasIcon,
            bool hasImage,
            int padding,
            int spacing)
        {
            return CalculateLayout(bounds, config, placement, hasTitle, hasIcon, hasImage, padding, spacing, null);
        }

        /// <summary>
        /// Calculates layout rectangles for tooltip elements with Graphics for precise text measurement.
        /// </summary>
        public static ToolTipLayoutMetrics CalculateLayout(
            Rectangle bounds,
            ToolTipConfig config,
            ToolTipPlacement placement,
            bool hasTitle,
            bool hasIcon,
            bool hasImage,
            int padding,
            int spacing,
            Graphics g)
        {
            var metrics = new ToolTipLayoutMetrics
            {
                ContentPadding = padding,
                ItemSpacing = spacing
            };

            int x = bounds.X + padding;
            int y = bounds.Y + padding;
            int availableWidth = bounds.Width - (padding * 2);
            int availableHeight = bounds.Height - (padding * 2);

            // Icon or image on left — size from StyleSpacing or config
            if (hasIcon || hasImage)
            {
                int iconSize = config.MaxImageSize.Height > 0
                    ? config.MaxImageSize.Height
                    : ResolveIconSize(config);

                metrics.IconRect = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + spacing;
                availableWidth -= (iconSize + spacing);
            }

            // Title — measure dynamically instead of hard-coding 18 px
            if (hasTitle)
            {
                int titleHeight = MeasureTitleHeight(g, config, availableWidth);
                metrics.TitleRect = new Rectangle(x, y, availableWidth, titleHeight);
                y += titleHeight + spacing;
            }

            // Close button (top-right if closable)
            if (config.Closable)
            {
                int btnSize = Math.Max(16, spacing * 2);
                metrics.CloseButtonRect = new Rectangle(
                    bounds.Right - padding - btnSize,
                    bounds.Y + padding,
                    btnSize, btnSize);

                // Shrink title width to avoid overlapping close button
                if (hasTitle && !metrics.TitleRect.IsEmpty)
                {
                    metrics.TitleRect = new Rectangle(
                        metrics.TitleRect.X,
                        metrics.TitleRect.Y,
                        metrics.TitleRect.Width - btnSize - spacing,
                        metrics.TitleRect.Height);
                }
            }

            // Text content
            if (!string.IsNullOrEmpty(config.Text))
            {
                int textTop = y;
                int textHeight = availableHeight - (y - bounds.Y - padding);
                // Reserve space for footer if shortcut keys exist
                if (config.Shortcuts != null && config.Shortcuts.Count > 0)
                {
                    int footerHeight = ResolveFooterHeight(config, spacing);
                    textHeight -= footerHeight + spacing;
                }
                metrics.TextRect = new Rectangle(x, textTop, availableWidth, Math.Max(0, textHeight));
            }

            // Divider and footer for shortcut keys
            if (config.Shortcuts != null && config.Shortcuts.Count > 0)
            {
                int footerHeight = ResolveFooterHeight(config, spacing);
                int dividerY = bounds.Bottom - padding - footerHeight - spacing;
                metrics.DividerRect = new Rectangle(
                    bounds.X + padding, dividerY,
                    bounds.Width - padding * 2, 1);
                metrics.FooterRect = new Rectangle(
                    bounds.X + padding, dividerY + spacing,
                    bounds.Width - padding * 2, footerHeight);
            }

            // Arrow position
            if (config.ShowArrow)
            {
                metrics.ArrowRect = CalculateArrowRect(bounds, placement, config.Offset);
            }

            return metrics;
        }

        /// <summary>
        /// Calculates arrow rectangle based on placement
        /// </summary>
        private static Rectangle CalculateArrowRect(Rectangle bounds, ToolTipPlacement placement, int offset)
        {
            const int arrowSize = 8;
            int arrowX = 0;
            int arrowY = 0;

            switch (placement)
            {
                case ToolTipPlacement.Top:
                case ToolTipPlacement.TopStart:
                case ToolTipPlacement.TopEnd:
                    arrowY = bounds.Bottom - arrowSize;
                    arrowX = bounds.Left + bounds.Width / 2 - arrowSize / 2;
                    break;

                case ToolTipPlacement.Bottom:
                case ToolTipPlacement.BottomStart:
                case ToolTipPlacement.BottomEnd:
                    arrowY = bounds.Top;
                    arrowX = bounds.Left + bounds.Width / 2 - arrowSize / 2;
                    break;

                case ToolTipPlacement.Left:
                case ToolTipPlacement.LeftStart:
                case ToolTipPlacement.LeftEnd:
                    arrowX = bounds.Right - arrowSize;
                    arrowY = bounds.Top + bounds.Height / 2 - arrowSize / 2;
                    break;

                case ToolTipPlacement.Right:
                case ToolTipPlacement.RightStart:
                case ToolTipPlacement.RightEnd:
                    arrowX = bounds.Left;
                    arrowY = bounds.Top + bounds.Height / 2 - arrowSize / 2;
                    break;
            }

            return new Rectangle(arrowX, arrowY, arrowSize, arrowSize);
        }

        /// <summary>
        /// Calculates optimal tooltip size based on content.
        /// Uses StyleTypography for font resolution instead of hard-coded fonts.
        /// </summary>
        public static Size CalculateOptimalSize(
            Graphics g,
            ToolTipConfig config,
            int padding,
            int spacing,
            int minWidth,
            int maxWidth)
        {
            int width = minWidth;
            int height = padding * 2;

            // Resolve fonts from StyleTypography
            var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
            string family = ResolvePrimaryFontFamily(style);

            // Measure title
            if (!string.IsNullOrEmpty(config.Title))
            {
                float titleSize = StyleTypography.GetFontSize(style) - 2.5f;
                var titleFont = BeepFontManager.GetCachedFont(family, titleSize, FontStyle.Bold)
                                ?? new Font("Segoe UI", 10.5f, FontStyle.Bold);
                var titleMeasured = TextRenderer.MeasureText(g, config.Title, titleFont);
                width = Math.Max(width, (int)titleMeasured.Width + padding * 2);
                height += (int)titleMeasured.Height + spacing;
            }

            // Measure text
            if (!string.IsNullOrEmpty(config.Text))
            {
                float bodySize = StyleTypography.GetFontSize(style) - 4f;
                var textFont = config.Font
                    ?? BeepFontManager.GetCachedFont(family, bodySize, FontStyle.Regular)
                    ?? new Font("Segoe UI", 9.5f);
                var textSize = TextRenderer.MeasureText(g, config.Text, textFont, new Size(maxWidth - padding * 2, int.MaxValue), TextFormatFlags.WordBreak);
                width = Math.Max(width, Math.Min((int)textSize.Width + padding * 2, maxWidth));
                height += (int)textSize.Height;
            }

            // Add icon/image space if present
            if (config.Icon != null || !string.IsNullOrEmpty(config.IconPath) || !string.IsNullOrEmpty(config.ImagePath))
            {
                int iconSize = config.MaxImageSize.Height > 0
                    ? config.MaxImageSize.Height
                    : ResolveIconSize(config);
                width += iconSize + spacing;
                height = Math.Max(height, iconSize + padding * 2);
            }

            // Add footer space for shortcut keys
            if (config.Shortcuts != null && config.Shortcuts.Count > 0)
            {
                height += ResolveFooterHeight(config, spacing) + spacing; // divider + footer
            }

            // Constrain to min/max
            width = Math.Max(minWidth, Math.Min(width, maxWidth));
            height = Math.Max(GetMinHeight(config), height);

            return new Size(width, height);
        }

        #region Private Helpers

        /// <summary>
        /// Resolves icon size from StyleSpacing for the config's style.
        /// </summary>
        private static int ResolveIconSize(ToolTipConfig config)
        {
            var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
            return StyleSpacing.GetIconSize(style);
        }

        /// <summary>
        /// Measures title text height dynamically using Graphics when available.
        /// Falls back to a style-aware estimate based on font metrics.
        /// </summary>
        private static int MeasureTitleHeight(Graphics g, ToolTipConfig config, int availableWidth)
        {
            if (string.IsNullOrEmpty(config.Title))
                return 0;

            var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
            string family = ResolvePrimaryFontFamily(style);
            float titleSize = StyleTypography.GetFontSize(style) - 2.5f;
            var titleFont = BeepFontManager.GetCachedFont(family, titleSize, FontStyle.Bold)
                            ?? new Font("Segoe UI", 10.5f, FontStyle.Bold);

            if (g != null)
            {
                var measured = TextRenderer.MeasureText(g, config.Title, titleFont,
                    new Size(availableWidth, int.MaxValue), TextFormatFlags.WordBreak);
                return (int)Math.Ceiling((double)measured.Height);
            }

            // Fallback: estimate from font height × line-height multiplier
            float lineHeight = StyleTypography.GetLineHeight(style);
            return (int)Math.Ceiling(titleFont.GetHeight() * lineHeight);
        }

        /// <summary>
        /// Calculates footer height for shortcut key badges.
        /// </summary>
        private static int ResolveFooterHeight(ToolTipConfig config, int spacing)
        {
            // Shortcut key badges are typically 20-24 px tall
            return 24;
        }

        /// <summary>
        /// Minimum height based on layout variant to prevent collapse.
        /// </summary>
        private static int GetMinHeight(ToolTipConfig config)
        {
            var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
            int itemHeight = StyleSpacing.GetItemHeight(style);

            return config.LayoutVariant switch
            {
                ToolTipLayoutVariant.Rich => itemHeight + 16,
                ToolTipLayoutVariant.Card => itemHeight * 2,
                ToolTipLayoutVariant.Preview => (config.PreviewImageSize.Height > 0 ? config.PreviewImageSize.Height : 80) + 60,
                ToolTipLayoutVariant.Tour => itemHeight * 3,
                ToolTipLayoutVariant.Shortcut => itemHeight,
                _ => Math.Max(32, itemHeight - 8) // Simple
            };
        }

        /// <summary>
        /// Extracts the first font family name from a comma-separated list.
        /// </summary>
        private static string ResolvePrimaryFontFamily(BeepControlStyle style)
        {
            string families = StyleTypography.GetFontFamily(style);
            int comma = families.IndexOf(',');
            return comma > 0 ? families.Substring(0, comma).Trim() : families.Trim();
        }

        #endregion
    }

    /// <summary>
    /// Layout metrics for tooltip elements.
    /// Expanded with Footer, Divider, CloseButton rects for rich content layouts.
    /// </summary>
    public class ToolTipLayoutMetrics
    {
        public Rectangle IconRect { get; set; } = Rectangle.Empty;
        public Rectangle TitleRect { get; set; } = Rectangle.Empty;
        public Rectangle TextRect { get; set; } = Rectangle.Empty;
        public Rectangle ArrowRect { get; set; } = Rectangle.Empty;

        /// <summary>Footer area for shortcut badges, action links.</summary>
        public Rectangle FooterRect { get; set; } = Rectangle.Empty;

        /// <summary>Horizontal divider between body and footer.</summary>
        public Rectangle DividerRect { get; set; } = Rectangle.Empty;

        /// <summary>Close button (×) rect, populated when config.Closable == true.</summary>
        public Rectangle CloseButtonRect { get; set; } = Rectangle.Empty;

        /// <summary>Resolved content padding from StyleSpacing.</summary>
        public int ContentPadding { get; set; }

        /// <summary>Resolved item spacing from StyleSpacing.</summary>
        public int ItemSpacing { get; set; }
    }
}
