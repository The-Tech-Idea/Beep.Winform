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
            beepPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // CloseButton
            // 
            CloseButton.ApplyThemeOnImage = true;
            CloseButton.BackColor = Color.FromArgb(224, 255, 255);
            CloseButton.ForeColor = Color.FromArgb(0, 105, 148);
            CloseButton.Location = new Point(578, 0);
            CloseButton.ParentBackColor = Color.FromArgb(224, 255, 255);
            CloseButton.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            CloseButton.ToolTipText = "Close";
            // 
            // MaximizeButton
            // 
            MaximizeButton.ApplyThemeOnImage = true;
            MaximizeButton.BackColor = Color.FromArgb(224, 255, 255);
            MaximizeButton.ForeColor = Color.FromArgb(0, 105, 148);
            MaximizeButton.Location = new Point(548, 0);
            MaximizeButton.MaxImageSize = new Size(20, 20);
            MaximizeButton.ParentBackColor = Color.FromArgb(224, 255, 255);
            MaximizeButton.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            MaximizeButton.ToolTipText = "Maximize";
            // 
            // MinimizeButton
            // 
            MinimizeButton.ApplyThemeOnImage = true;
            MinimizeButton.BackColor = Color.FromArgb(224, 255, 255);
            MinimizeButton.ForeColor = Color.FromArgb(0, 105, 148);
            MinimizeButton.Location = new Point(518, 0);
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
            beepPanel1.DrawingRect = new Rectangle(1, 1, 606, 28);
            beepPanel1.ForeColor = Color.FromArgb(0, 128, 255);
            beepPanel1.Location = new Point(210, 10);
            beepPanel1.ParentBackColor = Color.FromArgb(224, 255, 255);
            beepPanel1.Size = new Size(608, 30);
            beepPanel1.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            // 
            // TitleLabel
            // 
            TitleLabel.ApplyThemeOnImage = true;
            TitleLabel.BackColor = Color.FromArgb(224, 255, 255);
            TitleLabel.DrawingRect = new Rectangle(1, 1, 484, 26);
            TitleLabel.Font = new Font("Segoe UI", 12F);
            TitleLabel.ForeColor = Color.FromArgb(0, 105, 148);
            TitleLabel.ImagePath = "H:\\downloads\\9632709-function-button\\9632709-function-button\\svg\\027-restart.svg";
            TitleLabel.ParentBackColor = Color.White;
            TitleLabel.Size = new Size(486, 28);
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
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(1, 1, 198, 651);
            beepSideMenu1.Easing = EasingType.Linear;
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
            beepSideMenu1.Size = new Size(200, 653);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 0;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.OceanTheme;
            beepSideMenu1.Title = "Asset HR Digital WorkSpace";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.UseGradientBackground = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(828, 673);
            Controls.Add(beepSideMenu1);
            DoubleBuffered = true;
            LogoImage = "H:\\downloads\\9632709-function-button\\9632709-function-button\\svg\\027-restart.svg";
            Name = "Form2";
            Padding = new Padding(10);
            Text = "Asset HR Digital WorkSpace";
            Controls.SetChildIndex(beepSideMenu1, 0);
            Controls.SetChildIndex(FunctionsPanel1, 0);
            Controls.SetChildIndex(beepPanel1, 0);
            beepPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BeepSideMenu beepSideMenu1;
      
    }
}