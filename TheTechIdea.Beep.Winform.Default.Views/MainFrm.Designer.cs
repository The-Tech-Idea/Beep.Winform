using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    partial class MainFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            beepDisplayContainer1 = new TheTechIdea.Beep.Winform.Controls.BeepDisplayContainer();
            beepMenuAppBar1 = new TheTechIdea.Beep.Winform.Controls.MenuBar.BeepMenuAppBar();
            beepAppTree1 = new TheTechIdea.Beep.Winform.Controls.ITrees.BeepTreeView.BeepAppTree();
            beepAppBar1 = new TheTechIdea.Beep.Winform.Controls.BeepAppBar();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepAppBar = beepAppBar1;
            beepuiManager1.BeepiForm = this;
            beepuiManager1.BeepMenuBar = beepMenuAppBar1;
            beepuiManager1.IsRounded = false;
            beepuiManager1.ShowBorder = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepuiManager1.Title = "Beep Development and Data Platform";
            // 
            // beepDisplayContainer1
            // 
           
            beepDisplayContainer1.AnimationDuration = 500;
            beepDisplayContainer1.AnimationType = DisplayAnimationType.None;
            beepDisplayContainer1.ApplyThemeToChilds = false;
            beepDisplayContainer1.BackColor = Color.FromArgb(245, 245, 245);
            beepDisplayContainer1.BadgeBackColor = Color.Red;
            beepDisplayContainer1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepDisplayContainer1.BadgeForeColor = Color.White;
            beepDisplayContainer1.BadgeShape = BadgeShape.Circle;
            beepDisplayContainer1.BadgeText = "";
            beepDisplayContainer1.BlockID = null;
            beepDisplayContainer1.BorderColor = Color.FromArgb(200, 200, 200);
            beepDisplayContainer1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepDisplayContainer1.BorderRadius = 3;
            beepDisplayContainer1.BorderStyle = BorderStyle.FixedSingle;
            beepDisplayContainer1.BorderThickness = 1;
            beepDisplayContainer1.BottomoffsetForDrawingRect = 0;
            beepDisplayContainer1.BoundProperty = null;
            beepDisplayContainer1.CanBeFocused = true;
            beepDisplayContainer1.CanBeHovered = false;
            beepDisplayContainer1.CanBePressed = true;
            beepDisplayContainer1.Category = Utilities.DbFieldCategory.String;
            beepDisplayContainer1.ComponentName = "BeepDisplayContainer";
            beepDisplayContainer1.ContainerType = Vis.Modules.ContainerTypeEnum.TabbedPanel;
            beepDisplayContainer1.DataContext = null;
            beepDisplayContainer1.DataSourceProperty = null;
            beepDisplayContainer1.DisabledBackColor = Color.Gray;
            beepDisplayContainer1.DisabledForeColor = Color.Empty;
            beepDisplayContainer1.Dock = DockStyle.Fill;
            beepDisplayContainer1.DrawingRect = new Rectangle(2, 2, 990, 715);
            beepDisplayContainer1.Easing = EasingType.Linear;
            beepDisplayContainer1.FieldID = null;
            beepDisplayContainer1.FocusBackColor = Color.White;
            beepDisplayContainer1.FocusBorderColor = Color.Gray;
            beepDisplayContainer1.FocusForeColor = Color.Black;
            beepDisplayContainer1.FocusIndicatorColor = Color.Blue;
            beepDisplayContainer1.Form = null;
            beepDisplayContainer1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepDisplayContainer1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepDisplayContainer1.GradientStartColor = Color.White;
            beepDisplayContainer1.GuidID = "b3ee9f08-fabd-4339-b553-35f73d487bb5";
            beepDisplayContainer1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepDisplayContainer1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepDisplayContainer1.HoveredBackcolor = Color.Wheat;
            beepDisplayContainer1.HoverForeColor = Color.Black;
            beepDisplayContainer1.Id = -1;
            
            beepDisplayContainer1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepDisplayContainer1.Info");
            beepDisplayContainer1.IsAcceptButton = false;
            beepDisplayContainer1.IsBorderAffectedByTheme = true;
            beepDisplayContainer1.IsCancelButton = false;
            beepDisplayContainer1.IsChild = false;
            beepDisplayContainer1.IsCustomeBorder = false;
            beepDisplayContainer1.IsDefault = false;
            beepDisplayContainer1.IsDeleted = false;
            beepDisplayContainer1.IsDirty = false;
            beepDisplayContainer1.IsEditable = false;
            beepDisplayContainer1.IsFocused = false;
            beepDisplayContainer1.IsFrameless = true;
            beepDisplayContainer1.IsHovered = false;
            beepDisplayContainer1.IsNew = false;
            beepDisplayContainer1.IsPressed = false;
            beepDisplayContainer1.IsReadOnly = false;
            beepDisplayContainer1.IsRequired = false;
            beepDisplayContainer1.IsRounded = false;
            beepDisplayContainer1.IsRoundedAffectedByTheme = false;
            beepDisplayContainer1.IsSelected = false;
            beepDisplayContainer1.IsShadowAffectedByTheme = false;
            beepDisplayContainer1.IsVisible = false;
            beepDisplayContainer1.Items = (List<object>)resources.GetObject("beepDisplayContainer1.Items");
            beepDisplayContainer1.LeftoffsetForDrawingRect = 0;
            beepDisplayContainer1.LinkedProperty = null;
            beepDisplayContainer1.Location = new Point(203, 78);
            beepDisplayContainer1.Name = "beepDisplayContainer1";
            beepDisplayContainer1.OverrideFontSize = TypeStyleFontSize.None;
            beepDisplayContainer1.Padding = new Padding(2);
            beepDisplayContainer1.ParentBackColor = Color.Empty;
            beepDisplayContainer1.ParentControl = null;
            beepDisplayContainer1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepDisplayContainer1.PressedBorderColor = Color.Gray;
            beepDisplayContainer1.PressedForeColor = Color.Black;
            beepDisplayContainer1.RightoffsetForDrawingRect = 0;
            beepDisplayContainer1.SavedGuidID = null;
            beepDisplayContainer1.SavedID = null;
            beepDisplayContainer1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepDisplayContainer1.ShadowOffset = 0;
            beepDisplayContainer1.ShadowOpacity = 0.5F;
            beepDisplayContainer1.ShowAllBorders = false;
            beepDisplayContainer1.ShowBottomBorder = false;
            beepDisplayContainer1.ShowFocusIndicator = false;
            beepDisplayContainer1.ShowLeftBorder = false;
            beepDisplayContainer1.ShowRightBorder = false;
            beepDisplayContainer1.ShowShadow = false;
            beepDisplayContainer1.ShowTopBorder = false;
            beepDisplayContainer1.Size = new Size(994, 719);
            beepDisplayContainer1.SlideFrom = SlideDirection.Left;
            beepDisplayContainer1.StaticNotMoving = false;
            beepDisplayContainer1.TabIndex = 2;
            beepDisplayContainer1.TempBackColor = Color.Empty;
            beepDisplayContainer1.Text = "beepDisplayContainer1";
            beepDisplayContainer1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepDisplayContainer1.ToolTipText = "";
            beepDisplayContainer1.TopoffsetForDrawingRect = 0;
            beepDisplayContainer1.UseGradientBackground = false;
            beepDisplayContainer1.UseThemeFont = true;
            // 
            // beepMenuAppBar1
            // 
         
            beepMenuAppBar1.ActiveMenuButton = null;
            beepMenuAppBar1.AnimationDuration = 500;
            beepMenuAppBar1.AnimationType = DisplayAnimationType.None;
            beepMenuAppBar1.ApplyThemeToChilds = false;
            beepMenuAppBar1.BackColor = Color.FromArgb(240, 240, 240);
            beepMenuAppBar1.BadgeBackColor = Color.Red;
            beepMenuAppBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepMenuAppBar1.BadgeForeColor = Color.White;
            beepMenuAppBar1.BadgeShape = BadgeShape.Circle;
            beepMenuAppBar1.BadgeText = "";
            beepMenuAppBar1.beepServices = null;
            beepMenuAppBar1.BlockID = null;
            beepMenuAppBar1.BorderColor = Color.FromArgb(200, 200, 200);
            beepMenuAppBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepMenuAppBar1.BorderRadius = 3;
            beepMenuAppBar1.BorderStyle = BorderStyle.FixedSingle;
            beepMenuAppBar1.BorderThickness = 1;
            beepMenuAppBar1.BottomoffsetForDrawingRect = 0;
            beepMenuAppBar1.BoundProperty = "SelectedMenuItem";
            beepMenuAppBar1.CanBeFocused = true;
            beepMenuAppBar1.CanBeHovered = false;
            beepMenuAppBar1.CanBePressed = true;
            beepMenuAppBar1.Category = Utilities.DbFieldCategory.String;
            beepMenuAppBar1.ComponentName = "beepMenuAppBar1";
            beepMenuAppBar1.DataContext = null;
            beepMenuAppBar1.DataSourceProperty = null;
            beepMenuAppBar1.DisabledBackColor = Color.Gray;
            beepMenuAppBar1.DisabledForeColor = Color.Empty;
            beepMenuAppBar1.Dock = DockStyle.Top;
            beepMenuAppBar1.DrawingRect = new Rectangle(0, 0, 994, 35);
            beepMenuAppBar1.Easing = EasingType.Linear;
            beepMenuAppBar1.FieldID = null;
            beepMenuAppBar1.FocusBackColor = Color.White;
            beepMenuAppBar1.FocusBorderColor = Color.Gray;
            beepMenuAppBar1.FocusForeColor = Color.Black;
            beepMenuAppBar1.FocusIndicatorColor = Color.Blue;
            beepMenuAppBar1.Font = new Font("Segoe UI", 9F);
            beepMenuAppBar1.ForeColor = Color.FromArgb(240, 240, 240);
            beepMenuAppBar1.Form = null;
            beepMenuAppBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepMenuAppBar1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepMenuAppBar1.GradientStartColor = Color.White;
            beepMenuAppBar1.GuidID = "6f369a04-3d65-47eb-9d84-d364141bc239";
            beepMenuAppBar1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepMenuAppBar1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepMenuAppBar1.HoveredBackcolor = Color.Wheat;
            beepMenuAppBar1.HoverForeColor = Color.Black;
            beepMenuAppBar1.Id = -1;
            beepMenuAppBar1.ImageSize = 20;
          
            beepMenuAppBar1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepMenuAppBar1.Info");
            beepMenuAppBar1.IsAcceptButton = false;
            beepMenuAppBar1.IsBorderAffectedByTheme = true;
            beepMenuAppBar1.IsCancelButton = false;
            beepMenuAppBar1.IsChild = false;
            beepMenuAppBar1.IsCustomeBorder = false;
            beepMenuAppBar1.IsDefault = false;
            beepMenuAppBar1.IsDeleted = false;
            beepMenuAppBar1.IsDirty = false;
            beepMenuAppBar1.IsEditable = false;
            beepMenuAppBar1.IsFocused = false;
            beepMenuAppBar1.IsFrameless = true;
            beepMenuAppBar1.IsHovered = false;
            beepMenuAppBar1.IsNew = false;
            beepMenuAppBar1.IsPressed = false;
            beepMenuAppBar1.IsReadOnly = false;
            beepMenuAppBar1.IsRequired = false;
            beepMenuAppBar1.IsRounded = false;
            beepMenuAppBar1.IsRoundedAffectedByTheme = false;
            beepMenuAppBar1.IsSelected = false;
            beepMenuAppBar1.IsShadowAffectedByTheme = false;
            beepMenuAppBar1.IsVisible = false;
            beepMenuAppBar1.Items = (List<object>)resources.GetObject("beepMenuAppBar1.Items");
            beepMenuAppBar1.LeftoffsetForDrawingRect = 0;
            beepMenuAppBar1.LinkedProperty = null;
            beepMenuAppBar1.Location = new Point(203, 43);
            beepMenuAppBar1.MenuItemHeight = 35;
            beepMenuAppBar1.MenuItemWidth = 60;
            beepMenuAppBar1.Name = "beepMenuAppBar1";
            beepMenuAppBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepMenuAppBar1.ParentBackColor = Color.Empty;
            beepMenuAppBar1.ParentControl = null;
            beepMenuAppBar1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepMenuAppBar1.PressedBorderColor = Color.Gray;
            beepMenuAppBar1.PressedForeColor = Color.Black;
            beepMenuAppBar1.RightoffsetForDrawingRect = 0;
            beepMenuAppBar1.SavedGuidID = null;
            beepMenuAppBar1.SavedID = null;
            beepMenuAppBar1.SelectedIndex = -1;
            beepMenuAppBar1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepMenuAppBar1.ShadowOffset = 0;
            beepMenuAppBar1.ShadowOpacity = 0.5F;
            beepMenuAppBar1.ShowAllBorders = false;
            beepMenuAppBar1.ShowBottomBorder = false;
            beepMenuAppBar1.ShowFocusIndicator = false;
            beepMenuAppBar1.ShowLeftBorder = false;
            beepMenuAppBar1.ShowRightBorder = false;
            beepMenuAppBar1.ShowShadow = false;
            beepMenuAppBar1.ShowTopBorder = false;
            beepMenuAppBar1.Size = new Size(994, 35);
            beepMenuAppBar1.SlideFrom = SlideDirection.Left;
            beepMenuAppBar1.StaticNotMoving = false;
            beepMenuAppBar1.TabIndex = 3;
            beepMenuAppBar1.TempBackColor = Color.Empty;
            beepMenuAppBar1.Text = "beepMenuAppBar1";
            beepMenuAppBar1.TextFont = new Font("Segoe UI", 9F);
            beepMenuAppBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepMenuAppBar1.ToolTipText = "";
            beepMenuAppBar1.TopoffsetForDrawingRect = 0;
            beepMenuAppBar1.UseGradientBackground = false;
            beepMenuAppBar1.UseThemeFont = true;
            // 
            // beepAppTree1
            // 
          
            beepAppTree1.AllowMultiSelect = false;
            beepAppTree1.AnimationDuration = 500;
            beepAppTree1.AnimationType = DisplayAnimationType.None;
            beepAppTree1.ApplyThemeToChilds = false;
            beepAppTree1.args = null;
            beepAppTree1.AutoScroll = true;
            beepAppTree1.BackColor = Color.FromArgb(240, 240, 240);
            beepAppTree1.BadgeBackColor = Color.Red;
            beepAppTree1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepAppTree1.BadgeForeColor = Color.White;
            beepAppTree1.BadgeShape = BadgeShape.Circle;
            beepAppTree1.BadgeText = "";
            beepAppTree1.BeepService = null;
            beepAppTree1.BlockID = null;
            beepAppTree1.BorderColor = Color.FromArgb(200, 200, 200);
            beepAppTree1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepAppTree1.BorderRadius = 3;
            beepAppTree1.BorderStyle = BorderStyle.FixedSingle;
            beepAppTree1.BorderThickness = 1;
            beepAppTree1.BottomoffsetForDrawingRect = 0;
            beepAppTree1.BoundProperty = null;
            beepAppTree1.Branches = (List<Vis.Modules.IBranch>)resources.GetObject("beepAppTree1.Branches");
            beepAppTree1.CanBeFocused = true;
            beepAppTree1.CanBeHovered = false;
            beepAppTree1.CanBePressed = true;
            beepAppTree1.Category = Utilities.DbFieldCategory.String;
            beepAppTree1.CategoryIcon = "Category.svg";
            beepAppTree1.ClickedNode = null;
            beepAppTree1.ComponentName = "beepAppTree1";
            beepAppTree1.CurrentBranch = null;
            beepAppTree1.CurrentMenutems = null;
            beepAppTree1.DataContext = null;
            beepAppTree1.DataSourceProperty = null;
            beepAppTree1.DisabledBackColor = Color.Gray;
            beepAppTree1.DisabledForeColor = Color.Empty;
            beepAppTree1.DMEEditor = null;
            beepAppTree1.Dock = DockStyle.Left;
            beepAppTree1.DrawingRect = new Rectangle(1, 1, 198, 752);
            beepAppTree1.DropHandler = null;
            beepAppTree1.Easing = EasingType.Linear;
            beepAppTree1.ExtensionsHelpers = null;
            beepAppTree1.FieldID = null;
            beepAppTree1.Filterstring = null;
            beepAppTree1.FocusBackColor = Color.White;
            beepAppTree1.FocusBorderColor = Color.Gray;
            beepAppTree1.FocusForeColor = Color.Black;
            beepAppTree1.FocusIndicatorColor = Color.Blue;
            beepAppTree1.Font = new Font("Arial", 10F);
            beepAppTree1.Form = null;
            beepAppTree1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepAppTree1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepAppTree1.GradientStartColor = Color.White;
            beepAppTree1.GuidID = "fb89b68d-3016-42c0-8e62-2f34a9f894ef";
            beepAppTree1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepAppTree1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepAppTree1.HoveredBackcolor = Color.Wheat;
            beepAppTree1.HoverForeColor = Color.Black;
            beepAppTree1.Id = -1;
          
            beepAppTree1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepAppTree1.Info");
            beepAppTree1.IsAcceptButton = false;
            beepAppTree1.IsBorderAffectedByTheme = true;
            beepAppTree1.IsCancelButton = false;
            beepAppTree1.IsCheckBoxon = false;
            beepAppTree1.IsChild = false;
            beepAppTree1.IsCustomeBorder = false;
            beepAppTree1.IsDefault = false;
            beepAppTree1.IsDeleted = false;
            beepAppTree1.IsDirty = false;
            beepAppTree1.IsEditable = false;
            beepAppTree1.IsFocused = false;
            beepAppTree1.IsFrameless = true;
            beepAppTree1.IsHovered = false;
            beepAppTree1.IsNew = false;
            beepAppTree1.IsPressed = false;
            beepAppTree1.IsReadOnly = false;
            beepAppTree1.IsRequired = false;
            beepAppTree1.IsRounded = false;
            beepAppTree1.IsRoundedAffectedByTheme = false;
            beepAppTree1.IsSelected = false;
            beepAppTree1.IsShadowAffectedByTheme = false;
            beepAppTree1.IsVisible = false;
            beepAppTree1.Items = (List<object>)resources.GetObject("beepAppTree1.Items");
            beepAppTree1.LeftoffsetForDrawingRect = 0;
            beepAppTree1.LinkedProperty = null;
            beepAppTree1.Location = new Point(3, 43);
            beepAppTree1.Name = "beepAppTree1";
            beepAppTree1.NodeHeight = 20;
            beepAppTree1.NodeImageSize = 20;
            beepAppTree1.Nodeseq = 0;
            beepAppTree1.NodeWidth = 100;
            beepAppTree1.ObjectType = "Beep";
            beepAppTree1.OverrideFontSize = TypeStyleFontSize.None;
            beepAppTree1.Padding = new Padding(1);
            beepAppTree1.ParentBackColor = Color.Empty;
            beepAppTree1.ParentControl = null;
            beepAppTree1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepAppTree1.PressedBorderColor = Color.Gray;
            beepAppTree1.PressedForeColor = Color.Black;
            beepAppTree1.RightoffsetForDrawingRect = 0;
            beepAppTree1.SavedGuidID = null;
            beepAppTree1.SavedID = null;
            beepAppTree1.SelectedBranch = null;
            beepAppTree1.SelectedBranchID = 0;
            beepAppTree1.SelectedBranchs = (List<int>)resources.GetObject("beepAppTree1.SelectedBranchs");
            beepAppTree1.SelectedItem = null;
            beepAppTree1.SelectIcon = "Select.svg";
            beepAppTree1.SeqID = 1;
            beepAppTree1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepAppTree1.ShadowOffset = 0;
            beepAppTree1.ShadowOpacity = 0.5F;
            beepAppTree1.ShowAllBorders = false;
            beepAppTree1.ShowBottomBorder = false;
            beepAppTree1.ShowCheckBox = false;
            beepAppTree1.ShowFocusIndicator = false;
            beepAppTree1.ShowLeftBorder = false;
            beepAppTree1.ShowNodeImage = true;
            beepAppTree1.ShowRightBorder = false;
            beepAppTree1.ShowShadow = false;
            beepAppTree1.ShowTopBorder = false;
            beepAppTree1.Size = new Size(200, 754);
            beepAppTree1.SlideFrom = SlideDirection.Left;
            beepAppTree1.StaticNotMoving = false;
            beepAppTree1.TabIndex = 2;
            beepAppTree1.TempBackColor = Color.Empty;
            beepAppTree1.Text = "beepAppTree1";
            beepAppTree1.TextAlignment = ContentAlignment.MiddleLeft;
            beepAppTree1.TextFont = new Font("Arial", 10F);
            beepAppTree1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepAppTree1.ToolTipText = "";
            beepAppTree1.TopoffsetForDrawingRect = 0;
            beepAppTree1.Treebranchhandler = null;
            beepAppTree1.TreeType = "Beep";
            beepAppTree1.UseGradientBackground = false;
            beepAppTree1.UseScaledFont = false;
            beepAppTree1.UseThemeFont = true;
            beepAppTree1.VisManager = null;
            // 
            // beepAppBar1
            // 
          
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = DisplayAnimationType.None;
            beepAppBar1.ApplyThemeButtons = false;
            beepAppBar1.ApplyThemeOnLogo = false;
            beepAppBar1.ApplyThemeToChilds = false;
            beepAppBar1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            beepAppBar1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            beepAppBar1.BackColor = Color.FromArgb(230, 230, 230);
            beepAppBar1.BadgeBackColor = Color.Red;
            beepAppBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepAppBar1.BadgeForeColor = Color.White;
            beepAppBar1.BadgeShape = BadgeShape.Circle;
            beepAppBar1.BadgeText = "";
            beepAppBar1.BlockID = null;
            beepAppBar1.BorderColor = Color.Black;
            beepAppBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepAppBar1.BorderRadius = 3;
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
            beepAppBar1.DrawingRect = new Rectangle(0, 0, 1194, 40);
            beepAppBar1.Easing = EasingType.Linear;
            beepAppBar1.FieldID = null;
            beepAppBar1.FocusBackColor = Color.Gray;
            beepAppBar1.FocusBorderColor = Color.Gray;
            beepAppBar1.FocusForeColor = Color.Black;
            beepAppBar1.FocusIndicatorColor = Color.Blue;
            beepAppBar1.Font = new Font("Segoe UI", 16F);
            beepAppBar1.Form = null;
            beepAppBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepAppBar1.GradientEndColor = Color.Gray;
            beepAppBar1.GradientStartColor = Color.Gray;
            beepAppBar1.GuidID = "2feb3ec3-b1d8-4dd6-b1ec-70c10f5932b9";
            beepAppBar1.HoverBackColor = Color.Gray;
            beepAppBar1.HoverBorderColor = Color.Gray;
            beepAppBar1.HoveredBackcolor = Color.Wheat;
            beepAppBar1.HoverForeColor = Color.Black;
            beepAppBar1.Id = -1;
          
            beepAppBar1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepAppBar1.Info");
            beepAppBar1.IsAcceptButton = false;
            beepAppBar1.IsBorderAffectedByTheme = false;
            beepAppBar1.IsCancelButton = false;
            beepAppBar1.IsChild = false;
            beepAppBar1.IsCustomeBorder = false;
            beepAppBar1.IsDefault = false;
            beepAppBar1.IsDeleted = false;
            beepAppBar1.IsDirty = false;
            beepAppBar1.IsEditable = false;
            beepAppBar1.IsFocused = false;
            beepAppBar1.IsFrameless = true;
            beepAppBar1.IsHovered = false;
            beepAppBar1.IsNew = false;
            beepAppBar1.IsPressed = false;
            beepAppBar1.IsReadOnly = false;
            beepAppBar1.IsRequired = false;
            beepAppBar1.IsRounded = false;
            beepAppBar1.IsRoundedAffectedByTheme = false;
            beepAppBar1.IsSelected = false;
            beepAppBar1.IsShadowAffectedByTheme = false;
            beepAppBar1.IsVisible = false;
            beepAppBar1.Items = (List<object>)resources.GetObject("beepAppBar1.Items");
            beepAppBar1.LeftoffsetForDrawingRect = 0;
            beepAppBar1.LinkedProperty = null;
            beepAppBar1.Location = new Point(3, 3);
            beepAppBar1.Name = "beepAppBar1";
            beepAppBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepAppBar1.ParentBackColor = Color.Empty;
            beepAppBar1.ParentControl = null;
            beepAppBar1.PressedBackColor = Color.Gray;
            beepAppBar1.PressedBorderColor = Color.Gray;
            beepAppBar1.PressedForeColor = Color.Black;
            beepAppBar1.RightoffsetForDrawingRect = 0;
            beepAppBar1.SavedGuidID = null;
            beepAppBar1.SavedID = null;
            beepAppBar1.SearchBoxPlaceholder = "Search...";
            beepAppBar1.SearchBoxText = "";
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
            beepAppBar1.ShowTitle = true;
            beepAppBar1.ShowTopBorder = false;
            beepAppBar1.Size = new Size(1194, 40);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 4;
            beepAppBar1.TempBackColor = Color.Empty;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.TextFont = new Font("Segoe UI", 16F);
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.TopoffsetForDrawingRect = 0;
            beepAppBar1.UseGradientBackground = false;
            beepAppBar1.UseThemeFont = true;
            // 
            // MainFrm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(1200, 800);
            Controls.Add(beepDisplayContainer1);
            Controls.Add(beepMenuAppBar1);
            Controls.Add(beepAppTree1);
            Controls.Add(beepAppBar1);
            Name = "MainFrm";
            Text = "Beep Development and Data Platform";
            ResumeLayout(false);
        }

        #endregion
        private Controls.BeepDisplayContainer beepDisplayContainer1;
        private Controls.ITrees.BeepTreeView.BeepAppTree beepAppTree1;
        private Controls.MenuBar.BeepMenuAppBar beepMenuAppBar1;
        private Controls.BeepAppBar beepAppBar1;
    }
}