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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            beepButton1 = new BeepButton();
            beepImage1 = new BeepImage();
            button1 = new Button();
            SuspendLayout();
            // 
            // beepButton1
            // 
            beepButton1.AnimationDuration = 500;
            beepButton1.AnimationType = DisplayAnimationType.None;
            beepButton1.ApplyThemeOnImage = false;
            beepButton1.ApplyThemeToChilds = true;
            beepButton1.AutoDrawHitListComponents = true;
            beepButton1.AutoSizeContent = false;
            beepButton1.BackColor = Color.White;
            beepButton1.BadgeBackColor = Color.Red;
            beepButton1.BadgeFont = new Font("Segoe UI", 8F, FontStyle.Bold);
            beepButton1.BadgeForeColor = Color.White;
            beepButton1.BadgeShape = BadgeShape.Circle;
            beepButton1.BadgeText = "";
            beepButton1.BlockID = null;
            beepButton1.BorderColor = Color.FromArgb(33, 150, 243);
            beepButton1.BorderDashStyle = DashStyle.Solid;
            beepButton1.BorderPainter = BeepControlStyle.None;
            beepButton1.BorderRadius = 8;
            beepButton1.BorderSize = 1;
            beepButton1.BorderStyle = BorderStyle.FixedSingle;
            beepButton1.BorderThickness = 1;
            beepButton1.BottomoffsetForDrawingRect = 0;
            beepButton1.BoundProperty = null;
            beepButton1.ButtonErrorText = "";
            beepButton1.ButtonHasError = false;
            beepButton1.ButtonHelperText = "";
            beepButton1.ButtonLabel = "";
            beepButton1.ButtonMinSize = new Size(32, 32);
            beepButton1.ButtonType = ButtonType.Normal;
            beepButton1.CanBeFocused = true;
            beepButton1.CanBeHovered = true;
            beepButton1.CanBePressed = true;
            beepButton1.CanBeSelected = true;
            beepButton1.Category = Utilities.DbFieldCategory.Boolean;
            beepButton1.ComponentName = "beepButton1";
            beepButton1.DataContext = null;
            beepButton1.DataSourceProperty = null;
            beepButton1.DisabledBackColor = Color.FromArgb(200, 200, 200);
            beepButton1.DisabledBorderColor = Color.Gray;
            beepButton1.DisabledForeColor = Color.Gray;
            beepButton1.DrawingRect = new Rectangle(0, 0, 100, 36);
            beepButton1.Easing = EasingType.Linear;
            beepButton1.EnableHighQualityRendering = true;
            beepButton1.EnableMaterialStyle = false;
            beepButton1.EnableRippleEffect = false;
            beepButton1.EnableSplashEffect = false;
            beepButton1.ErrorColor = Color.FromArgb(176, 0, 32);
            beepButton1.ErrorText = "";
            beepButton1.ExternalDrawingLayer = DrawingLayer.AfterAll;
            beepButton1.FieldID = null;
            beepButton1.FilledBackgroundColor = Color.FromArgb(245, 245, 245);
            beepButton1.FloatingLabel = true;
            beepButton1.FocusBackColor = Color.FromArgb(25, 118, 210);
            beepButton1.FocusBorderColor = Color.RoyalBlue;
            beepButton1.FocusForeColor = Color.White;
            beepButton1.FocusIndicatorColor = Color.RoyalBlue;
            beepButton1.Font = new Font("Arial", 10F);
            beepButton1.Form = null;
            beepButton1.GlassmorphismBlur = 10F;
            beepButton1.GlassmorphismOpacity = 0.1F;
            beepButton1.GradientAngle = 0F;
            beepButton1.GradientDirection = LinearGradientMode.Horizontal;
            beepButton1.GradientEndColor = Color.Gray;
            beepButton1.GradientStartColor = Color.LightGray;
            beepButton1.GridMode = false;
            beepButton1.GuidID = "fca2ff28-2ae1-4d19-a16c-d26fd0de32a8";
            beepButton1.HasError = false;
            beepButton1.HelperText = "";
            beepButton1.HelperTextOn = false;
            beepButton1.HideText = false;
            beepButton1.HitAreaEventOn = false;
            beepButton1.HitTestControl = null;
            beepButton1.HoverBackColor = Color.FromArgb(227, 242, 253);
            beepButton1.HoverBorderColor = Color.Blue;
            beepButton1.HoveredBackcolor = Color.FromArgb(227, 242, 253);
            beepButton1.HoverForeColor = Color.FromArgb(33, 150, 243);
            beepButton1.IconSize = 20;
            beepButton1.Id = -1;
            beepButton1.Image = null;
            beepButton1.ImageAlign = ContentAlignment.MiddleLeft;
            beepButton1.ImageClicked = null;
            beepButton1.ImageEmbededin = ImageEmbededin.Button;
            beepButton1.ImagePath = null;
            beepButton1.InactiveBorderColor = Color.Gray;
            beepButton1.InnerShape = null;
            beepButton1.IsAcceptButton = false;
            beepButton1.IsBorderAffectedByTheme = true;
            beepButton1.IsCancelButton = false;
            beepButton1.IsChild = false;
            beepButton1.IsColorFromTheme = true;
            beepButton1.IsCustomeBorder = false;
            beepButton1.IsDefault = false;
            beepButton1.IsDeleted = false;
            beepButton1.IsDirty = false;
            beepButton1.IsEditable = true;
            beepButton1.IsFocused = false;
            beepButton1.IsFrameless = false;
            beepButton1.IsHovered = false;
            beepButton1.IsNew = false;
            beepButton1.IsPopupOpen = false;
            beepButton1.IsPressed = false;
            beepButton1.IsReadOnly = false;
            beepButton1.IsRequired = false;
            beepButton1.IsRounded = true;
            beepButton1.IsRoundedAffectedByTheme = true;
            beepButton1.IsSelected = false;
            beepButton1.IsSelectedOptionOn = false;
            beepButton1.IsShadowAffectedByTheme = true;
            beepButton1.IsSideMenuChild = false;
            beepButton1.IsStillButton = false;
            beepButton1.IsTransparentBackground = false;
            beepButton1.IsValid = true;
            beepButton1.IsVisible = true;
            beepButton1.Items = (List<object>)resources.GetObject("beepButton1.Items");
            beepButton1.LabelText = "";
            beepButton1.LabelTextOn = false;
            beepButton1.LeadingIconPath = "";
            beepButton1.LeadingImagePath = "";
            beepButton1.LeftoffsetForDrawingRect = 0;
            beepButton1.LinkedProperty = null;
            beepButton1.Location = new Point(183, 166);
            beepButton1.Margin = new Padding(0);
            beepButton1.MaterialBorderVariant = MaterialTextFieldVariant.Standard;
            beepButton1.MaterialCustomPadding = new Padding(0);
            beepButton1.MaterialFillColor = Color.FromArgb(245, 245, 245);
            beepButton1.MaterialIconPadding = 8;
            beepButton1.MaterialIconSize = 20;
            beepButton1.MaterialOutlineColor = Color.FromArgb(140, 140, 140);
            beepButton1.MaterialPreserveContentArea = true;
            beepButton1.MaterialPrimaryColor = Color.FromArgb(25, 118, 210);
            beepButton1.MaxHitListDrawPerFrame = 0;
            beepButton1.MaxImageSize = new Size(32, 32);
            beepButton1.ModernGradientType = ModernGradientType.None;
            beepButton1.Name = "beepButton1";
            beepButton1.OverrideFontSize = TypeStyleFontSize.None;
            beepButton1.PainterKind = BaseControlPainterKind.Classic;
            beepButton1.ParentBackColor = Color.Empty;
            beepButton1.ParentControl = null;
            beepButton1.PopPosition = BeepPopupFormPosition.Bottom;
            beepButton1.PopupListForm = null;
            beepButton1.PopupMode = false;
            beepButton1.PressedBackColor = Color.FromArgb(21, 101, 192);
            beepButton1.PressedBorderColor = Color.DarkGray;
            beepButton1.PressedForeColor = Color.White;
            beepButton1.RadialCenter = (PointF)resources.GetObject("beepButton1.RadialCenter");
            beepButton1.RightoffsetForDrawingRect = 0;
            beepButton1.SavedGuidID = null;
            beepButton1.SavedID = null;
            beepButton1.ScaleMode = ImageScaleMode.KeepAspectRatio;
            beepButton1.SelectedBackColor = Color.FromArgb(25, 118, 210);
            beepButton1.SelectedBorderColor = Color.Green;
            beepButton1.SelectedForeColor = Color.White;
            beepButton1.SelectedIndex = -1;
            beepButton1.SelectedItem = null;
            beepButton1.SelectedValue = null;
            beepButton1.ShadowColor = Color.Black;
            beepButton1.ShadowOffset = 3;
            beepButton1.ShadowOpacity = 0.25F;
            beepButton1.ShowAllBorders = false;
            beepButton1.ShowBottomBorder = false;
            beepButton1.ShowFocusIndicator = false;
            beepButton1.ShowLeftBorder = false;
            beepButton1.ShowRightBorder = false;
            beepButton1.ShowShadow = false;
            beepButton1.ShowTopBorder = false;
            beepButton1.SlideFrom = SlideDirection.Left;
            beepButton1.SplashColor = Color.Gray;
            beepButton1.StandardImages = (List<SimpleItem>)resources.GetObject("beepButton1.StandardImages");
            beepButton1.StaticNotMoving = false;
            beepButton1.TabIndex = 0;
            beepButton1.Tag = this;
            beepButton1.TempBackColor = Color.LightGray;
            beepButton1.Text = "beepButton1";
            beepButton1.TextAlign = ContentAlignment.MiddleCenter;
            beepButton1.TextFont = new Font("Arial", 10F);
            beepButton1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepButton1.Theme = null;
            beepButton1.ToolTipText = null;
            beepButton1.TopoffsetForDrawingRect = 0;
            beepButton1.TrailingIconPath = "";
            beepButton1.TrailingImagePath = "";
            beepButton1.UseExternalBufferedGraphics = true;
            beepButton1.UseFormStylePaint = true;
            beepButton1.UseGlassmorphism = false;
            beepButton1.UseGradientBackground = false;
            beepButton1.UseRichToolTip = true;
            beepButton1.UseScaledFont = false;
            beepButton1.UseThemeFont = true;
            // 
            // beepImage1
            // 
            beepImage1.AllowManualRotation = true;
            beepImage1.AnimationDuration = 500;
            beepImage1.AnimationType = DisplayAnimationType.None;
            beepImage1.ApplyThemeOnImage = false;
            beepImage1.ApplyThemeToChilds = true;
            beepImage1.AutoDrawHitListComponents = true;
            beepImage1.BackColor = SystemColors.Control;
            beepImage1.BadgeBackColor = Color.FromArgb(33, 150, 243);
            beepImage1.BadgeFont = new Font("Segoe UI", 8F, FontStyle.Bold);
            beepImage1.BadgeForeColor = Color.White;
            beepImage1.BadgeShape = BadgeShape.Circle;
            beepImage1.BadgeText = "";
            beepImage1.BaseSize = 50;
            beepImage1.BlockID = null;
            beepImage1.BorderColor = Color.FromArgb(33, 150, 243);
            beepImage1.BorderDashStyle = DashStyle.Solid;
            beepImage1.BorderPainter = BeepControlStyle.None;
            beepImage1.BorderRadius = 8;
            beepImage1.BorderStyle = BorderStyle.FixedSingle;
            beepImage1.BorderThickness = 1;
            beepImage1.BottomoffsetForDrawingRect = 0;
            beepImage1.BoundProperty = "ImagePath";
            beepImage1.CanBeFocused = false;
            beepImage1.CanBeHovered = true;
            beepImage1.CanBePressed = true;
            beepImage1.CanBeSelected = true;
            beepImage1.Category = Utilities.DbFieldCategory.String;
            beepImage1.ClipShape = ImageClipShape.None;
            beepImage1.ComponentName = "beepImage1";
            beepImage1.CornerRadius = 10F;
            beepImage1.CustomClipPath = null;
            beepImage1.DataContext = null;
            beepImage1.DataSourceProperty = null;
            beepImage1.DisabledBackColor = Color.FromArgb(200, 200, 200);
            beepImage1.DisabledBorderColor = Color.LightGray;
            beepImage1.DisabledForeColor = Color.Gray;
            beepImage1.DrawingRect = new Rectangle(0, 0, 100, 100);
            beepImage1.Easing = EasingType.Linear;
            beepImage1.EnableHighQualityRendering = true;
            beepImage1.EnableMaterialStyle = false;
            beepImage1.EnableRippleEffect = false;
            beepImage1.EnableSplashEffect = false;
            beepImage1.ErrorColor = Color.FromArgb(176, 0, 32);
            beepImage1.ErrorText = "";
            beepImage1.ExternalDrawingLayer = DrawingLayer.AfterAll;
            beepImage1.FieldID = null;
            beepImage1.FillColor = Color.Black;
            beepImage1.FilledBackgroundColor = Color.FromArgb(245, 245, 245);
            beepImage1.FloatingLabel = true;
            beepImage1.FocusBackColor = Color.LightYellow;
            beepImage1.FocusBorderColor = Color.RoyalBlue;
            beepImage1.FocusForeColor = Color.Black;
            beepImage1.FocusIndicatorColor = Color.RoyalBlue;
            beepImage1.ForeColor = Color.FromArgb(33, 150, 243);
            beepImage1.Form = null;
            beepImage1.GlassmorphismBlur = 10F;
            beepImage1.GlassmorphismOpacity = 0.1F;
            beepImage1.GradientAngle = 0F;
            beepImage1.GradientDirection = LinearGradientMode.Horizontal;
            beepImage1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepImage1.GradientStartColor = Color.FromArgb(255, 255, 255);
            beepImage1.Grayscale = false;
            beepImage1.GridMode = false;
            beepImage1.GuidID = "23fe82b2-e0c5-44ce-b858-8e9d59d923f7";
            beepImage1.HasError = false;
            beepImage1.HelperText = "";
            beepImage1.HelperTextOn = false;
            beepImage1.HitAreaEventOn = false;
            beepImage1.HitTestControl = null;
            beepImage1.HoverBackColor = Color.LightBlue;
            beepImage1.HoverBorderColor = Color.Blue;
            beepImage1.HoveredBackcolor = Color.LightBlue;
            beepImage1.HoverForeColor = Color.Black;
            beepImage1.IconSize = 20;
            beepImage1.Id = -1;
            beepImage1.Image = null;
            beepImage1.ImageEmbededin = ImageEmbededin.Button;
            beepImage1.ImagePath = null;
            beepImage1.InactiveBorderColor = Color.Gray;
            beepImage1.InnerShape = null;
            beepImage1.IsAcceptButton = false;
            beepImage1.IsBorderAffectedByTheme = true;
            beepImage1.IsBouncing = false;
            beepImage1.IsCancelButton = false;
            beepImage1.IsChild = true;
            beepImage1.IsCustomeBorder = false;
            beepImage1.IsDefault = false;
            beepImage1.IsDeleted = false;
            beepImage1.IsDirty = false;
            beepImage1.IsEditable = true;
            beepImage1.IsFading = false;
            beepImage1.IsFocused = false;
            beepImage1.IsFrameless = false;
            beepImage1.IsHovered = false;
            beepImage1.IsNew = false;
            beepImage1.IsPressed = false;
            beepImage1.IsPulsing = false;
            beepImage1.IsReadOnly = false;
            beepImage1.IsRequired = false;
            beepImage1.IsRounded = true;
            beepImage1.IsRoundedAffectedByTheme = true;
            beepImage1.IsSelected = false;
            beepImage1.IsSelectedOptionOn = false;
            beepImage1.IsShadowAffectedByTheme = true;
            beepImage1.IsShaking = false;
            beepImage1.IsSpinning = false;
            beepImage1.IsStillImage = false;
            beepImage1.IsTransparentBackground = false;
            beepImage1.IsValid = true;
            beepImage1.IsVisible = true;
            beepImage1.Items = (List<object>)resources.GetObject("beepImage1.Items");
            beepImage1.LabelText = "";
            beepImage1.LabelTextOn = false;
            beepImage1.LeadingIconPath = "";
            beepImage1.LeadingImagePath = "";
            beepImage1.LeftoffsetForDrawingRect = 0;
            beepImage1.LinkedProperty = null;
            beepImage1.Location = new Point(179, 252);
            beepImage1.ManualRotationAngle = 0F;
            beepImage1.MaterialBorderVariant = MaterialTextFieldVariant.Standard;
            beepImage1.MaterialCustomPadding = new Padding(0);
            beepImage1.MaterialFillColor = Color.FromArgb(245, 245, 245);
            beepImage1.MaterialIconPadding = 8;
            beepImage1.MaterialIconSize = 20;
            beepImage1.MaterialOutlineColor = Color.FromArgb(140, 140, 140);
            beepImage1.MaterialPrimaryColor = Color.FromArgb(25, 118, 210);
            beepImage1.MaxHitListDrawPerFrame = 0;
            beepImage1.MinimumSize = new Size(16, 16);
            beepImage1.ModernGradientType = ModernGradientType.None;
            beepImage1.Name = "beepImage1";
            beepImage1.Opacity = 1F;
            beepImage1.OverrideFontSize = TypeStyleFontSize.None;
            beepImage1.PainterKind = BaseControlPainterKind.Classic;
            beepImage1.ParentBackColor = SystemColors.Control;
            beepImage1.ParentControl = null;
            beepImage1.PreserveSvgBackgrounds = false;
            beepImage1.PressedBackColor = Color.Gray;
            beepImage1.PressedBorderColor = Color.DarkGray;
            beepImage1.PressedForeColor = Color.White;
            beepImage1.RadialCenter = (PointF)resources.GetObject("beepImage1.RadialCenter");
            beepImage1.RightoffsetForDrawingRect = 0;
            beepImage1.SavedGuidID = null;
            beepImage1.SavedID = null;
            beepImage1.ScaleFactor = 1F;
            beepImage1.ScaleMode = ImageScaleMode.KeepAspectRatio;
            beepImage1.SelectedBackColor = Color.LightGreen;
            beepImage1.SelectedBorderColor = Color.Green;
            beepImage1.SelectedForeColor = Color.Black;
            beepImage1.SelectedValue = null;
            beepImage1.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            beepImage1.ShadowOffset = 3;
            beepImage1.ShadowOpacity = 0.25F;
            beepImage1.ShowAllBorders = false;
            beepImage1.ShowBottomBorder = false;
            beepImage1.ShowFocusIndicator = false;
            beepImage1.ShowLeftBorder = false;
            beepImage1.ShowRightBorder = false;
            beepImage1.ShowShadow = false;
            beepImage1.ShowTopBorder = false;
            beepImage1.SlideFrom = SlideDirection.Left;
            beepImage1.SpinSpeed = 5F;
            beepImage1.StaticNotMoving = false;
            beepImage1.StrokeColor = Color.Black;
            beepImage1.TabIndex = 1;
            beepImage1.Tag = this;
            beepImage1.TempBackColor = Color.LightGray;
            beepImage1.Text = "beepImage1";
            beepImage1.Theme = null;
            beepImage1.ToolTipText = null;
            beepImage1.TopoffsetForDrawingRect = 0;
            beepImage1.TrailingIconPath = "";
            beepImage1.TrailingImagePath = "";
            beepImage1.UseExternalBufferedGraphics = true;
            beepImage1.UseFormStylePaint = true;
            beepImage1.UseGlassmorphism = false;
            beepImage1.UseGradientBackground = false;
            beepImage1.UseRichToolTip = true;
            beepImage1.UseThemeFont = true;
            beepImage1.Velocity = 0F;
            // 
            // button1
            // 
            button1.Location = new Point(410, 255);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(beepImage1);
            Controls.Add(beepButton1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private BeepButton beepButton1;
        private BeepImage beepImage1;
        private Button button1;
    }
}