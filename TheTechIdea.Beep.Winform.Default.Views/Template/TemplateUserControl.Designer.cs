using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Default.Views.Template
{
    partial class TemplateUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateUserControl));
            MainTemplatePanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            SuspendLayout();
            // 
            // MainTemplatePanel
            // 
            MainTemplatePanel.AnimationDuration = 500;
            MainTemplatePanel.AnimationType = DisplayAnimationType.None;
            MainTemplatePanel.ApplyThemeToChilds = true;
            MainTemplatePanel.AutoDrawHitListComponents = true;
            MainTemplatePanel.BackColor = Color.FromArgb(245, 245, 245);
            MainTemplatePanel.BadgeBackColor = Color.FromArgb(33, 150, 243);
            MainTemplatePanel.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            MainTemplatePanel.BadgeForeColor = Color.White;
            MainTemplatePanel.BadgeShape = BadgeShape.Circle;
            MainTemplatePanel.BadgeText = "";
            MainTemplatePanel.BlockID = null;
            MainTemplatePanel.BorderColor = Color.FromArgb(200, 200, 200);
            MainTemplatePanel.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            MainTemplatePanel.BorderRadius = 6;
            MainTemplatePanel.BorderStyle = BorderStyle.FixedSingle;
            MainTemplatePanel.BorderThickness = 1;
            MainTemplatePanel.BottomoffsetForDrawingRect = 0;
            MainTemplatePanel.BoundProperty = null;
            MainTemplatePanel.CanBeFocused = false;
            MainTemplatePanel.CanBeHovered = false;
            MainTemplatePanel.CanBePressed = false;
            MainTemplatePanel.Category = Utilities.DbFieldCategory.String;
            MainTemplatePanel.ComponentName = "beepPanel1";
            MainTemplatePanel.DataSourceProperty = null;
            MainTemplatePanel.DisabledBackColor = Color.FromArgb(200, 200, 200);
            MainTemplatePanel.DisabledBorderColor = Color.LightGray;
            MainTemplatePanel.DisabledForeColor = Color.Gray;
            MainTemplatePanel.DisableDpiAndScaling = false;
            MainTemplatePanel.Dock = DockStyle.Fill;
            MainTemplatePanel.DrawingRect = new Rectangle(0, 0, 770, 432);
            MainTemplatePanel.Easing = EasingType.Linear;
            MainTemplatePanel.EnableHighQualityRendering = true;
            MainTemplatePanel.EnableMaterialStyle = false;
            MainTemplatePanel.EnableRippleEffect = false;
            MainTemplatePanel.EnableSplashEffect = false;
            MainTemplatePanel.ErrorColor = Color.FromArgb(176, 0, 32);
            MainTemplatePanel.ErrorText = "";
            MainTemplatePanel.ExternalDrawingLayer = DrawingLayer.AfterAll;
            MainTemplatePanel.FieldID = null;
            MainTemplatePanel.FilledBackgroundColor = Color.FromArgb(245, 245, 245);
            MainTemplatePanel.FloatingLabel = true;
            MainTemplatePanel.FocusBackColor = Color.White;
            MainTemplatePanel.FocusBorderColor = Color.Gray;
            MainTemplatePanel.FocusForeColor = Color.Black;
            MainTemplatePanel.FocusIndicatorColor = Color.Blue;
            MainTemplatePanel.Font = new Font("Arial", 16F);
            MainTemplatePanel.ForeColor = Color.FromArgb(33, 37, 41);
            MainTemplatePanel.Form = null;
            MainTemplatePanel.GlassmorphismBlur = 10F;
            MainTemplatePanel.GlassmorphismOpacity = 0.1F;
            MainTemplatePanel.GradientAngle = 0F;
            MainTemplatePanel.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            MainTemplatePanel.GradientEndColor = Color.FromArgb(230, 230, 230);
            MainTemplatePanel.GradientStartColor = Color.FromArgb(255, 255, 255);
            MainTemplatePanel.GridMode = false;
            MainTemplatePanel.GuidID = "905607c4-3ab1-429e-a4df-9ddaad4ec3ee";
            MainTemplatePanel.HasError = false;
            MainTemplatePanel.HelperText = "";
            MainTemplatePanel.HitAreaEventOn = false;
            MainTemplatePanel.HitTestControl = null;
            MainTemplatePanel.HoverBackColor = Color.Wheat;
            MainTemplatePanel.HoverBorderColor = Color.Gray;
            MainTemplatePanel.HoveredBackcolor = Color.Wheat;
            MainTemplatePanel.HoverForeColor = Color.Black;
            MainTemplatePanel.IconSize = 20;
            MainTemplatePanel.Id = -1;
            MainTemplatePanel.InactiveBorderColor = Color.Gray;
            MainTemplatePanel.InnerShape = null;
            MainTemplatePanel.IsAcceptButton = false;
            MainTemplatePanel.IsBorderAffectedByTheme = false;
            MainTemplatePanel.IsCancelButton = false;
            MainTemplatePanel.IsChild = false;
            MainTemplatePanel.IsCustomeBorder = false;
            MainTemplatePanel.IsDefault = false;
            MainTemplatePanel.IsDeleted = false;
            MainTemplatePanel.IsDirty = false;
            MainTemplatePanel.IsEditable = false;
            MainTemplatePanel.IsFocused = false;
            MainTemplatePanel.IsFocusedOn = false;
            MainTemplatePanel.IsFrameless = false;
            MainTemplatePanel.IsHovered = false;
            MainTemplatePanel.IsHoveringOn = false;
            MainTemplatePanel.IsNew = false;
            MainTemplatePanel.IsPressed = false;
            MainTemplatePanel.IsPressedOn = false;
            MainTemplatePanel.IsReadOnly = false;
            MainTemplatePanel.IsRequired = false;
            MainTemplatePanel.IsRounded = false;
            MainTemplatePanel.IsRoundedAffectedByTheme = false;
            MainTemplatePanel.IsSelected = false;
            MainTemplatePanel.IsSelectedOn = false;
            MainTemplatePanel.IsSelectedOptionOn = false;
            MainTemplatePanel.IsShadowAffectedByTheme = false;
            MainTemplatePanel.IsValid = true;
            MainTemplatePanel.IsVisible = true;
            MainTemplatePanel.Items = (List<object>)resources.GetObject("MainTemplatePanel.Items");
            MainTemplatePanel.LabelText = "";
            MainTemplatePanel.LeadingIconPath = "";
            MainTemplatePanel.LeadingImagePath = "";
            MainTemplatePanel.LeftoffsetForDrawingRect = 0;
            MainTemplatePanel.LinkedProperty = null;
            MainTemplatePanel.Location = new Point(0, 0);
            MainTemplatePanel.MaterialBorderVariant = MaterialTextFieldVariant.Standard;
            MainTemplatePanel.MaterialCustomPadding = new Padding(0);
            MainTemplatePanel.MaterialFillColor = Color.FromArgb(245, 245, 245);
            MainTemplatePanel.MaterialIconPadding = 8;
            MainTemplatePanel.MaterialIconSize = 20;
            MainTemplatePanel.MaterialOutlineColor = Color.FromArgb(140, 140, 140);
            MainTemplatePanel.MaterialPrimaryColor = Color.FromArgb(25, 118, 210);
            MainTemplatePanel.MaterialVariant = MaterialTextFieldVariant.Standard;
            MainTemplatePanel.MaxHitListDrawPerFrame = 0;
            MainTemplatePanel.ModernGradientType = ModernGradientType.None;
            MainTemplatePanel.Name = "MainTemplatePanel";
            MainTemplatePanel.OverrideFontSize = TypeStyleFontSize.None;
            MainTemplatePanel.ParentBackColor = Color.Empty;
            MainTemplatePanel.ParentControl = null;
            MainTemplatePanel.PressedBackColor = Color.White;
            MainTemplatePanel.PressedBorderColor = Color.Gray;
            MainTemplatePanel.PressedForeColor = Color.Gray;
            MainTemplatePanel.RadialCenter = (PointF)resources.GetObject("MainTemplatePanel.RadialCenter");
            MainTemplatePanel.RightoffsetForDrawingRect = 0;
            MainTemplatePanel.SavedGuidID = null;
            MainTemplatePanel.SavedID = null;
            MainTemplatePanel.ScaleMode = ImageScaleMode.KeepAspectRatio;
            MainTemplatePanel.SelectedBackColor = Color.White;
            MainTemplatePanel.SelectedBorderColor = Color.Green;
            MainTemplatePanel.SelectedForeColor = Color.Black;
            MainTemplatePanel.SelectedValue = null;
            MainTemplatePanel.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            MainTemplatePanel.ShadowOffset = 0;
            MainTemplatePanel.ShadowOpacity = 0.25F;
            MainTemplatePanel.ShowAllBorders = false;
            MainTemplatePanel.ShowBottomBorder = false;
            MainTemplatePanel.ShowFocusIndicator = false;
            MainTemplatePanel.ShowLeftBorder = false;
            MainTemplatePanel.ShowRightBorder = false;
            MainTemplatePanel.ShowShadow = false;
            MainTemplatePanel.ShowTitle = false;
            MainTemplatePanel.ShowTitleLine = false;
            MainTemplatePanel.ShowTitleLineinFullWidth = false;
            MainTemplatePanel.ShowTopBorder = false;
            MainTemplatePanel.Size = new Size(770, 432);
            MainTemplatePanel.SlideFrom = SlideDirection.Left;
            MainTemplatePanel.StaticNotMoving = false;
            MainTemplatePanel.TabIndex = 0;
            MainTemplatePanel.TabStop = false;
            MainTemplatePanel.Tag = this;
            MainTemplatePanel.TempBackColor = Color.Empty;
            MainTemplatePanel.Text = "beepPanel1";
            MainTemplatePanel.TextFont = new Font("Arial", 16F);
            MainTemplatePanel.Theme = "DefaultTheme";
            MainTemplatePanel.TitleAlignment = ContentAlignment.TopLeft;
            MainTemplatePanel.TitleBottomY = 0;
            MainTemplatePanel.TitleLineColor = Color.Gray;
            MainTemplatePanel.TitleLineThickness = 2;
            MainTemplatePanel.TitleText = "";
            MainTemplatePanel.ToolTipText = "";
            MainTemplatePanel.TopoffsetForDrawingRect = 0;
            MainTemplatePanel.TrailingIconPath = "";
            MainTemplatePanel.TrailingImagePath = "";
            MainTemplatePanel.UseExternalBufferedGraphics = false;
            MainTemplatePanel.UseGlassmorphism = false;
            MainTemplatePanel.UseGradientBackground = false;
            MainTemplatePanel.UseRichToolTip = true;
            MainTemplatePanel.UseThemeFont = true;
            // 
            // TemplateUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MainTemplatePanel);
            Name = "TemplateUserControl";
            Size = new Size(770, 432);
            ResumeLayout(false);
        }

        #endregion

        public Controls.BeepPanel MainTemplatePanel;
    }
}
