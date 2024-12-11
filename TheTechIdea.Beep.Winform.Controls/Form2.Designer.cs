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
            beepListBox1 = new BeepListBox();
            SuspendLayout();
            // 
            // beepListBox1
            // 
            beepListBox1.ActiveBackColor = Color.Gray;
            beepListBox1.AnimationDuration = 500;
            beepListBox1.AnimationType = DisplayAnimationType.None;
            beepListBox1.ApplyThemeToChilds = true;
            beepListBox1.BackColor = Color.FromArgb(245, 245, 245);
            beepListBox1.BlockID = null;
            beepListBox1.BorderColor = Color.Black;
            beepListBox1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepListBox1.BorderRadius = 5;
            beepListBox1.BorderStyle = BorderStyle.FixedSingle;
            beepListBox1.BorderThickness = 1;
            beepListBox1.BottomoffsetForDrawingRect = 0;
            beepListBox1.BoundProperty = null;
            beepListBox1.DataContext = null;
            beepListBox1.DisabledBackColor = Color.Gray;
            beepListBox1.DisabledForeColor = Color.Empty;
            beepListBox1.DrawingRect = new Rectangle(4, 4, 563, 322);
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
            beepListBox1.GuidID = "98b1218a-cdeb-47f7-b32b-2375faddbff8";
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
            beepListBox1.LeftoffsetForDrawingRect = 0;
            beepListBox1.ListItems.Add((Common.SimpleItem)resources.GetObject("beepListBox1.ListItems"));
            beepListBox1.Location = new Point(122, 79);
            beepListBox1.MenuItemHeight = 20;
            beepListBox1.Name = "beepListBox1";
            beepListBox1.OverrideFontSize = TypeStyleFontSize.None;
            beepListBox1.ParentBackColor = Color.Empty;
            beepListBox1.PressedBackColor = Color.Gray;
            beepListBox1.PressedBorderColor = Color.Gray;
            beepListBox1.PressedForeColor = Color.Black;
            beepListBox1.RightoffsetForDrawingRect = 0;
            beepListBox1.SavedGuidID = null;
            beepListBox1.SavedID = null;
            beepListBox1.SelectedIndex = -1;
            beepListBox1.ShadowColor = Color.Black;
            beepListBox1.ShadowOffset = 3;
            beepListBox1.ShadowOpacity = 0.5F;
            beepListBox1.ShowAllBorders = true;
            beepListBox1.ShowBottomBorder = true;
            beepListBox1.ShowCheckBox = false;
            beepListBox1.ShowFocusIndicator = false;
            beepListBox1.ShowImage = false;
            beepListBox1.ShowLeftBorder = true;
            beepListBox1.ShowRightBorder = true;
            beepListBox1.ShowShadow = true;
            beepListBox1.ShowTitle = true;
            beepListBox1.ShowTitleLine = true;
            beepListBox1.ShowTitleLineinFullWidth = true;
            beepListBox1.ShowTopBorder = true;
            beepListBox1.Size = new Size(571, 330);
            beepListBox1.SlideFrom = SlideDirection.Left;
            beepListBox1.StaticNotMoving = false;
            beepListBox1.TabIndex = 1;
            beepListBox1.Text = "beepListBox1";
            beepListBox1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepListBox1.TitleAlignment = ContentAlignment.TopLeft;
            beepListBox1.TitleBottomY = 43;
            beepListBox1.TitleLineColor = Color.Gray;
            beepListBox1.TitleLineThickness = 2;
            beepListBox1.TitleText = "List Box";
            beepListBox1.ToolTipText = "";
            beepListBox1.TopoffsetForDrawingRect = 0;
            beepListBox1.UseGradientBackground = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(beepListBox1);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
        }

        #endregion
        private BeepListBox beepListBox1;
    }
}