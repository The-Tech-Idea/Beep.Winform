using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepPopupListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepPopupListForm));
            _beepListBox = new BeepListBox();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepiForm = this;
            beepuiManager1.Theme = "DefaultTheme";
            // 
            // _beepListBox
            // 
        
            _beepListBox.AnimationDuration = 500;
            _beepListBox.AnimationType = DisplayAnimationType.None;
            _beepListBox.ApplyThemeToChilds = true;
            _beepListBox.BackColor = Color.FromArgb(240, 240, 240);
            _beepListBox.BadgeBackColor = Color.Red;
            _beepListBox.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            _beepListBox.BadgeForeColor = Color.White;
            _beepListBox.BadgeShape = BadgeShape.Circle;
            _beepListBox.BadgeText = "";
            _beepListBox.BlockID = null;
            _beepListBox.BorderColor = Color.Black;
            _beepListBox.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _beepListBox.BorderRadius = 1;
            _beepListBox.BorderStyle = BorderStyle.FixedSingle;
            _beepListBox.BorderThickness = 1;
            _beepListBox.BottomoffsetForDrawingRect = 0;
            _beepListBox.BoundProperty = "SelectedMenuItem";
            _beepListBox.CanBeFocused = true;
            _beepListBox.CanBeHovered = false;
            _beepListBox.CanBePressed = true;
            _beepListBox.Category = Utilities.DbFieldCategory.String;
            _beepListBox.Collapsed = false;
            _beepListBox.ComponentName = "beepListBox1";
            _beepListBox.DataContext = null;
            _beepListBox.DataSourceProperty = null;
            _beepListBox.DisabledBackColor = Color.Gray;
            _beepListBox.DisabledForeColor = Color.Empty;
            _beepListBox.Dock = DockStyle.Fill;
            _beepListBox.DrawingRect = new Rectangle(1, 1, 629, 763);
            _beepListBox.Easing = EasingType.Linear;
            _beepListBox.FieldID = null;
            _beepListBox.FocusBackColor = Color.Gray;
            _beepListBox.FocusBorderColor = Color.Gray;
            _beepListBox.FocusForeColor = Color.Black;
            _beepListBox.FocusIndicatorColor = Color.Blue;
            _beepListBox.Font = new Font("Segoe UI", 14F);
            _beepListBox.ForeColor = Color.FromArgb(0, 0, 0);
            _beepListBox.Form = null;
            _beepListBox.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            _beepListBox.GradientEndColor = Color.Gray;
            _beepListBox.GradientStartColor = Color.Gray;
            _beepListBox.GuidID = "da073259-cfc0-42b8-bdb4-de295de2007b";
            _beepListBox.HoverBackColor = Color.Gray;
            _beepListBox.HoverBorderColor = Color.Gray;
            _beepListBox.HoveredBackcolor = Color.Wheat;
            _beepListBox.HoverForeColor = Color.Black;
            _beepListBox.Id = -1;
            _beepListBox.ImageSize = 18;
            _beepListBox.Info = (SimpleItem)resources.GetObject("_beepListBox.Info");
            _beepListBox.IsAcceptButton = false;
            _beepListBox.IsBorderAffectedByTheme = true;
            _beepListBox.IsCancelButton = false;
            _beepListBox.IsChild = false;
            _beepListBox.IsCustomeBorder = false;
            _beepListBox.IsDefault = false;
            _beepListBox.IsDeleted = false;
            _beepListBox.IsDirty = false;
            _beepListBox.IsEditable = false;
            _beepListBox.IsFocused = false;
            _beepListBox.IsFrameless = false;
            _beepListBox.IsHovered = false;
            _beepListBox.IsItemChilds = true;
            _beepListBox.IsNew = false;
            _beepListBox.IsPressed = false;
            _beepListBox.IsReadOnly = false;
            _beepListBox.IsRequired = false;
            _beepListBox.IsRounded = true;
            _beepListBox.IsRoundedAffectedByTheme = true;
            _beepListBox.IsSelected = false;
            _beepListBox.IsShadowAffectedByTheme = true;
            _beepListBox.IsVisible = false;
            _beepListBox.Items = (List<object>)resources.GetObject("_beepListBox.Items");
            _beepListBox.LeftoffsetForDrawingRect = 0;
            _beepListBox.LinkedProperty = null;
            _beepListBox.Location = new Point(3, 3);
            _beepListBox.MenuItemHeight = 20;
            _beepListBox.Name = "_beepListBox";
            _beepListBox.OverrideFontSize = TypeStyleFontSize.None;
            _beepListBox.ParentBackColor = Color.Empty;
            _beepListBox.ParentControl = null;
            _beepListBox.PressedBackColor = Color.Gray;
            _beepListBox.PressedBorderColor = Color.Gray;
            _beepListBox.PressedForeColor = Color.Black;
            _beepListBox.RightoffsetForDrawingRect = 0;
            _beepListBox.SavedGuidID = null;
            _beepListBox.SavedID = null;
            _beepListBox.SelectedIndex = -1;
            _beepListBox.SelectedItem = null;
            _beepListBox.ShadowColor = Color.Black;
            _beepListBox.ShadowOffset = 0;
            _beepListBox.ShadowOpacity = 0.5F;
            _beepListBox.ShowAllBorders = true;
            _beepListBox.ShowBottomBorder = true;
            _beepListBox.ShowCheckBox = false;
            _beepListBox.ShowFocusIndicator = false;
            _beepListBox.ShowHilightBox = true;
            _beepListBox.ShowImage = false;
            _beepListBox.ShowLeftBorder = true;
            _beepListBox.ShowRightBorder = true;
            _beepListBox.ShowShadow = false;
            _beepListBox.ShowTitle = false;
            _beepListBox.ShowTitleLine = false;
            _beepListBox.ShowTitleLineinFullWidth = true;
            _beepListBox.ShowTopBorder = true;
            _beepListBox.Size = new Size(631, 765);
            _beepListBox.SlideFrom = SlideDirection.Left;
            _beepListBox.StaticNotMoving = false;
            _beepListBox.TabIndex = 0;
            _beepListBox.TempBackColor = Color.Empty;
            _beepListBox.TextFont = new Font("Segoe UI", 14F);
            _beepListBox.Theme = "DefaultTheme";
            _beepListBox.TitleAlignment = ContentAlignment.TopLeft;
            _beepListBox.TitleBottomY = 0;
            _beepListBox.TitleLineColor = Color.Gray;
            _beepListBox.TitleLineThickness = 2;
            _beepListBox.TitleText = "List Box";
            _beepListBox.ToolTipText = "";
            _beepListBox.TopoffsetForDrawingRect = 0;
            _beepListBox.UseGradientBackground = false;
            _beepListBox.UseThemeFont = true;
            // 
            // BeepPopupListForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(637, 771);
            Controls.Add(_beepListBox);
            Name = "BeepPopupListForm";
            Text = "BeepPopupListForm";
            ResumeLayout(false);
        }

        #endregion

        private BeepListBox _beepListBox;
    }
}