namespace TheTechIdea.Beep.Winform.Controls
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
            beepAppBar1 = new BeepAppBar();
            beepSideMenu1 = new BeepSideMenu();
            beepCard1 = new BeepCard();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepAppBar = beepAppBar1;
            beepuiManager1.BeepiForm = this;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.LogoImage = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007852-team-management\\10007852-team-management\\svg\\003-innovation.svg";
            beepuiManager1.ShowBorder = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepuiManager1.Title = "Simple Info Apps";
            // 
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = DisplayAnimationType.None;
            beepAppBar1.ApplyThemeOnImage = false;
            beepAppBar1.BackColor = Color.FromArgb(70, 130, 180);
            beepAppBar1.BlockID = null;
            beepAppBar1.BorderColor = Color.Black;
            beepAppBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepAppBar1.BorderRadius = 5;
            beepAppBar1.BorderStyle = BorderStyle.FixedSingle;
            beepAppBar1.BorderThickness = 1;
            beepAppBar1.DataContext = null;
            beepAppBar1.DisabledBackColor = Color.Gray;
            beepAppBar1.DisabledForeColor = Color.Empty;
            beepAppBar1.Dock = DockStyle.Top;
            beepAppBar1.DrawingRect = new Rectangle(1, 1, 726, 54);
            beepAppBar1.Easing = EasingType.Linear;
            beepAppBar1.FieldID = null;
            beepAppBar1.FocusBackColor = Color.Gray;
            beepAppBar1.FocusBorderColor = Color.Gray;
            beepAppBar1.FocusForeColor = Color.Black;
            beepAppBar1.FocusIndicatorColor = Color.Blue;
            beepAppBar1.Form = null;
            beepAppBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepAppBar1.GradientEndColor = Color.Gray;
            beepAppBar1.GradientStartColor = Color.Gray;
            beepAppBar1.HoverBackColor = Color.Gray;
            beepAppBar1.HoverBorderColor = Color.Gray;
            beepAppBar1.HoveredBackcolor = Color.Wheat;
            beepAppBar1.HoverForeColor = Color.Black;
            beepAppBar1.Id = -1;
            beepAppBar1.InactiveBackColor = Color.Gray;
            beepAppBar1.InactiveBorderColor = Color.Gray;
            beepAppBar1.InactiveForeColor = Color.Black;
            beepAppBar1.IsAcceptButton = false;
            beepAppBar1.IsBorderAffectedByTheme = true;
            beepAppBar1.IsCancelButton = false;
            beepAppBar1.IsChild = false;
            beepAppBar1.IsCustomeBorder = false;
            beepAppBar1.IsDefault = false;
            beepAppBar1.IsFocused = false;
            beepAppBar1.IsFramless = false;
            beepAppBar1.IsHovered = false;
            beepAppBar1.IsPressed = false;
            beepAppBar1.IsRounded = true;
            beepAppBar1.IsRoundedAffectedByTheme = true;
            beepAppBar1.IsShadowAffectedByTheme = true;
            beepAppBar1.Location = new Point(214, 20);
            beepAppBar1.Name = "beepAppBar1";
            beepAppBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepAppBar1.ParentBackColor = Color.Empty;
            beepAppBar1.PressedBackColor = Color.Gray;
            beepAppBar1.PressedBorderColor = Color.Gray;
            beepAppBar1.PressedForeColor = Color.Black;
            beepAppBar1.SavedGuidID = null;
            beepAppBar1.SavedID = null;
            beepAppBar1.ShadowColor = Color.Black;
            beepAppBar1.ShadowOffset = 0;
            beepAppBar1.ShadowOpacity = 0.5F;
            beepAppBar1.ShowAllBorders = false;
            beepAppBar1.ShowBottomBorder = false;
            beepAppBar1.ShowCloseIcon = true;
            beepAppBar1.ShowFocusIndicator = false;
            beepAppBar1.ShowHamburgerIcon = false;
            beepAppBar1.ShowLeftBorder = false;
            beepAppBar1.ShowLogoIcon = false;
            beepAppBar1.ShowMaximizeIcon = true;
            beepAppBar1.ShowMinimizeIcon = true;
            beepAppBar1.ShowNotificationIcon = true;
            beepAppBar1.ShowProfileIcon = true;
            beepAppBar1.ShowRightBorder = false;
            beepAppBar1.ShowSearchBox = true;
            beepAppBar1.ShowShadow = false;
            beepAppBar1.ShowTopBorder = false;
            beepAppBar1.SideMenu = null;
            beepAppBar1.Size = new Size(728, 56);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 5;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.UseGradientBackground = false;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(65, 105, 225);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationStep = 20;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.ApplyThemeOnImages = false;
            beepSideMenu1.BackColor = Color.FromArgb(200, 225, 245);
            beepSideMenu1.BeepAppBar = null;
            beepSideMenu1.BeepForm = this;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(176, 196, 222);
            beepSideMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSideMenu1.BorderRadius = 5;
            beepSideMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepSideMenu1.BorderThickness = 1;
            beepSideMenu1.CollapsedWidth = 64;
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(1, 1, 202, 751);
            beepSideMenu1.Easing = EasingType.Linear;
            beepSideMenu1.ExpandedWidth = 204;
            beepSideMenu1.FieldID = null;
            beepSideMenu1.FocusBackColor = Color.White;
            beepSideMenu1.FocusBorderColor = Color.Gray;
            beepSideMenu1.FocusForeColor = Color.Black;
            beepSideMenu1.FocusIndicatorColor = Color.Blue;
            beepSideMenu1.Font = new Font("Segoe UI", 9F);
            beepSideMenu1.ForeColor = Color.White;
            beepSideMenu1.Form = null;
            beepSideMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSideMenu1.GradientEndColor = Color.FromArgb(176, 196, 222);
            beepSideMenu1.GradientStartColor = Color.FromArgb(245, 245, 245);
            beepSideMenu1.HilightPanelSize = 5;
            beepSideMenu1.HoverBackColor = Color.FromArgb(70, 130, 180);
            beepSideMenu1.HoverBorderColor = Color.FromArgb(65, 105, 225);
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
            beepSideMenu1.IsCustomeBorder = false;
            beepSideMenu1.IsDefault = false;
            beepSideMenu1.IsFocused = false;
            beepSideMenu1.IsFramless = false;
            beepSideMenu1.IsHovered = false;
            beepSideMenu1.IsPressed = false;
            beepSideMenu1.IsRounded = true;
            beepSideMenu1.IsRoundedAffectedByTheme = true;
            beepSideMenu1.IsShadowAffectedByTheme = false;
            beepSideMenu1.Location = new Point(10, 20);
            beepSideMenu1.LogoImage = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007852-team-management\\10007852-team-management\\svg\\003-innovation.svg";
            beepSideMenu1.Name = "beepSideMenu1";
            beepSideMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepSideMenu1.Padding = new Padding(5);
            beepSideMenu1.ParentBackColor = Color.White;
            beepSideMenu1.PressedBackColor = Color.FromArgb(65, 105, 225);
            beepSideMenu1.PressedBorderColor = Color.Gray;
            beepSideMenu1.PressedForeColor = Color.Black;
            beepSideMenu1.SavedGuidID = null;
            beepSideMenu1.SavedID = null;
            beepSideMenu1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepSideMenu1.ShadowOffset = 0;
            beepSideMenu1.ShadowOpacity = 0.5F;
            beepSideMenu1.ShowAllBorders = false;
            beepSideMenu1.ShowBottomBorder = false;
            beepSideMenu1.ShowFocusIndicator = false;
            beepSideMenu1.ShowLeftBorder = false;
            beepSideMenu1.ShowRightBorder = false;
            beepSideMenu1.ShowShadow = false;
            beepSideMenu1.ShowTopBorder = false;
            beepSideMenu1.Size = new Size(204, 753);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 6;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepSideMenu1.Title = "Simple Info Apps";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.UseGradientBackground = false;
            // 
            // beepCard1
            // 
            beepCard1.ActiveBackColor = Color.Gray;
            beepCard1.AnimationDuration = 500;
            beepCard1.AnimationType = DisplayAnimationType.None;
            beepCard1.BackColor = Color.FromArgb(220, 245, 254);
            beepCard1.BlockID = null;
            beepCard1.BorderColor = Color.Black;
            beepCard1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepCard1.BorderRadius = 5;
            beepCard1.BorderStyle = BorderStyle.FixedSingle;
            beepCard1.BorderThickness = 1;
            beepCard1.DataContext = null;
            beepCard1.DisabledBackColor = Color.Gray;
            beepCard1.DisabledForeColor = Color.Empty;
            beepCard1.DrawingRect = new Rectangle(1, 1, 317, 156);
            beepCard1.Easing = EasingType.Linear;
            beepCard1.FieldID = null;
            beepCard1.FocusBackColor = Color.Gray;
            beepCard1.FocusBorderColor = Color.Gray;
            beepCard1.FocusForeColor = Color.Black;
            beepCard1.FocusIndicatorColor = Color.Blue;
            beepCard1.Form = null;
            beepCard1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepCard1.GradientEndColor = Color.Gray;
            beepCard1.GradientStartColor = Color.Gray;
            beepCard1.HeaderAlignment = ContentAlignment.TopLeft;
            beepCard1.HeaderText = "Success Criteria";
            beepCard1.HoverBackColor = Color.Gray;
            beepCard1.HoverBorderColor = Color.Gray;
            beepCard1.HoveredBackcolor = Color.Wheat;
            beepCard1.HoverForeColor = Color.Black;
            beepCard1.Id = -1;
            beepCard1.ImageAlignment = ContentAlignment.TopRight;
            beepCard1.ImagePath = null;
            beepCard1.InactiveBackColor = Color.Gray;
            beepCard1.InactiveBorderColor = Color.Gray;
            beepCard1.InactiveForeColor = Color.Black;
            beepCard1.IsAcceptButton = false;
            beepCard1.IsBorderAffectedByTheme = true;
            beepCard1.IsCancelButton = false;
            beepCard1.IsChild = false;
            beepCard1.IsCustomeBorder = false;
            beepCard1.IsDefault = false;
            beepCard1.IsFocused = false;
            beepCard1.IsFramless = false;
            beepCard1.IsHovered = false;
            beepCard1.IsPressed = false;
            beepCard1.IsRounded = true;
            beepCard1.IsRoundedAffectedByTheme = true;
            beepCard1.IsShadowAffectedByTheme = true;
            beepCard1.Location = new Point(25, 20);
            beepCard1.MaxImageSize = 64;
            beepCard1.Multiline = true;
            beepCard1.Name = "beepCard1";
            beepCard1.OverrideFontSize = TypeStyleFontSize.None;
            beepCard1.ParagraphText = resources.GetString("beepCard1.ParagraphText");
            beepCard1.ParentBackColor = Color.Empty;
            beepCard1.PressedBackColor = Color.Gray;
            beepCard1.PressedBorderColor = Color.Gray;
            beepCard1.PressedForeColor = Color.Black;
            beepCard1.SavedGuidID = null;
            beepCard1.SavedID = null;
            beepCard1.ShadowColor = Color.Black;
            beepCard1.ShadowOffset = 0;
            beepCard1.ShadowOpacity = 0.5F;
            beepCard1.ShowAllBorders = false;
            beepCard1.ShowBottomBorder = false;
            beepCard1.ShowFocusIndicator = false;
            beepCard1.ShowLeftBorder = false;
            beepCard1.ShowRightBorder = false;
            beepCard1.ShowShadow = false;
            beepCard1.ShowTopBorder = false;
            beepCard1.Size = new Size(319, 158);
            beepCard1.SlideFrom = SlideDirection.Left;
            beepCard1.StaticNotMoving = false;
            beepCard1.TabIndex = 7;
            beepCard1.Text = "beepCard1";
            beepCard1.TextAlignment = ContentAlignment.BottomCenter;
            beepCard1.Theme = Vis.Modules.EnumBeepThemes.WinterTheme;
            beepCard1.ToolTipText = "";
            beepCard1.UseGradientBackground = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(245, 245, 245);
            panel1.Controls.Add(beepCard1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(214, 76);
            panel1.Name = "panel1";
            panel1.Size = new Size(728, 697);
            panel1.TabIndex = 8;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(952, 783);
            Controls.Add(panel1);
            Controls.Add(beepAppBar1);
            Controls.Add(beepSideMenu1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Padding = new Padding(10);
            Text = "Form1";
            Controls.SetChildIndex(beepSideMenu1, 0);
            Controls.SetChildIndex(beepAppBar1, 0);
            Controls.SetChildIndex(panel1, 0);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private BeepSimpleGrid beepSimpleGrid1;
        private BeepDatePicker beepDatePicker1;
        
        private BeepAppBar beepAppBar1;
        private BeepSideMenu beepSideMenu1;
        private BeepCard beepCard1;
        private Panel panel1;
    }
}