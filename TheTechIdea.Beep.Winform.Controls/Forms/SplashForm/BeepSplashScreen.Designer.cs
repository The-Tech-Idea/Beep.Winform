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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepSplashScreen));
            _logoImage = new BeepImage();
            _titleLabel = new BeepLabel();
            SuspendLayout();
         
            // 
            // _logoImage
            // 
            _logoImage.AllowManualRotation = true;
            _logoImage.AnimationDuration = 500;
            _logoImage.AnimationType = DisplayAnimationType.None;
            _logoImage.ApplyThemeOnImage = false;
            _logoImage.ApplyThemeToChilds = true;
            _logoImage.AutoDrawHitListComponents = true;
            _logoImage.BackColor = Color.White;
            _logoImage.BadgeBackColor = Color.Red;
            _logoImage.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _logoImage.BadgeForeColor = Color.White;
            _logoImage.BadgeShape = BadgeShape.Circle;
            _logoImage.BadgeText = "";
            _logoImage.BaseSize = 50;
            _logoImage.BlockID = null;
            _logoImage.BorderColor = Color.FromArgb(33, 150, 243);
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
            _logoImage.ClipShape = ImageClipShape.None;
            _logoImage.ComponentName = "_logoImage";
            _logoImage.CornerRadius = 10F;
            _logoImage.CustomClipPath = null;
            _logoImage.DataSourceProperty = null;
            _logoImage.DisabledBackColor = Color.Gray;
            _logoImage.DisabledBorderColor = Color.Empty;
            _logoImage.DisabledForeColor = Color.Empty;
            // REMOVED: DisableDpiAndScaling - .NET 8/9+ handles DPI automatically
            _logoImage.DrawingRect = new Rectangle(0, 0, 443, 395);
            _logoImage.Easing = EasingType.Linear;
         
            _logoImage.Enabled = true;
            _logoImage.EnableHighQualityRendering = true;
            _logoImage.EnableRippleEffect = false;
            _logoImage.EnableSplashEffect = true;
            _logoImage.ExternalDrawingLayer = Models.DrawingLayer.AfterAll;
            _logoImage.FieldID = null;
            _logoImage.FillColor = Color.Black;
            _logoImage.FilledBackgroundColor = Color.FromArgb(20, 0, 0, 0);
            _logoImage.FloatingLabel = true;
            _logoImage.FocusBackColor = Color.FromArgb(41, 128, 185);
            _logoImage.FocusBorderColor = Color.Gray;
            _logoImage.FocusForeColor = Color.White;
            _logoImage.FocusIndicatorColor = Color.Blue;
            _logoImage.ForeColor = Color.FromArgb(33, 150, 243);
            _logoImage.Form = null;
            _logoImage.GlassmorphismBlur = 10F;
            _logoImage.GlassmorphismOpacity = 0.1F;
            _logoImage.GradientAngle = 0F;
            _logoImage.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _logoImage.GradientEndColor = Color.FromArgb(230, 230, 230);
            _logoImage.GradientStartColor = Color.FromArgb(255, 255, 255);
            _logoImage.Grayscale = false;
            _logoImage.GridMode = false;
            _logoImage.GuidID = "664a5370-11d7-486c-aa04-d713199ea5e8";
            _logoImage.HelperText = "";
            _logoImage.HitAreaEventOn = false;
            _logoImage.HitTestControl = null;
            _logoImage.HoverBackColor = Color.FromArgb(41, 128, 185);
            _logoImage.HoverBorderColor = Color.FromArgb(52, 152, 219);
            _logoImage.HoveredBackcolor = Color.Wheat;
            _logoImage.HoverForeColor = Color.White;
            _logoImage.Id = -1;
            _logoImage.Image = null;
            _logoImage.ImageEmbededin = ImageEmbededin.Button;
            _logoImage.ImagePath = null;
            _logoImage.InactiveBorderColor = Color.Gray;
            _logoImage.IsAcceptButton = false;
            _logoImage.IsBorderAffectedByTheme = true;
            _logoImage.IsBouncing = false;
            _logoImage.IsCancelButton = false;
            _logoImage.IsChild = false;
            _logoImage.IsCustomeBorder = false;
            _logoImage.IsDefault = false;
            _logoImage.IsDeleted = false;
            _logoImage.IsDirty = false;
            _logoImage.IsEditable = false;
            _logoImage.IsFading = false;
            _logoImage.IsFocused = false;
            _logoImage.IsFrameless = false;
            _logoImage.IsHovered = false;
            _logoImage.IsNew = false;
            _logoImage.IsPressed = false;
            _logoImage.IsPulsing = false;
            _logoImage.IsReadOnly = false;
            _logoImage.IsRequired = false;
            _logoImage.IsRounded = false;
            _logoImage.IsRoundedAffectedByTheme = true;
            _logoImage.IsSelected = false;
            _logoImage.IsSelectedOptionOn = false;
            _logoImage.IsShadowAffectedByTheme = true;
            _logoImage.IsShaking = false;
            _logoImage.IsSpinning = false;
            _logoImage.IsStillImage = false;
            _logoImage.IsVisible = false;
            _logoImage.Items = (List<object>)resources.GetObject("_logoImage.Items");
            _logoImage.LabelText = "";
            _logoImage.LeftoffsetForDrawingRect = 0;
            _logoImage.LinkedProperty = null;
            _logoImage.Location = new Point(129, 95);
            _logoImage.ManualRotationAngle = 0F;
            _logoImage.MaterialBorderVariant = MaterialTextFieldVariant.Standard;
            _logoImage.MaxHitListDrawPerFrame = 0;
            _logoImage.ModernGradientType = ModernGradientType.None;
            _logoImage.Name = "_logoImage";
            _logoImage.Opacity = 1F;
            _logoImage.OverrideFontSize = TypeStyleFontSize.None;
            _logoImage.ParentBackColor = Color.Empty;
            _logoImage.ParentControl = null;
            _logoImage.PreserveSvgBackgrounds = false;
            _logoImage.PressedBackColor = Color.FromArgb(41, 128, 185);
            _logoImage.PressedBorderColor = Color.White;
            _logoImage.PressedForeColor = Color.White;
            _logoImage.RadialCenter = (PointF)resources.GetObject("_logoImage.RadialCenter");
            _logoImage.RightoffsetForDrawingRect = 0;
            _logoImage.SavedGuidID = null;
            _logoImage.SavedID = null;
            _logoImage.ScaleFactor = 1F;
            _logoImage.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _logoImage.SelectedBackColor = Color.White;
            _logoImage.SelectedBorderColor = Color.Empty;
            _logoImage.SelectedForeColor = Color.Black;
            _logoImage.SelectedValue = null;
            _logoImage.ShadowColor = Color.FromArgb(50, 0, 0, 0);
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
            _logoImage.StrokeColor = Color.Black;
            _logoImage.TabIndex = 0;
            _logoImage.Tag = this;
            _logoImage.TempBackColor = Color.Empty;
            _logoImage.Text = "beepImage1";
            _logoImage.Theme = "DefaultType";
            _logoImage.ToolTipText = "";
            _logoImage.TopoffsetForDrawingRect = 0;
          
            _logoImage.UseExternalBufferedGraphics = false;
            _logoImage.UseGlassmorphism = false;
            _logoImage.UseGradientBackground = false;
            _logoImage.UseThemeFont = true;
            _logoImage.Velocity = 0F;
            // 
            // _titleLabel
            // 
            _titleLabel.AnimationDuration = 500;
            _titleLabel.AnimationType = DisplayAnimationType.None;
            _titleLabel.ApplyThemeOnImage = false;
            _titleLabel.ApplyThemeToChilds = true;
            _titleLabel.AutoDrawHitListComponents = true;
            _titleLabel.BackColor = Color.White;
            _titleLabel.BadgeBackColor = Color.Red;
            _titleLabel.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _titleLabel.BadgeForeColor = Color.White;
            _titleLabel.BadgeShape = BadgeShape.Circle;
            _titleLabel.BadgeText = "";
            _titleLabel.BlockID = null;
            _titleLabel.BorderColor = Color.LightGray;
            _titleLabel.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _titleLabel.BorderRadius = 4;
            _titleLabel.BorderStyle = BorderStyle.FixedSingle;
            _titleLabel.BorderThickness = 1;
            _titleLabel.BottomoffsetForDrawingRect = 0;
            _titleLabel.BoundProperty = "Text";
            _titleLabel.CanBeFocused = true;
            _titleLabel.CanBeHovered = false;
            _titleLabel.CanBePressed = true;
            _titleLabel.Category = Utilities.DbFieldCategory.String;
            _titleLabel.ComponentName = "_titleLabel";
            _titleLabel.DataSourceProperty = null;
            _titleLabel.DisabledBackColor = Color.LightGray;
            _titleLabel.DisabledBorderColor = Color.Gray;
            _titleLabel.DisabledForeColor = Color.Gray;
            // REMOVED: DisableDpiAndScaling - .NET 8/9+ handles DPI automatically
            _titleLabel.DrawingRect = new Rectangle(1, 1, 441, 52);
            _titleLabel.Easing = EasingType.Linear;
            _titleLabel.EnableHighQualityRendering = true;
            _titleLabel.PainterKind= BaseControlPainterKind.Classic;
            _titleLabel.EnableRippleEffect = false;
            _titleLabel.EnableSplashEffect = false;
            _titleLabel.ErrorColor = Color.FromArgb(176, 0, 32);
            _titleLabel.ErrorText = "";
         
            _titleLabel.FieldID = null;
            _titleLabel.FilledBackgroundColor = Color.FromArgb(20, 0, 0, 0);
            _titleLabel.FloatingLabel = true;
            _titleLabel.FocusBackColor = Color.Gray;
            _titleLabel.FocusBorderColor = Color.Gray;
            _titleLabel.FocusForeColor = Color.Black;
            _titleLabel.FocusIndicatorColor = Color.Blue;
            _titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _titleLabel.ForeColor = Color.FromArgb(33, 33, 33);
            _titleLabel.Form = null;
            _titleLabel.GlassmorphismBlur = 10F;
            _titleLabel.GlassmorphismOpacity = 0.1F;
            _titleLabel.GradientAngle = 0F;
            _titleLabel.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _titleLabel.GradientEndColor = Color.Gray;
            _titleLabel.GradientStartColor = Color.Gray;
            _titleLabel.GridMode = false;
            _titleLabel.GuidID = "a235d68c-f2d6-433e-8c36-dbb7b51bc63e";
            _titleLabel.HasError = false;
            _titleLabel.HeaderSubheaderSpacing = 2;
            _titleLabel.HelperText = "";
            _titleLabel.HideText = false;
            _titleLabel.HitAreaEventOn = false;
            _titleLabel.HitTestControl = null;
            _titleLabel.HoverBackColor = Color.FromArgb(230, 240, 255);
            _titleLabel.HoverBorderColor = Color.Gray;
            _titleLabel.HoveredBackcolor = Color.FromArgb(230, 240, 255);
            _titleLabel.HoverForeColor = Color.FromArgb(33, 150, 243);
            _titleLabel.IconSize = 20;
            _titleLabel.Id = -1;
            _titleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            _titleLabel.ImagePath = null;
            _titleLabel.InactiveBorderColor = Color.Gray;
            _titleLabel.InnerShape = null;
            _titleLabel.IsAcceptButton = false;
            _titleLabel.IsBorderAffectedByTheme = true;
            _titleLabel.IsCancelButton = false;
            _titleLabel.IsChild = false;
            _titleLabel.IsCustomeBorder = false;
            _titleLabel.IsDefault = false;
            _titleLabel.IsDeleted = false;
            _titleLabel.IsDirty = false;
            _titleLabel.IsEditable = true;
            _titleLabel.IsFocused = false;
          
            _titleLabel.IsFrameless = false;
            _titleLabel.IsHovered = false;
 
            _titleLabel.IsNew = false;
            _titleLabel.IsPressed = false;
     
            _titleLabel.IsReadOnly = false;
            _titleLabel.IsRequired = false;
            _titleLabel.IsRounded = false;
            _titleLabel.IsRoundedAffectedByTheme = true;
            _titleLabel.IsSelected = false;
    
            _titleLabel.IsSelectedOptionOn = false;
            _titleLabel.IsShadowAffectedByTheme = true;
            _titleLabel.IsValid = true;
            _titleLabel.IsVisible = true;
            _titleLabel.Items = (List<object>)resources.GetObject("_titleLabel.Items");
            _titleLabel.LabelBackColor = Color.Empty;
            _titleLabel.LabelErrorText = "";
            _titleLabel.LabelHasError = false;
            _titleLabel.LabelHelperText = "";
            _titleLabel.LabelText = "";
            _titleLabel.LeadingIconPath = "";
            _titleLabel.LeadingImagePath = "";
            _titleLabel.LeftoffsetForDrawingRect = 0;
            _titleLabel.LinkedProperty = null;
            _titleLabel.Location = new Point(129, 24);
            _titleLabel.Margin = new Padding(0);
            _titleLabel.MaterialBorderRadius = 4;
            _titleLabel.MaterialBorderVariant = MaterialTextFieldVariant.Standard;
            _titleLabel.MaterialCustomPadding = new Padding(0);
            _titleLabel.MaterialFillColor = Color.FromArgb(245, 245, 245);
            _titleLabel.MaterialIconPadding = 8;
            _titleLabel.MaterialIconSize = 20;
            _titleLabel.MaterialOutlineColor = Color.FromArgb(140, 140, 140);
            _titleLabel.MaterialPrimaryColor = Color.FromArgb(25, 118, 210);
            _titleLabel.MaterialVariant = MaterialTextFieldVariant.Standard;
            _titleLabel.MaxHitListDrawPerFrame = 0;
            _titleLabel.MaxImageSize = new Size(16, 16);
            _titleLabel.MinimumSize = new Size(152, 40);
            _titleLabel.ModernGradientType = ModernGradientType.None;
            _titleLabel.Multiline = false;
            _titleLabel.Name = "_titleLabel";
            _titleLabel.OverrideFontSize = TypeStyleFontSize.None;
            _titleLabel.Padding = new Padding(1);
            _titleLabel.ParentBackColor = Color.Empty;
            _titleLabel.ParentControl = null;
            _titleLabel.PressedBackColor = Color.Gray;
            _titleLabel.PressedBorderColor = Color.Gray;
            _titleLabel.PressedForeColor = Color.Black;
            _titleLabel.RadialCenter = (PointF)resources.GetObject("_titleLabel.RadialCenter");
            _titleLabel.RightoffsetForDrawingRect = 0;
            _titleLabel.SavedGuidID = null;
            _titleLabel.SavedID = null;
            _titleLabel.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _titleLabel.SelectedBackColor = Color.FromArgb(200, 220, 255);
            _titleLabel.SelectedBorderColor = Color.Green;
            _titleLabel.SelectedForeColor = Color.FromArgb(33, 33, 33);
            _titleLabel.SelectedValue = null;
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
            _titleLabel.SubHeaderFont = new Font("Segoe UI", 16F);
            _titleLabel.SubHeaderForeColor = Color.FromArgb(83, 83, 83);
            _titleLabel.SubHeaderText = "";
            _titleLabel.TabIndex = 1;
            _titleLabel.Tag = this;
        
            _titleLabel.Text = "beepLabel1";
            _titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            _titleLabel.TextFont = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _titleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            _titleLabel.Theme = "DefaultType";
            _titleLabel.ToolTipText = "";
            _titleLabel.TopoffsetForDrawingRect = 0;
            _titleLabel.TrailingIconPath = "";
            _titleLabel.TrailingImagePath = "";
            _titleLabel.UseExternalBufferedGraphics = false;
            _titleLabel.UseGlassmorphism = false;
            _titleLabel.UseGradientBackground = false;
            _titleLabel.UseRichToolTip = true;
            _titleLabel.UseScaledFont = false;
            _titleLabel.UseThemeFont = false;
            // 
            // BeepSplashScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
           
            ClientSize = new Size(715, 637);
            Controls.Add(_titleLabel);
            Controls.Add(_logoImage);
          
         
            Name = "BeepSplashScreen";
            Opacity = 0.1D;
        
            ShowCaptionBar = false;
            StartPosition = FormStartPosition.CenterScreen;
          
            Theme = "DefaultType";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private BeepImage _logoImage;
        private BeepLabel _titleLabel;
    }
}