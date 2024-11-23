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
            beepProgressBar1 = new BeepProgressBar();
            beepAppBar1 = new BeepAppBar();
            beepButton1 = new BeepButton();
            beepButton2 = new BeepButton();
            SuspendLayout();
            // 
            // beepProgressBar1
            // 
            beepProgressBar1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepProgressBar1.AnimationDuration = 500;
            beepProgressBar1.AnimationType = DisplayAnimationType.None;
            beepProgressBar1.BlockID = null;
            beepProgressBar1.BorderColor = Color.FromArgb(200, 200, 200);
            beepProgressBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepProgressBar1.BorderRadius = 5;
            beepProgressBar1.BorderStyle = BorderStyle.FixedSingle;
            beepProgressBar1.BorderThickness = 1;
            beepProgressBar1.CustomText = "";
            beepProgressBar1.DataContext = null;
            beepProgressBar1.DisabledBackColor = Color.Gray;
            beepProgressBar1.DisabledForeColor = Color.Empty;
            beepProgressBar1.DrawingRect = new Rectangle(4, 4, 166, 22);
            beepProgressBar1.Easing = EasingType.Linear;
            beepProgressBar1.FieldID = null;
            beepProgressBar1.FocusBackColor = Color.White;
            beepProgressBar1.FocusBorderColor = Color.Gray;
            beepProgressBar1.FocusForeColor = Color.Black;
            beepProgressBar1.FocusIndicatorColor = Color.Blue;
            beepProgressBar1.Form = null;
            beepProgressBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepProgressBar1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepProgressBar1.GradientStartColor = Color.White;
            beepProgressBar1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepProgressBar1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepProgressBar1.HoveredBackcolor = Color.Wheat;
            beepProgressBar1.HoverForeColor = Color.Black;
            beepProgressBar1.Id = -1;
            beepProgressBar1.InactiveBackColor = Color.Gray;
            beepProgressBar1.InactiveBorderColor = Color.Gray;
            beepProgressBar1.InactiveForeColor = Color.Black;
            beepProgressBar1.IsAcceptButton = false;
            beepProgressBar1.IsBorderAffectedByTheme = true;
            beepProgressBar1.IsCancelButton = false;
            beepProgressBar1.IsChild = false;
            beepProgressBar1.IsDefault = false;
            beepProgressBar1.IsFocused = false;
            beepProgressBar1.IsFramless = false;
            beepProgressBar1.IsHovered = false;
            beepProgressBar1.IsPressed = false;
            beepProgressBar1.IsRounded = true;
            beepProgressBar1.IsShadowAffectedByTheme = true;
            beepProgressBar1.Location = new Point(426, 265);
            beepProgressBar1.Maximum = 100;
            beepProgressBar1.Minimum = 0;
            beepProgressBar1.Name = "beepProgressBar1";
            beepProgressBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepProgressBar1.ParentBackColor = Color.Empty;
            beepProgressBar1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepProgressBar1.PressedBorderColor = Color.Gray;
            beepProgressBar1.PressedForeColor = Color.Black;
            beepProgressBar1.ProgressColor = Color.LightGreen;
            beepProgressBar1.SavedGuidID = null;
            beepProgressBar1.SavedID = null;
            beepProgressBar1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepProgressBar1.ShadowOffset = 3;
            beepProgressBar1.ShadowOpacity = 0.5F;
            beepProgressBar1.ShowAllBorders = true;
            beepProgressBar1.ShowBottomBorder = true;
            beepProgressBar1.ShowFocusIndicator = false;
            beepProgressBar1.ShowLeftBorder = true;
            beepProgressBar1.ShowRightBorder = true;
            beepProgressBar1.ShowShadow = true;
            beepProgressBar1.ShowTopBorder = true;
            beepProgressBar1.Size = new Size(174, 30);
            beepProgressBar1.SlideFrom = SlideDirection.Left;
            beepProgressBar1.StaticNotMoving = false;
            beepProgressBar1.TabIndex = 0;
            beepProgressBar1.Text = "beepProgressBar1";
            beepProgressBar1.TextColor = Color.Black;
            beepProgressBar1.TextFont = new Font("Times New Roman", 11F, FontStyle.Bold | FontStyle.Italic);
            beepProgressBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepProgressBar1.ToolTipText = "";
            beepProgressBar1.UseGradientBackground = false;
            beepProgressBar1.Value = 0;
            beepProgressBar1.VisualMode = ProgressBarDisplayMode.CurrProgress;
            // 
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = DisplayAnimationType.None;
            beepAppBar1.ApplyThemeOnImage = false;
            beepAppBar1.BackColor = Color.FromArgb(240, 240, 240);
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
            beepAppBar1.DrawingRect = new Rectangle(1, 1, 850, 35);
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
            beepAppBar1.IsDefault = false;
            beepAppBar1.IsFocused = false;
            beepAppBar1.IsFramless = false;
            beepAppBar1.IsHovered = false;
            beepAppBar1.IsPressed = false;
            beepAppBar1.IsRounded = true;
            beepAppBar1.IsShadowAffectedByTheme = true;
            beepAppBar1.Location = new Point(0, 0);
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
            beepAppBar1.ShowFocusIndicator = false;
            beepAppBar1.ShowLeftBorder = false;
            beepAppBar1.ShowRightBorder = false;
            beepAppBar1.ShowShadow = false;
            beepAppBar1.ShowTopBorder = false;
            beepAppBar1.SideMenu = null;
            beepAppBar1.Size = new Size(852, 37);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 1;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.ZenTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.UseGradientBackground = false;
            // 
            // beepButton1
            // 
            beepButton1.ActiveBackColor = Color.Gray;
            beepButton1.AnimationDuration = 500;
            beepButton1.AnimationType = DisplayAnimationType.None;
            beepButton1.ApplyThemeOnImage = false;
            beepButton1.BackColor = Color.White;
            beepButton1.BlockID = null;
            beepButton1.BorderColor = Color.Black;
            beepButton1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepButton1.BorderRadius = 5;
            beepButton1.BorderSize = 1;
            beepButton1.BorderStyle = BorderStyle.FixedSingle;
            beepButton1.BorderThickness = 1;
            beepButton1.DataContext = null;
            beepButton1.DisabledBackColor = Color.Gray;
            beepButton1.DisabledForeColor = Color.Empty;
            beepButton1.DrawingRect = new Rectangle(4, 4, 112, 32);
            beepButton1.Easing = EasingType.Linear;
            beepButton1.FieldID = null;
            beepButton1.FlatAppearance = true;
            beepButton1.FlatStyle = FlatStyle.Standard;
            beepButton1.FocusBackColor = Color.Gray;
            beepButton1.FocusBorderColor = Color.Gray;
            beepButton1.FocusForeColor = Color.Black;
            beepButton1.FocusIndicatorColor = Color.Blue;
            beepButton1.Font = new Font("Segoe UI", 12F);
            beepButton1.ForeColor = Color.Black;
            beepButton1.Form = null;
            beepButton1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepButton1.GradientEndColor = Color.Gray;
            beepButton1.GradientStartColor = Color.Gray;
            beepButton1.HideText = false;
            beepButton1.HoverBackColor = Color.Gray;
            beepButton1.HoverBorderColor = Color.Gray;
            beepButton1.HoveredBackcolor = Color.Wheat;
            beepButton1.HoverForeColor = Color.Black;
            beepButton1.Id = -1;
            beepButton1.Image = null;
            beepButton1.ImageAlign = ContentAlignment.MiddleLeft;
            beepButton1.ImageClicked = null;
            beepButton1.ImagePath = null;
            beepButton1.InactiveBackColor = Color.Gray;
            beepButton1.InactiveBorderColor = Color.Gray;
            beepButton1.InactiveForeColor = Color.Black;
            beepButton1.IsAcceptButton = false;
            beepButton1.IsBorderAffectedByTheme = true;
            beepButton1.IsCancelButton = false;
            beepButton1.IsChild = false;
            beepButton1.IsDefault = false;
            beepButton1.IsFocused = false;
            beepButton1.IsFramless = false;
            beepButton1.IsHovered = false;
            beepButton1.IsPressed = false;
            beepButton1.IsRounded = true;
            beepButton1.IsSelected = false;
            beepButton1.IsShadowAffectedByTheme = true;
            beepButton1.IsSideMenuChild = false;
            beepButton1.IsStillButton = false;
            beepButton1.Location = new Point(253, 188);
            beepButton1.Margin = new Padding(0);
            beepButton1.MaxImageSize = new Size(32, 32);
            beepButton1.Name = "beepButton1";
            beepButton1.OverrideFontSize = TypeStyleFontSize.None;
            beepButton1.ParentBackColor = Color.Empty;
            beepButton1.PressedBackColor = Color.Gray;
            beepButton1.PressedBorderColor = Color.Gray;
            beepButton1.PressedForeColor = Color.Black;
            beepButton1.SavedGuidID = null;
            beepButton1.SavedID = null;
            beepButton1.SelectedBorderColor = Color.Blue;
            beepButton1.ShadowColor = Color.Black;
            beepButton1.ShadowOffset = 3;
            beepButton1.ShadowOpacity = 0.5F;
            beepButton1.ShowAllBorders = true;
            beepButton1.ShowBottomBorder = true;
            beepButton1.ShowFocusIndicator = false;
            beepButton1.ShowLeftBorder = true;
            beepButton1.ShowRightBorder = true;
            beepButton1.ShowShadow = true;
            beepButton1.ShowTopBorder = true;
            beepButton1.Size = new Size(120, 40);
            beepButton1.SlideFrom = SlideDirection.Left;
            beepButton1.StaticNotMoving = false;
            beepButton1.TabIndex = 2;
            beepButton1.Text = "beepButton1";
            beepButton1.TextAlign = ContentAlignment.MiddleCenter;
            beepButton1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepButton1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepButton1.ToolTipText = "";
            beepButton1.UseGradientBackground = false;
            // 
            // beepButton2
            // 
            beepButton2.ActiveBackColor = Color.Gray;
            beepButton2.AnimationDuration = 500;
            beepButton2.AnimationType = DisplayAnimationType.None;
            beepButton2.ApplyThemeOnImage = false;
            beepButton2.BackColor = Color.White;
            beepButton2.BlockID = null;
            beepButton2.BorderColor = Color.Black;
            beepButton2.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepButton2.BorderRadius = 5;
            beepButton2.BorderSize = 1;
            beepButton2.BorderStyle = BorderStyle.FixedSingle;
            beepButton2.BorderThickness = 1;
            beepButton2.DataContext = null;
            beepButton2.DisabledBackColor = Color.Gray;
            beepButton2.DisabledForeColor = Color.Empty;
            beepButton2.DrawingRect = new Rectangle(1, 1, 18, 21);
            beepButton2.Easing = EasingType.Linear;
            beepButton2.FieldID = null;
            beepButton2.FlatAppearance = true;
            beepButton2.FlatStyle = FlatStyle.Standard;
            beepButton2.FocusBackColor = Color.Gray;
            beepButton2.FocusBorderColor = Color.Gray;
            beepButton2.FocusForeColor = Color.Black;
            beepButton2.FocusIndicatorColor = Color.Blue;
            beepButton2.Font = new Font("Segoe UI", 12F);
            beepButton2.ForeColor = Color.Black;
            beepButton2.Form = null;
            beepButton2.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepButton2.GradientEndColor = Color.Gray;
            beepButton2.GradientStartColor = Color.Gray;
            beepButton2.HideText = true;
            beepButton2.HoverBackColor = Color.Gray;
            beepButton2.HoverBorderColor = Color.Gray;
            beepButton2.HoveredBackcolor = Color.Wheat;
            beepButton2.HoverForeColor = Color.Black;
            beepButton2.Id = -1;
            beepButton2.Image = null;
            beepButton2.ImageAlign = ContentAlignment.MiddleCenter;
            beepButton2.ImageClicked = null;
            beepButton2.ImagePath = "H:\\dev\\iconPacks\\svg_interface_ui\\sort-numeric-down-alt.svg";
            beepButton2.InactiveBackColor = Color.Gray;
            beepButton2.InactiveBorderColor = Color.Gray;
            beepButton2.InactiveForeColor = Color.Black;
            beepButton2.IsAcceptButton = false;
            beepButton2.IsBorderAffectedByTheme = true;
            beepButton2.IsCancelButton = false;
            beepButton2.IsChild = false;
            beepButton2.IsDefault = false;
            beepButton2.IsFocused = false;
            beepButton2.IsFramless = false;
            beepButton2.IsHovered = false;
            beepButton2.IsPressed = false;
            beepButton2.IsRounded = true;
            beepButton2.IsSelected = false;
            beepButton2.IsShadowAffectedByTheme = true;
            beepButton2.IsSideMenuChild = false;
            beepButton2.IsStillButton = false;
            beepButton2.Location = new Point(267, 307);
            beepButton2.Margin = new Padding(0);
            beepButton2.MaxImageSize = new Size(15, 15);
            beepButton2.Name = "beepButton2";
            beepButton2.OverrideFontSize = TypeStyleFontSize.None;
            beepButton2.ParentBackColor = Color.Empty;
            beepButton2.PressedBackColor = Color.Gray;
            beepButton2.PressedBorderColor = Color.Gray;
            beepButton2.PressedForeColor = Color.Black;
            beepButton2.SavedGuidID = null;
            beepButton2.SavedID = null;
            beepButton2.SelectedBorderColor = Color.Blue;
            beepButton2.ShadowColor = Color.Black;
            beepButton2.ShadowOffset = 0;
            beepButton2.ShadowOpacity = 0.5F;
            beepButton2.ShowAllBorders = false;
            beepButton2.ShowBottomBorder = false;
            beepButton2.ShowFocusIndicator = false;
            beepButton2.ShowLeftBorder = false;
            beepButton2.ShowRightBorder = false;
            beepButton2.ShowShadow = false;
            beepButton2.ShowTopBorder = false;
            beepButton2.Size = new Size(20, 23);
            beepButton2.SlideFrom = SlideDirection.Left;
            beepButton2.StaticNotMoving = false;
            beepButton2.TabIndex = 3;
            beepButton2.Text = "beepButton2";
            beepButton2.TextAlign = ContentAlignment.MiddleCenter;
            beepButton2.TextImageRelation = TextImageRelation.Overlay;
            beepButton2.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepButton2.ToolTipText = "";
            beepButton2.UseGradientBackground = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(852, 450);
            Controls.Add(beepButton2);
            Controls.Add(beepButton1);
            Controls.Add(beepAppBar1);
            Controls.Add(beepProgressBar1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion
        private BeepSimpleGrid beepSimpleGrid1;
        private BeepDatePicker beepDatePicker1;
        private BeepProgressBar beepProgressBar1;
        private BeepAppBar beepAppBar1;
        private BeepButton beepButton1;
        private BeepButton beepButton2;
    }
}