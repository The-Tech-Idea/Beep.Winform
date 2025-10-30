using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;

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
            button1 = new Button();
            label1 = new Label();
            beepMenuBar1 = new BeepMenuBar();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = Color.Transparent;
            button1.Location = new Point(304, 163);
            button1.Name = "button1";
            button1.Size = new Size(137, 59);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.ForeColor = Color.White;
            label1.Location = new Point(304, 270);
            label1.Name = "label1";
            label1.Size = new Size(207, 35);
            label1.TabIndex = 1;
            label1.Text = "label1";
            // 
            // beepMenuBar1
            // 
            beepMenuBar1.ActiveMenuButton = null;
            beepMenuBar1.AnimationDuration = 500;
            beepMenuBar1.AnimationType = DisplayAnimationType.None;
            beepMenuBar1.ApplyThemeToChilds = true;
            beepMenuBar1.AutoDrawHitListComponents = true;
            beepMenuBar1.BackColor = Color.Transparent;
            beepMenuBar1.BadgeBackColor = Color.Red;
            beepMenuBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepMenuBar1.BadgeForeColor = Color.White;
            beepMenuBar1.BadgeShape = BadgeShape.Circle;
            beepMenuBar1.BadgeText = "";
            beepMenuBar1.BlockID = null;
            beepMenuBar1.BorderColor = Color.LightGray;
            beepMenuBar1.BorderDashStyle = DashStyle.Solid;
            beepMenuBar1.BorderPainter = BeepControlStyle.None;
            beepMenuBar1.BorderRadius = 8;
            beepMenuBar1.BorderStyle = BorderStyle.FixedSingle;
            beepMenuBar1.BorderThickness = 1;
            beepMenuBar1.BottomoffsetForDrawingRect = 0;
            beepMenuBar1.BoundProperty = "SelectedMenuItem";
            beepMenuBar1.CanBeFocused = false;
            beepMenuBar1.CanBeHovered = false;
            beepMenuBar1.CanBePressed = false;
            beepMenuBar1.CanBeSelected = false;
            beepMenuBar1.Category = Utilities.DbFieldCategory.String;
            beepMenuBar1.ComponentName = "BaseControl";
            beepMenuBar1.DataContext = null;
            beepMenuBar1.DataSourceProperty = null;
            beepMenuBar1.DisabledBackColor = Color.LightGray;
            beepMenuBar1.DisabledBorderColor = Color.Gray;
            beepMenuBar1.DisabledForeColor = Color.DarkGray;
            beepMenuBar1.DrawingRect = new Rectangle(0, 0, 755, 34);
            beepMenuBar1.Easing = EasingType.Linear;
            beepMenuBar1.EnableHighQualityRendering = true;
            beepMenuBar1.EnableMaterialStyle = false;
            beepMenuBar1.EnableRippleEffect = false;
            beepMenuBar1.EnableSplashEffect = false;
            beepMenuBar1.ErrorColor = Color.FromArgb(176, 0, 32);
            beepMenuBar1.ErrorText = "";
            beepMenuBar1.ExternalDrawingLayer = DrawingLayer.AfterAll;
            beepMenuBar1.FieldID = null;
            beepMenuBar1.FilledBackgroundColor = Color.FromArgb(245, 245, 245);
            beepMenuBar1.FloatingLabel = true;
            beepMenuBar1.FocusBackColor = Color.LightYellow;
            beepMenuBar1.FocusBorderColor = Color.RoyalBlue;
            beepMenuBar1.FocusForeColor = Color.Black;
            beepMenuBar1.FocusIndicatorColor = Color.RoyalBlue;
            beepMenuBar1.ForeColor = Color.Black;
            beepMenuBar1.Form = null;
            beepMenuBar1.GlassmorphismBlur = 10F;
            beepMenuBar1.GlassmorphismOpacity = 0.1F;
            beepMenuBar1.GradientAngle = 0F;
            beepMenuBar1.GradientDirection = LinearGradientMode.Horizontal;
            beepMenuBar1.GradientEndColor = Color.Gray;
            beepMenuBar1.GradientStartColor = Color.LightGray;
            beepMenuBar1.GridMode = false;
            beepMenuBar1.GuidID = "a40b833b-1c51-4c8c-9864-3a3e50bf3a7b";
            beepMenuBar1.HasError = false;
            beepMenuBar1.HelperText = "";
            beepMenuBar1.HelperTextOn = false;
            beepMenuBar1.HitAreaEventOn = false;
            beepMenuBar1.HitTestControl = null;
            beepMenuBar1.HoverBackColor = Color.LightBlue;
            beepMenuBar1.HoverBorderColor = Color.Blue;
            beepMenuBar1.HoveredBackcolor = Color.LightBlue;
            beepMenuBar1.HoverForeColor = Color.Black;
            beepMenuBar1.IconSize = 20;
            beepMenuBar1.Id = -1;
            beepMenuBar1.ImageSize = 20;
            beepMenuBar1.InactiveBorderColor = Color.Gray;
            beepMenuBar1.InnerShape = null;
            beepMenuBar1.IsAcceptButton = false;
            beepMenuBar1.IsBorderAffectedByTheme = false;
            beepMenuBar1.IsCancelButton = false;
            beepMenuBar1.IsChild = true;
            beepMenuBar1.IsCustomeBorder = false;
            beepMenuBar1.IsDefault = false;
            beepMenuBar1.IsDeleted = false;
            beepMenuBar1.IsDirty = false;
            beepMenuBar1.IsEditable = true;
            beepMenuBar1.IsFocused = false;
            beepMenuBar1.IsFrameless = true;
            beepMenuBar1.IsHovered = false;
            beepMenuBar1.IsNew = false;
            beepMenuBar1.IsPressed = false;
            beepMenuBar1.IsReadOnly = false;
            beepMenuBar1.IsRequired = false;
            beepMenuBar1.IsRounded = false;
            beepMenuBar1.IsRoundedAffectedByTheme = false;
            beepMenuBar1.IsSelected = false;
            beepMenuBar1.IsSelectedOptionOn = false;
            beepMenuBar1.IsShadowAffectedByTheme = false;
            beepMenuBar1.IsTransparentBackground = false;
            beepMenuBar1.IsValid = true;
            beepMenuBar1.IsVisible = true;
            beepMenuBar1.Items = (List<object>)resources.GetObject("beepMenuBar1.Items");
            beepMenuBar1.LabelText = "";
            beepMenuBar1.LabelTextOn = false;
            beepMenuBar1.LeadingIconPath = "";
            beepMenuBar1.LeadingImagePath = "";
            beepMenuBar1.LeftoffsetForDrawingRect = 0;
            beepMenuBar1.LinkedProperty = null;
            beepMenuBar1.Location = new Point(67, 81);
            beepMenuBar1.MaterialBorderVariant = MaterialTextFieldVariant.Standard;
            beepMenuBar1.MaterialCustomPadding = new Padding(0);
            beepMenuBar1.MaterialFillColor = Color.FromArgb(245, 245, 245);
            beepMenuBar1.MaterialIconPadding = 8;
            beepMenuBar1.MaterialIconSize = 20;
            beepMenuBar1.MaterialOutlineColor = Color.FromArgb(140, 140, 140);
            beepMenuBar1.MaterialPrimaryColor = Color.FromArgb(25, 118, 210);
            beepMenuBar1.MaxHitListDrawPerFrame = 0;
           beepMenuBar1.MenuItemHeight = 32;
            beepMenuBar1.MenuItemWidth = 60;
            beepMenuBar1.MenuStyle = Winform.Controls.Forms.ModernForm.FormStyle.Modern;
            beepMenuBar1.ModernGradientType = ModernGradientType.None;
            beepMenuBar1.Name = "beepMenuBar1";
            beepMenuBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepMenuBar1.PainterKind = BaseControlPainterKind.Classic;
            beepMenuBar1.ParentBackColor = Color.FromArgb(0, 0, 0);
            beepMenuBar1.ParentControl = null;
            beepMenuBar1.PressedBackColor = Color.Gray;
            beepMenuBar1.PressedBorderColor = Color.DarkGray;
            beepMenuBar1.PressedForeColor = Color.White;
            beepMenuBar1.RadialCenter = (PointF)resources.GetObject("beepMenuBar1.RadialCenter");
            beepMenuBar1.RightoffsetForDrawingRect = 0;
            beepMenuBar1.SavedGuidID = null;
            beepMenuBar1.SavedID = null;
            beepMenuBar1.ScaleMode = ImageScaleMode.KeepAspectRatio;
            beepMenuBar1.SelectedBackColor = Color.LightGreen;
            beepMenuBar1.SelectedBorderColor = Color.Green;
            beepMenuBar1.SelectedForeColor = Color.Black;
            beepMenuBar1.SelectedIndex = -1;
            beepMenuBar1.SelectedValue = null;
            beepMenuBar1.ShadowColor = Color.Black;
            beepMenuBar1.ShadowOffset = 3;
            beepMenuBar1.ShadowOpacity = 0.25F;
            beepMenuBar1.ShowAllBorders = false;
            beepMenuBar1.ShowBottomBorder = false;
            beepMenuBar1.ShowFocusIndicator = false;
            beepMenuBar1.ShowLeftBorder = false;
            beepMenuBar1.ShowRightBorder = false;
            beepMenuBar1.ShowShadow = false;
            beepMenuBar1.ShowTopBorder = false;
            beepMenuBar1.Size = new Size(755, 34);
            beepMenuBar1.SlideFrom = SlideDirection.Left;
            beepMenuBar1.StaticNotMoving = false;
            beepMenuBar1.TabIndex = 2;
            beepMenuBar1.Tag = this;
            beepMenuBar1.TempBackColor = Color.LightGray;
            beepMenuBar1.Text = "beepMenuBar1";
            beepMenuBar1.TextFont = new Font("Arial", 10F);
            beepMenuBar1.Theme = null;
            beepMenuBar1.ToolTipText = null;
            beepMenuBar1.TopoffsetForDrawingRect = 0;
            beepMenuBar1.TrailingIconPath = "";
            beepMenuBar1.TrailingImagePath = "";
            beepMenuBar1.UseExternalBufferedGraphics = true;
            beepMenuBar1.UseFormStylePaint = true;
            beepMenuBar1.UseGlassmorphism = false;
            beepMenuBar1.UseGradientBackground = false;
            beepMenuBar1.UseRichToolTip = true;
            beepMenuBar1.UseThemeFont = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 0, 0);
            BackdropEffect = Winform.Controls.Forms.ModernForm.BackdropEffect.Mica;
            ClientSize = new Size(1706, 1456);
            Controls.Add(beepMenuBar1);
            Controls.Add(label1);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.Sizable;
            FormStyle = Winform.Controls.Forms.ModernForm.FormStyle.Terminal;
            Location = new Point(0, 0);
            Margin = new Padding(6);
            Name = "Form1";
            Padding = new Padding(10);
            ShowProfileButton = true;
            ShowSearchBox = true;
            ShowStyleButton = true;
            ShowThemeButton = true;
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Label label1;
        private BeepMenuBar beepMenuBar1;
    }
}