using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Images;
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

        // ─── Button controls ───────────────────────────────────────────────
        private readonly BeepButton _leftButton;
        private readonly BeepButton _middleButton;
        private readonly BeepButton _rightButton;

        // ─── State ─────────────────────────────────────────────────────────
        private DialogType           _dialogType   = DialogType.None;
        private BeepDialogButtons    _dialogButtons = BeepDialogButtons.OkCancel;

        // ─── Public API ────────────────────────────────────────────────────
        public string ReturnValue { get; private set; } = string.Empty;
        public SimpleItem? ReturnItem { get; private set; }
        public List<SimpleItem>? Items { get; set; }

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
                Stretch      = true
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
                Padding      = new Padding(12, 8, 12, 4)
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

            _bodyPanel.Controls.Add(_messageLabel);
            _bodyPanel.Controls.Add(_inputBox);
            _bodyPanel.Controls.Add(_comboBox);
            _bodyPanel.Controls.Add(_validationLabel);

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

            // Wire validation
            _inputBox.TextChanged += InputBox_TextChanged;

            // ── Add panels to form (Dock stacking: Fill first, then Bottom, then Top) ─
            Controls.Add(_bodyPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_headerPanel);

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
                    DialogResult = DialogResult.OK;
                    break;
                case DialogType.GetInputFromList:
                    ReturnValue  = _comboBox.SelectedItem?.Text ?? string.Empty;
                    ReturnItem   = _comboBox.SelectedItem;
                    DialogResult = DialogResult.OK;
                    break;
                default:
                    ReturnValue  = _leftButton.Text;
                    DialogResult = _dialogButtons switch
                    {
                        BeepDialogButtons.YesNo              => DialogResult.Yes,
                        BeepDialogButtons.AbortRetryIgnore   => DialogResult.Abort,
                        BeepDialogButtons.SaveDontSaveCancel => DialogResult.Yes,
                        BeepDialogButtons.SaveAllDontSaveCancel => DialogResult.Yes,
                        BeepDialogButtons.TryAgainContinue   => DialogResult.Retry,
                        _                                    => DialogResult.OK
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
                BeepDialogButtons.AbortRetryIgnore      => DialogResult.Ignore,
                BeepDialogButtons.SaveDontSaveCancel    => DialogResult.No,
                BeepDialogButtons.SaveAllDontSaveCancel => DialogResult.No,
                BeepDialogButtons.Close                 => DialogResult.Cancel,
                BeepDialogButtons.Help                  => DialogResult.OK,
                _                                       => DialogResult.OK
            };
            Close();
        }

        private void RightButton_Click(object? sender, EventArgs e)
        {
            ReturnValue  = null;
            DialogResult = _dialogButtons switch
            {
                BeepDialogButtons.YesNo              => DialogResult.No,
                BeepDialogButtons.AbortRetryIgnore   => DialogResult.Retry,
                BeepDialogButtons.SaveDontSaveCancel => DialogResult.No,
                BeepDialogButtons.SaveAllDontSaveCancel => DialogResult.No,
                BeepDialogButtons.TryAgainContinue   => DialogResult.Continue,
                _                                    => DialogResult.Cancel
            };
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (_middleButton.Visible) { _middleButton.PerformClick(); return true; }
                if (_leftButton.Visible)   { _leftButton.PerformClick();   return true; }
            }
            if (keyData == Keys.Escape)
            {
                if (_rightButton.Visible) { _rightButton.PerformClick(); return true; }
                DialogResult = DialogResult.Cancel;
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ─── Configuration ─────────────────────────────────────────────────

        private void ConfigureForDialogType()
        {
            SetIcon();
            SetButtonVisibilityAndCaptions();
            SetInputVisibility();
        }

        private void SetIcon()
        {
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
            // Hide all first
            _leftButton.Visible   = false;
            _middleButton.Visible = false;
            _rightButton.Visible  = false;

            switch (_dialogType)
            {
                case DialogType.Information:
                case DialogType.Warning:
                case DialogType.Error:
                    _middleButton.Text    = "OK";
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
                    ShowLeftRight("OK", Svgs.Check, "Cancel", Svgs.Cancel);
                    break;
                case BeepDialogButtons.YesNo:
                    ShowLeftRight("Yes", Svgs.Yes, "No", Svgs.No);
                    break;
                case BeepDialogButtons.AbortRetryIgnore:
                    ShowAll("Abort", Svgs.Abort, "Ignore", Svgs.InfoIgnore, "Retry", Svgs.TryAgain);
                    break;
                case BeepDialogButtons.SaveDontSaveCancel:
                    ShowAll("Save", Svgs.Save, "Don't Save", Svgs.DontSave, "Cancel", Svgs.Cancel);
                    break;
                case BeepDialogButtons.SaveAllDontSaveCancel:
                    ShowAll("Save All", Svgs.SaveAll, "Don't Save", Svgs.DontSave, "Cancel", Svgs.Cancel);
                    break;
                case BeepDialogButtons.Close:
                    _middleButton.Text      = "Close";
                    _middleButton.ImagePath = Svgs.Close;
                    _middleButton.Visible   = true;
                    break;
                case BeepDialogButtons.Help:
                    _middleButton.Text      = "Help";
                    _middleButton.ImagePath = Svgs.InfoHelp;
                    _middleButton.Visible   = true;
                    break;
                case BeepDialogButtons.TryAgainContinue:
                    ShowLeftRight("Try Again", Svgs.TryAgain, "Continue", Svgs.Continue);
                    break;
                default:
                    ShowLeftRight("OK", Svgs.Check, "Cancel", Svgs.Cancel);
                    break;
            }
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

            if (_dialogType == DialogType.GetInputString)
            {
                _inputBox.Text    = string.Empty;
                _inputBox.Visible = true;
                _inputBox.Focus();
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
            _leftButton.Enabled = false;
        }

        private void ClearValidation()
        {
            _validationLabel.Text    = string.Empty;
            _validationLabel.Visible = false;
            if (_currentTheme != null)
                _inputBox.BorderColor = _currentTheme.TextBoxBorderColor != Color.Empty
                    ? _currentTheme.TextBoxBorderColor : _currentTheme.ButtonBorderColor;
            _leftButton.Enabled = true;
        }

        // ─── Theme ─────────────────────────────────────────────────────────

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;

            _headerPanel.Theme    = Theme;
            _bodyPanel.Theme      = Theme;
            _buttonPanel.Theme    = Theme;
            _dialogIcon.Theme     = Theme;
            _titleLabel.Theme     = Theme;
            _messageLabel.Theme   = Theme;
            _inputBox.Theme       = Theme;
            _comboBox.Theme       = Theme;
            _validationLabel.Theme = Theme;
            _leftButton.Theme     = Theme;
            _middleButton.Theme   = Theme;
            _rightButton.Theme    = Theme;

            if (_currentTheme != null)
            {
                // Semantic button colours
                var accent = _currentTheme.AccentColor != Color.Empty
                    ? _currentTheme.AccentColor : Color.FromArgb(59, 130, 246);
                var errClr = _currentTheme.ErrorColor != Color.Empty
                    ? _currentTheme.ErrorColor : Color.FromArgb(220, 38, 38);

                if (_dialogType == DialogType.Error)
                {
                    var primary = _leftButton.Visible ? _leftButton : _middleButton;
                    primary.BackColor      = errClr;
                    primary.ForeColor      = Color.White;
                    primary.HoverBackColor = System.Windows.Forms.ControlPaint.Light(errClr, 0.1f);
                }
                else
                {
                    var primary = _leftButton.Visible ? _leftButton : _middleButton;
                    primary.BackColor      = accent;
                    primary.ForeColor      = Color.White;
                    primary.HoverBackColor = System.Windows.Forms.ControlPaint.Light(accent, 0.15f);
                }
            }

            Invalidate();
        }
    }
}
