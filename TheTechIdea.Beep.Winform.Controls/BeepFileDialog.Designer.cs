using TheTechIdea.Beep.Vis.Modules;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepFileDialog));
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
            beepuiManager1.BeepiForm = this;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
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
            splitContainer1.Size = new Size(857, 508);
            splitContainer1.SplitterDistance = 196;
            splitContainer1.TabIndex = 0;
            // 
            // _folderSelectionModeCheckBox
            // 
            _folderSelectionModeCheckBox.AutoSize = true;
            _folderSelectionModeCheckBox.BackColor = Color.White;
            _folderSelectionModeCheckBox.Location = new Point(42, 59);
            _folderSelectionModeCheckBox.Name = "_folderSelectionModeCheckBox";
            _folderSelectionModeCheckBox.Size = new Size(82, 19);
            _folderSelectionModeCheckBox.TabIndex = 1;
            _folderSelectionModeCheckBox.Text = "checkBox1";
            _folderSelectionModeCheckBox.UseVisualStyleBackColor = false;
            // 
            // _specialFoldersComboBox
            // 
          
            _specialFoldersComboBox.AnimationDuration = 500;
            _specialFoldersComboBox.AnimationType = DisplayAnimationType.None;
            _specialFoldersComboBox.ApplyThemeToChilds = true;
            _specialFoldersComboBox.BackColor = Color.FromArgb(245, 245, 245);
            _specialFoldersComboBox.BadgeBackColor = Color.Red;
            _specialFoldersComboBox.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _specialFoldersComboBox.BadgeForeColor = Color.White;
            _specialFoldersComboBox.BadgeShape = BadgeShape.Circle;
            _specialFoldersComboBox.BadgeText = "";
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
            _specialFoldersComboBox.DrawingRect = new Rectangle(0, 0, 176, 21);
            _specialFoldersComboBox.Easing = EasingType.Linear;
            _specialFoldersComboBox.FieldID = null;
            _specialFoldersComboBox.FocusBackColor = Color.White;
            _specialFoldersComboBox.FocusBorderColor = Color.Gray;
            _specialFoldersComboBox.FocusForeColor = Color.Black;
            _specialFoldersComboBox.FocusIndicatorColor = Color.Blue;
            _specialFoldersComboBox.Form = null;
            _specialFoldersComboBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _specialFoldersComboBox.GradientEndColor = Color.FromArgb(230, 230, 230);
            _specialFoldersComboBox.GradientStartColor = Color.White;
            _specialFoldersComboBox.GuidID = "39979e8e-e83a-4860-8db0-2a212451522d";
            _specialFoldersComboBox.HoverBackColor = Color.FromArgb(230, 230, 230);
            _specialFoldersComboBox.HoverBorderColor = Color.FromArgb(0, 120, 215);
            _specialFoldersComboBox.HoveredBackcolor = Color.Wheat;
            _specialFoldersComboBox.HoverForeColor = Color.Black;
            _specialFoldersComboBox.Id = -1;
           
            _specialFoldersComboBox.Info = (Models.SimpleItem)resources.GetObject("_specialFoldersComboBox.Info");
            _specialFoldersComboBox.IsAcceptButton = false;
            _specialFoldersComboBox.IsBorderAffectedByTheme = true;
            _specialFoldersComboBox.IsCancelButton = false;
            _specialFoldersComboBox.IsChild = false;
            _specialFoldersComboBox.IsCustomeBorder = false;
            _specialFoldersComboBox.IsDefault = false;
            _specialFoldersComboBox.IsDeleted = false;
            _specialFoldersComboBox.IsDirty = false;
            _specialFoldersComboBox.IsEditable = false;
            _specialFoldersComboBox.IsFocused = false;
            _specialFoldersComboBox.IsFrameless = false;
            _specialFoldersComboBox.IsHovered = false;
            _specialFoldersComboBox.IsNew = false;
            _specialFoldersComboBox.IsPopupOpen = false;
            _specialFoldersComboBox.IsPressed = false;
            _specialFoldersComboBox.IsReadOnly = false;
            _specialFoldersComboBox.IsRequired = false;
            _specialFoldersComboBox.IsRounded = true;
            _specialFoldersComboBox.IsRoundedAffectedByTheme = true;
            _specialFoldersComboBox.IsSelected = false;
            _specialFoldersComboBox.IsShadowAffectedByTheme = true;
            _specialFoldersComboBox.IsVisible = false;
            _specialFoldersComboBox.Items = (List<object>)resources.GetObject("_specialFoldersComboBox.Items");
            _specialFoldersComboBox.LeftoffsetForDrawingRect = 0;
            _specialFoldersComboBox.LinkedProperty = null;
            _specialFoldersComboBox.Location = new Point(3, 18);
            _specialFoldersComboBox.Name = "_specialFoldersComboBox";
            _specialFoldersComboBox.OverrideFontSize = TypeStyleFontSize.None;
            _specialFoldersComboBox.ParentBackColor = Color.Empty;
            _specialFoldersComboBox.ParentControl = null;
            _specialFoldersComboBox.PressedBackColor = Color.FromArgb(0, 120, 215);
            _specialFoldersComboBox.PressedBorderColor = Color.Gray;
            _specialFoldersComboBox.PressedForeColor = Color.Black;
            _specialFoldersComboBox.RightoffsetForDrawingRect = 0;
            _specialFoldersComboBox.SavedGuidID = null;
            _specialFoldersComboBox.SavedID = null;
            _specialFoldersComboBox.SelectedIndex = -1;
            _specialFoldersComboBox.SelectedItem = null;
            _specialFoldersComboBox.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            _specialFoldersComboBox.ShadowOffset = 0;
            _specialFoldersComboBox.ShadowOpacity = 0.5F;
            _specialFoldersComboBox.ShowAllBorders = false;
            _specialFoldersComboBox.ShowBottomBorder = false;
            _specialFoldersComboBox.ShowFocusIndicator = false;
            _specialFoldersComboBox.ShowLeftBorder = false;
            _specialFoldersComboBox.ShowRightBorder = false;
            _specialFoldersComboBox.ShowShadow = false;
            _specialFoldersComboBox.ShowTopBorder = false;
            _specialFoldersComboBox.Size = new Size(176, 21);
            _specialFoldersComboBox.SlideFrom = SlideDirection.Left;
            _specialFoldersComboBox.StaticNotMoving = false;
            _specialFoldersComboBox.TabIndex = 0;
            _specialFoldersComboBox.TempBackColor = Color.Empty;
            _specialFoldersComboBox.Text = "beepComboBox1";
            _specialFoldersComboBox.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
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
            splitContainer2.Size = new Size(657, 508);
            splitContainer2.SplitterDistance = 490;
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
            splitContainer4.Size = new Size(490, 508);
            splitContainer4.SplitterDistance = 52;
            splitContainer4.TabIndex = 0;
            // 
            // _searchBox
            // 
            _searchBox.AcceptsReturn = false;
            _searchBox.AcceptsTab = false;
          
            _searchBox.AnimationDuration = 500;
            _searchBox.AnimationType = DisplayAnimationType.None;
            _searchBox.ApplyThemeOnImage = false;
            _searchBox.ApplyThemeToChilds = true;
            _searchBox.AutoCompleteMode = AutoCompleteMode.None;
            _searchBox.AutoCompleteSource = AutoCompleteSource.None;
            _searchBox.BackColor = Color.White;
            _searchBox.BadgeBackColor = Color.Red;
            _searchBox.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _searchBox.BadgeForeColor = Color.White;
            _searchBox.BadgeShape = BadgeShape.Circle;
            _searchBox.BadgeText = "";
            _searchBox.BlockID = null;
            _searchBox.BorderColor = Color.FromArgb(200, 200, 200);
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
            _searchBox.DrawingRect = new Rectangle(2, 2, 318, 17);
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
         
            _searchBox.Info = (Models.SimpleItem)resources.GetObject("_searchBox.Info");
            _searchBox.IsAcceptButton = false;
            _searchBox.IsBorderAffectedByTheme = true;
            _searchBox.IsCancelButton = false;
            _searchBox.IsChild = true;
            _searchBox.IsCustomeBorder = false;
            _searchBox.IsDefault = false;
            _searchBox.IsDeleted = false;
            _searchBox.IsDirty = false;
            _searchBox.IsEditable = false;
            _searchBox.IsFocused = false;
            _searchBox.IsFrameless = false;
            _searchBox.IsHovered = false;
            _searchBox.IsNew = false;
            _searchBox.IsPressed = false;
            _searchBox.IsReadOnly = false;
            _searchBox.IsRequired = false;
            _searchBox.IsRounded = true;
            _searchBox.IsRoundedAffectedByTheme = true;
            _searchBox.IsSelected = false;
            _searchBox.IsShadowAffectedByTheme = true;
            _searchBox.IsVisible = false;
            _searchBox.Items = (List<object>)resources.GetObject("_searchBox.Items");
            _searchBox.LeftoffsetForDrawingRect = 0;
            _searchBox.LinkedProperty = null;
            _searchBox.Location = new Point(66, 18);
            _searchBox.MaskFormat = Vis.Modules.TextBoxMaskFormat.None;
            _searchBox.MaxImageSize = new Size(16, 16);
            _searchBox.Modified = false;
            _searchBox.Multiline = false;
            _searchBox.Name = "_searchBox";
            _searchBox.OnlyCharacters = false;
            _searchBox.OnlyDigits = false;
            _searchBox.OverrideFontSize = TypeStyleFontSize.None;
            _searchBox.Padding = new Padding(2);
            _searchBox.ParentBackColor = Color.White;
            _searchBox.ParentControl = null;
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
            _searchBox.ShowAllBorders = false;
            _searchBox.ShowBottomBorder = false;
            _searchBox.ShowFocusIndicator = false;
            _searchBox.ShowLeftBorder = false;
            _searchBox.ShowRightBorder = false;
            _searchBox.ShowScrollbars = false;
            _searchBox.ShowShadow = false;
            _searchBox.ShowTopBorder = false;
            _searchBox.ShowVerticalScrollBar = false;
            _searchBox.Size = new Size(322, 21);
            _searchBox.SlideFrom = SlideDirection.Left;
            _searchBox.StaticNotMoving = false;
            _searchBox.TabIndex = 0;
            _searchBox.TempBackColor = Color.Empty;
            _searchBox.Text = "beepTextBox1";
            _searchBox.TextAlignment = HorizontalAlignment.Left;
            _searchBox.TextFont = new Font("Segoe UI", 9F);
            _searchBox.TextImageRelation = TextImageRelation.ImageBeforeText;
            _searchBox.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
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
            _fileListView.Size = new Size(490, 402);
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
            panel1.Location = new Point(0, 402);
            panel1.Name = "panel1";
            panel1.Size = new Size(490, 50);
            panel1.TabIndex = 1;
            // 
            // _okButton
            // 
         
            _okButton.AnimationDuration = 500;
            _okButton.AnimationType = DisplayAnimationType.None;
            _okButton.ApplyThemeOnImage = false;
            _okButton.ApplyThemeToChilds = true;
            _okButton.BackColor = Color.FromArgb(240, 240, 240);
            _okButton.BadgeBackColor = Color.Red;
            _okButton.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _okButton.BadgeForeColor = Color.White;
            _okButton.BadgeShape = BadgeShape.Circle;
            _okButton.BadgeText = "";
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
            _okButton.DrawingRect = new Rectangle(0, 0, 67, 33);
            _okButton.Easing = EasingType.Linear;
            _okButton.FieldID = null;
            _okButton.FocusBackColor = Color.Gray;
            _okButton.FocusBorderColor = Color.Gray;
            _okButton.FocusForeColor = Color.Black;
            _okButton.FocusIndicatorColor = Color.Blue;
            _okButton.Font = new Font("Segoe UI", 9F);
            _okButton.ForeColor = Color.FromArgb(0, 0, 0);
            _okButton.Form = null;
            _okButton.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _okButton.GradientEndColor = Color.Gray;
            _okButton.GradientStartColor = Color.Gray;
            _okButton.GuidID = "f7c2764a-99ce-4529-8d6e-441392733956";
            _okButton.HideText = false;
            _okButton.HoverBackColor = Color.FromArgb(230, 230, 230);
            _okButton.HoverBorderColor = Color.Gray;
            _okButton.HoveredBackcolor = Color.Wheat;
            _okButton.HoverForeColor = Color.FromArgb(0, 0, 0);
            _okButton.Id = -1;
            _okButton.Image = null;
            _okButton.ImageAlign = ContentAlignment.MiddleLeft;
            _okButton.ImageClicked = null;
            _okButton.ImageEmbededin = ImageEmbededin.Button;
            _okButton.ImagePath = null;
           
            _okButton.Info = (Models.SimpleItem)resources.GetObject("_okButton.Info");
            _okButton.IsAcceptButton = false;
            _okButton.IsBorderAffectedByTheme = true;
            _okButton.IsCancelButton = false;
            _okButton.IsChild = false;
            _okButton.IsCustomeBorder = false;
            _okButton.IsDefault = false;
            _okButton.IsDeleted = false;
            _okButton.IsDirty = false;
            _okButton.IsEditable = false;
            _okButton.IsFocused = false;
            _okButton.IsFrameless = false;
            _okButton.IsHovered = false;
            _okButton.IsNew = false;
            _okButton.IsPopupOpen = false;
            _okButton.IsPressed = false;
            _okButton.IsReadOnly = false;
            _okButton.IsRequired = false;
            _okButton.IsRounded = true;
            _okButton.IsRoundedAffectedByTheme = true;
            _okButton.IsSelected = false;
           
            _okButton.IsShadowAffectedByTheme = true;
            _okButton.IsSideMenuChild = false;
            _okButton.IsStillButton = false;
            _okButton.IsVisible = false;
            _okButton.Items = (List<object>)resources.GetObject("_okButton.Items");
            _okButton.LeftoffsetForDrawingRect = 0;
            _okButton.LinkedProperty = null;
            _okButton.Location = new Point(380, 11);
            _okButton.Margin = new Padding(0);
            _okButton.MaxImageSize = new Size(32, 32);
            _okButton.Name = "_okButton";
            _okButton.OverrideFontSize = TypeStyleFontSize.None;
            _okButton.ParentBackColor = Color.Empty;
            _okButton.ParentControl = null;
            _okButton.PopPosition = Vis.Modules.BeepPopupFormPosition.Bottom;
            _okButton.PopupMode = false;
            _okButton.PressedBackColor = Color.Gray;
            _okButton.PressedBorderColor = Color.Gray;
            _okButton.PressedForeColor = Color.Black;
            _okButton.RightoffsetForDrawingRect = 0;
            _okButton.SavedGuidID = null;
            _okButton.SavedID = null;
           
            _okButton.SelectedIndex = 0;
            _okButton.SelectedItem = null;
            _okButton.ShadowColor = Color.Black;
            _okButton.ShadowOffset = 0;
            _okButton.ShadowOpacity = 0.5F;
            _okButton.ShowAllBorders = false;
            _okButton.ShowBottomBorder = false;
            _okButton.ShowFocusIndicator = false;
            _okButton.ShowLeftBorder = false;
            _okButton.ShowRightBorder = false;
            _okButton.ShowShadow = false;
            _okButton.ShowTopBorder = false;
            _okButton.Size = new Size(67, 33);
            _okButton.SlideFrom = SlideDirection.Left;
            _okButton.StaticNotMoving = false;
            _okButton.TabIndex = 2;
            _okButton.TempBackColor = Color.Empty;
            _okButton.Text = "OK";
            _okButton.TextAlign = ContentAlignment.MiddleCenter;
            _okButton.TextFont = new Font("Segoe UI", 9F);
            _okButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            _okButton.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            _okButton.ToolTipText = "";
            _okButton.TopoffsetForDrawingRect = 0;
            _okButton.UseGradientBackground = false;
            _okButton.UseScaledFont = false;
            _okButton.UseThemeFont = true;
            // 
            // _cancelButton
            // 
         
            _cancelButton.AnimationDuration = 500;
            _cancelButton.AnimationType = DisplayAnimationType.None;
            _cancelButton.ApplyThemeOnImage = false;
            _cancelButton.ApplyThemeToChilds = true;
            _cancelButton.BackColor = Color.FromArgb(240, 240, 240);
            _cancelButton.BadgeBackColor = Color.Red;
            _cancelButton.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _cancelButton.BadgeForeColor = Color.White;
            _cancelButton.BadgeShape = BadgeShape.Circle;
            _cancelButton.BadgeText = "";
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
            _cancelButton.DrawingRect = new Rectangle(0, 0, 72, 33);
            _cancelButton.Easing = EasingType.Linear;
            _cancelButton.FieldID = null;
            _cancelButton.FocusBackColor = Color.Gray;
            _cancelButton.FocusBorderColor = Color.Gray;
            _cancelButton.FocusForeColor = Color.Black;
            _cancelButton.FocusIndicatorColor = Color.Blue;
            _cancelButton.Font = new Font("Segoe UI", 9F);
            _cancelButton.ForeColor = Color.FromArgb(0, 0, 0);
            _cancelButton.Form = null;
            _cancelButton.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _cancelButton.GradientEndColor = Color.Gray;
            _cancelButton.GradientStartColor = Color.Gray;
            _cancelButton.GuidID = "f7c2764a-99ce-4529-8d6e-441392733956";
            _cancelButton.HideText = false;
            _cancelButton.HoverBackColor = Color.FromArgb(230, 230, 230);
            _cancelButton.HoverBorderColor = Color.Gray;
            _cancelButton.HoveredBackcolor = Color.Wheat;
            _cancelButton.HoverForeColor = Color.FromArgb(0, 0, 0);
            _cancelButton.Id = -1;
            _cancelButton.Image = null;
            _cancelButton.ImageAlign = ContentAlignment.MiddleLeft;
            _cancelButton.ImageClicked = null;
            _cancelButton.ImageEmbededin = ImageEmbededin.Button;
            _cancelButton.ImagePath = null;
          
            _cancelButton.Info = (Models.SimpleItem)resources.GetObject("_cancelButton.Info");
            _cancelButton.IsAcceptButton = false;
            _cancelButton.IsBorderAffectedByTheme = true;
            _cancelButton.IsCancelButton = false;
            _cancelButton.IsChild = false;
            _cancelButton.IsCustomeBorder = false;
            _cancelButton.IsDefault = false;
            _cancelButton.IsDeleted = false;
            _cancelButton.IsDirty = false;
            _cancelButton.IsEditable = false;
            _cancelButton.IsFocused = false;
            _cancelButton.IsFrameless = false;
            _cancelButton.IsHovered = false;
            _cancelButton.IsNew = false;
            _cancelButton.IsPopupOpen = false;
            _cancelButton.IsPressed = false;
            _cancelButton.IsReadOnly = false;
            _cancelButton.IsRequired = false;
            _cancelButton.IsRounded = true;
            _cancelButton.IsRoundedAffectedByTheme = true;
            _cancelButton.IsSelected = false;
           
            _cancelButton.IsShadowAffectedByTheme = true;
            _cancelButton.IsSideMenuChild = false;
            _cancelButton.IsStillButton = false;
            _cancelButton.IsVisible = false;
            _cancelButton.Items = (List<object>)resources.GetObject("_cancelButton.Items");
            _cancelButton.LeftoffsetForDrawingRect = 0;
            _cancelButton.LinkedProperty = null;
            _cancelButton.Location = new Point(4, 11);
            _cancelButton.Margin = new Padding(0);
            _cancelButton.MaxImageSize = new Size(32, 32);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.OverrideFontSize = TypeStyleFontSize.None;
            _cancelButton.ParentBackColor = Color.Empty;
            _cancelButton.ParentControl = null;
            _cancelButton.PopPosition = Vis.Modules.BeepPopupFormPosition.Bottom;
            _cancelButton.PopupMode = false;
            _cancelButton.PressedBackColor = Color.Gray;
            _cancelButton.PressedBorderColor = Color.Gray;
            _cancelButton.PressedForeColor = Color.Black;
            _cancelButton.RightoffsetForDrawingRect = 0;
            _cancelButton.SavedGuidID = null;
            _cancelButton.SavedID = null;
           
            _cancelButton.SelectedIndex = 0;
            _cancelButton.SelectedItem = null;
            _cancelButton.ShadowColor = Color.Black;
            _cancelButton.ShadowOffset = 0;
            _cancelButton.ShadowOpacity = 0.5F;
            _cancelButton.ShowAllBorders = false;
            _cancelButton.ShowBottomBorder = false;
            _cancelButton.ShowFocusIndicator = false;
            _cancelButton.ShowLeftBorder = false;
            _cancelButton.ShowRightBorder = false;
            _cancelButton.ShowShadow = false;
            _cancelButton.ShowTopBorder = false;
            _cancelButton.Size = new Size(72, 33);
            _cancelButton.SlideFrom = SlideDirection.Left;
            _cancelButton.StaticNotMoving = false;
            _cancelButton.TabIndex = 1;
            _cancelButton.TempBackColor = Color.Empty;
            _cancelButton.Text = "Cancel";
            _cancelButton.TextAlign = ContentAlignment.MiddleCenter;
            _cancelButton.TextFont = new Font("Segoe UI", 9F);
            _cancelButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            _cancelButton.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
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
          
            _fileNameTextBox.AnimationDuration = 500;
            _fileNameTextBox.AnimationType = DisplayAnimationType.None;
            _fileNameTextBox.ApplyThemeOnImage = false;
            _fileNameTextBox.ApplyThemeToChilds = true;
            _fileNameTextBox.AutoCompleteMode = AutoCompleteMode.None;
            _fileNameTextBox.AutoCompleteSource = AutoCompleteSource.None;
            _fileNameTextBox.BackColor = Color.White;
            _fileNameTextBox.BadgeBackColor = Color.Red;
            _fileNameTextBox.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _fileNameTextBox.BadgeForeColor = Color.White;
            _fileNameTextBox.BadgeShape = BadgeShape.Circle;
            _fileNameTextBox.BadgeText = "";
            _fileNameTextBox.BlockID = null;
            _fileNameTextBox.BorderColor = Color.FromArgb(200, 200, 200);
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
            _fileNameTextBox.DrawingRect = new Rectangle(5, 5, 288, 15);
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
           
            _fileNameTextBox.Info = (Models.SimpleItem)resources.GetObject("_fileNameTextBox.Info");
            _fileNameTextBox.IsAcceptButton = false;
            _fileNameTextBox.IsBorderAffectedByTheme = true;
            _fileNameTextBox.IsCancelButton = false;
            _fileNameTextBox.IsChild = true;
            _fileNameTextBox.IsCustomeBorder = false;
            _fileNameTextBox.IsDefault = false;
            _fileNameTextBox.IsDeleted = false;
            _fileNameTextBox.IsDirty = false;
            _fileNameTextBox.IsEditable = false;
            _fileNameTextBox.IsFocused = false;
            _fileNameTextBox.IsFrameless = false;
            _fileNameTextBox.IsHovered = false;
            _fileNameTextBox.IsNew = false;
            _fileNameTextBox.IsPressed = false;
            _fileNameTextBox.IsReadOnly = false;
            _fileNameTextBox.IsRequired = false;
            _fileNameTextBox.IsRounded = true;
            _fileNameTextBox.IsRoundedAffectedByTheme = true;
            _fileNameTextBox.IsSelected = false;
            _fileNameTextBox.IsShadowAffectedByTheme = true;
            _fileNameTextBox.IsVisible = false;
            _fileNameTextBox.Items = (List<object>)resources.GetObject("_fileNameTextBox.Items");
            _fileNameTextBox.LeftoffsetForDrawingRect = 0;
            _fileNameTextBox.LinkedProperty = null;
            _fileNameTextBox.Location = new Point(79, 10);
            _fileNameTextBox.MaskFormat = Vis.Modules.TextBoxMaskFormat.None;
            _fileNameTextBox.MaxImageSize = new Size(16, 16);
            _fileNameTextBox.MaximumSize = new Size(0, 25);
            _fileNameTextBox.MinimumSize = new Size(50, 25);
            _fileNameTextBox.Modified = false;
            _fileNameTextBox.Multiline = false;
            _fileNameTextBox.Name = "_fileNameTextBox";
            _fileNameTextBox.OnlyCharacters = false;
            _fileNameTextBox.OnlyDigits = false;
            _fileNameTextBox.OverrideFontSize = TypeStyleFontSize.None;
            _fileNameTextBox.Padding = new Padding(5);
            _fileNameTextBox.ParentBackColor = Color.White;
            _fileNameTextBox.ParentControl = null;
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
            _fileNameTextBox.ShowAllBorders = false;
            _fileNameTextBox.ShowBottomBorder = false;
            _fileNameTextBox.ShowFocusIndicator = false;
            _fileNameTextBox.ShowLeftBorder = false;
            _fileNameTextBox.ShowRightBorder = false;
            _fileNameTextBox.ShowScrollbars = false;
            _fileNameTextBox.ShowShadow = false;
            _fileNameTextBox.ShowTopBorder = false;
            _fileNameTextBox.ShowVerticalScrollBar = false;
            _fileNameTextBox.Size = new Size(298, 25);
            _fileNameTextBox.SlideFrom = SlideDirection.Left;
            _fileNameTextBox.StaticNotMoving = false;
            _fileNameTextBox.TabIndex = 0;
            _fileNameTextBox.TempBackColor = Color.Empty;
            _fileNameTextBox.TextAlignment = HorizontalAlignment.Left;
            _fileNameTextBox.TextFont = new Font("Segoe UI", 9F);
            _fileNameTextBox.TextImageRelation = TextImageRelation.ImageBeforeText;
            _fileNameTextBox.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
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
            splitContainer3.Size = new Size(163, 508);
            splitContainer3.SplitterDistance = 41;
            splitContainer3.TabIndex = 0;
            // 
            // _previewPane
            // 
            _previewPane.BackColor = Color.White;
            _previewPane.Dock = DockStyle.Fill;
            _previewPane.Location = new Point(0, 0);
            _previewPane.Name = "_previewPane";
            _previewPane.Size = new Size(163, 463);
            _previewPane.TabIndex = 0;
            // 
            // BeepFileDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(863, 514);
            Controls.Add(splitContainer1);
            Name = "BeepFileDialog";
            Padding = new Padding(3);
            Text = "BeepFileDialog";
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