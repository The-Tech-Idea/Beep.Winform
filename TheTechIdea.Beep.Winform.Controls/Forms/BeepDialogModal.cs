
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;



namespace TheTechIdea.Beep.Winform.Controls
{
  
    public enum  DialogType
    {
        Information,
        Warning,
        Error,
        Question,
        GetInputString,
        GetInputFromList,
        None

    }
    public partial class BeepDialogModal : BeepiFormPro
    {
        private static readonly Dictionary<DialogType, string> dialogIcons = new()
{
    { DialogType.Information, Svgs.Information },
    { DialogType.Warning, Svgs.InfoWarning },
    { DialogType.Error, Svgs.Error },
    { DialogType.Question, Svgs.Question },
    { DialogType.GetInputString, Svgs.Input },
    { DialogType.GetInputFromList, Svgs.Input },
    { DialogType.None, Svgs.Information }
};

        private static readonly Dictionary<DialogType, (bool textBox, bool comboBox)> inputVisibility = new()
{
    { DialogType.GetInputString, (true, false) },
    { DialogType.GetInputFromList, (false, true) },
    { DialogType.Information, (false, false) },
    { DialogType.Warning, (false, false) },
    { DialogType.Error, (false, false) },
    { DialogType.Question, (false, false) },
    { DialogType.None, (false, false) }
};
        private static readonly Dictionary<BeepDialogButtons, (string left, string middle, string right)> buttonCaptions = new()
{
    { BeepDialogButtons.OkCancel, ("OK", "", "Cancel") },
    { BeepDialogButtons.YesNo, ("Yes", "", "No") },
    { BeepDialogButtons.AbortRetryIgnore, ("Abort", "Ignore", "Retry") },
    { BeepDialogButtons.SaveDontSaveCancel, ("Save", "Don't Save", "Cancel") },
    { BeepDialogButtons.SaveAllDontSaveCancel, ("Save All", "Don't Save", "Cancel") },
    { BeepDialogButtons.Close, ("", "Close", "") },
    { BeepDialogButtons.Help, ("", "Help", "") },
    { BeepDialogButtons.TryAgainContinue, ("Try Again", "", "Continue") }
};

        [Browsable(true)]
        public string Message
        {
            get => CaptionTextBox.Text;
            set => CaptionTextBox.Text = value;
        }
        [Browsable(true)]
        public string Title
        {
            get => TitleLabel.Text;
            set => TitleLabel.Text = value;
        }
        [Browsable(true)]
        public string LeftButtonCaption
        {
            get => LeftButton.Text;
            set => LeftButton.Text = value;
        }
        [Browsable(true)]
        public string RightButtonCaption
        {
            get => RightButton.Text;
            set => RightButton.Text = value;
        }
        [Browsable(true)]
        public string MiddleButtonCaption
        {
            get => MiddleButton.Text;
            set => MiddleButton.Text = value;
        }
        private string okCaption = "OK";
        private string cancelCaption = "Cancel";
        private string yesCaption = "Yes";
        private string noCaption = "No";
        private string abortCaption = "Abort";
        private string retryCaption = "Retry";
        private string ignoreCaption = "Ignore";
        private string saveCaption = "Save";
        private string saveAllCaption = "Save All";
        private string dontSaveCaption = "Don't Save";
        private string closeCaption = "Close";
        private string helpCaption = "Help";
        private string tryAgainCaption = "Try Again";
        private string continueCaption = "Continue";
        public string OkCaption { get => okCaption; set => okCaption = value; }
        public string CancelCaption { get => cancelCaption; set => cancelCaption = value; }
        public string YesCaption { get => yesCaption; set => yesCaption = value; }
        public string NoCaption { get => noCaption; set => noCaption = value; }
        public string AbortCaption { get => abortCaption; set => abortCaption = value; }
        public string RetryCaption { get => retryCaption; set => retryCaption = value; }
        public string IgnoreCaption { get => ignoreCaption; set => ignoreCaption = value; }
        public string SaveCaption { get => saveCaption; set => saveCaption = value; }
        public string SaveAllCaption { get => saveAllCaption; set => saveAllCaption = value; }
        public string DontSaveCaption { get => dontSaveCaption; set => dontSaveCaption = value; }
        public string CloseCaption { get => closeCaption; set => closeCaption = value; }
        public string HelpCaption { get => helpCaption; set => helpCaption = value; }
        public string TryAgainCaption { get => tryAgainCaption; set => tryAgainCaption = value; }
        public string ContinueCaption { get => continueCaption; set => continueCaption = value; }
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }


