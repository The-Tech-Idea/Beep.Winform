using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    public static class BeepTabAdornmentLayoutHelper
    {
        private const int IconSize = 16, BadgePaddingH = 4, BadgeMinWidth = 16;
        private const int BadgeDotSize = 8, DirtyDotSize = 6, BusySize = 12;
        private const int AdornmentGap = 3, CloseSize = 14, CloseGap = 4, EdgePadding = 6;

        private static int S(int v, Control c) => DpiScalingHelper.ScaleValue(v, c);

        public static void Calculate(BeepTabHeaderItemLayout layout, Font font,
            bool showCloseButton, bool isHorizontal = true, Control c = null)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));
            font = TabFontHelpers.ResolveSafeFont(font);

            int iconS = S(IconSize, c), badgePadH = S(BadgePaddingH, c), badgeMinW = S(BadgeMinWidth, c);
            int badgeDot = S(BadgeDotSize, c), dirtyDot = S(DirtyDotSize, c), busyS = S(BusySize, c);
            int gap = S(AdornmentGap, c), closeS = S(CloseSize, c), closeG = S(CloseGap, c), edgeP = S(EdgePadding, c);

            var st = layout.Item?.GetAdornmentState() ?? BeepTabAdornmentState.Empty;
            bool hasIcon = layout.Item?.HasIcon == true;
            bool hasBadge = st.HasBadge;
            bool isDot = st.BadgeKind == BeepTabBadgeKind.Dot;
            bool isDirty = st.IsDirty;
            bool isBusy = st.IsBusy;
            int textHeight = Math.Max(1, font.Height);

            if (isHorizontal)
            {
                int left = layout.Bounds.X + edgeP;
                int y = layout.Bounds.Y + Math.Max(0, (layout.Bounds.Height - textHeight) / 2);
                if (hasIcon) { layout.IconBounds = new Rectangle(left, y, iconS, iconS); left += iconS + gap; }

                // Adornments are placed right-to-left along a cursor, each consuming its own space.
                // Every one of them used to be positioned at `Bounds.Right - edgeP - size`, i.e. the
                // *same slot* — so the close button, dirty dot, busy ring and badge were all drawn on
                // top of one another. MeasureHorizontalAdornmentWidth had always reserved room for
                // each of them cumulatively, so the tab was wide enough; the layout simply never used
                // the space. A tab with a badge and a close button showed one of them.
                int CenterY(int size) => layout.Bounds.Y + Math.Max(0, (layout.Bounds.Height - size) / 2);
                int right = layout.Bounds.Right - edgeP;

                if (showCloseButton)
                {
                    layout.CloseButtonBounds = new Rectangle(right - closeS, CenterY(closeS), closeS, closeS);
                    right -= closeS + closeG;
                }

                if (isDirty)
                {
                    layout.DirtyMarkerBounds = new Rectangle(right - dirtyDot, CenterY(dirtyDot), dirtyDot, dirtyDot);
                    right -= dirtyDot + gap;
                }
                else if (isBusy)
                {
                    layout.BusyIndicatorBounds = new Rectangle(right - busyS, CenterY(busyS), busyS, busyS);
                    right -= busyS + gap;
                }

                if (hasBadge)
                {
                    int bw = isDot ? badgeDot : badgeMinW;
                    int bh = isDot ? badgeDot : textHeight;
                    layout.BadgeBounds = new Rectangle(right - bw, CenterY(bh), bw, bh);
                    right -= bw + gap;
                }

                layout.TextBounds = new Rectangle(left, y, Math.Max(0, right - left), textHeight);
                if (!string.IsNullOrEmpty(layout.Item?.SubText))
                { layout.SubTextBounds = new Rectangle(left, y + textHeight, layout.TextBounds.Width, textHeight); }
            }
            else
            {
                int top = layout.Bounds.Y + edgeP;
                int cx = layout.Bounds.X + layout.Bounds.Width / 2;
                if (hasIcon) { layout.IconBounds = new Rectangle(cx - iconS / 2, top, iconS, iconS); top += iconS + gap; }

                // The label is rotated to run *down* a vertical tab, so its box has to be tall, not
                // one line high. This used to allocate `textHeight` — about 15px — inside a ~30px
                // wide tab, so every caption ellipsised away to nothing and vertical tabs rendered
                // with no text at all, only their close glyph. Reserve the run from here down to
                // whatever sits at the bottom (close button, badge, dirty dot or busy ring).
                // No edge padding here: MeasureTab's extent already includes one edge pad via
                // MeasureHorizontalAdornmentWidth, and subtracting it at both ends cost ~6px — just
                // enough to clip the last glyph off a caption the tab had been sized to fit.
                int bottomReserve = 0;
                if (showCloseButton) bottomReserve += closeS + closeG;
                if (hasBadge) bottomReserve += (isDot ? badgeDot : textHeight) + gap;
                else if (isDirty) bottomReserve += dirtyDot + gap;
                else if (isBusy) bottomReserve += busyS + gap;

                int subTextRun = string.IsNullOrEmpty(layout.Item?.SubText) ? 0 : textHeight + gap;
                int textRun = Math.Max(0, layout.Bounds.Bottom - bottomReserve - subTextRun - top);

                layout.TextBounds = new Rectangle(
                    layout.Bounds.X + edgeP, top,
                    Math.Max(0, layout.Bounds.Width - edgeP * 2), textRun);

                if (!string.IsNullOrEmpty(layout.Item?.SubText))
                {
                    layout.SubTextBounds = new Rectangle(
                        layout.TextBounds.X, layout.TextBounds.Bottom + gap,
                        layout.TextBounds.Width, textHeight);
                }
                if (hasBadge) { var bw = isDot ? badgeDot : badgeMinW; layout.BadgeBounds = new Rectangle(cx - bw / 2, layout.Bounds.Bottom - edgeP - textHeight, bw, textHeight); }
                if (isDirty) layout.DirtyMarkerBounds = new Rectangle(cx - dirtyDot / 2, layout.Bounds.Bottom - edgeP - dirtyDot, dirtyDot, dirtyDot);
                else if (isBusy) layout.BusyIndicatorBounds = new Rectangle(cx - busyS / 2, layout.Bounds.Bottom - edgeP - busyS, busyS, busyS);
                if (showCloseButton) layout.CloseButtonBounds = new Rectangle(cx - closeS / 2, layout.Bounds.Bottom - edgeP - closeS, closeS, closeS);
            }
        }

        /// <summary>
        /// Width a tab must reserve for padding and adornments, on top of its caption.
        /// </summary>
        /// <remarks>
        /// Edge padding counts <b>twice</b> — the layout applies it at both the leading and trailing
        /// edge. Reserving it once left every tab 6px short, which went unnoticed only because the
        /// adornments were all drawn in the same slot and the caption was allowed to run underneath
        /// them. Once the adornments were laid out in sequence, the shortfall showed up immediately
        /// as an ellipsised caption on a tab that had been measured to fit it.
        /// </remarks>
        public static int MeasureHorizontalAdornmentWidth(BeepTabAdornmentState adornment, bool showCloseButton, Control c = null)
        {
            int w = S(EdgePadding, c) * 2;
            if (adornment.HasIcon) w += S(IconSize, c) + S(AdornmentGap, c);
            if (adornment.HasBadge) w += (adornment.BadgeKind == BeepTabBadgeKind.Dot ? S(BadgeDotSize, c) : S(BadgeMinWidth, c)) + S(AdornmentGap, c);
            if (adornment.IsDirty) w += S(DirtyDotSize, c) + S(AdornmentGap, c);
            else if (adornment.IsBusy) w += S(BusySize, c) + S(AdornmentGap, c);
            if (showCloseButton) w += S(CloseSize, c) + S(CloseGap, c);
            return w;
        }
    }
}
