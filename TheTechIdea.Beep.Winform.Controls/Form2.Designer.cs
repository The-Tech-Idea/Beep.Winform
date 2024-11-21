namespace TheTechIdea.Beep.Winform.Controls
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            beepSideMenu1 = new BeepSideMenu();
            beepLabel1 = new BeepLabel();
            beepImage1 = new BeepImage();
            beepButton1 = new BeepButton();
            beepCircularButton1 = new BeepCircularButton();
            beepPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // CloseButton
            // 
            CloseButton.ApplyThemeOnImage = true;
            CloseButton.BackColor = Color.FromArgb(245, 245, 245);
            CloseButton.Font = new Font("Segoe UI", 12F);
            CloseButton.ForeColor = Color.FromArgb(70, 130, 180);
            CloseButton.Location = new Point(408, 0);
            CloseButton.ParentBackColor = Color.FromArgb(245, 245, 245);
            CloseButton.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            CloseButton.ToolTipText = "Close";
            // 
            // MaximizeButton
            // 
            MaximizeButton.ApplyThemeOnImage = true;
            MaximizeButton.BackColor = Color.FromArgb(245, 245, 245);
            MaximizeButton.Font = new Font("Segoe UI", 12F);
            MaximizeButton.ForeColor = Color.FromArgb(70, 130, 180);
            MaximizeButton.Location = new Point(378, 0);
            MaximizeButton.MaxImageSize = new Size(20, 20);
            MaximizeButton.ParentBackColor = Color.FromArgb(245, 245, 245);
            MaximizeButton.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            MaximizeButton.ToolTipText = "Maximize";
            // 
            // MinimizeButton
            // 
            MinimizeButton.ApplyThemeOnImage = true;
            MinimizeButton.BackColor = Color.FromArgb(245, 245, 245);
            MinimizeButton.Font = new Font("Segoe UI", 12F);
            MinimizeButton.ForeColor = Color.FromArgb(70, 130, 180);
            MinimizeButton.Location = new Point(348, 0);
            MinimizeButton.MaxImageSize = new Size(20, 20);
            MinimizeButton.ParentBackColor = Color.FromArgb(245, 245, 245);
            MinimizeButton.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            MinimizeButton.ToolTipText = "Minimize";
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepiForm = this;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.LogoImage = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\016-admin.svg";
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepuiManager1.Title = "Asset HR Digital Space";
            // 
            // beepPanel1
            // 
            beepPanel1.BackColor = Color.FromArgb(245, 245, 245);
            beepPanel1.DrawingRect = new Rectangle(1, 1, 436, 28);
            beepPanel1.ForeColor = Color.FromArgb(70, 130, 180);
            beepPanel1.Location = new Point(380, 10);
            beepPanel1.ParentBackColor = Color.FromArgb(245, 245, 245);
            beepPanel1.ShowAllBorders = true;
            beepPanel1.ShowBottomBorder = true;
            beepPanel1.Size = new Size(438, 30);
            beepPanel1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            // 
            // TitleLabel
            // 
            TitleLabel.DrawingRect = new Rectangle(1, 1, 314, 26);
            TitleLabel.Font = new Font("Segoe UI", 12F);
            TitleLabel.ForeColor = Color.FromArgb(70, 130, 180);
            TitleLabel.Size = new Size(316, 28);
            TitleLabel.Text = "Asset HR Digital WorkSpace";
            TitleLabel.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            // 
            // FunctionsPanel1
            // 
            FunctionsPanel1.BackColor = Color.FromArgb(245, 245, 245);
            FunctionsPanel1.Location = new Point(773, 63);
            FunctionsPanel1.ParentBackColor = Color.FromArgb(245, 245, 245);
            FunctionsPanel1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(65, 105, 225);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationStep = 20;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.ApplyThemeOnImages = false;
            beepSideMenu1.BackColor = Color.FromArgb(200, 225, 245);
            beepSideMenu1.BeepForm = this;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(176, 196, 222);
            beepSideMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSideMenu1.BorderRadius = 5;
            beepSideMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepSideMenu1.BorderThickness = 1;
            beepSideMenu1.CollapsedWidth = 64;
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(1, 1, 368, 651);
            beepSideMenu1.Easing = EasingType.Linear;
            beepSideMenu1.ExpandedWidth = 370;
            beepSideMenu1.FieldID = null;
            beepSideMenu1.FocusBackColor = Color.White;
            beepSideMenu1.FocusBorderColor = Color.Gray;
            beepSideMenu1.FocusForeColor = Color.Black;
            beepSideMenu1.FocusIndicatorColor = Color.Blue;
            beepSideMenu1.Font = new Font("Segoe UI", 9F);
            beepSideMenu1.ForeColor = Color.White;
            beepSideMenu1.Form = null;
            beepSideMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSideMenu1.GradientEndColor = Color.FromArgb(176, 196, 222);
            beepSideMenu1.GradientStartColor = Color.FromArgb(245, 245, 245);
            beepSideMenu1.HilightPanelSize = 5;
            beepSideMenu1.HoverBackColor = Color.FromArgb(70, 130, 180);
            beepSideMenu1.HoverBorderColor = Color.FromArgb(65, 105, 225);
            beepSideMenu1.HoveredBackcolor = Color.Wheat;
            beepSideMenu1.HoverForeColor = Color.Black;
            beepSideMenu1.Id = -1;
            beepSideMenu1.InactiveBackColor = Color.Gray;
            beepSideMenu1.InactiveBorderColor = Color.Gray;
            beepSideMenu1.InactiveForeColor = Color.Black;
            beepSideMenu1.IsAcceptButton = false;
            beepSideMenu1.IsBorderAffectedByTheme = false;
            beepSideMenu1.IsCancelButton = false;
            beepSideMenu1.IsChild = false;
            beepSideMenu1.IsDefault = false;
            beepSideMenu1.IsFocused = false;
            beepSideMenu1.IsFramless = false;
            beepSideMenu1.IsHovered = false;
            beepSideMenu1.IsPressed = false;
            beepSideMenu1.IsRounded = false;
            beepSideMenu1.IsShadowAffectedByTheme = false;
            beepSideMenu1.Items.Add((Template.SimpleMenuItem)resources.GetObject("beepSideMenu1.Items"));
            beepSideMenu1.Items.Add((Template.SimpleMenuItem)resources.GetObject("beepSideMenu1.Items1"));
            beepSideMenu1.Location = new Point(10, 10);
            beepSideMenu1.LogoImage = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\016-admin.svg";
            beepSideMenu1.Name = "beepSideMenu1";
            beepSideMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepSideMenu1.Padding = new Padding(2);
            beepSideMenu1.ParentBackColor = Color.FromArgb(245, 245, 245);
            beepSideMenu1.PressedBackColor = Color.FromArgb(65, 105, 225);
            beepSideMenu1.PressedBorderColor = Color.Gray;
            beepSideMenu1.PressedForeColor = Color.Black;
            beepSideMenu1.SavedGuidID = null;
            beepSideMenu1.SavedID = null;
            beepSideMenu1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepSideMenu1.ShadowOffset = 0;
            beepSideMenu1.ShadowOpacity = 0.5F;
            beepSideMenu1.ShowAllBorders = true;
            beepSideMenu1.ShowBottomBorder = true;
            beepSideMenu1.ShowFocusIndicator = false;
            beepSideMenu1.ShowLeftBorder = true;
            beepSideMenu1.ShowRightBorder = true;
            beepSideMenu1.ShowShadow = false;
            beepSideMenu1.ShowTopBorder = true;
            beepSideMenu1.Size = new Size(370, 653);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 0;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepSideMenu1.Title = "Asset HR Digital Space";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.UseGradientBackground = false;
            // 
            // beepLabel1
            // 
            beepLabel1.ActiveBackColor = Color.Gray;
            beepLabel1.AnimationDuration = 500;
            beepLabel1.AnimationType = DisplayAnimationType.None;
            beepLabel1.ApplyThemeOnImage = false;
            beepLabel1.BackColor = Color.FromArgb(245, 245, 245);
            beepLabel1.BlockID = null;
            beepLabel1.BorderColor = Color.Black;
            beepLabel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepLabel1.BorderRadius = 5;
            beepLabel1.BorderStyle = BorderStyle.FixedSingle;
            beepLabel1.BorderThickness = 1;
            beepLabel1.DataContext = null;
            beepLabel1.DisabledBackColor = Color.Gray;
            beepLabel1.DisabledForeColor = Color.Empty;
            beepLabel1.DrawingRect = new Rectangle(1, 1, 178, 35);
            beepLabel1.Easing = EasingType.Linear;
            beepLabel1.FieldID = null;
            beepLabel1.FocusBackColor = Color.Gray;
            beepLabel1.FocusBorderColor = Color.Gray;
            beepLabel1.FocusForeColor = Color.Black;
            beepLabel1.FocusIndicatorColor = Color.Blue;
            beepLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            beepLabel1.ForeColor = Color.FromArgb(60, 60, 60);
            beepLabel1.Form = null;
            beepLabel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepLabel1.GradientEndColor = Color.Gray;
            beepLabel1.GradientStartColor = Color.Gray;
            beepLabel1.HideText = false;
            beepLabel1.HoverBackColor = Color.Gray;
            beepLabel1.HoverBorderColor = Color.Gray;
            beepLabel1.HoveredBackcolor = Color.Wheat;
            beepLabel1.HoverForeColor = Color.Black;
            beepLabel1.Id = -1;
            beepLabel1.ImageAlign = ContentAlignment.MiddleLeft;
            beepLabel1.ImagePath = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\018-puzzle.svg";
            beepLabel1.InactiveBackColor = Color.Gray;
            beepLabel1.InactiveBorderColor = Color.Gray;
            beepLabel1.InactiveForeColor = Color.Black;
            beepLabel1.IsAcceptButton = false;
            beepLabel1.IsBorderAffectedByTheme = true;
            beepLabel1.IsCancelButton = false;
            beepLabel1.IsChild = false;
            beepLabel1.IsDefault = false;
            beepLabel1.IsFocused = false;
            beepLabel1.IsFramless = false;
            beepLabel1.IsHovered = false;
            beepLabel1.IsPressed = false;
            beepLabel1.IsRounded = false;
            beepLabel1.IsShadowAffectedByTheme = true;
            beepLabel1.Location = new Point(470, 216);
            beepLabel1.Margin = new Padding(0);
            beepLabel1.MaxImageSize = new Size(16, 16);
            beepLabel1.Name = "beepLabel1";
            beepLabel1.OverrideFontSize = TypeStyleFontSize.None;
            beepLabel1.ParentBackColor = Color.Empty;
            beepLabel1.PressedBackColor = Color.Gray;
            beepLabel1.PressedBorderColor = Color.Gray;
            beepLabel1.PressedForeColor = Color.Black;
            beepLabel1.SavedGuidID = null;
            beepLabel1.SavedID = null;
            beepLabel1.ShadowColor = Color.Black;
            beepLabel1.ShadowOffset = 0;
            beepLabel1.ShadowOpacity = 0.5F;
            beepLabel1.ShowAllBorders = true;
            beepLabel1.ShowBottomBorder = true;
            beepLabel1.ShowFocusIndicator = false;
            beepLabel1.ShowLeftBorder = true;
            beepLabel1.ShowRightBorder = true;
            beepLabel1.ShowShadow = false;
            beepLabel1.ShowTopBorder = true;
            beepLabel1.Size = new Size(180, 37);
            beepLabel1.SlideFrom = SlideDirection.Left;
            beepLabel1.StaticNotMoving = false;
            beepLabel1.TabIndex = 6;
            beepLabel1.Text = "beepLabel1";
            beepLabel1.TextAlign = ContentAlignment.MiddleLeft;
            beepLabel1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepLabel1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepLabel1.ToolTipText = "";
            beepLabel1.UseGradientBackground = false;
            // 
            // beepImage1
            // 
            beepImage1.ActiveBackColor = Color.FromArgb(65, 105, 225);
            beepImage1.AnimationDuration = 500;
            beepImage1.AnimationType = DisplayAnimationType.None;
            beepImage1.ApplyThemeOnImage = false;
            beepImage1.BlockID = null;
            beepImage1.BorderColor = Color.FromArgb(176, 196, 222);
            beepImage1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepImage1.BorderRadius = 5;
            beepImage1.BorderStyle = BorderStyle.FixedSingle;
            beepImage1.BorderThickness = 1;
            beepImage1.DataContext = null;
            beepImage1.DisabledBackColor = Color.Gray;
            beepImage1.DisabledForeColor = Color.Empty;
            beepImage1.DrawingRect = new Rectangle(1, 1, 147, 87);
            beepImage1.Easing = EasingType.Linear;
            beepImage1.FieldID = null;
            beepImage1.FocusBackColor = Color.FromArgb(65, 105, 225);
            beepImage1.FocusBorderColor = Color.Gray;
            beepImage1.FocusForeColor = Color.White;
            beepImage1.FocusIndicatorColor = Color.Blue;
            beepImage1.Form = null;
            beepImage1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepImage1.GradientEndColor = Color.FromArgb(176, 196, 222);
            beepImage1.GradientStartColor = Color.FromArgb(245, 245, 245);
            beepImage1.HoverBackColor = Color.FromArgb(70, 130, 180);
            beepImage1.HoverBorderColor = Color.FromArgb(65, 105, 225);
            beepImage1.HoveredBackcolor = Color.Wheat;
            beepImage1.HoverForeColor = Color.White;
            beepImage1.Id = -1;
            beepImage1.Image = null;
            beepImage1.ImagePath = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\020-organization.svg";
            beepImage1.InactiveBackColor = Color.Gray;
            beepImage1.InactiveBorderColor = Color.Gray;
            beepImage1.InactiveForeColor = Color.Black;
            beepImage1.IsAcceptButton = false;
            beepImage1.IsBorderAffectedByTheme = true;
            beepImage1.IsCancelButton = false;
            beepImage1.IsChild = false;
            beepImage1.IsDefault = false;
            beepImage1.IsFocused = false;
            beepImage1.IsFramless = false;
            beepImage1.IsHovered = false;
            beepImage1.IsPressed = false;
            beepImage1.IsRounded = false;
            beepImage1.IsShadowAffectedByTheme = true;
            beepImage1.IsStillImage = false;
            beepImage1.Location = new Point(501, 358);
            beepImage1.Name = "beepImage1";
            beepImage1.OverrideFontSize = TypeStyleFontSize.None;
            beepImage1.ParentBackColor = Color.Empty;
            beepImage1.PressedBackColor = Color.FromArgb(65, 105, 225);
            beepImage1.PressedBorderColor = Color.Gray;
            beepImage1.PressedForeColor = Color.White;
            beepImage1.SavedGuidID = null;
            beepImage1.SavedID = null;
            beepImage1.ScaleMode = ImageScaleMode.KeepAspectRatio;
            beepImage1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepImage1.ShadowOffset = 0;
            beepImage1.ShadowOpacity = 0.5F;
            beepImage1.ShowAllBorders = true;
            beepImage1.ShowBottomBorder = true;
            beepImage1.ShowFocusIndicator = false;
            beepImage1.ShowLeftBorder = true;
            beepImage1.ShowRightBorder = true;
            beepImage1.ShowShadow = false;
            beepImage1.ShowTopBorder = true;
            beepImage1.Size = new Size(149, 89);
            beepImage1.SlideFrom = SlideDirection.Left;
            beepImage1.StaticNotMoving = false;
            beepImage1.TabIndex = 4;
            beepImage1.Text = "beepImage1";
            beepImage1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepImage1.ToolTipText = "";
            beepImage1.UseGradientBackground = false;
            // 
            // beepButton1
            // 
            beepButton1.ActiveBackColor = Color.Gray;
            beepButton1.AnimationDuration = 500;
            beepButton1.AnimationType = DisplayAnimationType.None;
            beepButton1.ApplyThemeOnImage = false;
            beepButton1.BackColor = Color.FromArgb(245, 245, 245);
            beepButton1.BlockID = null;
            beepButton1.BorderColor = Color.Black;
            beepButton1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepButton1.BorderRadius = 5;
            beepButton1.BorderSize = 1;
            beepButton1.BorderStyle = BorderStyle.FixedSingle;
            beepButton1.BorderThickness = 1;
            beepButton1.DataContext = null;
            beepButton1.DisabledBackColor = Color.Gray;
            beepButton1.DisabledForeColor = Color.Empty;
            beepButton1.DrawingRect = new Rectangle(1, 1, 172, 38);
            beepButton1.Easing = EasingType.Linear;
            beepButton1.FieldID = null;
            beepButton1.FlatAppearance = true;
            beepButton1.FlatStyle = FlatStyle.Standard;
            beepButton1.FocusBackColor = Color.Gray;
            beepButton1.FocusBorderColor = Color.Gray;
            beepButton1.FocusForeColor = Color.Black;
            beepButton1.FocusIndicatorColor = Color.Blue;
            beepButton1.Font = new Font("Segoe UI", 12F);
            beepButton1.ForeColor = Color.White;
            beepButton1.Form = null;
            beepButton1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepButton1.GradientEndColor = Color.Gray;
            beepButton1.GradientStartColor = Color.Gray;
            beepButton1.HideText = false;
            beepButton1.HoverBackColor = Color.Gray;
            beepButton1.HoverBorderColor = Color.Gray;
            beepButton1.HoveredBackcolor = Color.Wheat;
            beepButton1.HoverForeColor = Color.Black;
            beepButton1.Id = -1;
            beepButton1.Image = null;
            beepButton1.ImageAlign = ContentAlignment.MiddleRight;
            beepButton1.ImageClicked = null;
            beepButton1.ImagePath = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\016-admin.svg";
            beepButton1.InactiveBackColor = Color.Gray;
            beepButton1.InactiveBorderColor = Color.Gray;
            beepButton1.InactiveForeColor = Color.Black;
            beepButton1.IsAcceptButton = false;
            beepButton1.IsBorderAffectedByTheme = true;
            beepButton1.IsCancelButton = false;
            beepButton1.IsChild = false;
            beepButton1.IsDefault = false;
            beepButton1.IsFocused = false;
            beepButton1.IsFramless = false;
            beepButton1.IsHovered = false;
            beepButton1.IsPressed = false;
            beepButton1.IsRounded = false;
            beepButton1.IsSelected = false;
            beepButton1.IsShadowAffectedByTheme = true;
            beepButton1.IsSideMenuChild = false;
            beepButton1.IsStillButton = false;
            beepButton1.Location = new Point(501, 300);
            beepButton1.Margin = new Padding(0);
            beepButton1.MaxImageSize = new Size(32, 32);
            beepButton1.Name = "beepButton1";
            beepButton1.OverrideFontSize = TypeStyleFontSize.None;
            beepButton1.ParentBackColor = Color.Empty;
            beepButton1.PressedBackColor = Color.Gray;
            beepButton1.PressedBorderColor = Color.Gray;
            beepButton1.PressedForeColor = Color.Black;
            beepButton1.SavedGuidID = null;
            beepButton1.SavedID = null;
            beepButton1.SelectedBorderColor = Color.Blue;
            beepButton1.ShadowColor = Color.Black;
            beepButton1.ShadowOffset = 0;
            beepButton1.ShadowOpacity = 0.5F;
            beepButton1.ShowAllBorders = true;
            beepButton1.ShowBottomBorder = true;
            beepButton1.ShowFocusIndicator = false;
            beepButton1.ShowLeftBorder = true;
            beepButton1.ShowRightBorder = true;
            beepButton1.ShowShadow = false;
            beepButton1.ShowTopBorder = true;
            beepButton1.Size = new Size(174, 40);
            beepButton1.SlideFrom = SlideDirection.Left;
            beepButton1.StaticNotMoving = false;
            beepButton1.TabIndex = 7;
            beepButton1.Text = "beepButton1";
            beepButton1.TextAlign = ContentAlignment.MiddleLeft;
            beepButton1.TextImageRelation = TextImageRelation.Overlay;
            beepButton1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepButton1.ToolTipText = "";
            beepButton1.UseGradientBackground = false;
            // 
            // beepCircularButton1
            // 
            beepCircularButton1.ActiveBackColor = Color.Gray;
            beepCircularButton1.AnimationDuration = 500;
            beepCircularButton1.AnimationType = DisplayAnimationType.None;
            beepCircularButton1.ApplyThemeOnImage = false;
            beepCircularButton1.BackColor = Color.FromArgb(245, 245, 245);
            beepCircularButton1.BlockID = null;
            beepCircularButton1.BorderColor = Color.Black;
            beepCircularButton1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepCircularButton1.BorderRadius = 5;
            beepCircularButton1.BorderStyle = BorderStyle.FixedSingle;
            beepCircularButton1.BorderThickness = 1;
            beepCircularButton1.DataContext = null;
            beepCircularButton1.DisabledBackColor = Color.Gray;
            beepCircularButton1.DisabledForeColor = Color.Empty;
            beepCircularButton1.DrawingRect = new Rectangle(1, 1, 131, 103);
            beepCircularButton1.Easing = EasingType.Linear;
            beepCircularButton1.FieldID = null;
            beepCircularButton1.FocusBackColor = Color.Gray;
            beepCircularButton1.FocusBorderColor = Color.Gray;
            beepCircularButton1.FocusForeColor = Color.Black;
            beepCircularButton1.FocusIndicatorColor = Color.Blue;
            beepCircularButton1.Font = new Font("Segoe UI", 12F);
            beepCircularButton1.ForeColor = Color.White;
            beepCircularButton1.Form = null;
            beepCircularButton1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepCircularButton1.GradientEndColor = Color.Gray;
            beepCircularButton1.GradientStartColor = Color.Gray;
            beepCircularButton1.HideText = false;
            beepCircularButton1.HoverBackColor = Color.Gray;
            beepCircularButton1.HoverBorderColor = Color.Gray;
            beepCircularButton1.HoveredBackcolor = Color.Wheat;
            beepCircularButton1.HoverForeColor = Color.Black;
            beepCircularButton1.Id = -1;
            beepCircularButton1.ImagePath = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\018-puzzle.svg";
            beepCircularButton1.InactiveBackColor = Color.Gray;
            beepCircularButton1.InactiveBorderColor = Color.Gray;
            beepCircularButton1.InactiveForeColor = Color.Black;
            beepCircularButton1.IsAcceptButton = false;
            beepCircularButton1.IsBorderAffectedByTheme = true;
            beepCircularButton1.IsCancelButton = false;
            beepCircularButton1.IsChild = true;
            beepCircularButton1.IsDefault = false;
            beepCircularButton1.IsFocused = false;
            beepCircularButton1.IsForColorSet = false;
            beepCircularButton1.IsFramless = true;
            beepCircularButton1.IsHovered = false;
            beepCircularButton1.IsPressed = false;
            beepCircularButton1.IsRounded = false;
            beepCircularButton1.IsShadowAffectedByTheme = true;
            beepCircularButton1.Location = new Point(542, 487);
            beepCircularButton1.Name = "beepCircularButton1";
            beepCircularButton1.OverrideFontSize = TypeStyleFontSize.None;
            beepCircularButton1.ParentBackColor = Color.FromArgb(245, 245, 245);
            beepCircularButton1.PressedBackColor = Color.Gray;
            beepCircularButton1.PressedBorderColor = Color.Gray;
            beepCircularButton1.PressedForeColor = Color.Black;
            beepCircularButton1.SavedGuidID = null;
            beepCircularButton1.SavedID = null;
            beepCircularButton1.ShadowColor = Color.Black;
            beepCircularButton1.ShadowOffset = 0;
            beepCircularButton1.ShadowOpacity = 0.5F;
            beepCircularButton1.ShowAllBorders = true;
            beepCircularButton1.ShowBorder = true;
            beepCircularButton1.ShowBottomBorder = true;
            beepCircularButton1.ShowFocusIndicator = false;
            beepCircularButton1.ShowLeftBorder = true;
            beepCircularButton1.ShowRightBorder = true;
            beepCircularButton1.ShowShadow = false;
            beepCircularButton1.ShowTopBorder = true;
            beepCircularButton1.Size = new Size(133, 105);
            beepCircularButton1.SlideFrom = SlideDirection.Left;
            beepCircularButton1.StaticNotMoving = false;
            beepCircularButton1.TabIndex = 8;
            beepCircularButton1.Text = "beepCircularButton1";
            beepCircularButton1.TextAlign = ContentAlignment.MiddleCenter;
            beepCircularButton1.TextLocation = TextLocation.Below;
            beepCircularButton1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepCircularButton1.ToolTipText = "";
            beepCircularButton1.UseGradientBackground = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(828, 673);
            Controls.Add(beepCircularButton1);
            Controls.Add(beepButton1);
            Controls.Add(beepImage1);
            Controls.Add(beepLabel1);
            Controls.Add(beepSideMenu1);
            DoubleBuffered = true;
            Name = "Form2";
            Padding = new Padding(10);
            Text = "Asset HR Digital Space";
            Controls.SetChildIndex(beepSideMenu1, 0);
            Controls.SetChildIndex(FunctionsPanel1, 0);
            Controls.SetChildIndex(beepPanel1, 0);
            Controls.SetChildIndex(beepLabel1, 0);
            Controls.SetChildIndex(beepImage1, 0);
            Controls.SetChildIndex(beepButton1, 0);
            Controls.SetChildIndex(beepCircularButton1, 0);
            beepPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BeepSideMenu beepSideMenu1;
        private BeepLabel beepLabel1;
        private BeepImage beepImage1;
        private BeepButton beepButton1;
        private BeepCircularButton beepCircularButton1;
    }
}