        public string ReturnValue { get; set; }
        public SimpleItem ReturnItem { get; private set; }
        public  List<SimpleItem> Items { get; set; }
         
        private DialogType dialogType = DialogType.None;
        public DialogType DialogType
        {
            get { return dialogType; }
            set
            {
                dialogType = value;
                SetDialogType();
            }

        }
        private BeepDialogButtons dialogButtons = BeepDialogButtons.OkCancel;
        [Browsable(true)]
        public BeepDialogButtons DialogButtons
        {
            get { return dialogButtons; }
            set
            {
                dialogButtons = value;
                SetDialogType();
            }
        }
        public BeepDialogModal()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Ensure it's a modal dialog
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
           
            

        }

        private void SetDialogType()
        {
            SetVisiblityofButtonsBasedondialogType();
            SetCaptionsBasedonDialogType();
            SetButtonsDialogResultBasedonDialgType();
            SetInputTypeBasedonDialogType();
            SetLeftCenterRightButtonIcons();
            LeftButton.Invalidate();
            RightButton.Invalidate();
            MiddleButton.Invalidate();

            SetDialogImage();
            // Accessibility and default button mapping
            SetDefaultButtonAndAccessibility();
        }

        private void SetDefaultButtonAndAccessibility()
        {
            // Accessibility name
            LeftButton.AccessibleName = "LeftActionButton";
            MiddleButton.AccessibleName = "MiddleActionButton";
            RightButton.AccessibleName = "RightActionButton";

            // Default/Cancel mapping based on visible buttons - use custom handling instead of AcceptButton binding
            // Focus the default button for keyboard users
            if (MiddleButton.Visible)
            {
                MiddleButton.Focus();
            }
            else if (LeftButton.Visible)
            {
                LeftButton.Focus();
            }
            else if (RightButton.Visible)
            {
                RightButton.Focus();
            }
        }
        #region "Setting Visible Buttons"
        public void SetVisiblityofButtonsBasedondialogType()
        {
            switch (DialogType)
            {
                case DialogType.Information:
                    SetOkCancel();
                    break;
                case DialogType.Warning:
                    SetOkCancel();
                    break;
                case DialogType.Error:
                    SetOkCancel();
                    break;
                case DialogType.Question:
                    SetYesNo();
                    break;
                case DialogType.GetInputString:
                    SetOkCancel();
                    break;
                case DialogType.GetInputFromList:
                    SetOkCancel();
                    break;
                case DialogType.None:
                    SetOkCancel();
                    break;
                default:
                    SetOkCancel();
                    break;
            }
        }
        public void SetAllButton()
        {
            LeftButton.Visible = true;
            RightButton.Visible = true;
            MiddleButton.Visible = true;
        }
        public void SetLeftRightButton()
        {
            LeftButton.Visible = true;
            RightButton.Visible = true;
            MiddleButton.Visible = false;
        }
        public void SetLeftMiddleRightButton()
        {
            LeftButton.Visible = true;
            RightButton.Visible = true;
            MiddleButton.Visible = true;
        }
        public void SetMiddleButton()
        {
            LeftButton.Visible = false;
            RightButton.Visible = false;
            MiddleButton.Visible = true;
        }
        public void SetLeftButton()
        {
            LeftButton.Visible = true;
            RightButton.Visible = false;
            MiddleButton.Visible = false;
        }
        public void SetRightButton()
        {
            LeftButton.Visible = false;
            RightButton.Visible = true;
            MiddleButton.Visible = false;
        }
        private void LeftButton_Click(object sender, EventArgs e)
        {
            // Handle special dialog types that need input capture
            switch (dialogType)
            {
                case DialogType.GetInputString:
                    ReturnValue = InputTextBox.Text;
                    DialogResult = DialogResult.OK;
                    break;
                case DialogType.GetInputFromList:
                    if (SelectFromListComboBox.SelectedItem != null)
                    {
                        ReturnValue = SelectFromListComboBox.SelectedItem.Text;
                        ReturnItem = SelectFromListComboBox.SelectedItem;
                    }
                    DialogResult = DialogResult.OK;
                    break;
                case DialogType.Information:
                case DialogType.Warning:
                case DialogType.Error:
                    ReturnValue = LeftButtonCaption;
                    DialogResult = DialogResult.OK;
                    break;
                case DialogType.Question:
                    // Handle based on button types
                    switch (dialogButtons)
                    {
                        case BeepDialogButtons.OkCancel:
                            ReturnValue = OkCaption;
                            DialogResult = DialogResult.OK;
                            break;
                        case BeepDialogButtons.YesNo:
                            ReturnValue = YesCaption;
                            DialogResult = DialogResult.Yes;
                            break;
                        case BeepDialogButtons.AbortRetryIgnore:
                            ReturnValue = AbortCaption;
                            DialogResult = DialogResult.Abort;
                            break;
                        case BeepDialogButtons.SaveDontSaveCancel:
                            ReturnValue = SaveCaption;
                            DialogResult = DialogResult.Yes; // Save = Yes
                            break;
                        case BeepDialogButtons.SaveAllDontSaveCancel:
                            ReturnValue = SaveAllCaption;
                            DialogResult = DialogResult.Yes; // SaveAll = Yes
                            break;
                        case BeepDialogButtons.TryAgainContinue:
                            ReturnValue = TryAgainCaption;
                            DialogResult = DialogResult.Retry; // TryAgain = Retry
                            break;
                        default:
                            ReturnValue = LeftButtonCaption;
                            DialogResult = DialogResult.OK;
                            break;
                    }
                    break;
                case DialogType.None:
                    ReturnValue = LeftButtonCaption;
                    DialogResult = DialogResult.OK;
                    break;
                default:
                    ReturnValue = LeftButtonCaption;
                    DialogResult = DialogResult.OK;
                    break;
            }

            this.Close();
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            ReturnValue = null;
            // Return dialog result based on dialog type just like SetCaptionsBasedonDialogType
            switch (DialogType)
            {
                case DialogType.Information:
                    DialogResult = DialogResult.Cancel;
                    break;
                case DialogType.Warning:
                    DialogResult = DialogResult.Cancel;
                    break;
                case DialogType.Error:
                    DialogResult = DialogResult.Cancel;
                    break;
                case DialogType.Question:
                    switch (dialogButtons)
                    {
                        case BeepDialogButtons.OkCancel:
                            DialogResult = DialogResult.Cancel;
                            break;
                        case BeepDialogButtons.YesNo:
                            DialogResult = DialogResult.No;
                            break;
                        case BeepDialogButtons.AbortRetryIgnore:
                            DialogResult = DialogResult.Retry;
                            break;
                        case BeepDialogButtons.SaveDontSaveCancel:
                            DialogResult = DialogResult.No; // Don't Save
                            break;
                        case BeepDialogButtons.SaveAllDontSaveCancel:
                            DialogResult = DialogResult.No; // Don't Save
                            break;
                        case BeepDialogButtons.TryAgainContinue:
                            DialogResult = DialogResult.Continue;
                            break;
                        default:
                            DialogResult = DialogResult.Cancel;
                            break;
                    }
                    break;
                case DialogType.GetInputString:
                    DialogResult = DialogResult.Cancel;
                    break;
                case DialogType.GetInputFromList:
                    DialogResult = DialogResult.Cancel;
                    break;
                case DialogType.None:
                    DialogResult = DialogResult.Cancel;
                    break;
                default:
                    DialogResult = DialogResult.Cancel;
                    break;
            }

            this.Close();
        }

        private void CenterButton_Click(object sender, EventArgs e)
        {
            ReturnValue = MiddleButtonCaption;
           
            DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            // Provide common keyboard behavior: Enter = Accept, Esc = Cancel/Close
            if (keyData == Keys.Enter)
            {
                if (MiddleButton != null && MiddleButton.Visible) { MiddleButton.PerformClick(); return true; }
                if (LeftButton != null && LeftButton.Visible) { LeftButton.PerformClick(); return true; }
            }
            if (keyData == Keys.Escape)
            {
                if (RightButton != null && RightButton.Visible) { RightButton.PerformClick(); return true; }
                if (LeftButton != null && LeftButton.Visible) { LeftButton.PerformClick(); return true; }
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion "Setting Visible Buttons"
        #region "Setting Button Captions"
        public void SetCaptionsBasedonDialogType()
        {
            switch (DialogType)
            {
                case DialogType.Information:
                    SetOk();
                    break;
                case DialogType.Warning:
                    SetOk();
                    break;
                case DialogType.Error:
                    SetOk();
                    break;
                case DialogType.Question:
                    SetYesNo();
                    break;
                case DialogType.GetInputString:
                    SetOkCancel();
                    break;
                case DialogType.GetInputFromList:
                    SetOkCancel();
                    break;
                case DialogType.None:
                    SetOkCancel();
                    break;
                default:
                    SetOkCancel();
                    break;
            }

        }
        public void SetOk()
        {
            MiddleButtonCaption = OkCaption;
            MiddleButton.Text = OkCaption;
            SetMiddleButton();
        }
        public void SetOkCancel()
        {
            LeftButtonCaption = okCaption;
            LeftButton.Text = okCaption;
            RightButtonCaption = CancelCaption;
            RightButton.Text = CancelCaption;
            SetLeftRightButton();
        }
        public void SetYesNo()
        {
            LeftButtonCaption = YesCaption;
            LeftButton.Text = YesCaption;
            RightButtonCaption = NoCaption;
            RightButton.Text = NoCaption;
            SetLeftRightButton();
        }
        public void SetAbortRetryIgnore()
        {
            LeftButtonCaption = AbortCaption;
            LeftButton.Text = AbortCaption;
            RightButtonCaption = RetryCaption;
            RightButton.Text = RetryCaption;
            MiddleButtonCaption = IgnoreCaption;
            MiddleButton.Text = IgnoreCaption;
            SetLeftMiddleRightButton();
        }
        public void SetSaveDontSaveCancel()
        {
            LeftButtonCaption = SaveCaption;
            LeftButton.Text = SaveCaption;
            RightButtonCaption = DontSaveCaption;
            RightButton.Text = DontSaveCaption;
            MiddleButtonCaption = CancelCaption;
            MiddleButton.Text = CancelCaption;
            SetLeftMiddleRightButton();
        }
        public void SetSaveAllDontSaveCancel()
        {
            LeftButtonCaption = SaveAllCaption;
            LeftButton.Text = SaveAllCaption;
            RightButtonCaption = DontSaveCaption;
            RightButton.Text = DontSaveCaption;

            MiddleButtonCaption = CancelCaption;
            MiddleButton.Text = CancelCaption;
            SetLeftMiddleRightButton();
        }
        public void SetClose()
        {
            MiddleButtonCaption = CloseCaption;
            MiddleButton.Text = CloseCaption;
            SetMiddleButton();
        }
        public void SetHelp()
        {
            MiddleButtonCaption = HelpCaption;
            MiddleButton.Text = HelpCaption;
            SetMiddleButton();
        }
        public void SetTryAgainContinue()
        {
            LeftButtonCaption = TryAgainCaption;
            LeftButton.Text = TryAgainCaption;
            RightButtonCaption = ContinueCaption;
            RightButton.Text = ContinueCaption;
            SetLeftRightButton();
        }
        #endregion "Setting Button Captions"
        #region "Setting Icons"
        public void SetDialogImage()
        {
            DialogImage.ImagePath = ResolveDialogIconPath(DialogType);
            DialogImage.Invalidate();
        }
        public void SetLeftCenterRightButtonIcons()
        {
            LeftButton.ImagePath = ResolveButtonIconPath(LeftButton.Text);
            RightButton.ImagePath = ResolveButtonIconPath(RightButton.Text);
            MiddleButton.ImagePath = ResolveButtonIconPath(MiddleButton.Text);
        }
        #endregion "Setting Icons"
        #region "Setting Buttons Click Events"
        public void SetButtonsDialogResultBasedonDialgType()
        {
            switch (DialogType)
            {
                case DialogType.Information:
                    MiddleButton.Click += LeftButton_Click;
                    break;
                case DialogType.Warning:
                    MiddleButton.Click += LeftButton_Click;
                    break;
                case DialogType.Error:
                    MiddleButton.Click += LeftButton_Click;
                    break;
                case DialogType.Question:
                    switch (dialogButtons)
                    {
                        case BeepDialogButtons.OkCancel:
                            LeftButton.Click += LeftButton_Click;
                            RightButton.Click += RightButton_Click;
                            break;
                        case BeepDialogButtons.YesNo:
                            LeftButton.Click += LeftButton_Click;
                            RightButton.Click += RightButton_Click;
                            break;
                        case BeepDialogButtons.AbortRetryIgnore:
                            LeftButton.Click += LeftButton_Click;
                            RightButton.Click += RightButton_Click;
                            MiddleButton.Click += CenterButton_Click;
                            break;
                        case BeepDialogButtons.SaveDontSaveCancel:
                            LeftButton.Click += LeftButton_Click;
                            RightButton.Click += RightButton_Click;
                            MiddleButton.Click += CenterButton_Click;
                            break;
                        case BeepDialogButtons.SaveAllDontSaveCancel:
                            LeftButton.Click += LeftButton_Click;
                            RightButton.Click += RightButton_Click;
                            MiddleButton.Click += CenterButton_Click;
                            break;
                        case BeepDialogButtons.TryAgainContinue:
                            LeftButton.Click += LeftButton_Click;
                            RightButton.Click += RightButton_Click;
                            break;
                        default:
                            LeftButton.Click += LeftButton_Click;
                            RightButton.Click += RightButton_Click;
                            break;
                    }
                    break;
                case DialogType.GetInputString:
                    LeftButton.Click += LeftButton_Click;
                    RightButton.Click += RightButton_Click;
                    break;
                case DialogType.GetInputFromList:
                    LeftButton.Click += LeftButton_Click;
                    RightButton.Click += RightButton_Click;
                    break;
                case DialogType.None:
                    LeftButton.Click += LeftButton_Click;
                    RightButton.Click += RightButton_Click;
                    break;
                default:
                    LeftButton.Click += LeftButton_Click;
                    RightButton.Click += RightButton_Click;
                    break;
            }

        }
        #endregion "Setting Buttons Click Events"
        #region "Setting Visible Input"
        public void SetInputTypeBasedonDialogType()
        {
            switch (DialogType)
            {
                case DialogType.Information:
                    SetInputVisible();
                    break;
                case DialogType.Warning:
                    SetInputVisible();
                    break;
                case DialogType.Error:
                    SetInputVisible();
                    break;
                case DialogType.Question:
                    SetInputVisible();
                    break;
                case DialogType.GetInputString:
                    SetInputVisible();
                    break;
                case DialogType.GetInputFromList:
                    SetInputListVisible();
                    break;
                case DialogType.None:
                    SetInputVisible();
                    break;
                default:
                    SetInputVisible();
                    break;
            }
        }
        public void SetInputVisible()
        {
            InputTextBox.Visible = true;
            InputTextBox.Text = "";
            InputTextBox.Focus();
            SelectFromListComboBox.Visible = false;
        }
        public void SetInputListVisible()
        {
            InputTextBox.Visible = false;
            SelectFromListComboBox.Visible = true;
            SelectFromListComboBox.ListItems.Clear(); // Clear existing items first

            if (Items != null && Items.Count > 0)
            {
                foreach (var item in Items)
                {
                    SelectFromListComboBox.ListItems.Add(item);
                }

                // Set initial selection and return value
                SelectFromListComboBox.SelectedIndex = 0;
                ReturnValue = SelectFromListComboBox.SelectedItem?.Text;
                ReturnItem = SelectFromListComboBox.SelectedItem;
            }

            // Attach event handler after initialization
            SelectFromListComboBox.SelectedItemChanged += SelectFromListComboBox_SelectedItemChanged;
        }


        private void SelectFromListComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
        
            if (e.SelectedItem != null)
            {
                ReturnValue = e.SelectedItem.Text;
                ReturnItem = e.SelectedItem;
            }

        }
        #endregion "Setting Visible Input"
        public override void ApplyTheme()
        {
            // base.ApplyTheme();
            if (panel1 == null) return;
           panel1.Theme = Theme;
            panel1.GradientStartColor = _currentTheme.GradientStartColor;
            panel1.GradientEndColor= _currentTheme.GradientEndColor;
            panel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            panel3.Theme= Theme;
            panel3.GradientStartColor = _currentTheme.GradientStartColor;
            panel3.GradientEndColor= _currentTheme.GradientEndColor;
            panel3.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            LeftButton.Theme = Theme;
            RightButton.Theme = Theme;
            MiddleButton.Theme = Theme;
            DialogImage.Theme = Theme;
            CaptionTextBox.Theme = Theme;
            TitleLabel.Theme = Theme;
            InputTextBox.Theme = Theme;
            SelectFromListComboBox.Theme = Theme;
            DialogImage.ApplyThemeOnImage = true;

            var titleFont = ResolveThemeFont(_currentTheme?.TitleStyle, 12f, FontStyle.Bold);
            var bodyFont = ResolveThemeFont(_currentTheme?.BodyStyle, 10f, FontStyle.Regular);
            var buttonFont = ResolveThemeFont(_currentTheme?.DialogOkButtonFont ?? _currentTheme?.ButtonStyle, 10f, FontStyle.Regular);

            TitleLabel.Font = titleFont;
            CaptionTextBox.Font = bodyFont;
            InputTextBox.Font = bodyFont;
            SelectFromListComboBox.Font = bodyFont;
            LeftButton.Font = buttonFont;
            RightButton.Font = buttonFont;
            MiddleButton.Font = buttonFont;
            this.Font = bodyFont;
        }

        private static string ResolveDialogIconPath(DialogType type)
        {
            if (dialogIcons.TryGetValue(type, out var path))
                return path;

            return Svgs.Information;
        }

        private static string ResolveButtonIconPath(string? caption)
        {
            if (string.IsNullOrWhiteSpace(caption))
                return Svgs.InfoHelp;

            return caption.Trim().ToLowerInvariant() switch
            {
                "ok" => Svgs.Check,
                "cancel" => Svgs.Cancel,
                "yes" => Svgs.Yes,
                "no" => Svgs.No,
                "abort" => Svgs.Abort,
                "retry" => Svgs.TryAgain,
                "ignore" => Svgs.InfoIgnore,
                "save" => Svgs.Save,
                "save all" => Svgs.SaveAll,
                "don't save" => Svgs.DontSave,
                "close" => Svgs.Close,
                "help" => Svgs.InfoHelp,
                "try again" => Svgs.TryAgain,
                "continue" => Svgs.Continue,
                _ => Svgs.InfoHelp
            };
        }

        private static Font ResolveThemeFont(TypographyStyle? style, float fallbackSize, FontStyle fallbackStyle)
        {
            if (style != null)
                return BeepThemesManager.ToFont(style);

            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, fallbackSize, fallbackStyle);
        }

    }
}
