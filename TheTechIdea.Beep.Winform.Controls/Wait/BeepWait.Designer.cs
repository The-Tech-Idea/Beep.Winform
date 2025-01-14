﻿namespace TheTechIdea.Beep.Winform.Controls.Wait
{
    partial class BeepWait
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
            panel2 = new Panel();
            messege = new BeepTextBox();
            LogopictureBox = new BeepImage();
            _spinnerImage = new BeepImage();
            label2 = new BeepLabel();
            label1 = new BeepLabel();
            Title = new BeepLabel();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.ShowBorder = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.AutumnTheme;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(222, 210, 155);
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(messege);
            panel2.Controls.Add(LogopictureBox);
            panel2.Controls.Add(_spinnerImage);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(Title);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 3);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(463, 348);
            panel2.TabIndex = 3;
            panel2.UseWaitCursor = true;
            // 
            // messege
            // 
            messege.AcceptsReturn = false;
            messege.AcceptsTab = false;
            messege.ActiveBackColor = Color.Gray;
            messege.AnimationDuration = 500;
            messege.AnimationType = DisplayAnimationType.None;
            messege.ApplyThemeOnImage = false;
            messege.ApplyThemeToChilds = true;
            messege.AutoCompleteMode = AutoCompleteMode.None;
            messege.AutoCompleteSource = AutoCompleteSource.None;
            messege.BackColor = Color.FromArgb(200, 180, 135);
            messege.BlockID = null;
            messege.BorderColor = Color.Black;
            messege.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            messege.BorderRadius = 1;
            messege.BorderStyle = BorderStyle.FixedSingle;
            messege.BorderThickness = 1;
            messege.BottomoffsetForDrawingRect = 0;
            messege.BoundProperty = "Text";
            messege.CanBeFocused = true;
            messege.CanBeHovered = false;
            messege.CanBePressed = true;
            messege.Category = Utilities.DbFieldCategory.String;
            messege.ComponentName = "messege";
            messege.CustomMask = "";
            messege.DataContext = null;
            messege.DataSourceProperty = null;
            messege.DateFormat = "MM/dd/yyyy HH:mm:ss";
            messege.DateTimeFormat = "MM/dd/yyyy HH:mm:ss";
            messege.DisabledBackColor = Color.Gray;
            messege.DisabledForeColor = Color.Empty;
            messege.DrawingRect = new Rectangle(0, 0, 455, 174);
            messege.Easing = EasingType.Linear;
            messege.FieldID = null;
            messege.FocusBackColor = Color.Gray;
            messege.FocusBorderColor = Color.Gray;
            messege.FocusForeColor = Color.Black;
            messege.FocusIndicatorColor = Color.Blue;
            messege.Font = new Font("Segoe UI", 9F);
            messege.Form = null;
            messege.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            messege.GradientEndColor = Color.Gray;
            messege.GradientStartColor = Color.Gray;
            messege.GuidID = "b1abe803-d1b4-40cb-acae-e5333afa4a7c";
            messege.HideSelection = true;
            messege.HoverBackColor = Color.Gray;
            messege.HoverBorderColor = Color.Gray;
            messege.HoveredBackcolor = Color.Wheat;
            messege.HoverForeColor = Color.Black;
            messege.Id = -1;
            messege.ImageAlign = ContentAlignment.MiddleLeft;
            messege.ImagePath = null;
            messege.InactiveBackColor = Color.Gray;
            messege.InactiveBorderColor = Color.Gray;
            messege.InactiveForeColor = Color.Black;
            // 
            // 
            // 
            messege.InnerTextBox.BackColor = Color.FromArgb(222, 210, 155);
            messege.InnerTextBox.BorderStyle = BorderStyle.None;
            messege.InnerTextBox.Dock = DockStyle.Fill;
            messege.InnerTextBox.Font = new Font("Segoe UI", 9F);
            messege.InnerTextBox.ForeColor = Color.White;
            messege.InnerTextBox.Location = new Point(0, 0);
            messege.InnerTextBox.Multiline = true;
            messege.InnerTextBox.Name = "";
            messege.InnerTextBox.Size = new Size(455, 174);
            messege.InnerTextBox.TabIndex = 0;
            messege.InnerTextBox.UseWaitCursor = true;
            messege.IsAcceptButton = false;
            messege.IsBorderAffectedByTheme = true;
            messege.IsCancelButton = false;
            messege.IsChild = true;
            messege.IsCustomeBorder = false;
            messege.IsDefault = false;
            messege.IsFocused = false;
            messege.IsFramless = false;
            messege.IsHovered = false;
            messege.IsPressed = false;
            messege.IsRounded = true;
            messege.IsRoundedAffectedByTheme = true;
            messege.IsShadowAffectedByTheme = true;
            messege.LeftoffsetForDrawingRect = 0;
            messege.LinkedProperty = null;
            messege.Location = new Point(3, 145);
            messege.MaskFormat = Vis.Modules.TextBoxMaskFormat.None;
            messege.Modified = false;
            messege.Multiline = true;
            messege.Name = "messege";
            messege.OnlyCharacters = false;
            messege.OnlyDigits = false;
            messege.OverrideFontSize = TypeStyleFontSize.None;
            messege.ParentBackColor = Color.Empty;
            messege.PasswordChar = '\0';
            messege.PlaceholderText = "";
            messege.PressedBackColor = Color.Gray;
            messege.PressedBorderColor = Color.Gray;
            messege.PressedForeColor = Color.Black;
            messege.ReadOnly = false;
            messege.RightoffsetForDrawingRect = 0;
            messege.SavedGuidID = null;
            messege.SavedID = null;
            messege.ScrollBars = ScrollBars.None;
            messege.SelectionStart = 0;
            messege.ShadowColor = Color.Black;
            messege.ShadowOffset = 0;
            messege.ShadowOpacity = 0.5F;
            messege.ShowAllBorders = false;
            messege.ShowBottomBorder = false;
            messege.ShowFocusIndicator = false;
            messege.ShowLeftBorder = false;
            messege.ShowRightBorder = false;
            messege.ShowScrollbars = false;
            messege.ShowShadow = false;
            messege.ShowTopBorder = false;
            messege.ShowVerticalScrollBar = false;
            messege.Size = new Size(455, 174);
            messege.SlideFrom = SlideDirection.Left;
            messege.StaticNotMoving = false;
            messege.TabIndex = 7;
            messege.TextAlignment = HorizontalAlignment.Left;
            messege.TextImageRelation = TextImageRelation.ImageBeforeText;
            messege.Theme = Vis.Modules.EnumBeepThemes.AutumnTheme;
            messege.TimeFormat = "HH:mm:ss";
            messege.ToolTipText = "";
            messege.TopoffsetForDrawingRect = 0;
            messege.UseGradientBackground = false;
            messege.UseSystemPasswordChar = false;
            messege.UseThemeFont = true;
            messege.UseWaitCursor = true;
            messege.WordWrap = true;
            // 
            // LogopictureBox
            // 
            LogopictureBox.ActiveBackColor = Color.FromArgb(139, 69, 19);
            LogopictureBox.AllowManualRotation = true;
            LogopictureBox.AnimationDuration = 500;
            LogopictureBox.AnimationType = DisplayAnimationType.None;
            LogopictureBox.ApplyThemeOnImage = false;
            LogopictureBox.ApplyThemeToChilds = true;
            LogopictureBox.BackColor = Color.FromArgb(222, 210, 155);
            LogopictureBox.BlockID = null;
            LogopictureBox.BorderColor = Color.FromArgb(210, 105, 30);
            LogopictureBox.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            LogopictureBox.BorderRadius = 1;
            LogopictureBox.BorderStyle = BorderStyle.FixedSingle;
            LogopictureBox.BorderThickness = 1;
            LogopictureBox.BottomoffsetForDrawingRect = 0;
            LogopictureBox.BoundProperty = "ImagePath";
            LogopictureBox.CanBeFocused = true;
            LogopictureBox.CanBeHovered = false;
            LogopictureBox.CanBePressed = true;
            LogopictureBox.Category = Utilities.DbFieldCategory.String;
            LogopictureBox.ComponentName = "LogopictureBox";
            LogopictureBox.DataContext = null;
            LogopictureBox.DataSourceProperty = null;
            LogopictureBox.DisabledBackColor = Color.Gray;
            LogopictureBox.DisabledForeColor = Color.Empty;
            LogopictureBox.DrawingRect = new Rectangle(0, 0, 25, 25);
            LogopictureBox.Easing = EasingType.Linear;
            LogopictureBox.FieldID = null;
            LogopictureBox.FocusBackColor = Color.FromArgb(139, 69, 19);
            LogopictureBox.FocusBorderColor = Color.Gray;
            LogopictureBox.FocusForeColor = Color.White;
            LogopictureBox.FocusIndicatorColor = Color.Blue;
            LogopictureBox.Form = null;
            LogopictureBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            LogopictureBox.GradientEndColor = Color.FromArgb(210, 105, 30);
            LogopictureBox.GradientStartColor = Color.FromArgb(255, 248, 220);
            LogopictureBox.GuidID = "406bd223-e60d-4992-ab0e-02ec79e3ced7";
            LogopictureBox.HoverBackColor = Color.FromArgb(160, 82, 45);
            LogopictureBox.HoverBorderColor = Color.FromArgb(160, 82, 45);
            LogopictureBox.HoveredBackcolor = Color.Wheat;
            LogopictureBox.HoverForeColor = Color.White;
            LogopictureBox.Id = -1;
            LogopictureBox.Image = null;
            LogopictureBox.ImagePath = null;
            LogopictureBox.InactiveBackColor = Color.Gray;
            LogopictureBox.InactiveBorderColor = Color.Gray;
            LogopictureBox.InactiveForeColor = Color.Black;
            LogopictureBox.IsAcceptButton = false;
            LogopictureBox.IsBorderAffectedByTheme = true;
            LogopictureBox.IsCancelButton = false;
            LogopictureBox.IsChild = false;
            LogopictureBox.IsCustomeBorder = false;
            LogopictureBox.IsDefault = false;
            LogopictureBox.IsFocused = false;
            LogopictureBox.IsFramless = false;
            LogopictureBox.IsHovered = false;
            LogopictureBox.IsPressed = false;
            LogopictureBox.IsRounded = true;
            LogopictureBox.IsRoundedAffectedByTheme = true;
            LogopictureBox.IsShadowAffectedByTheme = true;
            LogopictureBox.IsSpinning = false;
            LogopictureBox.IsStillImage = false;
            LogopictureBox.LeftoffsetForDrawingRect = 0;
            LogopictureBox.LinkedProperty = null;
            LogopictureBox.Location = new Point(226, 322);
            LogopictureBox.ManualRotationAngle = 0F;
            LogopictureBox.Name = "LogopictureBox";
            LogopictureBox.OverrideFontSize = TypeStyleFontSize.None;
            LogopictureBox.ParentBackColor = Color.Empty;
            LogopictureBox.PressedBackColor = Color.FromArgb(139, 69, 19);
            LogopictureBox.PressedBorderColor = Color.Gray;
            LogopictureBox.PressedForeColor = Color.White;
            LogopictureBox.RightoffsetForDrawingRect = 0;
            LogopictureBox.SavedGuidID = null;
            LogopictureBox.SavedID = null;
            LogopictureBox.ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio;
            LogopictureBox.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            LogopictureBox.ShadowOffset = 0;
            LogopictureBox.ShadowOpacity = 0.5F;
            LogopictureBox.ShowAllBorders = false;
            LogopictureBox.ShowBottomBorder = false;
            LogopictureBox.ShowFocusIndicator = false;
            LogopictureBox.ShowLeftBorder = false;
            LogopictureBox.ShowRightBorder = false;
            LogopictureBox.ShowShadow = false;
            LogopictureBox.ShowTopBorder = false;
            LogopictureBox.Size = new Size(25, 25);
            LogopictureBox.SlideFrom = SlideDirection.Left;
            LogopictureBox.SpinSpeed = 5F;
            LogopictureBox.StaticNotMoving = false;
            LogopictureBox.TabIndex = 6;
            LogopictureBox.Text = "beepImage1";
            LogopictureBox.Theme = Vis.Modules.EnumBeepThemes.AutumnTheme;
            LogopictureBox.ToolTipText = "";
            LogopictureBox.TopoffsetForDrawingRect = 0;
            LogopictureBox.UseGradientBackground = false;
            LogopictureBox.UseThemeFont = true;
            LogopictureBox.UseWaitCursor = true;
            // 
            // _spinnerImage
            // 
            _spinnerImage.ActiveBackColor = Color.FromArgb(139, 69, 19);
            _spinnerImage.AllowManualRotation = true;
            _spinnerImage.AnimationDuration = 500;
            _spinnerImage.AnimationType = DisplayAnimationType.None;
            _spinnerImage.ApplyThemeOnImage = false;
            _spinnerImage.ApplyThemeToChilds = true;
            _spinnerImage.BackColor = Color.FromArgb(222, 210, 155);
            _spinnerImage.BlockID = null;
            _spinnerImage.BorderColor = Color.FromArgb(210, 105, 30);
            _spinnerImage.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _spinnerImage.BorderRadius = 1;
            _spinnerImage.BorderStyle = BorderStyle.None;
            _spinnerImage.BorderThickness = 1;
            _spinnerImage.BottomoffsetForDrawingRect = 0;
            _spinnerImage.BoundProperty = "ImagePath";
            _spinnerImage.CanBeFocused = true;
            _spinnerImage.CanBeHovered = false;
            _spinnerImage.CanBePressed = true;
            _spinnerImage.Category = Utilities.DbFieldCategory.String;
            _spinnerImage.ComponentName = "pictureBox1";
            _spinnerImage.DataContext = null;
            _spinnerImage.DataSourceProperty = null;
            _spinnerImage.DisabledBackColor = Color.Gray;
            _spinnerImage.DisabledForeColor = Color.Empty;
            _spinnerImage.Dock = DockStyle.Top;
            _spinnerImage.DrawingRect = new Rectangle(0, 0, 461, 103);
            _spinnerImage.Easing = EasingType.Linear;
            _spinnerImage.FieldID = null;
            _spinnerImage.FocusBackColor = Color.FromArgb(139, 69, 19);
            _spinnerImage.FocusBorderColor = Color.Gray;
            _spinnerImage.FocusForeColor = Color.White;
            _spinnerImage.FocusIndicatorColor = Color.Blue;
            _spinnerImage.Form = null;
            _spinnerImage.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _spinnerImage.GradientEndColor = Color.FromArgb(210, 105, 30);
            _spinnerImage.GradientStartColor = Color.FromArgb(255, 248, 220);
            _spinnerImage.GuidID = "8b76c261-b88a-4f49-b900-1818beb66b95";
            _spinnerImage.HoverBackColor = Color.FromArgb(160, 82, 45);
            _spinnerImage.HoverBorderColor = Color.FromArgb(160, 82, 45);
            _spinnerImage.HoveredBackcolor = Color.Wheat;
            _spinnerImage.HoverForeColor = Color.White;
            _spinnerImage.Id = -1;
            _spinnerImage.Image = null;
            _spinnerImage.ImagePath = null;
            _spinnerImage.InactiveBackColor = Color.Gray;
            _spinnerImage.InactiveBorderColor = Color.Gray;
            _spinnerImage.InactiveForeColor = Color.Black;
            _spinnerImage.IsAcceptButton = false;
            _spinnerImage.IsBorderAffectedByTheme = true;
            _spinnerImage.IsCancelButton = false;
            _spinnerImage.IsChild = false;
            _spinnerImage.IsCustomeBorder = false;
            _spinnerImage.IsDefault = false;
            _spinnerImage.IsFocused = false;
            _spinnerImage.IsFramless = false;
            _spinnerImage.IsHovered = false;
            _spinnerImage.IsPressed = false;
            _spinnerImage.IsRounded = true;
            _spinnerImage.IsRoundedAffectedByTheme = true;
            _spinnerImage.IsShadowAffectedByTheme = true;
            _spinnerImage.IsSpinning = false;
            _spinnerImage.IsStillImage = false;
            _spinnerImage.LeftoffsetForDrawingRect = 0;
            _spinnerImage.LinkedProperty = null;
            _spinnerImage.Location = new Point(0, 36);
            _spinnerImage.ManualRotationAngle = 0F;
            _spinnerImage.Name = "_spinnerImage";
            _spinnerImage.OverrideFontSize = TypeStyleFontSize.None;
            _spinnerImage.ParentBackColor = Color.Empty;
            _spinnerImage.PressedBackColor = Color.FromArgb(139, 69, 19);
            _spinnerImage.PressedBorderColor = Color.Gray;
            _spinnerImage.PressedForeColor = Color.White;
            _spinnerImage.RightoffsetForDrawingRect = 0;
            _spinnerImage.SavedGuidID = null;
            _spinnerImage.SavedID = null;
            _spinnerImage.ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio;
            _spinnerImage.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            _spinnerImage.ShadowOffset = 0;
            _spinnerImage.ShadowOpacity = 0.5F;
            _spinnerImage.ShowAllBorders = false;
            _spinnerImage.ShowBottomBorder = false;
            _spinnerImage.ShowFocusIndicator = false;
            _spinnerImage.ShowLeftBorder = false;
            _spinnerImage.ShowRightBorder = false;
            _spinnerImage.ShowShadow = false;
            _spinnerImage.ShowTopBorder = false;
            _spinnerImage.Size = new Size(461, 103);
            _spinnerImage.SlideFrom = SlideDirection.Left;
            _spinnerImage.SpinSpeed = 5F;
            _spinnerImage.StaticNotMoving = false;
            _spinnerImage.TabIndex = 5;
            _spinnerImage.Text = "beepImage1";
            _spinnerImage.Theme = Vis.Modules.EnumBeepThemes.AutumnTheme;
            _spinnerImage.ToolTipText = "";
            _spinnerImage.TopoffsetForDrawingRect = 0;
            _spinnerImage.UseGradientBackground = false;
            _spinnerImage.UseThemeFont = true;
            _spinnerImage.UseWaitCursor = true;
            // 
            // label2
            // 
            label2.ActiveBackColor = Color.Gray;
            label2.Anchor = AnchorStyles.Bottom;
            label2.AnimationDuration = 500;
            label2.AnimationType = DisplayAnimationType.None;
            label2.ApplyThemeOnImage = false;
            label2.ApplyThemeToChilds = true;
            label2.BackColor = Color.FromArgb(205, 133, 63);
            label2.BlockID = null;
            label2.BorderColor = Color.Black;
            label2.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            label2.BorderRadius = 1;
            label2.BorderStyle = BorderStyle.FixedSingle;
            label2.BorderThickness = 1;
            label2.BottomoffsetForDrawingRect = 0;
            label2.BoundProperty = "Text";
            label2.CanBeFocused = true;
            label2.CanBeHovered = false;
            label2.CanBePressed = true;
            label2.Category = Utilities.DbFieldCategory.String;
            label2.ComponentName = "label2";
            label2.DataContext = null;
            label2.DataSourceProperty = null;
            label2.DisabledBackColor = Color.Gray;
            label2.DisabledForeColor = Color.Empty;
            label2.DrawingRect = new Rectangle(1, 1, 81, 17);
            label2.Easing = EasingType.Linear;
            label2.FieldID = null;
            label2.FocusBackColor = Color.Gray;
            label2.FocusBorderColor = Color.Gray;
            label2.FocusForeColor = Color.Black;
            label2.FocusIndicatorColor = Color.Blue;
            label2.Font = new Font("Segoe UI", 9F);
            label2.ForeColor = Color.White;
            label2.Form = null;
            label2.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            label2.GradientEndColor = Color.Gray;
            label2.GradientStartColor = Color.Gray;
            label2.GuidID = "2079cf69-9749-4a75-a09e-b14b4b829d17";
            label2.HideText = false;
            label2.HoverBackColor = Color.FromArgb(222, 210, 155);
            label2.HoverBorderColor = Color.Gray;
            label2.HoveredBackcolor = Color.Wheat;
            label2.HoverForeColor = Color.White;
            label2.Id = -1;
            label2.ImageAlign = ContentAlignment.MiddleLeft;
            label2.ImagePath = null;
            label2.InactiveBackColor = Color.Gray;
            label2.InactiveBorderColor = Color.Gray;
            label2.InactiveForeColor = Color.Black;
            label2.IsAcceptButton = false;
            label2.IsBorderAffectedByTheme = true;
            label2.IsCancelButton = false;
            label2.IsChild = false;
            label2.IsCustomeBorder = false;
            label2.IsDefault = false;
            label2.IsFocused = false;
            label2.IsFramless = false;
            label2.IsHovered = false;
            label2.IsPressed = false;
            label2.IsRounded = true;
            label2.IsRoundedAffectedByTheme = true;
            label2.IsShadowAffectedByTheme = true;
            label2.LabelBackColor = Color.Empty;
            label2.LeftoffsetForDrawingRect = 0;
            label2.LinkedProperty = null;
            label2.Location = new Point(3, 322);
            label2.Margin = new Padding(0);
            label2.MaxImageSize = new Size(16, 16);
            label2.Name = "label2";
            label2.OverrideFontSize = TypeStyleFontSize.None;
            label2.Padding = new Padding(1);
            label2.ParentBackColor = Color.Empty;
            label2.PressedBackColor = Color.Gray;
            label2.PressedBorderColor = Color.Gray;
            label2.PressedForeColor = Color.Black;
            label2.RightoffsetForDrawingRect = 0;
            label2.SavedGuidID = null;
            label2.SavedID = null;
            label2.ShadowColor = Color.Black;
            label2.ShadowOffset = 0;
            label2.ShadowOpacity = 0.5F;
            label2.ShowAllBorders = false;
            label2.ShowBottomBorder = false;
            label2.ShowFocusIndicator = false;
            label2.ShowLeftBorder = false;
            label2.ShowRightBorder = false;
            label2.ShowShadow = false;
            label2.ShowTopBorder = false;
            label2.Size = new Size(83, 19);
            label2.SlideFrom = SlideDirection.Left;
            label2.StaticNotMoving = false;
            label2.TabIndex = 4;
            label2.Text = "The Tech Idea";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.TextImageRelation = TextImageRelation.ImageBeforeText;
            label2.Theme = Vis.Modules.EnumBeepThemes.AutumnTheme;
            label2.ToolTipText = "";
            label2.TopoffsetForDrawingRect = 0;
            label2.UseGradientBackground = false;
            label2.UseScaledFont = false;
            label2.UseThemeFont = true;
            label2.UseWaitCursor = true;
            // 
            // label1
            // 
            label1.ActiveBackColor = Color.Gray;
            label1.Anchor = AnchorStyles.Bottom;
            label1.AnimationDuration = 500;
            label1.AnimationType = DisplayAnimationType.None;
            label1.ApplyThemeOnImage = false;
            label1.ApplyThemeToChilds = true;
            label1.BackColor = Color.FromArgb(205, 133, 63);
            label1.BlockID = null;
            label1.BorderColor = Color.Black;
            label1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            label1.BorderRadius = 1;
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.BorderThickness = 1;
            label1.BottomoffsetForDrawingRect = 0;
            label1.BoundProperty = "Text";
            label1.CanBeFocused = true;
            label1.CanBeHovered = false;
            label1.CanBePressed = true;
            label1.Category = Utilities.DbFieldCategory.String;
            label1.ComponentName = "label1";
            label1.DataContext = null;
            label1.DataSourceProperty = null;
            label1.DisabledBackColor = Color.Gray;
            label1.DisabledForeColor = Color.Empty;
            label1.DrawingRect = new Rectangle(1, 1, 100, 17);
            label1.Easing = EasingType.Linear;
            label1.FieldID = null;
            label1.FocusBackColor = Color.Gray;
            label1.FocusBorderColor = Color.Gray;
            label1.FocusForeColor = Color.Black;
            label1.FocusIndicatorColor = Color.Blue;
            label1.Font = new Font("Segoe UI", 9F);
            label1.ForeColor = Color.White;
            label1.Form = null;
            label1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            label1.GradientEndColor = Color.Gray;
            label1.GradientStartColor = Color.Gray;
            label1.GuidID = "05c50703-03f3-4e4d-bce6-c1b3fb402107";
            label1.HideText = false;
            label1.HoverBackColor = Color.FromArgb(222, 210, 155);
            label1.HoverBorderColor = Color.Gray;
            label1.HoveredBackcolor = Color.Wheat;
            label1.HoverForeColor = Color.White;
            label1.Id = -1;
            label1.ImageAlign = ContentAlignment.MiddleLeft;
            label1.ImagePath = null;
            label1.InactiveBackColor = Color.Gray;
            label1.InactiveBorderColor = Color.Gray;
            label1.InactiveForeColor = Color.Black;
            label1.IsAcceptButton = false;
            label1.IsBorderAffectedByTheme = true;
            label1.IsCancelButton = false;
            label1.IsChild = false;
            label1.IsCustomeBorder = false;
            label1.IsDefault = false;
            label1.IsFocused = false;
            label1.IsFramless = false;
            label1.IsHovered = false;
            label1.IsPressed = false;
            label1.IsRounded = true;
            label1.IsRoundedAffectedByTheme = true;
            label1.IsShadowAffectedByTheme = true;
            label1.LabelBackColor = Color.Empty;
            label1.LeftoffsetForDrawingRect = 0;
            label1.LinkedProperty = null;
            label1.Location = new Point(358, 322);
            label1.Margin = new Padding(0);
            label1.MaxImageSize = new Size(16, 16);
            label1.Name = "label1";
            label1.OverrideFontSize = TypeStyleFontSize.None;
            label1.Padding = new Padding(1);
            label1.ParentBackColor = Color.Empty;
            label1.PressedBackColor = Color.Gray;
            label1.PressedBorderColor = Color.Gray;
            label1.PressedForeColor = Color.Black;
            label1.RightoffsetForDrawingRect = 0;
            label1.SavedGuidID = null;
            label1.SavedID = null;
            label1.ShadowColor = Color.Black;
            label1.ShadowOffset = 0;
            label1.ShadowOpacity = 0.5F;
            label1.ShowAllBorders = false;
            label1.ShowBottomBorder = false;
            label1.ShowFocusIndicator = false;
            label1.ShowLeftBorder = false;
            label1.ShowRightBorder = false;
            label1.ShowShadow = false;
            label1.ShowTopBorder = false;
            label1.Size = new Size(102, 19);
            label1.SlideFrom = SlideDirection.Left;
            label1.StaticNotMoving = false;
            label1.TabIndex = 3;
            label1.Text = "Powered by Beep";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.TextImageRelation = TextImageRelation.ImageBeforeText;
            label1.Theme = Vis.Modules.EnumBeepThemes.AutumnTheme;
            label1.ToolTipText = "";
            label1.TopoffsetForDrawingRect = 0;
            label1.UseGradientBackground = false;
            label1.UseScaledFont = false;
            label1.UseThemeFont = true;
            label1.UseWaitCursor = true;
            // 
            // Title
            // 
            Title.ActiveBackColor = Color.Gray;
            Title.AnimationDuration = 500;
            Title.AnimationType = DisplayAnimationType.None;
            Title.ApplyThemeOnImage = false;
            Title.ApplyThemeToChilds = true;
            Title.BackColor = Color.FromArgb(205, 133, 63);
            Title.BlockID = null;
            Title.BorderColor = Color.Black;
            Title.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            Title.BorderRadius = 1;
            Title.BorderStyle = BorderStyle.FixedSingle;
            Title.BorderThickness = 1;
            Title.BottomoffsetForDrawingRect = 0;
            Title.BoundProperty = "Text";
            Title.CanBeFocused = true;
            Title.CanBeHovered = false;
            Title.CanBePressed = true;
            Title.Category = Utilities.DbFieldCategory.String;
            Title.ComponentName = "Title";
            Title.DataContext = null;
            Title.DataSourceProperty = null;
            Title.DisabledBackColor = Color.Gray;
            Title.DisabledForeColor = Color.Empty;
            Title.Dock = DockStyle.Top;
            Title.DrawingRect = new Rectangle(1, 1, 459, 34);
            Title.Easing = EasingType.Linear;
            Title.FieldID = null;
            Title.FocusBackColor = Color.Gray;
            Title.FocusBorderColor = Color.Gray;
            Title.FocusForeColor = Color.Black;
            Title.FocusIndicatorColor = Color.Blue;
            Title.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Title.ForeColor = Color.White;
            Title.Form = null;
            Title.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            Title.GradientEndColor = Color.Gray;
            Title.GradientStartColor = Color.Gray;
            Title.GuidID = "b3a5330f-524d-4013-a2e6-ef719c7f17fc";
            Title.HideText = false;
            Title.HoverBackColor = Color.FromArgb(222, 210, 155);
            Title.HoverBorderColor = Color.Gray;
            Title.HoveredBackcolor = Color.Wheat;
            Title.HoverForeColor = Color.White;
            Title.Id = -1;
            Title.ImageAlign = ContentAlignment.MiddleLeft;
            Title.ImagePath = null;
            Title.InactiveBackColor = Color.Gray;
            Title.InactiveBorderColor = Color.Gray;
            Title.InactiveForeColor = Color.Black;
            Title.IsAcceptButton = false;
            Title.IsBorderAffectedByTheme = true;
            Title.IsCancelButton = false;
            Title.IsChild = false;
            Title.IsCustomeBorder = false;
            Title.IsDefault = false;
            Title.IsFocused = false;
            Title.IsFramless = false;
            Title.IsHovered = false;
            Title.IsPressed = false;
            Title.IsRounded = true;
            Title.IsRoundedAffectedByTheme = true;
            Title.IsShadowAffectedByTheme = true;
            Title.LabelBackColor = Color.Empty;
            Title.LeftoffsetForDrawingRect = 0;
            Title.LinkedProperty = null;
            Title.Location = new Point(0, 0);
            Title.Margin = new Padding(0);
            Title.MaxImageSize = new Size(16, 16);
            Title.Name = "Title";
            Title.OverrideFontSize = TypeStyleFontSize.None;
            Title.Padding = new Padding(1);
            Title.ParentBackColor = Color.Empty;
            Title.PressedBackColor = Color.Gray;
            Title.PressedBorderColor = Color.Gray;
            Title.PressedForeColor = Color.Black;
            Title.RightoffsetForDrawingRect = 0;
            Title.SavedGuidID = null;
            Title.SavedID = null;
            Title.ShadowColor = Color.Black;
            Title.ShadowOffset = 0;
            Title.ShadowOpacity = 0.5F;
            Title.ShowAllBorders = false;
            Title.ShowBottomBorder = false;
            Title.ShowFocusIndicator = false;
            Title.ShowLeftBorder = false;
            Title.ShowRightBorder = false;
            Title.ShowShadow = false;
            Title.ShowTopBorder = false;
            Title.Size = new Size(461, 36);
            Title.SlideFrom = SlideDirection.Left;
            Title.StaticNotMoving = false;
            Title.TabIndex = 8;
            Title.Text = "Beep Data Management";
            Title.TextAlign = ContentAlignment.MiddleCenter;
            Title.TextImageRelation = TextImageRelation.ImageBeforeText;
            Title.Theme = Vis.Modules.EnumBeepThemes.AutumnTheme;
            Title.ToolTipText = "";
            Title.TopoffsetForDrawingRect = 0;
            Title.UseGradientBackground = false;
            Title.UseScaledFont = false;
            Title.UseThemeFont = false;
            Title.UseWaitCursor = true;
            // 
            // BeepWait
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(210, 105, 30);
            ClientSize = new Size(469, 354);
            Controls.Add(panel2);
            Margin = new Padding(4, 3, 4, 3);
            Name = "BeepWait";
            ShowInTaskbar = false;
            Text = "BeepWait";
            Theme = Vis.Modules.EnumBeepThemes.AutumnTheme;
            TopMost = true;
            UseWaitCursor = true;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        public Panel panel2;
        public BeepLabel label1;
        public BeepLabel label2;
        public BeepImage LogopictureBox;
        public BeepImage _spinnerImage;
        public BeepTextBox messege;
        public BeepLabel Title;
    }
}