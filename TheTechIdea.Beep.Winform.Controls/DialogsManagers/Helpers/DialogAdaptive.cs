using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// Switches a dialog's presentation when the window hosting it is narrow.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A 600px dialog with a horizontal button row is right on a desktop and wrong in a 480px app
    /// window or a remote session, where the actions crowd or clip. Every current framework switches
    /// presentation below a breakpoint — web dialogs become bottom sheets, Material specifies
    /// full-screen dialogs at compact widths — and the desktop equivalent is to use the width that is
    /// actually available and stack the actions.
    /// </para>
    /// <para>
    /// Above the breakpoint this does nothing at all. Adaptive layout that alters the common case is a
    /// regression wearing a feature's clothes, so the desktop path has to be untouched rather than
    /// merely similar.
    /// </para>
    /// </remarks>
    internal static class DialogAdaptive
    {
        /// <summary>
        /// Owner width below which the dialog changes shape, before DPI scaling.
        /// </summary>
        /// <remarks>
        /// 560 sits above the 300–600 the config already allows, so a dialog at its default
        /// <see cref="DialogConfig.MaxWidth"/> of 600 cannot fit an owner narrower than this with
        /// margins to spare — which is the condition worth reacting to.
        /// </remarks>
        private const int NarrowBreakpoint = 560;

        /// <summary>Breathing room either side of a dialog filling a narrow owner.</summary>
        private const int SideMargin = 16;

        public static void Apply(DialogConfig config, Form? owner)
        {
            if (config == null) return;

            int available = owner is { IsDisposed: false }
                ? owner.ClientSize.Width
                : Screen.PrimaryScreen?.WorkingArea.Width ?? 0;

            if (available <= 0) return;

            float scale = owner is { IsDisposed: false } ? owner.DeviceDpi / 96f : 1f;
            int breakpoint = (int)Math.Round(NarrowBreakpoint * scale);
            if (available >= breakpoint) return;               // desktop path, untouched

            int margin = (int)Math.Round(SideMargin * scale);
            int usable = Math.Max(config.MinWidth, available - margin * 2);

            // The dialog takes the width that exists rather than the width it would prefer.
            config.MaxWidth = Math.Min(config.MaxWidth, usable);

            // Stacked actions, because a row of them does not fit a narrow dialog. This is the wire
            // stage 05 connected: DialogButtonLayout.Vertical was implemented and never given the
            // config's value, and this stage is mostly the first real use of it.
            config.ButtonLayout = DialogButtonLayout.Vertical;

            // Icon-left needs room beside the icon for a column of text. Centred reads better once
            // that room is gone, and it is the presentation the references use at small sizes.
            if (config.Presentation == DialogPresentation.TitleBar)
            {
                config.Presentation = DialogPresentation.Centred;
            }
        }
    }
}
