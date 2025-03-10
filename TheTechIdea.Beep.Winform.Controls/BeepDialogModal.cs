
using System.ComponentModel;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum BeepDialogButtons
    {
        OkCancel,
        YesNo,
        AbortRetryIgnore,
        SaveDontSaveCancel,
        SaveAllDontSaveCancel,
        Close,
        Help,
        TryAgainContinue
    }
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
    public partial class BeepDialogModal : BeepiForm
    {
        private static readonly Dictionary<DialogType, string> dialogIcons = new()
{
    { DialogType.Information, "TheTechIdea.Beep.Winform.GFX.SVG.Information.svg" },
    { DialogType.Warning, "TheTechIdea.Beep.Winform.GFX.SVG.Warning.svg" },
    { DialogType.Error, "TheTechIdea.Beep.Winform.GFX.SVG.Error.svg" },
    { DialogType.Question, "TheTechIdea.Beep.Winform.GFX.SVG.Question.svg" },
    { DialogType.GetInputString, "TheTechIdea.Beep.Winform.GFX.SVG.Input.svg" },
    { DialogType.GetInputFromList, "TheTechIdea.Beep.Winform.GFX.SVG.Input.svg" },
    { DialogType.None, "TheTechIdea.Beep.Winform.GFX.SVG.Information.svg" }
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

 
        public string ReturnValue { get; set; }

        public  List<SimpleItem> Items { get; set; }

     
        private string Informationicon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.information.svg";
        private string Warningicon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.warning.svg";
        private string Erroricon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.error.svg";
        private string Questionicon = "TheTechIdea.Beep.Winform.GFX.SVG.question.svg";
        private string Inputicon = "TheTechIdea.Beep.Winform.GFX.SVG.input.svg";
        private string InputListicon = "TheTechIdea.Beep.Winform.GFX.SVG.input.svg";

        private string Okicon = "TheTechIdea.Beep.Winform.GFX.SVG.ok.svg";
        private string Cancelicon = "TheTechIdea.Beep.Winform.GFX.SVG.cancel.svg";
        private string Yesicon = "TheTechIdea.Beep.Winform.GFX.SVG.yes.svg";
        private string Noicon = "TheTechIdea.Beep.Winform.GFX.SVG.no.svg";
        private string Aborticon = "TheTechIdea.Beep.Winform.GFX.SVG.abort.svg";
        private string Retryicon = "TheTechIdea.Beep.Winform.GFX.SVG.retry.svg";
        private string Ignoreicon = "TheTechIdea.Beep.Winform.GFX.SVG.ignore.svg";
        private string Saveicon = "TheTechIdea.Beep.Winform.GFX.SVG.save.svg";
        private string SaveAllicon = "TheTechIdea.Beep.Winform.GFX.SVG.saveall.svg";
        private string DontSaveicon = "TheTechIdea.Beep.Winform.GFX.SVG.dontsave.svg";
        private string Closeicon = "TheTechIdea.Beep.Winform.GFX.SVG.closesquare.svg";
        private string Helpicon = "TheTechIdea.Beep.Winform.GFX.SVG.help.svg";
        private string TryAgainicon = "TheTechIdea.Beep.Winform.GFX.SVG.tryagain.svg";
        private string Continueicon = "TheTechIdea.Beep.Winform.GFX.SVG.continue.svg";

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
            if(dialogType== DialogType.GetInputString)
            {
                ReturnValue = InputTextBox.Text;
            }
            if(dialogType == DialogType.GetInputFromList)
            {
                ReturnValue = SelectFromListComboBox.SelectedItem.ToString();
            }
           
            DialogResult = DialogResult.Yes;
            this.Close();
        }
        private void RightButton_Click(object sender, EventArgs e)
        {
            ReturnValue = null;

            DialogResult = DialogResult.No;
            this.Close();
        }
        private void CenterButton_Click(object sender, EventArgs e)
        {
            ReturnValue = MiddleButtonCaption;
           
            DialogResult = DialogResult.OK;
            this.Close();
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
            LeftButtonCaption = OkCaption;
            LeftButton.Text = OkCaption;
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
            // Set Dialog Image in ImagePath property in DialogImage based on Dialogtype
            switch (DialogType)
            {
                case DialogType.Information:
                    DialogImage.ImagePath = Informationicon;
                    break;
                case DialogType.Warning:
                    DialogImage.ImagePath = Warningicon;
                    break;
                case DialogType.Error:
                    DialogImage.ImagePath = Erroricon;
                    break;
                case DialogType.Question:
                    DialogImage.ImagePath = Questionicon;
                    break;
                case DialogType.GetInputString:
                    DialogImage.ImagePath = Inputicon;
                    break;
                case DialogType.GetInputFromList:
                    DialogImage.ImagePath = InputListicon;
                    break;
                case DialogType.None:
                    DialogImage.ImagePath = Informationicon;
                    break;
                default:
                    DialogImage.ImagePath = Informationicon;
                    break;
            }
            DialogImage.Invalidate();
        }
        public void SetLeftCenterRightButtonIcons()
        {
            // Set Left , Right and Center buttons icons in ImagePath property in each buttons based on Dialogtype
            switch (DialogType)
            {
                case DialogType.Information:
                    LeftButton.ImagePath = Okicon;
                    RightButton.ImagePath = Cancelicon;
                    MiddleButton.ImagePath = Helpicon;
                    break;
                case DialogType.Warning:
                    LeftButton.ImagePath = Okicon;
                    RightButton.ImagePath = Cancelicon;
                    MiddleButton.ImagePath = Helpicon;
                    break;
                case DialogType.Error:
                    LeftButton.ImagePath = Okicon;
                    RightButton.ImagePath = Cancelicon;
                    MiddleButton.ImagePath = Helpicon;
                    break;
                case DialogType.Question:
                    switch (dialogButtons)
                    {
                        case BeepDialogButtons.OkCancel:
                            LeftButton.ImagePath = Okicon;
                            RightButton.ImagePath = Cancelicon;
                            MiddleButton.ImagePath = Helpicon;
                            break;
                        case BeepDialogButtons.YesNo:
                            LeftButton.ImagePath = Yesicon;
                            RightButton.ImagePath = Noicon;
                            MiddleButton.ImagePath = Helpicon;
                            break;
                        case BeepDialogButtons.AbortRetryIgnore:
                            LeftButton.ImagePath = Aborticon;
                            RightButton.ImagePath = Retryicon;
                            MiddleButton.ImagePath = Ignoreicon;
                            break;
                        case BeepDialogButtons.SaveDontSaveCancel:
                            LeftButton.ImagePath = Saveicon;
                            RightButton.ImagePath = DontSaveicon;
                            MiddleButton.ImagePath = Cancelicon;
                            break;
                        case BeepDialogButtons.SaveAllDontSaveCancel:
                            LeftButton.ImagePath = SaveAllicon;
                            RightButton.ImagePath = DontSaveicon;
                            MiddleButton.ImagePath = Cancelicon;
                            break;
                        
                        case BeepDialogButtons.TryAgainContinue:
                            LeftButton.ImagePath = TryAgainicon;
                            RightButton.ImagePath = Continueicon;
                            MiddleButton.ImagePath = Helpicon;
                            break;
                        default:
                            LeftButton.ImagePath = Okicon;
                            RightButton.ImagePath = Cancelicon;
                            MiddleButton.ImagePath = Helpicon;
                            break;
                    }
                   
                    break;
                case DialogType.GetInputString:
                    LeftButton.ImagePath = Okicon;
                    RightButton.ImagePath = Cancelicon;
                    MiddleButton.ImagePath = Helpicon;
                    break;
                case DialogType.GetInputFromList:
                    LeftButton.ImagePath = Okicon;
                    RightButton.ImagePath = Cancelicon;
                    MiddleButton.ImagePath = Helpicon;
                    break;
                case DialogType.None:
                    LeftButton.ImagePath = Okicon;
                    RightButton.ImagePath = Cancelicon;
                    MiddleButton.ImagePath = Helpicon;
                    break;
                default:
                    LeftButton.ImagePath = Okicon;
                    RightButton.ImagePath = Cancelicon;
                    MiddleButton.ImagePath = Helpicon;
                    break;
            }


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
            foreach (var item in Items)
            {
                SelectFromListComboBox.ListItems.Add(item);
            }
            SelectFromListComboBox.SelectedItemChanged += SelectFromListComboBox_SelectedItemChanged;
            SelectFromListComboBox.SelectedIndex = 0;
        }

        private void SelectFromListComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
           DialogResult = DialogResult.OK;
            ReturnValue = SelectFromListComboBox.SelectedItem.ToString();
           
        }
        #endregion "Setting Visible Input"
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            panel1.BackColor = _currentTheme.BackColor;
            panel2.BackColor = _currentTheme.BackColor;
            panel3.BackColor = _currentTheme.BackColor;
            LeftButton.Theme = Theme;
            RightButton.Theme = Theme;
            MiddleButton.Theme = Theme;
            DialogImage.Theme = Theme;
            CaptionTextBox.Theme = Theme;
            TitleLabel.Theme = Theme;
            InputTextBox.Theme = Theme;
            SelectFromListComboBox.Theme = Theme;
          
            
        }

    }
}
