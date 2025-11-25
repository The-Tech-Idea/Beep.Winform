using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepPopupListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepPopupListForm));
            System.Drawing.Drawing2D.GraphicsPath graphicsPath1 = new System.Drawing.Drawing2D.GraphicsPath();
            _beepListBox = new BeepListBox();
            SuspendLayout();
            // 
            // _beepListBox
            // 
            _beepListBox.AnimationDuration = 500;
            _beepListBox.AnimationType = DisplayAnimationType.None;
            _beepListBox.ApplyThemeOnImage = false;
            _beepListBox.ApplyThemeToChilds = true;
            _beepListBox.AutoDrawHitListComponents = true;
            _beepListBox.BackColor = Color.White;
            _beepListBox.BadgeBackColor = Color.FromArgb(33, 150, 243);
            _beepListBox.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _beepListBox.BadgeForeColor = Color.White;
            _beepListBox.BadgeShape = BadgeShape.Circle;
            _beepListBox.BadgeText = "";
            _beepListBox.BlockID = null;
            _beepListBox.BorderColor = Color.LightGray;
            _beepListBox.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _beepListBox.BorderRadius = 8;
            _beepListBox.BorderStyle = BorderStyle.FixedSingle;
            _beepListBox.BorderThickness = 1;
            _beepListBox.BottomoffsetForDrawingRect = 0;
            _beepListBox.BoundProperty = "SelectedMenuItem";
            _beepListBox.CanBeFocused = true;
            _beepListBox.CanBeHovered = false;
            _beepListBox.CanBePressed = true;
            _beepListBox.Category = Utilities.DbFieldCategory.String;
           
            _beepListBox.ComponentName = "beepListBox1";
            _beepListBox.DataSourceProperty = null;
            _beepListBox.DisabledBackColor = Color.FromArgb(200, 200, 200);
            _beepListBox.DisabledBorderColor = Color.LightGray;
            _beepListBox.DisabledForeColor = Color.Gray;
            // REMOVED: DisableDpiAndScaling - .NET 8/9+ handles DPI automatically
            _beepListBox.Dock = DockStyle.Fill;
            _beepListBox.DrawingRect = new Rectangle(24, 8, 587, 753);
            _beepListBox.Easing = EasingType.Linear;
            _beepListBox.EnableHighQualityRendering = true;
            _beepListBox.EnableRippleEffect = false;
            _beepListBox.EnableSplashEffect = false;
            _beepListBox.ErrorColor = Color.FromArgb(176, 0, 32);
            _beepListBox.ErrorText = "";
            
            _beepListBox.FieldID = null;
            _beepListBox.FilledBackgroundColor = Color.FromArgb(245, 245, 245);
            _beepListBox.FloatingLabel = true;
            _beepListBox.FocusBackColor = Color.DodgerBlue;
            _beepListBox.FocusBorderColor = Color.Gray;
            _beepListBox.FocusForeColor = Color.White;
            _beepListBox.FocusIndicatorColor = Color.Blue;
            _beepListBox.Font = new Font("Arial", 12F);
            _beepListBox.ForeColor = Color.Black;
            _beepListBox.Form = null;
            _beepListBox.GlassmorphismBlur = 10F;
            _beepListBox.GlassmorphismOpacity = 0.1F;
            _beepListBox.GradientAngle = 0F;
            _beepListBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _beepListBox.GradientEndColor = Color.FromArgb(230, 230, 230);
            _beepListBox.GradientStartColor = Color.FromArgb(255, 255, 255);
            _beepListBox.GridMode = false;
            _beepListBox.GuidID = "da073259-cfc0-42b8-bdb4-de295de2007b";
            _beepListBox.HasError = false;
            _beepListBox.HelperText = "";
            _beepListBox.HitAreaEventOn = false;
            _beepListBox.HitTestControl = null;
            _beepListBox.HoverBackColor = Color.LightSteelBlue;
            _beepListBox.HoverBorderColor = Color.SteelBlue;
            _beepListBox.HoveredBackcolor = Color.LightSteelBlue;
            _beepListBox.HoverForeColor = Color.DarkBlue;
            _beepListBox.IconSize = 20;
            _beepListBox.Id = -1;
            _beepListBox.ImageSize = 18;
            _beepListBox.InactiveBorderColor = Color.Gray;
            graphicsPath1.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
            _beepListBox.InnerShape = graphicsPath1;
            _beepListBox.IsAcceptButton = false;
            _beepListBox.IsBorderAffectedByTheme = false;
            _beepListBox.IsCancelButton = false;
            _beepListBox.IsChild = false;
            _beepListBox.IsCustomeBorder = false;
            _beepListBox.IsDefault = false;
            _beepListBox.IsDeleted = false;
            _beepListBox.IsDirty = false;
            _beepListBox.IsEditable = false;
            _beepListBox.IsFocused = false;
           
            _beepListBox.IsFrameless = false;
            _beepListBox.IsHovered = false;

     
            _beepListBox.IsNew = false;
            _beepListBox.IsPressed = false;
         
            _beepListBox.IsReadOnly = false;
            _beepListBox.IsRequired = false;
            _beepListBox.IsRounded = true;
            _beepListBox.IsRoundedAffectedByTheme = true;
            _beepListBox.IsSelected = false;
        
            _beepListBox.IsSelectedOptionOn = false;
            _beepListBox.IsShadowAffectedByTheme = true;
            _beepListBox.IsValid = true;
            _beepListBox.IsVisible = true;
            _beepListBox.Items = (List<object>)resources.GetObject("_beepListBox.Items");
            _beepListBox.LabelText = "";
            _beepListBox.LeadingIconPath = "";
            _beepListBox.LeadingImagePath = "";
            _beepListBox.LeftoffsetForDrawingRect = 0;
            _beepListBox.LinkedProperty = null;
            _beepListBox.Location = new Point(1, 1);
            _beepListBox.MaterialBorderVariant = MaterialTextFieldVariant.Outlined;
            
            _beepListBox.MaxHitListDrawPerFrame = 0;
            _beepListBox.MenuItemHeight = 20;
            _beepListBox.MinimumSize = new Size(136, 56);
            _beepListBox.ModernGradientType = ModernGradientType.None;
            _beepListBox.Name = "_beepListBox";
            _beepListBox.OverrideFontSize = TypeStyleFontSize.None;
            _beepListBox.Padding = new Padding(10);
            _beepListBox.ParentBackColor = Color.Empty;
            _beepListBox.ParentControl = null;
            _beepListBox.PressedBackColor = Color.FromArgb(21, 101, 192);
            _beepListBox.PressedBorderColor = Color.FromArgb(21, 101, 192);
            _beepListBox.PressedForeColor = Color.White;
            _beepListBox.RadialCenter = (PointF)resources.GetObject("_beepListBox.RadialCenter");
            _beepListBox.RightoffsetForDrawingRect = 0;
            _beepListBox.SavedGuidID = null;
            _beepListBox.SavedID = null;
            _beepListBox.ScaleMode = ImageScaleMode.KeepAspectRatio;
          
            _beepListBox.SearchText = "";
            _beepListBox.SelectedBackColor = Color.DodgerBlue;
            _beepListBox.SelectedBorderColor = Color.RoyalBlue;
            _beepListBox.SelectedForeColor = Color.White;
            _beepListBox.SelectedIndex = -1;
            _beepListBox.SelectedItem = null;
            _beepListBox.SelectedValue = null;
            _beepListBox.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            _beepListBox.ShadowOffset = 0;
            _beepListBox.ShadowOpacity = 0.25F;
            _beepListBox.ShowAllBorders = false;
            _beepListBox.ShowBottomBorder = true;
            _beepListBox.ShowCheckBox = false;
            _beepListBox.ShowFocusIndicator = false;
            _beepListBox.ShowHilightBox = true;
            _beepListBox.ShowImage = false;
            _beepListBox.ShowLeftBorder = true;
            _beepListBox.ShowRightBorder = true;
            _beepListBox.ShowSearch = false;
            _beepListBox.ShowShadow = false;
         
            _beepListBox.ShowTopBorder = true;
            _beepListBox.Size = new Size(635, 769);
            _beepListBox.SlideFrom = SlideDirection.Left;
            _beepListBox.StaticNotMoving = false;
            _beepListBox.TabIndex = 0;
            _beepListBox.Tag = this;
           
            _beepListBox.TextFont = new Font("Arial", 12F);
            _beepListBox.Theme = "DefaultType";
           
            _beepListBox.ToolTipText = "";
            _beepListBox.TopoffsetForDrawingRect = 0;
            _beepListBox.TrailingIconPath = "";
            _beepListBox.TrailingImagePath = "";
            _beepListBox.UseExternalBufferedGraphics = false;
            _beepListBox.UseGlassmorphism = false;
            _beepListBox.UseGradientBackground = false;
            _beepListBox.UseRichToolTip = true;
            _beepListBox.UseThemeFont = true;
            // 
            // BeepPopupListForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
        
            ClientSize = new Size(637, 771);
            Controls.Add(_beepListBox);
            Name = "BeepPopupListForm";
            Text = "BeepPopupListForm";
            Theme = "DefaultType";
            ResumeLayout(false);
        }

        #endregion

        private BeepListBox _beepListBox;
    }
}