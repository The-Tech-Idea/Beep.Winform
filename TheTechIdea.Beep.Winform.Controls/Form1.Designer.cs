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
            beepAppBar1 = new BeepAppBar();
            beepMenuBar1 = new BeepMenuBar();
            beepSideMenu1 = new BeepSideMenu();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepAppBar = beepAppBar1;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.LogoImage = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\008-partnership.svg";
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.EarthyTheme;
            // 
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = DisplayAnimationType.None;
            beepAppBar1.ApplyThemeOnImage = false;
            beepAppBar1.ApplyThemeToChilds = true;
            beepAppBar1.BackColor = Color.FromArgb(139, 69, 19);
            beepAppBar1.BlockID = null;
            beepAppBar1.BorderColor = Color.Black;
            beepAppBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepAppBar1.BorderRadius = 1;
            beepAppBar1.BorderStyle = BorderStyle.FixedSingle;
            beepAppBar1.BorderThickness = 1;
            beepAppBar1.BottomoffsetForDrawingRect = 0;
            beepAppBar1.BoundProperty = null;
            beepAppBar1.CanBeFocused = true;
            beepAppBar1.CanBeHovered = false;
            beepAppBar1.CanBePressed = true;
            beepAppBar1.Category = Utilities.DbFieldCategory.String;
            beepAppBar1.ComponentName = "beepAppBar1";
            beepAppBar1.DataContext = null;
            beepAppBar1.DataSourceProperty = null;
            beepAppBar1.DisabledBackColor = Color.Gray;
            beepAppBar1.DisabledForeColor = Color.Empty;
            beepAppBar1.Dock = DockStyle.Top;
            beepAppBar1.DrawingRect = new Rectangle(1, 1, 648, 31);
            beepAppBar1.Easing = EasingType.Linear;
            beepAppBar1.FieldID = null;
            beepAppBar1.FocusBackColor = Color.Gray;
            beepAppBar1.FocusBorderColor = Color.Gray;
            beepAppBar1.FocusForeColor = Color.Black;
            beepAppBar1.FocusIndicatorColor = Color.Blue;
            beepAppBar1.ForeColor = Color.White;
            beepAppBar1.Form = null;
            beepAppBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepAppBar1.GradientEndColor = Color.Gray;
            beepAppBar1.GradientStartColor = Color.Gray;
            beepAppBar1.GuidID = "3d2ffe02-9f86-40f7-a9ec-f9dabd0dcf44";
            beepAppBar1.HoverBackColor = Color.Gray;
            beepAppBar1.HoverBorderColor = Color.Gray;
            beepAppBar1.HoveredBackcolor = Color.Wheat;
            beepAppBar1.HoverForeColor = Color.Black;
            beepAppBar1.Id = -1;
            beepAppBar1.InactiveBackColor = Color.Gray;
            beepAppBar1.InactiveBorderColor = Color.Gray;
            beepAppBar1.InactiveForeColor = Color.Black;
            beepAppBar1.IsAcceptButton = false;
            beepAppBar1.IsBorderAffectedByTheme = false;
            beepAppBar1.IsCancelButton = false;
            beepAppBar1.IsChild = false;
            beepAppBar1.IsCustomeBorder = false;
            beepAppBar1.IsDefault = false;
            beepAppBar1.IsFocused = false;
            beepAppBar1.IsFramless = true;
            beepAppBar1.IsHovered = false;
            beepAppBar1.IsPressed = false;
            beepAppBar1.IsRounded = true;
            beepAppBar1.IsRoundedAffectedByTheme = false;
            beepAppBar1.IsShadowAffectedByTheme = false;
            beepAppBar1.LeftoffsetForDrawingRect = 0;
            beepAppBar1.LinkedProperty = null;
            beepAppBar1.Location = new Point(147, 3);
            beepAppBar1.Name = "beepAppBar1";
            beepAppBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepAppBar1.ParentBackColor = Color.Empty;
            beepAppBar1.PressedBackColor = Color.Gray;
            beepAppBar1.PressedBorderColor = Color.Gray;
            beepAppBar1.PressedForeColor = Color.Black;
            beepAppBar1.RightoffsetForDrawingRect = 0;
            beepAppBar1.SavedGuidID = null;
            beepAppBar1.SavedID = null;
            beepAppBar1.ShadowColor = Color.Black;
            beepAppBar1.ShadowOffset = 0;
            beepAppBar1.ShadowOpacity = 0.5F;
            beepAppBar1.ShowAllBorders = true;
            beepAppBar1.ShowBottomBorder = true;
            beepAppBar1.ShowCloseIcon = true;
            beepAppBar1.ShowFocusIndicator = false;
            beepAppBar1.ShowLeftBorder = true;
            beepAppBar1.ShowLogoIcon = false;
            beepAppBar1.ShowMaximizeIcon = true;
            beepAppBar1.ShowMinimizeIcon = true;
            beepAppBar1.ShowNotificationIcon = true;
            beepAppBar1.ShowProfileIcon = true;
            beepAppBar1.ShowRightBorder = true;
            beepAppBar1.ShowSearchBox = true;
            beepAppBar1.ShowShadow = false;
            beepAppBar1.ShowTitle = false;
            beepAppBar1.ShowTopBorder = true;
            beepAppBar1.SideMenu = null;
            beepAppBar1.Size = new Size(650, 33);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 0;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.EarthyTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.TopoffsetForDrawingRect = 0;
            beepAppBar1.UseGradientBackground = false;
            // 
            // beepMenuBar1
            // 
            beepMenuBar1.ActiveBackColor = Color.FromArgb(85, 107, 47);
            beepMenuBar1.AnimationDuration = 500;
            beepMenuBar1.AnimationType = DisplayAnimationType.None;
            beepMenuBar1.ApplyThemeToChilds = true;
            beepMenuBar1.BackColor = Color.FromArgb(160, 82, 45);
            beepMenuBar1.BlockID = null;
            beepMenuBar1.BorderColor = Color.FromArgb(160, 82, 45);
            beepMenuBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepMenuBar1.BorderRadius = 1;
            beepMenuBar1.BorderStyle = BorderStyle.FixedSingle;
            beepMenuBar1.BorderThickness = 1;
            beepMenuBar1.BottomoffsetForDrawingRect = 0;
            beepMenuBar1.BoundProperty = "SelectedItem";
            beepMenuBar1.CanBeFocused = true;
            beepMenuBar1.CanBeHovered = false;
            beepMenuBar1.CanBePressed = true;
            beepMenuBar1.Category = Utilities.DbFieldCategory.String;
            beepMenuBar1.ComponentName = "beepMenuBar1";
            beepMenuBar1.DataContext = null;
            beepMenuBar1.DataSourceProperty = null;
            beepMenuBar1.DisabledBackColor = Color.Gray;
            beepMenuBar1.DisabledForeColor = Color.Empty;
            beepMenuBar1.Dock = DockStyle.Top;
            beepMenuBar1.DrawingRect = new Rectangle(1, 1, 648, 18);
            beepMenuBar1.Easing = EasingType.Linear;
            beepMenuBar1.FieldID = null;
            beepMenuBar1.FocusBackColor = Color.White;
            beepMenuBar1.FocusBorderColor = Color.Gray;
            beepMenuBar1.FocusForeColor = Color.Black;
            beepMenuBar1.FocusIndicatorColor = Color.Blue;
            beepMenuBar1.Form = null;
            beepMenuBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepMenuBar1.GradientEndColor = Color.FromArgb(160, 82, 45);
            beepMenuBar1.GradientStartColor = Color.FromArgb(245, 245, 220);
            beepMenuBar1.GuidID = "2a522637-29f9-4339-b720-6a738a174d54";
            beepMenuBar1.HoverBackColor = Color.FromArgb(160, 82, 45);
            beepMenuBar1.HoverBorderColor = Color.FromArgb(160, 82, 45);
            beepMenuBar1.HoveredBackcolor = Color.Wheat;
            beepMenuBar1.HoverForeColor = Color.Black;
            beepMenuBar1.Id = -1;
            beepMenuBar1.ImageSize = 14;
            beepMenuBar1.InactiveBackColor = Color.Gray;
            beepMenuBar1.InactiveBorderColor = Color.Gray;
            beepMenuBar1.InactiveForeColor = Color.Black;
            beepMenuBar1.IsAcceptButton = false;
            beepMenuBar1.IsBorderAffectedByTheme = true;
            beepMenuBar1.IsCancelButton = false;
            beepMenuBar1.IsChild = false;
            beepMenuBar1.IsCustomeBorder = false;
            beepMenuBar1.IsDefault = false;
            beepMenuBar1.IsFocused = false;
            beepMenuBar1.IsFramless = true;
            beepMenuBar1.IsHovered = false;
            beepMenuBar1.IsPressed = false;
            beepMenuBar1.IsRounded = true;
            beepMenuBar1.IsRoundedAffectedByTheme = true;
            beepMenuBar1.IsShadowAffectedByTheme = true;
            beepMenuBar1.LeftoffsetForDrawingRect = 0;
            beepMenuBar1.LinkedProperty = null;
            beepMenuBar1.Location = new Point(147, 36);
            beepMenuBar1.MenuItemHeight = 16;
            beepMenuBar1.MenuItemWidth = 60;
            beepMenuBar1.Name = "beepMenuBar1";
            beepMenuBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepMenuBar1.ParentBackColor = Color.Empty;
            beepMenuBar1.PressedBackColor = Color.FromArgb(85, 107, 47);
            beepMenuBar1.PressedBorderColor = Color.Gray;
            beepMenuBar1.PressedForeColor = Color.Black;
            beepMenuBar1.RightoffsetForDrawingRect = 0;
            beepMenuBar1.SavedGuidID = null;
            beepMenuBar1.SavedID = null;
            beepMenuBar1.SelectedIndex = -1;
            beepMenuBar1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepMenuBar1.ShadowOffset = 0;
            beepMenuBar1.ShadowOpacity = 0.5F;
            beepMenuBar1.ShowAllBorders = true;
            beepMenuBar1.ShowBottomBorder = true;
            beepMenuBar1.ShowFocusIndicator = false;
            beepMenuBar1.ShowLeftBorder = true;
            beepMenuBar1.ShowRightBorder = true;
            beepMenuBar1.ShowShadow = false;
            beepMenuBar1.ShowTopBorder = true;
            beepMenuBar1.Size = new Size(650, 20);
            beepMenuBar1.SlideFrom = SlideDirection.Left;
            beepMenuBar1.StaticNotMoving = false;
            beepMenuBar1.TabIndex = 1;
            beepMenuBar1.Text = "beepMenuBar1";
            beepMenuBar1.Theme = Vis.Modules.EnumBeepThemes.EarthyTheme;
            beepMenuBar1.ToolTipText = "";
            beepMenuBar1.TopoffsetForDrawingRect = 0;
            beepMenuBar1.UseGradientBackground = false;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(85, 107, 47);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationStep = 20;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.ApplyThemeOnImages = false;
            beepSideMenu1.ApplyThemeToChilds = false;
            beepSideMenu1.BackColor = Color.FromArgb(160, 82, 45);
            beepSideMenu1.BeepAppBar = null;
            beepSideMenu1.BeepForm = null;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(160, 82, 45);
            beepSideMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSideMenu1.BorderRadius = 1;
            beepSideMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepSideMenu1.BorderThickness = 1;
            beepSideMenu1.BottomoffsetForDrawingRect = 0;
            beepSideMenu1.BoundProperty = null;
            beepSideMenu1.ButtonSize = new Size(188, 40);
            beepSideMenu1.CanBeFocused = true;
            beepSideMenu1.CanBeHovered = false;
            beepSideMenu1.CanBePressed = true;
            beepSideMenu1.Category = Utilities.DbFieldCategory.String;
            beepSideMenu1.CollapsedWidth = 64;
            beepSideMenu1.ComponentName = "beepSideMenu1";
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DataSourceProperty = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(6, 6, 132, 432);
            beepSideMenu1.Easing = EasingType.Linear;
            beepSideMenu1.ExpandedWidth = 200;
            beepSideMenu1.FieldID = null;
            beepSideMenu1.FocusBackColor = Color.White;
            beepSideMenu1.FocusBorderColor = Color.Gray;
            beepSideMenu1.FocusForeColor = Color.Black;
            beepSideMenu1.FocusIndicatorColor = Color.Blue;
            beepSideMenu1.Font = new Font("Segoe UI", 9F);
            beepSideMenu1.ForeColor = Color.White;
            beepSideMenu1.Form = null;
            beepSideMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSideMenu1.GradientEndColor = Color.FromArgb(160, 82, 45);
            beepSideMenu1.GradientStartColor = Color.FromArgb(245, 245, 220);
            beepSideMenu1.GuidID = "7252db3a-4432-44c1-88fe-9d418e7bfa85";
            beepSideMenu1.HilightPanelSize = 5;
            beepSideMenu1.HoverBackColor = Color.FromArgb(160, 82, 45);
            beepSideMenu1.HoverBorderColor = Color.FromArgb(160, 82, 45);
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
            beepSideMenu1.IsFramless = true;
            beepSideMenu1.IsHovered = false;
            beepSideMenu1.IsPressed = false;
            beepSideMenu1.IsRounded = true;
            beepSideMenu1.IsRoundedAffectedByTheme = true;
            beepSideMenu1.IsShadowAffectedByTheme = false;
            beepSideMenu1.LeftoffsetForDrawingRect = 0;
            beepSideMenu1.LinkedProperty = null;
            beepSideMenu1.Location = new Point(3, 3);
            beepSideMenu1.LogoImage = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\008-partnership.svg";
            beepSideMenu1.Name = "beepSideMenu1";
            beepSideMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepSideMenu1.Padding = new Padding(5);
            beepSideMenu1.ParentBackColor = Color.Empty;
            beepSideMenu1.PressedBackColor = Color.FromArgb(85, 107, 47);
            beepSideMenu1.PressedBorderColor = Color.Gray;
            beepSideMenu1.PressedForeColor = Color.Black;
            beepSideMenu1.RightoffsetForDrawingRect = 0;
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
            beepSideMenu1.Size = new Size(144, 444);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 2;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.EarthyTheme;
            beepSideMenu1.Title = "Beep Form";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.TopoffsetForDrawingRect = 0;
            beepSideMenu1.UseGradientBackground = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(160, 82, 45);
            ClientSize = new Size(800, 450);
            Controls.Add(beepMenuBar1);
            Controls.Add(beepAppBar1);
            Controls.Add(beepSideMenu1);
            Name = "Form1";
            Text = "Form1";
            Theme = Vis.Modules.EnumBeepThemes.EarthyTheme;
            ResumeLayout(false);
        }

        #endregion

        private BeepAppBar beepAppBar1;
        private BeepMenuBar beepMenuBar1;
        private BeepSideMenu beepSideMenu1;
    }
}