using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepDialogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepDialogForm));
            beepDialogBox1 = new BeepDialogBox();
            SuspendLayout();
            // 
          
            // 
            // beepDialogBox1
            // 
          
            beepDialogBox1.AnimationDuration = 500;
            beepDialogBox1.AnimationType = DisplayAnimationType.None;
            beepDialogBox1.ApplyThemeToChilds = true;
            beepDialogBox1.BackColor = Color.FromArgb(245, 245, 245);
            beepDialogBox1.BadgeBackColor = Color.Red;
            beepDialogBox1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepDialogBox1.BadgeForeColor = Color.White;
            beepDialogBox1.BadgeText = "";
            beepDialogBox1.BlockID = null;
            beepDialogBox1.BorderColor = Color.Black;
            beepDialogBox1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepDialogBox1.BorderRadius = 3;
            beepDialogBox1.BorderStyle = BorderStyle.FixedSingle;
            beepDialogBox1.BorderThickness = 1;
            beepDialogBox1.BottomoffsetForDrawingRect = 0;
            beepDialogBox1.BoundProperty = null;
            beepDialogBox1.CanBeFocused = true;
            beepDialogBox1.CanBeHovered = false;
            beepDialogBox1.CanBePressed = true;
            beepDialogBox1.Category = Utilities.DbFieldCategory.String;
            beepDialogBox1.ComponentName = "beepDialogBox1";
            beepDialogBox1.DataContext = null;
            beepDialogBox1.DataSourceProperty = null;
            beepDialogBox1.DialogResult = Vis.Modules.BeepDialogResult.None;
            beepDialogBox1.DisabledBackColor = Color.Gray;
            beepDialogBox1.DisabledForeColor = Color.Empty;
            beepDialogBox1.Dock = DockStyle.Fill;
            beepDialogBox1.DrawingRect = new Rectangle(1, 1, 511, 332);
            beepDialogBox1.Easing = EasingType.Linear;
            beepDialogBox1.FieldID = null;
            beepDialogBox1.FocusBackColor = Color.Gray;
            beepDialogBox1.FocusBorderColor = Color.Gray;
            beepDialogBox1.FocusForeColor = Color.Black;
            beepDialogBox1.FocusIndicatorColor = Color.Blue;
            beepDialogBox1.Font = new Font("Segoe UI", 14F);
            beepDialogBox1.ForeColor = Color.FromArgb(0, 0, 0);
            beepDialogBox1.Form = null;
            beepDialogBox1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepDialogBox1.GradientEndColor = Color.Gray;
            beepDialogBox1.GradientStartColor = Color.Gray;
            beepDialogBox1.GuidID = "aba9226c-760d-4b3b-ba5b-b65b55e5e30a";
            beepDialogBox1.HoverBackColor = Color.Gray;
            beepDialogBox1.HoverBorderColor = Color.Gray;
            beepDialogBox1.HoveredBackcolor = Color.Wheat;
            beepDialogBox1.HoverForeColor = Color.Black;
            beepDialogBox1.Id = -1;
     
            beepDialogBox1.InactiveBorderColor = Color.Gray;

            beepDialogBox1.IsAcceptButton = false;
            beepDialogBox1.IsBorderAffectedByTheme = true;
            beepDialogBox1.IsCancelButton = false;
            beepDialogBox1.IsChild = false;
            beepDialogBox1.IsCustomeBorder = false;
            beepDialogBox1.IsDefault = false;
            beepDialogBox1.IsDeleted = false;
            beepDialogBox1.IsDirty = false;
            beepDialogBox1.IsEditable = false;
            beepDialogBox1.IsFocused = false;
            beepDialogBox1.IsFrameless = false;
            beepDialogBox1.IsHovered = false;
            beepDialogBox1.IsNew = false;
            beepDialogBox1.IsPressed = false;
            beepDialogBox1.IsReadOnly = false;
            beepDialogBox1.IsRequired = false;
            beepDialogBox1.IsRounded = true;
            beepDialogBox1.IsRoundedAffectedByTheme = true;
            beepDialogBox1.IsSelected = false;
            beepDialogBox1.IsShadowAffectedByTheme = true;
            beepDialogBox1.IsVisible = false;
            beepDialogBox1.LeftoffsetForDrawingRect = 0;
            beepDialogBox1.LinkedProperty = null;
            beepDialogBox1.Location = new Point(4, 4);
            beepDialogBox1.Name = "beepDialogBox1";
            beepDialogBox1.OverrideFontSize = TypeStyleFontSize.None;
            beepDialogBox1.ParentBackColor = Color.Empty;
            beepDialogBox1.ParentControl = null;
            beepDialogBox1.PressedBackColor = Color.Gray;
            beepDialogBox1.PressedBorderColor = Color.Gray;
            beepDialogBox1.PressedForeColor = Color.Black;
            beepDialogBox1.PrimaryButtonColor = Color.FromArgb(240, 240, 240);
            beepDialogBox1.PrimaryButtonText = "OK";
            beepDialogBox1.RightoffsetForDrawingRect = 0;
            beepDialogBox1.SavedGuidID = null;
            beepDialogBox1.SavedID = null;
            beepDialogBox1.SecondaryButtonColor = Color.FromArgb(240, 240, 240);
            beepDialogBox1.SecondaryButtonText = "Cancel";
            beepDialogBox1.ShadowColor = Color.Black;
            beepDialogBox1.ShadowOffset = 0;
            beepDialogBox1.ShadowOpacity = 0.5F;
            beepDialogBox1.ShowAllBorders = true;
            beepDialogBox1.ShowBottomBorder = true;
            beepDialogBox1.ShowFocusIndicator = false;
            beepDialogBox1.ShowLeftBorder = true;
            beepDialogBox1.ShowRightBorder = true;
            beepDialogBox1.ShowShadow = false;
            beepDialogBox1.ShowTitle = true;
            beepDialogBox1.ShowTitleLine = true;
            beepDialogBox1.ShowTitleLineinFullWidth = true;
            beepDialogBox1.ShowTopBorder = true;
            beepDialogBox1.Size = new Size(513, 334);
            beepDialogBox1.SlideFrom = SlideDirection.Left;
            beepDialogBox1.StaticNotMoving = false;
            beepDialogBox1.TabIndex = 0;
          
            beepDialogBox1.Text = "beepDialogBox1";
            beepDialogBox1.TextFont = new Font("Segoe UI", 14F);
           
            beepDialogBox1.TitleAlignment = ContentAlignment.TopLeft;
            beepDialogBox1.TitleBottomY = 36;
            beepDialogBox1.TitleLineColor = Color.Gray;
            beepDialogBox1.TitleLineThickness = 2;
            beepDialogBox1.TitleText = "Dialog Title";
            beepDialogBox1.ToolTipText = "";
            beepDialogBox1.TopoffsetForDrawingRect = 0;
            beepDialogBox1.UseGradientBackground = false;
            beepDialogBox1.UseThemeFont = true;
            // 
            // BeepDialogForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(521, 342);
            Controls.Add(beepDialogBox1);
            Name = "BeepDialogForm";
            Text = "BeepDialogForm";
            ResumeLayout(false);
        }

        #endregion

        private BeepDialogBox beepDialogBox1;
    }
}