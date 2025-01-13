using TheTechIdea.Beep.Vis.Modules;

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
            MessegeTextBox.SuspendLayout();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.IsRounded = false;
            beepuiManager1.ShowBorder = false;
            beepuiManager1.Theme = EnumBeepThemes.ModernDarkTheme;
            // 
            // LogoImage
            // 
            LogoImage.ActiveBackColor = Color.FromArgb(80, 80, 80);
            LogoImage.AllowManualRotation = true;
            LogoImage.AnimationDuration = 500;
            LogoImage.AnimationType = DisplayAnimationType.None;
            LogoImage.ApplyThemeOnImage = false;
            LogoImage.ApplyThemeToChilds = true;
            LogoImage.BackColor = Color.FromArgb(30, 30, 30);
            LogoImage.BlockID = null;
            LogoImage.BorderColor = Color.Gray;
            LogoImage.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            LogoImage.BorderRadius = 5;
            LogoImage.BorderStyle = BorderStyle.FixedSingle;
            LogoImage.BorderThickness = 1;
            LogoImage.BottomoffsetForDrawingRect = 0;
            LogoImage.BoundProperty = null;
            LogoImage.CanBeFocused = true;
            LogoImage.CanBeHovered = false;
            LogoImage.CanBePressed = true;
            LogoImage.Category = Utilities.DbFieldCategory.String;
            LogoImage.ComponentName = "LogoImage";
            LogoImage.DataContext = null;
            LogoImage.DataSourceProperty = null;
            LogoImage.DisabledBackColor = Color.Gray;
            LogoImage.DisabledForeColor = Color.Empty;
            LogoImage.DrawingRect = new Rectangle(0, 0, 100, 70);
            LogoImage.Easing = EasingType.Linear;
            LogoImage.FieldID = null;
            LogoImage.FocusBackColor = Color.FromArgb(80, 80, 80);
            LogoImage.FocusBorderColor = Color.Gray;
            LogoImage.FocusForeColor = Color.White;
            LogoImage.FocusIndicatorColor = Color.Blue;
            LogoImage.Form = null;
            LogoImage.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            LogoImage.GradientEndColor = Color.FromArgb(60, 60, 60);
            LogoImage.GradientStartColor = Color.FromArgb(30, 30, 30);
            LogoImage.GuidID = "230e3670-3fee-43c0-8ef7-4c313d44d923";
            LogoImage.HoverBackColor = Color.FromArgb(70, 70, 70);
            LogoImage.HoverBorderColor = Color.CornflowerBlue;
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
            LogoImage.IsChild = false;
            LogoImage.IsCustomeBorder = false;
            LogoImage.IsDefault = false;
            LogoImage.IsFocused = false;
            LogoImage.IsFramless = false;
            LogoImage.IsHovered = false;
            LogoImage.IsPressed = false;
            LogoImage.IsRounded = false;
            LogoImage.IsRoundedAffectedByTheme = true;
            LogoImage.IsShadowAffectedByTheme = true;
            LogoImage.IsSpinning = false;
            LogoImage.IsStillImage = false;
            LogoImage.LeftoffsetForDrawingRect = 0;
            LogoImage.LinkedProperty = null;
            LogoImage.Location = new Point(12, 12);
            LogoImage.ManualRotationAngle = 0F;
            LogoImage.Name = "LogoImage";
            LogoImage.OverrideFontSize = TypeStyleFontSize.None;
            LogoImage.ParentBackColor = Color.FromArgb(200, 248, 255);
            LogoImage.PressedBackColor = Color.FromArgb(80, 80, 80);
            LogoImage.PressedBorderColor = Color.Gray;
            LogoImage.PressedForeColor = Color.White;
            LogoImage.RightoffsetForDrawingRect = 0;
            LogoImage.SavedGuidID = null;
            LogoImage.SavedID = null;
            LogoImage.ScaleMode = ImageScaleMode.KeepAspectRatio;
            LogoImage.ShadowColor = Color.Black;
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
            LogoImage.Theme = EnumBeepThemes.ModernDarkTheme;
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
            TitleLabel1.BackColor = Color.FromArgb(45, 45, 45);
            TitleLabel1.BlockID = null;
            TitleLabel1.BorderColor = Color.Black;
            TitleLabel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            TitleLabel1.BorderRadius = 5;
            TitleLabel1.BorderStyle = BorderStyle.FixedSingle;
            TitleLabel1.BorderThickness = 1;
            TitleLabel1.BottomoffsetForDrawingRect = 0;
            TitleLabel1.BoundProperty = "Text";
            TitleLabel1.CanBeFocused = true;
            TitleLabel1.CanBeHovered = false;
            TitleLabel1.CanBePressed = true;
            TitleLabel1.Category = Utilities.DbFieldCategory.String;
            TitleLabel1.ComponentName = "TitleLabel1";
            TitleLabel1.DataContext = null;
            TitleLabel1.DataSourceProperty = null;
            TitleLabel1.DisabledBackColor = Color.Gray;
            TitleLabel1.DisabledForeColor = Color.Empty;
            TitleLabel1.DrawingRect = new Rectangle(0, 0, 369, 27);
            TitleLabel1.Easing = EasingType.Linear;
            TitleLabel1.FieldID = null;
            TitleLabel1.FocusBackColor = Color.Gray;
            TitleLabel1.FocusBorderColor = Color.Gray;
            TitleLabel1.FocusForeColor = Color.Black;
            TitleLabel1.FocusIndicatorColor = Color.Blue;
            TitleLabel1.Font = new Font("Segoe UI", 9.700001F);
            TitleLabel1.ForeColor = Color.White;
            TitleLabel1.Form = null;
            TitleLabel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            TitleLabel1.GradientEndColor = Color.Gray;
            TitleLabel1.GradientStartColor = Color.Gray;
            TitleLabel1.GuidID = "434bd0d7-53d6-4384-9655-e0c860823a95";
            TitleLabel1.HideText = false;
            TitleLabel1.HoverBackColor = Color.FromArgb(30, 30, 30);
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
            TitleLabel1.IsChild = false;
            TitleLabel1.IsCustomeBorder = false;
            TitleLabel1.IsDefault = false;
            TitleLabel1.IsFocused = false;
            TitleLabel1.IsFramless = false;
            TitleLabel1.IsHovered = false;
            TitleLabel1.IsPressed = false;
            TitleLabel1.IsRounded = false;
            TitleLabel1.IsRoundedAffectedByTheme = true;
            TitleLabel1.IsShadowAffectedByTheme = true;
            TitleLabel1.LabelBackColor = Color.Empty;
            TitleLabel1.LeftoffsetForDrawingRect = 0;
            TitleLabel1.LinkedProperty = null;
            TitleLabel1.Location = new Point(115, 32);
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
            TitleLabel1.Size = new Size(369, 27);
            TitleLabel1.SlideFrom = SlideDirection.Left;
            TitleLabel1.StaticNotMoving = false;
            TitleLabel1.TabIndex = 1;
            TitleLabel1.TextAlign = ContentAlignment.MiddleLeft;
            TitleLabel1.TextImageRelation = TextImageRelation.ImageBeforeText;
            TitleLabel1.Theme = EnumBeepThemes.ModernDarkTheme;
            TitleLabel1.ToolTipText = "";
            TitleLabel1.TopoffsetForDrawingRect = 0;
            TitleLabel1.UseGradientBackground = false;
            TitleLabel1.UseScaledFont = false;
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
            MessegeTextBox.BackColor = Color.FromArgb(30, 30, 30);
            MessegeTextBox.BlockID = null;
            MessegeTextBox.BorderColor = Color.Black;
            MessegeTextBox.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            MessegeTextBox.BorderRadius = 5;
            MessegeTextBox.BorderStyle = BorderStyle.FixedSingle;
            MessegeTextBox.BorderThickness = 1;
            MessegeTextBox.BottomoffsetForDrawingRect = 0;
            MessegeTextBox.BoundProperty = "Text";
            MessegeTextBox.CanBeFocused = true;
            MessegeTextBox.CanBeHovered = false;
            MessegeTextBox.CanBePressed = true;
            MessegeTextBox.Category = Utilities.DbFieldCategory.String;
            MessegeTextBox.ComponentName = "MessegeTextBox";
            MessegeTextBox.Controls.Add(_spinnerImage);
            MessegeTextBox.CustomMask = "";
            MessegeTextBox.DataContext = null;
            MessegeTextBox.DataSourceProperty = null;
            MessegeTextBox.DateFormat = "MM/dd/yyyy HH:mm:ss";
            MessegeTextBox.DateTimeFormat = "MM/dd/yyyy HH:mm:ss";
            MessegeTextBox.DisabledBackColor = Color.Gray;
            MessegeTextBox.DisabledForeColor = Color.Empty;
            MessegeTextBox.DrawingRect = new Rectangle(0, 0, 496, 174);
            MessegeTextBox.Easing = EasingType.Linear;
            MessegeTextBox.FieldID = null;
            MessegeTextBox.FocusBackColor = Color.Gray;
            MessegeTextBox.FocusBorderColor = Color.Gray;
            MessegeTextBox.FocusForeColor = Color.Black;
            MessegeTextBox.FocusIndicatorColor = Color.Blue;
            MessegeTextBox.Font = new Font("Segoe UI", 9.700001F);
            MessegeTextBox.Form = null;
            MessegeTextBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            MessegeTextBox.GradientEndColor = Color.Gray;
            MessegeTextBox.GradientStartColor = Color.Gray;
            MessegeTextBox.GuidID = "a1e32ff6-3c1a-4bfc-abf0-a62bfe4d56df";
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
            MessegeTextBox.InnerTextBox.BackColor = Color.FromArgb(250, 250, 250);
            MessegeTextBox.InnerTextBox.BorderStyle = BorderStyle.None;
            MessegeTextBox.InnerTextBox.Dock = DockStyle.Fill;
            MessegeTextBox.InnerTextBox.Font = new Font("Segoe UI", 11.5F);
            MessegeTextBox.InnerTextBox.ForeColor = Color.FromArgb(60, 60, 60);
            MessegeTextBox.InnerTextBox.Location = new Point(0, 0);
            MessegeTextBox.InnerTextBox.Multiline = true;
            MessegeTextBox.InnerTextBox.Name = "";
            MessegeTextBox.InnerTextBox.Size = new Size(496, 174);
            MessegeTextBox.InnerTextBox.TabIndex = 0;
            MessegeTextBox.IsAcceptButton = false;
            MessegeTextBox.IsBorderAffectedByTheme = true;
            MessegeTextBox.IsCancelButton = false;
            MessegeTextBox.IsChild = false;
            MessegeTextBox.IsCustomeBorder = true;
            MessegeTextBox.IsDefault = false;
            MessegeTextBox.IsFocused = false;
            MessegeTextBox.IsFramless = false;
            MessegeTextBox.IsHovered = false;
            MessegeTextBox.IsPressed = false;
            MessegeTextBox.IsRounded = false;
            MessegeTextBox.IsRoundedAffectedByTheme = true;
            MessegeTextBox.IsShadowAffectedByTheme = true;
            MessegeTextBox.LeftoffsetForDrawingRect = 0;
            MessegeTextBox.LinkedProperty = null;
            MessegeTextBox.Location = new Point(6, 88);
            MessegeTextBox.MaskFormat = TextBoxMaskFormat.None;
            MessegeTextBox.MinimumSize = new Size(100, 30);
            MessegeTextBox.Modified = false;
            MessegeTextBox.Multiline = true;
            MessegeTextBox.Name = "MessegeTextBox";
            MessegeTextBox.OnlyCharacters = false;
            MessegeTextBox.OnlyDigits = false;
            MessegeTextBox.OverrideFontSize = TypeStyleFontSize.None;
            MessegeTextBox.ParentBackColor = Color.FromArgb(245, 222, 179);
            MessegeTextBox.PasswordChar = '\0';
            MessegeTextBox.PlaceholderText = "";
            MessegeTextBox.PressedBackColor = Color.Gray;
            MessegeTextBox.PressedBorderColor = Color.Gray;
            MessegeTextBox.PressedForeColor = Color.Black;
            MessegeTextBox.ReadOnly = false;
            MessegeTextBox.RightoffsetForDrawingRect = 0;
            MessegeTextBox.SavedGuidID = null;
            MessegeTextBox.SavedID = null;
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
            MessegeTextBox.ShowScrollbars = false;
            MessegeTextBox.ShowShadow = false;
            MessegeTextBox.ShowTopBorder = false;
            MessegeTextBox.ShowVerticalScrollBar = false;
            MessegeTextBox.Size = new Size(496, 174);
            MessegeTextBox.SlideFrom = SlideDirection.Left;
            MessegeTextBox.StaticNotMoving = false;
            MessegeTextBox.TabIndex = 2;
            MessegeTextBox.TextAlignment = HorizontalAlignment.Left;
            MessegeTextBox.TextImageRelation = TextImageRelation.ImageBeforeText;
            MessegeTextBox.Theme = EnumBeepThemes.ModernDarkTheme;
            MessegeTextBox.TimeFormat = "HH:mm:ss";
            MessegeTextBox.ToolTipText = "";
            MessegeTextBox.TopoffsetForDrawingRect = 0;
            MessegeTextBox.UseGradientBackground = false;
            MessegeTextBox.UseSystemPasswordChar = false;
            MessegeTextBox.WordWrap = true;
            // 
            // _spinnerImage
            // 
            _spinnerImage.ActiveBackColor = Color.FromArgb(80, 80, 80);
            _spinnerImage.AllowManualRotation = true;
            _spinnerImage.AnimationDuration = 500;
            _spinnerImage.AnimationType = DisplayAnimationType.None;
            _spinnerImage.ApplyThemeOnImage = false;
            _spinnerImage.ApplyThemeToChilds = true;
            _spinnerImage.BackColor = Color.FromArgb(30, 30, 30);
            _spinnerImage.BlockID = null;
            _spinnerImage.BorderColor = Color.Gray;
            _spinnerImage.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _spinnerImage.BorderRadius = 5;
            _spinnerImage.BorderStyle = BorderStyle.FixedSingle;
            _spinnerImage.BorderThickness = 1;
            _spinnerImage.BottomoffsetForDrawingRect = 0;
            _spinnerImage.BoundProperty = null;
            _spinnerImage.CanBeFocused = true;
            _spinnerImage.CanBeHovered = false;
            _spinnerImage.CanBePressed = true;
            _spinnerImage.Category = Utilities.DbFieldCategory.String;
            _spinnerImage.ComponentName = "_spinnerImage";
            _spinnerImage.DataContext = null;
            _spinnerImage.DataSourceProperty = null;
            _spinnerImage.DisabledBackColor = Color.Gray;
            _spinnerImage.DisabledForeColor = Color.Empty;
            _spinnerImage.DrawingRect = new Rectangle(0, 0, 105, 74);
            _spinnerImage.Easing = EasingType.Linear;
            _spinnerImage.FieldID = null;
            _spinnerImage.FocusBackColor = Color.FromArgb(80, 80, 80);
            _spinnerImage.FocusBorderColor = Color.Gray;
            _spinnerImage.FocusForeColor = Color.White;
            _spinnerImage.FocusIndicatorColor = Color.Blue;
            _spinnerImage.Form = null;
            _spinnerImage.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _spinnerImage.GradientEndColor = Color.FromArgb(60, 60, 60);
            _spinnerImage.GradientStartColor = Color.FromArgb(30, 30, 30);
            _spinnerImage.GuidID = "588ee86a-43ba-4945-9469-507ca30adac8";
            _spinnerImage.HoverBackColor = Color.FromArgb(70, 70, 70);
            _spinnerImage.HoverBorderColor = Color.CornflowerBlue;
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
            _spinnerImage.IsRounded = false;
            _spinnerImage.IsRoundedAffectedByTheme = true;
            _spinnerImage.IsShadowAffectedByTheme = true;
            _spinnerImage.IsSpinning = false;
            _spinnerImage.IsStillImage = false;
            _spinnerImage.LeftoffsetForDrawingRect = 0;
            _spinnerImage.LinkedProperty = null;
            _spinnerImage.Location = new Point(373, 49);
            _spinnerImage.ManualRotationAngle = 0F;
            _spinnerImage.Name = "_spinnerImage";
            _spinnerImage.OverrideFontSize = TypeStyleFontSize.None;
            _spinnerImage.ParentBackColor = Color.FromArgb(70, 130, 180);
            _spinnerImage.PressedBackColor = Color.FromArgb(80, 80, 80);
            _spinnerImage.PressedBorderColor = Color.Gray;
            _spinnerImage.PressedForeColor = Color.White;
            _spinnerImage.RightoffsetForDrawingRect = 0;
            _spinnerImage.SavedGuidID = null;
            _spinnerImage.SavedID = null;
            _spinnerImage.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _spinnerImage.ShadowColor = Color.Black;
            _spinnerImage.ShadowOffset = 0;
            _spinnerImage.ShadowOpacity = 0.5F;
            _spinnerImage.ShowAllBorders = false;
            _spinnerImage.ShowBottomBorder = false;
            _spinnerImage.ShowFocusIndicator = false;
            _spinnerImage.ShowLeftBorder = false;
            _spinnerImage.ShowRightBorder = false;
            _spinnerImage.ShowShadow = false;
            _spinnerImage.ShowTopBorder = false;
            _spinnerImage.Size = new Size(105, 74);
            _spinnerImage.SlideFrom = SlideDirection.Left;
            _spinnerImage.SpinSpeed = 5F;
            _spinnerImage.StaticNotMoving = false;
            _spinnerImage.TabIndex = 1;
            _spinnerImage.Text = "beepImage1";
            _spinnerImage.Theme = EnumBeepThemes.ModernDarkTheme;
            _spinnerImage.ToolTipText = "";
            _spinnerImage.TopoffsetForDrawingRect = 0;
            _spinnerImage.UseGradientBackground = false;
            // 
            // BeepWaitScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.Gray;
            ClientSize = new Size(508, 268);
            Controls.Add(MessegeTextBox);
            Controls.Add(TitleLabel1);
            Controls.Add(LogoImage);
            Name = "BeepWaitScreen";
            StartPosition = FormStartPosition.CenterScreen;
            Theme = EnumBeepThemes.ModernDarkTheme;
            TopMost = true;
            MessegeTextBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BeepImage LogoImage;
        private BeepLabel TitleLabel1;
        private BeepTextBox MessegeTextBox;
        private BeepImage _spinnerImage;
    }
}