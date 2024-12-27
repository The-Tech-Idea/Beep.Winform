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
            beepTree1 = new BeepTree();
            beepAppBar1 = new BeepAppBar();
            beepDataNavigator1 = new BeepDataNavigator();
            beepLabel1 = new BeepLabel();
            SuspendLayout();
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
            beepTree1.BorderRadius = 5;
            beepTree1.BorderStyle = BorderStyle.FixedSingle;
            beepTree1.BorderThickness = 1;
            beepTree1.BottomoffsetForDrawingRect = 0;
            beepTree1.BoundProperty = null;
            beepTree1.CanBeFocused = true;
            beepTree1.CanBeHovered = false;
            beepTree1.CanBePressed = true;
            beepTree1.DataContext = null;
            beepTree1.DisabledBackColor = Color.Gray;
            beepTree1.DisabledForeColor = Color.Empty;
            beepTree1.DrawingRect = new Rectangle(9, 9, 194, 338);
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
            beepTree1.GuidID = "b5c1cb99-481d-4ba4-a94e-d0d862737f06";
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
            beepTree1.Location = new Point(153, 57);
            beepTree1.Name = "beepTree1";
            beepTree1.NodeHeight = 40;
            beepTree1.NodeImageSize = 16;
            beepTree1.Nodes.Add((Common.SimpleItem)resources.GetObject("beepTree1.Nodes"));
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
            beepTree1.ShadowOffset = 3;
            beepTree1.ShadowOpacity = 0.5F;
            beepTree1.ShowAllBorders = true;
            beepTree1.ShowBottomBorder = true;
            beepTree1.ShowFocusIndicator = false;
            beepTree1.ShowLeftBorder = true;
            beepTree1.ShowNodeImage = true;
            beepTree1.ShowRightBorder = true;
            beepTree1.ShowShadow = true;
            beepTree1.ShowTopBorder = true;
            beepTree1.Size = new Size(212, 356);
            beepTree1.SlideFrom = SlideDirection.Left;
            beepTree1.StaticNotMoving = false;
            beepTree1.TabIndex = 0;
            beepTree1.Text = "beepTree1";
            beepTree1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepTree1.ToolTipText = "";
            beepTree1.TopoffsetForDrawingRect = 0;
            beepTree1.UseGradientBackground = false;
            // 
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = DisplayAnimationType.None;
            beepAppBar1.ApplyThemeOnImage = true;
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
            beepAppBar1.DataContext = null;
            beepAppBar1.DisabledBackColor = Color.Gray;
            beepAppBar1.DisabledForeColor = Color.Empty;
            beepAppBar1.Dock = DockStyle.Top;
            beepAppBar1.DrawingRect = new Rectangle(0, 0, 800, 31);
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
            beepAppBar1.GuidID = "102ed23a-9a01-44ca-94d2-57488d555e28";
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
            beepAppBar1.Location = new Point(0, 0);
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
            beepAppBar1.ShowAllBorders = false;
            beepAppBar1.ShowBottomBorder = false;
            beepAppBar1.ShowCloseIcon = true;
            beepAppBar1.ShowFocusIndicator = false;
            beepAppBar1.ShowLeftBorder = false;
            beepAppBar1.ShowLogoIcon = true;
            beepAppBar1.ShowMaximizeIcon = true;
            beepAppBar1.ShowMinimizeIcon = true;
            beepAppBar1.ShowNotificationIcon = true;
            beepAppBar1.ShowProfileIcon = true;
            beepAppBar1.ShowRightBorder = false;
            beepAppBar1.ShowSearchBox = true;
            beepAppBar1.ShowShadow = false;
            beepAppBar1.ShowTopBorder = false;
            beepAppBar1.SideMenu = null;
            beepAppBar1.Size = new Size(800, 31);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 1;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.TopoffsetForDrawingRect = 0;
            beepAppBar1.UseGradientBackground = false;
            // 
            // beepDataNavigator1
            // 
            beepDataNavigator1.ActiveBackColor = Color.Gray;
            beepDataNavigator1.AnimationDuration = 500;
            beepDataNavigator1.AnimationType = DisplayAnimationType.None;
            beepDataNavigator1.ApplyThemeToChilds = false;
            beepDataNavigator1.BackColor = Color.FromArgb(240, 240, 240);
            beepDataNavigator1.BlockID = null;
            beepDataNavigator1.BorderColor = Color.Black;
            beepDataNavigator1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepDataNavigator1.BorderRadius = 1;
            beepDataNavigator1.BorderStyle = BorderStyle.FixedSingle;
            beepDataNavigator1.BorderThickness = 1;
            beepDataNavigator1.BottomoffsetForDrawingRect = 0;
            beepDataNavigator1.BoundProperty = null;
            beepDataNavigator1.ButtonHeight = -8;
            beepDataNavigator1.ButtonSpacing = 5;
            beepDataNavigator1.ButtonWidth = 15;
            beepDataNavigator1.CanBeFocused = true;
            beepDataNavigator1.CanBeHovered = false;
            beepDataNavigator1.CanBePressed = true;
            beepDataNavigator1.DataContext = null;
            beepDataNavigator1.DisabledBackColor = Color.Gray;
            beepDataNavigator1.DisabledForeColor = Color.Empty;
            beepDataNavigator1.DrawingRect = new Rectangle(1, 1, 325, 21);
            beepDataNavigator1.Easing = EasingType.Linear;
            beepDataNavigator1.FieldID = null;
            beepDataNavigator1.FocusBackColor = Color.Gray;
            beepDataNavigator1.FocusBorderColor = Color.Gray;
            beepDataNavigator1.FocusForeColor = Color.Black;
            beepDataNavigator1.FocusIndicatorColor = Color.Blue;
            beepDataNavigator1.Form = null;
            beepDataNavigator1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepDataNavigator1.GradientEndColor = Color.Gray;
            beepDataNavigator1.GradientStartColor = Color.Gray;
            beepDataNavigator1.GuidID = "50e35c2c-2332-439a-bb80-7ace0f04dd0d";
            beepDataNavigator1.HoverBackColor = Color.Gray;
            beepDataNavigator1.HoverBorderColor = Color.Gray;
            beepDataNavigator1.HoveredBackcolor = Color.Wheat;
            beepDataNavigator1.HoverForeColor = Color.Black;
            beepDataNavigator1.Id = -1;
            beepDataNavigator1.InactiveBackColor = Color.Gray;
            beepDataNavigator1.InactiveBorderColor = Color.Gray;
            beepDataNavigator1.InactiveForeColor = Color.Black;
            beepDataNavigator1.IsAcceptButton = false;
            beepDataNavigator1.IsBorderAffectedByTheme = false;
            beepDataNavigator1.IsCancelButton = false;
            beepDataNavigator1.IsChild = false;
            beepDataNavigator1.IsCustomeBorder = false;
            beepDataNavigator1.IsDefault = false;
            beepDataNavigator1.IsFocused = false;
            beepDataNavigator1.IsFramless = false;
            beepDataNavigator1.IsHovered = false;
            beepDataNavigator1.IsPressed = false;
            beepDataNavigator1.IsRounded = true;
            beepDataNavigator1.IsRoundedAffectedByTheme = true;
            beepDataNavigator1.IsShadowAffectedByTheme = false;
            beepDataNavigator1.LeftoffsetForDrawingRect = 0;
            beepDataNavigator1.Location = new Point(384, 101);
            beepDataNavigator1.Name = "beepDataNavigator1";
            beepDataNavigator1.OverrideFontSize = TypeStyleFontSize.None;
            beepDataNavigator1.ParentBackColor = Color.Empty;
            beepDataNavigator1.PressedBackColor = Color.Gray;
            beepDataNavigator1.PressedBorderColor = Color.Gray;
            beepDataNavigator1.PressedForeColor = Color.Black;
            beepDataNavigator1.RightoffsetForDrawingRect = 0;
            beepDataNavigator1.SavedGuidID = null;
            beepDataNavigator1.SavedID = null;
            beepDataNavigator1.ShadowColor = Color.Black;
            beepDataNavigator1.ShadowOffset = 0;
            beepDataNavigator1.ShadowOpacity = 0.5F;
            beepDataNavigator1.ShowAllBorders = true;
            beepDataNavigator1.ShowBottomBorder = true;
            beepDataNavigator1.ShowFocusIndicator = false;
            beepDataNavigator1.ShowLeftBorder = true;
            beepDataNavigator1.ShowPrint = false;
            beepDataNavigator1.ShowRightBorder = true;
            beepDataNavigator1.ShowSendEmail = false;
            beepDataNavigator1.ShowShadow = false;
            beepDataNavigator1.ShowTopBorder = true;
            beepDataNavigator1.Size = new Size(327, 23);
            beepDataNavigator1.SlideFrom = SlideDirection.Left;
            beepDataNavigator1.StaticNotMoving = false;
            beepDataNavigator1.TabIndex = 2;
            beepDataNavigator1.Text = "beepDataNavigator1";
            beepDataNavigator1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepDataNavigator1.ToolTipText = "";
            beepDataNavigator1.TopoffsetForDrawingRect = 0;
            beepDataNavigator1.UnitOfWork = null;
            beepDataNavigator1.UseGradientBackground = false;
            beepDataNavigator1.XOffset = 2;
            beepDataNavigator1.YOffset = 2;
            // 
            // beepLabel1
            // 
            beepLabel1.ActiveBackColor = Color.Gray;
            beepLabel1.AnimationDuration = 500;
            beepLabel1.AnimationType = DisplayAnimationType.None;
            beepLabel1.ApplyThemeOnImage = false;
            beepLabel1.ApplyThemeToChilds = true;
            beepLabel1.BackColor = SystemColors.Control;
            beepLabel1.BlockID = null;
            beepLabel1.BorderColor = Color.Black;
            beepLabel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepLabel1.BorderRadius = 1;
            beepLabel1.BorderStyle = BorderStyle.FixedSingle;
            beepLabel1.BorderThickness = 1;
            beepLabel1.BottomoffsetForDrawingRect = 0;
            beepLabel1.BoundProperty = "Text";
            beepLabel1.CanBeFocused = true;
            beepLabel1.CanBeHovered = false;
            beepLabel1.CanBePressed = true;
            beepLabel1.DataContext = null;
            beepLabel1.DisabledBackColor = Color.Gray;
            beepLabel1.DisabledForeColor = Color.Empty;
            beepLabel1.DrawingRect = new Rectangle(1, 1, 98, 21);
            beepLabel1.Easing = EasingType.Linear;
            beepLabel1.FieldID = null;
            beepLabel1.FocusBackColor = Color.Gray;
            beepLabel1.FocusBorderColor = Color.Gray;
            beepLabel1.FocusForeColor = Color.Black;
            beepLabel1.FocusIndicatorColor = Color.Blue;
            beepLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            beepLabel1.ForeColor = Color.Black;
            beepLabel1.Form = null;
            beepLabel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepLabel1.GradientEndColor = Color.Gray;
            beepLabel1.GradientStartColor = Color.Gray;
            beepLabel1.GuidID = "a270c952-fe11-4fb7-8003-44e829918fb6";
            beepLabel1.HideText = false;
            beepLabel1.HoverBackColor = Color.FromArgb(230, 230, 230);
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
            beepLabel1.IsChild = true;
            beepLabel1.IsCustomeBorder = false;
            beepLabel1.IsDefault = false;
            beepLabel1.IsFocused = false;
            beepLabel1.IsFramless = false;
            beepLabel1.IsHovered = false;
            beepLabel1.IsPressed = false;
            beepLabel1.IsRounded = true;
            beepLabel1.IsRoundedAffectedByTheme = true;
            beepLabel1.IsShadowAffectedByTheme = true;
            beepLabel1.LeftoffsetForDrawingRect = 0;
            beepLabel1.Location = new Point(536, 267);
            beepLabel1.Margin = new Padding(0);
            beepLabel1.MaxImageSize = new Size(16, 16);
            beepLabel1.MaximumSize = new Size(0, 23);
            beepLabel1.MinimumSize = new Size(0, 23);
            beepLabel1.Name = "beepLabel1";
            beepLabel1.OverrideFontSize = TypeStyleFontSize.None;
            beepLabel1.ParentBackColor = SystemColors.Control;
            beepLabel1.PressedBackColor = Color.Gray;
            beepLabel1.PressedBorderColor = Color.Gray;
            beepLabel1.PressedForeColor = Color.Black;
            beepLabel1.RightoffsetForDrawingRect = 0;
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
            beepLabel1.Size = new Size(100, 23);
            beepLabel1.SlideFrom = SlideDirection.Left;
            beepLabel1.StaticNotMoving = false;
            beepLabel1.TabIndex = 3;
            beepLabel1.Text = "beepLabel1";
            beepLabel1.TextAlign = ContentAlignment.MiddleLeft;
            beepLabel1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepLabel1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepLabel1.ToolTipText = "";
            beepLabel1.TopoffsetForDrawingRect = 0;
            beepLabel1.UseGradientBackground = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(beepLabel1);
            Controls.Add(beepDataNavigator1);
            Controls.Add(beepAppBar1);
            Controls.Add(beepTree1);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
        }

        #endregion

        private BeepTree beepTree1;
        private BeepAppBar beepAppBar1;
        private BeepDataNavigator beepDataNavigator1;
        private BeepLabel beepLabel1;
    }
}