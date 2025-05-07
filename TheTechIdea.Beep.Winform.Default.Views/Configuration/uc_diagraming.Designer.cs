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
            beepAccordionMenu1 = new BeepAccordionMenu();
            beepLightTextBox1 = new BeepLightTextBox();
            SuspendLayout();
            // 
            // beepAccordionMenu1
            // 
            beepAccordionMenu1.AnimationDelay = 15;
            beepAccordionMenu1.AnimationDuration = 500;
            beepAccordionMenu1.AnimationStep = 20;
            beepAccordionMenu1.AnimationType = DisplayAnimationType.None;
            beepAccordionMenu1.ApplyThemeToChilds = false;
            beepAccordionMenu1.BackColor = Color.FromArgb(255, 105, 180);
            beepAccordionMenu1.BadgeBackColor = Color.Red;
            beepAccordionMenu1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepAccordionMenu1.BadgeForeColor = Color.White;
            beepAccordionMenu1.BadgeShape = BadgeShape.Circle;
            beepAccordionMenu1.BadgeText = "";
            beepAccordionMenu1.BlockID = null;
            beepAccordionMenu1.BorderColor = Color.Black;
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
            beepAccordionMenu1.ChildItemHeight = 30;
            beepAccordionMenu1.CollapsedWidth = 64;
            beepAccordionMenu1.ComponentName = "beepAccordionMenu1";
            beepAccordionMenu1.DataSourceProperty = null;
            beepAccordionMenu1.DisabledBackColor = Color.White;
            beepAccordionMenu1.DisabledForeColor = Color.Black;
            beepAccordionMenu1.DrawingRect = new Rectangle(5, 5, 260, 285);
            beepAccordionMenu1.Easing = EasingType.Linear;
            beepAccordionMenu1.ExpandedWidth = 200;
            beepAccordionMenu1.ExternalDrawingLayer = BeepControl.DrawingLayer.AfterAll;
            beepAccordionMenu1.FieldID = null;
            beepAccordionMenu1.FocusBackColor = Color.White;
            beepAccordionMenu1.FocusBorderColor = Color.Gray;
            beepAccordionMenu1.FocusForeColor = Color.Black;
            beepAccordionMenu1.FocusIndicatorColor = Color.Blue;
            beepAccordionMenu1.Form = null;
            beepAccordionMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepAccordionMenu1.GradientEndColor = Color.Gray;
            beepAccordionMenu1.GradientStartColor = Color.Gray;
            beepAccordionMenu1.GuidID = "df78180c-41ba-49cc-926c-615dcbac3953";
            beepAccordionMenu1.HitAreaEventOn = false;
            beepAccordionMenu1.HitTestControl = null;
            beepAccordionMenu1.HoverBackColor = Color.White;
            beepAccordionMenu1.HoverBorderColor = Color.Gray;
            beepAccordionMenu1.HoveredBackcolor = Color.Wheat;
            beepAccordionMenu1.HoverForeColor = Color.Black;
            beepAccordionMenu1.Id = -1;
            beepAccordionMenu1.InactiveBorderColor = Color.Gray;
            beepAccordionMenu1.IndentationWidth = 20;
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
            beepAccordionMenu1.ItemHeight = 40;
            beepAccordionMenu1.Items = (List<object>)resources.GetObject("beepAccordionMenu1.Items");
            beepAccordionMenu1.LeftoffsetForDrawingRect = 0;
            beepAccordionMenu1.LinkedProperty = null;
            beepAccordionMenu1.ListItems.Add((Controls.Models.SimpleItem)resources.GetObject("beepAccordionMenu1.ListItems"));
            beepAccordionMenu1.ListItems.Add((Controls.Models.SimpleItem)resources.GetObject("beepAccordionMenu1.ListItems1"));
            beepAccordionMenu1.ListItems.Add((Controls.Models.SimpleItem)resources.GetObject("beepAccordionMenu1.ListItems2"));
            beepAccordionMenu1.Location = new Point(278, 80);
            beepAccordionMenu1.Name = "beepAccordionMenu1";
            beepAccordionMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepAccordionMenu1.Padding = new Padding(5);
            beepAccordionMenu1.ParentBackColor = Color.Empty;
            beepAccordionMenu1.ParentControl = null;
            beepAccordionMenu1.PressedBackColor = Color.White;
            beepAccordionMenu1.PressedBorderColor = Color.Gray;
            beepAccordionMenu1.PressedForeColor = Color.Gray;
            beepAccordionMenu1.RightoffsetForDrawingRect = 0;
            beepAccordionMenu1.SavedGuidID = null;
            beepAccordionMenu1.SavedID = null;
            beepAccordionMenu1.SelectedBackColor = Color.White;
            beepAccordionMenu1.SelectedForeColor = Color.Black;
            beepAccordionMenu1.SelectedItem = null;
            beepAccordionMenu1.SelectedValue = null;
            beepAccordionMenu1.ShadowColor = Color.Black;
            beepAccordionMenu1.ShadowOffset = 0;
            beepAccordionMenu1.ShadowOpacity = 0.5F;
            beepAccordionMenu1.ShowAllBorders = false;
            beepAccordionMenu1.ShowBottomBorder = false;
            beepAccordionMenu1.ShowFocusIndicator = false;
            beepAccordionMenu1.ShowLeftBorder = false;
            beepAccordionMenu1.ShowRightBorder = false;
            beepAccordionMenu1.ShowShadow = false;
            beepAccordionMenu1.ShowTopBorder = false;
            beepAccordionMenu1.Size = new Size(270, 295);
            beepAccordionMenu1.SlideFrom = SlideDirection.Left;
            beepAccordionMenu1.StaticNotMoving = false;
            beepAccordionMenu1.TabIndex = 0;
            beepAccordionMenu1.Tag = this;
            beepAccordionMenu1.TempBackColor = Color.Empty;
            beepAccordionMenu1.Text = "beepAccordionMenu1";
            beepAccordionMenu1.Theme = EnumBeepThemes.ZenTheme;
            beepAccordionMenu1.Title = "Accordion";
            beepAccordionMenu1.ToolTipText = "";
            beepAccordionMenu1.TopoffsetForDrawingRect = 0;
            beepAccordionMenu1.UseGradientBackground = false;
            beepAccordionMenu1.UseThemeFont = true;
            // 
            // beepLightTextBox1
            // 
            beepLightTextBox1.ActivationMode = BeepLightTextBox.EditActivation.DoubleClick;
            beepLightTextBox1.AllowSearch = true;
            beepLightTextBox1.AnimationDuration = 500;
            beepLightTextBox1.AnimationType = DisplayAnimationType.None;
            beepLightTextBox1.ApplyThemeToChilds = false;
            beepLightTextBox1.BackColor = Color.FromArgb(250, 250, 250);
            beepLightTextBox1.BadgeBackColor = Color.Red;
            beepLightTextBox1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepLightTextBox1.BadgeForeColor = Color.White;
            beepLightTextBox1.BadgeShape = BadgeShape.Circle;
            beepLightTextBox1.BadgeText = "";
            beepLightTextBox1.BlockID = null;
            beepLightTextBox1.BorderColor = Color.FromArgb(211, 211, 211);
            beepLightTextBox1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepLightTextBox1.BorderRadius = 3;
            beepLightTextBox1.BorderStyle = BorderStyle.FixedSingle;
            beepLightTextBox1.BorderThickness = 1;
            beepLightTextBox1.BottomoffsetForDrawingRect = 0;
            beepLightTextBox1.BoundProperty = "Text";
            beepLightTextBox1.CanBeFocused = true;
            beepLightTextBox1.CanBeHovered = true;
            beepLightTextBox1.CanBePressed = true;
            beepLightTextBox1.Category = Utilities.DbFieldCategory.String;
            beepLightTextBox1.ComponentName = "beepLightTextBox1";
            beepLightTextBox1.CustomMask = "";
            beepLightTextBox1.DataSourceProperty = null;
            beepLightTextBox1.DateFormat = "MM/dd/yyyy";
            beepLightTextBox1.DateTimeFormat = "MM/dd/yyyy HH:mm:ss";
            beepLightTextBox1.DateValidationMessage = "Please enter a valid date.";
            beepLightTextBox1.DecimalValidationMessage = "Please enter a valid number.";
            beepLightTextBox1.DisabledBackColor = Color.FromArgb(200, 200, 200);
            beepLightTextBox1.DisabledForeColor = Color.FromArgb(150, 150, 150);
            beepLightTextBox1.DrawingRect = new Rectangle(5, 5, 174, 31);
            beepLightTextBox1.DropdownPosition = BeepPopupFormPosition.Bottom;
            beepLightTextBox1.Easing = EasingType.Linear;
            beepLightTextBox1.EmailValidationMessage = "Please enter a valid email address.";
            beepLightTextBox1.ExternalDrawingLayer = BeepControl.DrawingLayer.AfterAll;
            beepLightTextBox1.FieldID = null;
            beepLightTextBox1.FocusBackColor = Color.White;
            beepLightTextBox1.FocusBorderColor = Color.Gray;
            beepLightTextBox1.FocusForeColor = Color.Black;
            beepLightTextBox1.FocusIndicatorColor = Color.Blue;
            beepLightTextBox1.Font = new Font("Segoe UI", 11F);
            beepLightTextBox1.ForeColor = Color.FromArgb(51, 51, 51);
            beepLightTextBox1.Form = null;
            beepLightTextBox1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepLightTextBox1.GradientEndColor = Color.FromArgb(245, 245, 220);
            beepLightTextBox1.GradientStartColor = Color.FromArgb(240, 240, 240);
            beepLightTextBox1.GuidID = "0d446277-9fde-4d65-9685-0b09900e0934";
            beepLightTextBox1.HitAreaEventOn = false;
            beepLightTextBox1.HitTestControl = null;
            beepLightTextBox1.HoverBackColor = Color.FromArgb(245, 245, 245);
            beepLightTextBox1.HoverBorderColor = Color.FromArgb(144, 169, 144);
            beepLightTextBox1.HoveredBackcolor = Color.Wheat;
            beepLightTextBox1.HoverForeColor = Color.FromArgb(51, 51, 51);
            beepLightTextBox1.Id = -1;
            beepLightTextBox1.ImagePath = "dropdown_arrow.svg";
            beepLightTextBox1.InactiveBorderColor = Color.FromArgb(119, 136, 153);
            beepLightTextBox1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepLightTextBox1.Info");
            beepLightTextBox1.IsAcceptButton = false;
            beepLightTextBox1.IsBorderAffectedByTheme = false;
            beepLightTextBox1.IsCancelButton = false;
            beepLightTextBox1.IsChild = false;
            beepLightTextBox1.IsCustomeBorder = false;
            beepLightTextBox1.IsDefault = false;
            beepLightTextBox1.IsDeleted = false;
            beepLightTextBox1.IsDirty = false;
            beepLightTextBox1.IsEditable = false;
            beepLightTextBox1.IsFocused = false;
            beepLightTextBox1.IsFrameless = false;
            beepLightTextBox1.IsHovered = false;
            beepLightTextBox1.IsNew = false;
            beepLightTextBox1.IsPressed = false;
            beepLightTextBox1.IsReadOnly = false;
            beepLightTextBox1.IsRequired = false;
            beepLightTextBox1.IsRounded = true;
            beepLightTextBox1.IsRoundedAffectedByTheme = true;
            beepLightTextBox1.IsSelected = false;
            beepLightTextBox1.IsSelectedOptionOn = false;
            beepLightTextBox1.IsShadowAffectedByTheme = false;
            beepLightTextBox1.IsVisible = false;
            beepLightTextBox1.LeftoffsetForDrawingRect = 0;
            beepLightTextBox1.Lines = 3;
            beepLightTextBox1.LinkedProperty = null;
            beepLightTextBox1.Location = new Point(42, 210);
            beepLightTextBox1.MaskFormat = TextBoxMaskFormat.None;
            beepLightTextBox1.MaxDropdownHeight = 200;
            beepLightTextBox1.MaxDropdownWidth = 0;
            beepLightTextBox1.MaxImageSize = new Size(16, 16);
            beepLightTextBox1.Multiline = false;
            beepLightTextBox1.Name = "beepLightTextBox1";
            beepLightTextBox1.OnlyCharacters = false;
            beepLightTextBox1.OnlyDigits = false;
            beepLightTextBox1.OverrideFontSize = TypeStyleFontSize.None;
            beepLightTextBox1.Padding = new Padding(4);
            beepLightTextBox1.ParentBackColor = Color.Empty;
            beepLightTextBox1.ParentControl = null;
            beepLightTextBox1.PasswordMode = false;
            beepLightTextBox1.PhoneValidationMessage = "Please enter a valid phone number.";
            beepLightTextBox1.PlaceholderText = "";
            beepLightTextBox1.PressedBackColor = Color.White;
            beepLightTextBox1.PressedBorderColor = Color.Gray;
            beepLightTextBox1.PressedForeColor = Color.Gray;
            beepLightTextBox1.ReadOnly = false;
            beepLightTextBox1.RequiredErrorMessage = "This field is required.";
            beepLightTextBox1.RightoffsetForDrawingRect = 0;
            beepLightTextBox1.SavedGuidID = null;
            beepLightTextBox1.SavedID = null;
            beepLightTextBox1.ScrollBars = ScrollBars.None;
            beepLightTextBox1.SelectedBackColor = Color.FromArgb(250, 250, 250);
            beepLightTextBox1.SelectedForeColor = Color.FromArgb(51, 51, 51);
            beepLightTextBox1.SelectedItem = null;
            beepLightTextBox1.SelectedValue = null;
            beepLightTextBox1.ShadowColor = Color.FromArgb(255, 192, 203);
            beepLightTextBox1.ShadowOffset = 0;
            beepLightTextBox1.ShadowOpacity = 0.5F;
            beepLightTextBox1.ShowAllBorders = true;
            beepLightTextBox1.ShowBottomBorder = true;
            beepLightTextBox1.ShowFocusIndicator = false;
            beepLightTextBox1.ShowLeftBorder = true;
            beepLightTextBox1.ShowRightBorder = true;
            beepLightTextBox1.ShowShadow = false;
            beepLightTextBox1.ShowTooltipOnValidationError = true;
            beepLightTextBox1.ShowTopBorder = true;
            beepLightTextBox1.ShowValidationIndicator = true;
            beepLightTextBox1.Size = new Size(184, 41);
            beepLightTextBox1.SlideFrom = SlideDirection.Left;
            beepLightTextBox1.StaticNotMoving = false;
            beepLightTextBox1.TabIndex = 1;
            beepLightTextBox1.Tag = this;
            beepLightTextBox1.TempBackColor = Color.Empty;
            beepLightTextBox1.Text = "beepLightTextBox1";
            beepLightTextBox1.TextAlignment = HorizontalAlignment.Left;
            beepLightTextBox1.TextColor = Color.FromArgb(51, 51, 51);
            beepLightTextBox1.TextFont = new Font("Segoe UI", 11F);
            beepLightTextBox1.Theme = EnumBeepThemes.ZenTheme;
            beepLightTextBox1.TimeFormat = "HH:mm:ss";
            beepLightTextBox1.TimeValidationMessage = "Please enter a valid time.";
            beepLightTextBox1.ToolTipText = "";
            beepLightTextBox1.TopoffsetForDrawingRect = 0;
            beepLightTextBox1.UseGradientBackground = false;
            beepLightTextBox1.UseThemeFont = true;
            beepLightTextBox1.ValidateOnLostFocus = true;
            beepLightTextBox1.ValidationToolTipIcon = ToolTipIcon.Error;
            beepLightTextBox1.ValidationToolTipTitle = "Validation Error";
            beepLightTextBox1.WordWrap = true;
            // 
            // uc_diagraming
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(beepLightTextBox1);
            Controls.Add(beepAccordionMenu1);
            Name = "uc_diagraming";
            Size = new Size(948, 583);
            Theme = EnumBeepThemes.ZenTheme;
            ResumeLayout(false);
        }

        #endregion

        private BeepAccordionMenu beepAccordionMenu1;
        private BeepLightTextBox beepLightTextBox1;
    }
}
