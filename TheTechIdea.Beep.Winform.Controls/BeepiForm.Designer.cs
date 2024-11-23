﻿namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepiForm
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
            components = new System.ComponentModel.Container();
            CloseButton = new BeepButton();
            MaximizeButton = new BeepButton();
            MinimizeButton = new BeepButton();
            beepuiManager1 = new BeepUIManager(components);
            beepPanel1 = new BeepPanel();
            TitleLabel = new BeepLabel();
            beepPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // CloseButton
            // 
            CloseButton.ActiveBackColor = Color.Transparent;
            CloseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CloseButton.AnimationDuration = 500;
            CloseButton.AnimationType = DisplayAnimationType.None;
            CloseButton.ApplyThemeOnImage = false;
            CloseButton.BackColor = SystemColors.Control;
            CloseButton.BlockID = null;
            CloseButton.BorderColor = Color.Black;
            CloseButton.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            CloseButton.BorderRadius = 5;
            CloseButton.BorderSize = 1;
            CloseButton.BorderStyle = BorderStyle.None;
            CloseButton.BorderThickness = 1;
            CloseButton.DataContext = null;
            CloseButton.DisabledBackColor = Color.Gray;
            CloseButton.DisabledForeColor = Color.Empty;
            CloseButton.DrawingRect = new Rectangle(1, 1, 18, 18);
            CloseButton.Easing = EasingType.Linear;
            CloseButton.FieldID = null;
            CloseButton.FlatAppearance = true;
            CloseButton.FlatStyle = FlatStyle.Standard;
            CloseButton.FocusBackColor = Color.Transparent;
            CloseButton.FocusBorderColor = Color.Transparent;
            CloseButton.FocusForeColor = Color.Black;
            CloseButton.FocusIndicatorColor = Color.Blue;
            CloseButton.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            CloseButton.ForeColor = Color.White;
            CloseButton.Form = null;
            CloseButton.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            CloseButton.GradientEndColor = Color.Transparent;
            CloseButton.GradientStartColor = Color.Transparent;
            CloseButton.HideText = false;
            CloseButton.HoverBackColor = Color.Transparent;
            CloseButton.HoverBorderColor = Color.Transparent;
            CloseButton.HoveredBackcolor = Color.Transparent;
            CloseButton.HoverForeColor = Color.Black;
            CloseButton.Id = -1;
            CloseButton.Image = null;
            CloseButton.ImageAlign = ContentAlignment.MiddleCenter;
            CloseButton.ImageClicked = null;
            CloseButton.ImagePath = "";
            CloseButton.InactiveBackColor = Color.Transparent;
            CloseButton.InactiveBorderColor = Color.Transparent;
            CloseButton.InactiveForeColor = Color.Black;
            CloseButton.IsAcceptButton = false;
            CloseButton.IsBorderAffectedByTheme = true;
            CloseButton.IsCancelButton = false;
            CloseButton.IsChild = true;
            CloseButton.IsCustomeBorder = false;
            CloseButton.IsDefault = false;
            CloseButton.IsFocused = false;
            CloseButton.IsFramless = true;
            CloseButton.IsHovered = false;
            CloseButton.IsPressed = false;
            CloseButton.IsRounded = false;
            CloseButton.IsSelected = false;
            CloseButton.IsShadowAffectedByTheme = true;
            CloseButton.IsSideMenuChild = false;
            CloseButton.IsStillButton = false;
            CloseButton.Location = new Point(743, 4);
            CloseButton.Margin = new Padding(0);
            CloseButton.MaxImageSize = new Size(20, 20);
            CloseButton.Name = "CloseButton";
            CloseButton.OverrideFontSize = TypeStyleFontSize.None;
            CloseButton.Padding = new Padding(1);
            CloseButton.ParentBackColor = SystemColors.Control;
            CloseButton.PressedBackColor = Color.Transparent;
            CloseButton.PressedBorderColor = Color.Transparent;
            CloseButton.PressedForeColor = Color.Black;
            CloseButton.SavedGuidID = null;
            CloseButton.SavedID = null;
            CloseButton.SelectedBorderColor = Color.Blue;
            CloseButton.ShadowColor = Color.Black;
            CloseButton.ShadowOffset = 0;
            CloseButton.ShadowOpacity = 0.5F;
            CloseButton.ShowAllBorders = true;
            CloseButton.ShowBottomBorder = true;
            CloseButton.ShowFocusIndicator = false;
            CloseButton.ShowLeftBorder = true;
            CloseButton.ShowRightBorder = true;
            CloseButton.ShowShadow = false;
            CloseButton.ShowTopBorder = true;
            CloseButton.Size = new Size(20, 20);
            CloseButton.SlideFrom = SlideDirection.Left;
            CloseButton.StaticNotMoving = false;
            CloseButton.TabIndex = 0;
            CloseButton.Text = "X";
            CloseButton.TextAlign = ContentAlignment.MiddleCenter;
            CloseButton.TextImageRelation = TextImageRelation.ImageAboveText;
            CloseButton.Theme = Vis.Modules.EnumBeepThemes.MaterialDesignTheme;
            CloseButton.ToolTipText = "";
            CloseButton.UseGradientBackground = false;
            // 
            // MaximizeButton
            // 
            MaximizeButton.ActiveBackColor = Color.Transparent;
            MaximizeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MaximizeButton.AnimationDuration = 500;
            MaximizeButton.AnimationType = DisplayAnimationType.None;
            MaximizeButton.ApplyThemeOnImage = false;
            MaximizeButton.BackColor = SystemColors.Control;
            MaximizeButton.BlockID = null;
            MaximizeButton.BorderColor = Color.Black;
            MaximizeButton.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            MaximizeButton.BorderRadius = 5;
            MaximizeButton.BorderSize = 1;
            MaximizeButton.BorderStyle = BorderStyle.None;
            MaximizeButton.BorderThickness = 1;
            MaximizeButton.DataContext = null;
            MaximizeButton.DisabledBackColor = Color.Gray;
            MaximizeButton.DisabledForeColor = Color.Empty;
            MaximizeButton.DrawingRect = new Rectangle(1, 1, 18, 18);
            MaximizeButton.Easing = EasingType.Linear;
            MaximizeButton.FieldID = null;
            MaximizeButton.FlatAppearance = true;
            MaximizeButton.FlatStyle = FlatStyle.Standard;
            MaximizeButton.FocusBackColor = Color.Transparent;
            MaximizeButton.FocusBorderColor = Color.Transparent;
            MaximizeButton.FocusForeColor = Color.Black;
            MaximizeButton.FocusIndicatorColor = Color.Blue;
            MaximizeButton.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            MaximizeButton.ForeColor = Color.White;
            MaximizeButton.Form = null;
            MaximizeButton.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            MaximizeButton.GradientEndColor = Color.Transparent;
            MaximizeButton.GradientStartColor = Color.Transparent;
            MaximizeButton.HideText = false;
            MaximizeButton.HoverBackColor = Color.Transparent;
            MaximizeButton.HoverBorderColor = Color.Transparent;
            MaximizeButton.HoveredBackcolor = Color.Transparent;
            MaximizeButton.HoverForeColor = Color.Black;
            MaximizeButton.Id = -1;
            MaximizeButton.Image = null;
            MaximizeButton.ImageAlign = ContentAlignment.MiddleCenter;
            MaximizeButton.ImageClicked = null;
            MaximizeButton.ImagePath = null;
            MaximizeButton.InactiveBackColor = Color.Transparent;
            MaximizeButton.InactiveBorderColor = Color.Transparent;
            MaximizeButton.InactiveForeColor = Color.Black;
            MaximizeButton.IsAcceptButton = false;
            MaximizeButton.IsBorderAffectedByTheme = true;
            MaximizeButton.IsCancelButton = false;
            MaximizeButton.IsChild = true;
            MaximizeButton.IsCustomeBorder = false;
            MaximizeButton.IsDefault = false;
            MaximizeButton.IsFocused = false;
            MaximizeButton.IsFramless = true;
            MaximizeButton.IsHovered = false;
            MaximizeButton.IsPressed = false;
            MaximizeButton.IsRounded = false;
            MaximizeButton.IsSelected = false;
            MaximizeButton.IsShadowAffectedByTheme = true;
            MaximizeButton.IsSideMenuChild = false;
            MaximizeButton.IsStillButton = false;
            MaximizeButton.Location = new Point(717, 4);
            MaximizeButton.Margin = new Padding(0);
            MaximizeButton.MaxImageSize = new Size(32, 32);
            MaximizeButton.Name = "MaximizeButton";
            MaximizeButton.OverrideFontSize = TypeStyleFontSize.None;
            MaximizeButton.Padding = new Padding(1);
            MaximizeButton.ParentBackColor = SystemColors.Control;
            MaximizeButton.PressedBackColor = Color.Transparent;
            MaximizeButton.PressedBorderColor = Color.Transparent;
            MaximizeButton.PressedForeColor = Color.Black;
            MaximizeButton.SavedGuidID = null;
            MaximizeButton.SavedID = null;
            MaximizeButton.SelectedBorderColor = Color.Blue;
            MaximizeButton.ShadowColor = Color.Black;
            MaximizeButton.ShadowOffset = 0;
            MaximizeButton.ShadowOpacity = 0.5F;
            MaximizeButton.ShowAllBorders = true;
            MaximizeButton.ShowBottomBorder = true;
            MaximizeButton.ShowFocusIndicator = false;
            MaximizeButton.ShowLeftBorder = true;
            MaximizeButton.ShowRightBorder = true;
            MaximizeButton.ShowShadow = false;
            MaximizeButton.ShowTopBorder = true;
            MaximizeButton.Size = new Size(20, 20);
            MaximizeButton.SlideFrom = SlideDirection.Left;
            MaximizeButton.StaticNotMoving = false;
            MaximizeButton.TabIndex = 1;
            MaximizeButton.Text = "◱";
            MaximizeButton.TextAlign = ContentAlignment.MiddleCenter;
            MaximizeButton.TextImageRelation = TextImageRelation.ImageAboveText;
            MaximizeButton.Theme = Vis.Modules.EnumBeepThemes.MaterialDesignTheme;
            MaximizeButton.ToolTipText = "";
            MaximizeButton.UseGradientBackground = false;
            // 
            // MinimizeButton
            // 
            MinimizeButton.ActiveBackColor = Color.Transparent;
            MinimizeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MinimizeButton.AnimationDuration = 500;
            MinimizeButton.AnimationType = DisplayAnimationType.None;
            MinimizeButton.ApplyThemeOnImage = false;
            MinimizeButton.BackColor = SystemColors.Control;
            MinimizeButton.BlockID = null;
            MinimizeButton.BorderColor = Color.Black;
            MinimizeButton.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            MinimizeButton.BorderRadius = 5;
            MinimizeButton.BorderSize = 1;
            MinimizeButton.BorderStyle = BorderStyle.None;
            MinimizeButton.BorderThickness = 1;
            MinimizeButton.DataContext = null;
            MinimizeButton.DisabledBackColor = Color.Gray;
            MinimizeButton.DisabledForeColor = Color.Empty;
            MinimizeButton.DrawingRect = new Rectangle(1, 1, 18, 18);
            MinimizeButton.Easing = EasingType.Linear;
            MinimizeButton.FieldID = null;
            MinimizeButton.FlatAppearance = true;
            MinimizeButton.FlatStyle = FlatStyle.Standard;
            MinimizeButton.FocusBackColor = Color.Transparent;
            MinimizeButton.FocusBorderColor = Color.Transparent;
            MinimizeButton.FocusForeColor = Color.Black;
            MinimizeButton.FocusIndicatorColor = Color.Blue;
            MinimizeButton.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            MinimizeButton.ForeColor = Color.White;
            MinimizeButton.Form = null;
            MinimizeButton.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            MinimizeButton.GradientEndColor = Color.Transparent;
            MinimizeButton.GradientStartColor = Color.Transparent;
            MinimizeButton.HideText = false;
            MinimizeButton.HoverBackColor = Color.Transparent;
            MinimizeButton.HoverBorderColor = Color.Transparent;
            MinimizeButton.HoveredBackcolor = Color.Transparent;
            MinimizeButton.HoverForeColor = Color.Black;
            MinimizeButton.Id = -1;
            MinimizeButton.Image = null;
            MinimizeButton.ImageAlign = ContentAlignment.MiddleCenter;
            MinimizeButton.ImageClicked = null;
            MinimizeButton.ImagePath = null;
            MinimizeButton.InactiveBackColor = Color.Transparent;
            MinimizeButton.InactiveBorderColor = Color.Transparent;
            MinimizeButton.InactiveForeColor = Color.Black;
            MinimizeButton.IsAcceptButton = false;
            MinimizeButton.IsBorderAffectedByTheme = true;
            MinimizeButton.IsCancelButton = false;
            MinimizeButton.IsChild = true;
            MinimizeButton.IsCustomeBorder = false;
            MinimizeButton.IsDefault = false;
            MinimizeButton.IsFocused = false;
            MinimizeButton.IsFramless = true;
            MinimizeButton.IsHovered = false;
            MinimizeButton.IsPressed = false;
            MinimizeButton.IsRounded = false;
            MinimizeButton.IsSelected = false;
            MinimizeButton.IsShadowAffectedByTheme = true;
            MinimizeButton.IsSideMenuChild = false;
            MinimizeButton.IsStillButton = false;
            MinimizeButton.Location = new Point(691, 4);
            MinimizeButton.Margin = new Padding(0);
            MinimizeButton.MaxImageSize = new Size(32, 32);
            MinimizeButton.Name = "MinimizeButton";
            MinimizeButton.OverrideFontSize = TypeStyleFontSize.None;
            MinimizeButton.Padding = new Padding(1);
            MinimizeButton.ParentBackColor = SystemColors.Control;
            MinimizeButton.PressedBackColor = Color.Transparent;
            MinimizeButton.PressedBorderColor = Color.Transparent;
            MinimizeButton.PressedForeColor = Color.Black;
            MinimizeButton.SavedGuidID = null;
            MinimizeButton.SavedID = null;
            MinimizeButton.SelectedBorderColor = Color.Blue;
            MinimizeButton.ShadowColor = Color.Black;
            MinimizeButton.ShadowOffset = 0;
            MinimizeButton.ShadowOpacity = 0.5F;
            MinimizeButton.ShowAllBorders = true;
            MinimizeButton.ShowBottomBorder = true;
            MinimizeButton.ShowFocusIndicator = false;
            MinimizeButton.ShowLeftBorder = true;
            MinimizeButton.ShowRightBorder = true;
            MinimizeButton.ShowShadow = false;
            MinimizeButton.ShowTopBorder = true;
            MinimizeButton.Size = new Size(20, 20);
            MinimizeButton.SlideFrom = SlideDirection.Left;
            MinimizeButton.StaticNotMoving = false;
            MinimizeButton.TabIndex = 2;
            MinimizeButton.Text = "--";
            MinimizeButton.TextAlign = ContentAlignment.MiddleCenter;
            MinimizeButton.TextImageRelation = TextImageRelation.ImageAboveText;
            MinimizeButton.Theme = Vis.Modules.EnumBeepThemes.MaterialDesignTheme;
            MinimizeButton.ToolTipText = "";
            MinimizeButton.UseGradientBackground = false;
            // 
            // beepuiManager1
            // 
            beepuiManager1.ApplyThemeOnImage = false;
            beepuiManager1.BeepiForm = null;
            beepuiManager1.BeepSideMenu = null;
            beepuiManager1.IsRounded = false;
            beepuiManager1.LogoImage = "";
            beepuiManager1.ShowBorder = true;
            beepuiManager1.ShowShadow = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.MaterialDesignTheme;
            beepuiManager1.Title = "Beep Form";
            // 
            // beepPanel1
            // 
            beepPanel1.ActiveBackColor = Color.FromArgb(205, 133, 63);
            beepPanel1.AnimationDuration = 500;
            beepPanel1.AnimationType = DisplayAnimationType.None;
            beepPanel1.BackColor = SystemColors.Control;
            beepPanel1.BlockID = null;
            beepPanel1.BorderColor = Color.FromArgb(205, 133, 63);
            beepPanel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepPanel1.BorderRadius = 1;
            beepPanel1.BorderStyle = BorderStyle.None;
            beepPanel1.BorderThickness = 1;
            beepPanel1.Controls.Add(TitleLabel);
            beepPanel1.Controls.Add(MaximizeButton);
            beepPanel1.Controls.Add(CloseButton);
            beepPanel1.Controls.Add(MinimizeButton);
            beepPanel1.DataContext = null;
            beepPanel1.DisabledBackColor = Color.Gray;
            beepPanel1.DisabledForeColor = Color.Empty;
            beepPanel1.Dock = DockStyle.Top;
            beepPanel1.DrawingRect = new Rectangle(1, 1, 772, 34);
            beepPanel1.Easing = EasingType.Linear;
            beepPanel1.FieldID = null;
            beepPanel1.FocusBackColor = Color.Transparent;
            beepPanel1.FocusBorderColor = Color.Transparent;
            beepPanel1.FocusForeColor = Color.Black;
            beepPanel1.FocusIndicatorColor = Color.Blue;
            beepPanel1.ForeColor = Color.Black;
            beepPanel1.Form = null;
            beepPanel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepPanel1.GradientEndColor = Color.FromArgb(222, 184, 135);
            beepPanel1.GradientStartColor = Color.FromArgb(245, 245, 220);
            beepPanel1.HoverBackColor = Color.FromArgb(222, 184, 135);
            beepPanel1.HoverBorderColor = Color.Transparent;
            beepPanel1.HoveredBackcolor = Color.Transparent;
            beepPanel1.HoverForeColor = Color.Black;
            beepPanel1.Id = -1;
            beepPanel1.InactiveBackColor = Color.Transparent;
            beepPanel1.InactiveBorderColor = Color.Transparent;
            beepPanel1.InactiveForeColor = Color.Black;
            beepPanel1.IsAcceptButton = false;
            beepPanel1.IsBorderAffectedByTheme = false;
            beepPanel1.IsCancelButton = false;
            beepPanel1.IsChild = true;
            beepPanel1.IsCustomeBorder = false;
            beepPanel1.IsDefault = false;
            beepPanel1.IsFocused = false;
            beepPanel1.IsFramless = false;
            beepPanel1.IsHovered = false;
            beepPanel1.IsPressed = false;
            beepPanel1.IsRounded = false;
            beepPanel1.IsShadowAffectedByTheme = false;
            beepPanel1.Location = new Point(0, 0);
            beepPanel1.Name = "beepPanel1";
            beepPanel1.OverrideFontSize = TypeStyleFontSize.None;
            beepPanel1.ParentBackColor = SystemColors.Control;
            beepPanel1.PressedBackColor = Color.Transparent;
            beepPanel1.PressedBorderColor = Color.Transparent;
            beepPanel1.PressedForeColor = Color.Black;
            beepPanel1.SavedGuidID = null;
            beepPanel1.SavedID = null;
            beepPanel1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepPanel1.ShadowOffset = 0;
            beepPanel1.ShadowOpacity = 0.5F;
            beepPanel1.ShowAllBorders = true;
            beepPanel1.ShowBottomBorder = true;
            beepPanel1.ShowFocusIndicator = false;
            beepPanel1.ShowLeftBorder = true;
            beepPanel1.ShowRightBorder = true;
            beepPanel1.ShowShadow = false;
            beepPanel1.ShowTitle = false;
            beepPanel1.ShowTitleLine = false;
            beepPanel1.ShowTitleLineinFullWidth = true;
            beepPanel1.ShowTopBorder = true;
            beepPanel1.Size = new Size(774, 36);
            beepPanel1.SlideFrom = SlideDirection.Left;
            beepPanel1.StaticNotMoving = false;
            beepPanel1.TabIndex = 4;
            beepPanel1.Text = "beepPanel1";
            beepPanel1.Theme = Vis.Modules.EnumBeepThemes.MaterialDesignTheme;
            beepPanel1.TitleAlignment = ContentAlignment.TopLeft;
            beepPanel1.TitleBottomY = 0;
            beepPanel1.TitleLineColor = Color.Gray;
            beepPanel1.TitleLineThickness = 2;
            beepPanel1.TitleText = "Panel Title";
            beepPanel1.ToolTipText = "";
            beepPanel1.UseGradientBackground = false;
            // 
            // TitleLabel
            // 
            TitleLabel.ActiveBackColor = Color.FromArgb(255, 20, 147);
            TitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TitleLabel.AnimationDuration = 500;
            TitleLabel.AnimationType = DisplayAnimationType.None;
            TitleLabel.ApplyThemeOnImage = false;
            TitleLabel.BackColor = SystemColors.Control;
            TitleLabel.BlockID = null;
            TitleLabel.BorderColor = Color.FromArgb(0, 255, 255);
            TitleLabel.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            TitleLabel.BorderRadius = 5;
            TitleLabel.BorderStyle = BorderStyle.None;
            TitleLabel.BorderThickness = 1;
            TitleLabel.DataContext = null;
            TitleLabel.DisabledBackColor = Color.Gray;
            TitleLabel.DisabledForeColor = Color.Empty;
            TitleLabel.DrawingRect = new Rectangle(1, 1, 185, 26);
            TitleLabel.Easing = EasingType.Linear;
            TitleLabel.FieldID = null;
            TitleLabel.FocusBackColor = Color.Black;
            TitleLabel.FocusBorderColor = Color.Transparent;
            TitleLabel.FocusForeColor = Color.Black;
            TitleLabel.FocusIndicatorColor = Color.Blue;
            TitleLabel.Font = new Font("Segoe UI", 12F);
            TitleLabel.ForeColor = Color.FromArgb(33, 150, 243);
            TitleLabel.Form = null;
            TitleLabel.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            TitleLabel.GradientEndColor = Color.FromArgb(30, 30, 30);
            TitleLabel.GradientStartColor = Color.FromArgb(0, 0, 0);
            TitleLabel.HideText = false;
            TitleLabel.HoverBackColor = Color.FromArgb(0, 255, 127);
            TitleLabel.HoverBorderColor = Color.FromArgb(0, 255, 127);
            TitleLabel.HoveredBackcolor = Color.Transparent;
            TitleLabel.HoverForeColor = Color.Black;
            TitleLabel.Id = -1;
            TitleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            TitleLabel.ImagePath = "";
            TitleLabel.InactiveBackColor = Color.Transparent;
            TitleLabel.InactiveBorderColor = Color.Transparent;
            TitleLabel.InactiveForeColor = Color.Black;
            TitleLabel.IsAcceptButton = false;
            TitleLabel.IsBorderAffectedByTheme = true;
            TitleLabel.IsCancelButton = false;
            TitleLabel.IsChild = true;
            TitleLabel.IsCustomeBorder = false;
            TitleLabel.IsDefault = false;
            TitleLabel.IsFocused = false;
            TitleLabel.IsFramless = true;
            TitleLabel.IsHovered = false;
            TitleLabel.IsPressed = false;
            TitleLabel.IsRounded = false;
            TitleLabel.IsShadowAffectedByTheme = true;
            TitleLabel.Location = new Point(12, 5);
            TitleLabel.Margin = new Padding(0);
            TitleLabel.MaxImageSize = new Size(16, 16);
            TitleLabel.Name = "TitleLabel";
            TitleLabel.OverrideFontSize = TypeStyleFontSize.None;
            TitleLabel.Padding = new Padding(2);
            TitleLabel.ParentBackColor = SystemColors.Control;
            TitleLabel.PressedBackColor = Color.FromArgb(255, 20, 147);
            TitleLabel.PressedBorderColor = Color.Transparent;
            TitleLabel.PressedForeColor = Color.Black;
            TitleLabel.SavedGuidID = null;
            TitleLabel.SavedID = null;
            TitleLabel.ShadowColor = Color.FromArgb(100, 0, 255, 255);
            TitleLabel.ShadowOffset = 0;
            TitleLabel.ShadowOpacity = 0.5F;
            TitleLabel.ShowAllBorders = true;
            TitleLabel.ShowBottomBorder = true;
            TitleLabel.ShowFocusIndicator = false;
            TitleLabel.ShowLeftBorder = true;
            TitleLabel.ShowRightBorder = true;
            TitleLabel.ShowShadow = false;
            TitleLabel.ShowTopBorder = true;
            TitleLabel.Size = new Size(187, 28);
            TitleLabel.SlideFrom = SlideDirection.Left;
            TitleLabel.StaticNotMoving = false;
            TitleLabel.TabIndex = 5;
            TitleLabel.Text = "beepLabel1";
            TitleLabel.TextAlign = ContentAlignment.MiddleLeft;
            TitleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            TitleLabel.Theme = Vis.Modules.EnumBeepThemes.MaterialDesignTheme;
            TitleLabel.ToolTipText = "";
            TitleLabel.UseGradientBackground = false;
            // 
            // BeepiForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(774, 644);
            Controls.Add(beepPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "BeepiForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Beep i Form";
            beepPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        public BeepButton CloseButton;
        public BeepButton MaximizeButton;
        public BeepButton MinimizeButton;
        public BeepUIManager beepuiManager1;
        public BeepPanel beepPanel1;
        public BeepLabel TitleLabel;
    }
}