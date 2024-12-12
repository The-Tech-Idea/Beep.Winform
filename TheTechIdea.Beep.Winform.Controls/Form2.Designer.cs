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
            SuspendLayout();
            // 
            // beepTree1
            // 
            beepTree1.ActiveBackColor = Color.Gray;
            beepTree1.AllowMultiSelect = false;
            beepTree1.AnimationDuration = 500;
            beepTree1.AnimationType = DisplayAnimationType.None;
            beepTree1.ApplyThemeToChilds = false;
            beepTree1.AutoScroll = true;
            beepTree1.BlockID = null;
            beepTree1.BorderColor = Color.Black;
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
            beepTree1.DrawingRect = new Rectangle(4, 4, 204, 348);
            beepTree1.Easing = EasingType.Linear;
            beepTree1.FieldID = null;
            beepTree1.FocusBackColor = Color.Gray;
            beepTree1.FocusBorderColor = Color.Gray;
            beepTree1.FocusForeColor = Color.Black;
            beepTree1.FocusIndicatorColor = Color.Blue;
            beepTree1.Form = null;
            beepTree1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepTree1.GradientEndColor = Color.Gray;
            beepTree1.GradientStartColor = Color.Gray;
            beepTree1.GuidID = "b5c1cb99-481d-4ba4-a94e-d0d862737f06";
            beepTree1.HoverBackColor = Color.Gray;
            beepTree1.HoverBorderColor = Color.Gray;
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
            beepTree1.ParentBackColor = Color.Empty;
            beepTree1.PressedBackColor = Color.Gray;
            beepTree1.PressedBorderColor = Color.Gray;
            beepTree1.PressedForeColor = Color.Black;
            beepTree1.RightoffsetForDrawingRect = 0;
            beepTree1.SavedGuidID = null;
            beepTree1.SavedID = null;
            beepTree1.ShadowColor = Color.Black;
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
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(beepTree1);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
        }

        #endregion

        private BeepTree beepTree1;
    }
}