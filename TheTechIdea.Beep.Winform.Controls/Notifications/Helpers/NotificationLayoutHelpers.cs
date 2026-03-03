using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Helpers
{
    /// <summary>
    /// Calculates layout rectangles for all notification elements.
    /// All sizes are DPI-scaled. Text heights are measured from the supplied
    /// fonts (passed in from <c>NotificationPainterBase</c>) so text never
    /// clips or overruns its container.
    /// </summary>
    public static class NotificationLayoutHelpers
    {
        // ── Internal DPI helper ───────────────────────────────────────────────
        private static int S(int v, Control c) =>
            c != null ? DpiScalingHelper.ScaleValue(v, c) : v;

        // ── Main entry point ──────────────────────────────────────────────────

        /// <summary>
        /// Calculates all layout rectangles for a notification given its bounds,
        /// layout mode, and the fonts that will actually be used to render text.
        /// </summary>
        /// <param name="titleFont">Font used for the title; null ⇒ use system default.</param>
        /// <param name="messageFont">Font used for the message; null ⇒ use system default.</param>
        public static NotificationLayoutMetrics CalculateLayout(
            Rectangle        bounds,
            NotificationLayout layout,
            bool hasIcon,
            bool hasTitle,
            bool hasMessage,
            bool hasActions,
            bool showCloseButton,
            bool showProgressBar,
            int  iconSize,
            int  padding,
            int  spacing,
            Control ownerControl,
            Font titleFont   = null,
            Font messageFont = null)
        {
            var m = layout switch
            {
                NotificationLayout.Standard  => CalculateStandard (bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing, ownerControl, titleFont, messageFont),
                NotificationLayout.Compact   => CalculateCompact  (bounds, hasIcon, hasTitle, hasMessage,             showProgressBar, iconSize, padding, spacing, ownerControl, titleFont, messageFont),
                NotificationLayout.Prominent => CalculateProminent(bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing, ownerControl, titleFont, messageFont),
                NotificationLayout.Banner    => CalculateBanner   (bounds, hasIcon, hasTitle, hasMessage,             showProgressBar, iconSize, padding, spacing, ownerControl, titleFont, messageFont),
                NotificationLayout.Toast     => CalculateToast    (bounds, hasIcon, hasTitle,                                          iconSize, padding, spacing, ownerControl, titleFont),
                // New layout types – simple sensible defaults
                NotificationLayout.Elevated  => CalculateElevated (bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing, ownerControl, titleFont, messageFont),
                NotificationLayout.Snackbar  => CalculateSnackbar (bounds, hasTitle, hasActions, padding, spacing, ownerControl, messageFont),
                NotificationLayout.StatusBar => CalculateStatusBar(bounds, hasTitle, padding, ownerControl, messageFont),
                NotificationLayout.Chip      => CalculateChip     (bounds, hasIcon, hasTitle, padding, spacing, ownerControl, titleFont),
                _                            => CalculateStandard (bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing, ownerControl, titleFont, messageFont),
            };

            // ── Close button (top-right, always DPI-scaled) ───────────────────
            if (showCloseButton && m.CloseButtonRect.IsEmpty)
            {
                int closeSz = S(20, ownerControl);
                m.CloseButtonRect = new Rectangle(
                    bounds.Right - padding - closeSz,
                    bounds.Y + padding,
                    closeSz, closeSz);
            }

            // ── Progress bar (full-width, bottom edge) ────────────────────────
            if (showProgressBar && m.ProgressBarRect.IsEmpty)
            {
                int barH = S(4, ownerControl);
                m.ProgressBarRect = new Rectangle(bounds.X, bounds.Bottom - barH, bounds.Width, barH);
            }

            return m;
        }

        // ── Font measurement helper ───────────────────────────────────────────

        /// <summary>Measures the height needed for one line of text in <paramref name="font"/>.</summary>
        private static int OneLineHeight(Font font, Control owner)
        {
            if (font == null) return S(18, owner);
            // TextRenderer.MeasureText is reliable and matches what DrawText will produce
            return TextRenderer.MeasureText("Wg", font).Height;
        }

        // ── Standard layout ───────────────────────────────────────────────────
        // [ICON | TITLE      ] [X]
        // [     | MESSAGE    ]
        // [     | ACTIONS    ]
        // [====== PROGRESS ==]

        private static NotificationLayoutMetrics CalculateStandard(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage, bool hasActions,
            bool showProgressBar, int iconSize, int padding, int spacing,
            Control owner, Font titleFont, Font messageFont)
        {
            var m = new NotificationLayoutMetrics();
            int progH = showProgressBar ? S(4, owner) : 0;
            int closeW = S(20 + spacing, owner);     // reserved for close button

            // Working area after padding
            int innerX   = bounds.X + padding;
            int innerY   = bounds.Y + padding;
            int innerW   = bounds.Width  - padding * 2 - closeW;
            int innerH   = bounds.Height - padding * 2 - progH;
            int innerBot = bounds.Y + padding + innerH;

            // Icon – vertically centred in working area
            if (hasIcon)
            {
                int iconY = bounds.Y + (bounds.Height - progH - iconSize) / 2;
                m.IconRect = new Rectangle(innerX, iconY, iconSize, iconSize);
                innerX += iconSize + spacing;
                innerW -= iconSize + spacing;
            }

            int curY = bounds.Y + padding;

            // Title – measured single-line height
            int titleH = hasTitle ? OneLineHeight(titleFont, owner) : 0;
            if (hasTitle)
            {
                m.TitleRect = new Rectangle(innerX, curY, innerW, titleH);
                curY += titleH + S(4, owner);
            }

            // Actions – aligned to bottom
            int actionH = 0;
            if (hasActions)
            {
                actionH = S(32, owner);
                m.ActionsRect = new Rectangle(innerX, innerBot - actionH, innerW, actionH);
            }

            // Message fills the remaining space between title bottom and actions/bottom
            if (hasMessage)
            {
                int msgBot = hasActions ? m.ActionsRect.Top - spacing : innerBot;
                int msgH   = Math.Max(S(16, owner), msgBot - curY);
                if (msgH > 0)
                    m.MessageRect = new Rectangle(innerX, curY, innerW, msgH);
            }

            return m;
        }

        // ── Compact layout ────────────────────────────────────────────────────
        // [ICON | TITLE  MSG ] [X]   — all on one line, fits small height

        private static NotificationLayoutMetrics CalculateCompact(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage,
            bool showProgressBar, int iconSize, int padding, int spacing,
            Control owner, Font titleFont, Font messageFont)
        {
            var m = new NotificationLayoutMetrics();
            int progH   = showProgressBar ? S(4, owner) : 0;
            int closeW  = S(20 + spacing, owner);
            int lineH   = Math.Max(OneLineHeight(titleFont, owner), OneLineHeight(messageFont, owner));
            int innerX  = bounds.X + padding;
            int innerW  = bounds.Width - padding * 2 - closeW;
            int centreY = bounds.Y + (bounds.Height - progH - lineH) / 2;

            if (hasIcon)
            {
                int iy = bounds.Y + (bounds.Height - progH - iconSize) / 2;
                m.IconRect = new Rectangle(innerX, iy, iconSize, iconSize);
                innerX += iconSize + spacing;
                innerW -= iconSize + spacing;
            }

            if (hasTitle && hasMessage)
            {
                // Split: title takes 40 %, message takes 60 %
                int titleW = (int)(innerW * 0.40);
                m.TitleRect   = new Rectangle(innerX, centreY, titleW, lineH);
                m.MessageRect = new Rectangle(innerX + titleW + spacing, centreY, innerW - titleW - spacing, lineH);
            }
            else if (hasTitle)
            {
                m.TitleRect = new Rectangle(innerX, centreY, innerW, lineH);
            }
            else if (hasMessage)
            {
                m.MessageRect = new Rectangle(innerX, centreY, innerW, lineH);
            }

            return m;
        }

        // ── Prominent layout ──────────────────────────────────────────────────
        // [     ICON     ]   (centred, 32 dp)
        // [    TITLE     ]   (centred, large)
        // [   MESSAGE    ]
        // [   ACTIONS    ]

        private static NotificationLayoutMetrics CalculateProminent(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage, bool hasActions,
            bool showProgressBar, int iconSize, int padding, int spacing,
            Control owner, Font titleFont, Font messageFont)
        {
            var m     = new NotificationLayoutMetrics();
            int progH = showProgressBar ? S(4, owner) : 0;
            int largeIcon = Math.Max(iconSize, S(32, owner));
            int innerW    = bounds.Width - padding * 2;
            int curY      = bounds.Y + padding;

            if (hasIcon)
            {
                m.IconRect = new Rectangle(bounds.X + (bounds.Width - largeIcon) / 2, curY, largeIcon, largeIcon);
                curY += largeIcon + spacing;
            }

            int titleH = hasTitle ? OneLineHeight(titleFont, owner) + S(2, owner) : 0;
            if (hasTitle)
            {
                m.TitleRect = new Rectangle(bounds.X + padding, curY, innerW, titleH);
                curY += titleH + S(4, owner);
            }

            int actionH = hasActions ? S(32, owner) : 0;
            int msgBot  = bounds.Bottom - padding - progH - (hasActions ? actionH + spacing : 0);
            if (hasMessage && msgBot > curY)
                m.MessageRect = new Rectangle(bounds.X + padding, curY, innerW, msgBot - curY);

            if (hasActions)
                m.ActionsRect = new Rectangle(bounds.X + padding, bounds.Bottom - padding - progH - actionH, innerW, actionH);

            return m;
        }

        // ── Banner layout ─────────────────────────────────────────────────────
        // [ICON | TITLE  MSG  | ACTIONS ] — single horizontal row

        private static NotificationLayoutMetrics CalculateBanner(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage,
            bool showProgressBar, int iconSize, int padding, int spacing,
            Control owner, Font titleFont, Font messageFont)
        {
            var m       = new NotificationLayoutMetrics();
            int progH   = showProgressBar ? S(4, owner) : 0;
            int closeW  = S(20 + spacing, owner);
            int innerH  = bounds.Height - padding * 2 - progH;
            int innerX  = bounds.X + padding;
            int innerW  = bounds.Width - padding * 2 - closeW;
            int centreY = bounds.Y + padding;

            if (hasIcon)
            {
                int iy = bounds.Y + (bounds.Height - progH - iconSize) / 2;
                m.IconRect = new Rectangle(innerX, iy, iconSize, iconSize);
                innerX += iconSize + spacing;
                innerW -= iconSize + spacing;
            }

            // Reserve right section for actions
            int actionW = 0;
            if (hasTitle && hasMessage)   // split 35 / 65
            {
                int titleW = (int)(innerW * 0.35);
                m.TitleRect   = new Rectangle(innerX, centreY, titleW, innerH);
                m.MessageRect = new Rectangle(innerX + titleW + spacing, centreY, innerW - titleW - spacing, innerH);
            }
            else if (hasTitle)
            {
                m.TitleRect = new Rectangle(innerX, centreY, innerW, innerH);
            }
            else if (hasMessage)
            {
                m.MessageRect = new Rectangle(innerX, centreY, innerW, innerH);
            }

            return m;
        }

        // ── Toast layout ──────────────────────────────────────────────────────
        // [ICON | TITLE ] [X]   — absolute minimal, single line

        private static NotificationLayoutMetrics CalculateToast(
            Rectangle bounds, bool hasIcon, bool hasTitle,
            int iconSize, int padding, int spacing,
            Control owner, Font titleFont)
        {
            var m      = new NotificationLayoutMetrics();
            int closeW = S(20 + spacing, owner);
            int lineH  = OneLineHeight(titleFont, owner);
            int innerX = bounds.X + padding;
            int innerW = bounds.Width - padding * 2 - closeW;
            int centreY = bounds.Y + (bounds.Height - lineH) / 2;

            if (hasIcon)
            {
                int iy = bounds.Y + (bounds.Height - iconSize) / 2;
                m.IconRect = new Rectangle(innerX, iy, iconSize, iconSize);
                innerX += iconSize + spacing;
                innerW -= iconSize + spacing;
            }

            if (hasTitle)
                m.TitleRect = new Rectangle(innerX, centreY, innerW, lineH);

            return m;
        }

        // ── Elevated layout ───────────────────────────────────────────────────
        // Like Standard but icon is a larger (40 dp) circle badge

        private static NotificationLayoutMetrics CalculateElevated(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage, bool hasActions,
            bool showProgressBar, int iconSize, int padding, int spacing,
            Control owner, Font titleFont, Font messageFont)
        {
            int largeIcon = S(40, owner);
            return CalculateStandard(bounds, hasIcon, hasTitle, hasMessage, hasActions,
                showProgressBar, largeIcon, padding, spacing, owner, titleFont, messageFont);
        }

        // ── Snackbar layout ───────────────────────────────────────────────────
        // [  MESSAGE  ] [ACTION] — single line, no title, no icon

        private static NotificationLayoutMetrics CalculateSnackbar(
            Rectangle bounds, bool hasTitle, bool hasActions, int padding, int spacing,
            Control owner, Font messageFont)
        {
            var m       = new NotificationLayoutMetrics();
            int lineH   = OneLineHeight(messageFont, owner);
            int centreY = bounds.Y + (bounds.Height - lineH) / 2;
            int innerW  = bounds.Width - padding * 2;
            int actionW = hasActions ? S(80, owner) : 0;

            m.MessageRect = new Rectangle(bounds.X + padding, centreY, innerW - actionW - (hasActions ? spacing : 0), lineH);
            if (hasActions)
                m.ActionsRect = new Rectangle(bounds.Right - padding - actionW, centreY, actionW, lineH);

            return m;
        }

        // ── StatusBar layout ──────────────────────────────────────────────────
        // [ ● TYPE | MESSAGE                  ] — full-width, single line

        private static NotificationLayoutMetrics CalculateStatusBar(
            Rectangle bounds, bool hasTitle, int padding, Control owner, Font messageFont)
        {
            var m       = new NotificationLayoutMetrics();
            int dotSz   = S(8, owner);
            int spacing = S(6, owner);
            int lineH   = OneLineHeight(messageFont, owner);
            int centreY = bounds.Y + (bounds.Height - lineH) / 2;
            int x       = bounds.X + padding;

            // Small type dot
            m.IconRect = new Rectangle(x, bounds.Y + (bounds.Height - dotSz) / 2, dotSz, dotSz);
            x += dotSz + spacing;
            m.MessageRect = new Rectangle(x, centreY, bounds.Right - padding - x, lineH);
            return m;
        }

        // ── Chip layout ───────────────────────────────────────────────────────
        // [● TITLE  ×] — pill-shaped, very compact

        private static NotificationLayoutMetrics CalculateChip(
            Rectangle bounds, bool hasIcon, bool hasTitle, int padding, int spacing,
            Control owner, Font titleFont)
        {
            var m       = new NotificationLayoutMetrics();
            int lineH   = OneLineHeight(titleFont, owner);
            int closeSz = S(16, owner);
            int innerX  = bounds.X + padding;
            int centreY = bounds.Y + (bounds.Height - lineH) / 2;
            int closeX  = bounds.Right - padding - closeSz;

            if (hasIcon)
            {
                int iconSz = S(14, owner);
                m.IconRect = new Rectangle(innerX, bounds.Y + (bounds.Height - iconSz) / 2, iconSz, iconSz);
                innerX += iconSz + S(4, owner);
            }

            m.TitleRect      = new Rectangle(innerX, centreY, closeX - innerX - spacing, lineH);
            m.CloseButtonRect = new Rectangle(closeX, bounds.Y + (bounds.Height - closeSz) / 2, closeSz, closeSz);
            return m;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Result type
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Pre-calculated rectangles for all notification UI elements.</summary>
    public class NotificationLayoutMetrics
    {
        public Rectangle IconRect        { get; set; } = Rectangle.Empty;
        public Rectangle TitleRect       { get; set; } = Rectangle.Empty;
        public Rectangle MessageRect     { get; set; } = Rectangle.Empty;
        public Rectangle ActionsRect     { get; set; } = Rectangle.Empty;
        public Rectangle CloseButtonRect { get; set; } = Rectangle.Empty;
        public Rectangle ProgressBarRect { get; set; } = Rectangle.Empty;
    }
}
