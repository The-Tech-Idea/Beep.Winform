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
            beepListBox1 = new BeepListBox();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepAppBar = beepAppBar1;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            // 
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = DisplayAnimationType.None;
            beepAppBar1.ApplyThemeOnImage = false;
            beepAppBar1.ApplyThemeToChilds = true;
            beepAppBar1.BackColor = Color.FromArgb(230, 230, 230);
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
            beepAppBar1.DrawingRect = new Rectangle(1, 1, 488, 30);
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
            beepAppBar1.IsBorderAffectedByTheme = false;
            beepAppBar1.IsCancelButton = false;
            beepAppBar1.IsChild = false;
            beepAppBar1.IsCustomeBorder = false;
            beepAppBar1.IsDefault = false;
            beepAppBar1.IsFocused = false;
            beepAppBar1.IsFramless = false;
            beepAppBar1.IsHovered = false;
            beepAppBar1.IsPressed = false;
            beepAppBar1.IsRounded = true;
            beepAppBar1.IsRoundedAffectedByTheme = false;
            beepAppBar1.IsShadowAffectedByTheme = false;
            beepAppBar1.Location = new Point(305, 5);
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
            beepAppBar1.ShowTopBorder = true;
            beepAppBar1.SideMenu = null;
            beepAppBar1.Size = new Size(490, 32);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 0;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.UseGradientBackground = false;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(200, 200, 200);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationStep = 20;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.ApplyThemeOnImages = false;
            beepSideMenu1.ApplyThemeToChilds = false;
            beepSideMenu1.BackColor = Color.FromArgb(230, 230, 230);
            beepSideMenu1.BeepAppBar = null;
            beepSideMenu1.BeepForm = null;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(200, 200, 200);
            beepSideMenu1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSideMenu1.BorderRadius = 5;
            beepSideMenu1.BorderStyle = BorderStyle.FixedSingle;
            beepSideMenu1.BorderThickness = 1;
            beepSideMenu1.CollapsedWidth = 64;
            beepSideMenu1.DataContext = null;
            beepSideMenu1.DisabledBackColor = Color.Gray;
            beepSideMenu1.DisabledForeColor = Color.Empty;
            beepSideMenu1.Dock = DockStyle.Left;
            beepSideMenu1.DrawingRect = new Rectangle(1, 1, 298, 531);
            beepSideMenu1.Easing = EasingType.Linear;
            beepSideMenu1.ExpandedWidth = 300;
            beepSideMenu1.FieldID = null;
            beepSideMenu1.FocusBackColor = Color.Black;
            beepSideMenu1.FocusBorderColor = Color.Gray;
            beepSideMenu1.FocusForeColor = Color.Black;
            beepSideMenu1.FocusIndicatorColor = Color.Blue;
            beepSideMenu1.Font = new Font("Segoe UI", 9F);
            beepSideMenu1.ForeColor = Color.White;
            beepSideMenu1.Form = null;
            beepSideMenu1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSideMenu1.GradientEndColor = Color.White;
            beepSideMenu1.GradientStartColor = Color.White;
            beepSideMenu1.HilightPanelSize = 5;
            beepSideMenu1.HoverBackColor = Color.FromArgb(210, 210, 210);
            beepSideMenu1.HoverBorderColor = Color.Black;
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
            beepSideMenu1.Location = new Point(5, 5);
            beepSideMenu1.LogoImage = "";
            beepSideMenu1.Name = "beepSideMenu1";
            beepSideMenu1.OverrideFontSize = TypeStyleFontSize.None;
            beepSideMenu1.Padding = new Padding(5);
            beepSideMenu1.ParentBackColor = Color.White;
            beepSideMenu1.PressedBackColor = Color.FromArgb(200, 200, 200);
            beepSideMenu1.PressedBorderColor = Color.Gray;
            beepSideMenu1.PressedForeColor = Color.Black;
            beepSideMenu1.SavedGuidID = null;
            beepSideMenu1.SavedID = null;
            beepSideMenu1.ShadowColor = Color.Empty;
            beepSideMenu1.ShadowOffset = 0;
            beepSideMenu1.ShadowOpacity = 0.5F;
            beepSideMenu1.ShowAllBorders = true;
            beepSideMenu1.ShowBottomBorder = true;
            beepSideMenu1.ShowFocusIndicator = false;
            beepSideMenu1.ShowLeftBorder = true;
            beepSideMenu1.ShowRightBorder = true;
            beepSideMenu1.ShowShadow = false;
            beepSideMenu1.ShowTopBorder = true;
            beepSideMenu1.Size = new Size(300, 533);
            beepSideMenu1.SlideFrom = SlideDirection.Left;
            beepSideMenu1.StaticNotMoving = false;
            beepSideMenu1.TabIndex = 1;
            beepSideMenu1.Text = "beepSideMenu1";
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            beepSideMenu1.Title = "Beep Form";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.UseGradientBackground = false;
            // 
            // beepListBox1
            // 
            beepListBox1.ActiveBackColor = Color.Gray;
            beepListBox1.AnimationDuration = 500;
            beepListBox1.AnimationType = DisplayAnimationType.None;
            beepListBox1.ApplyThemeToChilds = true;
            beepListBox1.BackColor = Color.FromArgb(230, 230, 230);
            beepListBox1.BlockID = null;
            beepListBox1.BorderColor = Color.Black;
            beepListBox1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepListBox1.BorderRadius = 5;
            beepListBox1.BorderStyle = BorderStyle.FixedSingle;
            beepListBox1.BorderThickness = 1;
            beepListBox1.DataContext = null;
            beepListBox1.DisabledBackColor = Color.Gray;
            beepListBox1.DisabledForeColor = Color.Empty;
            beepListBox1.DrawingRect = new Rectangle(1, 1, 174, 333);
            beepListBox1.Easing = EasingType.Linear;
            beepListBox1.FieldID = null;
            beepListBox1.FocusBackColor = Color.Gray;
            beepListBox1.FocusBorderColor = Color.Gray;
            beepListBox1.FocusForeColor = Color.Black;
            beepListBox1.FocusIndicatorColor = Color.Blue;
            beepListBox1.Form = null;
            beepListBox1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepListBox1.GradientEndColor = Color.Gray;
            beepListBox1.GradientStartColor = Color.Gray;
            beepListBox1.HoverBackColor = Color.Gray;
            beepListBox1.HoverBorderColor = Color.Gray;
            beepListBox1.HoveredBackcolor = Color.Wheat;
            beepListBox1.HoverForeColor = Color.Black;
            beepListBox1.Id = -1;
            beepListBox1.InactiveBackColor = Color.Gray;
            beepListBox1.InactiveBorderColor = Color.Gray;
            beepListBox1.InactiveForeColor = Color.Black;
            beepListBox1.IsAcceptButton = false;
            beepListBox1.IsBorderAffectedByTheme = true;
            beepListBox1.IsCancelButton = false;
            beepListBox1.IsChild = false;
            beepListBox1.IsCustomeBorder = false;
            beepListBox1.IsDefault = false;
            beepListBox1.IsFocused = false;
            beepListBox1.IsFramless = false;
            beepListBox1.IsHovered = false;
            beepListBox1.IsPressed = false;
            beepListBox1.IsRounded = true;
            beepListBox1.IsRoundedAffectedByTheme = true;
            beepListBox1.IsShadowAffectedByTheme = true;
            beepListBox1.ListItems.Add((Models.SimpleItem)resources.GetObject("beepListBox1.ListItems"));
            beepListBox1.ListItems.Add((Models.SimpleItem)resources.GetObject("beepListBox1.ListItems1"));
            beepListBox1.Location = new Point(394, 135);
            beepListBox1.MenuItemHeight = 20;
            beepListBox1.Name = "beepListBox1";
            beepListBox1.OverrideFontSize = TypeStyleFontSize.None;
            beepListBox1.ParentBackColor = Color.Empty;
            beepListBox1.PressedBackColor = Color.Gray;
            beepListBox1.PressedBorderColor = Color.Gray;
            beepListBox1.PressedForeColor = Color.Black;
            beepListBox1.SavedGuidID = null;
            beepListBox1.SavedID = null;
            beepListBox1.SelectedIndex = -1;
            beepListBox1.ShadowColor = Color.Black;
            beepListBox1.ShadowOffset = 0;
            beepListBox1.ShadowOpacity = 0.5F;
            beepListBox1.ShowAllBorders = true;
            beepListBox1.ShowBottomBorder = true;
            beepListBox1.ShowFocusIndicator = false;
            beepListBox1.ShowImage = true;
            beepListBox1.ShowLeftBorder = true;
            beepListBox1.ShowRightBorder = true;
            beepListBox1.ShowShadow = false;
            beepListBox1.ShowTitle = true;
            beepListBox1.ShowTitleLine = true;
            beepListBox1.ShowTitleLineinFullWidth = true;
            beepListBox1.ShowTopBorder = true;
            beepListBox1.Size = new Size(176, 335);
            beepListBox1.SlideFrom = SlideDirection.Left;
            beepListBox1.StaticNotMoving = false;
            beepListBox1.TabIndex = 2;
            beepListBox1.Text = "beepListBox1";
            beepListBox1.Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            beepListBox1.TitleAlignment = ContentAlignment.TopLeft;
            beepListBox1.TitleBottomY = 40;
            beepListBox1.TitleLineColor = Color.Gray;
            beepListBox1.TitleLineThickness = 2;
            beepListBox1.TitleText = "List Box";
            beepListBox1.ToolTipText = "";
            beepListBox1.UseGradientBackground = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(800, 543);
            Controls.Add(beepListBox1);
            Controls.Add(beepAppBar1);
            Controls.Add(beepSideMenu1);
            Name = "Form1";
            Text = "Form1";
            Theme = Vis.Modules.EnumBeepThemes.MonochromeTheme;
            ResumeLayout(false);
        }

        #endregion

        private BeepAppBar beepAppBar1;
        private BeepSideMenu beepSideMenu1;
        private BeepListBox beepListBox1;
    }
}