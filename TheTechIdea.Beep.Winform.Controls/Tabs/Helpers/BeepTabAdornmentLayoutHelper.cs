using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Calculates the bounds for all adornment elements inside a single tab header item rectangle.
    ///
    /// Allocation order (left-to-right in horizontal headers, with RTL awareness if needed):
    ///   [icon] [title text] [subtext] [badge] [dirty-dot | busy-spinner] [close button]
    ///
    /// The helper respects priority rules when space is constrained:
    ///   1. Keep selection visible (close button retained for selected/dirty).
    ///   2. Collapse subtext before main text.
    ///   3. Collapse labels before icons only when configured.
    ///   4. Move items into overflow before clipping text.
    /// </summary>
    public static class BeepTabAdornmentLayoutHelper
    {
        // ── Sizing constants (logical pixels, DPI-independent) ────────────────

        private const int IconSize = 16;
        private const int BadgePaddingH = 4;
        private const int BadgePaddingV = 2;
        private const int BadgeMinWidth = 16;
        private const int BadgeDotSize = 8;
        private const int DirtyDotSize = 6;
        private const int BusySize = 12;
        private const int AdornmentGap = 3;
        private const int CloseSize = 14;
        private const int CloseGap = 4;
        private const int EdgePadding = 6;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Populates all adornment-related bounds on <paramref name="layout"/> given
        /// the tab's outer bounds, font, and current adornment state.
        /// The caller is responsible for setting <c>layout.Bounds</c> and
        /// <c>layout.HasCloseButton</c> before calling this method.
        /// </summary>
        public static void Calculate(
            BeepTabHeaderItemLayout layout,
            Font font,
            bool showCloseButton,
            bool isHorizontal = true)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            font = TabFontHelpers.ResolveSafeFont(font);

            if (layout.Bounds.IsEmpty)
            {
                ClearBounds(layout);
                return;
            }

            BeepTabItem item = layout.Item;
            BeepTabAdornmentState adornment = item.GetAdornmentState();

            if (isHorizontal)
            {
                CalculateHorizontal(layout, font, showCloseButton, adornment);
            }
            else
            {
                CalculateVertical(layout, font, showCloseButton, adornment);
            }
        }

        /// <summary>
        /// Returns the minimum width that the adornment set needs in a horizontal header.
        /// Useful for measuring tabs during overflow calculation.
        /// </summary>
        public static int MeasureHorizontalAdornmentWidth(
            BeepTabAdornmentState adornment,
            bool showCloseButton)
        {
            int width = EdgePadding;

            if (adornment.HasIcon)
            {
                width += IconSize + AdornmentGap;
            }

            if (adornment.HasBadge)
            {
                width += adornment.BadgeKind == BeepTabBadgeKind.Dot
                    ? BadgeDotSize + AdornmentGap
                    : BadgeMinWidth + AdornmentGap;
            }

            if (adornment.IsDirty)
            {
                width += DirtyDotSize + AdornmentGap;
            }
            else if (adornment.IsBusy)
            {
                width += BusySize + AdornmentGap;
            }

            if (showCloseButton)
            {
                width += CloseSize + CloseGap;
            }

            width += EdgePadding;
            return width;
        }

        // ── Horizontal layout ─────────────────────────────────────────────────

        private static void CalculateHorizontal(
            BeepTabHeaderItemLayout layout,
            Font font,
            bool showCloseButton,
            BeepTabAdornmentState adornment)
        {
            Rectangle bounds = layout.Bounds;
            int centerY = bounds.Top + bounds.Height / 2;

            // ── Cursor tracks the leading edge ───────────────────────────────
            int left = bounds.Left + EdgePadding;
            int right = bounds.Right - EdgePadding;

            // ── Close button – always reserve from the right ─────────────────
            bool resolvedClose = showCloseButton && layout.HasCloseButton;
            if (resolvedClose)
            {
                int y = centerY - CloseSize / 2;
                layout.CloseButtonBounds = new Rectangle(right - CloseSize, y, CloseSize, CloseSize);
                right -= CloseSize + CloseGap;
            }
            else
            {
                layout.CloseButtonBounds = Rectangle.Empty;
            }

            // ── Dirty dot / busy – reserve from the right after close ────────
            if (adornment.IsDirty)
            {
                int y = centerY - DirtyDotSize / 2;
                layout.DirtyMarkerBounds = new Rectangle(right - DirtyDotSize, y, DirtyDotSize, DirtyDotSize);
                layout.BusyIndicatorBounds = Rectangle.Empty;
                right -= DirtyDotSize + AdornmentGap;
            }
            else if (adornment.IsBusy)
            {
                int y = centerY - BusySize / 2;
                layout.BusyIndicatorBounds = new Rectangle(right - BusySize, y, BusySize, BusySize);
                layout.DirtyMarkerBounds = Rectangle.Empty;
                right -= BusySize + AdornmentGap;
            }
            else
            {
                layout.DirtyMarkerBounds = Rectangle.Empty;
                layout.BusyIndicatorBounds = Rectangle.Empty;
            }

            // ── Badge – reserve from the right ───────────────────────────────
            if (adornment.HasBadge)
            {
                int fontHeight = TabFontHelpers.GetSafeFontHeight(font);
                int badgeW = adornment.BadgeKind == BeepTabBadgeKind.Dot
                    ? BadgeDotSize
                    : Math.Max(BadgeMinWidth, MeasureBadgeTextWidth(adornment.BadgeText, font) + BadgePaddingH * 2);
                int badgeH = Math.Max(BadgeDotSize, fontHeight - 2);
                int y = centerY - badgeH / 2;
                layout.BadgeBounds = new Rectangle(right - badgeW, y, badgeW, badgeH);
                right -= badgeW + AdornmentGap;
            }
            else
            {
                layout.BadgeBounds = Rectangle.Empty;
            }

            // ── Icon – leading edge ───────────────────────────────────────────
            if (adornment.HasIcon)
            {
                int y = centerY - IconSize / 2;
                layout.IconBounds = new Rectangle(left, y, IconSize, IconSize);
                left += IconSize + AdornmentGap;
            }
            else
            {
                layout.IconBounds = Rectangle.Empty;
            }

            // ── Text area (title + optional subtext stacked) ─────────────────
            int textAreaLeft = left;
            int textAreaRight = right;
            int textAreaWidth = Math.Max(0, textAreaRight - textAreaLeft);

            if (adornment.HasSubText)
            {
                int fontHeight = TabFontHelpers.GetSafeFontHeight(font);
                int titleH = fontHeight;
                int subH = Math.Max(fontHeight - 2, 10);
                int stackH = titleH + subH + 2;
                int stackTop = centerY - stackH / 2;

                layout.TextBounds = new Rectangle(textAreaLeft, stackTop, textAreaWidth, titleH);
                layout.SubTextBounds = new Rectangle(textAreaLeft, stackTop + titleH + 2, textAreaWidth, subH);
            }
            else
            {
                int titleH = TabFontHelpers.GetSafeFontHeight(font);
                layout.TextBounds = new Rectangle(textAreaLeft, centerY - titleH / 2, textAreaWidth, titleH);
                layout.SubTextBounds = Rectangle.Empty;
            }
        }

        // ── Vertical layout ───────────────────────────────────────────────────

        private static void CalculateVertical(
            BeepTabHeaderItemLayout layout,
            Font font,
            bool showCloseButton,
            BeepTabAdornmentState adornment)
        {
            Rectangle bounds = layout.Bounds;
            int centerX = bounds.Left + bounds.Width / 2;

            int top = bounds.Top + EdgePadding;
            int bottom = bounds.Bottom - EdgePadding;

            // Close button at the very bottom
            bool resolvedClose = showCloseButton && layout.HasCloseButton;
            if (resolvedClose)
            {
                int x = centerX - CloseSize / 2;
                layout.CloseButtonBounds = new Rectangle(x, bottom - CloseSize, CloseSize, CloseSize);
                bottom -= CloseSize + CloseGap;
            }
            else
            {
                layout.CloseButtonBounds = Rectangle.Empty;
            }

            // Dirty/busy below text
            if (adornment.IsDirty)
            {
                layout.DirtyMarkerBounds = new Rectangle(centerX - DirtyDotSize / 2, bottom - DirtyDotSize, DirtyDotSize, DirtyDotSize);
                layout.BusyIndicatorBounds = Rectangle.Empty;
                bottom -= DirtyDotSize + AdornmentGap;
            }
            else if (adornment.IsBusy)
            {
                layout.BusyIndicatorBounds = new Rectangle(centerX - BusySize / 2, bottom - BusySize, BusySize, BusySize);
                layout.DirtyMarkerBounds = Rectangle.Empty;
                bottom -= BusySize + AdornmentGap;
            }
            else
            {
                layout.DirtyMarkerBounds = Rectangle.Empty;
                layout.BusyIndicatorBounds = Rectangle.Empty;
            }

            // Badge below text
            if (adornment.HasBadge)
            {
                int fontHeight = TabFontHelpers.GetSafeFontHeight(font);
                int badgeW = adornment.BadgeKind == BeepTabBadgeKind.Dot
                    ? BadgeDotSize
                    : Math.Max(BadgeMinWidth, MeasureBadgeTextWidth(adornment.BadgeText, font) + BadgePaddingH * 2);
                int badgeH = Math.Max(BadgeDotSize, fontHeight - 2);
                layout.BadgeBounds = new Rectangle(centerX - badgeW / 2, bottom - badgeH, badgeW, badgeH);
                bottom -= badgeH + AdornmentGap;
            }
            else
            {
                layout.BadgeBounds = Rectangle.Empty;
            }

            // Icon at the top
            if (adornment.HasIcon)
            {
                layout.IconBounds = new Rectangle(centerX - IconSize / 2, top, IconSize, IconSize);
                top += IconSize + AdornmentGap;
            }
            else
            {
                layout.IconBounds = Rectangle.Empty;
            }

            // Text
            int textAreaHeight = Math.Max(0, bottom - top);
            int titleH = TabFontHelpers.GetSafeFontHeight(font);
            if (adornment.HasSubText)
            {
                int subH = Math.Max(titleH - 2, 10);
                int stackH = titleH + subH + 2;
                int stackTop = top + (textAreaHeight - stackH) / 2;
                layout.TextBounds = new Rectangle(bounds.Left + EdgePadding, stackTop, bounds.Width - EdgePadding * 2, titleH);
                layout.SubTextBounds = new Rectangle(bounds.Left + EdgePadding, stackTop + titleH + 2, bounds.Width - EdgePadding * 2, subH);
            }
            else
            {
                int textTop = top + (textAreaHeight - titleH) / 2;
                layout.TextBounds = new Rectangle(bounds.Left + EdgePadding, textTop, bounds.Width - EdgePadding * 2, titleH);
                layout.SubTextBounds = Rectangle.Empty;
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private static void ClearBounds(BeepTabHeaderItemLayout layout)
        {
            layout.TextBounds = Rectangle.Empty;
            layout.CloseButtonBounds = Rectangle.Empty;
            layout.IconBounds = Rectangle.Empty;
            layout.SubTextBounds = Rectangle.Empty;
            layout.BadgeBounds = Rectangle.Empty;
            layout.DirtyMarkerBounds = Rectangle.Empty;
            layout.BusyIndicatorBounds = Rectangle.Empty;
        }

        private static int MeasureBadgeTextWidth(string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            return TabFontHelpers.MeasureTextWidthSafe(text, font);
        }
    }
}
