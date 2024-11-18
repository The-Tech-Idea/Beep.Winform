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
            beepSideMenu1 = new BeepSideMenu();
            beepPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // CloseButton
            // 
            CloseButton.ApplyThemeOnImage = true;
            CloseButton.BackColor = Color.FromArgb(15, 15, 30);
            CloseButton.Font = new Font("Segoe UI", 12F);
            CloseButton.ForeColor = Color.FromArgb(230, 230, 250);
            CloseButton.Location = new Point(624, 5);
            CloseButton.ParentBackColor = Color.FromArgb(15, 15, 30);
            CloseButton.Theme = Vis.Modules.EnumBeepThemes.MidnightTheme;
            CloseButton.ToolTipText = "Close";
            // 
            // MaximizeButton
            // 
            MaximizeButton.ApplyThemeOnImage = true;
            MaximizeButton.BackColor = Color.FromArgb(15, 15, 30);
            MaximizeButton.Font = new Font("Segoe UI", 12F);
            MaximizeButton.ForeColor = Color.FromArgb(230, 230, 250);
            MaximizeButton.Location = new Point(598, 5);
            MaximizeButton.MaxImageSize = new Size(20, 20);
            MaximizeButton.ParentBackColor = Color.FromArgb(15, 15, 30);
            MaximizeButton.Theme = Vis.Modules.EnumBeepThemes.MidnightTheme;
            MaximizeButton.ToolTipText = "Maximize";
            // 
            // MinimizeButton
            // 
            MinimizeButton.ApplyThemeOnImage = true;
            MinimizeButton.BackColor = Color.FromArgb(15, 15, 30);
            MinimizeButton.Font = new Font("Segoe UI", 12F);
            MinimizeButton.ForeColor = Color.FromArgb(230, 230, 250);
            MinimizeButton.Location = new Point(572, 5);
            MinimizeButton.MaxImageSize = new Size(20, 20);
            MinimizeButton.ParentBackColor = Color.FromArgb(15, 15, 30);
            MinimizeButton.Theme = Vis.Modules.EnumBeepThemes.MidnightTheme;
            MinimizeButton.ToolTipText = "Minimize";
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepiForm = this;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.MidnightTheme;
            // 
            // beepPanel1
            // 
            beepPanel1.BackColor = Color.FromArgb(15, 15, 30);
            beepPanel1.DrawingRect = new Rectangle(1, 1, 655, 45);
            beepPanel1.ForeColor = Color.FromArgb(230, 230, 250);
            beepPanel1.Location = new Point(210, 10);
            beepPanel1.ParentBackColor = Color.FromArgb(15, 15, 30);
            beepPanel1.Size = new Size(657, 47);
            beepPanel1.Theme = Vis.Modules.EnumBeepThemes.MidnightTheme;
            // 
            // TitleLabel
            // 
            TitleLabel.ApplyThemeOnImage = true;
            TitleLabel.BackColor = Color.FromArgb(15, 15, 30);
            TitleLabel.DrawingRect = new Rectangle(1, 1, -2, 26);
            TitleLabel.ForeColor = Color.FromArgb(230, 230, 250);
            TitleLabel.ParentBackColor = Color.White;
            TitleLabel.Size = new Size(0, 28);
            TitleLabel.Theme = Vis.Modules.EnumBeepThemes.MidnightTheme;
            // 
            // FunctionsPanel1
            // 
            FunctionsPanel1.BackColor = Color.FromArgb(15, 15, 30);
            FunctionsPanel1.Location = new Point(822, 63);
            FunctionsPanel1.ParentBackColor = Color.FromArgb(15, 15, 30);
            FunctionsPanel1.Theme = Vis.Modules.EnumBeepThemes.MidnightTheme;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(65, 105, 225);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.BackColor = Color.FromArgb(15, 15, 30);
            beepSideMenu1.BeepForm = this;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(45, 45, 80);
            beepSideMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSideMenu1.BorderRadius = 5;
            beepSideMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepSideMenu1.BorderThickness = 1;
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(1, 1, 198, 600);
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
            beepSideMenu1.GradientEndColor = Color.FromArgb(25, 25, 50);
            beepSideMenu1.GradientStartColor = Color.FromArgb(15, 15, 30);
            beepSideMenu1.HoverBackColor = Color.FromArgb(72, 61, 139);
            beepSideMenu1.HoverBorderColor = Color.FromArgb(100, 149, 237);
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
            beepSideMenu1.Location = new Point(10, 10);
            beepSideMenu1.LogoImage = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg";
            beepSideMenu1.Name = "beepSideMenu1";
            beepSideMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepSideMenu1.ParentBackColor = Color.Empty;
            beepSideMenu1.PressedBackColor = Color.FromArgb(65, 105, 225);
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
            beepSideMenu1.Size = new Size(200, 602);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 0;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.MidnightTheme;
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.UseGradientBackground = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(877, 622);
            Controls.Add(beepSideMenu1);
            DoubleBuffered = true;
            Name = "Form2";
            Padding = new Padding(10);
            Text = "Form2";
            Controls.SetChildIndex(beepSideMenu1, 0);
            Controls.SetChildIndex(beepPanel1, 0);
            Controls.SetChildIndex(FunctionsPanel1, 0);
            beepPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BeepSideMenu beepSideMenu1;
      
    }
}