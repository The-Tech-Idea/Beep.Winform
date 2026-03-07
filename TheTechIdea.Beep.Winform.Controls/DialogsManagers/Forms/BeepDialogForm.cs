using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WinDR = System.Windows.Forms.DialogResult;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    /// <summary>
    /// Programmatic modal dialog form — NO designer file.
    /// Follows the same pattern as <see cref="BeepCommandPaletteDialog"/>.
    /// </summary>
    public class BeepDialogForm : BeepiFormPro
    {
        // ─── Layout panels ─────────────────────────────────────────────────
        private readonly BeepPanel _headerPanel;
        private readonly BeepPanel _bodyPanel;
        private readonly BeepPanel _buttonPanel;

        // ─── Header controls ───────────────────────────────────────────────
        private readonly BeepImage  _dialogIcon;
        private readonly BeepLabel  _titleLabel;

        // ─── Body controls ─────────────────────────────────────────────────
        private readonly BeepLabel   _messageLabel;
        private readonly BeepTextBox _inputBox;
        private readonly BeepComboBox _comboBox;
        private readonly BeepLabel   _validationLabel;
        private readonly BeepButton  _detailsToggleButton;
        private readonly BeepLabel   _detailsLabel;
        private readonly BeepLabel   _confirmationHintLabel;
        private readonly BeepTextBox _confirmationBox;

        // ─── Button controls ───────────────────────────────────────────────
        private readonly BeepButton _leftButton;
        private readonly BeepButton _middleButton;
        private readonly BeepButton _rightButton;

        // ─── State ─────────────────────────────────────────────────────────
        private DialogType           _dialogType   = DialogType.None;
        private BeepDialogButtons    _dialogButtons = BeepDialogButtons.OkCancel;
        private bool _detailsExpanded;
        private bool _inputValidationPassed = true;
        private bool _typedConfirmationPassed = true;
        private bool _initialized;

        // ─── Auto-close countdown ──────────────────────────────────────────
        private System.Windows.Forms.Timer? _autoCloseTimer;
        private int  _autoCloseRemainingMs;
        private string _autoCloseBaseLabel = string.Empty;

        // ─── Public API ────────────────────────────────────────────────────
        public string ReturnValue { get; private set; } = string.Empty;
        public SimpleItem? ReturnItem { get; private set; }
        public List<SimpleItem>? Items { get; set; }
        public Dictionary<BeepDialogButtons, string>? CustomButtonLabels { get; set; }
        public bool AllowEscapeClose { get; set; } = true;
        public BeepDialogButtons? DefaultActionButton { get; set; }
        public DialogPreset PresetIntent { get; set; } = DialogPreset.None;
        public bool AllowCaptionCloseButton { get; set; } = true;
        public bool RequireTypedConfirmation { get; set; }
        public string ConfirmationKeyword { get; set; } = string.Empty;
        public bool DisablePrimaryUntilAcknowledged { get; set; }

        public string DetailsText
        {
            get => _detailsLabel.Text;
            set
            {
                _detailsLabel.Text = value ?? string.Empty;
                UpdateDetailsVisibility();
            }
        }

        public Func<string, string?>? InputValidator { get; set; }

        public string Title
        {
            get => _titleLabel.Text;
            set => _titleLabel.Text = value ?? string.Empty;
        }

        public string Message
        {
            get => _messageLabel.Text;
            set => _messageLabel.Text = value ?? string.Empty;
        }

        public DialogType DialogType
        {
            get => _dialogType;
            set { _dialogType = value; ConfigureForDialogType(); }
        }

        public BeepDialogButtons DialogButtons
        {
            get => _dialogButtons;
            set { _dialogButtons = value; ConfigureForDialogType(); }
        }

        // ─── Constructor ───────────────────────────────────────────────────
        public BeepDialogForm()
        {
            ShowCaptionBar    = false;
            ShowCloseButton   = false;
            ShowInTaskbar     = false;
            StartPosition     = FormStartPosition.CenterParent;
            ClientSize        = new Size(480, 220);
            MinimumSize       = new Size(360, 180);

            // ── Header panel (icon + title) ────────────────────────────────
            _headerPanel = new BeepPanel
            {
                Dock         = DockStyle.Top,
                Height       = 56,
                IsFrameless  = true,
                ShowTitle    = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding      = new Padding(8, 8, 8, 4)
            };

            _dialogIcon = new BeepImage
            {
                Size         = new Size(36, 36),
                Location     = new Point(8, 10),
                UseThemeColors = true,
                ImagePath    = Svgs.Information,
                ScaleMode    = ImageScaleMode.Stretch
            };

            _titleLabel = new BeepLabel
            {
                Location     = new Point(52, 14),
                Size         = new Size(400, 28),
                Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                IsFrameless  = true,
                Text         = string.Empty
            };

            _headerPanel.Controls.Add(_dialogIcon);
            _headerPanel.Controls.Add(_titleLabel);

            // ── Body panel (message + optional input) ──────────────────────
            _bodyPanel = new BeepPanel
            {
                Dock         = DockStyle.Fill,
                IsFrameless  = true,
                ShowTitle    = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding      = new Padding(12, 8, 12, 4),
                AutoScroll   = true
            };

            _messageLabel = new BeepLabel
            {
                Location     = new Point(12, 8),
                Size         = new Size(440, 60),
                Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                IsFrameless  = true,
                WordWrap     = true,
                Text         = string.Empty
            };

            _inputBox = new BeepTextBox
            {
                Location     = new Point(12, 76),
                Size         = new Size(440, 30),
                Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                Visible      = false
            };

            _comboBox = new BeepComboBox
            {
                Location     = new Point(12, 76),
                Size         = new Size(440, 30),
                Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                Visible      = false
            };

            _validationLabel = new BeepLabel
            {
                Location     = new Point(12, 110),
                Size         = new Size(440, 18),
                Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                IsFrameless  = true,
                Visible      = false,
                Text         = string.Empty
            };

            _detailsToggleButton = new BeepButton
            {
                Location     = new Point(12, 132),
                Size         = new Size(128, 26),
                UseThemeColors = true,
                Visible      = false,
                Text         = "Show details"
            };

            _detailsLabel = new BeepLabel
            {
                Location     = new Point(12, 164),
                Size         = new Size(440, 64),
                Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                IsFrameless  = true,
                WordWrap     = true,
                Visible      = false,
                Text         = string.Empty
            };

            _confirmationHintLabel = new BeepLabel
            {
                Location     = new Point(12, 132),
                Size         = new Size(440, 18),
                Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                IsFrameless  = true,
                Visible      = false,
                Text         = string.Empty
            };

            _confirmationBox = new BeepTextBox
            {
                Location     = new Point(12, 154),
                Size         = new Size(440, 30),
                Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                Visible      = false,
                PlaceholderText = "Type confirmation text"
            };

            _bodyPanel.Controls.Add(_messageLabel);
            _bodyPanel.Controls.Add(_inputBox);
            _bodyPanel.Controls.Add(_comboBox);
            _bodyPanel.Controls.Add(_validationLabel);
            _bodyPanel.Controls.Add(_detailsToggleButton);
            _bodyPanel.Controls.Add(_detailsLabel);
            _bodyPanel.Controls.Add(_confirmationHintLabel);
            _bodyPanel.Controls.Add(_confirmationBox);

            // ── Button panel ───────────────────────────────────────────────
            _buttonPanel = new BeepPanel
            {
                Dock         = DockStyle.Bottom,
                Height       = 54,
                IsFrameless  = true,
                ShowTitle    = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding      = new Padding(8, 8, 8, 8)
            };

            _leftButton = new BeepButton
            {
                Size     = new Size(110, 34),
                Location = new Point(10, 10),
                Anchor   = AnchorStyles.Left | AnchorStyles.Bottom,
                UseThemeColors = true,
                Visible  = false
            };

            _middleButton = new BeepButton
            {
                Size     = new Size(110, 34),
                Location = new Point(180, 10),   // centred; adjusted on resize
                Anchor   = AnchorStyles.Bottom,
                UseThemeColors = true,
                Visible  = false
            };

            _rightButton = new BeepButton
            {
                Size     = new Size(110, 34),
                Location = new Point(350, 10),
                Anchor   = AnchorStyles.Right | AnchorStyles.Bottom,
                UseThemeColors = true,
                Visible  = false
            };

            _buttonPanel.Controls.Add(_leftButton);
            _buttonPanel.Controls.Add(_middleButton);
            _buttonPanel.Controls.Add(_rightButton);

            // Wire buttons
            _leftButton.Click   += LeftButton_Click;
            _middleButton.Click += MiddleButton_Click;
            _rightButton.Click  += RightButton_Click;
            _detailsToggleButton.Click += DetailsToggleButton_Click;

            // Wire validation
            _inputBox.TextChanged += InputBox_TextChanged;
            _confirmationBox.TextChanged += ConfirmationBox_TextChanged;

            // ── Add panels to form (Dock stacking: Fill first, then Bottom, then Top) ─
            Controls.Add(_bodyPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_headerPanel);

            // All fields are now initialized — allow ConfigureForDialogType and OnResize to run.
            _initialized = true;

            // Default to Information / OkCancel
            ConfigureForDialogType();
        }

        // ─── Button handlers ───────────────────────────────────────────────

        private void LeftButton_Click(object? sender, EventArgs e)
        {
            switch (_dialogType)
            {
                case DialogType.GetInputString:
                    ReturnValue = _inputBox.Text;
                    DialogResult = WinDR.OK;
                    break;
                case DialogType.GetInputFromList:
                    ReturnValue  = _comboBox.SelectedItem?.Text ?? string.Empty;
                    ReturnItem   = _comboBox.SelectedItem;
                    DialogResult = WinDR.OK;
                    break;
                default:
                    ReturnValue  = _leftButton.Text;
                    DialogResult = _dialogButtons switch
                    {
                        BeepDialogButtons.YesNo              => WinDR.Yes,
                        BeepDialogButtons.AbortRetryIgnore   => WinDR.Abort,
                        BeepDialogButtons.SaveDontSaveCancel => WinDR.Yes,
                        BeepDialogButtons.SaveAllDontSaveCancel => WinDR.Yes,
                        BeepDialogButtons.TryAgainContinue   => WinDR.Retry,
                        _                                    => WinDR.OK
                    };
                    break;
            }
            Close();
        }

        private void MiddleButton_Click(object? sender, EventArgs e)
        {
            ReturnValue  = _middleButton.Text;
            DialogResult = _dialogButtons switch
            {
                BeepDialogButtons.AbortRetryIgnore      => WinDR.Ignore,
                BeepDialogButtons.SaveDontSaveCancel    => WinDR.No,
                BeepDialogButtons.SaveAllDontSaveCancel => WinDR.No,
                BeepDialogButtons.Close                 => WinDR.Cancel,
                BeepDialogButtons.Help                  => WinDR.OK,
                _                                       => WinDR.OK
            };
            Close();
        }

        private void RightButton_Click(object? sender, EventArgs e)
        {
            ReturnValue  = null;
            DialogResult = _dialogButtons switch
            {
                BeepDialogButtons.YesNo              => WinDR.No,
                BeepDialogButtons.AbortRetryIgnore   => WinDR.Retry,
                BeepDialogButtons.SaveDontSaveCancel => WinDR.Cancel,
                BeepDialogButtons.SaveAllDontSaveCancel => WinDR.Cancel,
                BeepDialogButtons.TryAgainContinue   => WinDR.Continue,
                _                                    => WinDR.Cancel
            };
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Tab | Keys.Shift))
            {
                CycleFocus(backward: true);
                return true;
            }

            if (keyData == Keys.Tab)
            {
                CycleFocus(backward: false);
                return true;
            }

            if (keyData == Keys.Enter)
            {
                var preferred = ResolveDefaultButtonControl() as BeepButton;
                if (preferred != null && preferred.Visible && preferred.Enabled)
                {
                    preferred.PerformClick();
                    return true;
                }

                if (_middleButton.Visible) { _middleButton.PerformClick(); return true; }
                if (_leftButton.Visible)   { _leftButton.PerformClick();   return true; }
            }
            if (keyData == Keys.Escape)
            {
                if (!AllowEscapeClose)
                {
                    // Consume ESC to enforce a blocking/acknowledgement flow.
                    return true;
                }

                if (_rightButton.Visible) { _rightButton.PerformClick(); return true; }
                DialogResult = WinDR.Cancel;
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Prefer explicit input focus for input dialogs.
            if (_inputBox.Visible)
            {
                _inputBox.Focus();
                return;
            }

            if (_comboBox.Visible)
            {
                _comboBox.Focus();
                return;
            }

            var preferred = ResolveDefaultButtonControl();
            preferred?.Focus();
        }

        private Control? ResolveDefaultButtonControl()
        {
            if (DefaultActionButton.HasValue)
            {
                var btn = MapLogicalButtonToControl(DefaultActionButton.Value);
                if (btn != null && btn.Visible)
                    return btn;
            }

            if (_middleButton.Visible) return _middleButton;
            if (_leftButton.Visible) return _leftButton;
            if (_rightButton.Visible) return _rightButton;
            return null;
        }

        private void CycleFocus(bool backward)
        {
            var order = new List<Control>();

            if (_inputBox.Visible && _inputBox.Enabled) order.Add(_inputBox);
            if (_comboBox.Visible && _comboBox.Enabled) order.Add(_comboBox);
            if (_confirmationBox.Visible && _confirmationBox.Enabled) order.Add(_confirmationBox);

            if (_leftButton.Visible && _leftButton.Enabled) order.Add(_leftButton);
            if (_middleButton.Visible && _middleButton.Enabled) order.Add(_middleButton);
            if (_rightButton.Visible && _rightButton.Enabled) order.Add(_rightButton);

            if (order.Count == 0)
                return;

            Control? current = ActiveControl;
            int index = order.IndexOf(current!);
            if (index < 0)
                index = backward ? 0 : -1;

            int next = backward ? (index - 1 + order.Count) % order.Count
                                : (index + 1) % order.Count;

            order[next].Focus();
        }

        private BeepButton? MapLogicalButtonToControl(BeepDialogButtons logicalButton)
        {
            switch (_dialogButtons)
            {
                case BeepDialogButtons.OkCancel:
                    return logicalButton == BeepDialogButtons.Ok ? _leftButton
                         : logicalButton == BeepDialogButtons.Cancel ? _rightButton
                         : null;

                case BeepDialogButtons.YesNo:
                    return logicalButton == BeepDialogButtons.Yes ? _leftButton
                         : logicalButton == BeepDialogButtons.No ? _rightButton
                         : null;

                case BeepDialogButtons.AbortRetryIgnore:
                    return logicalButton == BeepDialogButtons.Abort ? _leftButton
                         : logicalButton == BeepDialogButtons.Ignore ? _middleButton
                         : logicalButton == BeepDialogButtons.Retry ? _rightButton
                         : null;

                case BeepDialogButtons.SaveDontSaveCancel:
                case BeepDialogButtons.SaveAllDontSaveCancel:
                    return logicalButton == BeepDialogButtons.Yes ? _leftButton
                         : logicalButton == BeepDialogButtons.No ? _middleButton
                         : logicalButton == BeepDialogButtons.Cancel ? _rightButton
                         : null;

                case BeepDialogButtons.TryAgainContinue:
                    return logicalButton == BeepDialogButtons.Retry ? _leftButton
                         : logicalButton == BeepDialogButtons.Continue ? _rightButton
                         : null;

                case BeepDialogButtons.Close:
                case BeepDialogButtons.Help:
                    return _middleButton;

                default:
                    return logicalButton == BeepDialogButtons.Ok ? _leftButton
                         : logicalButton == BeepDialogButtons.Cancel ? _rightButton
                         : null;
            }
        }

        // ─── Configuration ─────────────────────────────────────────────────

        private void ConfigureForDialogType()
        {
            if (!_initialized)
                return; // Constructor not yet complete

            SetIcon();
            SetButtonVisibilityAndCaptions();
            SetInputVisibility();
            UpdateDetailsVisibility();
            UpdateTypedConfirmationVisibility();
            UpdatePrimaryActionEnabledState();
            ReflowBodyContent();
        }

        private void DetailsToggleButton_Click(object? sender, EventArgs e)
        {
            _detailsExpanded = !_detailsExpanded;
            UpdateDetailsVisibility();
        }

        private void ConfirmationBox_TextChanged(object? sender, EventArgs e)
        {
            if (!RequireTypedConfirmation)
            {
                _typedConfirmationPassed = true;
                UpdatePrimaryActionEnabledState();
                return;
            }

            _typedConfirmationPassed = string.Equals(
                (_confirmationBox.Text ?? string.Empty).Trim(),
                (ConfirmationKeyword ?? string.Empty).Trim(),
                StringComparison.OrdinalIgnoreCase);

            UpdatePrimaryActionEnabledState();
        }

        private void SetIcon()
        {
            if (_dialogIcon == null) return;

            // Preset intent wins over generic dialog type mapping.
            if (PresetIntent != DialogPreset.None)
            {
                _dialogIcon.ImagePath = PresetIntent switch
                {
                    DialogPreset.DestructiveConfirm => Svgs.Error,
                    DialogPreset.BlockingError => Svgs.Error,
                    DialogPreset.UnsavedChanges => Svgs.InfoWarning,
                    DialogPreset.InlineValidation => Svgs.InfoWarning,
                    DialogPreset.SessionTimeout => Svgs.InfoWarning,
                    DialogPreset.Success => Svgs.CheckCircle,
                    DialogPreset.SuccessWithUndo => Svgs.CheckCircle,
                    DialogPreset.MultiStepProgress => Svgs.Loading,
                    DialogPreset.Announcement => Svgs.Notice,
                    DialogPreset.Question => Svgs.Question,
                    DialogPreset.Warning => Svgs.InfoWarning,
                    DialogPreset.Danger => Svgs.Error,
                    _ => Svgs.Information
                };
                return;
            }

            _dialogIcon.ImagePath = _dialogType switch
            {
                DialogType.Warning          => Svgs.InfoWarning,
                DialogType.Error            => Svgs.Error,
                DialogType.Question         => Svgs.Question,
                DialogType.GetInputString   => Svgs.Input,
                DialogType.GetInputFromList => Svgs.Input,
                _                           => Svgs.Information
            };
        }

        private void SetButtonVisibilityAndCaptions()
        {
            if (_leftButton == null || _middleButton == null || _rightButton == null)
                return;

            // Hide all first
            _leftButton.Visible   = false;
            _middleButton.Visible = false;
            _rightButton.Visible  = false;

            switch (_dialogType)
            {
                case DialogType.Information:
                case DialogType.Warning:
                case DialogType.Error:
                    _middleButton.Text    = GetLabel(BeepDialogButtons.Ok, "OK");
                    _middleButton.Visible = true;
                    _middleButton.ImagePath = Svgs.Check;
                    break;

                case DialogType.None:
                case DialogType.GetInputString:
                case DialogType.GetInputFromList:
                    ApplyButtonSet();
                    break;

                case DialogType.Question:
                    ApplyButtonSet();
                    break;
            }

            PositionButtons();
        }

        private void ApplyButtonSet()
        {
            switch (_dialogButtons)
            {
                case BeepDialogButtons.OkCancel:
                    ShowLeftRight(
                        GetLabel(BeepDialogButtons.Ok, "OK"), Svgs.Check,
                        GetLabel(BeepDialogButtons.Cancel, "Cancel"), Svgs.Cancel);
                    break;
                case BeepDialogButtons.YesNo:
                    ShowLeftRight(
                        GetLabel(BeepDialogButtons.Yes, "Yes"), Svgs.Yes,
                        GetLabel(BeepDialogButtons.No, "No"), Svgs.No);
                    break;
                case BeepDialogButtons.AbortRetryIgnore:
                    ShowAll(
                        GetLabel(BeepDialogButtons.Abort, "Abort"), Svgs.Abort,
                        GetLabel(BeepDialogButtons.Ignore, "Ignore"), Svgs.InfoIgnore,
                        GetLabel(BeepDialogButtons.Retry, "Retry"), Svgs.TryAgain);
                    break;
                case BeepDialogButtons.SaveDontSaveCancel:
                    ShowAll(
                        GetLabel(BeepDialogButtons.Yes, "Save"), Svgs.Save,
                        GetLabel(BeepDialogButtons.No, "Don't Save"), Svgs.DontSave,
                        GetLabel(BeepDialogButtons.Cancel, "Cancel"), Svgs.Cancel);
                    break;
                case BeepDialogButtons.SaveAllDontSaveCancel:
                    ShowAll(
                        GetLabel(BeepDialogButtons.Yes, "Save All"), Svgs.SaveAll,
                        GetLabel(BeepDialogButtons.No, "Don't Save"), Svgs.DontSave,
                        GetLabel(BeepDialogButtons.Cancel, "Cancel"), Svgs.Cancel);
                    break;
                case BeepDialogButtons.Close:
                    _middleButton.Text      = GetLabel(BeepDialogButtons.Close, "Close");
                    _middleButton.ImagePath = Svgs.Close;
                    _middleButton.Visible   = true;
                    break;
                case BeepDialogButtons.Help:
                    _middleButton.Text      = GetLabel(BeepDialogButtons.Help, "Help");
                    _middleButton.ImagePath = Svgs.InfoHelp;
                    _middleButton.Visible   = true;
                    break;
                case BeepDialogButtons.TryAgainContinue:
                    ShowLeftRight(
                        GetLabel(BeepDialogButtons.Retry, "Try Again"), Svgs.TryAgain,
                        GetLabel(BeepDialogButtons.Continue, "Continue"), Svgs.Continue);
                    break;
                default:
                    ShowLeftRight(
                        GetLabel(BeepDialogButtons.Ok, "OK"), Svgs.Check,
                        GetLabel(BeepDialogButtons.Cancel, "Cancel"), Svgs.Cancel);
                    break;
            }
        }

        private string GetLabel(BeepDialogButtons button, string fallback)
        {
            if (CustomButtonLabels != null &&
                CustomButtonLabels.TryGetValue(button, out var value) &&
                !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            return fallback;
        }

        private void ShowLeftRight(string leftText, string leftIcon, string rightText, string rightIcon)
        {
            _leftButton.Text      = leftText;  _leftButton.ImagePath  = leftIcon;  _leftButton.Visible  = true;
            _rightButton.Text     = rightText; _rightButton.ImagePath = rightIcon; _rightButton.Visible = true;
        }

        private void ShowAll(string leftText, string leftIcon, string midText, string midIcon, string rightText, string rightIcon)
        {
            _leftButton.Text      = leftText;  _leftButton.ImagePath  = leftIcon;  _leftButton.Visible  = true;
            _middleButton.Text    = midText;   _middleButton.ImagePath = midIcon;  _middleButton.Visible = true;
            _rightButton.Text     = rightText; _rightButton.ImagePath = rightIcon; _rightButton.Visible = true;
        }

        private void PositionButtons()
        {
            if (!_initialized) return;

            const int btnW = 110, btnH = 34, margin = 10;
            int panelW = _buttonPanel.ClientSize.Width > 0 ? _buttonPanel.ClientSize.Width : 470;
            int y      = (_buttonPanel.ClientSize.Height > 0 ? _buttonPanel.ClientSize.Height : 54) / 2 - btnH / 2;

            if (_middleButton.Visible && !_leftButton.Visible && !_rightButton.Visible)
            {
                // Single centre button
                _middleButton.SetBounds((panelW - btnW) / 2, y, btnW, btnH);
            }
            else if (_leftButton.Visible && _rightButton.Visible && !_middleButton.Visible)
            {
                // Two buttons right-aligned
                _rightButton.SetBounds(panelW - margin - btnW,                y, btnW, btnH);
                _leftButton.SetBounds( panelW - margin - btnW * 2 - margin,  y, btnW, btnH);
            }
            else if (_leftButton.Visible && _middleButton.Visible && _rightButton.Visible)
            {
                // Three buttons right-aligned
                _rightButton.SetBounds( panelW - margin - btnW,                        y, btnW, btnH);
                _middleButton.SetBounds(panelW - margin - btnW * 2 - margin,           y, btnW, btnH);
                _leftButton.SetBounds(  panelW - margin - btnW * 3 - margin * 2,       y, btnW, btnH);
            }
        }

        private void SetInputVisibility()
        {
            _inputBox.Visible = false;
            _comboBox.Visible = false;
            _inputValidationPassed = true;

            if (_dialogType == DialogType.GetInputString)
            {
                _inputBox.Text    = string.Empty;
                _inputBox.Visible = true;
                // Focus is set in OnShown, not here — the form may not be visible yet.
            }
            else if (_dialogType == DialogType.GetInputFromList)
            {
                _comboBox.ListItems.Clear();
                if (Items != null)
                {
                    foreach (var item in Items)
                        _comboBox.ListItems.Add(item);
                    if (_comboBox.ListItems.Count > 0)
                    {
                        _comboBox.SelectedIndex = 0;
                        ReturnValue = _comboBox.SelectedItem?.Text ?? string.Empty;
                        ReturnItem  = _comboBox.SelectedItem;
                    }
                }
                _comboBox.Visible = true;
            }
        }

        private void UpdateDetailsVisibility()
        {
            if (_detailsLabel == null || _detailsToggleButton == null) return;

            bool hasDetails = !string.IsNullOrWhiteSpace(_detailsLabel.Text);
            _detailsToggleButton.Visible = hasDetails;

            _detailsLabel.Visible = hasDetails && _detailsExpanded;
            _detailsToggleButton.Text = _detailsExpanded ? "Hide details" : "Show details";

            // Keep confirmation controls below details section when expanded.
            int detailsBottom = _detailsLabel.Visible ? (_detailsLabel.Bottom + 8) : (_detailsToggleButton.Bottom + 6);
            _confirmationHintLabel.Top = detailsBottom;
            _confirmationBox.Top = _confirmationHintLabel.Bottom + 4;
            ReflowBodyContent();
        }

        private void UpdateTypedConfirmationVisibility()
        {
            if (_confirmationHintLabel == null || _confirmationBox == null) return;

            bool enabled = RequireTypedConfirmation;
            _typedConfirmationPassed = !enabled;

            _confirmationHintLabel.Visible = enabled;
            _confirmationBox.Visible = enabled;

            if (enabled)
            {
                var keyword = string.IsNullOrWhiteSpace(ConfirmationKeyword) ? "CONFIRM" : ConfirmationKeyword;
                _confirmationHintLabel.Text = $"Type '{keyword}' to continue.";
                _confirmationBox.Text = string.Empty;
                _confirmationBox.PlaceholderText = keyword;
            }
            else
            {
                _confirmationHintLabel.Text = string.Empty;
                _confirmationBox.Text = string.Empty;
            }

            ReflowBodyContent();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!_initialized) return; // Constructor not yet complete
            PositionButtons();
            ReflowBodyContent();
        }

        private void ReflowBodyContent()
        {
            if (_bodyPanel == null)
                return;

            int left = _bodyPanel.Padding.Left;
            int top = _bodyPanel.Padding.Top;
            int width = Math.Max(120, _bodyPanel.ClientSize.Width - _bodyPanel.Padding.Left - _bodyPanel.Padding.Right - 6);
            int y = top;

            int messageHeight = MeasureWrappedTextHeight(_messageLabel.Text, _messageLabel.Font, width, 40, 200);
            _messageLabel.SetBounds(left, y, width, messageHeight);
            y += messageHeight + 8;

            if (_inputBox.Visible)
            {
                _inputBox.SetBounds(left, y, width, 30);
                y += 34;
            }
            else if (_comboBox.Visible)
            {
                _comboBox.SetBounds(left, y, width, 30);
                y += 34;
            }

            if (_validationLabel.Visible)
            {
                _validationLabel.SetBounds(left, y, width, 18);
                y += 22;
            }

            if (_detailsToggleButton.Visible)
            {
                _detailsToggleButton.SetBounds(left, y, 130, 26);
                y += 30;
            }

            if (_detailsLabel.Visible)
            {
                int detailsHeight = MeasureWrappedTextHeight(_detailsLabel.Text, _detailsLabel.Font, width, 48, 220);
                _detailsLabel.SetBounds(left, y, width, detailsHeight);
                y += detailsHeight + 8;
            }

            if (_confirmationHintLabel.Visible)
            {
                _confirmationHintLabel.SetBounds(left, y, width, 18);
                y += 22;
            }

            if (_confirmationBox.Visible)
            {
                _confirmationBox.SetBounds(left, y, width, 30);
                y += 34;
            }

            _bodyPanel.AutoScrollMinSize = new Size(0, y + _bodyPanel.Padding.Bottom);
        }

        private static int MeasureWrappedTextHeight(string? text, Font? font, int width, int minHeight, int maxHeight)
        {
            if (width < 10)
                return minHeight;

            string value = string.IsNullOrWhiteSpace(text) ? " " : text;
            var measured = TextRenderer.MeasureText(
                value,
                font ?? SystemFonts.MessageBoxFont,
                new Size(width, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.LeftAndRightPadding);

            int height = Math.Max(minHeight, measured.Height + 4);
            return Math.Min(maxHeight, height);
        }

        private void UpdatePrimaryActionEnabledState()
        {
            bool enabled = _inputValidationPassed && _typedConfirmationPassed;
            if (DisablePrimaryUntilAcknowledged || RequireTypedConfirmation || !_inputValidationPassed)
            {
                SetPrimaryActionEnabled(enabled);
            }
            else
            {
                SetPrimaryActionEnabled(true);
            }
        }

        private void SetPrimaryActionEnabled(bool enabled)
        {
            var primary = _leftButton.Visible ? _leftButton : _middleButton;
            if (primary != null)
                primary.Enabled = enabled;
        }

        // ─── Validation ────────────────────────────────────────────────────

        private void InputBox_TextChanged(object? sender, EventArgs e)
        {
            if (InputValidator == null) { ClearValidation(); return; }
            string? error = InputValidator(_inputBox.Text);
            if (string.IsNullOrEmpty(error))
                ClearValidation();
            else
                ShowValidation(error);
        }

        private void ShowValidation(string message)
        {
            _validationLabel.Text    = message;
            _validationLabel.Visible = true;
            if (_currentTheme != null)
            {
                _validationLabel.ForeColor = _currentTheme.ErrorColor != Color.Empty
                    ? _currentTheme.ErrorColor : Color.FromArgb(220, 38, 38);
                _inputBox.BorderColor = _validationLabel.ForeColor;
            }
            _inputValidationPassed = false;
            UpdatePrimaryActionEnabledState();
            ReflowBodyContent();
        }

        private void ClearValidation()
        {
            _validationLabel.Text    = string.Empty;
            _validationLabel.Visible = false;
            if (_currentTheme != null)
                _inputBox.BorderColor = _currentTheme.TextBoxBorderColor != Color.Empty
                    ? _currentTheme.TextBoxBorderColor : _currentTheme.ButtonBorderColor;
            _inputValidationPassed = true;
            UpdatePrimaryActionEnabledState();
            ReflowBodyContent();
        }

        // ─── Auto-close ────────────────────────────────────────────────────

        /// <summary>
        /// Starts a countdown timer that auto-closes the dialog after
        /// <paramref name="timeoutMs"/> milliseconds. The primary button label
        /// is annotated with the remaining seconds, e.g. "OK (5)".
        /// </summary>
        public void StartAutoClose(int timeoutMs)
        {
            if (timeoutMs <= 0) return;

            _autoCloseRemainingMs = timeoutMs;

            // Capture starting label of the primary button.
            var primary = _leftButton.Visible ? _leftButton
                        : _middleButton.Visible ? _middleButton
                        : null;
            if (primary != null)
                _autoCloseBaseLabel = primary.Text;

            _autoCloseTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _autoCloseTimer.Tick += AutoCloseTimer_Tick;
            _autoCloseTimer.Start();

            // Update label immediately so the initial second is visible.
            UpdateAutoCloseLabel();
        }

        private void AutoCloseTimer_Tick(object? sender, EventArgs e)
        {
            _autoCloseRemainingMs -= 1000;

            if (_autoCloseRemainingMs <= 0)
            {
                StopAutoCloseTimer();
                // Treat as cancel (dismiss without primary action).
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
                return;
            }

            UpdateAutoCloseLabel();
        }

        private void UpdateAutoCloseLabel()
        {
            var primary = _leftButton.Visible ? _leftButton
                        : _middleButton.Visible ? _middleButton
                        : null;
            if (primary == null) return;

            int secs = (int)Math.Ceiling(_autoCloseRemainingMs / 1000.0);
            primary.Text = string.IsNullOrEmpty(_autoCloseBaseLabel)
                ? $"({secs})"
                : $"{_autoCloseBaseLabel} ({secs})";
        }

        private void StopAutoCloseTimer()
        {
            if (_autoCloseTimer == null) return;
            _autoCloseTimer.Stop();
            _autoCloseTimer.Tick -= AutoCloseTimer_Tick;
            _autoCloseTimer.Dispose();
            _autoCloseTimer = null;

            // Restore original button label.
            var primary = _leftButton.Visible ? _leftButton
                        : _middleButton.Visible ? _middleButton
                        : null;
            if (primary != null && !string.IsNullOrEmpty(_autoCloseBaseLabel))
                primary.Text = _autoCloseBaseLabel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                StopAutoCloseTimer();
            base.Dispose(disposing);
        }

        // ─── Theme ─────────────────────────────────────────────────────────

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;

            ShowCloseButton = AllowCaptionCloseButton;

            _headerPanel.Theme    = Theme;
            _bodyPanel.Theme      = Theme;
            _buttonPanel.Theme    = Theme;
            _dialogIcon.Theme     = Theme;
            _titleLabel.Theme     = Theme;
            _messageLabel.Theme   = Theme;
            _inputBox.Theme       = Theme;
            _comboBox.Theme       = Theme;
            _validationLabel.Theme = Theme;
            _detailsToggleButton.Theme = Theme;
            _detailsLabel.Theme = Theme;
            _confirmationHintLabel.Theme = Theme;
            _confirmationBox.Theme = Theme;
            _leftButton.Theme     = Theme;
            _middleButton.Theme   = Theme;
            _rightButton.Theme    = Theme;

            if (_currentTheme != null)
            {
                // Resolve preset-aware primary colour via centralised adapter.
                var presetForColor = PresetIntent != DialogPreset.None
                    ? PresetIntent
                    : (_dialogType == DialogType.Error ? DialogPreset.Danger : DialogPreset.None);

                Color primaryColor = DialogStyleAdapter.GetPresetPrimaryColor(presetForColor, _currentTheme);

                var primary = _leftButton.Visible ? _leftButton : _middleButton;
                if (primary != null)
                {
                    primary.BackColor      = primaryColor;
                    primary.ForeColor      = Color.White;
                    primary.HoverBackColor = System.Windows.Forms.ControlPaint.Light(primaryColor, 0.12f);
                }

                // Keep tertiary/cancel actions neutral for clearer hierarchy.
                if (_rightButton.Visible)
                {
                    _rightButton.BackColor = _currentTheme.ButtonBackColor;
                    _rightButton.ForeColor = _currentTheme.ButtonForeColor;
                }

                // Apply subtle header tint for semantic presets.
                var headerTint = DialogStyleAdapter.GetPresetHeaderTint(PresetIntent, _currentTheme);
                if (headerTint != Color.Empty)
                    _headerPanel.BackColor = headerTint;
            }

            Invalidate();
        }
    }
}
