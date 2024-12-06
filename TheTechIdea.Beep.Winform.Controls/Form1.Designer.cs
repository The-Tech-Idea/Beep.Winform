﻿using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls
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
            beepAppBar1 = new BeepAppBar();
            beepSideMenu1 = new BeepSideMenu();
            beepDatePicker1 = new BeepDatePicker();
            beepTabs1 = new BeepTabs();
            tabPage1 = new TabPage();
            beepButton1 = new BeepButton();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            beepButton2 = new BeepButton();
            tabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepAppBar = beepAppBar1;
            beepuiManager1.BeepiForm = this;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.IsRounded = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.SpringTheme;
            // 
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = DisplayAnimationType.None;
            beepAppBar1.ApplyThemeOnImage = false;
            beepAppBar1.ApplyThemeToChilds = true;
            beepAppBar1.BackColor = Color.FromArgb(60, 179, 113);
            beepAppBar1.BlockID = null;
            beepAppBar1.BorderColor = Color.Black;
            beepAppBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepAppBar1.BorderRadius = 5;
            beepAppBar1.BorderStyle = BorderStyle.FixedSingle;
            beepAppBar1.BorderThickness = 1;
            beepAppBar1.BottomoffsetForDrawingRect = 0;
            beepAppBar1.BoundProperty = null;
            beepAppBar1.DataContext = null;
            beepAppBar1.DisabledBackColor = Color.Gray;
            beepAppBar1.DisabledForeColor = Color.Empty;
            beepAppBar1.Dock = DockStyle.Top;
            beepAppBar1.DrawingRect = new Rectangle(1, 1, 755, 33);
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
            beepAppBar1.GuidID = "bc4c3d66-5ced-4b3a-8fda-27110b8fd43d";
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
            beepAppBar1.IsFramless = true;
            beepAppBar1.IsHovered = false;
            beepAppBar1.IsPressed = false;
            beepAppBar1.IsRounded = false;
            beepAppBar1.IsRoundedAffectedByTheme = false;
            beepAppBar1.IsShadowAffectedByTheme = false;
            beepAppBar1.LeftoffsetForDrawingRect = 0;
            beepAppBar1.Location = new Point(305, 5);
            beepAppBar1.Name = "beepAppBar1";
            beepAppBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepAppBar1.ParentBackColor = Color.Empty;
            beepAppBar1.PressedBackColor = Color.Gray;
            beepAppBar1.PressedBorderColor = Color.Gray;
            beepAppBar1.PressedForeColor = Color.Black;
            beepAppBar1.RightoffsetForDrawingRect = 0;
            beepAppBar1.SavedGuidID = null;
            beepAppBar1.SavedID = null;
            beepAppBar1.ShadowColor = Color.Black;
            beepAppBar1.ShadowOffset = 0;
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
            beepAppBar1.ShowShadow = false;
            beepAppBar1.ShowTopBorder = true;
            beepAppBar1.SideMenu = null;
            beepAppBar1.Size = new Size(757, 35);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 0;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.SpringTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.TopoffsetForDrawingRect = 0;
            beepAppBar1.UseGradientBackground = false;
            beepAppBar1.Click += beepAppBar1_Click;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(46, 139, 87);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationStep = 20;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.ApplyThemeOnImages = false;
            beepSideMenu1.ApplyThemeToChilds = false;
            beepSideMenu1.BackColor = Color.FromArgb(240, 255, 240);
            beepSideMenu1.BeepAppBar = null;
            beepSideMenu1.BeepForm = this;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(144, 238, 144);
            beepSideMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSideMenu1.BorderRadius = 5;
            beepSideMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepSideMenu1.BorderThickness = 1;
            beepSideMenu1.BottomoffsetForDrawingRect = 0;
            beepSideMenu1.BoundProperty = null;
            beepSideMenu1.CollapsedWidth = 64;
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(6, 6, 288, 567);
            beepSideMenu1.Easing = EasingType.Linear;
            beepSideMenu1.ExpandedWidth = 300;
            beepSideMenu1.FieldID = null;
            beepSideMenu1.FocusBackColor = Color.White;
            beepSideMenu1.FocusBorderColor = Color.Gray;
            beepSideMenu1.FocusForeColor = Color.Black;
            beepSideMenu1.FocusIndicatorColor = Color.Blue;
            beepSideMenu1.Font = new Font("Segoe UI", 9F);
            beepSideMenu1.ForeColor = Color.White;
            beepSideMenu1.Form = null;
            beepSideMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSideMenu1.GradientEndColor = Color.FromArgb(144, 238, 144);
            beepSideMenu1.GradientStartColor = Color.FromArgb(245, 255, 250);
            beepSideMenu1.GuidID = "ba5081bb-bcd5-4ac1-abbf-4e18ae223ecc";
            beepSideMenu1.HilightPanelSize = 5;
            beepSideMenu1.HoverBackColor = Color.FromArgb(60, 179, 113);
            beepSideMenu1.HoverBorderColor = Color.FromArgb(34, 139, 34);
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
            beepSideMenu1.IsFramless = true;
            beepSideMenu1.IsHovered = false;
            beepSideMenu1.IsPressed = false;
            beepSideMenu1.IsRounded = false;
            beepSideMenu1.IsRoundedAffectedByTheme = true;
            beepSideMenu1.IsShadowAffectedByTheme = false;
            beepSideMenu1.LeftoffsetForDrawingRect = 0;
            beepSideMenu1.Location = new Point(5, 5);
            beepSideMenu1.LogoImage = "";
            beepSideMenu1.Name = "beepSideMenu1";
            beepSideMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepSideMenu1.Padding = new Padding(5);
            beepSideMenu1.ParentBackColor = Color.FromArgb(245, 255, 250);
            beepSideMenu1.PressedBackColor = Color.FromArgb(46, 139, 87);
            beepSideMenu1.PressedBorderColor = Color.Gray;
            beepSideMenu1.PressedForeColor = Color.Black;
            beepSideMenu1.RightoffsetForDrawingRect = 0;
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
            beepSideMenu1.Size = new Size(300, 579);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 1;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.SpringTheme;
            beepSideMenu1.Title = "Beep Form";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.TopoffsetForDrawingRect = 0;
            beepSideMenu1.UseGradientBackground = false;
            // 
            // beepDatePicker1
            // 
            beepDatePicker1.ActiveBackColor = Color.FromArgb(65, 105, 225);
            beepDatePicker1.AnimationDuration = 500;
            beepDatePicker1.AnimationType = DisplayAnimationType.None;
            beepDatePicker1.ApplyThemeToChilds = true;
            beepDatePicker1.BlockID = null;
            beepDatePicker1.BorderColor = Color.FromArgb(176, 196, 222);
            beepDatePicker1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepDatePicker1.BorderRadius = 5;
            beepDatePicker1.BorderStyle = BorderStyle.FixedSingle;
            beepDatePicker1.BorderThickness = 1;
            beepDatePicker1.BottomoffsetForDrawingRect = 0;
            beepDatePicker1.BoundProperty = "SelectedDate";
            beepDatePicker1.DataContext = null;
            beepDatePicker1.DateFormat = "M/d/yyyy";
            beepDatePicker1.DisabledBackColor = Color.Gray;
            beepDatePicker1.DisabledForeColor = Color.Empty;
            beepDatePicker1.DrawingRect = new Rectangle(1, 1, 211, 20);
            beepDatePicker1.Easing = EasingType.Linear;
            beepDatePicker1.FieldID = null;
            beepDatePicker1.FocusBackColor = Color.White;
            beepDatePicker1.FocusBorderColor = Color.Gray;
            beepDatePicker1.FocusForeColor = Color.Black;
            beepDatePicker1.FocusIndicatorColor = Color.Blue;
            beepDatePicker1.Form = null;
            beepDatePicker1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepDatePicker1.GradientEndColor = Color.FromArgb(176, 196, 222);
            beepDatePicker1.GradientStartColor = Color.FromArgb(245, 245, 245);
            beepDatePicker1.GuidID = "cd4649df-e3c0-416b-966d-e38dd079a6fa";
            beepDatePicker1.HoverBackColor = Color.FromArgb(70, 130, 180);
            beepDatePicker1.HoverBorderColor = Color.FromArgb(65, 105, 225);
            beepDatePicker1.HoveredBackcolor = Color.Wheat;
            beepDatePicker1.HoverForeColor = Color.Black;
            beepDatePicker1.Id = -1;
            beepDatePicker1.InactiveBackColor = Color.Gray;
            beepDatePicker1.InactiveBorderColor = Color.Gray;
            beepDatePicker1.InactiveForeColor = Color.Black;
            beepDatePicker1.IsAcceptButton = false;
            beepDatePicker1.IsBorderAffectedByTheme = true;
            beepDatePicker1.IsCancelButton = false;
            beepDatePicker1.IsChild = false;
            beepDatePicker1.IsCustomeBorder = false;
            beepDatePicker1.IsDefault = false;
            beepDatePicker1.IsFocused = false;
            beepDatePicker1.IsFramless = false;
            beepDatePicker1.IsHovered = false;
            beepDatePicker1.IsPressed = false;
            beepDatePicker1.IsRounded = false;
            beepDatePicker1.IsRoundedAffectedByTheme = true;
            beepDatePicker1.IsShadowAffectedByTheme = true;
            beepDatePicker1.LeftoffsetForDrawingRect = 0;
            beepDatePicker1.Location = new Point(41, 79);
            beepDatePicker1.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            beepDatePicker1.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            beepDatePicker1.Name = "beepDatePicker1";
            beepDatePicker1.OverrideFontSize = TypeStyleFontSize.None;
            beepDatePicker1.ParentBackColor = Color.Empty;
            beepDatePicker1.PressedBackColor = Color.FromArgb(65, 105, 225);
            beepDatePicker1.PressedBorderColor = Color.Gray;
            beepDatePicker1.PressedForeColor = Color.Black;
            beepDatePicker1.RightoffsetForDrawingRect = 0;
            beepDatePicker1.SavedGuidID = null;
            beepDatePicker1.SavedID = null;
            beepDatePicker1.SelectedDate = "12/5/2024";
            beepDatePicker1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepDatePicker1.ShadowOffset = 0;
            beepDatePicker1.ShadowOpacity = 0.5F;
            beepDatePicker1.ShowAllBorders = true;
            beepDatePicker1.ShowBottomBorder = true;
            beepDatePicker1.ShowFocusIndicator = false;
            beepDatePicker1.ShowLeftBorder = true;
            beepDatePicker1.ShowRightBorder = true;
            beepDatePicker1.ShowShadow = false;
            beepDatePicker1.ShowTopBorder = true;
            beepDatePicker1.Size = new Size(213, 22);
            beepDatePicker1.SlideFrom = SlideDirection.Left;
            beepDatePicker1.StaticNotMoving = false;
            beepDatePicker1.TabIndex = 4;
            beepDatePicker1.Text = "beepDatePicker1";
            beepDatePicker1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepDatePicker1.ToolTipText = "";
            beepDatePicker1.TopoffsetForDrawingRect = 0;
            beepDatePicker1.UseGradientBackground = false;
            // 
            // beepTabs1
            // 
            beepTabs1.ActiveBackColor = Color.FromArgb(65, 105, 225);
            beepTabs1.AnimationDuration = 500;
            beepTabs1.AnimationType = DisplayAnimationType.None;
            beepTabs1.ApplyThemeToChilds = true;
            beepTabs1.BackColor = Color.FromArgb(144, 238, 144);
            beepTabs1.BlockID = null;
            beepTabs1.BorderColor = Color.FromArgb(176, 196, 222);
            beepTabs1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepTabs1.BorderRadius = 5;
            beepTabs1.BorderStyle = BorderStyle.FixedSingle;
            beepTabs1.BorderThickness = 1;
            beepTabs1.BottomoffsetForDrawingRect = 0;
            beepTabs1.BoundProperty = null;
            beepTabs1.DataContext = null;
            beepTabs1.DisabledBackColor = Color.Gray;
            beepTabs1.DisabledForeColor = Color.Empty;
            beepTabs1.DrawingRect = new Rectangle(1, 1, 463, 336);
            beepTabs1.Easing = EasingType.Linear;
            beepTabs1.FieldID = null;
            beepTabs1.FocusBackColor = Color.White;
            beepTabs1.FocusBorderColor = Color.Gray;
            beepTabs1.FocusForeColor = Color.Black;
            beepTabs1.FocusIndicatorColor = Color.Blue;
            beepTabs1.Form = null;
            beepTabs1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepTabs1.GradientEndColor = Color.FromArgb(176, 196, 222);
            beepTabs1.GradientStartColor = Color.FromArgb(245, 245, 245);
            beepTabs1.GuidID = "d579970f-09a6-4cee-a049-55df72defabc";
            beepTabs1.HeaderButtonSize = 30;
            beepTabs1.HoverBackColor = Color.FromArgb(70, 130, 180);
            beepTabs1.HoverBorderColor = Color.FromArgb(65, 105, 225);
            beepTabs1.HoveredBackcolor = Color.Wheat;
            beepTabs1.HoverForeColor = Color.Black;
            beepTabs1.Id = -1;
            beepTabs1.InactiveBackColor = Color.Gray;
            beepTabs1.InactiveBorderColor = Color.Gray;
            beepTabs1.InactiveForeColor = Color.Black;
            beepTabs1.IsAcceptButton = false;
            beepTabs1.IsBorderAffectedByTheme = true;
            beepTabs1.IsCancelButton = false;
            beepTabs1.IsChild = false;
            beepTabs1.IsCustomeBorder = false;
            beepTabs1.IsDefault = false;
            beepTabs1.IsFocused = false;
            beepTabs1.IsFramless = false;
            beepTabs1.IsHovered = false;
            beepTabs1.IsPressed = false;
            beepTabs1.IsRounded = false;
            beepTabs1.IsRoundedAffectedByTheme = true;
            beepTabs1.IsShadowAffectedByTheme = true;
            beepTabs1.LeftoffsetForDrawingRect = 0;
            beepTabs1.Location = new Point(401, 189);
            beepTabs1.MinimumSize = new Size(200, 100);
            beepTabs1.Name = "beepTabs1";
            beepTabs1.OverrideFontSize = TypeStyleFontSize.None;
            beepTabs1.ParentBackColor = Color.Empty;
            beepTabs1.PressedBackColor = Color.FromArgb(65, 105, 225);
            beepTabs1.PressedBorderColor = Color.Gray;
            beepTabs1.PressedForeColor = Color.Black;
            beepTabs1.RightoffsetForDrawingRect = 0;
            beepTabs1.SavedGuidID = null;
            beepTabs1.SavedID = null;
            beepTabs1.SelectedIndex = 0;
            beepTabs1.SelectedTab = tabPage1;
            beepTabs1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepTabs1.ShadowOffset = 0;
            beepTabs1.ShadowOpacity = 0.5F;
            beepTabs1.ShowAllBorders = true;
            beepTabs1.ShowBottomBorder = true;
            beepTabs1.ShowFocusIndicator = false;
            beepTabs1.ShowLeftBorder = true;
            beepTabs1.ShowRightBorder = true;
            beepTabs1.ShowShadow = false;
            beepTabs1.ShowTopBorder = true;
            beepTabs1.Size = new Size(465, 338);
            beepTabs1.SlideFrom = SlideDirection.Left;
            beepTabs1.StaticNotMoving = false;
            beepTabs1.TabIndex = 2;
            beepTabs1.TabPages.AddRange(new TabPage[] { tabPage1, tabPage2, tabPage3 });
            beepTabs1.Text = "beepTabs1";
            beepTabs1.Theme = Vis.Modules.EnumBeepThemes.SpringTheme;
            beepTabs1.ToolTipText = "";
            beepTabs1.TopoffsetForDrawingRect = 0;
            beepTabs1.UseGradientBackground = false;
            // 
            // tabPage1
            // 
            tabPage1.AllowDrop = true;
            tabPage1.BackColor = Color.FromArgb(245, 255, 250);
            tabPage1.Controls.Add(beepButton1);
            tabPage1.Location = new Point(0, 0);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(463, 306);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            // 
            // beepButton1
            // 
            beepButton1.ActiveBackColor = Color.Gray;
            beepButton1.AnimationDuration = 500;
            beepButton1.AnimationType = DisplayAnimationType.None;
            beepButton1.ApplyThemeOnImage = false;
            beepButton1.ApplyThemeToChilds = true;
            beepButton1.BackColor = Color.FromArgb(245, 255, 250);
            beepButton1.BlockID = null;
            beepButton1.BorderColor = Color.Black;
            beepButton1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepButton1.BorderRadius = 5;
            beepButton1.BorderSize = 1;
            beepButton1.BorderStyle = BorderStyle.FixedSingle;
            beepButton1.BorderThickness = 1;
            beepButton1.BottomoffsetForDrawingRect = 0;
            beepButton1.BoundProperty = null;
            beepButton1.DataContext = null;
            beepButton1.DisabledBackColor = Color.Gray;
            beepButton1.DisabledForeColor = Color.Empty;
            beepButton1.DrawingRect = new Rectangle(1, 1, 118, 45);
            beepButton1.Easing = EasingType.Linear;
            beepButton1.FieldID = null;
            beepButton1.FlatAppearance = true;
            beepButton1.FlatStyle = FlatStyle.Standard;
            beepButton1.FocusBackColor = Color.Gray;
            beepButton1.FocusBorderColor = Color.Gray;
            beepButton1.FocusForeColor = Color.Black;
            beepButton1.FocusIndicatorColor = Color.Blue;
            beepButton1.Font = new Font("Segoe UI", 12F);
            beepButton1.ForeColor = Color.Black;
            beepButton1.Form = null;
            beepButton1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepButton1.GradientEndColor = Color.Gray;
            beepButton1.GradientStartColor = Color.Gray;
            beepButton1.GuidID = "b45a2f80-f7e8-4305-9f24-80d26c24c3c7";
            beepButton1.HideText = false;
            beepButton1.HoverBackColor = Color.FromArgb(60, 179, 113);
            beepButton1.HoverBorderColor = Color.Gray;
            beepButton1.HoveredBackcolor = Color.Wheat;
            beepButton1.HoverForeColor = Color.White;
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
            beepButton1.LeftoffsetForDrawingRect = 0;
            beepButton1.Location = new Point(74, 160);
            beepButton1.Margin = new Padding(0);
            beepButton1.MaxImageSize = new Size(32, 32);
            beepButton1.Name = "beepButton1";
            beepButton1.OverrideFontSize = TypeStyleFontSize.None;
            beepButton1.ParentBackColor = Color.Empty;
            beepButton1.PressedBackColor = Color.Gray;
            beepButton1.PressedBorderColor = Color.Gray;
            beepButton1.PressedForeColor = Color.Black;
            beepButton1.RightoffsetForDrawingRect = 0;
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
            beepButton1.Size = new Size(120, 47);
            beepButton1.SlideFrom = SlideDirection.Left;
            beepButton1.StaticNotMoving = false;
            beepButton1.TabIndex = 3;
            beepButton1.Text = "beepButton1";
            beepButton1.TextAlign = ContentAlignment.MiddleCenter;
            beepButton1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepButton1.Theme = Vis.Modules.EnumBeepThemes.SpringTheme;
            beepButton1.ToolTipText = "";
            beepButton1.TopoffsetForDrawingRect = 0;
            beepButton1.UseGradientBackground = false;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.FromArgb(245, 255, 250);
            tabPage2.Location = new Point(0, 0);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(463, 306);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            // 
            // tabPage3
            // 
            tabPage3.BackColor = Color.FromArgb(245, 255, 250);
            tabPage3.Location = new Point(0, 0);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(463, 306);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "tabPage3";
            // 
            // beepButton2
            // 
            beepButton2.ActiveBackColor = Color.Gray;
            beepButton2.AnimationDuration = 500;
            beepButton2.AnimationType = DisplayAnimationType.None;
            beepButton2.ApplyThemeOnImage = false;
            beepButton2.ApplyThemeToChilds = true;
            beepButton2.BackColor = Color.FromArgb(245, 255, 250);
            beepButton2.BlockID = null;
            beepButton2.BorderColor = Color.Black;
            beepButton2.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepButton2.BorderRadius = 5;
            beepButton2.BorderSize = 1;
            beepButton2.BorderStyle = BorderStyle.FixedSingle;
            beepButton2.BorderThickness = 1;
            beepButton2.BottomoffsetForDrawingRect = 0;
            beepButton2.BoundProperty = null;
            beepButton2.DataContext = null;
            beepButton2.DisabledBackColor = Color.Gray;
            beepButton2.DisabledForeColor = Color.Empty;
            beepButton2.DrawingRect = new Rectangle(1, 1, 68, 13);
            beepButton2.Easing = EasingType.Linear;
            beepButton2.FieldID = null;
            beepButton2.FlatAppearance = true;
            beepButton2.FlatStyle = FlatStyle.Standard;
            beepButton2.FocusBackColor = Color.Gray;
            beepButton2.FocusBorderColor = Color.Gray;
            beepButton2.FocusForeColor = Color.Black;
            beepButton2.FocusIndicatorColor = Color.Blue;
            beepButton2.Font = new Font("Segoe UI", 12F);
            beepButton2.ForeColor = Color.Black;
            beepButton2.Form = null;
            beepButton2.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepButton2.GradientEndColor = Color.Gray;
            beepButton2.GradientStartColor = Color.Gray;
            beepButton2.GuidID = "aa637e2f-f2e5-4b98-97cf-23249528fad9";
            beepButton2.HideText = false;
            beepButton2.HoverBackColor = Color.FromArgb(60, 179, 113);
            beepButton2.HoverBorderColor = Color.Gray;
            beepButton2.HoveredBackcolor = Color.Wheat;
            beepButton2.HoverForeColor = Color.White;
            beepButton2.Id = -1;
            beepButton2.Image = null;
            beepButton2.ImageAlign = ContentAlignment.MiddleLeft;
            beepButton2.ImageClicked = null;
            beepButton2.ImagePath = null;
            beepButton2.InactiveBackColor = Color.Gray;
            beepButton2.InactiveBorderColor = Color.Gray;
            beepButton2.InactiveForeColor = Color.Black;
            beepButton2.IsAcceptButton = false;
            beepButton2.IsBorderAffectedByTheme = true;
            beepButton2.IsCancelButton = false;
            beepButton2.IsChild = false;
            beepButton2.IsCustomeBorder = false;
            beepButton2.IsDefault = false;
            beepButton2.IsFocused = false;
            beepButton2.IsFramless = false;
            beepButton2.IsHovered = false;
            beepButton2.IsPressed = false;
            beepButton2.IsRounded = false;
            beepButton2.IsRoundedAffectedByTheme = true;
            beepButton2.IsSelected = false;
            beepButton2.IsShadowAffectedByTheme = true;
            beepButton2.IsSideMenuChild = false;
            beepButton2.IsStillButton = false;
            beepButton2.LeftoffsetForDrawingRect = 0;
            beepButton2.Location = new Point(556, 119);
            beepButton2.Margin = new Padding(0);
            beepButton2.MaxImageSize = new Size(32, 32);
            beepButton2.Name = "beepButton2";
            beepButton2.OverrideFontSize = TypeStyleFontSize.None;
            beepButton2.ParentBackColor = Color.Empty;
            beepButton2.PressedBackColor = Color.Gray;
            beepButton2.PressedBorderColor = Color.Gray;
            beepButton2.PressedForeColor = Color.Black;
            beepButton2.RightoffsetForDrawingRect = 0;
            beepButton2.SavedGuidID = null;
            beepButton2.SavedID = null;
            beepButton2.SelectedBorderColor = Color.Blue;
            beepButton2.ShadowColor = Color.Black;
            beepButton2.ShadowOffset = 0;
            beepButton2.ShadowOpacity = 0.5F;
            beepButton2.ShowAllBorders = true;
            beepButton2.ShowBottomBorder = true;
            beepButton2.ShowFocusIndicator = false;
            beepButton2.ShowLeftBorder = true;
            beepButton2.ShowRightBorder = true;
            beepButton2.ShowShadow = false;
            beepButton2.ShowTopBorder = true;
            beepButton2.Size = new Size(70, 15);
            beepButton2.SlideFrom = SlideDirection.Left;
            beepButton2.StaticNotMoving = false;
            beepButton2.TabIndex = 3;
            beepButton2.Text = "beepButton2";
            beepButton2.TextAlign = ContentAlignment.MiddleCenter;
            beepButton2.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepButton2.Theme = Vis.Modules.EnumBeepThemes.SpringTheme;
            beepButton2.ToolTipText = "";
            beepButton2.TopoffsetForDrawingRect = 0;
            beepButton2.UseGradientBackground = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(144, 238, 144);
            ClientSize = new Size(1067, 589);
            Controls.Add(beepButton2);
            Controls.Add(beepTabs1);
            Controls.Add(beepAppBar1);
            Controls.Add(beepSideMenu1);
            Name = "Form1";
            Text = "Form1";
            Theme = Vis.Modules.EnumBeepThemes.SpringTheme;
            Load += Form1_Load;
            tabPage1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BeepAppBar beepAppBar1;
        private BeepSideMenu beepSideMenu1;
        private BeepDatePicker beepDatePicker1;
        private BeepTabs beepTabs1;
        private TabPage tabPage2;
        private TabPage tabPage1;
        private TabPage tabPage3;
        private BeepButton beepButton1;
        private BeepButton beepButton2;
    }
}