using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.CommandPalette;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms;
using DialogShowAnimation = TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models.DialogShowAnimation;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Unified Dialog Manager for all dialog operations in Beep.Winform
    /// Provides both static singleton access and instance-based usage
    /// </summary>
    public partial class BeepDialogManager : IDialogManager
    {
        #region Singleton Pattern

        private static BeepDialogManager? _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the singleton instance of BeepDialogManager
        /// </summary>
        public static BeepDialogManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new BeepDialogManager();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Fields

        private Form? _hostForm;
        private IBeepTheme? _defaultTheme;
        private BeepControlStyle _defaultStyle = BeepControlStyle.Material3;
        private DialogShowAnimation _defaultAnimation = DialogShowAnimation.None;
        private int _animationDuration = 200;
        private DialogManagerOptions _options = new DialogManagerOptions();
        private readonly Dictionary<string, Rectangle> _dialogRectState = new();
        private readonly Dictionary<string, Queue<string>> _recentInputMemory = new();
        private Form? _activeModalDialog;
        private DialogConfig? _activeDialogConfig;

        public event EventHandler<DialogConfig>? DialogOpened;
        public event EventHandler<DialogReturn>? DialogConfirmed;
        public event EventHandler<DialogReturn>? DialogCancelled;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new BeepDialogManager instance
        /// </summary>
        public BeepDialogManager()
        {
        }

        /// <summary>
        /// Creates a new BeepDialogManager with a host form
        /// </summary>
        /// <param name="hostForm">The form to host dialogs on</param>
        public BeepDialogManager(Form hostForm)
        {
            _hostForm = hostForm;
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Sets the host form for dialogs. Dialogs will be centered on this form.
        /// </summary>
        public BeepDialogManager SetHostForm(Form? form)
        {
            _hostForm = form;
            return this;
        }

        /// <summary>
        /// Sets the default theme for dialogs
        /// </summary>
        public BeepDialogManager SetDefaultTheme(IBeepTheme theme)
        {
            _defaultTheme = theme;
            return this;
        }

        /// <summary>
        /// Sets the default control style for dialogs
        /// </summary>
        public BeepDialogManager SetDefaultStyle(BeepControlStyle style)
        {
            _defaultStyle = style;
            _options.DefaultStyle = style;
            return this;
        }

        /// <summary>
        /// Sets the default animation for dialogs
        /// </summary>
        public BeepDialogManager SetDefaultAnimation(DialogShowAnimation animation, int durationMs = 200)
        {
            _defaultAnimation = animation;
            _animationDuration = durationMs;
            _options.DefaultAnimation = animation;
            return this;
        }

        public BeepDialogManager Configure(Action<DialogManagerOptions> configure)
        {
            configure?.Invoke(_options);
            _defaultStyle = _options.DefaultStyle;
            _defaultAnimation = _options.DefaultAnimation;
            return this;
        }

        /// <summary>
        /// Gets or sets the host form
        /// </summary>
        public Form? HostForm
        {
            get => _hostForm;
            set => _hostForm = value;
        }

        /// <summary>
        /// Gets or sets the default theme
        /// </summary>
        public IBeepTheme? DefaultTheme
        {
            get => _defaultTheme;
            set => _defaultTheme = value;
        }

        /// <summary>
        /// Gets or sets the default control style
        /// </summary>
        public BeepControlStyle DefaultStyle
        {
            get => _defaultStyle;
            set => _defaultStyle = value;
        }

        /// <summary>
        /// Gets or sets the default animation
        /// </summary>
        public DialogShowAnimation DefaultAnimation
        {
            get => _defaultAnimation;
            set => _defaultAnimation = value;
        }

        public DialogManagerOptions Options => _options;

        #endregion

        #region Advanced Dialog Types

        /// <summary>
        /// Show a wizard using the Beep WizardManager.
        /// </summary>
        public DialogResult ShowWizard(WizardConfig config)
        {
            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            return owner != null
                ? WizardManager.ShowWizard(config, owner)
                : WizardManager.ShowWizard(config);
        }

        /// <summary>
        /// Show a modeless (non-blocking) Beep-themed content window.
        /// </summary>
        public void ShowModeless(Control content, DialogConfig config)
        {
            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var dialog = new BeepModelessDialog { Text = config.Title };
            dialog.ApplyTheme();
            if (content != null)
            {
                content.Dock = DockStyle.Fill;
                dialog.Controls.Add(content);
            }
            if (owner != null)
            {
                dialog.PositionRelativeToOwner(owner, config.Position);
                dialog.Show(owner);
            }
            else
            {
                dialog.Show();
            }
        }

        public DialogResult ShowCommandPalette(IEnumerable<CommandAction> actions)
        {
            using var palette = new BeepCommandPaletteDialog();
            palette.SetActions(actions);
            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            return owner != null ? palette.ShowDialog(owner) : palette.ShowDialog();
        }

        // ShowQuickActions removed: its whole body was `return ShowCommandPalette(actions);`. It had
        // no callers and is not on IDialogManager, so it was a second name for one behaviour - the
        // same duplication as the Error/ShowError pairs, but without an [Obsolete] marker to make it
        // visible. In the reference products "quick actions" is a *scoped* palette, not a synonym;
        // if that is wanted it belongs as a filter argument to ShowCommandPalette, not a second entry
        // point that does exactly the same thing.

        #endregion

        #region Core Show Methods (Sync)

        /// <summary>
        /// Shows a dialog with full configuration.
        /// Safe to call from both UI and background threads via Control.Invoke.
        /// </summary>
        public DialogReturn Show(DialogConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            config.Style = config.Style == default ? _options.DefaultStyle : config.Style;
            config.Animation = config.Animation == default ? _defaultAnimation : config.Animation;
            config.AnimationDuration = config.AnimationDuration == 0 ? _animationDuration : config.AnimationDuration;
            config.ReducedMotion = config.ReducedMotion || _options.ReducedMotion;

            Control? uiTarget = _hostForm
                             ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);

            if (uiTarget != null && uiTarget.InvokeRequired)
            {
                DialogReturn result = default!;
                uiTarget.Invoke(new Action(() => result = ShowDialogInternal(config)));
                return result;
            }

            return ShowDialogInternal(config);
        }

        /// <summary>
        /// Internal method to show dialog
        /// </summary>
        private DialogReturn ShowDialogInternal(DialogConfig config)
        {
            // Forms must live on an STA thread that owns a message pump.
            // If this assertion fires, the caller is on a background thread
            // and ShowAsync should have been used instead of Show.
            System.Diagnostics.Debug.Assert(
                Thread.CurrentThread.GetApartmentState() == ApartmentState.STA,
                "BeepDialogManager: ShowDialogInternal must run on an STA thread.");
            // Backward-compatible behavior: if callers set CloseOnClickOutside,
            // ensure backdrop clicks actually dismiss unless a stricter policy is explicit.
            if (config.CloseOnClickOutside && config.BackdropClickPolicy == DialogBackdropClickPolicy.Ignore)
            {
                config.BackdropClickPolicy = DialogBackdropClickPolicy.CancelDialog;
            }

            DialogOpened?.Invoke(this, config);

            try
            {
                using Form dialog = CreateDialog(config);
                _activeModalDialog = dialog;
                _activeDialogConfig = config;

                // Set owner
                var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
                var previousFocusedControl = owner?.ActiveControl;

                if (config.RememberSizeAndPosition &&
                    !string.IsNullOrWhiteSpace(config.DialogKey))
                {
                    var persisted = DialogStateStore.Load(config.DialogKey);
                    if (persisted.HasValue)
                    {
                        dialog.StartPosition = FormStartPosition.Manual;
                        dialog.Location = persisted.Value.Location;
                        dialog.Size = persisted.Value.Size;
                    }
                    else if (_dialogRectState.TryGetValue(config.DialogKey, out var previous))
                    {
                        dialog.StartPosition = FormStartPosition.Manual;
                        dialog.Location = previous.Location;
                        dialog.Size = previous.Size;
                    }
                }

                // Validation gate. `ValidationCallback` was declared, documented as "called before
                // dialog closes on OK/Yes — return false to keep the dialog open", and **never
                // invoked anywhere**: a caller could set it and nothing would ever call it. It has
                // to run here rather than beside DataExtractionCallback, because by the time the
                // result is being built the dialog has already closed and there is nothing left to
                // cancel.
                FormClosingEventHandler? validationGate = null;
                if (config.ValidationCallback != null)
                {
                    validationGate = (_, e) =>
                    {
                        // Only gate an accept. Cancelling, or closing via the system menu, must
                        // never be blocked — a dialog the user cannot escape is a worse defect than
                        // the one this hook exists to prevent.
                        if (dialog.DialogResult is not (DialogResult.OK
                                                        or DialogResult.Yes))
                        {
                            return;
                        }

                        var pending = CreateDialogReturn(dialog, dialog.DialogResult);
                        if (!config.ValidationCallback(pending))
                        {
                            e.Cancel = true;
                        }
                    };
                    dialog.FormClosing += validationGate;
                }

                // Show dialog
                var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

                if (validationGate != null) dialog.FormClosing -= validationGate;
                var dialogReturn = CreateDialogReturn(dialog, result);
                if (dialogReturn.Submit) DialogConfirmed?.Invoke(this, dialogReturn);
                else DialogCancelled?.Invoke(this, dialogReturn);
                if (config.RememberSizeAndPosition && !string.IsNullOrWhiteSpace(config.DialogKey))
                {
                    var bounds = new Rectangle(dialog.Location, dialog.Size);
                    _dialogRectState[config.DialogKey] = bounds;
                    DialogStateStore.Save(config.DialogKey, bounds);
                }

                // Return focus to the previously active owner control when possible.
                if (previousFocusedControl != null && !previousFocusedControl.IsDisposed && previousFocusedControl.CanFocus)
                {
                    previousFocusedControl.Focus();
                }
                else if (owner != null && !owner.IsDisposed && owner.CanFocus)
                {
                    owner.Focus();
                }

                return dialogReturn;
            }
            finally
            {
                _activeModalDialog = null;
                _activeDialogConfig = null;
            }
        }

        /// <summary>
        /// Creates appropriate dialog form based on config
        /// </summary>
        internal Form CreateDialog(DialogConfig config)
        {
            Form dialog;
            var preset  = config.Preset;
            bool hasCustomControl = config.CustomControl != null;

            if (hasCustomControl)
            {
                var dlg = new BeepCustomDialog();
                dlg.Title = config.Title;
                dlg.PresetIntent = preset;
                dlg.TypedButtons = ResolveTypedButtons(config);
                dlg.SetCustomControl(config.CustomControl!);
                if (config.CustomControlMinHeight > 0)
                    dlg.ClientSize = new Size(dlg.ClientSize.Width, Math.Max(dlg.ClientSize.Height, config.CustomControlMinHeight + 120));
                config.InitializationCallback?.Invoke(config.CustomControl);
                dialog = dlg;
            }
            else if (preset == DialogPreset.Question
                  || config.Buttons?.Contains(BeepDialogButtons.YesNo) == true)
            {
                var dlg = new BeepQuestionDialog();
                dlg.Title = config.Title;
                dlg.Message = config.Message;
                dlg.DetailsText = config.Details;
                dlg.PresetIntent = preset;
                dlg.TypedButtons = ResolveTypedButtons(config);
                if (config.CustomButtonLabels != null) dlg.CustomButtonLabels = config.CustomButtonLabels;
                dialog = dlg;
            }
            else
            {
                var dlg = new BeepMessageDialog();
                dlg.Title = config.Title;
                dlg.Message = config.Message;
                dlg.DetailsText = config.Details;
                if (config.AutoCloseTimeout > 0) dlg.StartAutoClose(config.AutoCloseTimeout);
                dialog = dlg;
            }

            if (config.CustomSize.HasValue)
                dialog.Size = config.CustomSize.Value;

            if (config.Position == DialogPosition.Custom && config.CustomLocation.HasValue)
            {
                dialog.StartPosition = FormStartPosition.Manual;
                dialog.Location = config.CustomLocation.Value;
            }
            else
            {
                // Placed by DialogPlacementEngine rather than FormStartPosition.
                //
                // The engine had zero callers while placement was done by hand here and in 16 other
                // spots. Adopting it was gated on proving it agrees, and it does: for an owner inside
                // the work area it returns exactly what CenterParent computes — measured identical at
                // (540, 370) — so no dialog anyone uses today moves.
                //
                // What it adds is the two cases CenterParent gets wrong. With the owner against the
                // right edge, CenterParent puts the dialog at x=3090 on a 3440-wide desktop, hanging
                // it off the screen; the engine clamps it to 3012. And a dialog larger than the work
                // area gets an origin of (8, 8) instead of a negative one the user cannot drag back
                // from. Placement now happens deferred, because the dialog's final size is only known
                // after its layout has run.
                DialogPlacementStrategy strategy = config.Position switch
                {
                    DialogPosition.CenterScreen => DialogPlacementStrategy.CenterScreen,
                    _ => DialogPlacementStrategy.CenterOwner
                };

                Form placementOwner = _hostForm
                    ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null!);

                dialog.StartPosition = FormStartPosition.Manual;

                Form toPlace = dialog;
                void PlaceOnLoad(object? sender, EventArgs e)
                {
                    toPlace.Load -= PlaceOnLoad;
                    toPlace.Location = DialogPlacementEngine.Place(placementOwner, toPlace.Size, strategy);
                }

                dialog.Load += PlaceOnLoad;
            }

            return dialog;
        }

        private DialogButton[] ResolveTypedButtons(DialogConfig config)
        {
            if (config.TypedButtons is { Length: > 0 })
                return config.TypedButtons;
            if (config.Buttons != null && config.Buttons.Length > 0)
                return ConvertLegacyButtons(config.Buttons, config.CustomButtonLabels);
            return new[] { DialogButton.Ok() };
        }

        internal static DialogButton[] ConvertLegacyButtons(
            BeepDialogButtons[] buttons,
            Dictionary<BeepDialogButtons, string>? customLabels)
        {
            if (buttons.Length == 0)
            {
                return new[] { DialogButton.Ok() };
            }

            var typedButtons = new List<DialogButton>();
            foreach (var button in buttons)
            {
                switch (button)
                {
                    case BeepDialogButtons.OkCancel:
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.Ok, GetLabel(customLabels, BeepDialogButtons.Ok)));
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.Cancel, GetLabel(customLabels, BeepDialogButtons.Cancel)));
                        break;

                    case BeepDialogButtons.YesNo:
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.Yes, GetLabel(customLabels, BeepDialogButtons.Yes)));
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.No, GetLabel(customLabels, BeepDialogButtons.No)));
                        break;

                    case BeepDialogButtons.AbortRetryIgnore:
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.Abort, GetLabel(customLabels, BeepDialogButtons.Abort)));
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.Retry, GetLabel(customLabels, BeepDialogButtons.Retry)));
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.Ignore, GetLabel(customLabels, BeepDialogButtons.Ignore)));
                        break;

                    case BeepDialogButtons.SaveDontSaveCancel:
                        typedButtons.Add(DialogButton.Save(GetLabel(customLabels, BeepDialogButtons.Yes) ?? GetLabel(customLabels, BeepDialogButtons.Ok) ?? "Save"));
                        typedButtons.Add(DialogButton.DontSave(GetLabel(customLabels, BeepDialogButtons.No) ?? "Don't Save"));
                        typedButtons.Add(DialogButton.Cancel(GetLabel(customLabels, BeepDialogButtons.Cancel) ?? "Cancel"));
                        break;

                    case BeepDialogButtons.SaveAllDontSaveCancel:
                        typedButtons.Add(DialogButton.Save(GetLabel(customLabels, BeepDialogButtons.Yes) ?? GetLabel(customLabels, BeepDialogButtons.Ok) ?? "Save All"));
                        typedButtons.Add(DialogButton.DontSave(GetLabel(customLabels, BeepDialogButtons.No) ?? "Don't Save"));
                        typedButtons.Add(DialogButton.Cancel(GetLabel(customLabels, BeepDialogButtons.Cancel) ?? "Cancel"));
                        break;

                    case BeepDialogButtons.TryAgainContinue:
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.Retry, GetLabel(customLabels, BeepDialogButtons.Retry) ?? "Try Again"));
                        typedButtons.Add(DialogButton.FromLegacy(BeepDialogButtons.Continue, GetLabel(customLabels, BeepDialogButtons.Continue) ?? "Continue"));
                        break;

                    default:
                        typedButtons.Add(DialogButton.FromLegacy(button, GetLabel(customLabels, button)));
                        break;
                }
            }

            return typedButtons.Count > 0 ? typedButtons.ToArray() : new[] { DialogButton.Ok() };
        }

        private static string? GetLabel(Dictionary<BeepDialogButtons, string>? customLabels, BeepDialogButtons button)
            => customLabels != null && customLabels.TryGetValue(button, out var label) ? label : null;

        /// <summary>
        /// Creates DialogReturn from dialog result
        /// </summary>
        private DialogReturn CreateDialogReturn(Form dialog, DialogResult result)
        {
            var dialogReturn = new DialogReturn
            {
                Result = ConvertDialogResult(result),
                Cancel = result == DialogResult.Cancel || result == DialogResult.No,
                Submit = result == DialogResult.OK || result == DialogResult.Yes,
            };

            if (dialog is BeepMessageDialog msgDialog)
            {
                dialogReturn.Value = msgDialog.ReturnValue;
                dialogReturn.UserAction = BeepDialogButtons.Ok;
            }
            else if (dialog is BeepQuestionDialog qDialog)
            {
                dialogReturn.Value = qDialog.ReturnValue;
                dialogReturn.UserAction = result == DialogResult.Yes ? BeepDialogButtons.Yes : BeepDialogButtons.No;
            }
            else if (dialog is BeepInputDialog inDialog)
            {
                dialogReturn.Value = inDialog.ReturnValue;
                dialogReturn.UserAction = result == DialogResult.OK ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel;
            }
            else if (dialog is BeepListDialog listDialog)
            {
                dialogReturn.Value = listDialog.ReturnValue;
                dialogReturn.Tag = listDialog.ReturnItem;
                dialogReturn.UserAction = result == DialogResult.OK ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel;
            }
            else if (dialog is BeepCustomDialog custDialog)
            {
                dialogReturn.Value = custDialog.ReturnValue;
                dialogReturn.UserAction = result == DialogResult.OK ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel;
                if (_activeDialogConfig?.DataExtractionCallback != null && dialogReturn.Submit)
                    _activeDialogConfig.DataExtractionCallback(dialogReturn);
            }
            else
            {
                dialogReturn.Value = string.Empty;
            }

            return dialogReturn;
        }

        #endregion

        #region Quick Semantic Dialogs (Async)

        void IDialogManager.MsgBox(string title, string promptText)
        {
            Show(DialogConfig.CreateInformation(title, promptText));
        }

        DialogReturn IDialogManager.ShowAlert(string title, string message, string icon)
        {
            var config = new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = icon?.ToLowerInvariant() switch
                {
                    "warning" => BeepDialogIcon.Warning,
                    "error" => BeepDialogIcon.Error,
                    "success" => BeepDialogIcon.Success,
                    _ => BeepDialogIcon.Information
                },
                Buttons = new[] { BeepDialogButtons.Ok }
            };
            return Show(config);
        }

        void IDialogManager.ShowMessege(string title, string message, string icon)
        {
            ((IDialogManager)this).ShowAlert(title, message, icon);
        }

        void IDialogManager.ShowException(string title, Exception ex)
        {
            var config = new DialogConfig
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Error" : title,
                Message = ex.Message,
                Details = ex.StackTrace ?? string.Empty,
                IconType = BeepDialogIcon.Error,
                Preset = DialogPreset.Danger,
                Buttons = new[] { BeepDialogButtons.Ok }
            };
            Show(config);
        }

        DialogReturn IDialogManager.Confirm(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon)
        {
            return ((IDialogManager)this).Confirm(title, message, buttons, icon, null);
        }

        DialogReturn IDialogManager.Confirm(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon, BeepDialogButtons? defaultButton)
        {
            var config = new DialogConfig
            {
                Title = title,
                Message = message,
                IconType = icon,
                Buttons = buttons,
                DefaultButton = defaultButton,
                Preset = DialogPreset.Question
            };
            return Show(config);
        }

        /// <summary>
        /// Shows a success dialog
        /// </summary>
        public DialogReturn Success(string title, string message)
        {
            return Show(DialogConfig.CreateSuccess(title, message));
        }

        /// <summary>
        /// Shows a warning dialog
        /// </summary>
        public DialogReturn Warning(string title, string message)
        {
            return Show(DialogConfig.CreateWarning(title, message));
        }

        /// <summary>
        /// Shows an error dialog
        /// </summary>
        public DialogReturn Error(string title, string message)
        {
            return Show(DialogConfig.CreateDanger(title, message));
        }

        /// <summary>
        /// Shows an information dialog
        /// </summary>
        public DialogReturn Info(string title, string message)
        {
            return Show(DialogConfig.CreateInformation(title, message));
        }

        /// <summary>
        /// Shows a question dialog
        /// </summary>
        public DialogReturn Question(string title, string message)
        {
            return Show(DialogConfig.CreateQuestion(title, message));
        }

        /// <summary>
        /// Shows a confirmation dialog and returns true if confirmed
        /// </summary>
        public bool Confirm(string title, string message)
        {
            return Show(DialogConfig.CreateQuestion(title, message)).Submit;
        }

        #endregion

        #region Animation

        private void ApplyShowAnimation(Form form, DialogShowAnimation animation, int durationMs)
        {
            if (form == null || animation == DialogShowAnimation.None)
                return;

            if (_options.ReducedMotion)
            {
                DialogMotionEngine.AnimateOpacity(form, 0, 1, Math.Min(120, durationMs), DialogAnimationEasing.EaseOutCubic);
                return;
            }

            switch (animation)
            {
                case DialogShowAnimation.FadeIn:
                    AnimateFadeIn(form, durationMs);
                    break;
                case DialogShowAnimation.SlideInFromTop:
                    AnimateSlideIn(form, durationMs, SlideDirection.Top);
                    break;
                case DialogShowAnimation.SlideInFromBottom:
                    AnimateSlideIn(form, durationMs, SlideDirection.Bottom);
                    break;
                case DialogShowAnimation.SlideInFromLeft:
                    AnimateSlideIn(form, durationMs, SlideDirection.Left);
                    break;
                case DialogShowAnimation.SlideInFromRight:
                    AnimateSlideIn(form, durationMs, SlideDirection.Right);
                    break;
                case DialogShowAnimation.ZoomIn:
                    AnimateZoomIn(form, durationMs);
                    break;
            }
        }

        private enum SlideDirection { Top, Bottom, Left, Right }

        private void AnimateFadeIn(Form form, int durationMs)
        {
            DialogMotionEngine.AnimateOpacity(form, 0, 1, durationMs, DialogAnimationEasing.EaseOutCubic, animationKey: $"dialog-opacity-{form.Handle}");
        }

        private void AnimateSlideIn(Form form, int durationMs, SlideDirection direction)
        {
            var finalLocation = form.Location;
            int distance = 50;

            form.Location = direction switch
            {
                SlideDirection.Top => new Point(finalLocation.X, finalLocation.Y - distance),
                SlideDirection.Bottom => new Point(finalLocation.X, finalLocation.Y + distance),
                SlideDirection.Left => new Point(finalLocation.X - distance, finalLocation.Y),
                SlideDirection.Right => new Point(finalLocation.X + distance, finalLocation.Y),
                _ => finalLocation
            };

            DialogMotionEngine.AnimateOpacity(form, 0, 1, durationMs, DialogAnimationEasing.EaseOutCubic, animationKey: $"dialog-opacity-{form.Handle}");
            DialogMotionEngine.AnimateTranslate(form, form.Location, finalLocation, durationMs, DialogAnimationEasing.EaseOutBack, animationKey: $"dialog-translate-{form.Handle}");
        }

        private void AnimateZoomIn(Form form, int durationMs)
        {
            var finalSize = form.Size;
            var finalLocation = form.Location;

            form.Size = new Size(finalSize.Width / 2, finalSize.Height / 2);
            form.Location = new Point(
                finalLocation.X + finalSize.Width / 4,
                finalLocation.Y + finalSize.Height / 4
            );
            form.Opacity = 0;

            int steps = 10;
            int interval = durationMs / steps;
            int step = 0;

            var timer = new System.Windows.Forms.Timer { Interval = Math.Max(1, interval) };
            timer.Tick += (s, e) =>
            {
                step++;
                double progress = (double)step / steps;
                form.Opacity = progress;

                int currentWidth = (int)(form.Size.Width + (finalSize.Width - form.Size.Width) * progress);
                int currentHeight = (int)(form.Size.Height + (finalSize.Height - form.Size.Height) * progress);
                form.Size = new Size(currentWidth, currentHeight);

                int currentX = (int)(form.Location.X + (finalLocation.X - form.Location.X) * progress);
                int currentY = (int)(form.Location.Y + (finalLocation.Y - form.Location.Y) * progress);
                form.Location = new Point(currentX, currentY);

                if (step >= steps)
                {
                    timer.Stop();
                    timer.Dispose();
                    form.Opacity = 1;
                    form.Size = finalSize;
                    form.Location = finalLocation;
                }
            };
            timer.Start();
        }

        private void AnimateFadeOut(Form form, int durationMs, Action? completed = null)
        {
            DialogMotionEngine.AnimateOpacity(form, form.Opacity, 0, durationMs, DialogAnimationEasing.EaseInOutQuad, completed);
        }

        private void AnimateSlideOut(Form form, int durationMs, SlideDirection direction, Action? completed = null)
        {
            var from = form.Location;
            var distance = 40;
            var to = direction switch
            {
                SlideDirection.Top => new Point(from.X, from.Y - distance),
                SlideDirection.Bottom => new Point(from.X, from.Y + distance),
                SlideDirection.Left => new Point(from.X - distance, from.Y),
                _ => new Point(from.X + distance, from.Y)
            };
            DialogMotionEngine.AnimateOpacity(form, form.Opacity, 0, durationMs, DialogAnimationEasing.EaseInOutQuad);
            DialogMotionEngine.AnimateTranslate(form, from, to, durationMs, DialogAnimationEasing.EaseInOutQuad, completed);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Maps BeepDialogIcon to DialogType
        /// </summary>
        private DialogType MapIconToDialogType(BeepDialogIcon icon)
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
        /// Maps DialogPreset and BeepDialogIcon to DialogType
        /// </summary>
        private DialogType MapPresetToDialogType(DialogPreset preset, BeepDialogIcon icon)
        {
            if (preset != DialogPreset.None)
            {
                return preset switch
                {
                    DialogPreset.Success => DialogType.Information,
                    DialogPreset.SuccessWithUndo => DialogType.Information,
                    DialogPreset.Warning => DialogType.Warning,
                    DialogPreset.UnsavedChanges => DialogType.Warning,
                    DialogPreset.InlineValidation => DialogType.Warning,
                    DialogPreset.Danger => DialogType.Error,
                    DialogPreset.DestructiveConfirm => DialogType.Error,
                    DialogPreset.BlockingError => DialogType.Error,
                    DialogPreset.Question => DialogType.Question,
                    DialogPreset.SessionTimeout => DialogType.Question,
                    DialogPreset.Information => DialogType.Information,
                    DialogPreset.MultiStepProgress => DialogType.Information,
                    DialogPreset.Announcement => DialogType.Information,
                    _ => DialogType.None
                };
            }

            return MapIconToDialogType(icon);
        }

        /// <summary>
        /// Maps buttons array to BeepDialogButtons enum
        /// </summary>
        private BeepDialogButtons MapButtonsArray(BeepDialogButtons[] buttons)
        {
            if (buttons == null || buttons.Length == 0)
                return BeepDialogButtons.Ok;

            // Check for common combinations
            if (buttons.Length == 2)
            {
                if (ContainsButtons(buttons, BeepDialogButtons.Ok, BeepDialogButtons.Cancel))
                    return BeepDialogButtons.OkCancel;
                if (ContainsButtons(buttons, BeepDialogButtons.Yes, BeepDialogButtons.No))
                    return BeepDialogButtons.YesNo;
            }

            if (buttons.Length == 3)
            {
                if (ContainsButtons(buttons, BeepDialogButtons.Abort, BeepDialogButtons.Retry, BeepDialogButtons.Ignore))
                    return BeepDialogButtons.AbortRetryIgnore;

                // Compatibility mapping: callers may pass Yes/No/Cancel for unsaved-changes style prompts.
                if (ContainsButtons(buttons, BeepDialogButtons.Yes, BeepDialogButtons.No, BeepDialogButtons.Cancel))
                    return BeepDialogButtons.SaveDontSaveCancel;
            }

            // Default to first button or Ok
            return buttons[0];
        }

        private bool ContainsButtons(BeepDialogButtons[] buttons, params BeepDialogButtons[] check)
        {
            foreach (var btn in check)
            {
                bool found = false;
                foreach (var b in buttons)
                {
                    if (b == btn) { found = true; break; }
                }
                if (!found) return false;
            }
            return true;
        }

        internal void StoreRecentInput(DialogConfig config, string value)
        {
            if (!config.EnableRecentInputMemory || string.IsNullOrWhiteSpace(config.DialogKey) || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (!_recentInputMemory.TryGetValue(config.DialogKey, out var queue))
            {
                queue = new Queue<string>();
                _recentInputMemory[config.DialogKey] = queue;
            }

            if (queue.Count >= Math.Max(1, config.RecentInputCapacity))
            {
                queue.Dequeue();
            }
            queue.Enqueue(value);
        }

        public IReadOnlyCollection<string> GetRecentInputs(string dialogKey)
        {
            if (string.IsNullOrWhiteSpace(dialogKey) || !_recentInputMemory.TryGetValue(dialogKey, out var queue))
            {
                return Array.Empty<string>();
            }
            return queue.ToArray();
        }

        public bool ConfirmDestructiveWithUndo(string title, string message, Action undoAction)
        {
            var result = Show(DialogConfig.CreateDanger(title, message));
            if (result.Submit)
            {
                var dedupeKey = $"undo::{title?.Trim()}::{message?.Trim()}";
                SnackbarUndo("Action completed", undoAction, 4000, dedupeKey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts DialogResult to BeepDialogResult
        /// </summary>
        private BeepDialogResult ConvertDialogResult(DialogResult result)
        {
            return result switch
            {
                DialogResult.OK => BeepDialogResult.OK,
                DialogResult.Cancel => BeepDialogResult.Cancel,
                DialogResult.Yes => BeepDialogResult.Yes,
                DialogResult.No => BeepDialogResult.No,
                DialogResult.Abort => BeepDialogResult.Abort,
                DialogResult.Retry => BeepDialogResult.Retry,
                DialogResult.Ignore => BeepDialogResult.Ignore,
                _ => BeepDialogResult.None
            };
        }

        #endregion

        #region Cleanup (Phase 18)
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _activeModalDialog = null;
            _hostForm = null;
        }
        #endregion

    }
}

