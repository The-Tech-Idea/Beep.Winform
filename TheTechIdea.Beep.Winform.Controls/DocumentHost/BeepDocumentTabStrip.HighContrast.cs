// BeepDocumentTabStrip.HighContrast.cs
// High-contrast and system-colour accessibility support for BeepDocumentTabStrip.
// When Windows High Contrast mode is active the control automatically maps all
// Beep-theme colours to their closest SystemColors equivalents so that every tab
// (background, foreground, focus ring, borders, indicators) satisfies WCAG 2.2 AA.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────
        // High-contrast override colours
        // ─────────────────────────────────────────────────────────────────────

        // These are returned by the EffectiveXxx helpers used by the painting
        // layer when SystemInformation.HighContrast is true.  They are reset to
        // the theme values whenever the mode is disabled.

        private Color _hcBack        = SystemColors.Control;
        private Color _hcFore        = SystemColors.ControlText;
        private Color _hcBorder      = SystemColors.ControlDark;
        private Color _hcAccent      = SystemColors.Highlight;
        private Color _hcHighlight   = SystemColors.HighlightText;
        private Color _hcActiveBack  = SystemColors.Window;
        private Color _hcInactiveText = SystemColors.GrayText;

        // ─────────────────────────────────────────────────────────────────────
        // Effective-colour helpers — used by the painting code
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Tab strip and inactive tab background colour.</summary>
        internal Color EffectivePanelBack   => SystemInformation.HighContrast
            ? _hcBack
            : (_theme?.PanelBackColor ?? BackColor);

        /// <summary>Active tab background colour.</summary>
        internal Color EffectiveActiveBack  => SystemInformation.HighContrast
            ? _hcActiveBack
            : (_theme?.BackgroundColor ?? BackColor);

        /// <summary>Primary foreground / text colour.</summary>
        internal Color EffectiveFore        => SystemInformation.HighContrast
            ? _hcFore
            : (_theme?.ForeColor ?? ForeColor);

        /// <summary>Inactive / secondary text colour.</summary>
        internal Color EffectiveInactiveText => SystemInformation.HighContrast
            ? _hcInactiveText
            : (_theme?.SecondaryTextColor ?? SystemColors.GrayText);

        /// <summary>Border or separator colour.</summary>
        internal Color EffectiveBorder      => SystemInformation.HighContrast
            ? _hcBorder
            : (_theme?.BorderColor ?? SystemColors.ControlDark);

        /// <summary>Active-indicator and accent colour.</summary>
        internal Color EffectiveAccent      => SystemInformation.HighContrast
            ? _hcAccent
            : (_theme?.PrimaryColor ?? SystemColors.Highlight);

        // ─────────────────────────────────────────────────────────────────────
        // System-colour change hook (HC on/off toggle at runtime)
        // OnHandleCreated hook lives in BeepDocumentTabStrip.cs (calls ApplyHighContrastTheme at end)
        // ─────────────────────────────────────────────────────────────────

        protected override void OnSystemColorsChanged(System.EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            ApplyHighContrastTheme();
            Invalidate();
        }

        // ─────────────────────────────────────────────────────────────────────
        // ApplyHighContrastTheme
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Adjusts the control's <see cref="Control.BackColor"/> and
        /// <see cref="Control.ForeColor"/> to Windows system colours when high-contrast
        /// mode is active, restoring the Beep-theme values when it is not.
        /// The <c>EffectiveXxx</c> helpers use the same detection logic at paint-time
        /// so that every colour token resolves correctly without patching each DrawTab
        /// method individually.
        /// </summary>
        public void ApplyHighContrastTheme()
        {
            if (SystemInformation.HighContrast)
            {
                // Refresh the HC cache from the current system palette
                _hcBack         = SystemColors.Control;
                _hcFore         = SystemColors.ControlText;
                _hcBorder       = SystemColors.ControlDark;
                _hcAccent       = SystemColors.Highlight;
                _hcHighlight    = SystemColors.HighlightText;
                _hcActiveBack   = SystemColors.Window;
                _hcInactiveText = SystemColors.GrayText;

                // Override WinForms built-in properties so framework-drawn
                // elements (e.g. ControlPaint focus rectangles) also use HC
                BackColor = SystemColors.Control;
                ForeColor = SystemColors.ControlText;
            }
            else
            {
                // Restore from Beep theme
                if (_theme != null)
                {
                    BackColor = _theme.PanelBackColor;
                    ForeColor = _theme.ForeColor;
                }
            }
        }
    }
}
