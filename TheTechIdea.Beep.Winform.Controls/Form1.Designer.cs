﻿namespace TheTechIdea.Beep.Winform.Controls
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            beepAppBar1 = new BeepAppBar();
            beepSideMenu1 = new BeepSideMenu();
            beepListBox1 = new BeepListBox();
            beepCheckBox1 = new BeepCheckBox();
            beepButton1 = new BeepButton();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepAppBar = beepAppBar1;
            beepuiManager1.BeepiForm = this;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.IsRounded = false;
            beepuiManager1.ShowShadow = true;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.RetroTheme;
            // 
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = DisplayAnimationType.None;
            beepAppBar1.ApplyThemeOnImage = false;
            beepAppBar1.ApplyThemeToChilds = true;
            beepAppBar1.BackColor = Color.FromArgb(75, 0, 130);
            beepAppBar1.BlockID = null;
            beepAppBar1.BorderColor = Color.Black;
            beepAppBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepAppBar1.BorderRadius = 5;
            beepAppBar1.BorderStyle = BorderStyle.FixedSingle;
            beepAppBar1.BorderThickness = 1;
            beepAppBar1.DataContext = null;
            beepAppBar1.DisabledBackColor = Color.Gray;
            beepAppBar1.DisabledForeColor = Color.Empty;
            beepAppBar1.Dock = DockStyle.Top;
            beepAppBar1.DrawingRect = new Rectangle(4, 4, 482, 24);
            beepAppBar1.Easing = EasingType.Linear;
            beepAppBar1.FieldID = null;
            beepAppBar1.FocusBackColor = Color.Gray;
            beepAppBar1.FocusBorderColor = Color.Gray;
            beepAppBar1.FocusForeColor = Color.Black;
            beepAppBar1.FocusIndicatorColor = Color.Blue;
            beepAppBar1.Form = null;
            beepAppBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepAppBar1.GradientEndColor = Color.Gray;
            beepAppBar1.GradientStartColor = Color.Gray;
            beepAppBar1.HoverBackColor = Color.Gray;
            beepAppBar1.HoverBorderColor = Color.Gray;
            beepAppBar1.HoveredBackcolor = Color.Wheat;
            beepAppBar1.HoverForeColor = Color.Black;
            beepAppBar1.Id = -1;
            beepAppBar1.InactiveBackColor = Color.Gray;
            beepAppBar1.InactiveBorderColor = Color.Gray;
            beepAppBar1.InactiveForeColor = Color.Black;
            beepAppBar1.IsAcceptButton = false;
            beepAppBar1.IsBorderAffectedByTheme = false;
            beepAppBar1.IsCancelButton = false;
            beepAppBar1.IsChild = false;
            beepAppBar1.IsCustomeBorder = false;
            beepAppBar1.IsDefault = false;
            beepAppBar1.IsFocused = false;
            beepAppBar1.IsHovered = false;
            beepAppBar1.IsPressed = false;
            beepAppBar1.IsRounded = false;
            beepAppBar1.IsRoundedAffectedByTheme = false;
            beepAppBar1.IsShadowAffectedByTheme = false;
            beepAppBar1.Location = new Point(305, 5);
            beepAppBar1.Name = "beepAppBar1";
            beepAppBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepAppBar1.ParentBackColor = Color.Empty;
            beepAppBar1.PressedBackColor = Color.Gray;
            beepAppBar1.PressedBorderColor = Color.Gray;
            beepAppBar1.PressedForeColor = Color.Black;
            beepAppBar1.SavedGuidID = null;
            beepAppBar1.SavedID = null;
            beepAppBar1.ShadowColor = Color.Black;
            beepAppBar1.ShadowOffset = 3;
            beepAppBar1.ShadowOpacity = 0.5F;
            beepAppBar1.ShowAllBorders = true;
            beepAppBar1.ShowBottomBorder = true;
            beepAppBar1.ShowCloseIcon = true;
            beepAppBar1.ShowFocusIndicator = false;
            beepAppBar1.ShowLeftBorder = true;
            beepAppBar1.ShowLogoIcon = false;
            beepAppBar1.ShowMaximizeIcon = true;
            beepAppBar1.ShowMinimizeIcon = true;
            beepAppBar1.ShowNotificationIcon = true;
            beepAppBar1.ShowProfileIcon = true;
            beepAppBar1.ShowRightBorder = true;
            beepAppBar1.ShowSearchBox = true;
            beepAppBar1.ShowShadow = true;
            beepAppBar1.ShowTopBorder = true;
            beepAppBar1.SideMenu = null;
            beepAppBar1.Size = new Size(490, 32);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 0;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.RetroTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.UseGradientBackground = false;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(255, 255, 0);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationStep = 20;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.ApplyThemeOnImages = false;
            beepSideMenu1.ApplyThemeToChilds = false;
            beepSideMenu1.BackColor = Color.FromArgb(148, 0, 211);
            beepSideMenu1.BeepAppBar = null;
            beepSideMenu1.BeepForm = this;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(255, 105, 180);
            beepSideMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSideMenu1.BorderRadius = 5;
            beepSideMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepSideMenu1.BorderThickness = 1;
            beepSideMenu1.CollapsedWidth = 64;
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(4, 4, 292, 525);
            beepSideMenu1.Easing = EasingType.Linear;
            beepSideMenu1.ExpandedWidth = 300;
            beepSideMenu1.FieldID = null;
            beepSideMenu1.FocusBackColor = Color.Black;
            beepSideMenu1.FocusBorderColor = Color.Gray;
            beepSideMenu1.FocusForeColor = Color.Black;
            beepSideMenu1.FocusIndicatorColor = Color.Blue;
            beepSideMenu1.Font = new Font("Segoe UI", 9F);
            beepSideMenu1.ForeColor = Color.White;
            beepSideMenu1.Form = null;
            beepSideMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSideMenu1.GradientEndColor = Color.FromArgb(75, 0, 130);
            beepSideMenu1.GradientStartColor = Color.Black;
            beepSideMenu1.HilightPanelSize = 5;
            beepSideMenu1.HoverBackColor = Color.FromArgb(0, 255, 255);
            beepSideMenu1.HoverBorderColor = Color.FromArgb(255, 255, 0);
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
            beepSideMenu1.IsCustomeBorder = false;
            beepSideMenu1.IsDefault = false;
            beepSideMenu1.IsFocused = false;
            beepSideMenu1.IsFramless = false;
            beepSideMenu1.IsHovered = false;
            beepSideMenu1.IsPressed = false;
            beepSideMenu1.IsRounded = false;
            beepSideMenu1.IsRoundedAffectedByTheme = true;
            beepSideMenu1.IsShadowAffectedByTheme = false;
            beepSideMenu1.Items.Add((Models.SimpleItem)resources.GetObject("beepSideMenu1.Items"));
            beepSideMenu1.Items.Add((Models.SimpleItem)resources.GetObject("beepSideMenu1.Items1"));
            beepSideMenu1.Location = new Point(5, 5);
            beepSideMenu1.LogoImage = "";
            beepSideMenu1.Name = "beepSideMenu1";
            beepSideMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepSideMenu1.Padding = new Padding(5);
            beepSideMenu1.ParentBackColor = Color.White;
            beepSideMenu1.PressedBackColor = Color.FromArgb(255, 255, 0);
            beepSideMenu1.PressedBorderColor = Color.Gray;
            beepSideMenu1.PressedForeColor = Color.Black;
            beepSideMenu1.SavedGuidID = null;
            beepSideMenu1.SavedID = null;
            beepSideMenu1.ShadowColor = Color.FromArgb(100, 255, 255, 0);
            beepSideMenu1.ShadowOffset = 3;
            beepSideMenu1.ShadowOpacity = 0.5F;
            beepSideMenu1.ShowAllBorders = true;
            beepSideMenu1.ShowBottomBorder = true;
            beepSideMenu1.ShowFocusIndicator = false;
            beepSideMenu1.ShowLeftBorder = true;
            beepSideMenu1.ShowRightBorder = true;
            beepSideMenu1.ShowShadow = true;
            beepSideMenu1.ShowTopBorder = true;
            beepSideMenu1.Size = new Size(300, 533);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 1;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.RetroTheme;
            beepSideMenu1.Title = "Beep Form";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.UseGradientBackground = false;
            // 
            // beepListBox1
            // 
            beepListBox1.ActiveBackColor = Color.Gray;
            beepListBox1.AnimationDuration = 500;
            beepListBox1.AnimationType = DisplayAnimationType.None;
            beepListBox1.ApplyThemeToChilds = true;
            beepListBox1.BackColor = Color.FromArgb(148, 0, 211);
            beepListBox1.BlockID = null;
            beepListBox1.BorderColor = Color.Black;
            beepListBox1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepListBox1.BorderRadius = 5;
            beepListBox1.BorderStyle = BorderStyle.FixedSingle;
            beepListBox1.BorderThickness = 1;
            beepListBox1.DataContext = null;
            beepListBox1.DisabledBackColor = Color.Gray;
            beepListBox1.DisabledForeColor = Color.Empty;
            beepListBox1.DrawingRect = new Rectangle(4, 4, 168, 327);
            beepListBox1.Easing = EasingType.Linear;
            beepListBox1.FieldID = null;
            beepListBox1.FocusBackColor = Color.Gray;
            beepListBox1.FocusBorderColor = Color.Gray;
            beepListBox1.FocusForeColor = Color.Black;
            beepListBox1.FocusIndicatorColor = Color.Blue;
            beepListBox1.Form = null;
            beepListBox1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepListBox1.GradientEndColor = Color.Gray;
            beepListBox1.GradientStartColor = Color.Gray;
            beepListBox1.HoverBackColor = Color.Gray;
            beepListBox1.HoverBorderColor = Color.Gray;
            beepListBox1.HoveredBackcolor = Color.Wheat;
            beepListBox1.HoverForeColor = Color.Black;
            beepListBox1.Id = -1;
            beepListBox1.InactiveBackColor = Color.Gray;
            beepListBox1.InactiveBorderColor = Color.Gray;
            beepListBox1.InactiveForeColor = Color.Black;
            beepListBox1.IsAcceptButton = false;
            beepListBox1.IsBorderAffectedByTheme = true;
            beepListBox1.IsCancelButton = false;
            beepListBox1.IsChild = false;
            beepListBox1.IsCustomeBorder = false;
            beepListBox1.IsDefault = false;
            beepListBox1.IsFocused = false;
            beepListBox1.IsFramless = false;
            beepListBox1.IsHovered = false;
            beepListBox1.IsPressed = false;
            beepListBox1.IsRounded = false;
            beepListBox1.IsRoundedAffectedByTheme = true;
            beepListBox1.IsShadowAffectedByTheme = true;
            beepListBox1.ListItems.Add((Models.SimpleItem)resources.GetObject("beepListBox1.ListItems"));
            beepListBox1.ListItems.Add((Models.SimpleItem)resources.GetObject("beepListBox1.ListItems1"));
            beepListBox1.Location = new Point(394, 135);
            beepListBox1.MenuItemHeight = 20;
            beepListBox1.Name = "beepListBox1";
            beepListBox1.OverrideFontSize = TypeStyleFontSize.None;
            beepListBox1.ParentBackColor = Color.Empty;
            beepListBox1.PressedBackColor = Color.Gray;
            beepListBox1.PressedBorderColor = Color.Gray;
            beepListBox1.PressedForeColor = Color.Black;
            beepListBox1.SavedGuidID = null;
            beepListBox1.SavedID = null;
            beepListBox1.SelectedIndex = -1;
            beepListBox1.ShadowColor = Color.Black;
            beepListBox1.ShadowOffset = 3;
            beepListBox1.ShadowOpacity = 0.5F;
            beepListBox1.ShowAllBorders = true;
            beepListBox1.ShowBottomBorder = true;
            beepListBox1.ShowFocusIndicator = false;
            beepListBox1.ShowImage = false;
            beepListBox1.ShowLeftBorder = true;
            beepListBox1.ShowRightBorder = true;
            beepListBox1.ShowShadow = true;
            beepListBox1.ShowTitle = true;
            beepListBox1.ShowTitleLine = true;
            beepListBox1.ShowTitleLineinFullWidth = true;
            beepListBox1.ShowTopBorder = true;
            beepListBox1.Size = new Size(176, 335);
            beepListBox1.SlideFrom = SlideDirection.Left;
            beepListBox1.StaticNotMoving = false;
            beepListBox1.TabIndex = 2;
            beepListBox1.Text = "beepListBox1";
            beepListBox1.Theme = Vis.Modules.EnumBeepThemes.RetroTheme;
            beepListBox1.TitleAlignment = ContentAlignment.TopLeft;
            beepListBox1.TitleBottomY = 43;
            beepListBox1.TitleLineColor = Color.Gray;
            beepListBox1.TitleLineThickness = 2;
            beepListBox1.TitleText = "List Box";
            beepListBox1.ToolTipText = "";
            beepListBox1.UseGradientBackground = false;
            // 
            // beepCheckBox1
            // 
            beepCheckBox1.ActiveBackColor = Color.Gray;
            beepCheckBox1.AnimationDuration = 500;
            beepCheckBox1.AnimationType = DisplayAnimationType.None;
            beepCheckBox1.ApplyThemeToChilds = true;
            beepCheckBox1.BackColor = Color.FromArgb(0, 0, 0);
            beepCheckBox1.BlockID = null;
            beepCheckBox1.BorderColor = Color.Black;
            beepCheckBox1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepCheckBox1.BorderRadius = 5;
            beepCheckBox1.BorderStyle = BorderStyle.FixedSingle;
            beepCheckBox1.BorderThickness = 1;
            beepCheckBox1.CheckedFont = new Font("Arial", 10F, FontStyle.Bold);
            beepCheckBox1.CheckedValue = 'N';
            beepCheckBox1.CheckMark = BeepCheckBox.CheckMarkShape.Square;
            beepCheckBox1.DataContext = null;
            beepCheckBox1.DisabledBackColor = Color.Gray;
            beepCheckBox1.DisabledForeColor = Color.Empty;
            beepCheckBox1.DrawingRect = new Rectangle(4, 4, 126, 22);
            beepCheckBox1.Easing = EasingType.Linear;
            beepCheckBox1.FalseValue = 'N';
            beepCheckBox1.FieldID = null;
            beepCheckBox1.FocusBackColor = Color.Gray;
            beepCheckBox1.FocusBorderColor = Color.Gray;
            beepCheckBox1.FocusForeColor = Color.Black;
            beepCheckBox1.FocusIndicatorColor = Color.Blue;
            beepCheckBox1.ForeColor = Color.FromArgb(255, 255, 0);
            beepCheckBox1.Form = null;
            beepCheckBox1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepCheckBox1.GradientEndColor = Color.Gray;
            beepCheckBox1.GradientStartColor = Color.Gray;
            beepCheckBox1.HoverBackColor = Color.Gray;
            beepCheckBox1.HoverBorderColor = Color.Gray;
            beepCheckBox1.HoveredBackcolor = Color.Wheat;
            beepCheckBox1.HoverForeColor = Color.Black;
            beepCheckBox1.Id = -1;
            beepCheckBox1.ImagePath = null;
            beepCheckBox1.InactiveBackColor = Color.Gray;
            beepCheckBox1.InactiveBorderColor = Color.Gray;
            beepCheckBox1.InactiveForeColor = Color.Black;
            beepCheckBox1.IsAcceptButton = false;
            beepCheckBox1.IsBorderAffectedByTheme = true;
            beepCheckBox1.IsCancelButton = false;
            beepCheckBox1.IsChild = false;
            beepCheckBox1.IsCustomeBorder = false;
            beepCheckBox1.IsDefault = false;
            beepCheckBox1.IsFocused = false;
            beepCheckBox1.IsFramless = false;
            beepCheckBox1.IsHovered = false;
            beepCheckBox1.IsPressed = false;
            beepCheckBox1.IsRounded = false;
            beepCheckBox1.IsRoundedAffectedByTheme = true;
            beepCheckBox1.IsShadowAffectedByTheme = true;
            beepCheckBox1.Location = new Point(636, 264);
            beepCheckBox1.Name = "beepCheckBox1";
            beepCheckBox1.OverrideFontSize = TypeStyleFontSize.None;
            beepCheckBox1.Padding = new Padding(5);
            beepCheckBox1.ParentBackColor = Color.Empty;
            beepCheckBox1.PressedBackColor = Color.Gray;
            beepCheckBox1.PressedBorderColor = Color.Gray;
            beepCheckBox1.PressedForeColor = Color.Black;
            beepCheckBox1.SavedGuidID = null;
            beepCheckBox1.SavedID = null;
            beepCheckBox1.ShadowColor = Color.Black;
            beepCheckBox1.ShadowOffset = 3;
            beepCheckBox1.ShadowOpacity = 0.5F;
            beepCheckBox1.ShowAllBorders = true;
            beepCheckBox1.ShowBottomBorder = true;
            beepCheckBox1.ShowFocusIndicator = false;
            beepCheckBox1.ShowLeftBorder = true;
            beepCheckBox1.ShowRightBorder = true;
            beepCheckBox1.ShowShadow = true;
            beepCheckBox1.ShowTopBorder = true;
            beepCheckBox1.Size = new Size(134, 30);
            beepCheckBox1.SlideFrom = SlideDirection.Left;
            beepCheckBox1.Spacing = 5;
            beepCheckBox1.State = BeepCheckBox.CheckBoxState.Unchecked;
            beepCheckBox1.StaticNotMoving = false;
            beepCheckBox1.TabIndex = 3;
            beepCheckBox1.Text = "beepCheckBox1";
            beepCheckBox1.TextAlignRelativeToCheckBox = BeepCheckBox.TextAlignment.Right;
            beepCheckBox1.Theme = Vis.Modules.EnumBeepThemes.RetroTheme;
            beepCheckBox1.ToolTipText = "";
            beepCheckBox1.TrueValue = 'Y';
            beepCheckBox1.UncheckedFont = new Font("Arial", 10F);
            beepCheckBox1.UseGradientBackground = false;
            // 
            // beepButton1
            // 
            beepButton1.ActiveBackColor = Color.Gray;
            beepButton1.AnimationDuration = 500;
            beepButton1.AnimationType = DisplayAnimationType.None;
            beepButton1.ApplyThemeOnImage = false;
            beepButton1.ApplyThemeToChilds = true;
            beepButton1.BackColor = Color.FromArgb(0, 0, 0);
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
            beepButton1.DrawingRect = new Rectangle(4, 4, 116, 26);
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
            beepButton1.HoverBackColor = Color.FromArgb(0, 255, 255);
            beepButton1.HoverBorderColor = Color.Gray;
            beepButton1.HoveredBackcolor = Color.Wheat;
            beepButton1.HoverForeColor = Color.Black;
            beepButton1.Id = -1;
            beepButton1.Image = null;
            beepButton1.ImageAlign = ContentAlignment.MiddleLeft;
            beepButton1.ImageClicked = null;
            beepButton1.ImagePath = null;
            beepButton1.InactiveBackColor = Color.Gray;
            beepButton1.InactiveBorderColor = Color.Gray;
            beepButton1.InactiveForeColor = Color.Black;
            beepButton1.IsAcceptButton = false;
            beepButton1.IsBorderAffectedByTheme = true;
            beepButton1.IsCancelButton = false;
            beepButton1.IsChild = false;
            beepButton1.IsCustomeBorder = false;
            beepButton1.IsDefault = false;
            beepButton1.IsFocused = false;
            beepButton1.IsFramless = false;
            beepButton1.IsHovered = false;
            beepButton1.IsPressed = false;
            beepButton1.IsRounded = false;
            beepButton1.IsRoundedAffectedByTheme = true;
            beepButton1.IsSelected = false;
            beepButton1.IsShadowAffectedByTheme = true;
            beepButton1.IsSideMenuChild = false;
            beepButton1.IsStillButton = false;
            beepButton1.Location = new Point(618, 353);
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
            beepButton1.ShadowOffset = 3;
            beepButton1.ShadowOpacity = 0.5F;
            beepButton1.ShowAllBorders = true;
            beepButton1.ShowBottomBorder = true;
            beepButton1.ShowFocusIndicator = false;
            beepButton1.ShowLeftBorder = true;
            beepButton1.ShowRightBorder = true;
            beepButton1.ShowShadow = true;
            beepButton1.ShowTopBorder = true;
            beepButton1.Size = new Size(124, 34);
            beepButton1.SlideFrom = SlideDirection.Left;
            beepButton1.StaticNotMoving = false;
            beepButton1.TabIndex = 4;
            beepButton1.Text = "beepButton1";
            beepButton1.TextAlign = ContentAlignment.MiddleCenter;
            beepButton1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepButton1.Theme = Vis.Modules.EnumBeepThemes.RetroTheme;
            beepButton1.ToolTipText = "";
            beepButton1.UseGradientBackground = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(255, 105, 180);
            ClientSize = new Size(800, 543);
            Controls.Add(beepButton1);
            Controls.Add(beepCheckBox1);
            Controls.Add(beepListBox1);
            Controls.Add(beepAppBar1);
            Controls.Add(beepSideMenu1);
            Name = "Form1";
            Text = "Form1";
            Theme = Vis.Modules.EnumBeepThemes.RetroTheme;
            ResumeLayout(false);
        }

        #endregion

        private BeepAppBar beepAppBar1;
        private BeepSideMenu beepSideMenu1;
        private BeepListBox beepListBox1;
        private BeepButton beepButton1;
        private BeepCheckBox beepCheckBox1;
    }
}