namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepWaitScreen
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
            LogoImage = new BeepImage();
            TitleLabel1 = new BeepLabel();
            MessegeTextBox = new BeepTextBox();
            _spinnerImage = new BeepImage();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.ShowBorder = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.GradientBurstTheme;
            // 
            // LogoImage
            // 
            LogoImage.ActiveBackColor = Color.FromArgb(255, 0, 255);
            LogoImage.AllowManualRotation = true;
            LogoImage.AnimationDuration = 500;
            LogoImage.AnimationType = DisplayAnimationType.None;
            LogoImage.ApplyThemeOnImage = false;
            LogoImage.ApplyThemeToChilds = true;
            LogoImage.BackColor = Color.FromArgb(200, 248, 255);
            LogoImage.BlockID = null;
            LogoImage.BorderColor = Color.FromArgb(138, 43, 226);
            LogoImage.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            LogoImage.BorderRadius = 5;
            LogoImage.BorderStyle = BorderStyle.FixedSingle;
            LogoImage.BorderThickness = 1;
            LogoImage.BottomoffsetForDrawingRect = 0;
            LogoImage.BoundProperty = null;
            LogoImage.DataContext = null;
            LogoImage.DisabledBackColor = Color.Gray;
            LogoImage.DisabledForeColor = Color.Empty;
            LogoImage.DrawingRect = new Rectangle(0, 0, 99, 69);
            LogoImage.Easing = EasingType.Linear;
            LogoImage.FieldID = null;
            LogoImage.FocusBackColor = Color.FromArgb(255, 0, 255);
            LogoImage.FocusBorderColor = Color.Gray;
            LogoImage.FocusForeColor = Color.White;
            LogoImage.FocusIndicatorColor = Color.Blue;
            LogoImage.Form = null;
            LogoImage.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            LogoImage.GradientEndColor = Color.FromArgb(0, 0, 255);
            LogoImage.GradientStartColor = Color.FromArgb(255, 0, 0);
            LogoImage.HoverBackColor = Color.FromArgb(255, 20, 147);
            LogoImage.HoverBorderColor = Color.FromArgb(75, 0, 130);
            LogoImage.HoveredBackcolor = Color.Wheat;
            LogoImage.HoverForeColor = Color.White;
            LogoImage.Id = -1;
            LogoImage.Image = null;
            LogoImage.ImagePath = null;
            LogoImage.InactiveBackColor = Color.Gray;
            LogoImage.InactiveBorderColor = Color.Gray;
            LogoImage.InactiveForeColor = Color.Black;
            LogoImage.IsAcceptButton = false;
            LogoImage.IsBorderAffectedByTheme = true;
            LogoImage.IsCancelButton = false;
            LogoImage.IsChild = true;
            LogoImage.IsCustomeBorder = false;
            LogoImage.IsDefault = false;
            LogoImage.IsFocused = false;
            LogoImage.IsFramless = false;
            LogoImage.IsHovered = false;
            LogoImage.IsPressed = false;
            LogoImage.IsRounded = true;
            LogoImage.IsRoundedAffectedByTheme = true;
            LogoImage.IsShadowAffectedByTheme = true;
            LogoImage.IsSpinning = false;
            LogoImage.IsStillImage = false;
            LogoImage.LeftoffsetForDrawingRect = 0;
            LogoImage.Location = new Point(12, 12);
            LogoImage.ManualRotationAngle = 0F;
            LogoImage.Name = "LogoImage";
            LogoImage.OverrideFontSize = TypeStyleFontSize.None;
            LogoImage.ParentBackColor = Color.FromArgb(200, 248, 255);
            LogoImage.PressedBackColor = Color.FromArgb(255, 0, 255);
            LogoImage.PressedBorderColor = Color.Gray;
            LogoImage.PressedForeColor = Color.White;
            LogoImage.RightoffsetForDrawingRect = 0;
            LogoImage.SavedGuidID = null;
            LogoImage.SavedID = null;
            LogoImage.ScaleMode = ImageScaleMode.KeepAspectRatio;
            LogoImage.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            LogoImage.ShadowOffset = 0;
            LogoImage.ShadowOpacity = 0.5F;
            LogoImage.ShowAllBorders = false;
            LogoImage.ShowBottomBorder = false;
            LogoImage.ShowFocusIndicator = false;
            LogoImage.ShowLeftBorder = false;
            LogoImage.ShowRightBorder = false;
            LogoImage.ShowShadow = false;
            LogoImage.ShowTopBorder = false;
            LogoImage.Size = new Size(100, 70);
            LogoImage.SlideFrom = SlideDirection.Left;
            LogoImage.SpinSpeed = 5F;
            LogoImage.StaticNotMoving = false;
            LogoImage.TabIndex = 0;
            LogoImage.Text = "beepImage1";
            LogoImage.Theme = Vis.Modules.EnumBeepThemes.GradientBurstTheme;
            LogoImage.ToolTipText = "";
            LogoImage.TopoffsetForDrawingRect = 0;
            LogoImage.UseGradientBackground = false;
            // 
            // TitleLabel1
            // 
            TitleLabel1.ActiveBackColor = Color.Gray;
            TitleLabel1.AnimationDuration = 500;
            TitleLabel1.AnimationType = DisplayAnimationType.None;
            TitleLabel1.ApplyThemeOnImage = false;
            TitleLabel1.ApplyThemeToChilds = true;
            TitleLabel1.BackColor = Color.FromArgb(200, 248, 255);
            TitleLabel1.BlockID = null;
            TitleLabel1.BorderColor = Color.Black;
            TitleLabel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            TitleLabel1.BorderRadius = 5;
            TitleLabel1.BorderStyle = BorderStyle.FixedSingle;
            TitleLabel1.BorderThickness = 1;
            TitleLabel1.BottomoffsetForDrawingRect = 0;
            TitleLabel1.BoundProperty = "Text";
            TitleLabel1.DataContext = null;
            TitleLabel1.DisabledBackColor = Color.Gray;
            TitleLabel1.DisabledForeColor = Color.Empty;
            TitleLabel1.DrawingRect = new Rectangle(0, 0, 330, 26);
            TitleLabel1.Easing = EasingType.Linear;
            TitleLabel1.FieldID = null;
            TitleLabel1.FocusBackColor = Color.Gray;
            TitleLabel1.FocusBorderColor = Color.Gray;
            TitleLabel1.FocusForeColor = Color.Black;
            TitleLabel1.FocusIndicatorColor = Color.Blue;
            TitleLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            TitleLabel1.ForeColor = Color.FromArgb(75, 0, 130);
            TitleLabel1.Form = null;
            TitleLabel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            TitleLabel1.GradientEndColor = Color.Gray;
            TitleLabel1.GradientStartColor = Color.Gray;
            TitleLabel1.HideText = false;
            TitleLabel1.HoverBackColor = Color.FromArgb(255, 20, 147);
            TitleLabel1.HoverBorderColor = Color.Gray;
            TitleLabel1.HoveredBackcolor = Color.Wheat;
            TitleLabel1.HoverForeColor = Color.White;
            TitleLabel1.Id = -1;
            TitleLabel1.ImageAlign = ContentAlignment.MiddleLeft;
            TitleLabel1.ImagePath = null;
            TitleLabel1.InactiveBackColor = Color.Gray;
            TitleLabel1.InactiveBorderColor = Color.Gray;
            TitleLabel1.InactiveForeColor = Color.Black;
            TitleLabel1.IsAcceptButton = false;
            TitleLabel1.IsBorderAffectedByTheme = true;
            TitleLabel1.IsCancelButton = false;
            TitleLabel1.IsChild = true;
            TitleLabel1.IsCustomeBorder = false;
            TitleLabel1.IsDefault = false;
            TitleLabel1.IsFocused = false;
            TitleLabel1.IsFramless = false;
            TitleLabel1.IsHovered = false;
            TitleLabel1.IsPressed = false;
            TitleLabel1.IsRounded = true;
            TitleLabel1.IsRoundedAffectedByTheme = true;
            TitleLabel1.IsShadowAffectedByTheme = true;
            TitleLabel1.LeftoffsetForDrawingRect = 0;
            TitleLabel1.Location = new Point(154, 28);
            TitleLabel1.Margin = new Padding(0);
            TitleLabel1.MaxImageSize = new Size(16, 16);
            TitleLabel1.MaximumSize = new Size(0, 27);
            TitleLabel1.MinimumSize = new Size(0, 27);
            TitleLabel1.Name = "TitleLabel1";
            TitleLabel1.OverrideFontSize = TypeStyleFontSize.None;
            TitleLabel1.ParentBackColor = Color.FromArgb(200, 248, 255);
            TitleLabel1.PressedBackColor = Color.Gray;
            TitleLabel1.PressedBorderColor = Color.Gray;
            TitleLabel1.PressedForeColor = Color.Black;
            TitleLabel1.RightoffsetForDrawingRect = 0;
            TitleLabel1.SavedGuidID = null;
            TitleLabel1.SavedID = null;
            TitleLabel1.ShadowColor = Color.Black;
            TitleLabel1.ShadowOffset = 0;
            TitleLabel1.ShadowOpacity = 0.5F;
            TitleLabel1.ShowAllBorders = false;
            TitleLabel1.ShowBottomBorder = false;
            TitleLabel1.ShowFocusIndicator = false;
            TitleLabel1.ShowLeftBorder = false;
            TitleLabel1.ShowRightBorder = false;
            TitleLabel1.ShowShadow = false;
            TitleLabel1.ShowTopBorder = false;
            TitleLabel1.Size = new Size(331, 27);
            TitleLabel1.SlideFrom = SlideDirection.Left;
            TitleLabel1.StaticNotMoving = false;
            TitleLabel1.TabIndex = 1;
            TitleLabel1.TextAlign = ContentAlignment.MiddleLeft;
            TitleLabel1.TextImageRelation = TextImageRelation.ImageBeforeText;
            TitleLabel1.Theme = Vis.Modules.EnumBeepThemes.GradientBurstTheme;
            TitleLabel1.ToolTipText = "";
            TitleLabel1.TopoffsetForDrawingRect = 0;
            TitleLabel1.UseGradientBackground = false;
            // 
            // MessegeTextBox
            // 
            MessegeTextBox.AcceptsReturn = false;
            MessegeTextBox.AcceptsTab = false;
            MessegeTextBox.ActiveBackColor = Color.Gray;
            MessegeTextBox.AnimationDuration = 500;
            MessegeTextBox.AnimationType = DisplayAnimationType.None;
            MessegeTextBox.ApplyThemeOnImage = false;
            MessegeTextBox.ApplyThemeToChilds = true;
            MessegeTextBox.AutoCompleteMode = AutoCompleteMode.None;
            MessegeTextBox.AutoCompleteSource = AutoCompleteSource.None;
            MessegeTextBox.BackColor = Color.FromArgb(200, 248, 255);
            MessegeTextBox.BlockID = null;
            MessegeTextBox.BorderColor = Color.Black;
            MessegeTextBox.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            MessegeTextBox.BorderRadius = 5;
            MessegeTextBox.BorderStyle = BorderStyle.FixedSingle;
            MessegeTextBox.BorderThickness = 1;
            MessegeTextBox.BottomoffsetForDrawingRect = 0;
            MessegeTextBox.BoundProperty = "Text";
            MessegeTextBox.DataContext = null;
            MessegeTextBox.DisabledBackColor = Color.Gray;
            MessegeTextBox.DisabledForeColor = Color.Empty;
            MessegeTextBox.DrawingRect = new Rectangle(0, 0, 350, 155);
            MessegeTextBox.Easing = EasingType.Linear;
            MessegeTextBox.FieldID = null;
            MessegeTextBox.FocusBackColor = Color.Gray;
            MessegeTextBox.FocusBorderColor = Color.Gray;
            MessegeTextBox.FocusForeColor = Color.Black;
            MessegeTextBox.FocusIndicatorColor = Color.Blue;
            MessegeTextBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            MessegeTextBox.Form = null;
            MessegeTextBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            MessegeTextBox.GradientEndColor = Color.Gray;
            MessegeTextBox.GradientStartColor = Color.Gray;
            MessegeTextBox.HideSelection = true;
            MessegeTextBox.HoverBackColor = Color.Gray;
            MessegeTextBox.HoverBorderColor = Color.Gray;
            MessegeTextBox.HoveredBackcolor = Color.Wheat;
            MessegeTextBox.HoverForeColor = Color.Black;
            MessegeTextBox.Id = -1;
            MessegeTextBox.ImageAlign = ContentAlignment.MiddleLeft;
            MessegeTextBox.ImagePath = null;
            MessegeTextBox.InactiveBackColor = Color.Gray;
            MessegeTextBox.InactiveBorderColor = Color.Gray;
            MessegeTextBox.InactiveForeColor = Color.Black;
            // 
            // 
            // 
            MessegeTextBox.InnerTextBox.BackColor = Color.FromArgb(40, 248, 255);
            MessegeTextBox.InnerTextBox.BorderStyle = BorderStyle.None;
            MessegeTextBox.InnerTextBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            MessegeTextBox.InnerTextBox.ForeColor = Color.FromArgb(75, 0, 130);
            MessegeTextBox.InnerTextBox.Location = new Point(0, 0);
            MessegeTextBox.InnerTextBox.Multiline = true;
            MessegeTextBox.InnerTextBox.Name = "";
            MessegeTextBox.InnerTextBox.Size = new Size(350, 155);
            MessegeTextBox.InnerTextBox.TabIndex = 0;
            MessegeTextBox.IsAcceptButton = false;
            MessegeTextBox.IsBorderAffectedByTheme = true;
            MessegeTextBox.IsCancelButton = false;
            MessegeTextBox.IsChild = true;
            MessegeTextBox.IsCustomeBorder = true;
            MessegeTextBox.IsDefault = false;
            MessegeTextBox.IsFocused = false;
            MessegeTextBox.IsFramless = false;
            MessegeTextBox.IsHovered = false;
            MessegeTextBox.IsPressed = false;
            MessegeTextBox.IsRounded = true;
            MessegeTextBox.IsRoundedAffectedByTheme = true;
            MessegeTextBox.IsShadowAffectedByTheme = true;
            MessegeTextBox.LeftoffsetForDrawingRect = 0;
            MessegeTextBox.Location = new Point(12, 91);
            MessegeTextBox.MaskFormat = "";
            MessegeTextBox.Modified = false;
            MessegeTextBox.Multiline = true;
            MessegeTextBox.Name = "MessegeTextBox";
            MessegeTextBox.OnlyCharacters = false;
            MessegeTextBox.OnlyDigits = false;
            MessegeTextBox.OverrideFontSize = TypeStyleFontSize.None;
            MessegeTextBox.ParentBackColor = Color.FromArgb(200, 248, 255);
            MessegeTextBox.PasswordChar = '\0';
            MessegeTextBox.PlaceholderText = "";
            MessegeTextBox.PressedBackColor = Color.Gray;
            MessegeTextBox.PressedBorderColor = Color.Gray;
            MessegeTextBox.PressedForeColor = Color.Black;
            MessegeTextBox.ReadOnly = false;
            MessegeTextBox.RightoffsetForDrawingRect = 0;
            MessegeTextBox.SavedGuidID = null;
            MessegeTextBox.SavedID = null;
            MessegeTextBox.scrollbars = false;
            MessegeTextBox.ScrollBars = ScrollBars.None;
            MessegeTextBox.SelectionStart = 0;
            MessegeTextBox.ShadowColor = Color.Black;
            MessegeTextBox.ShadowOffset = 0;
            MessegeTextBox.ShadowOpacity = 0.5F;
            MessegeTextBox.ShowAllBorders = false;
            MessegeTextBox.ShowBottomBorder = false;
            MessegeTextBox.ShowFocusIndicator = false;
            MessegeTextBox.ShowLeftBorder = false;
            MessegeTextBox.ShowRightBorder = false;
            MessegeTextBox.ShowShadow = false;
            MessegeTextBox.ShowTopBorder = false;
            MessegeTextBox.Size = new Size(351, 156);
            MessegeTextBox.SlideFrom = SlideDirection.Left;
            MessegeTextBox.StaticNotMoving = false;
            MessegeTextBox.TabIndex = 2;
            MessegeTextBox.TextAlignment = HorizontalAlignment.Left;
            MessegeTextBox.TextImageRelation = TextImageRelation.ImageBeforeText;
            MessegeTextBox.Theme = Vis.Modules.EnumBeepThemes.GradientBurstTheme;
            MessegeTextBox.ToolTipText = "";
            MessegeTextBox.TopoffsetForDrawingRect = 0;
            MessegeTextBox.UseGradientBackground = false;
            MessegeTextBox.UseSystemPasswordChar = false;
            MessegeTextBox.WordWrap = true;
            // 
            // _spinnerImage
            // 
            _spinnerImage.ActiveBackColor = Color.FromArgb(255, 0, 255);
            _spinnerImage.AllowManualRotation = true;
            _spinnerImage.AnimationDuration = 500;
            _spinnerImage.AnimationType = DisplayAnimationType.None;
            _spinnerImage.ApplyThemeOnImage = false;
            _spinnerImage.ApplyThemeToChilds = true;
            _spinnerImage.BackColor = Color.FromArgb(200, 248, 255);
            _spinnerImage.BlockID = null;
            _spinnerImage.BorderColor = Color.FromArgb(138, 43, 226);
            _spinnerImage.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _spinnerImage.BorderRadius = 5;
            _spinnerImage.BorderStyle = BorderStyle.FixedSingle;
            _spinnerImage.BorderThickness = 1;
            _spinnerImage.BottomoffsetForDrawingRect = 0;
            _spinnerImage.BoundProperty = null;
            _spinnerImage.DataContext = null;
            _spinnerImage.DisabledBackColor = Color.Gray;
            _spinnerImage.DisabledForeColor = Color.Empty;
            _spinnerImage.DrawingRect = new Rectangle(0, 0, 104, 58);
            _spinnerImage.Easing = EasingType.Linear;
            _spinnerImage.FieldID = null;
            _spinnerImage.FocusBackColor = Color.FromArgb(255, 0, 255);
            _spinnerImage.FocusBorderColor = Color.Gray;
            _spinnerImage.FocusForeColor = Color.White;
            _spinnerImage.FocusIndicatorColor = Color.Blue;
            _spinnerImage.Form = null;
            _spinnerImage.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _spinnerImage.GradientEndColor = Color.FromArgb(0, 0, 255);
            _spinnerImage.GradientStartColor = Color.FromArgb(255, 0, 0);
            _spinnerImage.HoverBackColor = Color.FromArgb(255, 20, 147);
            _spinnerImage.HoverBorderColor = Color.FromArgb(75, 0, 130);
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
            _spinnerImage.IsChild = true;
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
            _spinnerImage.Location = new Point(380, 140);
            _spinnerImage.ManualRotationAngle = 0F;
            _spinnerImage.Name = "_spinnerImage";
            _spinnerImage.OverrideFontSize = TypeStyleFontSize.None;
            _spinnerImage.ParentBackColor = Color.FromArgb(200, 248, 255);
            _spinnerImage.PressedBackColor = Color.FromArgb(255, 0, 255);
            _spinnerImage.PressedBorderColor = Color.Gray;
            _spinnerImage.PressedForeColor = Color.White;
            _spinnerImage.RightoffsetForDrawingRect = 0;
            _spinnerImage.SavedGuidID = null;
            _spinnerImage.SavedID = null;
            _spinnerImage.ScaleMode = ImageScaleMode.KeepAspectRatio;
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
            _spinnerImage.Size = new Size(105, 59);
            _spinnerImage.SlideFrom = SlideDirection.Left;
            _spinnerImage.SpinSpeed = 5F;
            _spinnerImage.StaticNotMoving = false;
            _spinnerImage.TabIndex = 1;
            _spinnerImage.Text = "beepImage1";
            _spinnerImage.Theme = Vis.Modules.EnumBeepThemes.GradientBurstTheme;
            _spinnerImage.ToolTipText = "";
            _spinnerImage.TopoffsetForDrawingRect = 0;
            _spinnerImage.UseGradientBackground = false;
            // 
            // BeepWaitScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(138, 43, 226);
            ClientSize = new Size(508, 268);
            Controls.Add(_spinnerImage);
            Controls.Add(MessegeTextBox);
            Controls.Add(TitleLabel1);
            Controls.Add(LogoImage);
            Name = "BeepWaitScreen";
            StartPosition = FormStartPosition.CenterScreen;
            Theme = Vis.Modules.EnumBeepThemes.GradientBurstTheme;
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private BeepImage LogoImage;
        private BeepLabel TitleLabel1;
        private BeepTextBox MessegeTextBox;
        private BeepImage _spinnerImage;
    }
}