namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepFileDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            _folderSelectionModeCheckBox = new CheckBox();
            _specialFoldersComboBox = new BeepComboBox();
            splitContainer2 = new SplitContainer();
            splitContainer4 = new SplitContainer();
            _searchBox = new BeepTextBox();
            _fileListView = new ListView();
            panel1 = new Panel();
            _okButton = new BeepButton();
            _cancelButton = new BeepButton();
            _fileNameTextBox = new BeepTextBox();
            splitContainer3 = new SplitContainer();
            _previewPane = new Panel();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            // 
            // splitContainer1
            // 
            splitContainer1.BackColor = Color.White;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = Color.White;
            splitContainer1.Panel1.Controls.Add(_folderSelectionModeCheckBox);
            splitContainer1.Panel1.Controls.Add(_specialFoldersComboBox);
            splitContainer1.Panel1.Paint += splitContainer1_Panel1_Paint;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = Color.White;
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(794, 444);
            splitContainer1.SplitterDistance = 182;
            splitContainer1.TabIndex = 0;
            // 
            // _folderSelectionModeCheckBox
            // 
            _folderSelectionModeCheckBox.AutoSize = true;
            _folderSelectionModeCheckBox.BackColor = Color.White;
            _folderSelectionModeCheckBox.Location = new Point(42, 59);
            _folderSelectionModeCheckBox.Name = "_folderSelectionModeCheckBox";
            _folderSelectionModeCheckBox.Size = new Size(83, 19);
            _folderSelectionModeCheckBox.TabIndex = 1;
            _folderSelectionModeCheckBox.Text = "checkBox1";
            _folderSelectionModeCheckBox.UseVisualStyleBackColor = false;
            // 
            // _specialFoldersComboBox
            // 
            _specialFoldersComboBox.ActiveBackColor = Color.FromArgb(200, 200, 200);
            _specialFoldersComboBox.AnimationDuration = 500;
            _specialFoldersComboBox.AnimationType = DisplayAnimationType.None;
            _specialFoldersComboBox.ApplyThemeToChilds = true;
            _specialFoldersComboBox.BackColor = Color.FromArgb(240, 240, 240);
            _specialFoldersComboBox.BlockID = null;
            _specialFoldersComboBox.BorderColor = Color.FromArgb(200, 200, 200);
            _specialFoldersComboBox.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _specialFoldersComboBox.BorderRadius = 1;
            _specialFoldersComboBox.BorderStyle = BorderStyle.FixedSingle;
            _specialFoldersComboBox.BorderThickness = 1;
            _specialFoldersComboBox.BottomoffsetForDrawingRect = 0;
            _specialFoldersComboBox.BoundProperty = null;
            _specialFoldersComboBox.CanBeFocused = true;
            _specialFoldersComboBox.CanBeHovered = false;
            _specialFoldersComboBox.CanBePressed = true;
            _specialFoldersComboBox.Category = Utilities.DbFieldCategory.Numeric;
            _specialFoldersComboBox.ComponentName = "_specialFoldersComboBox";
            _specialFoldersComboBox.DataContext = null;
            _specialFoldersComboBox.DataSourceProperty = null;
            _specialFoldersComboBox.DisabledBackColor = Color.Gray;
            _specialFoldersComboBox.DisabledForeColor = Color.Empty;
            _specialFoldersComboBox.DrawingRect = new Rectangle(1, 1, 174, 19);
            _specialFoldersComboBox.Easing = EasingType.Linear;
            _specialFoldersComboBox.FieldID = null;
            _specialFoldersComboBox.FocusBackColor = Color.Black;
            _specialFoldersComboBox.FocusBorderColor = Color.Gray;
            _specialFoldersComboBox.FocusForeColor = Color.Black;
            _specialFoldersComboBox.FocusIndicatorColor = Color.Blue;
            _specialFoldersComboBox.Form = null;
            _specialFoldersComboBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _specialFoldersComboBox.GradientEndColor = Color.White;
            _specialFoldersComboBox.GradientStartColor = Color.White;
            _specialFoldersComboBox.GuidID = "39979e8e-e83a-4860-8db0-2a212451522d";
            _specialFoldersComboBox.HoverBackColor = Color.FromArgb(210, 210, 210);
            _specialFoldersComboBox.HoverBorderColor = Color.Black;
            _specialFoldersComboBox.HoveredBackcolor = Color.Wheat;
            _specialFoldersComboBox.HoverForeColor = Color.Black;
            _specialFoldersComboBox.Id = -1;
            _specialFoldersComboBox.InactiveBackColor = Color.Gray;
            _specialFoldersComboBox.InactiveBorderColor = Color.Gray;
            _specialFoldersComboBox.InactiveForeColor = Color.Black;
            _specialFoldersComboBox.IsAcceptButton = false;
            _specialFoldersComboBox.IsBorderAffectedByTheme = true;
            _specialFoldersComboBox.IsCancelButton = false;
            _specialFoldersComboBox.IsChild = false;
            _specialFoldersComboBox.IsCustomeBorder = false;
            _specialFoldersComboBox.IsDefault = false;
            _specialFoldersComboBox.IsFocused = false;
            _specialFoldersComboBox.IsFramless = false;
            _specialFoldersComboBox.IsHovered = false;
            _specialFoldersComboBox.IsPressed = false;
            _specialFoldersComboBox.IsRounded = true;
            _specialFoldersComboBox.IsRoundedAffectedByTheme = true;
            _specialFoldersComboBox.IsShadowAffectedByTheme = true;
            _specialFoldersComboBox.LeftoffsetForDrawingRect = 0;
            _specialFoldersComboBox.LinkedProperty = null;
            _specialFoldersComboBox.Location = new Point(3, 18);
            _specialFoldersComboBox.Name = "_specialFoldersComboBox";
            _specialFoldersComboBox.OverrideFontSize = TypeStyleFontSize.None;
            _specialFoldersComboBox.ParentBackColor = Color.Empty;
            _specialFoldersComboBox.PressedBackColor = Color.FromArgb(200, 200, 200);
            _specialFoldersComboBox.PressedBorderColor = Color.Gray;
            _specialFoldersComboBox.PressedForeColor = Color.Black;
            _specialFoldersComboBox.RightoffsetForDrawingRect = 0;
            _specialFoldersComboBox.SavedGuidID = null;
            _specialFoldersComboBox.SavedID = null;
            _specialFoldersComboBox.SelectedIndex = -1;
            _specialFoldersComboBox.SelectedItem = null;
            _specialFoldersComboBox.ShadowColor = Color.Empty;
            _specialFoldersComboBox.ShadowOffset = 0;
            _specialFoldersComboBox.ShadowOpacity = 0.5F;
            _specialFoldersComboBox.ShowAllBorders = true;
            _specialFoldersComboBox.ShowBottomBorder = true;
            _specialFoldersComboBox.ShowFocusIndicator = false;
            _specialFoldersComboBox.ShowLeftBorder = true;
            _specialFoldersComboBox.ShowRightBorder = true;
            _specialFoldersComboBox.ShowShadow = false;
            _specialFoldersComboBox.ShowTopBorder = true;
            _specialFoldersComboBox.Size = new Size(176, 21);
            _specialFoldersComboBox.SlideFrom = SlideDirection.Left;
            _specialFoldersComboBox.StaticNotMoving = false;
            _specialFoldersComboBox.TabIndex = 0;
            _specialFoldersComboBox.Text = "beepComboBox1";
            _specialFoldersComboBox.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            _specialFoldersComboBox.ToolTipText = "";
            _specialFoldersComboBox.TopoffsetForDrawingRect = 0;
            _specialFoldersComboBox.UseGradientBackground = false;
            _specialFoldersComboBox.UseThemeFont = true;
            // 
            // splitContainer2
            // 
            splitContainer2.BackColor = Color.White;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.BackColor = Color.White;
            splitContainer2.Panel1.Controls.Add(splitContainer4);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.BackColor = Color.White;
            splitContainer2.Panel2.Controls.Add(splitContainer3);
            splitContainer2.Size = new Size(608, 444);
            splitContainer2.SplitterDistance = 454;
            splitContainer2.TabIndex = 0;
            // 
            // splitContainer4
            // 
            splitContainer4.BackColor = Color.White;
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.FixedPanel = FixedPanel.Panel1;
            splitContainer4.Location = new Point(0, 0);
            splitContainer4.Name = "splitContainer4";
            splitContainer4.Orientation = Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.BackColor = Color.White;
            splitContainer4.Panel1.Controls.Add(_searchBox);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.BackColor = Color.White;
            splitContainer4.Panel2.Controls.Add(_fileListView);
            splitContainer4.Panel2.Controls.Add(panel1);
            splitContainer4.Size = new Size(454, 444);
            splitContainer4.SplitterDistance = 52;
            splitContainer4.TabIndex = 0;
            // 
            // _searchBox
            // 
            _searchBox.AcceptsReturn = false;
            _searchBox.AcceptsTab = false;
            _searchBox.ActiveBackColor = Color.Gray;
            _searchBox.AnimationDuration = 500;
            _searchBox.AnimationType = DisplayAnimationType.None;
            _searchBox.ApplyThemeOnImage = false;
            _searchBox.ApplyThemeToChilds = true;
            _searchBox.AutoCompleteMode = AutoCompleteMode.None;
            _searchBox.AutoCompleteSource = AutoCompleteSource.None;
            _searchBox.BlockID = null;
            _searchBox.BorderColor = Color.Black;
            _searchBox.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _searchBox.BorderRadius = 1;
            _searchBox.BorderStyle = BorderStyle.FixedSingle;
            _searchBox.BorderThickness = 1;
            _searchBox.BottomoffsetForDrawingRect = 0;
            _searchBox.BoundProperty = "Text";
            _searchBox.CanBeFocused = true;
            _searchBox.CanBeHovered = false;
            _searchBox.CanBePressed = true;
            _searchBox.Category = Utilities.DbFieldCategory.String;
            _searchBox.ComponentName = "_searchBox";
            _searchBox.CustomMask = "";
            _searchBox.DataContext = null;
            _searchBox.DataSourceProperty = null;
            _searchBox.DateFormat = "MM/dd/yyyy HH:mm:ss";
            _searchBox.DateTimeFormat = "MM/dd/yyyy HH:mm:ss";
            _searchBox.DisabledBackColor = Color.Gray;
            _searchBox.DisabledForeColor = Color.Empty;
            _searchBox.DrawingRect = new Rectangle(3, 3, 316, 14);
            _searchBox.Easing = EasingType.Linear;
            _searchBox.FieldID = null;
            _searchBox.FocusBackColor = Color.Gray;
            _searchBox.FocusBorderColor = Color.Gray;
            _searchBox.FocusForeColor = Color.Black;
            _searchBox.FocusIndicatorColor = Color.Blue;
            _searchBox.Font = new Font("Segoe UI", 9F);
            _searchBox.Form = null;
            _searchBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _searchBox.GradientEndColor = Color.Gray;
            _searchBox.GradientStartColor = Color.Gray;
            _searchBox.GuidID = "4402ea50-19a3-47ae-b87b-724888a031f5";
            _searchBox.HideSelection = true;
            _searchBox.HoverBackColor = Color.Gray;
            _searchBox.HoverBorderColor = Color.Gray;
            _searchBox.HoveredBackcolor = Color.Wheat;
            _searchBox.HoverForeColor = Color.Black;
            _searchBox.Id = -1;
            _searchBox.ImageAlign = ContentAlignment.MiddleLeft;
            _searchBox.ImagePath = null;
            _searchBox.InactiveBackColor = Color.Gray;
            _searchBox.InactiveBorderColor = Color.Gray;
            _searchBox.InactiveForeColor = Color.Black;
            // 
            // 
            // 
            _searchBox.InnerTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _searchBox.InnerTextBox.BackColor = Color.White;
            _searchBox.InnerTextBox.BorderStyle = BorderStyle.None;
            _searchBox.InnerTextBox.Font = new Font("Segoe UI", 9F);
            _searchBox.InnerTextBox.ForeColor = Color.Black;
            _searchBox.InnerTextBox.Location = new Point(5, 2);
            _searchBox.InnerTextBox.Name = "";
            _searchBox.InnerTextBox.Size = new Size(312, 16);
            _searchBox.InnerTextBox.TabIndex = 0;
            _searchBox.InnerTextBox.Text = "beepTextBox1";
            _searchBox.IsAcceptButton = false;
            _searchBox.IsBorderAffectedByTheme = true;
            _searchBox.IsCancelButton = false;
            _searchBox.IsChild = true;
            _searchBox.IsCustomeBorder = false;
            _searchBox.IsDefault = false;
            _searchBox.IsFocused = false;
            _searchBox.IsFramless = false;
            _searchBox.IsHovered = false;
            _searchBox.IsPressed = false;
            _searchBox.IsRounded = true;
            _searchBox.IsRoundedAffectedByTheme = true;
            _searchBox.IsShadowAffectedByTheme = true;
            _searchBox.LeftoffsetForDrawingRect = 0;
            _searchBox.LinkedProperty = null;
            _searchBox.Location = new Point(66, 18);
            _searchBox.MaskFormat = Vis.Modules.TextBoxMaskFormat.None;
            _searchBox.Modified = false;
            _searchBox.Multiline = false;
            _searchBox.Name = "_searchBox";
            _searchBox.OnlyCharacters = false;
            _searchBox.OnlyDigits = false;
            _searchBox.OverrideFontSize = TypeStyleFontSize.None;
            _searchBox.Padding = new Padding(2);
            _searchBox.ParentBackColor = Color.Empty;
            _searchBox.PasswordChar = '\0';
            _searchBox.PlaceholderText = "";
            _searchBox.PressedBackColor = Color.Gray;
            _searchBox.PressedBorderColor = Color.Gray;
            _searchBox.PressedForeColor = Color.Black;
            _searchBox.ReadOnly = false;
            _searchBox.RightoffsetForDrawingRect = 0;
            _searchBox.SavedGuidID = null;
            _searchBox.SavedID = null;
            _searchBox.ScrollBars = ScrollBars.None;
            _searchBox.SelectionStart = 0;
            _searchBox.ShadowColor = Color.Black;
            _searchBox.ShadowOffset = 0;
            _searchBox.ShadowOpacity = 0.5F;
            _searchBox.ShowAllBorders = true;
            _searchBox.ShowBottomBorder = true;
            _searchBox.ShowFocusIndicator = false;
            _searchBox.ShowLeftBorder = true;
            _searchBox.ShowRightBorder = true;
            _searchBox.ShowScrollbars = false;
            _searchBox.ShowShadow = false;
            _searchBox.ShowTopBorder = true;
            _searchBox.ShowVerticalScrollBar = false;
            _searchBox.Size = new Size(322, 20);
            _searchBox.SlideFrom = SlideDirection.Left;
            _searchBox.StaticNotMoving = false;
            _searchBox.TabIndex = 0;
            _searchBox.Text = "beepTextBox1";
            _searchBox.TextAlignment = HorizontalAlignment.Left;
            _searchBox.TextFont = new Font("Segoe UI", 9F);
            _searchBox.TextImageRelation = TextImageRelation.ImageBeforeText;
            _searchBox.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            _searchBox.TimeFormat = "HH:mm:ss";
            _searchBox.ToolTipText = "";
            _searchBox.TopoffsetForDrawingRect = 0;
            _searchBox.UseGradientBackground = false;
            _searchBox.UseSystemPasswordChar = false;
            _searchBox.UseThemeFont = true;
            _searchBox.WordWrap = true;
            // 
            // _fileListView
            // 
            _fileListView.BackColor = Color.White;
            _fileListView.Dock = DockStyle.Fill;
            _fileListView.Location = new Point(0, 0);
            _fileListView.Name = "_fileListView";
            _fileListView.Size = new Size(454, 338);
            _fileListView.TabIndex = 0;
            _fileListView.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(_okButton);
            panel1.Controls.Add(_cancelButton);
            panel1.Controls.Add(_fileNameTextBox);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 338);
            panel1.Name = "panel1";
            panel1.Size = new Size(454, 50);
            panel1.TabIndex = 1;
            // 
            // _okButton
            // 
            _okButton.ActiveBackColor = Color.FromArgb(200, 200, 200);
            _okButton.AnimationDuration = 500;
            _okButton.AnimationType = DisplayAnimationType.None;
            _okButton.ApplyThemeOnImage = false;
            _okButton.ApplyThemeToChilds = true;
            _okButton.BackColor = Color.FromArgb(230, 230, 230);
            _okButton.BlockID = null;
            _okButton.BorderColor = Color.Black;
            _okButton.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _okButton.BorderRadius = 1;
            _okButton.BorderSize = 1;
            _okButton.BorderStyle = BorderStyle.FixedSingle;
            _okButton.BorderThickness = 1;
            _okButton.BottomoffsetForDrawingRect = 0;
            _okButton.BoundProperty = null;
            _okButton.CanBeFocused = true;
            _okButton.CanBeHovered = true;
            _okButton.CanBePressed = true;
            _okButton.Category = Utilities.DbFieldCategory.Boolean;
            _okButton.ComponentName = "beepButton1";
            _okButton.DataContext = null;
            _okButton.DataSourceProperty = null;
            _okButton.DisabledBackColor = Color.Gray;
            _okButton.DisabledForeColor = Color.Empty;
            _okButton.DrawingRect = new Rectangle(1, 1, 65, 31);
            _okButton.Easing = EasingType.Linear;
            _okButton.FieldID = null;
            _okButton.FocusBackColor = Color.Gray;
            _okButton.FocusBorderColor = Color.Gray;
            _okButton.FocusForeColor = Color.Black;
            _okButton.FocusIndicatorColor = Color.Blue;
            _okButton.Font = new Font("Segoe UI", 12F);
            _okButton.ForeColor = Color.Black;
            _okButton.Form = null;
            _okButton.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _okButton.GradientEndColor = Color.Gray;
            _okButton.GradientStartColor = Color.Gray;
            _okButton.GuidID = "f7c2764a-99ce-4529-8d6e-441392733956";
            _okButton.HideText = false;
            _okButton.HoverBackColor = Color.FromArgb(210, 210, 210);
            _okButton.HoverBorderColor = Color.Gray;
            _okButton.HoveredBackcolor = Color.Wheat;
            _okButton.HoverForeColor = Color.Black;
            _okButton.Id = -1;
            _okButton.Image = null;
            _okButton.ImageAlign = ContentAlignment.MiddleLeft;
            _okButton.ImageClicked = null;
            _okButton.ImagePath = null;
            _okButton.InactiveBackColor = Color.Gray;
            _okButton.InactiveBorderColor = Color.Gray;
            _okButton.InactiveForeColor = Color.Black;
            _okButton.IsAcceptButton = false;
            _okButton.IsBorderAffectedByTheme = true;
            _okButton.IsCancelButton = false;
            _okButton.IsChild = false;
            _okButton.IsCustomeBorder = false;
            _okButton.IsDefault = false;
            _okButton.IsFocused = false;
            _okButton.IsFramless = false;
            _okButton.IsHovered = false;
            _okButton.IsPopupOpen = false;
            _okButton.IsPressed = false;
            _okButton.IsRounded = true;
            _okButton.IsRoundedAffectedByTheme = true;
            _okButton.IsSelected = false;
            _okButton.IsSelectedAuto = true;
            _okButton.IsShadowAffectedByTheme = true;
            _okButton.IsSideMenuChild = false;
            _okButton.IsStillButton = false;
            _okButton.LeftoffsetForDrawingRect = 0;
            _okButton.LinkedProperty = null;
            _okButton.Location = new Point(380, 11);
            _okButton.Margin = new Padding(0);
            _okButton.MaxImageSize = new Size(32, 32);
            _okButton.Name = "_okButton";
            _okButton.OverrideFontSize = TypeStyleFontSize.None;
            _okButton.ParentBackColor = Color.Empty;
            _okButton.PopupMode = false;
            _okButton.PressedBackColor = Color.Gray;
            _okButton.PressedBorderColor = Color.Gray;
            _okButton.PressedForeColor = Color.Black;
            _okButton.RightoffsetForDrawingRect = 0;
            _okButton.SavedGuidID = null;
            _okButton.SavedID = null;
            _okButton.SelectedBorderColor = Color.Blue;
            _okButton.SelectedIndex = -1;
            _okButton.ShadowColor = Color.Black;
            _okButton.ShadowOffset = 0;
            _okButton.ShadowOpacity = 0.5F;
            _okButton.ShowAllBorders = true;
            _okButton.ShowBottomBorder = true;
            _okButton.ShowFocusIndicator = false;
            _okButton.ShowLeftBorder = true;
            _okButton.ShowRightBorder = true;
            _okButton.ShowShadow = false;
            _okButton.ShowTopBorder = true;
            _okButton.Size = new Size(67, 33);
            _okButton.SlideFrom = SlideDirection.Left;
            _okButton.StaticNotMoving = false;
            _okButton.TabIndex = 2;
            _okButton.Text = "OK";
            _okButton.TextAlign = ContentAlignment.MiddleCenter;
            _okButton.TextFont = new Font("Segoe UI", 9F);
            _okButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            _okButton.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            _okButton.ToolTipText = "";
            _okButton.TopoffsetForDrawingRect = 0;
            _okButton.UseGradientBackground = false;
            _okButton.UseScaledFont = false;
            _okButton.UseThemeFont = true;
            // 
            // _cancelButton
            // 
            _cancelButton.ActiveBackColor = Color.FromArgb(200, 200, 200);
            _cancelButton.AnimationDuration = 500;
            _cancelButton.AnimationType = DisplayAnimationType.None;
            _cancelButton.ApplyThemeOnImage = false;
            _cancelButton.ApplyThemeToChilds = true;
            _cancelButton.BackColor = Color.FromArgb(230, 230, 230);
            _cancelButton.BlockID = null;
            _cancelButton.BorderColor = Color.Black;
            _cancelButton.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _cancelButton.BorderRadius = 1;
            _cancelButton.BorderSize = 1;
            _cancelButton.BorderStyle = BorderStyle.FixedSingle;
            _cancelButton.BorderThickness = 1;
            _cancelButton.BottomoffsetForDrawingRect = 0;
            _cancelButton.BoundProperty = null;
            _cancelButton.CanBeFocused = true;
            _cancelButton.CanBeHovered = true;
            _cancelButton.CanBePressed = true;
            _cancelButton.Category = Utilities.DbFieldCategory.Boolean;
            _cancelButton.ComponentName = "_cancelButton";
            _cancelButton.DataContext = null;
            _cancelButton.DataSourceProperty = null;
            _cancelButton.DisabledBackColor = Color.Gray;
            _cancelButton.DisabledForeColor = Color.Empty;
            _cancelButton.DrawingRect = new Rectangle(1, 1, 70, 31);
            _cancelButton.Easing = EasingType.Linear;
            _cancelButton.FieldID = null;
            _cancelButton.FocusBackColor = Color.Gray;
            _cancelButton.FocusBorderColor = Color.Gray;
            _cancelButton.FocusForeColor = Color.Black;
            _cancelButton.FocusIndicatorColor = Color.Blue;
            _cancelButton.Font = new Font("Segoe UI", 12F);
            _cancelButton.ForeColor = Color.Black;
            _cancelButton.Form = null;
            _cancelButton.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _cancelButton.GradientEndColor = Color.Gray;
            _cancelButton.GradientStartColor = Color.Gray;
            _cancelButton.GuidID = "f7c2764a-99ce-4529-8d6e-441392733956";
            _cancelButton.HideText = false;
            _cancelButton.HoverBackColor = Color.FromArgb(210, 210, 210);
            _cancelButton.HoverBorderColor = Color.Gray;
            _cancelButton.HoveredBackcolor = Color.Wheat;
            _cancelButton.HoverForeColor = Color.Black;
            _cancelButton.Id = -1;
            _cancelButton.Image = null;
            _cancelButton.ImageAlign = ContentAlignment.MiddleLeft;
            _cancelButton.ImageClicked = null;
            _cancelButton.ImagePath = null;
            _cancelButton.InactiveBackColor = Color.Gray;
            _cancelButton.InactiveBorderColor = Color.Gray;
            _cancelButton.InactiveForeColor = Color.Black;
            _cancelButton.IsAcceptButton = false;
            _cancelButton.IsBorderAffectedByTheme = true;
            _cancelButton.IsCancelButton = false;
            _cancelButton.IsChild = false;
            _cancelButton.IsCustomeBorder = false;
            _cancelButton.IsDefault = false;
            _cancelButton.IsFocused = false;
            _cancelButton.IsFramless = false;
            _cancelButton.IsHovered = false;
            _cancelButton.IsPopupOpen = false;
            _cancelButton.IsPressed = false;
            _cancelButton.IsRounded = true;
            _cancelButton.IsRoundedAffectedByTheme = true;
            _cancelButton.IsSelected = false;
            _cancelButton.IsSelectedAuto = true;
            _cancelButton.IsShadowAffectedByTheme = true;
            _cancelButton.IsSideMenuChild = false;
            _cancelButton.IsStillButton = false;
            _cancelButton.LeftoffsetForDrawingRect = 0;
            _cancelButton.LinkedProperty = null;
            _cancelButton.Location = new Point(4, 11);
            _cancelButton.Margin = new Padding(0);
            _cancelButton.MaxImageSize = new Size(32, 32);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.OverrideFontSize = TypeStyleFontSize.None;
            _cancelButton.ParentBackColor = Color.Empty;
            _cancelButton.PopupMode = false;
            _cancelButton.PressedBackColor = Color.Gray;
            _cancelButton.PressedBorderColor = Color.Gray;
            _cancelButton.PressedForeColor = Color.Black;
            _cancelButton.RightoffsetForDrawingRect = 0;
            _cancelButton.SavedGuidID = null;
            _cancelButton.SavedID = null;
            _cancelButton.SelectedBorderColor = Color.Blue;
            _cancelButton.SelectedIndex = -1;
            _cancelButton.ShadowColor = Color.Black;
            _cancelButton.ShadowOffset = 0;
            _cancelButton.ShadowOpacity = 0.5F;
            _cancelButton.ShowAllBorders = true;
            _cancelButton.ShowBottomBorder = true;
            _cancelButton.ShowFocusIndicator = false;
            _cancelButton.ShowLeftBorder = true;
            _cancelButton.ShowRightBorder = true;
            _cancelButton.ShowShadow = false;
            _cancelButton.ShowTopBorder = true;
            _cancelButton.Size = new Size(72, 33);
            _cancelButton.SlideFrom = SlideDirection.Left;
            _cancelButton.StaticNotMoving = false;
            _cancelButton.TabIndex = 1;
            _cancelButton.Text = "Cancel";
            _cancelButton.TextAlign = ContentAlignment.MiddleCenter;
            _cancelButton.TextFont = new Font("Segoe UI", 9F);
            _cancelButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            _cancelButton.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            _cancelButton.ToolTipText = "";
            _cancelButton.TopoffsetForDrawingRect = 0;
            _cancelButton.UseGradientBackground = false;
            _cancelButton.UseScaledFont = false;
            _cancelButton.UseThemeFont = true;
            // 
            // _fileNameTextBox
            // 
            _fileNameTextBox.AcceptsReturn = false;
            _fileNameTextBox.AcceptsTab = false;
            _fileNameTextBox.ActiveBackColor = Color.Gray;
            _fileNameTextBox.AnimationDuration = 500;
            _fileNameTextBox.AnimationType = DisplayAnimationType.None;
            _fileNameTextBox.ApplyThemeOnImage = false;
            _fileNameTextBox.ApplyThemeToChilds = true;
            _fileNameTextBox.AutoCompleteMode = AutoCompleteMode.None;
            _fileNameTextBox.AutoCompleteSource = AutoCompleteSource.None;
            _fileNameTextBox.BlockID = null;
            _fileNameTextBox.BorderColor = Color.Black;
            _fileNameTextBox.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _fileNameTextBox.BorderRadius = 1;
            _fileNameTextBox.BorderStyle = BorderStyle.FixedSingle;
            _fileNameTextBox.BorderThickness = 1;
            _fileNameTextBox.BottomoffsetForDrawingRect = 0;
            _fileNameTextBox.BoundProperty = "Text";
            _fileNameTextBox.CanBeFocused = true;
            _fileNameTextBox.CanBeHovered = false;
            _fileNameTextBox.CanBePressed = true;
            _fileNameTextBox.Category = Utilities.DbFieldCategory.String;
            _fileNameTextBox.ComponentName = "beepTextBox1";
            _fileNameTextBox.CustomMask = "";
            _fileNameTextBox.DataContext = null;
            _fileNameTextBox.DataSourceProperty = null;
            _fileNameTextBox.DateFormat = "MM/dd/yyyy HH:mm:ss";
            _fileNameTextBox.DateTimeFormat = "MM/dd/yyyy HH:mm:ss";
            _fileNameTextBox.DisabledBackColor = Color.Gray;
            _fileNameTextBox.DisabledForeColor = Color.Empty;
            _fileNameTextBox.DrawingRect = new Rectangle(6, 6, 286, 13);
            _fileNameTextBox.Easing = EasingType.Linear;
            _fileNameTextBox.FieldID = null;
            _fileNameTextBox.FocusBackColor = Color.Gray;
            _fileNameTextBox.FocusBorderColor = Color.Gray;
            _fileNameTextBox.FocusForeColor = Color.Black;
            _fileNameTextBox.FocusIndicatorColor = Color.Blue;
            _fileNameTextBox.Font = new Font("Segoe UI", 9F);
            _fileNameTextBox.Form = null;
            _fileNameTextBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _fileNameTextBox.GradientEndColor = Color.Gray;
            _fileNameTextBox.GradientStartColor = Color.Gray;
            _fileNameTextBox.GuidID = "75e54f2d-2835-409a-8d35-e1736bc0398e";
            _fileNameTextBox.HideSelection = true;
            _fileNameTextBox.HoverBackColor = Color.Gray;
            _fileNameTextBox.HoverBorderColor = Color.Gray;
            _fileNameTextBox.HoveredBackcolor = Color.Wheat;
            _fileNameTextBox.HoverForeColor = Color.Black;
            _fileNameTextBox.Id = -1;
            _fileNameTextBox.ImageAlign = ContentAlignment.MiddleLeft;
            _fileNameTextBox.ImagePath = null;
            _fileNameTextBox.InactiveBackColor = Color.Gray;
            _fileNameTextBox.InactiveBorderColor = Color.Gray;
            _fileNameTextBox.InactiveForeColor = Color.Black;
            // 
            // 
            // 
            _fileNameTextBox.InnerTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _fileNameTextBox.InnerTextBox.BackColor = Color.White;
            _fileNameTextBox.InnerTextBox.BorderStyle = BorderStyle.None;
            _fileNameTextBox.InnerTextBox.Font = new Font("Segoe UI", 9F);
            _fileNameTextBox.InnerTextBox.ForeColor = Color.Black;
            _fileNameTextBox.InnerTextBox.Location = new Point(8, 5);
            _fileNameTextBox.InnerTextBox.Name = "";
            _fileNameTextBox.InnerTextBox.PlaceholderText = "File Name";
            _fileNameTextBox.InnerTextBox.Size = new Size(293, 16);
            _fileNameTextBox.InnerTextBox.TabIndex = 0;
            _fileNameTextBox.IsAcceptButton = false;
            _fileNameTextBox.IsBorderAffectedByTheme = true;
            _fileNameTextBox.IsCancelButton = false;
            _fileNameTextBox.IsChild = true;
            _fileNameTextBox.IsCustomeBorder = false;
            _fileNameTextBox.IsDefault = false;
            _fileNameTextBox.IsFocused = false;
            _fileNameTextBox.IsFramless = false;
            _fileNameTextBox.IsHovered = false;
            _fileNameTextBox.IsPressed = false;
            _fileNameTextBox.IsRounded = true;
            _fileNameTextBox.IsRoundedAffectedByTheme = true;
            _fileNameTextBox.IsShadowAffectedByTheme = true;
            _fileNameTextBox.LeftoffsetForDrawingRect = 0;
            _fileNameTextBox.LinkedProperty = null;
            _fileNameTextBox.Location = new Point(79, 11);
            _fileNameTextBox.MaskFormat = Vis.Modules.TextBoxMaskFormat.None;
            _fileNameTextBox.MaximumSize = new Size(0, 25);
            _fileNameTextBox.MinimumSize = new Size(50, 25);
            _fileNameTextBox.Modified = false;
            _fileNameTextBox.Multiline = false;
            _fileNameTextBox.Name = "_fileNameTextBox";
            _fileNameTextBox.OnlyCharacters = false;
            _fileNameTextBox.OnlyDigits = false;
            _fileNameTextBox.OverrideFontSize = TypeStyleFontSize.None;
            _fileNameTextBox.Padding = new Padding(5);
            _fileNameTextBox.ParentBackColor = Color.Empty;
            _fileNameTextBox.PasswordChar = '\0';
            _fileNameTextBox.PlaceholderText = "File Name";
            _fileNameTextBox.PressedBackColor = Color.Gray;
            _fileNameTextBox.PressedBorderColor = Color.Gray;
            _fileNameTextBox.PressedForeColor = Color.Black;
            _fileNameTextBox.ReadOnly = false;
            _fileNameTextBox.RightoffsetForDrawingRect = 0;
            _fileNameTextBox.SavedGuidID = null;
            _fileNameTextBox.SavedID = null;
            _fileNameTextBox.ScrollBars = ScrollBars.None;
            _fileNameTextBox.SelectionStart = 0;
            _fileNameTextBox.ShadowColor = Color.Black;
            _fileNameTextBox.ShadowOffset = 0;
            _fileNameTextBox.ShadowOpacity = 0.5F;
            _fileNameTextBox.ShowAllBorders = true;
            _fileNameTextBox.ShowBottomBorder = true;
            _fileNameTextBox.ShowFocusIndicator = false;
            _fileNameTextBox.ShowLeftBorder = true;
            _fileNameTextBox.ShowRightBorder = true;
            _fileNameTextBox.ShowScrollbars = false;
            _fileNameTextBox.ShowShadow = false;
            _fileNameTextBox.ShowTopBorder = true;
            _fileNameTextBox.ShowVerticalScrollBar = false;
            _fileNameTextBox.Size = new Size(298, 25);
            _fileNameTextBox.SlideFrom = SlideDirection.Left;
            _fileNameTextBox.StaticNotMoving = false;
            _fileNameTextBox.TabIndex = 0;
            _fileNameTextBox.TextAlignment = HorizontalAlignment.Left;
            _fileNameTextBox.TextFont = new Font("Segoe UI", 9F);
            _fileNameTextBox.TextImageRelation = TextImageRelation.ImageBeforeText;
            _fileNameTextBox.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            _fileNameTextBox.TimeFormat = "HH:mm:ss";
            _fileNameTextBox.ToolTipText = "";
            _fileNameTextBox.TopoffsetForDrawingRect = 0;
            _fileNameTextBox.UseGradientBackground = false;
            _fileNameTextBox.UseSystemPasswordChar = false;
            _fileNameTextBox.UseThemeFont = false;
            _fileNameTextBox.WordWrap = true;
            // 
            // splitContainer3
            // 
            splitContainer3.BackColor = Color.White;
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.FixedPanel = FixedPanel.Panel1;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.BackColor = Color.White;
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.BackColor = Color.White;
            splitContainer3.Panel2.Controls.Add(_previewPane);
            splitContainer3.Size = new Size(150, 444);
            splitContainer3.SplitterDistance = 41;
            splitContainer3.TabIndex = 0;
            // 
            // _previewPane
            // 
            _previewPane.BackColor = Color.White;
            _previewPane.Dock = DockStyle.Fill;
            _previewPane.Location = new Point(0, 0);
            _previewPane.Name = "_previewPane";
            _previewPane.Size = new Size(150, 399);
            _previewPane.TabIndex = 0;
            // 
            // BeepFileDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Name = "BeepFileDialog";
            Text = "BeepFileDialog";
            Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private BeepComboBox _specialFoldersComboBox;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer4;
        private ListView _fileListView;
        private SplitContainer splitContainer3;
        private CheckBox _folderSelectionModeCheckBox;
        private BeepTextBox _fileNameTextBox;
        private Panel panel1;
        private BeepButton _okButton;
        private BeepButton _cancelButton;
        private Panel _previewPane;
        private BeepTextBox _searchBox;
    }
}