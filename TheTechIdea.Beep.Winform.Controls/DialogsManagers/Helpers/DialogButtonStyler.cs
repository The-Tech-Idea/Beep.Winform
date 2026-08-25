using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// Applies a dialog action's role, colour and shape to the button that renders it.
    /// </summary>
    /// <remarks>
    /// The rule the references encode: <b>role decides the treatment, severity decides the colour</b>
    /// — with one exception that matters. A <see cref="DialogButtonRole.Destructive"/> action is
    /// filled in the error colour whatever the dialog's severity is. `dialog3.png` is an error dialog
    /// <i>and</i> carries a destructive button, and they are the same red on purpose: the colour is
    /// about the consequence of pressing, not the mood of the dialog.
    /// </remarks>
    public static class DialogButtonStyler
    {
        /// <summary>Minimum hit target at 100% scaling, before DPI.</summary>
        /// <remarks>
        /// The reference buttons are generous and a 24px control is not comfortably clickable. 32×64
        /// is the floor every platform guideline lands at or above; it scales with DPI below.
        /// </remarks>
        private const int MinLogicalHeight = 32;
        private const int MinLogicalWidth = 64;

        public static void Apply(BeepButton button, DialogButton spec, DialogSeverity severity,
                                 IBeepTheme? theme, DialogButtonShape shape)
        {
            if (button == null || spec == null) return;

            var role = spec.ResolvedRole;

            // Role decides the treatment; severity decides the colour. The button
            // resolves the severity colour from the theme itself, in its own
            // ApplyTheme, through the Severity property — so the colour follows a
            // theme switch and this folder never assigns a colour directly. A
            // destructive action is always error-coloured; a primary takes the
            // dialog's own severity; secondary and cancel keep the neutral chrome.
            button.Severity = role switch
            {
                DialogButtonRole.Destructive => DialogSeverity.Error,
                DialogButtonRole.Primary      => severity,
                _                             => null
            };

            ApplyShape(button, shape);
            ApplyIcon(button, spec);
            EnforceHitTarget(button);

            button.Enabled = spec.Enabled && !spec.IsPending;
            button.Visible = spec.Visible;

            if (!string.IsNullOrWhiteSpace(spec.Text))
            {
                button.Text = spec.Text;
                button.AccessibleName = spec.Text;
            }
        }

        /// <summary>Pill radius is half the height, which is what makes the ends semicircular.</summary>
        private static void ApplyShape(BeepButton button, DialogButtonShape shape)
        {
            button.IsRounded = true;
            button.BorderRadius = shape == DialogButtonShape.Pill
                ? Math.Max(1, button.Height / 2)
                : button.BorderRadius;
        }

        private static void ApplyIcon(BeepButton button, DialogButton spec)
        {
            if (string.IsNullOrWhiteSpace(spec.Icon)) return;

            button.ImagePath = spec.Icon;
            button.TextImageRelation = spec.IconPlacement == DialogButtonIconPlacement.Trailing
                ? TextImageRelation.TextBeforeImage
                : TextImageRelation.ImageBeforeText;
        }

        /// <summary>
        /// Grows the button to the minimum comfortable hit target, scaled for the display.
        /// </summary>
        /// <remarks>
        /// Applied to <c>MinimumSize</c> rather than <c>Size</c>: the shell already sizes footer
        /// buttons from their <c>MinimumSize</c>, so this raises the floor without fighting the
        /// layout for the actual bounds.
        /// </remarks>
        private static void EnforceHitTarget(BeepButton button)
        {
            float scale = button.DeviceDpi / 96f;

            int minW = (int)Math.Round(MinLogicalWidth * scale);
            int minH = (int)Math.Round(MinLogicalHeight * scale);

            var floor = new Size(
                Math.Max(button.MinimumSize.Width, minW),
                Math.Max(button.MinimumSize.Height, minH));

            if (button.MinimumSize != floor) button.MinimumSize = floor;
        }
    }
}
