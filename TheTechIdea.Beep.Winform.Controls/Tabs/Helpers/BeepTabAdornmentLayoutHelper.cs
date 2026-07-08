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
                layout.TextBounds = new Rectangle(left, y, Math.Max(0, layout.Bounds.Right - left - edgeP), textHeight);
                if (!string.IsNullOrEmpty(layout.Item?.SubText))
                { layout.SubTextBounds = new Rectangle(left, y + textHeight, layout.TextBounds.Width, textHeight); }
                if (hasBadge) { var bw = isDot ? badgeDot : badgeMinW; layout.BadgeBounds = new Rectangle(layout.Bounds.Right - edgeP - bw, y, bw, textHeight); }
                if (isDirty) layout.DirtyMarkerBounds = new Rectangle(layout.Bounds.Right - edgeP - dirtyDot, y, dirtyDot, dirtyDot);
                else if (isBusy) layout.BusyIndicatorBounds = new Rectangle(layout.Bounds.Right - edgeP - busyS, y, busyS, busyS);
                if (showCloseButton) layout.CloseButtonBounds = new Rectangle(layout.Bounds.Right - edgeP - closeS, y, closeS, closeS);
            }
            else
            {
                int top = layout.Bounds.Y + edgeP;
                int cx = layout.Bounds.X + layout.Bounds.Width / 2;
                if (hasIcon) { layout.IconBounds = new Rectangle(cx - iconS / 2, top, iconS, iconS); top += iconS + gap; }
                layout.TextBounds = new Rectangle(layout.Bounds.X + edgeP, top, Math.Max(0, layout.Bounds.Width - edgeP * 2), textHeight);
                if (!string.IsNullOrEmpty(layout.Item?.SubText)) { top += textHeight; layout.SubTextBounds = new Rectangle(layout.TextBounds.X, top, layout.TextBounds.Width, textHeight); }
                if (hasBadge) { var bw = isDot ? badgeDot : badgeMinW; layout.BadgeBounds = new Rectangle(cx - bw / 2, layout.Bounds.Bottom - edgeP - textHeight, bw, textHeight); }
                if (isDirty) layout.DirtyMarkerBounds = new Rectangle(cx - dirtyDot / 2, layout.Bounds.Bottom - edgeP - dirtyDot, dirtyDot, dirtyDot);
                else if (isBusy) layout.BusyIndicatorBounds = new Rectangle(cx - busyS / 2, layout.Bounds.Bottom - edgeP - busyS, busyS, busyS);
                if (showCloseButton) layout.CloseButtonBounds = new Rectangle(cx - closeS / 2, layout.Bounds.Bottom - edgeP - closeS, closeS, closeS);
            }
        }

        public static int MeasureHorizontalAdornmentWidth(BeepTabAdornmentState adornment, bool showCloseButton, Control c = null)
        {
            int w = S(EdgePadding, c);
            if (adornment.HasIcon) w += S(IconSize, c) + S(AdornmentGap, c);
            if (adornment.HasBadge) w += (adornment.BadgeKind == BeepTabBadgeKind.Dot ? S(BadgeDotSize, c) : S(BadgeMinWidth, c)) + S(AdornmentGap, c);
            if (adornment.IsDirty) w += S(DirtyDotSize, c) + S(AdornmentGap, c);
            else if (adornment.IsBusy) w += S(BusySize, c) + S(AdornmentGap, c);
            if (showCloseButton) w += S(CloseSize, c) + S(CloseGap, c);
            return w;
        }
    }
}
