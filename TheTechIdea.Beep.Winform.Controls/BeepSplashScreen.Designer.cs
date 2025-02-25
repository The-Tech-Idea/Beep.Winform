using TheTechIdea.Beep.Vis.Modules;

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
            // beepuiManager1
            // 
            beepuiManager1.IsRounded = false;
            beepuiManager1.ShowBorder = false;
            // 
            // _logoImage
            // 
            _logoImage.ActiveBackColor = Color.FromArgb(41, 128, 185);
            _logoImage.AllowManualRotation = true;
            _logoImage.AnimationDuration = 500;
            _logoImage.AnimationType = DisplayAnimationType.None;
            _logoImage.ApplyThemeOnImage = false;
            _logoImage.ApplyThemeToChilds = true;
            _logoImage.BackColor = Color.LightSkyBlue;
            _logoImage.BlockID = null;
            _logoImage.BorderColor = Color.FromArgb(189, 195, 199);
            _logoImage.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _logoImage.BorderRadius = 5;
            _logoImage.BorderStyle = BorderStyle.FixedSingle;
            _logoImage.BorderThickness = 1;
            _logoImage.BottomoffsetForDrawingRect = 0;
            _logoImage.BoundProperty = null;
            _logoImage.CanBeFocused = true;
            _logoImage.CanBeHovered = false;
            _logoImage.CanBePressed = true;
            _logoImage.Category = Utilities.DbFieldCategory.String;
            _logoImage.ComponentName = "_logoImage";
            _logoImage.DataContext = null;
            _logoImage.DataSourceProperty = null;
            _logoImage.DisabledBackColor = Color.Gray;
            _logoImage.DisabledForeColor = Color.Empty;
            _logoImage.DrawingRect = new Rectangle(0, 0, 443, 395);
            _logoImage.Easing = EasingType.Linear;
            _logoImage.FieldID = null;
            _logoImage.FocusBackColor = Color.FromArgb(41, 128, 185);
            _logoImage.FocusBorderColor = Color.Gray;
            _logoImage.FocusForeColor = Color.White;
            _logoImage.FocusIndicatorColor = Color.Blue;
            _logoImage.Form = null;
            _logoImage.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _logoImage.GradientEndColor = Color.White;
            _logoImage.GradientStartColor = Color.White;
            _logoImage.GuidID = "664a5370-11d7-486c-aa04-d713199ea5e8";
            _logoImage.HoverBackColor = Color.FromArgb(41, 128, 185);
            _logoImage.HoverBorderColor = Color.FromArgb(52, 152, 219);
            _logoImage.HoveredBackcolor = Color.Wheat;
            _logoImage.HoverForeColor = Color.White;
            _logoImage.Id = -1;
            _logoImage.Image = null;
            _logoImage.ImageEmbededin = ImageEmbededin.Button;
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
            _logoImage.IsFrameless = false;
            _logoImage.IsHovered = false;
            _logoImage.IsPressed = false;
            _logoImage.IsRounded = false;
            _logoImage.IsRoundedAffectedByTheme = true;
            _logoImage.IsShadowAffectedByTheme = true;
            _logoImage.IsSpinning = false;
            _logoImage.IsStillImage = false;
            _logoImage.LeftoffsetForDrawingRect = 0;
            _logoImage.LinkedProperty = null;
            _logoImage.Location = new Point(129, 95);
            _logoImage.ManualRotationAngle = 0F;
            _logoImage.Name = "_logoImage";
            _logoImage.OverrideFontSize = TypeStyleFontSize.None;
            _logoImage.ParentBackColor = Color.Empty;
            _logoImage.PressedBackColor = Color.FromArgb(41, 128, 185);
            _logoImage.PressedBorderColor = Color.Gray;
            _logoImage.PressedForeColor = Color.White;
            _logoImage.RightoffsetForDrawingRect = 0;
            _logoImage.SavedGuidID = null;
            _logoImage.SavedID = null;
            _logoImage.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _logoImage.ShadowColor = Color.Empty;
            _logoImage.ShadowOffset = 0;
            _logoImage.ShadowOpacity = 0.5F;
            _logoImage.ShowAllBorders = false;
            _logoImage.ShowBottomBorder = false;
            _logoImage.ShowFocusIndicator = false;
            _logoImage.ShowLeftBorder = false;
            _logoImage.ShowRightBorder = false;
            _logoImage.ShowShadow = false;
            _logoImage.ShowTopBorder = false;
            _logoImage.Size = new Size(443, 395);
            _logoImage.SlideFrom = SlideDirection.Left;
            _logoImage.SpinSpeed = 5F;
            _logoImage.StaticNotMoving = false;
            _logoImage.TabIndex = 0;
            _logoImage.Text = "beepImage1";
            _logoImage.Theme = EnumBeepThemes.FlatDesignTheme;
            _logoImage.ToolTipText = "";
            _logoImage.TopoffsetForDrawingRect = 0;
            _logoImage.UseGradientBackground = false;
            _logoImage.UseThemeFont = true;
            // 
            // _titleLabel
            // 
            _titleLabel.ActiveBackColor = Color.Gray;
            _titleLabel.AnimationDuration = 500;
            _titleLabel.AnimationType = DisplayAnimationType.None;
            _titleLabel.ApplyThemeOnImage = false;
            _titleLabel.ApplyThemeToChilds = true;
            _titleLabel.BackColor = Color.White;
            _titleLabel.BlockID = null;
            _titleLabel.BorderColor = Color.Black;
            _titleLabel.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _titleLabel.BorderRadius = 1;
            _titleLabel.BorderStyle = BorderStyle.FixedSingle;
            _titleLabel.BorderThickness = 1;
            _titleLabel.BottomoffsetForDrawingRect = 0;
            _titleLabel.BoundProperty = "Text";
            _titleLabel.CanBeFocused = true;
            _titleLabel.CanBeHovered = false;
            _titleLabel.CanBePressed = true;
            _titleLabel.Category = Utilities.DbFieldCategory.String;
            _titleLabel.ComponentName = "_titleLabel";
            _titleLabel.DataContext = null;
            _titleLabel.DataSourceProperty = null;
            _titleLabel.DisabledBackColor = Color.Gray;
            _titleLabel.DisabledForeColor = Color.Empty;
            _titleLabel.DrawingRect = new Rectangle(1, 1, 441, 52);
            _titleLabel.Easing = EasingType.Linear;
            _titleLabel.FieldID = null;
            _titleLabel.FocusBackColor = Color.Gray;
            _titleLabel.FocusBorderColor = Color.Gray;
            _titleLabel.FocusForeColor = Color.Black;
            _titleLabel.FocusIndicatorColor = Color.Blue;
            _titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _titleLabel.ForeColor = Color.FromArgb(44, 62, 80);
            _titleLabel.Form = null;
            _titleLabel.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _titleLabel.GradientEndColor = Color.Gray;
            _titleLabel.GradientStartColor = Color.Gray;
            _titleLabel.GuidID = "a235d68c-f2d6-433e-8c36-dbb7b51bc63e";
            _titleLabel.HideText = false;
            _titleLabel.HoverBackColor = Color.White;
            _titleLabel.HoverBorderColor = Color.Gray;
            _titleLabel.HoveredBackcolor = Color.Wheat;
            _titleLabel.HoverForeColor = Color.White;
            _titleLabel.Id = -1;
            _titleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            _titleLabel.ImagePath = null;
            _titleLabel.InactiveBackColor = Color.Gray;
            _titleLabel.InactiveBorderColor = Color.Gray;
            _titleLabel.InactiveForeColor = Color.Black;
            _titleLabel.IsAcceptButton = false;
            _titleLabel.IsBorderAffectedByTheme = true;
            _titleLabel.IsCancelButton = false;
            _titleLabel.IsChild = false;
            _titleLabel.IsCustomeBorder = false;
            _titleLabel.IsDefault = false;
            _titleLabel.IsFocused = false;
            _titleLabel.IsFrameless = false;
            _titleLabel.IsHovered = false;
            _titleLabel.IsPressed = false;
            _titleLabel.IsRounded = false;
            _titleLabel.IsRoundedAffectedByTheme = true;
            _titleLabel.IsShadowAffectedByTheme = true;
            _titleLabel.LabelBackColor = Color.Empty;
            _titleLabel.LeftoffsetForDrawingRect = 0;
            _titleLabel.LinkedProperty = null;
            _titleLabel.Location = new Point(129, 24);
            _titleLabel.Margin = new Padding(0);
            _titleLabel.MaxImageSize = new Size(16, 16);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.OverrideFontSize = TypeStyleFontSize.None;
            _titleLabel.Padding = new Padding(1);
            _titleLabel.ParentBackColor = Color.Empty;
            _titleLabel.PressedBackColor = Color.Gray;
            _titleLabel.PressedBorderColor = Color.Gray;
            _titleLabel.PressedForeColor = Color.Black;
            _titleLabel.RightoffsetForDrawingRect = 0;
            _titleLabel.SavedGuidID = null;
            _titleLabel.SavedID = null;
            _titleLabel.ShadowColor = Color.Black;
            _titleLabel.ShadowOffset = 0;
            _titleLabel.ShadowOpacity = 0.5F;
            _titleLabel.ShowAllBorders = false;
            _titleLabel.ShowBottomBorder = false;
            _titleLabel.ShowFocusIndicator = false;
            _titleLabel.ShowLeftBorder = false;
            _titleLabel.ShowRightBorder = false;
            _titleLabel.ShowShadow = false;
            _titleLabel.ShowTopBorder = false;
            _titleLabel.Size = new Size(443, 54);
            _titleLabel.SlideFrom = SlideDirection.Left;
            _titleLabel.StaticNotMoving = false;
            _titleLabel.TabIndex = 1;
            _titleLabel.Text = "beepLabel1";
            _titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            _titleLabel.TextFont = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _titleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            _titleLabel.Theme = EnumBeepThemes.FlatDesignTheme;
            _titleLabel.ToolTipText = "";
            _titleLabel.TopoffsetForDrawingRect = 0;
            _titleLabel.UseGradientBackground = false;
            _titleLabel.UseScaledFont = false;
            _titleLabel.UseThemeFont = false;
            // 
            // BeepSplashScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(701, 604);
            Controls.Add(_titleLabel);
            Controls.Add(_logoImage);
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