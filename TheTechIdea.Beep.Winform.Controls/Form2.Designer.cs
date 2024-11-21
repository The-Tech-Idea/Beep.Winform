namespace TheTechIdea.Beep.Winform.Controls
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            beepSideMenu1 = new BeepSideMenu();
            beepLabel1 = new BeepLabel();
            beepPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // CloseButton
            // 
            CloseButton.ApplyThemeOnImage = true;
            CloseButton.BackColor = Color.FromArgb(224, 255, 255);
            CloseButton.ForeColor = Color.FromArgb(0, 160, 176);
            CloseButton.Location = new Point(386, 0);
            CloseButton.ParentBackColor = Color.FromArgb(224, 255, 255);
            CloseButton.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            CloseButton.ToolTipText = "Close";
            // 
            // MaximizeButton
            // 
            MaximizeButton.ApplyThemeOnImage = true;
            MaximizeButton.BackColor = Color.FromArgb(224, 255, 255);
            MaximizeButton.ForeColor = Color.FromArgb(0, 160, 176);
            MaximizeButton.Location = new Point(356, 0);
            MaximizeButton.MaxImageSize = new Size(20, 20);
            MaximizeButton.ParentBackColor = Color.FromArgb(224, 255, 255);
            MaximizeButton.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            MaximizeButton.ToolTipText = "Maximize";
            // 
            // MinimizeButton
            // 
            MinimizeButton.ApplyThemeOnImage = true;
            MinimizeButton.BackColor = Color.FromArgb(224, 255, 255);
            MinimizeButton.ForeColor = Color.FromArgb(0, 160, 176);
            MinimizeButton.Location = new Point(326, 0);
            MinimizeButton.MaxImageSize = new Size(20, 20);
            MinimizeButton.ParentBackColor = Color.FromArgb(224, 255, 255);
            MinimizeButton.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            MinimizeButton.ToolTipText = "Minimize";
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepiForm = this;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.LogoImage = "H:\\downloads\\9632709-function-button\\9632709-function-button\\svg\\027-restart.svg";
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            beepuiManager1.Title = "Asset HR Digital WorkSpace";
            // 
            // beepPanel1
            // 
            beepPanel1.BackColor = Color.FromArgb(224, 255, 255);
            beepPanel1.DrawingRect = new Rectangle(1, 1, 414, 28);
            beepPanel1.ForeColor = Color.FromArgb(0, 128, 255);
            beepPanel1.Location = new Point(402, 10);
            beepPanel1.ParentBackColor = Color.FromArgb(224, 255, 255);
            beepPanel1.Size = new Size(416, 30);
            beepPanel1.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            // 
            // TitleLabel
            // 
            TitleLabel.ApplyThemeOnImage = true;
            TitleLabel.BackColor = Color.FromArgb(224, 255, 255);
            TitleLabel.DrawingRect = new Rectangle(1, 1, 292, 26);
            TitleLabel.ForeColor = Color.FromArgb(0, 160, 176);
            TitleLabel.ImagePath = "H:\\downloads\\9632709-function-button\\9632709-function-button\\svg\\027-restart.svg";
            TitleLabel.ParentBackColor = Color.White;
            TitleLabel.Size = new Size(294, 28);
            TitleLabel.Text = "Asset HR Digital WorkSpace";
            TitleLabel.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            // 
            // FunctionsPanel1
            // 
            FunctionsPanel1.BackColor = Color.FromArgb(224, 255, 255);
            FunctionsPanel1.Location = new Point(773, 63);
            FunctionsPanel1.ParentBackColor = Color.FromArgb(224, 255, 255);
            FunctionsPanel1.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(0, 123, 167);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationStep = 20;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.ApplyThemeOnImages = false;
            beepSideMenu1.BackColor = Color.FromArgb(224, 255, 255);
            beepSideMenu1.BeepForm = this;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(0, 160, 176);
            beepSideMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSideMenu1.BorderRadius = 5;
            beepSideMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepSideMenu1.BorderThickness = 1;
            beepSideMenu1.CollapsedWidth = 64;
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(1, 1, 390, 651);
            beepSideMenu1.Easing = EasingType.Linear;
            beepSideMenu1.ExpandedWidth = 392;
            beepSideMenu1.FieldID = null;
            beepSideMenu1.FocusBackColor = Color.White;
            beepSideMenu1.FocusBorderColor = Color.Gray;
            beepSideMenu1.FocusForeColor = Color.Black;
            beepSideMenu1.FocusIndicatorColor = Color.Blue;
            beepSideMenu1.Font = new Font("Segoe UI", 9F);
            beepSideMenu1.ForeColor = Color.White;
            beepSideMenu1.Form = null;
            beepSideMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSideMenu1.GradientEndColor = Color.FromArgb(175, 238, 238);
            beepSideMenu1.GradientStartColor = Color.FromArgb(224, 255, 255);
            beepSideMenu1.HilightPanelSize = 5;
            beepSideMenu1.HoverBackColor = Color.FromArgb(0, 139, 139);
            beepSideMenu1.HoverBorderColor = Color.FromArgb(0, 139, 139);
            beepSideMenu1.HoveredBackcolor = Color.Wheat;
            beepSideMenu1.HoverForeColor = Color.Black;
            beepSideMenu1.Id = -1;
            beepSideMenu1.InactiveBackColor = Color.Gray;
            beepSideMenu1.InactiveBorderColor = Color.Gray;
            beepSideMenu1.InactiveForeColor = Color.Black;
            beepSideMenu1.IsAcceptButton = false;
            beepSideMenu1.IsBorderAffectedByTheme = false;
            beepSideMenu1.IsCancelButton = false;
            beepSideMenu1.IsChild = false;
            beepSideMenu1.IsDefault = false;
            beepSideMenu1.IsFocused = false;
            beepSideMenu1.IsFramless = false;
            beepSideMenu1.IsHovered = false;
            beepSideMenu1.IsPressed = false;
            beepSideMenu1.IsRounded = false;
            beepSideMenu1.IsShadowAffectedByTheme = false;
            beepSideMenu1.Items.Add((Template.SimpleMenuItem)resources.GetObject("beepSideMenu1.Items"));
            beepSideMenu1.Items.Add((Template.SimpleMenuItem)resources.GetObject("beepSideMenu1.Items1"));
            beepSideMenu1.Location = new Point(10, 10);
            beepSideMenu1.LogoImage = null;
            beepSideMenu1.Name = "beepSideMenu1";
            beepSideMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepSideMenu1.Padding = new Padding(2);
            beepSideMenu1.ParentBackColor = Color.Empty;
            beepSideMenu1.PressedBackColor = Color.FromArgb(0, 123, 167);
            beepSideMenu1.PressedBorderColor = Color.Gray;
            beepSideMenu1.PressedForeColor = Color.Black;
            beepSideMenu1.SavedGuidID = null;
            beepSideMenu1.SavedID = null;
            beepSideMenu1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepSideMenu1.ShadowOffset = 0;
            beepSideMenu1.ShadowOpacity = 0.5F;
            beepSideMenu1.ShowAllBorders = true;
            beepSideMenu1.ShowBottomBorder = true;
            beepSideMenu1.ShowFocusIndicator = false;
            beepSideMenu1.ShowLeftBorder = true;
            beepSideMenu1.ShowRightBorder = true;
            beepSideMenu1.ShowShadow = false;
            beepSideMenu1.ShowTopBorder = true;
            beepSideMenu1.Size = new Size(392, 653);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 0;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            beepSideMenu1.Title = "Asset HR Digital WorkSpace";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.UseGradientBackground = false;
            // 
            // beepLabel1
            // 
            beepLabel1.ActiveBackColor = Color.Gray;
            beepLabel1.AnimationDuration = 500;
            beepLabel1.AnimationType = DisplayAnimationType.None;
            beepLabel1.ApplyThemeOnImage = false;
            beepLabel1.BackColor = Color.FromArgb(224, 255, 255);
            beepLabel1.BlockID = null;
            beepLabel1.BorderColor = Color.Black;
            beepLabel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepLabel1.BorderRadius = 5;
            beepLabel1.BorderStyle = BorderStyle.FixedSingle;
            beepLabel1.BorderThickness = 1;
            beepLabel1.DataContext = null;
            beepLabel1.DisabledBackColor = Color.Gray;
            beepLabel1.DisabledForeColor = Color.Empty;
            beepLabel1.DrawingRect = new Rectangle(1, 1, 118, 38);
            beepLabel1.Easing = EasingType.Linear;
            beepLabel1.FieldID = null;
            beepLabel1.FocusBackColor = Color.Gray;
            beepLabel1.FocusBorderColor = Color.Gray;
            beepLabel1.FocusForeColor = Color.Black;
            beepLabel1.FocusIndicatorColor = Color.Blue;
            beepLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            beepLabel1.ForeColor = Color.FromArgb(0, 105, 148);
            beepLabel1.Form = null;
            beepLabel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepLabel1.GradientEndColor = Color.Gray;
            beepLabel1.GradientStartColor = Color.Gray;
            beepLabel1.HideText = false;
            beepLabel1.HoverBackColor = Color.Gray;
            beepLabel1.HoverBorderColor = Color.Gray;
            beepLabel1.HoveredBackcolor = Color.Wheat;
            beepLabel1.HoverForeColor = Color.Black;
            beepLabel1.Id = -1;
            beepLabel1.ImageAlign = ContentAlignment.MiddleLeft;
            beepLabel1.ImagePath = null;
            beepLabel1.InactiveBackColor = Color.Gray;
            beepLabel1.InactiveBorderColor = Color.Gray;
            beepLabel1.InactiveForeColor = Color.Black;
            beepLabel1.IsAcceptButton = false;
            beepLabel1.IsBorderAffectedByTheme = true;
            beepLabel1.IsCancelButton = false;
            beepLabel1.IsChild = false;
            beepLabel1.IsDefault = false;
            beepLabel1.IsFocused = false;
            beepLabel1.IsFramless = false;
            beepLabel1.IsHovered = false;
            beepLabel1.IsPressed = false;
            beepLabel1.IsRounded = false;
            beepLabel1.IsShadowAffectedByTheme = true;
            beepLabel1.Location = new Point(433, 327);
            beepLabel1.Margin = new Padding(0);
            beepLabel1.MaxImageSize = new Size(16, 16);
            beepLabel1.Name = "beepLabel1";
            beepLabel1.OverrideFontSize = TypeStyleFontSize.None;
            beepLabel1.ParentBackColor = Color.Empty;
            beepLabel1.PressedBackColor = Color.Gray;
            beepLabel1.PressedBorderColor = Color.Gray;
            beepLabel1.PressedForeColor = Color.Black;
            beepLabel1.SavedGuidID = null;
            beepLabel1.SavedID = null;
            beepLabel1.ShadowColor = Color.Black;
            beepLabel1.ShadowOffset = 0;
            beepLabel1.ShadowOpacity = 0.5F;
            beepLabel1.ShowAllBorders = true;
            beepLabel1.ShowBottomBorder = true;
            beepLabel1.ShowFocusIndicator = false;
            beepLabel1.ShowLeftBorder = true;
            beepLabel1.ShowRightBorder = true;
            beepLabel1.ShowShadow = false;
            beepLabel1.ShowTopBorder = true;
            beepLabel1.Size = new Size(120, 40);
            beepLabel1.SlideFrom = SlideDirection.Left;
            beepLabel1.StaticNotMoving = false;
            beepLabel1.TabIndex = 6;
            beepLabel1.Text = "beepLabel1";
            beepLabel1.TextAlign = ContentAlignment.MiddleLeft;
            beepLabel1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepLabel1.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            beepLabel1.ToolTipText = "";
            beepLabel1.UseGradientBackground = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(828, 673);
            Controls.Add(beepLabel1);
            Controls.Add(beepSideMenu1);
            DoubleBuffered = true;
            Name = "Form2";
            Padding = new Padding(10);
            Text = "Asset HR Digital WorkSpace";
            Controls.SetChildIndex(beepSideMenu1, 0);
            Controls.SetChildIndex(FunctionsPanel1, 0);
            Controls.SetChildIndex(beepPanel1, 0);
            Controls.SetChildIndex(beepLabel1, 0);
            beepPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BeepSideMenu beepSideMenu1;
        private BeepLabel beepLabel1;
    }
}