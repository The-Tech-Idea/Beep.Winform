using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Shared DPI-aware metrics for display container tab header layout and painting.
    /// Keeps tab measurement and rendering aligned under the same spacing contract.
    /// </summary>
    /// <summary>Every drawn and interactive region inside one tab, produced together.</summary>
    internal struct TabSlotLayout
    {
        /// <summary>Leading icon, vertically centred. Empty when the tab has no icon.</summary>
        public Rectangle IconRect;

        /// <summary>The caption. Already excludes every other slot.</summary>
        public Rectangle TextRect;

        /// <summary>Unsaved-changes dot. Empty unless the tab is modified.</summary>
        public Rectangle ModifiedDotRect;

        /// <summary>Notification pill. Empty when there is no badge text.</summary>
        public Rectangle BadgeRect;

        /// <summary>Where the close glyph is painted - centred inside <see cref="CloseHitRect"/>.</summary>
        public Rectangle CloseGlyphRect;

        /// <summary>What the close affordance responds to. Deliberately larger than the glyph.</summary>
        public Rectangle CloseHitRect;
    }

    internal static class TabHeaderMetrics
    {
        public static int HorizontalPadding(Control ownerControl) => DpiScalingHelper.ScaleValue(10, ownerControl);

        public static int VerticalPadding(Control ownerControl) => DpiScalingHelper.ScaleValue(6, ownerControl);

        public static int TabGap(Control ownerControl) => DpiScalingHelper.ScaleValue(4, ownerControl);

        /// <summary>Width reserved for the close affordance. This is also its hit target, so it
        /// is held at or above <see cref="MinTouchTarget"/> rather than the former 22px.</summary>
        public static int CloseButtonSlotWidth(Control ownerControl)
            => Math.Max(DpiScalingHelper.ScaleValue(22, ownerControl), MinTouchTarget(ownerControl));

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

        /// <summary>
        /// The font a badge is drawn with. Resolved in one place because the tab's width depends on
        /// it: measuring the badge with a different font than it is painted with reserves the wrong
        /// amount of space, which is how the badge came to overlap its neighbours.
        /// </summary>
        public static Font ResolveBadgeFont(IBeepTheme theme, Font fallback)
            => BeepThemesManager.ToFont(theme?.LabelSmall)
               ?? BeepThemesManager.ToFont(theme?.BodySmall)
               ?? fallback;

        /// <summary>Width of a badge caption, measured exactly as the badge is drawn.</summary>
        public static int MeasureBadgeTextWidth(string badgeText, IBeepTheme theme, Font fallback)
        {
            if (string.IsNullOrEmpty(badgeText)) return 0;

            return TextRenderer.MeasureText(badgeText, ResolveBadgeFont(theme, fallback),
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;
        }

        /// <summary>Total width a badge occupies inside a tab, including its leading gap.</summary>
        public static int BadgeSlotWidth(int badgeTextWidth, Control ownerControl)
        {
            if (badgeTextWidth <= 0) return 0;
            int w = Math.Max(BadgeMinWidth(ownerControl), badgeTextWidth + BadgeHPadding(ownerControl) * 2);
            return w + DpiScalingHelper.ScaleValue(4, ownerControl);
        }

        /// <summary>Smallest comfortable pointer target, per WCAG 2.5.5 / platform guidance.</summary>
        public static int MinTouchTarget(Control ownerControl) => DpiScalingHelper.ScaleValue(24, ownerControl);

        /// <summary>Diameter of the modified/unsaved dot.</summary>
        public static int ModifiedDotSize(Control ownerControl) => Math.Max(3, DpiScalingHelper.ScaleValue(4, ownerControl));

        /// <summary>Total width the modified dot occupies, including its leading gap.</summary>
        public static int ModifiedDotSlotWidth(Control ownerControl)
            => ModifiedDotSize(ownerControl) + DpiScalingHelper.ScaleValue(4, ownerControl);

        /// <summary>
        /// Every interactive and drawn region inside one tab, computed together.
        /// </summary>
        /// <remarks>
        /// These used to be produced by four independent methods that each re-derived their own
        /// position from <c>bounds</c>, and they disagreed: the badge was anchored at
        /// <c>Right - w - 4</c>, which is inside the close slot; the modified dot was placed at
        /// <c>TextRect.Right + 2</c>, in the same contested space; and the text rect reserved the
        /// icon and close slots but not the badge, so a badge overdrew the caption's tail.
        ///
        /// One right-to-left cursor removes the possibility of disagreement: each slot consumes what
        /// it needs, and the caption receives exactly what is left.
        /// </remarks>
        public static TabSlotLayout GetSlotLayout(Rectangle bounds, bool hasIcon, bool showCloseButton,
            int badgeTextWidth, bool isModified, Control ownerControl, bool isPinned = false)
        {
            int hPad = HorizontalPadding(ownerControl);
            int vPad = VerticalPadding(ownerControl);

            var layout = new TabSlotLayout();

            // A pinned tab is icon-only, and that contract is enforced here so no draw site has to
            // remember it. Its width comes from PinnedTabWidth, which has room for nothing else.
            if (isPinned)
            {
                int pinIcon = IconSize(ownerControl);
                layout.IconRect = new Rectangle(
                    bounds.X + (bounds.Width - pinIcon) / 2,
                    bounds.Y + (bounds.Height - pinIcon) / 2,
                    pinIcon, pinIcon);
                return layout;
            }

            // Left edge: the icon, vertically centred.
            int leftX = bounds.X + hPad;
            if (hasIcon)
            {
                int iconSize = IconSize(ownerControl);
                layout.IconRect = new Rectangle(leftX, bounds.Y + (bounds.Height - iconSize) / 2, iconSize, iconSize);
                leftX += IconSlotWidth(ownerControl);
            }

            // Right edge, walking inward.
            int rightX = bounds.Right - hPad;

            if (showCloseButton)
            {
                int slot = CloseButtonSlotWidth(ownerControl);
                int glyph = CloseButtonSize(ownerControl);

                // The hit target is the whole slot, expanded to the tab's height. The glyph is
                // centred inside it -- previously the glyph rect served as both, so a 13px square
                // was the entire clickable area and it sat flush against the slot's left edge
                // rather than in its middle.
                layout.CloseHitRect = new Rectangle(rightX - slot, bounds.Y, slot, bounds.Height);
                layout.CloseGlyphRect = new Rectangle(
                    rightX - slot + (slot - glyph) / 2,
                    bounds.Y + (bounds.Height - glyph) / 2,
                    glyph, glyph);
                rightX -= slot;
            }

            if (badgeTextWidth > 0)
            {
                int h = BadgeHeight(ownerControl);
                int w = Math.Max(BadgeMinWidth(ownerControl), badgeTextWidth + BadgeHPadding(ownerControl) * 2);
                layout.BadgeRect = new Rectangle(rightX - w, bounds.Y + (bounds.Height - h) / 2, w, h);
                rightX -= w + DpiScalingHelper.ScaleValue(4, ownerControl);
            }

            if (isModified)
            {
                int dot = ModifiedDotSize(ownerControl);
                layout.ModifiedDotRect = new Rectangle(
                    rightX - dot, bounds.Y + (bounds.Height - dot) / 2, dot, dot);
                rightX -= ModifiedDotSlotWidth(ownerControl);
            }

            // Whatever survives between the icon and the right-hand cluster is the caption.
            layout.TextRect = new Rectangle(
                leftX,
                bounds.Y + vPad,
                Math.Max(0, rightX - leftX),
                Math.Max(0, bounds.Height - (vPad * 2)));

            return layout;
        }
    }
}
