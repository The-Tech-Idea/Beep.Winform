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

            // Colour is assigned for one role only: the destructive action.
            //
            // Everything else is left entirely alone, because BeepButton colours itself from the
            // theme and an assignment here is simply overwritten by its own ApplyTheme. A destructive
            // action is the exception worth making - it is the one button whose colour carries a
            // warning rather than a style, and the theme's own alert colour is what it takes.
            if (role == DialogButtonRole.Destructive)
            {
                Color alert = theme?.DialogErrorButtonBackColor is { IsEmpty: false } a
                    ? a
                    : DialogStyleAdapter.GetSeverityAccent(DialogSeverity.Error, theme);

                button.BackColor = alert;
                button.ForeColor = theme?.DialogErrorButtonForeColor is { IsEmpty: false } f
                    ? f
                    : ColorUtils.GetContrastColor(alert);
            }

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
