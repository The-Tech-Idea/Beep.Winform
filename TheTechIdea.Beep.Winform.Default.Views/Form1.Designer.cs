namespace TheTechIdea.Beep.Winform.Default.Views
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
            beepPanel1 = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            beepButton1 = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            beepPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // beepPanel1
            // 
            beepPanel1.AnimationDuration = 500;
            beepPanel1.AnimationType = Vis.Modules.DisplayAnimationType.None;
            beepPanel1.ApplyThemeToChilds = true;
            beepPanel1.AutoDrawHitListComponents = true;
            beepPanel1.BackColor = Color.FromArgb(245, 245, 245);
            beepPanel1.BadgeBackColor = Color.FromArgb(33, 150, 243);
            beepPanel1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepPanel1.BadgeForeColor = Color.White;
            beepPanel1.BadgeShape = Vis.Modules.BadgeShape.Circle;
            beepPanel1.BadgeText = "";
            beepPanel1.BlockID = null;
            beepPanel1.BorderColor = Color.FromArgb(200, 200, 200);
            beepPanel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepPanel1.BorderRadius = 6;
            beepPanel1.BorderStyle = BorderStyle.FixedSingle;
            beepPanel1.BorderThickness = 1;
            beepPanel1.BottomoffsetForDrawingRect = 0;
            beepPanel1.BoundProperty = null;
            beepPanel1.CanBeFocused = true;
            beepPanel1.CanBeHovered = true;
            beepPanel1.CanBePressed = true;
            beepPanel1.Category = Utilities.DbFieldCategory.String;
            beepPanel1.ComponentName = "BaseControl";
            beepPanel1.Controls.Add(beepButton1);
            beepPanel1.DataSourceProperty = null;
            beepPanel1.DisabledBackColor = Color.FromArgb(200, 200, 200);
            beepPanel1.DisabledBorderColor = Color.LightGray;
            beepPanel1.DisabledForeColor = Color.Gray;
            beepPanel1.DisableDpiAndScaling = true;
            beepPanel1.DrawingRect = new Rectangle(24, 8, 292, 268);
            beepPanel1.Easing = Vis.Modules.EasingType.Linear;
            beepPanel1.EnableHighQualityRendering = true;
            beepPanel1.EnableRippleEffect = false;
            beepPanel1.EnableSplashEffect = false;
           
          
            beepPanel1.ExternalDrawingLayer = Winform.Controls.Models.DrawingLayer.AfterAll;
            beepPanel1.FieldID = null;
            beepPanel1.FilledBackgroundColor = Color.FromArgb(245, 245, 245);
            beepPanel1.FloatingLabel = true;
            beepPanel1.FocusBackColor = Color.LightYellow;
            beepPanel1.FocusBorderColor = Color.RoyalBlue;
            beepPanel1.FocusForeColor = Color.Black;
            beepPanel1.FocusIndicatorColor = Color.RoyalBlue;
            beepPanel1.ForeColor = Color.FromArgb(33, 37, 41);
            beepPanel1.Form = null;
            beepPanel1.GlassmorphismBlur = 10F;
            beepPanel1.GlassmorphismOpacity = 0.1F;
            beepPanel1.GradientAngle = 0F;
            beepPanel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepPanel1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepPanel1.GradientStartColor = Color.FromArgb(255, 255, 255);
            beepPanel1.GridMode = false;
            beepPanel1.GuidID = "7c2028b3-8bd5-46c8-b85e-34290b2d2294";
        
            beepPanel1.HelperText = "";
            beepPanel1.HitAreaEventOn = false;
            beepPanel1.HitTestControl = null;
            beepPanel1.HoverBackColor = Color.LightBlue;
            beepPanel1.HoverBorderColor = Color.Blue;
            beepPanel1.HoveredBackcolor = Color.LightBlue;
            beepPanel1.HoverForeColor = Color.Black;
           
            beepPanel1.Id = -1;
            beepPanel1.InactiveBorderColor = Color.Gray;
            beepPanel1.IsAcceptButton = false;
            beepPanel1.IsBorderAffectedByTheme = true;
            beepPanel1.IsCancelButton = false;
            beepPanel1.IsChild = true;
            beepPanel1.IsCustomeBorder = false;
            beepPanel1.IsDefault = false;
            beepPanel1.IsDeleted = false;
            beepPanel1.IsDirty = false;
            beepPanel1.IsEditable = true;
            beepPanel1.IsFocused = false;
            beepPanel1.IsFrameless = false;
            beepPanel1.IsHovered = false;
            beepPanel1.IsNew = false;
            beepPanel1.IsPressed = false;
            beepPanel1.IsReadOnly = false;
            beepPanel1.IsRequired = false;
            beepPanel1.IsRounded = true;
            beepPanel1.IsRoundedAffectedByTheme = true;
            beepPanel1.IsSelected = false;
            beepPanel1.IsSelectedOptionOn = false;
            beepPanel1.IsShadowAffectedByTheme = true;
          
            beepPanel1.IsVisible = true;
            beepPanel1.Items = (List<object>)resources.GetObject("beepPanel1.Items");
            beepPanel1.LabelText = "";
        
            beepPanel1.LeftoffsetForDrawingRect = 0;
            beepPanel1.LinkedProperty = null;
            beepPanel1.Location = new Point(284, 67);
            beepPanel1.MaterialBorderVariant = Vis.Modules.MaterialTextFieldVariant.Standard;
           
            beepPanel1.MaxHitListDrawPerFrame = 0;
            beepPanel1.ModernGradientType = Vis.Modules.ModernGradientType.None;
            beepPanel1.Name = "beepPanel1";
            beepPanel1.OverrideFontSize = Vis.Modules.TypeStyleFontSize.None;
            beepPanel1.ParentBackColor = Color.Empty;
            beepPanel1.ParentControl = null;
            beepPanel1.PressedBackColor = Color.Gray;
            beepPanel1.PressedBorderColor = Color.DarkGray;
            beepPanel1.PressedForeColor = Color.White;
            beepPanel1.RadialCenter = (PointF)resources.GetObject("beepPanel1.RadialCenter");
            beepPanel1.RightoffsetForDrawingRect = 0;
            beepPanel1.SavedGuidID = null;
            beepPanel1.SavedID = null;
       
            beepPanel1.SelectedBackColor = Color.LightGreen;
            beepPanel1.SelectedBorderColor = Color.Green;
            beepPanel1.SelectedForeColor = Color.Black;
            beepPanel1.SelectedValue = null;
            beepPanel1.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            beepPanel1.ShadowOffset = 3;
            beepPanel1.ShadowOpacity = 0.25F;
            beepPanel1.ShowAllBorders = false;
            beepPanel1.ShowBottomBorder = false;
            beepPanel1.ShowFocusIndicator = false;
            beepPanel1.ShowLeftBorder = false;
            beepPanel1.ShowRightBorder = false;
            beepPanel1.ShowShadow = false;
            beepPanel1.ShowTitle = true;
            beepPanel1.ShowTitleLine = true;
            beepPanel1.ShowTitleLineinFullWidth = true;
            beepPanel1.ShowTopBorder = false;
            beepPanel1.Size = new Size(340, 284);
            beepPanel1.SlideFrom = Vis.Modules.SlideDirection.Left;
            beepPanel1.StaticNotMoving = false;
            beepPanel1.TabIndex = 0;
            beepPanel1.Tag = this;
            beepPanel1.TempBackColor = Color.Empty;
            beepPanel1.Text = "beepPanel1";
            beepPanel1.TextFont = new Font("Arial", 14F);
            beepPanel1.Theme = null;
            beepPanel1.TitleAlignment = ContentAlignment.TopLeft;
            beepPanel1.TitleBottomY = 39;
            beepPanel1.TitleLineColor = Color.FromArgb(33, 33, 33);
            beepPanel1.TitleLineThickness = 2;
            beepPanel1.TitleText = "Panel Title";
            beepPanel1.ToolTipText = null;
            beepPanel1.TopoffsetForDrawingRect = 0;
         
            beepPanel1.UIAnimation = Vis.Modules.ReactUIAnimation.None;
            beepPanel1.UIColor = Vis.Modules.ReactUIColor.Primary;
            beepPanel1.UICustomElevation = 0;
            beepPanel1.UIDensity = Vis.Modules.ReactUIDensity.Standard;
            beepPanel1.UIDisabled = false;
            beepPanel1.UIElevation = Vis.Modules.ReactUIElevation.None;
            beepPanel1.UIFullWidth = false;
            beepPanel1.UIShape = Vis.Modules.ReactUIShape.Rounded;
            beepPanel1.UISize = Vis.Modules.ReactUISize.Medium;
            beepPanel1.UIVariant = Vis.Modules.ReactUIVariant.Default;
            beepPanel1.UseExternalBufferedGraphics = false;
            beepPanel1.UseGlassmorphism = false;
            beepPanel1.UseGradientBackground = false;
     
            beepPanel1.UseThemeFont = true;
            // 
            // beepButton1
            // 
            beepButton1.AnimationDuration = 500;
            beepButton1.AnimationType = Vis.Modules.DisplayAnimationType.None;
            beepButton1.ApplyThemeOnImage = false;
            beepButton1.ApplyThemeToChilds = true;
            beepButton1.AutoDrawHitListComponents = true;
            beepButton1.BadgeBackColor = Color.Red;
            beepButton1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepButton1.BadgeForeColor = Color.White;
            beepButton1.BadgeShape = Vis.Modules.BadgeShape.Circle;
            beepButton1.BadgeText = "";
            beepButton1.BlockID = null;
            beepButton1.BorderColor = Color.Black;
            beepButton1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepButton1.BorderRadius = 8;
            beepButton1.BorderSize = 1;
            beepButton1.BorderStyle = BorderStyle.FixedSingle;
            beepButton1.BorderThickness = 0;
            beepButton1.BottomoffsetForDrawingRect = 0;
            beepButton1.BoundProperty = null;
            beepButton1.ButtonErrorText = "";
            beepButton1.ButtonHasError = false;
            beepButton1.ButtonHelperText = "";
            beepButton1.ButtonLabel = "";
            beepButton1.ButtonType = Vis.Modules.ButtonType.Normal;
            beepButton1.CanBeFocused = true;
            beepButton1.CanBeHovered = true;
            beepButton1.CanBePressed = true;
            beepButton1.Category = Utilities.DbFieldCategory.Boolean;
            beepButton1.ComponentName = "BaseControl";
            beepButton1.DataSourceProperty = null;
            beepButton1.DisabledBackColor = Color.LightGray;
            beepButton1.DisabledBorderColor = Color.Gray;
            beepButton1.DisabledForeColor = Color.DarkGray;
            beepButton1.DisableDpiAndScaling = true;
            beepButton1.DrawingRect = new Rectangle(24, 8, 79, 24);
            beepButton1.Easing = Vis.Modules.EasingType.Linear;
            beepButton1.EmbeddedImagePath = null;
            beepButton1.EnableHighQualityRendering = true;
            beepButton1.EnableRippleEffect = false;
            beepButton1.EnableSplashEffect = false;
            beepButton1.ErrorColor = Color.FromArgb(176, 0, 32);
            beepButton1.ErrorText = "";
            beepButton1.ExternalDrawingLayer = Winform.Controls.Models.DrawingLayer.AfterAll;
            beepButton1.FieldID = null;
            beepButton1.FilledBackgroundColor = Color.FromArgb(245, 245, 245);
            beepButton1.FloatingLabel = true;
            beepButton1.FocusBackColor = Color.LightYellow;
            beepButton1.FocusBorderColor = Color.RoyalBlue;
            beepButton1.FocusForeColor = Color.Black;
            beepButton1.FocusIndicatorColor = Color.RoyalBlue;
            beepButton1.Form = null;
            beepButton1.GlassmorphismBlur = 10F;
            beepButton1.GlassmorphismOpacity = 0.1F;
            beepButton1.GradientAngle = 0F;
            beepButton1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepButton1.GradientEndColor = Color.Gray;
            beepButton1.GradientStartColor = Color.LightGray;
            beepButton1.GridMode = false;
            beepButton1.GuidID = "d4b2731d-c73d-4dfe-ab7f-c2d540a0cf66";
            beepButton1.HasError = false;
            beepButton1.HelperText = "";
            beepButton1.HideText = false;
            beepButton1.HitAreaEventOn = false;
            beepButton1.HitTestControl = null;
            beepButton1.HoverBackColor = Color.LightBlue;
            beepButton1.HoverBorderColor = Color.Blue;
            beepButton1.HoveredBackcolor = Color.LightBlue;
            beepButton1.HoverForeColor = Color.Black;
            beepButton1.IconSize = 20;
            beepButton1.Id = -1;
            beepButton1.Image = null;
            beepButton1.ImageAlign = ContentAlignment.MiddleLeft;
            beepButton1.ImageClicked = null;
            beepButton1.ImageEmbededin = Vis.Modules.ImageEmbededin.Button;
            beepButton1.ImagePath = null;
            beepButton1.InactiveBorderColor = Color.Gray;
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
            beepButton1.IsValid = true;
            beepButton1.IsVisible = true;
            beepButton1.Items = (List<object>)resources.GetObject("beepButton1.Items");
            beepButton1.LabelText = "";
            beepButton1.LeadingIconPath = "";
            beepButton1.LeadingImagePath = "";
            beepButton1.LeftoffsetForDrawingRect = 0;
            beepButton1.LinkedProperty = null;
            beepButton1.Location = new Point(23, 50);
            beepButton1.Margin = new Padding(0);
            beepButton1.MaterialBorderVariant = Vis.Modules.MaterialTextFieldVariant.Outlined;
            beepButton1.MaterialCustomPadding = new Padding(0);
            beepButton1.MaterialFillColor = Color.FromArgb(245, 245, 245);
            beepButton1.MaterialIconPadding = 8;
            beepButton1.MaterialIconSize = 20;
            beepButton1.MaterialOutlineColor = Color.FromArgb(140, 140, 140);
            beepButton1.MaterialPrimaryColor = Color.FromArgb(25, 118, 210);
            beepButton1.MaxHitListDrawPerFrame = 0;
            beepButton1.MaxImageSize = new Size(32, 32);
            beepButton1.MinimumSize = new Size(127, 40);
            beepButton1.ModernGradientType = Vis.Modules.ModernGradientType.None;
            beepButton1.Name = "beepButton1";
            beepButton1.OverrideFontSize = Vis.Modules.TypeStyleFontSize.None;
            beepButton1.ParentBackColor = Color.Empty;
            beepButton1.ParentControl = null;
            beepButton1.PopPosition = Vis.Modules.BeepPopupFormPosition.Bottom;
            beepButton1.PopupListForm = null;
            beepButton1.PopupMode = false;
            beepButton1.PressedBackColor = Color.Gray;
            beepButton1.PressedBorderColor = Color.DarkGray;
            beepButton1.PressedForeColor = Color.White;
            beepButton1.RadialCenter = (PointF)resources.GetObject("beepButton1.RadialCenter");
            beepButton1.RightoffsetForDrawingRect = 0;
            beepButton1.SavedGuidID = null;
            beepButton1.SavedID = null;
            beepButton1.ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio;
            beepButton1.SelectedBackColor = Color.LightGreen;
            beepButton1.SelectedBorderColor = Color.Green;
            beepButton1.SelectedForeColor = Color.Black;
            beepButton1.SelectedIndex = 0;
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
            beepButton1.Size = new Size(127, 40);
            beepButton1.SlideFrom = Vis.Modules.SlideDirection.Left;
            beepButton1.SplashColor = Color.Gray;
            beepButton1.StandardImages = (List<Controls.Models.SimpleItem>)resources.GetObject("beepButton1.StandardImages");
            beepButton1.StaticNotMoving = false;
            beepButton1.StylePreset = Winform.Controls.Models.MaterialTextFieldStylePreset.MaterialOutlined;
            beepButton1.TabIndex = 0;
            beepButton1.Tag = beepPanel1;
            beepButton1.TempBackColor = Color.Empty;
            beepButton1.Text = "beepButton1";
            beepButton1.TextAlign = ContentAlignment.MiddleCenter;
            beepButton1.TextFont = new Font("Arial", 10F);
            beepButton1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepButton1.Theme = null;
            beepButton1.ToolTipText = null;
            beepButton1.TopoffsetForDrawingRect = 0;
            beepButton1.TrailingIconPath = "";
            beepButton1.TrailingImagePath = "";
            beepButton1.UIAnimation = Vis.Modules.ReactUIAnimation.None;
            beepButton1.UIColor = Vis.Modules.ReactUIColor.Primary;
            beepButton1.UICustomElevation = 0;
            beepButton1.UIDensity = Vis.Modules.ReactUIDensity.Standard;
            beepButton1.UIDisabled = false;
            beepButton1.UIElevation = Vis.Modules.ReactUIElevation.None;
            beepButton1.UIFullWidth = false;
            beepButton1.UIShape = Vis.Modules.ReactUIShape.Rounded;
            beepButton1.UISize = Vis.Modules.ReactUISize.Medium;
            beepButton1.UIVariant = Vis.Modules.ReactUIVariant.Default;
            beepButton1.UseExternalBufferedGraphics = false;
            beepButton1.UseGlassmorphism = false;
            beepButton1.UseGradientBackground = false;
            beepButton1.UseRichToolTip = true;
            beepButton1.UseScaledFont = false;
            beepButton1.UseThemeFont = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(beepPanel1);
            Name = "Form1";
            Text = "Form1";
            beepPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Controls.BeepPanel beepPanel1;
        private Controls.BeepButton beepButton1;
    }
}