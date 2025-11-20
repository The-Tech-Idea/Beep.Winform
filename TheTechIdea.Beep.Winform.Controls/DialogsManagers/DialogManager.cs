using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Static dialog manager - provides static access to dialog functionality
    /// Use BeepDialogManager for instance-based dialog management
    /// </summary>
    public static partial class DialogManager
    {
        private static BeepPopupForm? _hostForm;
        // Darkened overlay panel used when showing hosted dialogs
        private static Panel? _hostOverlay;

        /// <summary>
        /// Gets the host form for dialogs
        /// </summary>
        public static BeepPopupForm? HostForm
        {
            get => _hostForm;
            private set => _hostForm = value;
        }

        /// <summary>
        /// Sets the host form for dialogs
        /// </summary>
        public static void SetHostForm(BeepPopupForm popupForm)
        {
            HostForm = popupForm;
        }

        private static void EnsureOverlay()
        {
            if (HostForm == null) return;
            if (_hostOverlay != null) return;
            _hostOverlay = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(120, 0, 0, 0),
                Dock = System.Windows.Forms.DockStyle.Fill,
                Visible = false
            };
            HostForm.Controls.Add(_hostOverlay);
            _hostOverlay.BringToFront();
            _hostOverlay.Click += (s, e) => { /* optionally close or consume clicks */ };
        }

        #region Helper Methods (for Presets and Animations partials)

        /// <summary>
        /// Maps BeepDialogIcon to DialogType enum
        /// </summary>
        private static DialogType MapIconToDialogType(BeepDialogIcon icon)
        {
            return icon switch
            {
                BeepDialogIcon.Information => DialogType.Information,
                BeepDialogIcon.Warning => DialogType.Warning,
                BeepDialogIcon.Error => DialogType.Error,
                BeepDialogIcon.Question => DialogType.Question,
                BeepDialogIcon.Success => DialogType.Information,
                _ => DialogType.None
            };
        }

        /// <summary>
        /// Converts System.Windows.Forms.DialogResult to BeepDialogResult
        /// </summary>
        private static BeepDialogResult ConvertDialogResult(System.Windows.Forms.DialogResult result)
        {
            return result switch
            {
                System.Windows.Forms.DialogResult.OK => BeepDialogResult.OK,
                System.Windows.Forms.DialogResult.Cancel => BeepDialogResult.Cancel,
                System.Windows.Forms.DialogResult.Yes => BeepDialogResult.Yes,
                System.Windows.Forms.DialogResult.No => BeepDialogResult.No,
                System.Windows.Forms.DialogResult.Abort => BeepDialogResult.Abort,
                System.Windows.Forms.DialogResult.Retry => BeepDialogResult.Retry,
                System.Windows.Forms.DialogResult.Ignore => BeepDialogResult.Ignore,
                _ => BeepDialogResult.Cancel
            };
        }

        #endregion

        private static TheTechIdea.Beep.Vis.Modules.DialogShowAnimation ConvertAnimation(DialogsManagers.Models.DialogShowAnimation animation)
        {
            // Attempt a direct mapping based on enum name
            return animation switch
            {
                DialogsManagers.Models.DialogShowAnimation.FadeIn => TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.FadeIn,
                DialogsManagers.Models.DialogShowAnimation.SlideInFromTop => TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.SlideInFromTop,
                DialogsManagers.Models.DialogShowAnimation.SlideInFromBottom => TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.SlideInFromBottom,
                DialogsManagers.Models.DialogShowAnimation.SlideInFromLeft => TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.SlideInFromLeft,
                DialogsManagers.Models.DialogShowAnimation.SlideInFromRight => TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.SlideInFromRight,
                DialogsManagers.Models.DialogShowAnimation.ZoomIn => TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.ZoomIn,
                _ => TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.FadeIn,
            };
        }

        /// <summary>
        /// Public helper to apply a dialog show animation. Uses the internal animator.
        /// </summary>
        public static void ApplyDialogAnimation(Form form, TheTechIdea.Beep.Vis.Modules.DialogShowAnimation animation)
        {
            // Use the internal animator - wrap to accept Form which is compatible with BeepPopupForm
            if (form is BeepPopupForm bp)
                DialogAnimator.ApplyAnimation(bp, animation);
            else
            {
                // Try to apply using BeepPopupForm fallback (create temporary wrapper?) - just apply no-op or use fade/opacity on generic forms
                if (animation == TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.FadeIn)
                {
                    form.Opacity = 0;
                    var timer = new System.Windows.Forms.Timer { Interval = 20 };
                    int steps = 10, step = 0;
                    timer.Tick += (s, e) =>
                    {
                        step++;
                        form.Opacity = (double)step / steps;
                        if (step >= steps) { timer.Stop(); timer.Dispose(); form.Opacity = 1; }
                    };
                    timer.Start();
                }
            }
        }

        /// <summary>
        /// Shows a dialog hosted on the configured HostForm if available, with overlay and optional animation.
        /// Falls back to normal modal ShowDialog otherwise.
        /// </summary>
        public static DialogReturn ShowPresetDialogHosted(DialogConfig config, DialogsManagers.Models.DialogShowAnimation animation = DialogsManagers.Models.DialogShowAnimation.FadeIn)
        {
            EnsureOverlay();
            if (HostForm == null)
                return ShowPresetDialog(config);

            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = config.Title;
                dialog.Message = config.Message;
                dialog.DialogType = MapIconToDialogType(config.IconType);
                // Map buttons
                if (config.Buttons != null && config.Buttons.Length > 0)
                {
                    var buttonList = config.Buttons;
                    if (buttonList.Length == 2)
                    {
                        if (buttonList[0] == BeepDialogButtons.Cancel && buttonList[1] == BeepDialogButtons.Ok)
                            dialog.DialogButtons = BeepDialogButtons.OkCancel;
                        else if (buttonList[0] == BeepDialogButtons.Yes && buttonList[1] == BeepDialogButtons.No)
                            dialog.DialogButtons = BeepDialogButtons.YesNo;
                    }
                }

                // Setup overlay + animation
                if (_hostOverlay != null)
                {
                    _hostOverlay.Visible = true;
                    _hostOverlay.BringToFront();
                }

                // Host the dialog as a child of HostForm
                dialog.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                dialog.ShowInTaskbar = false;

                // Apply animation (convert to Vis module enum for animator)
                ApplyDialogAnimation(dialog, ConvertAnimation(animation));

                var result = dialog.ShowDialog(HostForm);

                if (_hostOverlay != null)
                    _hostOverlay.Visible = false;

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Cancel = result == System.Windows.Forms.DialogResult.Cancel
                };
            }
        }
    }
}
