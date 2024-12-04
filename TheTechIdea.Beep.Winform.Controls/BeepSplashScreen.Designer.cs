namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepSplashScreen
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
            _logoImage = new BeepImage();
            _titleLabel = new BeepLabel();
            SuspendLayout();
            // 
            // _logoImage
            // 
            _logoImage.ActiveBackColor = Color.FromArgb(0, 120, 215);
            _logoImage.AllowManualRotation = true;
            _logoImage.AnimationDuration = 500;
            _logoImage.AnimationType = DisplayAnimationType.None;
            _logoImage.ApplyThemeOnImage = false;
            _logoImage.ApplyThemeToChilds = true;
            _logoImage.BlockID = null;
            _logoImage.BorderColor = Color.FromArgb(200, 200, 200);
            _logoImage.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _logoImage.BorderRadius = 5;
            _logoImage.BorderStyle = BorderStyle.FixedSingle;
            _logoImage.BorderThickness = 1;
            _logoImage.BottomoffsetForDrawingRect = 0;
            _logoImage.BoundProperty = null;
            _logoImage.DataContext = null;
            _logoImage.DisabledBackColor = Color.Gray;
            _logoImage.DisabledForeColor = Color.Empty;
            _logoImage.DrawingRect = new Rectangle(4, 4, 438, 390);
            _logoImage.Easing = EasingType.Linear;
            _logoImage.FieldID = null;
            _logoImage.FocusBackColor = Color.FromArgb(0, 120, 215);
            _logoImage.FocusBorderColor = Color.Gray;
            _logoImage.FocusForeColor = Color.White;
            _logoImage.FocusIndicatorColor = Color.Blue;
            _logoImage.Form = null;
            _logoImage.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _logoImage.GradientEndColor = Color.FromArgb(230, 230, 230);
            _logoImage.GradientStartColor = Color.White;
            _logoImage.HoverBackColor = Color.FromArgb(230, 230, 230);
            _logoImage.HoverBorderColor = Color.FromArgb(0, 120, 215);
            _logoImage.HoveredBackcolor = Color.Wheat;
            _logoImage.HoverForeColor = Color.Black;
            _logoImage.Id = -1;
            _logoImage.Image = null;
            _logoImage.ImagePath = null;
            _logoImage.InactiveBackColor = Color.Gray;
            _logoImage.InactiveBorderColor = Color.Gray;
            _logoImage.InactiveForeColor = Color.Black;
            _logoImage.IsAcceptButton = false;
            _logoImage.IsBorderAffectedByTheme = true;
            _logoImage.IsCancelButton = false;
            _logoImage.IsChild = false;
            _logoImage.IsCustomeBorder = false;
            _logoImage.IsDefault = false;
            _logoImage.IsFocused = false;
            _logoImage.IsFramless = false;
            _logoImage.IsHovered = false;
            _logoImage.IsPressed = false;
            _logoImage.IsRounded = true;
            _logoImage.IsRoundedAffectedByTheme = true;
            _logoImage.IsShadowAffectedByTheme = true;
            _logoImage.IsSpinning = false;
            _logoImage.IsStillImage = false;
            _logoImage.LeftoffsetForDrawingRect = 0;
            _logoImage.Location = new Point(129, 95);
            _logoImage.ManualRotationAngle = 0F;
            _logoImage.Name = "_logoImage";
            _logoImage.OverrideFontSize = TypeStyleFontSize.None;
            _logoImage.ParentBackColor = Color.Empty;
            _logoImage.PressedBackColor = Color.FromArgb(0, 120, 215);
            _logoImage.PressedBorderColor = Color.Gray;
            _logoImage.PressedForeColor = Color.White;
            _logoImage.RightoffsetForDrawingRect = 0;
            _logoImage.SavedGuidID = null;
            _logoImage.SavedID = null;
            _logoImage.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _logoImage.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            _logoImage.ShadowOffset = 3;
            _logoImage.ShadowOpacity = 0.5F;
            _logoImage.ShowAllBorders = true;
            _logoImage.ShowBottomBorder = true;
            _logoImage.ShowFocusIndicator = false;
            _logoImage.ShowLeftBorder = true;
            _logoImage.ShowRightBorder = true;
            _logoImage.ShowShadow = true;
            _logoImage.ShowTopBorder = true;
            _logoImage.Size = new Size(443, 395);
            _logoImage.SlideFrom = SlideDirection.Left;
            _logoImage.SpinSpeed = 5F;
            _logoImage.StaticNotMoving = false;
            _logoImage.TabIndex = 0;
            _logoImage.Text = "beepImage1";
            _logoImage.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            _logoImage.ToolTipText = "";
            _logoImage.TopoffsetForDrawingRect = 0;
            _logoImage.UseGradientBackground = false;
            // 
            // _titleLabel
            // 
            _titleLabel.ActiveBackColor = Color.Gray;
            _titleLabel.AnimationDuration = 500;
            _titleLabel.AnimationType = DisplayAnimationType.None;
            _titleLabel.ApplyThemeOnImage = false;
            _titleLabel.ApplyThemeToChilds = true;
            _titleLabel.BackColor = SystemColors.Control;
            _titleLabel.BlockID = null;
            _titleLabel.BorderColor = Color.Black;
            _titleLabel.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _titleLabel.BorderRadius = 5;
            _titleLabel.BorderStyle = BorderStyle.FixedSingle;
            _titleLabel.BorderThickness = 1;
            _titleLabel.BottomoffsetForDrawingRect = 0;
            _titleLabel.BoundProperty = "Text";
            _titleLabel.DataContext = null;
            _titleLabel.DisabledBackColor = Color.Gray;
            _titleLabel.DisabledForeColor = Color.Empty;
            _titleLabel.DrawingRect = new Rectangle(3, 3, 411, 23);
            _titleLabel.Easing = EasingType.Linear;
            _titleLabel.FieldID = null;
            _titleLabel.FocusBackColor = Color.Gray;
            _titleLabel.FocusBorderColor = Color.Gray;
            _titleLabel.FocusForeColor = Color.Black;
            _titleLabel.FocusIndicatorColor = Color.Blue;
            _titleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _titleLabel.ForeColor = Color.Black;
            _titleLabel.Form = null;
            _titleLabel.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _titleLabel.GradientEndColor = Color.Gray;
            _titleLabel.GradientStartColor = Color.Gray;
            _titleLabel.HideText = false;
            _titleLabel.HoverBackColor = Color.FromArgb(230, 230, 230);
            _titleLabel.HoverBorderColor = Color.Gray;
            _titleLabel.HoveredBackcolor = Color.Wheat;
            _titleLabel.HoverForeColor = Color.Black;
            _titleLabel.Id = -1;
            _titleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            _titleLabel.ImagePath = null;
            _titleLabel.InactiveBackColor = Color.Gray;
            _titleLabel.InactiveBorderColor = Color.Gray;
            _titleLabel.InactiveForeColor = Color.Black;
            _titleLabel.IsAcceptButton = false;
            _titleLabel.IsBorderAffectedByTheme = true;
            _titleLabel.IsCancelButton = false;
            _titleLabel.IsChild = true;
            _titleLabel.IsCustomeBorder = false;
            _titleLabel.IsDefault = false;
            _titleLabel.IsFocused = false;
            _titleLabel.IsFramless = false;
            _titleLabel.IsHovered = false;
            _titleLabel.IsPressed = false;
            _titleLabel.IsRounded = true;
            _titleLabel.IsRoundedAffectedByTheme = true;
            _titleLabel.IsShadowAffectedByTheme = true;
            _titleLabel.LeftoffsetForDrawingRect = 0;
            _titleLabel.Location = new Point(135, 37);
            _titleLabel.Margin = new Padding(0);
            _titleLabel.MaxImageSize = new Size(16, 16);
            _titleLabel.MaximumSize = new Size(0, 27);
            _titleLabel.MinimumSize = new Size(0, 27);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.OverrideFontSize = TypeStyleFontSize.None;
            _titleLabel.ParentBackColor = SystemColors.Control;
            _titleLabel.PressedBackColor = Color.Gray;
            _titleLabel.PressedBorderColor = Color.Gray;
            _titleLabel.PressedForeColor = Color.Black;
            _titleLabel.RightoffsetForDrawingRect = 0;
            _titleLabel.SavedGuidID = null;
            _titleLabel.SavedID = null;
            _titleLabel.ShadowColor = Color.Black;
            _titleLabel.ShadowOffset = 3;
            _titleLabel.ShadowOpacity = 0.5F;
            _titleLabel.ShowAllBorders = false;
            _titleLabel.ShowBottomBorder = false;
            _titleLabel.ShowFocusIndicator = false;
            _titleLabel.ShowLeftBorder = false;
            _titleLabel.ShowRightBorder = false;
            _titleLabel.ShowShadow = true;
            _titleLabel.ShowTopBorder = false;
            _titleLabel.Size = new Size(415, 27);
            _titleLabel.SlideFrom = SlideDirection.Left;
            _titleLabel.StaticNotMoving = false;
            _titleLabel.TabIndex = 1;
            _titleLabel.Text = "beepLabel1";
            _titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            _titleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            _titleLabel.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            _titleLabel.ToolTipText = "";
            _titleLabel.TopoffsetForDrawingRect = 0;
            _titleLabel.UseGradientBackground = false;
            // 
            // BeepSplashScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(701, 604);
            Controls.Add(_titleLabel);
            Controls.Add(_logoImage);
            FormBorderStyle = FormBorderStyle.None;
            Name = "BeepSplashScreen";
            Opacity = 0.1D;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private BeepImage _logoImage;
        private BeepLabel _titleLabel;
    }
}