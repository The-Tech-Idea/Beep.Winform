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
            beepMenuBar1 = new BeepMenuBar();
            beepSideMenu1 = new BeepSideMenu();
            beepTree1 = new BeepTree();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepAppBar = beepAppBar1;
            beepuiManager1.BeepSideMenu = beepSideMenu1;
            beepuiManager1.LogoImage = "H:\\dev\\iconPacks\\10007852-team-management (2) (1)\\10007856-team-management\\10007856-team-management\\svg\\008-partnership.svg";
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
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
            beepAppBar1.ForeColor = Color.Black;
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
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.TopoffsetForDrawingRect = 0;
            beepAppBar1.UseGradientBackground = false;
            // 
            // beepMenuBar1
            // 
            beepMenuBar1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepMenuBar1.AnimationDuration = 500;
            beepMenuBar1.AnimationType = DisplayAnimationType.None;
            beepMenuBar1.ApplyThemeToChilds = true;
            beepMenuBar1.BackColor = Color.FromArgb(230, 230, 240);
            beepMenuBar1.BlockID = null;
            beepMenuBar1.BorderColor = Color.FromArgb(200, 200, 200);
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
            beepMenuBar1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepMenuBar1.GradientStartColor = Color.White;
            beepMenuBar1.GuidID = "2a522637-29f9-4339-b720-6a738a174d54";
            beepMenuBar1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepMenuBar1.HoverBorderColor = Color.FromArgb(0, 120, 215);
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
            beepMenuBar1.PressedBackColor = Color.FromArgb(0, 120, 215);
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
            beepMenuBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepMenuBar1.ToolTipText = "";
            beepMenuBar1.TopoffsetForDrawingRect = 0;
            beepMenuBar1.UseGradientBackground = false;
            // 
            // beepSideMenu1
            // 
            beepSideMenu1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepSideMenu1.AnimationDuration = 500;
            beepSideMenu1.AnimationStep = 20;
            beepSideMenu1.AnimationType = DisplayAnimationType.None;
            beepSideMenu1.ApplyThemeOnImages = false;
            beepSideMenu1.ApplyThemeToChilds = false;
            beepSideMenu1.BackColor = Color.FromArgb(230, 230, 240);
            beepSideMenu1.BeepAppBar = null;
            beepSideMenu1.BeepForm = null;
            beepSideMenu1.BlockID = null;
            beepSideMenu1.BorderColor = Color.FromArgb(200, 200, 200);
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
            beepSideMenu1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepSideMenu1.GradientStartColor = Color.White;
            beepSideMenu1.GuidID = "7252db3a-4432-44c1-88fe-9d418e7bfa85";
            beepSideMenu1.HilightPanelSize = 5;
            beepSideMenu1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepSideMenu1.HoverBorderColor = Color.FromArgb(0, 120, 215);
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
            beepSideMenu1.PressedBackColor = Color.FromArgb(0, 120, 215);
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
            beepSideMenu1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepSideMenu1.Title = "Beep Form";
            beepSideMenu1.ToolTipText = "";
            beepSideMenu1.TopoffsetForDrawingRect = 0;
            beepSideMenu1.UseGradientBackground = false;
            // 
            // beepTree1
            // 
            beepTree1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepTree1.AllowMultiSelect = false;
            beepTree1.AnimationDuration = 500;
            beepTree1.AnimationType = DisplayAnimationType.None;
            beepTree1.ApplyThemeToChilds = false;
            beepTree1.AutoScroll = true;
            beepTree1.BackColor = Color.FromArgb(245, 245, 245);
            beepTree1.BlockID = null;
            beepTree1.BorderColor = Color.FromArgb(200, 200, 200);
            beepTree1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepTree1.BorderRadius = 1;
            beepTree1.BorderStyle = BorderStyle.FixedSingle;
            beepTree1.BorderThickness = 1;
            beepTree1.BottomoffsetForDrawingRect = 0;
            beepTree1.BoundProperty = null;
            beepTree1.CanBeFocused = true;
            beepTree1.CanBeHovered = false;
            beepTree1.CanBePressed = true;
            beepTree1.Category = Utilities.DbFieldCategory.String;
            beepTree1.ComponentName = "BeepTree";
            beepTree1.DataContext = null;
            beepTree1.DataSourceProperty = null;
            beepTree1.DisabledBackColor = Color.Gray;
            beepTree1.DisabledForeColor = Color.Empty;
            beepTree1.DrawingRect = new Rectangle(6, 6, 158, 353);
            beepTree1.Easing = EasingType.Linear;
            beepTree1.FieldID = null;
            beepTree1.FocusBackColor = Color.White;
            beepTree1.FocusBorderColor = Color.Gray;
            beepTree1.FocusForeColor = Color.Black;
            beepTree1.FocusIndicatorColor = Color.Blue;
            beepTree1.Form = null;
            beepTree1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepTree1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepTree1.GradientStartColor = Color.White;
            beepTree1.GuidID = "2eef0cc6-60d0-4986-bfa5-6239656e61ae";
            beepTree1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepTree1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepTree1.HoveredBackcolor = Color.Wheat;
            beepTree1.HoverForeColor = Color.Black;
            beepTree1.Id = -1;
            beepTree1.InactiveBackColor = Color.Gray;
            beepTree1.InactiveBorderColor = Color.Gray;
            beepTree1.InactiveForeColor = Color.Black;
            beepTree1.IsAcceptButton = false;
            beepTree1.IsBorderAffectedByTheme = true;
            beepTree1.IsCancelButton = false;
            beepTree1.IsChild = false;
            beepTree1.IsCustomeBorder = false;
            beepTree1.IsDefault = false;
            beepTree1.IsFocused = false;
            beepTree1.IsFramless = false;
            beepTree1.IsHovered = false;
            beepTree1.IsPressed = false;
            beepTree1.IsRounded = true;
            beepTree1.IsRoundedAffectedByTheme = true;
            beepTree1.IsShadowAffectedByTheme = true;
            beepTree1.LeftoffsetForDrawingRect = 0;
            beepTree1.LinkedProperty = null;
            beepTree1.Location = new Point(204, 62);
            beepTree1.Name = "beepTree1";
            beepTree1.NodeHeight = 40;
            beepTree1.NodeImageSize = 16;
            beepTree1.Nodes.Add((Desktop.Common.SimpleItem)resources.GetObject("beepTree1.Nodes"));
            beepTree1.Nodes.Add((Desktop.Common.SimpleItem)resources.GetObject("beepTree1.Nodes1"));
            beepTree1.Nodes.Add((Desktop.Common.SimpleItem)resources.GetObject("beepTree1.Nodes2"));
            beepTree1.Nodeseq = 27;
            beepTree1.NodeWidth = 100;
            beepTree1.OverrideFontSize = TypeStyleFontSize.None;
            beepTree1.Padding = new Padding(5);
            beepTree1.ParentBackColor = Color.Empty;
            beepTree1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepTree1.PressedBorderColor = Color.Gray;
            beepTree1.PressedForeColor = Color.Black;
            beepTree1.RightoffsetForDrawingRect = 0;
            beepTree1.SavedGuidID = null;
            beepTree1.SavedID = null;
            beepTree1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepTree1.ShadowOffset = 0;
            beepTree1.ShadowOpacity = 0.5F;
            beepTree1.ShowAllBorders = true;
            beepTree1.ShowBottomBorder = true;
            beepTree1.ShowFocusIndicator = false;
            beepTree1.ShowLeftBorder = true;
            beepTree1.ShowNodeImage = true;
            beepTree1.ShowRightBorder = true;
            beepTree1.ShowShadow = false;
            beepTree1.ShowTopBorder = true;
            beepTree1.Size = new Size(170, 365);
            beepTree1.SlideFrom = SlideDirection.Left;
            beepTree1.StaticNotMoving = false;
            beepTree1.TabIndex = 3;
            beepTree1.Text = "beepTree1";
            beepTree1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepTree1.ToolTipText = "";
            beepTree1.TopoffsetForDrawingRect = 0;
            beepTree1.UseGradientBackground = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(800, 450);
            Controls.Add(beepTree1);
            Controls.Add(beepMenuBar1);
            Controls.Add(beepAppBar1);
            Controls.Add(beepSideMenu1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private BeepAppBar beepAppBar1;
        private BeepMenuBar beepMenuBar1;
        private BeepSideMenu beepSideMenu1;
        private BeepTree beepTree1;
    }
}