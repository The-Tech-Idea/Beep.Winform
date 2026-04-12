// DocumentStyleResolver.cs
// Centralised style/colour resolution for the DocumentHost subsystem.
// Keeps painting code free from direct theme-field access and provides
// safe fallbacks for null themes or high-contrast system overrides.
// ─────────────────────────────────────────────────────────────────────────────

using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Helpers
{
    /// <summary>
    /// Resolves themed colours and DPI-aware sizes for DocumentHost controls.
    /// All methods are null-safe and return sensible system-colour fallbacks.
    /// </summary>
    public static class DocumentStyleResolver
    {
        // ─────────────────────────────────────────────────────────────────────
        // Colour resolution
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Background colour for the tab strip area.</summary>
        public static Color StripBackground(IBeepTheme? theme)
            => theme?.PanelBackColor ?? SystemColors.Control;

        /// <summary>Background colour for the document content area.</summary>
        public static Color ContentBackground(IBeepTheme? theme)
            => theme?.BackgroundColor ?? SystemColors.Window;

        /// <summary>Primary foreground colour for tab titles.</summary>
        public static Color TabForeground(IBeepTheme? theme)
            => theme?.ForeColor ?? SystemColors.WindowText;

        /// <summary>Foreground colour for inactive/secondary tab text.</summary>
        public static Color TabForegroundInactive(IBeepTheme? theme)
        {
            Color fg = TabForeground(theme);
            return Color.FromArgb(178, fg);   // ≈70 % opacity
        }

        /// <summary>Background fill for the active tab.</summary>
        public static Color ActiveTabBackground(IBeepTheme? theme)
            => theme?.BackgroundColor ?? SystemColors.Window;

        /// <summary>Background fill for an inactive tab (transparent by convention).</summary>
        public static Color InactiveTabBackground(IBeepTheme? theme)
            => Color.Transparent;

        /// <summary>Accent / primary colour used for the active indicator bar, badge fill, etc.</summary>
        public static Color Accent(IBeepTheme? theme)
            => theme?.PrimaryColor ?? SystemColors.Highlight;

        /// <summary>Border / separator line colour.</summary>
        public static Color Border(IBeepTheme? theme)
            => theme?.BorderColor ?? SystemColors.ControlDark;

        /// <summary>Hover overlay tint (semi-transparent).</summary>
        public static Color HoverOverlay(IBeepTheme? theme)
        {
            Color acc = Accent(theme);
            return Color.FromArgb(26, acc);   // ≈10 % opacity
        }

        /// <summary>Close-button foreground colour.</summary>
        public static Color CloseButtonFore(IBeepTheme? theme)
            => theme?.ForeColor ?? SystemColors.WindowText;

        /// <summary>Close-button background on hover.</summary>
        public static Color CloseButtonHoverBackground(IBeepTheme? theme)
            => Color.FromArgb(60, Color.Red);

        /// <summary>Colour used for the badge pill background.</summary>
        public static Color BadgeBackground(IBeepTheme? theme)
            => theme?.PrimaryColor ?? Color.OrangeRed;

        /// <summary>Foreground text colour on a badge pill.</summary>
        public static Color BadgeForeground(IBeepTheme? theme)
            => Color.White;

        /// <summary>Tooltip popup background colour.</summary>
        public static Color TooltipBackground(IBeepTheme? theme)
            => theme?.PanelBackColor ?? Color.FromArgb(40, 44, 52);

        /// <summary>Tooltip primary text colour.</summary>
        public static Color TooltipForeground(IBeepTheme? theme)
            => theme?.ForeColor ?? Color.White;

        /// <summary>Tooltip secondary / body text colour.</summary>
        public static Color TooltipSecondary(IBeepTheme? theme)
            => theme?.SecondaryTextColor ?? Color.FromArgb(180, 185, 200);

        // ─────────────────────────────────────────────────────────────────────
        // DPI-aware sizing (no DpiScalingHelper dependency)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Scales a logical pixel value to physical pixels using the device DPI
        /// reported by <paramref name="control"/> (or 96 when the handle is not yet created).
        /// </summary>
        public static int Scale(int logical, Control control)
        {
            int dpi = control.IsHandleCreated ? control.DeviceDpi : 96;
            if (dpi <= 0) dpi = 96;
            return (int)(logical * (dpi / 96f));
        }

        /// <summary>
        /// Scales using a raw DPI value — useful in <c>Form</c>-derived popups that
        /// read <c>DeviceDpi</c> directly.
        /// </summary>
        public static int Scale(int logical, int deviceDpi)
        {
            int dpi = deviceDpi > 0 ? deviceDpi : 96;
            return (int)(logical * (dpi / 96f));
        }
    }
}
