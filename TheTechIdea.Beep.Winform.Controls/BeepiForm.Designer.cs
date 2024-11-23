namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepiForm
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
            components = new System.ComponentModel.Container();
            beepuiManager1 = new BeepUIManager(components);
            beepPanel1 = new BeepPanel();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.ApplyThemeOnImage = false;
            beepuiManager1.BeepAppBar = null;
            beepuiManager1.BeepFunctionsPanel = null;
            beepuiManager1.BeepiForm = null;
            beepuiManager1.BeepSideMenu = null;
            beepuiManager1.IsRounded = true;
            beepuiManager1.LogoImage = "";
            beepuiManager1.ShowBorder = true;
            beepuiManager1.ShowShadow = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.FlatDesignTheme;
            beepuiManager1.Title = "Beep Form";
            // 
            // beepPanel1
            // 
            beepPanel1.ActiveBackColor = Color.FromArgb(205, 133, 63);
            beepPanel1.AnimationDuration = 500;
            beepPanel1.AnimationType = DisplayAnimationType.None;
            beepPanel1.BackColor = Color.White;
            beepPanel1.BlockID = null;
            beepPanel1.BorderColor = Color.FromArgb(205, 133, 63);
            beepPanel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepPanel1.BorderRadius = 1;
            beepPanel1.BorderStyle = BorderStyle.None;
            beepPanel1.BorderThickness = 1;
            beepPanel1.DataContext = null;
            beepPanel1.DisabledBackColor = Color.Gray;
            beepPanel1.DisabledForeColor = Color.Empty;
            beepPanel1.Dock = DockStyle.Top;
            beepPanel1.DrawingRect = new Rectangle(1, 1, 772, 8);
            beepPanel1.Easing = EasingType.Linear;
            beepPanel1.FieldID = null;
            beepPanel1.FocusBackColor = Color.Transparent;
            beepPanel1.FocusBorderColor = Color.Transparent;
            beepPanel1.FocusForeColor = Color.Black;
            beepPanel1.FocusIndicatorColor = Color.Blue;
            beepPanel1.ForeColor = Color.FromArgb(44, 62, 80);
            beepPanel1.Form = null;
            beepPanel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepPanel1.GradientEndColor = Color.FromArgb(222, 184, 135);
            beepPanel1.GradientStartColor = Color.FromArgb(245, 245, 220);
            beepPanel1.HoverBackColor = Color.FromArgb(222, 184, 135);
            beepPanel1.HoverBorderColor = Color.Transparent;
            beepPanel1.HoveredBackcolor = Color.Transparent;
            beepPanel1.HoverForeColor = Color.Black;
            beepPanel1.Id = -1;
            beepPanel1.InactiveBackColor = Color.Transparent;
            beepPanel1.InactiveBorderColor = Color.Transparent;
            beepPanel1.InactiveForeColor = Color.Black;
            beepPanel1.IsAcceptButton = false;
            beepPanel1.IsBorderAffectedByTheme = false;
            beepPanel1.IsCancelButton = false;
            beepPanel1.IsChild = true;
            beepPanel1.IsCustomeBorder = false;
            beepPanel1.IsDefault = false;
            beepPanel1.IsFocused = false;
            beepPanel1.IsFramless = false;
            beepPanel1.IsHovered = false;
            beepPanel1.IsPressed = false;
            beepPanel1.IsRounded = true;
            beepPanel1.IsShadowAffectedByTheme = false;
            beepPanel1.Location = new Point(0, 0);
            beepPanel1.Name = "beepPanel1";
            beepPanel1.OverrideFontSize = TypeStyleFontSize.None;
            beepPanel1.ParentBackColor = Color.White;
            beepPanel1.PressedBackColor = Color.Transparent;
            beepPanel1.PressedBorderColor = Color.Transparent;
            beepPanel1.PressedForeColor = Color.Black;
            beepPanel1.SavedGuidID = null;
            beepPanel1.SavedID = null;
            beepPanel1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepPanel1.ShadowOffset = 0;
            beepPanel1.ShadowOpacity = 0.5F;
            beepPanel1.ShowAllBorders = true;
            beepPanel1.ShowBottomBorder = true;
            beepPanel1.ShowFocusIndicator = false;
            beepPanel1.ShowLeftBorder = true;
            beepPanel1.ShowRightBorder = true;
            beepPanel1.ShowShadow = false;
            beepPanel1.ShowTitle = false;
            beepPanel1.ShowTitleLine = false;
            beepPanel1.ShowTitleLineinFullWidth = true;
            beepPanel1.ShowTopBorder = true;
            beepPanel1.Size = new Size(774, 10);
            beepPanel1.SlideFrom = SlideDirection.Left;
            beepPanel1.StaticNotMoving = false;
            beepPanel1.TabIndex = 4;
            beepPanel1.Text = "beepPanel1";
            beepPanel1.Theme = Vis.Modules.EnumBeepThemes.FlatDesignTheme;
            beepPanel1.TitleAlignment = ContentAlignment.TopLeft;
            beepPanel1.TitleBottomY = 0;
            beepPanel1.TitleLineColor = Color.Gray;
            beepPanel1.TitleLineThickness = 2;
            beepPanel1.TitleText = "Panel Title";
            beepPanel1.ToolTipText = "";
            beepPanel1.UseGradientBackground = false;
            // 
            // BeepiForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(774, 644);
            Controls.Add(beepPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "BeepiForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Beep i Form";
            ResumeLayout(false);
        }

        #endregion
        public BeepUIManager beepuiManager1;
        private BeepPanel beepPanel1;
    }
}