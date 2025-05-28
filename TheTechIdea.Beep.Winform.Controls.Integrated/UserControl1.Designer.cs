using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Integrated
{
    partial class UserControl1
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControl1));
            beepDataBlock1 = new BeepDataBlock();
            SuspendLayout();
            // 
            // beepDataBlock1
            // 
          
            beepDataBlock1.AnimationDuration = 500;
            beepDataBlock1.AnimationType = DisplayAnimationType.None;
            beepDataBlock1.ApplyThemeToChilds = false;
            beepDataBlock1.BadgeBackColor = Color.Red;
            beepDataBlock1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepDataBlock1.BadgeForeColor = Color.White;
            beepDataBlock1.BadgeShape = BadgeShape.Circle;
            beepDataBlock1.BadgeText = "";
            beepDataBlock1.BlockID = null;
            beepDataBlock1.BlockMode = Modules.DataBlockMode.CRUD;
            beepDataBlock1.BorderColor = Color.Black;
            beepDataBlock1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepDataBlock1.BorderRadius = 3;
            beepDataBlock1.BorderStyle = BorderStyle.FixedSingle;
            beepDataBlock1.BorderThickness = 1;
            beepDataBlock1.BottomoffsetForDrawingRect = 0;
            beepDataBlock1.BoundProperty = null;
            beepDataBlock1.CanBeFocused = true;
            beepDataBlock1.CanBeHovered = false;
            beepDataBlock1.CanBePressed = true;
            beepDataBlock1.Category = Utilities.DbFieldCategory.String;
            beepDataBlock1.ChildBlocks = (List<Modules.IBeepDataBlock>)resources.GetObject("beepDataBlock1.ChildBlocks");
            beepDataBlock1.ComponentName = "beepDataBlock1";
            beepDataBlock1.DataContext = null;
            beepDataBlock1.DataSourceProperty = null;
            beepDataBlock1.DisabledBackColor = Color.Gray;
            beepDataBlock1.DisabledForeColor = Color.Empty;
            beepDataBlock1.DrawingRect = new Rectangle(1, 1, 447, 323);
            beepDataBlock1.Easing = EasingType.Linear;
            beepDataBlock1.EntityName = null;
            beepDataBlock1.FieldID = null;
            beepDataBlock1.FocusBackColor = Color.Gray;
            beepDataBlock1.FocusBorderColor = Color.Gray;
            beepDataBlock1.FocusForeColor = Color.Black;
            beepDataBlock1.FocusIndicatorColor = Color.Blue;
            beepDataBlock1.ForeignKeyPropertyName = null;
            beepDataBlock1.Form = null;
            beepDataBlock1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepDataBlock1.GradientEndColor = Color.Gray;
            beepDataBlock1.GradientStartColor = Color.Gray;
            beepDataBlock1.GuidID = "e81ecf4b-aa21-43b9-9455-af230c261f2b";
            beepDataBlock1.HoverBackColor = Color.Gray;
            beepDataBlock1.HoverBorderColor = Color.Gray;
            beepDataBlock1.HoveredBackcolor = Color.Wheat;
            beepDataBlock1.HoverForeColor = Color.Black;
            beepDataBlock1.Id = -1;
        
            beepDataBlock1.Info = (Models.SimpleItem)resources.GetObject("beepDataBlock1.Info");
            beepDataBlock1.IsAcceptButton = false;
            beepDataBlock1.IsBorderAffectedByTheme = false;
            beepDataBlock1.IsCancelButton = false;
            beepDataBlock1.IsChild = false;
            beepDataBlock1.IsCustomeBorder = false;
            beepDataBlock1.IsDefault = false;
            beepDataBlock1.IsDeleted = false;
            beepDataBlock1.IsDirty = false;
            beepDataBlock1.IsEditable = false;
            beepDataBlock1.IsFocused = false;
            beepDataBlock1.IsFrameless = false;
            beepDataBlock1.IsHovered = false;
            beepDataBlock1.IsNew = false;
            beepDataBlock1.IsPressed = false;
            beepDataBlock1.IsReadOnly = false;
            beepDataBlock1.IsRequired = false;
            beepDataBlock1.IsRounded = true;
            beepDataBlock1.IsRoundedAffectedByTheme = false;
            beepDataBlock1.IsSelected = false;
            beepDataBlock1.IsShadowAffectedByTheme = false;
            beepDataBlock1.IsVisible = false;
            beepDataBlock1.Items = (List<object>)resources.GetObject("beepDataBlock1.Items");
            beepDataBlock1.LeftoffsetForDrawingRect = 0;
            beepDataBlock1.LinkedProperty = null;
            beepDataBlock1.Location = new Point(50, 42);
            beepDataBlock1.MasterKeyPropertyName = null;
            beepDataBlock1.Name = "beepDataBlock1";
            beepDataBlock1.OverrideFontSize = TypeStyleFontSize.None;
            beepDataBlock1.ParentBackColor = Color.Empty;
            beepDataBlock1.ParentBlock = null;
            beepDataBlock1.ParentControl = null;
            beepDataBlock1.PressedBackColor = Color.Gray;
            beepDataBlock1.PressedBorderColor = Color.Gray;
            beepDataBlock1.PressedForeColor = Color.Black;
            beepDataBlock1.RightoffsetForDrawingRect = 0;
            beepDataBlock1.SavedGuidID = null;
            beepDataBlock1.SavedID = null;
            beepDataBlock1.SelectedEntityType = null;
            beepDataBlock1.ShadowColor = Color.Black;
            beepDataBlock1.ShadowOffset = 0;
            beepDataBlock1.ShadowOpacity = 0.5F;
            beepDataBlock1.ShowAllBorders = true;
            beepDataBlock1.ShowBottomBorder = true;
            beepDataBlock1.ShowFocusIndicator = false;
            beepDataBlock1.ShowLeftBorder = true;
            beepDataBlock1.ShowRightBorder = true;
            beepDataBlock1.ShowShadow = false;
            beepDataBlock1.ShowTopBorder = true;
            beepDataBlock1.Size = new Size(449, 325);
            beepDataBlock1.SlideFrom = SlideDirection.Left;
            beepDataBlock1.StaticNotMoving = false;
            beepDataBlock1.TabIndex = 0;
            beepDataBlock1.TempBackColor = Color.Empty;
            beepDataBlock1.Text = "beepDataBlock1";
            beepDataBlock1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepDataBlock1.ToolTipText = "";
            beepDataBlock1.TopoffsetForDrawingRect = 0;
            beepDataBlock1.UIComponents = (Dictionary<string, Vis.Modules.IBeepUIComponent>)resources.GetObject("beepDataBlock1.UIComponents");
            beepDataBlock1.UseGradientBackground = false;
            beepDataBlock1.UseThemeFont = true;
            // 
            // UserControl1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(beepDataBlock1);
            Name = "UserControl1";
            Size = new Size(592, 501);
            ResumeLayout(false);
        }

        #endregion

        private BeepDataBlock beepDataBlock1;
    }
}
