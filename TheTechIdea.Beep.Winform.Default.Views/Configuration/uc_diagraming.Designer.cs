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
            beepAccordionMenu1 = new BeepAccordionMenu();
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
            beepStepperBar1.ExternalDrawingLayer = BeepControl.DrawingLayer.AfterAll;
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
            beepStepperBar1.Location = new Point(734, 330);
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
            beepStepperBar1.Tag = this;
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
            beepWizard1.BackColor = Color.FromArgb(240, 240, 240);
            beepWizard1.BadgeBackColor = Color.Red;
            beepWizard1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepWizard1.BadgeForeColor = Color.White;
            beepWizard1.BadgeShape = BadgeShape.Circle;
            beepWizard1.BadgeText = "";
            beepWizard1.BlockID = null;
            beepWizard1.BorderColor = Color.FromArgb(211, 211, 211);
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
            beepWizard1.DrawingRect = new Rectangle(0, 0, 463, 297);
            beepWizard1.Easing = EasingType.Linear;
            beepWizard1.ExternalDrawingLayer = BeepControl.DrawingLayer.AfterAll;
            beepWizard1.FieldID = null;
            beepWizard1.FocusBackColor = Color.White;
            beepWizard1.FocusBorderColor = Color.Gray;
            beepWizard1.FocusForeColor = Color.Black;
            beepWizard1.FocusIndicatorColor = Color.Blue;
            beepWizard1.Form = null;
            beepWizard1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepWizard1.GradientEndColor = Color.FromArgb(245, 245, 220);
            beepWizard1.GradientStartColor = Color.FromArgb(240, 240, 240);
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
            beepWizard1.Location = new Point(552, 16);
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
            beepWizard1.ShadowColor = Color.FromArgb(255, 192, 203);
            beepWizard1.ShadowOffset = 0;
            beepWizard1.ShadowOpacity = 0.5F;
            beepWizard1.ShowAllBorders = false;
            beepWizard1.ShowBottomBorder = false;
            beepWizard1.ShowFocusIndicator = false;
            beepWizard1.ShowLeftBorder = false;
            beepWizard1.ShowRightBorder = false;
            beepWizard1.ShowShadow = false;
            beepWizard1.ShowTopBorder = false;
            beepWizard1.Size = new Size(463, 297);
            beepWizard1.SlideFrom = SlideDirection.Left;
            beepWizard1.StaticNotMoving = false;
            beepWizard1.TabIndex = 11;
            beepWizard1.Tag = this;
            beepWizard1.TempBackColor = Color.Empty;
            beepWizard1.Text = "beepWizard1";
            beepWizard1.Theme = EnumBeepThemes.ZenTheme;
            beepWizard1.Title = null;
            beepWizard1.ToolTipText = "";
            beepWizard1.TopoffsetForDrawingRect = 0;
            beepWizard1.UseGradientBackground = false;
            beepWizard1.UseThemeFont = true;
            beepWizard1.ViewType = WizardViewType.TopConnectedCircles;
            // 
            // beepAccordionMenu1
            // 
            beepAccordionMenu1.AnimationDuration = 500;
            beepAccordionMenu1.AnimationType = DisplayAnimationType.None;
            beepAccordionMenu1.ApplyThemeToChilds = false;
            beepAccordionMenu1.AutoScroll = true;
            beepAccordionMenu1.AutoScrollMinSize = new Size(0, 8);
            beepAccordionMenu1.BackColor = Color.FromArgb(0, 120, 215);
            beepAccordionMenu1.BadgeBackColor = Color.Red;
            beepAccordionMenu1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepAccordionMenu1.BadgeForeColor = Color.White;
            beepAccordionMenu1.BadgeShape = BadgeShape.Circle;
            beepAccordionMenu1.BadgeText = "";
            beepAccordionMenu1.BlockID = null;
            beepAccordionMenu1.BorderColor = Color.FromArgb(0, 100, 195);
            beepAccordionMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepAccordionMenu1.BorderRadius = 8;
            beepAccordionMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepAccordionMenu1.BorderThickness = 1;
            beepAccordionMenu1.BottomoffsetForDrawingRect = 0;
            beepAccordionMenu1.BoundProperty = null;
            beepAccordionMenu1.CanBeFocused = true;
            beepAccordionMenu1.CanBeHovered = false;
            beepAccordionMenu1.CanBePressed = true;
            beepAccordionMenu1.Category = Utilities.DbFieldCategory.String;
            beepAccordionMenu1.CollapsedButtonColor = Color.FromArgb(255, 255, 255);
            beepAccordionMenu1.ComponentName = "beepAccordionMenu1";
            beepAccordionMenu1.DataSourceProperty = null;
            beepAccordionMenu1.DisabledBackColor = Color.White;
            beepAccordionMenu1.DisabledForeColor = Color.Black;
            beepAccordionMenu1.DrawingRect = new Rectangle(1, 1, 344, 346);
            beepAccordionMenu1.Easing = EasingType.Linear;
            beepAccordionMenu1.ExpandButtonColor = Color.FromArgb(255, 255, 255);
            beepAccordionMenu1.ExternalDrawingLayer = BeepControl.DrawingLayer.AfterAll;
            beepAccordionMenu1.FieldID = null;
            beepAccordionMenu1.FocusBackColor = Color.White;
            beepAccordionMenu1.FocusBorderColor = Color.Gray;
            beepAccordionMenu1.FocusForeColor = Color.Black;
            beepAccordionMenu1.FocusIndicatorColor = Color.Blue;
            beepAccordionMenu1.ForeColor = Color.FromArgb(255, 255, 255);
            beepAccordionMenu1.Form = null;
            beepAccordionMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepAccordionMenu1.GradientEndColor = Color.FromArgb(255, 255, 255);
            beepAccordionMenu1.GradientStartColor = Color.FromArgb(245, 245, 245);
            beepAccordionMenu1.GuidID = "1df9a50a-e6fe-4586-9125-7c4d80d22679";
            beepAccordionMenu1.HeaderFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            beepAccordionMenu1.HitAreaEventOn = false;
            beepAccordionMenu1.HitTestControl = null;
            beepAccordionMenu1.HoverBackColor = Color.FromArgb(30, 140, 235);
            beepAccordionMenu1.HoverBorderColor = Color.DodgerBlue;
            beepAccordionMenu1.HoveredBackcolor = Color.Wheat;
            beepAccordionMenu1.HoverForeColor = Color.FromArgb(255, 255, 255);
            beepAccordionMenu1.Id = -1;
            beepAccordionMenu1.InactiveBorderColor = Color.Gray;
            beepAccordionMenu1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepAccordionMenu1.Info");
            beepAccordionMenu1.IsAcceptButton = false;
            beepAccordionMenu1.IsBorderAffectedByTheme = true;
            beepAccordionMenu1.IsCancelButton = false;
            beepAccordionMenu1.IsChild = false;
            beepAccordionMenu1.IsCustomeBorder = false;
            beepAccordionMenu1.IsDefault = false;
            beepAccordionMenu1.IsDeleted = false;
            beepAccordionMenu1.IsDirty = false;
            beepAccordionMenu1.IsEditable = false;
            beepAccordionMenu1.IsFocused = false;
            beepAccordionMenu1.IsFrameless = false;
            beepAccordionMenu1.IsHovered = false;
            beepAccordionMenu1.IsNew = false;
            beepAccordionMenu1.IsPressed = false;
            beepAccordionMenu1.IsReadOnly = false;
            beepAccordionMenu1.IsRequired = false;
            beepAccordionMenu1.IsRounded = true;
            beepAccordionMenu1.IsRoundedAffectedByTheme = true;
            beepAccordionMenu1.IsSelected = false;
            beepAccordionMenu1.IsSelectedOptionOn = false;
            beepAccordionMenu1.IsShadowAffectedByTheme = true;
            beepAccordionMenu1.IsVisible = false;
            beepAccordionMenu1.Items = (List<object>)resources.GetObject("beepAccordionMenu1.Items");
            beepAccordionMenu1.LeftoffsetForDrawingRect = 0;
            beepAccordionMenu1.LinkedProperty = null;
            beepAccordionMenu1.ListItems.Add((Controls.Models.SimpleItem)resources.GetObject("beepAccordionMenu1.ListItems"));
            beepAccordionMenu1.ListItems.Add((Controls.Models.SimpleItem)resources.GetObject("beepAccordionMenu1.ListItems1"));
            beepAccordionMenu1.ListItems.Add((Controls.Models.SimpleItem)resources.GetObject("beepAccordionMenu1.ListItems2"));
            beepAccordionMenu1.Location = new Point(18, 106);
            beepAccordionMenu1.Name = "beepAccordionMenu1";
            beepAccordionMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepAccordionMenu1.ParentBackColor = Color.Empty;
            beepAccordionMenu1.ParentControl = null;
            beepAccordionMenu1.PressedBackColor = Color.White;
            beepAccordionMenu1.PressedBorderColor = Color.Gray;
            beepAccordionMenu1.PressedForeColor = Color.Gray;
            beepAccordionMenu1.RightoffsetForDrawingRect = 0;
            beepAccordionMenu1.SavedGuidID = null;
            beepAccordionMenu1.SavedID = null;
            beepAccordionMenu1.SelectedBackColor = Color.FromArgb(108, 117, 125);
            beepAccordionMenu1.SelectedForeColor = Color.FromArgb(255, 255, 255);
            beepAccordionMenu1.SelectedIndex = -1;
            beepAccordionMenu1.SelectedItem = null;
            beepAccordionMenu1.SelectedValue = null;
            beepAccordionMenu1.ShadowColor = Color.FromArgb(173, 181, 189);
            beepAccordionMenu1.ShadowOffset = 0;
            beepAccordionMenu1.ShadowOpacity = 0.5F;
            beepAccordionMenu1.ShowAllBorders = true;
            beepAccordionMenu1.ShowBottomBorder = true;
            beepAccordionMenu1.ShowFocusIndicator = false;
            beepAccordionMenu1.ShowLeftBorder = true;
            beepAccordionMenu1.ShowRightBorder = true;
            beepAccordionMenu1.ShowShadow = false;
            beepAccordionMenu1.ShowTopBorder = true;
            beepAccordionMenu1.Size = new Size(346, 348);
            beepAccordionMenu1.SlideFrom = SlideDirection.Left;
            beepAccordionMenu1.StaticNotMoving = false;
            beepAccordionMenu1.TabIndex = 12;
            beepAccordionMenu1.Tag = this;
            beepAccordionMenu1.TempBackColor = Color.Empty;
            beepAccordionMenu1.Text = "beepAccordionMenu1";
            beepAccordionMenu1.Theme = EnumBeepThemes.DefaultTheme;
            beepAccordionMenu1.ToolTipText = "";
            beepAccordionMenu1.TopoffsetForDrawingRect = 0;
            beepAccordionMenu1.UseGradientBackground = false;
            beepAccordionMenu1.UseThemeFont = true;
            // 
            // uc_diagraming
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(beepAccordionMenu1);
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
        private BeepAccordionMenu beepAccordionMenu1;
    }
}
