using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    partial class uc_diagraming
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_diagraming));
            beepStepperBar1 = new BeepStepperBar();
            beepWizard1 = new BeepWizard();
            SuspendLayout();
            // 
            // beepStepperBar1
            // 
            beepStepperBar1.AnimationDuration = 500;
            beepStepperBar1.AnimationType = DisplayAnimationType.None;
            beepStepperBar1.ApplyThemeToChilds = false;
            beepStepperBar1.BackColor = Color.FromArgb(250, 250, 250);
            beepStepperBar1.BadgeBackColor = Color.Red;
            beepStepperBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepStepperBar1.BadgeForeColor = Color.White;
            beepStepperBar1.BadgeShape = BadgeShape.Circle;
            beepStepperBar1.BadgeText = "";
            beepStepperBar1.BlockID = null;
            beepStepperBar1.BorderColor = Color.Black;
            beepStepperBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepStepperBar1.BorderRadius = 8;
            beepStepperBar1.BorderStyle = BorderStyle.FixedSingle;
            beepStepperBar1.BorderThickness = 1;
            beepStepperBar1.BottomoffsetForDrawingRect = 0;
            beepStepperBar1.BoundProperty = null;
            beepStepperBar1.ButtonSize = new Size(30, 30);
            beepStepperBar1.CanBeFocused = true;
            beepStepperBar1.CanBeHovered = false;
            beepStepperBar1.CanBePressed = true;
            beepStepperBar1.Category = Utilities.DbFieldCategory.String;
            beepStepperBar1.CheckImage = "check.svg";
            beepStepperBar1.ComponentName = "beepStepperBar1";
            beepStepperBar1.DataSourceProperty = null;
            beepStepperBar1.DisabledBackColor = Color.FromArgb(200, 200, 200);
            beepStepperBar1.DisabledForeColor = Color.FromArgb(150, 150, 150);
            beepStepperBar1.DisplayMode = StepDisplayMode.StepNumber;
            beepStepperBar1.DrawingRect = new Rectangle(0, 0, 176, 290);
            beepStepperBar1.Easing = EasingType.Linear;
            beepStepperBar1.FieldID = null;
            beepStepperBar1.FocusBackColor = Color.FromArgb(119, 136, 153);
            beepStepperBar1.FocusBorderColor = Color.Gray;
            beepStepperBar1.FocusForeColor = Color.FromArgb(255, 255, 255);
            beepStepperBar1.FocusIndicatorColor = Color.Blue;
            beepStepperBar1.ForeColor = Color.FromArgb(255, 255, 255);
            beepStepperBar1.Form = null;
            beepStepperBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepStepperBar1.GradientEndColor = Color.Gray;
            beepStepperBar1.GradientStartColor = Color.Gray;
            beepStepperBar1.GuidID = "985a119b-7e2a-47a6-bb83-7a097ccd8ca7";
            beepStepperBar1.HitAreaEventOn = false;
            beepStepperBar1.HitTestControl = null;
            beepStepperBar1.HoverBackColor = Color.FromArgb(139, 156, 173);
            beepStepperBar1.HoverBorderColor = Color.Gray;
            beepStepperBar1.HoveredBackcolor = Color.Wheat;
            beepStepperBar1.HoverForeColor = Color.FromArgb(255, 255, 255);
            beepStepperBar1.Id = -1;
            beepStepperBar1.InactiveBorderColor = Color.Gray;
            beepStepperBar1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepStepperBar1.Info");
            beepStepperBar1.IsAcceptButton = false;
            beepStepperBar1.IsBorderAffectedByTheme = true;
            beepStepperBar1.IsCancelButton = false;
            beepStepperBar1.IsChild = false;
            beepStepperBar1.IsCustomeBorder = false;
            beepStepperBar1.IsDefault = false;
            beepStepperBar1.IsDeleted = false;
            beepStepperBar1.IsDirty = false;
            beepStepperBar1.IsEditable = false;
            beepStepperBar1.IsFocused = false;
            beepStepperBar1.IsFrameless = false;
            beepStepperBar1.IsHovered = false;
            beepStepperBar1.IsNew = false;
            beepStepperBar1.IsPressed = false;
            beepStepperBar1.IsReadOnly = false;
            beepStepperBar1.IsRequired = false;
            beepStepperBar1.IsRounded = true;
            beepStepperBar1.IsRoundedAffectedByTheme = true;
            beepStepperBar1.IsSelected = false;
            beepStepperBar1.IsSelectedOptionOn = true;
            beepStepperBar1.IsShadowAffectedByTheme = true;
            beepStepperBar1.IsVisible = false;
            beepStepperBar1.Items = (List<object>)resources.GetObject("beepStepperBar1.Items");
            beepStepperBar1.LeftoffsetForDrawingRect = 0;
            beepStepperBar1.LinkedProperty = null;
            beepStepperBar1.ListItems = (System.ComponentModel.BindingList<Controls.Models.SimpleItem>)resources.GetObject("beepStepperBar1.ListItems");
            beepStepperBar1.Location = new Point(66, 274);
            beepStepperBar1.Name = "beepStepperBar1";
            beepStepperBar1.Orientation = Orientation.Vertical;
            beepStepperBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepStepperBar1.ParentBackColor = Color.Empty;
            beepStepperBar1.ParentControl = null;
            beepStepperBar1.PressedBackColor = Color.FromArgb(99, 116, 153);
            beepStepperBar1.PressedBorderColor = Color.FromArgb(255, 255, 255);
            beepStepperBar1.PressedForeColor = Color.FromArgb(255, 255, 255);
            beepStepperBar1.RightoffsetForDrawingRect = 0;
            beepStepperBar1.SavedGuidID = null;
            beepStepperBar1.SavedID = null;
            beepStepperBar1.SelectedBackColor = Color.White;
            beepStepperBar1.SelectedForeColor = Color.Black;
            beepStepperBar1.SelectedValue = null;
            beepStepperBar1.ShadowColor = Color.Black;
            beepStepperBar1.ShadowOffset = 0;
            beepStepperBar1.ShadowOpacity = 0.5F;
            beepStepperBar1.ShowAllBorders = false;
            beepStepperBar1.ShowBottomBorder = false;
            beepStepperBar1.ShowFocusIndicator = false;
            beepStepperBar1.ShowLeftBorder = false;
            beepStepperBar1.ShowRightBorder = false;
            beepStepperBar1.ShowShadow = false;
            beepStepperBar1.ShowTopBorder = false;
            beepStepperBar1.Size = new Size(176, 290);
            beepStepperBar1.SlideFrom = SlideDirection.Left;
            beepStepperBar1.StaticNotMoving = false;
            beepStepperBar1.TabIndex = 9;
            beepStepperBar1.TempBackColor = Color.Empty;
            beepStepperBar1.Text = "beepStepperBar1";
            beepStepperBar1.Theme = EnumBeepThemes.ZenTheme;
            beepStepperBar1.ToolTipText = "";
            beepStepperBar1.TopoffsetForDrawingRect = 0;
            beepStepperBar1.UseGradientBackground = false;
            beepStepperBar1.UseThemeFont = true;
            // 
            // beepWizard1
            // 
            beepWizard1.AnimationDuration = 500;
            beepWizard1.AnimationType = DisplayAnimationType.None;
            beepWizard1.ApplyThemeToChilds = true;
            beepWizard1.BackColor = Color.FromArgb(33, 33, 33);
            beepWizard1.BadgeBackColor = Color.Red;
            beepWizard1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepWizard1.BadgeForeColor = Color.White;
            beepWizard1.BadgeShape = BadgeShape.Circle;
            beepWizard1.BadgeText = "";
            beepWizard1.BlockID = null;
            beepWizard1.BorderColor = Color.FromArgb(97, 97, 97);
            beepWizard1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepWizard1.BorderRadius = 8;
            beepWizard1.BorderStyle = BorderStyle.FixedSingle;
            beepWizard1.BorderThickness = 1;
            beepWizard1.BottomoffsetForDrawingRect = 0;
            beepWizard1.BoundProperty = null;
            beepWizard1.CanBeFocused = true;
            beepWizard1.CanBeHovered = false;
            beepWizard1.CanBePressed = true;
            beepWizard1.Category = Utilities.DbFieldCategory.String;
            beepWizard1.ComponentName = "beepWizard1";
            beepWizard1.DataSourceProperty = null;
            beepWizard1.Description = null;
            beepWizard1.DisabledBackColor = Color.White;
            beepWizard1.DisabledForeColor = Color.Black;
            beepWizard1.DrawingRect = new Rectangle(0, 0, 572, 551);
            beepWizard1.Easing = EasingType.Linear;
            beepWizard1.FieldID = null;
            beepWizard1.FocusBackColor = Color.White;
            beepWizard1.FocusBorderColor = Color.Gray;
            beepWizard1.FocusForeColor = Color.Black;
            beepWizard1.FocusIndicatorColor = Color.Blue;
            beepWizard1.Form = null;
            beepWizard1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepWizard1.GradientEndColor = Color.FromArgb(50, 50, 50);
            beepWizard1.GradientStartColor = Color.FromArgb(33, 33, 33);
            beepWizard1.GuidID = "aa5601e8-7353-431d-ba3f-628bf4a565a5";
            beepWizard1.HitAreaEventOn = false;
            beepWizard1.HitTestControl = null;
            beepWizard1.HoverBackColor = Color.White;
            beepWizard1.HoverBorderColor = Color.Gray;
            beepWizard1.HoveredBackcolor = Color.Wheat;
            beepWizard1.HoverForeColor = Color.Black;
            beepWizard1.Id = -1;
            beepWizard1.InactiveBorderColor = Color.Gray;
            beepWizard1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepWizard1.Info");
            beepWizard1.IsAcceptButton = false;
            beepWizard1.IsBorderAffectedByTheme = true;
            beepWizard1.IsCancelButton = false;
            beepWizard1.IsChild = false;
            beepWizard1.IsCustomeBorder = false;
            beepWizard1.IsDefault = false;
            beepWizard1.IsDeleted = false;
            beepWizard1.IsDirty = false;
            beepWizard1.IsEditable = false;
            beepWizard1.IsFocused = false;
            beepWizard1.IsFrameless = false;
            beepWizard1.IsHovered = false;
            beepWizard1.IsNew = false;
            beepWizard1.IsPressed = false;
            beepWizard1.IsReadOnly = false;
            beepWizard1.IsRequired = false;
            beepWizard1.IsRounded = true;
            beepWizard1.IsRoundedAffectedByTheme = true;
            beepWizard1.IsSelected = false;
            beepWizard1.IsSelectedOptionOn = true;
            beepWizard1.IsShadowAffectedByTheme = true;
            beepWizard1.IsVisible = false;
            beepWizard1.Items = (List<object>)resources.GetObject("beepWizard1.Items");
            beepWizard1.LeftoffsetForDrawingRect = 0;
            beepWizard1.LinkedProperty = null;
            beepWizard1.Location = new Point(254, 3);
            beepWizard1.LogoPath = "logo.svg";
            beepWizard1.Name = "beepWizard1";
            beepWizard1.OverrideFontSize = TypeStyleFontSize.None;
            beepWizard1.ParentBackColor = Color.Empty;
            beepWizard1.ParentControl = null;
            beepWizard1.PressedBackColor = Color.White;
            beepWizard1.PressedBorderColor = Color.Gray;
            beepWizard1.PressedForeColor = Color.Gray;
            beepWizard1.RightoffsetForDrawingRect = 0;
            beepWizard1.SavedGuidID = null;
            beepWizard1.SavedID = null;
            beepWizard1.SelectedBackColor = Color.White;
            beepWizard1.SelectedForeColor = Color.Black;
            beepWizard1.SelectedValue = null;
            beepWizard1.ShadowColor = Color.FromArgb(0, 0, 0);
            beepWizard1.ShadowOffset = 0;
            beepWizard1.ShadowOpacity = 0.5F;
            beepWizard1.ShowAllBorders = false;
            beepWizard1.ShowBottomBorder = false;
            beepWizard1.ShowFocusIndicator = false;
            beepWizard1.ShowLeftBorder = false;
            beepWizard1.ShowRightBorder = false;
            beepWizard1.ShowShadow = false;
            beepWizard1.ShowTopBorder = false;
            beepWizard1.Size = new Size(572, 551);
            beepWizard1.SlideFrom = SlideDirection.Left;
            beepWizard1.StaticNotMoving = false;
            beepWizard1.TabIndex = 11;
            beepWizard1.TempBackColor = Color.Empty;
            beepWizard1.Text = "beepWizard1";
            beepWizard1.Theme = EnumBeepThemes.DarkTheme;
            beepWizard1.Title = null;
            beepWizard1.ToolTipText = "";
            beepWizard1.TopoffsetForDrawingRect = 0;
            beepWizard1.UseGradientBackground = false;
            beepWizard1.UseThemeFont = true;
            beepWizard1.ViewType = WizardViewType.TopConnectedCircles;
            // 
            // uc_diagraming
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(beepWizard1);
            Controls.Add(beepStepperBar1);
            Name = "uc_diagraming";
            Size = new Size(948, 583);
            Theme = EnumBeepThemes.ZenTheme;
            ResumeLayout(false);
        }

        #endregion
        private BeepStepperBar beepStepperBar1;
        private BeepWizard beepWizard1;
    }
}
