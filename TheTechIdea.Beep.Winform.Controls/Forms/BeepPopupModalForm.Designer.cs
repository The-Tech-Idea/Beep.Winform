using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepPopupModalForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepPopupModalForm));
            beepPanel1 = new BeepPanel();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
         
            // 
            // beepPanel1
            // 
            
            beepPanel1.AnimationDuration = 500;
            beepPanel1.AnimationType = DisplayAnimationType.None;
            beepPanel1.ApplyThemeToChilds = false;
            beepPanel1.BackColor = Color.FromArgb(245, 245, 245);
            beepPanel1.BadgeBackColor = Color.Red;
            beepPanel1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepPanel1.BadgeForeColor = Color.White;
            beepPanel1.BadgeShape = BadgeShape.Circle;
            beepPanel1.BadgeText = "";
            beepPanel1.BlockID = null;
            beepPanel1.BorderColor = Color.Black;
            beepPanel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepPanel1.BorderRadius = 3;
            beepPanel1.BorderStyle = BorderStyle.FixedSingle;
            beepPanel1.BorderThickness = 1;
            beepPanel1.BottomoffsetForDrawingRect = 0;
            beepPanel1.BoundProperty = null;
            beepPanel1.CanBeFocused = true;
            beepPanel1.CanBeHovered = false;
            beepPanel1.CanBePressed = true;
            beepPanel1.Category = Utilities.DbFieldCategory.String;
            beepPanel1.ComponentName = "beepPanel1";
            beepPanel1.DataContext = null;
            beepPanel1.DataSourceProperty = null;
            beepPanel1.DisabledBackColor = Color.Gray;
            beepPanel1.DisabledForeColor = Color.Empty;
            beepPanel1.Dock = DockStyle.Fill;
            beepPanel1.DrawingRect = new Rectangle(0, 0, 794, 444);
            beepPanel1.Easing = EasingType.Linear;
            beepPanel1.FieldID = null;
            beepPanel1.FocusBackColor = Color.Gray;
            beepPanel1.FocusBorderColor = Color.Gray;
            beepPanel1.FocusForeColor = Color.Black;
            beepPanel1.FocusIndicatorColor = Color.Blue;
            beepPanel1.Font = new Font("Segoe UI", 14F);
            beepPanel1.ForeColor = Color.FromArgb(0, 0, 0);
            beepPanel1.Form = null;
            beepPanel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepPanel1.GradientEndColor = Color.Gray;
            beepPanel1.GradientStartColor = Color.Gray;
            beepPanel1.GuidID = "8d5b792c-b61b-47fc-970f-5e701986f26f";
            beepPanel1.HoverBackColor = Color.Gray;
            beepPanel1.HoverBorderColor = Color.Gray;
            beepPanel1.HoveredBackcolor = Color.Wheat;
            beepPanel1.HoverForeColor = Color.Black;
            beepPanel1.Id = -1;
          
            beepPanel1.Info = (SimpleItem)resources.GetObject("beepPanel1.Info");
            beepPanel1.IsAcceptButton = false;
            beepPanel1.IsBorderAffectedByTheme = true;
            beepPanel1.IsCancelButton = false;
            beepPanel1.IsChild = false;
            beepPanel1.IsCustomeBorder = false;
            beepPanel1.IsDefault = false;
            beepPanel1.IsDeleted = false;
            beepPanel1.IsDirty = false;
            beepPanel1.IsEditable = false;
            beepPanel1.IsFocused = false;
            beepPanel1.IsFrameless = false;
            beepPanel1.IsHovered = false;
            beepPanel1.IsNew = false;
            beepPanel1.IsPressed = false;
            beepPanel1.IsReadOnly = false;
            beepPanel1.IsRequired = false;
            beepPanel1.IsRounded = true;
            beepPanel1.IsRoundedAffectedByTheme = true;
            beepPanel1.IsSelected = false;
            beepPanel1.IsShadowAffectedByTheme = true;
            beepPanel1.IsVisible = false;
            beepPanel1.Items = (List<object>)resources.GetObject("beepPanel1.Items");
            beepPanel1.LeftoffsetForDrawingRect = 0;
            beepPanel1.LinkedProperty = null;
            beepPanel1.Location = new Point(3, 3);
            beepPanel1.Name = "beepPanel1";
            beepPanel1.OverrideFontSize = TypeStyleFontSize.None;
            beepPanel1.ParentBackColor = Color.Empty;
            beepPanel1.ParentControl = null;
            beepPanel1.PressedBackColor = Color.Gray;
            beepPanel1.PressedBorderColor = Color.Gray;
            beepPanel1.PressedForeColor = Color.Black;
            beepPanel1.RightoffsetForDrawingRect = 0;
            beepPanel1.SavedGuidID = null;
            beepPanel1.SavedID = null;
            beepPanel1.ShadowColor = Color.Black;
            beepPanel1.ShadowOffset = 0;
            beepPanel1.ShadowOpacity = 0.5F;
            beepPanel1.ShowAllBorders = false;
            beepPanel1.ShowBottomBorder = false;
            beepPanel1.ShowFocusIndicator = false;
            beepPanel1.ShowLeftBorder = false;
            beepPanel1.ShowRightBorder = false;
            beepPanel1.ShowShadow = false;
            beepPanel1.ShowTitle = false;
            beepPanel1.ShowTitleLine = false;
            beepPanel1.ShowTitleLineinFullWidth = true;
            beepPanel1.ShowTopBorder = false;
            beepPanel1.Size = new Size(794, 444);
            beepPanel1.SlideFrom = SlideDirection.Left;
            beepPanel1.StaticNotMoving = false;
            beepPanel1.TabIndex = 0;
        
            beepPanel1.Text = "beepPanel1";
            beepPanel1.TextFont = new Font("Segoe UI", 14F);
            beepPanel1.Theme = "DefaultType";
            beepPanel1.TitleAlignment = ContentAlignment.TopLeft;
            beepPanel1.TitleBottomY = 0;
            beepPanel1.TitleLineColor = Color.Gray;
            beepPanel1.TitleLineThickness = 2;
            beepPanel1.TitleText = "Panel Title";
            beepPanel1.ToolTipText = "";
            beepPanel1.TopoffsetForDrawingRect = 0;
            beepPanel1.UseGradientBackground = false;
            beepPanel1.UseThemeFont = true;
            // 
            // BeepPopupModalForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(800, 450);
            Controls.Add(beepPanel1);
            Name = "BeepPopupModalForm";
            Text = "BeepPopupModalForm";
            ResumeLayout(false);
        }

        #endregion

        private BeepPanel beepPanel1;
    }
}