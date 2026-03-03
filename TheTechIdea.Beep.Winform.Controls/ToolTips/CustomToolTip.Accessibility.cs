using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 11 — Accessibility partial for <see cref="CustomToolTip"/>.
    /// 
    /// Provides:
    ///  1. Keyboard-triggered dismiss (Escape, or configurable via <see cref="ToolTipConfig.KeyboardTriggerable"/>).
    ///  2. WCAG contrast enforcement — if the theme colours fail the minimum ratio the
    ///     <see cref="ToolTipAccessibilityHelpers"/> helper is called to substitute
    ///     accessible colours before painting.
    ///  3. Accessible object name/description forwarded to the Windows UIA / MSAA layer
    ///     so screen readers can announce the tooltip.
    /// </summary>
    public partial class CustomToolTip
    {
        // ──────────────────────────────────────────────────────────────
        // Accessible name / description
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets the MSAA/UIA accessible name and help-text based on the current config.
        /// Called automatically by <see cref="ApplyConfig"/>.
        /// </summary>
        internal void SyncAccessibility()
        {
            if (_config == null) return;

            // Build a useful accessible name: "Title — Message" or whichever is present.
            string name = string.IsNullOrEmpty(_config.Title)
                ? _config.Text
                : _config.Text is { Length: > 0 }
                    ? $"{_config.Title} — {_config.Text}"
                    : _config.Title;

            AccessibleName        = name ?? string.Empty;
            AccessibleDescription = _config.Text ?? string.Empty;
            AccessibleRole        = AccessibleRole.ToolTip;
        }

        // ──────────────────────────────────────────────────────────────
        // Keyboard handling
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Intercepts Escape to dismiss the tooltip when keyboard interaction is allowed.
        /// Derived types (e.g. <see cref="BeepPopover"/>) call base so this fires first.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                bool canDismissViaKeyboard = _config?.KeyboardTriggerable ?? true;
                if (canDismissViaKeyboard)
                {
                    Hide();
                    return true; // consumed
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ──────────────────────────────────────────────────────────────
        // High-contrast / WCAG colour enforcement
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Checks whether the current fore/back colours meet the configured
        /// <see cref="ToolTipConfig.MinContrastRatio"/> (defaults to 4.5 for AA).
        /// If they do not, replaces them with fully accessible substitutes.
        /// Should be called after theme colours are resolved.
        /// </summary>
        internal void EnforceContrastIfNeeded(ref Color backColor, ref Color foreColor)
        {
            double minRatio = (_config?.MinContrastRatio > 0) ? _config.MinContrastRatio : 4.5;

            if (!ToolTipAccessibilityHelpers.EnsureContrastRatio(foreColor, backColor, minRatio))
            {
                // AdjustForContrast returns the adjusted foreground colour
                foreColor = ToolTipAccessibilityHelpers.AdjustForContrast(foreColor, backColor, minRatio);
            }
        }

        /// <summary>
        /// Returns <c>true</c> when Windows high-contrast mode is active.
        /// Painting code can use this to opt out of gradients / translucency.
        /// </summary>
        public static bool IsHighContrastActive =>
            SystemInformation.HighContrast;

        // ──────────────────────────────────────────────────────────────
        // Focus support — allow tooltips to receive keyboard focus when triggered
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Makes the tooltip focusable when <see cref="ToolTipConfig.KeyboardTriggerable"/> is true,
        /// so keyboard users can Tab into it (e.g. for popover action buttons).
        /// Called from ShowAsync after the form is displayed.
        /// </summary>
        internal void ApplyFocusPolicy()
        {
            if (_config?.KeyboardTriggerable == true)
            {
                // Allow the form to appear in the tab order and accept keyboard input.
                this.Activate();
            }
        }
    }
}
