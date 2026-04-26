using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
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
        private Panel? _backdropOverlay;
        private DialogBackdropForm? _backdropForm;
        private IBeepTheme? _defaultTheme;
        private BeepControlStyle _defaultStyle = BeepControlStyle.Material3;
        private DialogShowAnimation _defaultAnimation = DialogShowAnimation.FadeIn;
        private int _animationDuration = 200;
        private DialogManagerOptions _options = new DialogManagerOptions();
        private readonly Dictionary<string, Rectangle> _dialogRectState = new();
        private readonly Dictionary<string, Queue<string>> _recentInputMemory = new();
        private BeepDialogForm? _activeModalDialog;
        private DialogConfig? _activeDialogConfig;

        public event EventHandler<DialogConfig>? DialogOpened;
        public event EventHandler<DialogReturn>? DialogConfirmed;
        public event EventHandler<DialogReturn>? DialogCancelled;
        public event EventHandler<DialogConfig>? DialogDismissedByBackdrop;

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
        public System.Windows.Forms.DialogResult ShowWizard(WizardConfig config)
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

        public System.Windows.Forms.DialogResult ShowCommandPalette(IEnumerable<CommandAction> actions)
        {
            using var palette = new BeepCommandPaletteDialog();
            palette.SetActions(actions);
            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            return owner != null ? palette.ShowDialog(owner) : palette.ShowDialog();
        }

        public System.Windows.Forms.DialogResult ShowQuickActions(IEnumerable<CommandAction> actions)
        {
            return ShowCommandPalette(actions);
        }

        #endregion

        #region Core Show Methods (Async)

        /// <summary>
        /// Shows a dialog with full configuration (async).
        /// Safe to call from both UI and background threads.
        /// When already on the UI thread the dialog runs synchronously via
        /// ShowDialog() so that callers using .GetAwaiter().GetResult() don't
        /// deadlock the WinForms SynchronizationContext.
        /// </summary>
        public Task<DialogReturn> ShowAsync(DialogConfig config, CancellationToken cancellationToken = default)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // Apply defaults if not specified
            config.Style = config.Style == default ? _options.DefaultStyle : config.Style;
            config.Animation = config.Animation == default ? _defaultAnimation : config.Animation;
            config.AnimationDuration = config.AnimationDuration == 0 ? _animationDuration : config.AnimationDuration;
            config.ReducedMotion = config.ReducedMotion || _options.ReducedMotion;

            // --- Already on the UI thread → run inline, return completed task ----
            bool onUiThread = (_hostForm != null && !_hostForm.InvokeRequired)
                           || (_hostForm == null && Application.OpenForms.Count > 0 && !Application.OpenForms[0].InvokeRequired)
                           || (_hostForm == null && Application.OpenForms.Count == 0);

            if (onUiThread)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = ShowDialogInternal(config);
                    return Task.FromResult(result);
                }
                catch (OperationCanceledException)
                {
                    return Task.FromCanceled<DialogReturn>(cancellationToken);
                }
                catch (Exception ex)
                {
                    return Task.FromException<DialogReturn>(ex);
                }
            }

            // --- Background thread → marshal to UI via BeginInvoke ---------------
            var tcs = new TaskCompletionSource<DialogReturn>(TaskCreationOptions.RunContinuationsAsynchronously);

            void ShowDialogAction()
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.TrySetCanceled(cancellationToken);
                        return;
                    }
                    var result = ShowDialogInternal(config);
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            var target = _hostForm as Control
                      ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);

            if (target != null)
            {
                target.BeginInvoke(new Action(ShowDialogAction));
            }
            else
            {
                // No UI control available – run inline (best-effort)
                ShowDialogAction();
            }

            if (cancellationToken.CanBeCanceled)
            {
                var registration = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
                tcs.Task.ContinueWith(_ => registration.Dispose(), TaskContinuationOptions.ExecuteSynchronously);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Shows a dialog with full configuration (sync)
        /// </summary>
        [Obsolete("Use ShowAsync for async-first dialog flow.")]
        public DialogReturn Show(DialogConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // Apply defaults if not specified
            config.Style = config.Style == default ? _options.DefaultStyle : config.Style;
            config.Animation = config.Animation == default ? _defaultAnimation : config.Animation;
            config.AnimationDuration = config.AnimationDuration == 0 ? _animationDuration : config.AnimationDuration;
            config.ReducedMotion = config.ReducedMotion || _options.ReducedMotion;

            return ShowDialogInternal(config);
        }

        /// <summary>
        /// Internal method to show dialog
        /// </summary>
        private DialogReturn ShowDialogInternal(DialogConfig config)
        {
            // Backward-compatible behavior: if callers set CloseOnClickOutside,
            // ensure backdrop clicks actually dismiss unless a stricter policy is explicit.
            if (config.CloseOnClickOutside && config.BackdropClickPolicy == DialogBackdropClickPolicy.Ignore)
            {
                config.BackdropClickPolicy = DialogBackdropClickPolicy.CancelDialog;
            }

            // Show backdrop if configured
            if (config.ShowBackdrop && _hostForm != null)
            {
                ShowBackdrop(config);
            }
            DialogOpened?.Invoke(this, config);

            try
            {
                using var dialog = CreateDialog(config);
                _activeModalDialog = dialog;
                _activeDialogConfig = config;

                // Apply animation
                if (config.Animation != DialogShowAnimation.None)
                {
                    ApplyShowAnimation(dialog, config.Animation, config.AnimationDuration);
                }

                // Set owner
                var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
                var previousFocusedControl = owner?.ActiveControl;

                if (config.RememberSizeAndPosition &&
                    !string.IsNullOrWhiteSpace(config.DialogKey) &&
                    _dialogRectState.TryGetValue(config.DialogKey, out var previous))
                {
                    dialog.StartPosition = FormStartPosition.Manual;
                    dialog.Location = previous.Location;
                    dialog.Size = previous.Size;
                }

                // Show dialog
                var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
                var dialogReturn = CreateDialogReturn(dialog, result);
                if (dialogReturn.Submit) DialogConfirmed?.Invoke(this, dialogReturn);
                else DialogCancelled?.Invoke(this, dialogReturn);
                if (config.RememberSizeAndPosition && !string.IsNullOrWhiteSpace(config.DialogKey))
                {
                    _dialogRectState[config.DialogKey] = new Rectangle(dialog.Location, dialog.Size);
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
                HideBackdrop();
            }
        }

        /// <summary>
        /// Creates appropriate dialog form based on config
        /// </summary>
        private BeepDialogForm CreateDialog(DialogConfig config)
        {
            var dialog = new BeepDialogForm();

            // ── Non-triggering properties first ────────────────────────────────────────
            // These do NOT call ConfigureForDialogType, so order among them doesn't matter.
            dialog.Title          = config.Title;
            dialog.Message        = config.Message;
            dialog.DetailsText    = config.Details;
            dialog.PresetIntent   = config.Preset;   // read by SetIcon()

            dialog.AllowEscapeClose              = config.CloseOnEscape;
            dialog.DefaultActionButton           = config.DefaultButton;
            dialog.RequireTypedConfirmation      = config.RequireTypedConfirmation;
            dialog.ConfirmationKeyword           = config.ConfirmationKeyword;
            dialog.DisablePrimaryUntilAcknowledged = config.DisablePrimaryUntilAcknowledged;
            dialog.AllowCaptionCloseButton       = config.ShowCloseButton;

            // CustomButtonLabels is read by GetLabel() inside SetButtonVisibilityAndCaptions —
            // must be set before DialogButtons and DialogType trigger ConfigureForDialogType.
            if (config.CustomButtonLabels != null && config.CustomButtonLabels.Count > 0)
                dialog.CustomButtonLabels = config.CustomButtonLabels;

            // ── Triggering properties — order matters ──────────────────────────────────
            // DialogButtons setter triggers ConfigureForDialogType. Set it before DialogType
            // so that when DialogType is set the button reconfigure uses the intended set.
            if (config.Buttons != null && config.Buttons.Length > 0)
                dialog.DialogButtons = MapButtonsArray(config.Buttons);

            // DialogType is set last — its setter triggers the authoritative
            // ConfigureForDialogType call that now sees all correct state.
            dialog.DialogType = MapPresetToDialogType(config.Preset, config.IconType);

            // Set items for list dialogs
            if (config.CustomControl == null && config.Preset == DialogPreset.None)
            {
                // Check if this is an input dialog that needs items
            }

            // Apply theme — set CurrentTheme (not just the name string) so _currentTheme
            // is non-null when ApplyTheme() runs and guards don't return early.
            {
                var theme = _defaultTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;
                if (theme != null)
                    dialog.CurrentTheme = theme;  // sets _currentTheme + ThemeName
            }
            //dialog.ApplyTheme();

            // Set position
            dialog.StartPosition = config.Position switch
            {
                DialogPosition.CenterParent => FormStartPosition.CenterParent,
                DialogPosition.CenterScreen => FormStartPosition.CenterScreen,
                DialogPosition.Custom when config.CustomLocation.HasValue => FormStartPosition.Manual,
                _ => FormStartPosition.CenterParent
            };

            if (config.Position == DialogPosition.Custom && config.CustomLocation.HasValue)
            {
                dialog.Location = config.CustomLocation.Value;
            }

            // Set size if custom
            if (config.CustomSize.HasValue)
            {
                dialog.Size = config.CustomSize.Value;
            }

            // Wire auto-close countdown from DialogConfig.AutoCloseTimeout.
            if (config.AutoCloseTimeout > 0)
                dialog.StartAutoClose(config.AutoCloseTimeout);

            return dialog;
        }

        /// <summary>
        /// Creates DialogReturn from dialog result
        /// </summary>
        private DialogReturn CreateDialogReturn(BeepDialogForm dialog, System.Windows.Forms.DialogResult result)
        {
            return new DialogReturn
            {
                Result = ConvertDialogResult(result),
                Value = dialog.ReturnValue ?? string.Empty,
                Tag = dialog.ReturnItem,
                Cancel = result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.No,
                Submit = result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes,
                UserAction = result switch
                {
                    System.Windows.Forms.DialogResult.Yes => BeepDialogButtons.Yes,
                    System.Windows.Forms.DialogResult.No => BeepDialogButtons.No,
                    System.Windows.Forms.DialogResult.Cancel => BeepDialogButtons.Cancel,
                    System.Windows.Forms.DialogResult.Abort => BeepDialogButtons.Abort,
                    System.Windows.Forms.DialogResult.Retry => BeepDialogButtons.Retry,
                    System.Windows.Forms.DialogResult.Ignore => BeepDialogButtons.Ignore,
                    _ => BeepDialogButtons.Ok
                }
            };
        }

        #endregion

        #region Quick Semantic Dialogs (Async)

        async Task IDialogManager.MsgBoxAsync(string title, string promptText, CancellationToken cancellationToken)
        {
            await ShowAsync(DialogConfig.CreateInformation(title, promptText), cancellationToken);
        }

        Task<DialogReturn> IDialogManager.ShowAlertAsync(string title, string message, string icon, CancellationToken cancellationToken)
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
            return ShowAsync(config, cancellationToken);
        }

        async Task IDialogManager.ShowMessegeAsync(string title, string message, string icon, CancellationToken cancellationToken)
        {
            await ((IDialogManager)this).ShowAlertAsync(title, message, icon, cancellationToken);
        }

        async Task IDialogManager.ShowExceptionAsync(string title, Exception ex, CancellationToken cancellationToken)
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
            await ShowAsync(config, cancellationToken);
        }

        Task<DialogReturn> IDialogManager.ConfirmAsync(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon, CancellationToken cancellationToken)
        {
            return ((IDialogManager)this).ConfirmAsync(title, message, buttons, icon, null, cancellationToken);
        }

        Task<DialogReturn> IDialogManager.ConfirmAsync(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon, BeepDialogButtons? defaultButton, CancellationToken cancellationToken)
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
            return ShowAsync(config, cancellationToken);
        }

        /// <summary>
        /// Shows a success dialog
        /// </summary>
        public Task<DialogReturn> Success(string title, string message)
        {
            return ShowAsync(DialogConfig.CreateSuccess(title, message));
        }

        /// <summary>
        /// Shows a success dialog (sync)
        /// </summary>
        [Obsolete("Use Success(...) or ShowAsync(...) for async-first dialog flow.")]
        public DialogReturn ShowSuccess(string title, string message)
        {
            return Show(DialogConfig.CreateSuccess(title, message));
        }

        /// <summary>
        /// Shows a warning dialog
        /// </summary>
        public Task<DialogReturn> Warning(string title, string message)
        {
            return ShowAsync(DialogConfig.CreateWarning(title, message));
        }

        /// <summary>
        /// Shows a warning dialog (sync)
        /// </summary>
        [Obsolete("Use Warning(...) or ShowAsync(...) for async-first dialog flow.")]
        public DialogReturn ShowWarning(string title, string message)
        {
            return Show(DialogConfig.CreateWarning(title, message));
        }

        /// <summary>
        /// Shows an error dialog
        /// </summary>
        public Task<DialogReturn> Error(string title, string message)
        {
            return ShowAsync(DialogConfig.CreateDanger(title, message));
        }

        /// <summary>
        /// Shows an error dialog (sync)
        /// </summary>
        [Obsolete("Use Error(...) or ShowAsync(...) for async-first dialog flow.")]
        public DialogReturn ShowError(string title, string message)
        {
            return Show(DialogConfig.CreateDanger(title, message));
        }

        /// <summary>
        /// Shows an information dialog
        /// </summary>
        public Task<DialogReturn> Info(string title, string message)
        {
            return ShowAsync(DialogConfig.CreateInformation(title, message));
        }

        /// <summary>
        /// Shows an information dialog (sync)
        /// </summary>
        [Obsolete("Use Info(...) or ShowAsync(...) for async-first dialog flow.")]
        public DialogReturn ShowInfo(string title, string message)
        {
            return Show(DialogConfig.CreateInformation(title, message));
        }

        /// <summary>
        /// Shows a question dialog
        /// </summary>
        public Task<DialogReturn> Question(string title, string message)
        {
            return ShowAsync(DialogConfig.CreateQuestion(title, message));
        }

        /// <summary>
        /// Shows a question dialog (sync)
        /// </summary>
        [Obsolete("Use Question(...) or ShowAsync(...) for async-first dialog flow.")]
        public DialogReturn ShowQuestion(string title, string message)
        {
            return Show(DialogConfig.CreateQuestion(title, message));
        }

        /// <summary>
        /// Shows a confirmation dialog and returns true if confirmed
        /// </summary>
        public async Task<bool> ConfirmAsync(string title, string message)
        {
            var result = await ShowAsync(DialogConfig.CreateQuestion(title, message));
            return result.Submit;
        }

        /// <summary>
        /// Shows a confirmation dialog and returns true if confirmed (sync)
        /// </summary>
        public bool ConfirmSync(string title, string message)
        {
            var result = Show(DialogConfig.CreateQuestion(title, message));
            return result.Submit;
        }

        #endregion

        #region Backdrop Management

        private void ShowBackdrop(DialogConfig config)
        {
            if (_hostForm == null)
            {
                return;
            }
            float opacity = config.BackdropOpacity;

            if (_backdropForm == null || _backdropForm.IsDisposed)
            {
                _backdropForm = new DialogBackdropForm
                {
                    Opacity = 0
                };
                _backdropForm.SetBounds(
                    Screen.FromControl(_hostForm).Bounds.X,
                    Screen.FromControl(_hostForm).Bounds.Y,
                    Screen.FromControl(_hostForm).Bounds.Width,
                    Screen.FromControl(_hostForm).Bounds.Height);
                _backdropForm.Click += HandleBackdropClicked;
                _backdropForm.Show(_hostForm);
            }

            _backdropForm.BackdropStyle = config.BackdropStyle;
            _backdropForm.TargetOpacity = opacity;
            _backdropForm.BringToFront();
            DialogMotionEngine.AnimateOpacity(
                _backdropForm,
                0,
                opacity,
                Math.Max(120, _animationDuration),
                config.BackdropTransitionStyle == DialogBackdropTransitionStyle.CrossDissolve
                    ? DialogAnimationEasing.EaseInOutQuad
                    : DialogAnimationEasing.EaseOutCubic,
                animationKey: "backdrop-opacity");
            _backdropForm.Invalidate();
        }

        private void HideBackdrop()
        {
            if (_backdropForm != null && !_backdropForm.IsDisposed)
            {
                var backdrop = _backdropForm;
                // Null the field inside the callback AFTER the animation
                // completes.  Previously it was set to null here, which
                // could allow ShowBackdrop to create a second overlay
                // while the first was still fading out.
                DialogMotionEngine.AnimateOpacity(backdrop, backdrop.Opacity, 0, Math.Max(100, _animationDuration / 2), DialogAnimationEasing.EaseInOutQuad, () =>
                {
                    if (!backdrop.IsDisposed)
                    {
                        backdrop.Hide();
                        backdrop.Dispose();
                    }
                    // Clear reference only once the form is actually gone
                    if (_backdropForm == backdrop)
                        _backdropForm = null;
                }, animationKey: "backdrop-opacity");
            }
            if (_backdropOverlay != null)
            {
                _backdropOverlay.Visible = false;
            }
        }

        private void HandleBackdropClicked(object? sender, EventArgs e)
        {
            DialogDismissedByBackdrop?.Invoke(this, _activeDialogConfig ?? new DialogConfig { Title = "Backdrop" });
            if (_activeDialogConfig == null || _activeModalDialog == null || _activeModalDialog.IsDisposed)
            {
                return;
            }

            switch (_activeDialogConfig.BackdropClickPolicy)
            {
                case DialogBackdropClickPolicy.CancelDialog:
                    _activeModalDialog.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    _activeModalDialog.Close();
                    break;
                case DialogBackdropClickPolicy.CloseDialog:
                    _activeModalDialog.Close();
                    break;
                default:
                    break;
            }
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

        public async Task<bool> ConfirmDestructiveWithUndoAsync(string title, string message, Action undoAction)
        {
            var result = await ShowAsync(DialogConfig.CreateDanger(title, message));
            if (result.Submit)
            {
                var dedupeKey = $"undo::{title?.Trim()}::{message?.Trim()}";
                SnackbarUndo("Action completed", undoAction, 4000, dedupeKey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts System.Windows.Forms.DialogResult to BeepDialogResult
        /// </summary>
        private BeepDialogResult ConvertDialogResult(System.Windows.Forms.DialogResult result)
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
                _ => BeepDialogResult.None
            };
        }

        #endregion

    }
}

