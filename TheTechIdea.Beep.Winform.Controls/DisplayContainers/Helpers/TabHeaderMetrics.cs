using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Shared DPI-aware metrics for display container tab header layout and painting.
    /// Keeps tab measurement and rendering aligned under the same spacing contract.
    /// </summary>
    internal static class TabHeaderMetrics
    {
        public static int HorizontalPadding(Control ownerControl) => DpiScalingHelper.ScaleValue(10, ownerControl);

        public static int VerticalPadding(Control ownerControl) => DpiScalingHelper.ScaleValue(6, ownerControl);

        public static int TabGap(Control ownerControl) => DpiScalingHelper.ScaleValue(4, ownerControl);

        public static int CloseButtonSlotWidth(Control ownerControl) => DpiScalingHelper.ScaleValue(22, ownerControl);

        public static int CloseButtonSize(Control ownerControl) => DpiScalingHelper.ScaleValue(13, ownerControl);

        public static int UtilityButtonSize(Control ownerControl) => DpiScalingHelper.ScaleValue(28, ownerControl);

        public static int UtilityButtonPadding(Control ownerControl) => DpiScalingHelper.ScaleValue(4, ownerControl);

        public static int UtilityButtonsReservedWidth(Control ownerControl) => DpiScalingHelper.ScaleValue(140, ownerControl);

        public static int NewTabButtonReservedWidth(Control ownerControl) => DpiScalingHelper.ScaleValue(40, ownerControl);

        public static int ScrollAreaOffset(Control ownerControl) => DpiScalingHelper.ScaleValue(40, ownerControl);

        public static int IndicatorThickness(Control ownerControl) => Math.Max(2, DpiScalingHelper.ScaleValue(3, ownerControl));

        public static int IndicatorInset(Control ownerControl) => Math.Max(0, DpiScalingHelper.ScaleValue(4, ownerControl));

        public static int TextContentPadding(Control ownerControl) => HorizontalPadding(ownerControl) * 2;

        // ── Icon metrics ─────────────────────────────────────────────────
        /// <summary>Total width reserved for the icon slot (icon + trailing gap).</summary>
        public static int IconSlotWidth(Control ownerControl) => DpiScalingHelper.ScaleValue(20, ownerControl);

        /// <summary>Square edge-length of the rendered icon (fits inside IconSlotWidth).</summary>
        public static int IconSize(Control ownerControl) => DpiScalingHelper.ScaleValue(16, ownerControl);

        // ── Badge metrics ────────────────────────────────────────────────
        /// <summary>Height of the notification badge pill.</summary>
        public static int BadgeHeight(Control ownerControl) => DpiScalingHelper.ScaleValue(16, ownerControl);

        /// <summary>Minimum width of the badge pill (ensures circle for single chars).</summary>
        public static int BadgeMinWidth(Control ownerControl) => DpiScalingHelper.ScaleValue(16, ownerControl);

        /// <summary>Horizontal padding inside the badge pill text.</summary>
        public static int BadgeHPadding(Control ownerControl) => DpiScalingHelper.ScaleValue(4, ownerControl);

        // ── Pinned tab metrics ───────────────────────────────────────────
        /// <summary>Width of a pinned (icon-only) tab.</summary>
        public static int PinnedTabWidth(Control ownerControl) => DpiScalingHelper.ScaleValue(38, ownerControl);

        /// <summary>Backward-compatible overload (no icon).</summary>
        public static Rectangle GetTextBounds(Rectangle bounds, bool showCloseButton, Control ownerControl)
            => GetTextBounds(bounds, showCloseButton, false, ownerControl);

        /// <summary>
        /// Calculates the text drawing rectangle inside a tab, optionally reserving
        /// space for a leading icon.
        /// </summary>
        public static Rectangle GetTextBounds(Rectangle bounds, bool showCloseButton, bool hasIcon, Control ownerControl)
        {
            int hPad = HorizontalPadding(ownerControl);
            int vPad = VerticalPadding(ownerControl);
            int closeSlot = showCloseButton ? CloseButtonSlotWidth(ownerControl) : 0;
            int iconSlot = hasIcon ? IconSlotWidth(ownerControl) : 0;

            return new Rectangle(
                bounds.X + hPad + iconSlot,
                bounds.Y + vPad,
                Math.Max(0, bounds.Width - (hPad * 2) - closeSlot - iconSlot),
                Math.Max(0, bounds.Height - (vPad * 2)));
        }

        /// <summary>Returns the square bounds for rendering a tab icon via StyledImagePainter.</summary>
        public static Rectangle GetIconBounds(Rectangle tabBounds, Control ownerControl)
        {
            int hPad = HorizontalPadding(ownerControl);
            int size = IconSize(ownerControl);
            return new Rectangle(
                tabBounds.X + hPad,
                tabBounds.Y + (tabBounds.Height - size) / 2,
                size, size);
        }

        /// <summary>
        /// Returns the bounds for a notification badge pill anchored near the top-right of the tab.
        /// The <paramref name="badgeTextWidth"/> is measured externally and passed in.
        /// </summary>
        public static Rectangle GetBadgeBounds(Rectangle tabBounds, int badgeTextWidth, Control ownerControl)
        {
            int h = BadgeHeight(ownerControl);
            int minW = BadgeMinWidth(ownerControl);
            int hPadInner = BadgeHPadding(ownerControl);
            int w = Math.Max(minW, badgeTextWidth + hPadInner * 2);

            int x = tabBounds.Right - w - DpiScalingHelper.ScaleValue(4, ownerControl);
            int y = tabBounds.Y + DpiScalingHelper.ScaleValue(2, ownerControl);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle GetCloseButtonBounds(Rectangle bounds, Control ownerControl)
        {
            int closeSlot = CloseButtonSlotWidth(ownerControl);
            int closeSize = CloseButtonSize(ownerControl);

            return new Rectangle(
                bounds.Right - closeSlot,
                bounds.Y + (bounds.Height - closeSize) / 2,
                closeSize,
                closeSize);
        }
    }
}