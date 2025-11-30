using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
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
        private IBeepTheme? _defaultTheme;
        private BeepControlStyle _defaultStyle = BeepControlStyle.Material3;
        private DialogShowAnimation _defaultAnimation = DialogShowAnimation.FadeIn;
        private int _animationDuration = 200;
        private readonly Dictionary<int, IProgressHandle> _progressDialogs = new();
        private int _progressTokenCounter = 0;
        private readonly Queue<DialogRequest> _dialogQueue = new();
        private bool _isShowingDialog = false;
        private readonly List<Form> _activeToasts = new();
        private readonly object _toastLock = new object();

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
            return this;
        }

        /// <summary>
        /// Sets the default animation for dialogs
        /// </summary>
        public BeepDialogManager SetDefaultAnimation(DialogShowAnimation animation, int durationMs = 200)
        {
            _defaultAnimation = animation;
            _animationDuration = durationMs;
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

        #endregion

        #region Core Show Methods (Async)

        /// <summary>
        /// Shows a dialog with full configuration (async)
        /// </summary>
        public async Task<DialogReturn> ShowAsync(DialogConfig config, CancellationToken cancellationToken = default)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // Apply defaults if not specified
            config.Style = config.Style == default ? _defaultStyle : config.Style;
            config.Animation = config.Animation == default ? _defaultAnimation : config.Animation;
            config.AnimationDuration = config.AnimationDuration == 0 ? _animationDuration : config.AnimationDuration;

            var tcs = new TaskCompletionSource<DialogReturn>();

            void ShowDialogAction()
            {
                try
                {
                    var result = ShowDialogInternal(config);
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            // Ensure we're on the UI thread
            if (_hostForm != null && _hostForm.InvokeRequired)
            {
                _hostForm.BeginInvoke(new Action(ShowDialogAction));
            }
            else if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
            {
                Application.OpenForms[0].BeginInvoke(new Action(ShowDialogAction));
            }
            else
            {
                ShowDialogAction();
            }

            // Handle cancellation
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            }

            return await tcs.Task;
        }

        /// <summary>
        /// Shows a dialog with full configuration (sync)
        /// </summary>
        public DialogReturn Show(DialogConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // Apply defaults if not specified
            config.Style = config.Style == default ? _defaultStyle : config.Style;
            config.Animation = config.Animation == default ? _defaultAnimation : config.Animation;
            config.AnimationDuration = config.AnimationDuration == 0 ? _animationDuration : config.AnimationDuration;

            return ShowDialogInternal(config);
        }

        /// <summary>
        /// Internal method to show dialog
        /// </summary>
        private DialogReturn ShowDialogInternal(DialogConfig config)
        {
            // Show backdrop if configured
            if (config.ShowBackdrop && _hostForm != null)
            {
                ShowBackdrop(config.BackdropOpacity);
            }

            try
            {
                using var dialog = CreateDialog(config);

                // Apply animation
                if (config.Animation != DialogShowAnimation.None)
                {
                    ApplyShowAnimation(dialog, config.Animation, config.AnimationDuration);
                }

                // Set owner
                var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);

                // Show dialog
                var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

                return CreateDialogReturn(dialog, result);
            }
            finally
            {
                HideBackdrop();
            }
        }

        /// <summary>
        /// Creates appropriate dialog form based on config
        /// </summary>
        private BeepDialogModal CreateDialog(DialogConfig config)
        {
            var dialog = new BeepDialogModal();

            // Set basic properties
            dialog.Title = config.Title;
            dialog.Message = config.Message;
            dialog.DialogType = MapPresetToDialogType(config.Preset, config.IconType);

            // Map buttons
            if (config.Buttons != null && config.Buttons.Length > 0)
            {
                dialog.DialogButtons = MapButtonsArray(config.Buttons);
            }

            // Set items for list dialogs
            if (config.CustomControl == null && config.Preset == DialogPreset.None)
            {
                // Check if this is an input dialog that needs items
            }

            // Apply theme
            if (config.UseBeepThemeColors)
            {
                var theme = _defaultTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;
                dialog.Theme = theme?.ThemeName ?? string.Empty;
            }

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

            return dialog;
        }

        /// <summary>
        /// Creates DialogReturn from dialog result
        /// </summary>
        private DialogReturn CreateDialogReturn(BeepDialogModal dialog, System.Windows.Forms.DialogResult result)
        {
            return new DialogReturn
            {
                Result = ConvertDialogResult(result),
                Value = dialog.ReturnValue ?? string.Empty,
                Tag = dialog.ReturnItem,
                Cancel = result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.No,
                Submit = result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes
            };
        }

        #endregion

        #region Quick Semantic Dialogs (Async)

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

        private void ShowBackdrop(float opacity)
        {
            if (_hostForm == null) return;

            if (_backdropOverlay == null)
            {
                _backdropOverlay = new Panel
                {
                    BackColor = Color.FromArgb((int)(opacity * 255), 0, 0, 0),
                    Dock = DockStyle.Fill,
                    Visible = false
                };
                _hostForm.Controls.Add(_backdropOverlay);
            }

            _backdropOverlay.BackColor = Color.FromArgb((int)(opacity * 255), 0, 0, 0);
            _backdropOverlay.BringToFront();
            _backdropOverlay.Visible = true;
        }

        private void HideBackdrop()
        {
            if (_backdropOverlay != null)
            {
                _backdropOverlay.Visible = false;
            }
        }

        #endregion

        #region Animation

        private void ApplyShowAnimation(Form form, DialogShowAnimation animation, int durationMs)
        {
            if (form == null || animation == DialogShowAnimation.None)
                return;

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
            form.Opacity = 0;
            int steps = 10;
            int interval = durationMs / steps;
            int step = 0;

            var timer = new System.Windows.Forms.Timer { Interval = Math.Max(1, interval) };
            timer.Tick += (s, e) =>
            {
                step++;
                form.Opacity = (double)step / steps;
                if (step >= steps)
                {
                    timer.Stop();
                    timer.Dispose();
                    form.Opacity = 1;
                }
            };
            timer.Start();
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

                int currentX = (int)(form.Location.X + (finalLocation.X - form.Location.X) * progress);
                int currentY = (int)(form.Location.Y + (finalLocation.Y - form.Location.Y) * progress);
                form.Location = new Point(currentX, currentY);

                if (step >= steps)
                {
                    timer.Stop();
                    timer.Dispose();
                    form.Opacity = 1;
                    form.Location = finalLocation;
                }
            };
            timer.Start();
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
                    DialogPreset.Warning => DialogType.Warning,
                    DialogPreset.Danger => DialogType.Error,
                    DialogPreset.Question => DialogType.Question,
                    DialogPreset.Information => DialogType.Information,
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

        #region Dialog Request Queue

        private class DialogRequest
        {
            public DialogConfig Config { get; set; } = new();
            public TaskCompletionSource<DialogReturn> TaskSource { get; set; } = new();
        }

        #endregion
    }
}

