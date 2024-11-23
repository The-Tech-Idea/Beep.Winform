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
            components = new System.ComponentModel.Container();
            DataNavigator.BeepBindingSource beepBindingSource1 = new DataNavigator.BeepBindingSource();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            beepAppBar1 = new BeepAppBar();
            beepuiManager1 = new BeepUIManager(components);
            beepCard1 = new BeepCard();
            beepDataNavigator1 = new BeepDataNavigator();
            beepSimpleGrid2 = new BeepSimpleGrid();
            SuspendLayout();
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
            beepAppBar1.DrawingRect = new Rectangle(1, 1, 850, 44);
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
            beepAppBar1.IsCustomeBorder = false;
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
            beepAppBar1.ShowAllBorders = true;
            beepAppBar1.ShowBottomBorder = true;
            beepAppBar1.ShowFocusIndicator = false;
            beepAppBar1.ShowLeftBorder = true;
            beepAppBar1.ShowRightBorder = true;
            beepAppBar1.ShowShadow = false;
            beepAppBar1.ShowTopBorder = true;
            beepAppBar1.SideMenu = null;
            beepAppBar1.Size = new Size(852, 46);
            beepAppBar1.SlideFrom = SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 1;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.HighlightTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.UseGradientBackground = false;
            // 
            // beepuiManager1
            // 
            beepuiManager1.ApplyThemeOnImage = true;
            beepuiManager1.BeepFunctionsPanel = null;
            beepuiManager1.BeepiForm = null;
            beepuiManager1.BeepSideMenu = null;
            beepuiManager1.IsRounded = true;
            beepuiManager1.LogoImage = "";
            beepuiManager1.ShowBorder = true;
            beepuiManager1.ShowShadow = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.HighlightTheme;
            beepuiManager1.Title = "Beep Form";
            // 
            // beepCard1
            // 
            beepCard1.ActiveBackColor = Color.Gray;
            beepCard1.AnimationDuration = 500;
            beepCard1.AnimationType = DisplayAnimationType.None;
            beepCard1.BlockID = null;
            beepCard1.BorderColor = Color.Black;
            beepCard1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepCard1.BorderRadius = 5;
            beepCard1.BorderStyle = BorderStyle.FixedSingle;
            beepCard1.BorderThickness = 1;
            beepCard1.DataContext = null;
            beepCard1.DisabledBackColor = Color.Gray;
            beepCard1.DisabledForeColor = Color.Empty;
            beepCard1.DrawingRect = new Rectangle(1, 1, 216, 169);
            beepCard1.Easing = EasingType.Linear;
            beepCard1.FieldID = null;
            beepCard1.FocusBackColor = Color.Gray;
            beepCard1.FocusBorderColor = Color.Gray;
            beepCard1.FocusForeColor = Color.Black;
            beepCard1.FocusIndicatorColor = Color.Blue;
            beepCard1.Form = null;
            beepCard1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepCard1.GradientEndColor = Color.Gray;
            beepCard1.GradientStartColor = Color.Gray;
            beepCard1.HeaderAlignment = ContentAlignment.TopLeft;
            beepCard1.HeaderText = "Card Title";
            beepCard1.HoverBackColor = Color.Gray;
            beepCard1.HoverBorderColor = Color.Gray;
            beepCard1.HoveredBackcolor = Color.Wheat;
            beepCard1.HoverForeColor = Color.Black;
            beepCard1.Id = -1;
            beepCard1.ImageAlignment = ContentAlignment.TopRight;
            beepCard1.ImagePath = null;
            beepCard1.InactiveBackColor = Color.Gray;
            beepCard1.InactiveBorderColor = Color.Gray;
            beepCard1.InactiveForeColor = Color.Black;
            beepCard1.IsAcceptButton = false;
            beepCard1.IsBorderAffectedByTheme = true;
            beepCard1.IsCancelButton = false;
            beepCard1.IsChild = false;
            beepCard1.IsCustomeBorder = false;
            beepCard1.IsDefault = false;
            beepCard1.IsFocused = false;
            beepCard1.IsFramless = false;
            beepCard1.IsHovered = false;
            beepCard1.IsPressed = false;
            beepCard1.IsRounded = true;
            beepCard1.IsShadowAffectedByTheme = true;
            beepCard1.Location = new Point(43, 75);
            beepCard1.MaxImageSize = 64;
            beepCard1.Name = "beepCard1";
            beepCard1.OverrideFontSize = TypeStyleFontSize.None;
            beepCard1.Padding = new Padding(5);
            beepCard1.ParagraphText = "Card Description";
            beepCard1.ParentBackColor = Color.Empty;
            beepCard1.PressedBackColor = Color.Gray;
            beepCard1.PressedBorderColor = Color.Gray;
            beepCard1.PressedForeColor = Color.Black;
            beepCard1.SavedGuidID = null;
            beepCard1.SavedID = null;
            beepCard1.ShadowColor = Color.Black;
            beepCard1.ShadowOffset = 0;
            beepCard1.ShadowOpacity = 0.5F;
            beepCard1.ShowAllBorders = true;
            beepCard1.ShowBottomBorder = true;
            beepCard1.ShowFocusIndicator = false;
            beepCard1.ShowLeftBorder = true;
            beepCard1.ShowRightBorder = true;
            beepCard1.ShowShadow = false;
            beepCard1.ShowTopBorder = true;
            beepCard1.Size = new Size(218, 171);
            beepCard1.SlideFrom = SlideDirection.Left;
            beepCard1.StaticNotMoving = false;
            beepCard1.TabIndex = 2;
            beepCard1.Text = "beepCard1";
            beepCard1.TextAlignment = ContentAlignment.BottomCenter;
            beepCard1.Theme = Vis.Modules.EnumBeepThemes.HighlightTheme;
            beepCard1.ToolTipText = "";
            beepCard1.UseGradientBackground = false;
            // 
            // beepDataNavigator1
            // 
            beepDataNavigator1.ActiveBackColor = Color.Gray;
            beepDataNavigator1.AnimationDuration = 500;
            beepDataNavigator1.AnimationType = DisplayAnimationType.None;
            beepBindingSource1.ChildUnitofWorks = (List<Editor.IUnitofWork>)resources.GetObject("beepBindingSource1.ChildUnitofWorks");
            beepBindingSource1.Filter = null;
            beepBindingSource1.Position = -1;
            beepBindingSource1.UnitofWork = null;
            beepDataNavigator1.BindingSource = beepBindingSource1;
            beepDataNavigator1.BlockID = null;
            beepDataNavigator1.BorderColor = Color.Black;
            beepDataNavigator1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepDataNavigator1.BorderRadius = 5;
            beepDataNavigator1.BorderStyle = BorderStyle.FixedSingle;
            beepDataNavigator1.BorderThickness = 1;
            beepDataNavigator1.ButtonHeight = 15;
            beepDataNavigator1.ButtonSpacing = 5;
            beepDataNavigator1.ButtonWidth = 15;
            beepDataNavigator1.DataContext = null;
            beepDataNavigator1.DisabledBackColor = Color.Gray;
            beepDataNavigator1.DisabledForeColor = Color.Empty;
            beepDataNavigator1.DrawingRect = new Rectangle(1, 1, 236, 17);
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
            beepDataNavigator1.IsShadowAffectedByTheme = false;
            beepDataNavigator1.Location = new Point(351, 228);
            beepDataNavigator1.MinimumSize = new Size(190, 19);
            beepDataNavigator1.Name = "beepDataNavigator1";
            beepDataNavigator1.OverrideFontSize = TypeStyleFontSize.None;
            beepDataNavigator1.ParentBackColor = Color.Empty;
            beepDataNavigator1.PressedBackColor = Color.Gray;
            beepDataNavigator1.PressedBorderColor = Color.Gray;
            beepDataNavigator1.PressedForeColor = Color.Black;
            beepDataNavigator1.SavedGuidID = null;
            beepDataNavigator1.SavedID = null;
            beepDataNavigator1.ShadowColor = Color.Black;
            beepDataNavigator1.ShadowOffset = 0;
            beepDataNavigator1.ShadowOpacity = 0.5F;
            beepDataNavigator1.ShowAllBorders = true;
            beepDataNavigator1.ShowBottomBorder = true;
            beepDataNavigator1.ShowFocusIndicator = false;
            beepDataNavigator1.ShowLeftBorder = true;
            beepDataNavigator1.ShowRightBorder = true;
            beepDataNavigator1.ShowShadow = false;
            beepDataNavigator1.ShowTopBorder = true;
            beepDataNavigator1.Size = new Size(238, 19);
            beepDataNavigator1.SlideFrom = SlideDirection.Left;
            beepDataNavigator1.StaticNotMoving = false;
            beepDataNavigator1.TabIndex = 3;
            beepDataNavigator1.Text = "beepDataNavigator1";
            beepDataNavigator1.Theme = Vis.Modules.EnumBeepThemes.HighlightTheme;
            beepDataNavigator1.ToolTipText = "";
            beepDataNavigator1.UseGradientBackground = false;
            beepDataNavigator1.XOffset = 5;
            beepDataNavigator1.YOffset = 5;
            // 
            // beepSimpleGrid2
            // 
            beepSimpleGrid2.ActiveBackColor = Color.Gray;
            beepSimpleGrid2.AnimationDuration = 500;
            beepSimpleGrid2.AnimationType = DisplayAnimationType.None;
            beepSimpleGrid2.BackColor = Color.White;
            beepSimpleGrid2.beepGridColumns = (System.ComponentModel.BindingList<Grid.BeepGridColumnConfig>)resources.GetObject("beepSimpleGrid2.beepGridColumns");
            beepSimpleGrid2.BlockID = null;
            beepSimpleGrid2.BorderColor = Color.Black;
            beepSimpleGrid2.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSimpleGrid2.BorderRadius = 5;
            beepSimpleGrid2.BorderStyle = BorderStyle.FixedSingle;
            beepSimpleGrid2.BorderThickness = 1;
            beepSimpleGrid2.BottomRow = null;
            beepSimpleGrid2.ColumnHeight = 20;
            beepSimpleGrid2.DataContext = null;
            beepSimpleGrid2.DataSource = null;
            beepSimpleGrid2.DataSourceType = GridDataSourceType.Fixed;
            beepSimpleGrid2.DisabledBackColor = Color.Gray;
            beepSimpleGrid2.DisabledForeColor = Color.Empty;
            beepSimpleGrid2.DrawingRect = new Rectangle(1, 1, 387, 151);
            beepSimpleGrid2.Easing = EasingType.Linear;
            beepSimpleGrid2.FieldID = null;
            beepSimpleGrid2.FocusBackColor = Color.Gray;
            beepSimpleGrid2.FocusBorderColor = Color.Gray;
            beepSimpleGrid2.FocusForeColor = Color.Black;
            beepSimpleGrid2.FocusIndicatorColor = Color.Blue;
            beepSimpleGrid2.ForeColor = Color.FromArgb(0, 0, 0);
            beepSimpleGrid2.Form = null;
            beepSimpleGrid2.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSimpleGrid2.GradientEndColor = Color.Gray;
            beepSimpleGrid2.GradientStartColor = Color.Gray;
            beepSimpleGrid2.HoverBackColor = Color.Gray;
            beepSimpleGrid2.HoverBorderColor = Color.Gray;
            beepSimpleGrid2.HoveredBackcolor = Color.Wheat;
            beepSimpleGrid2.HoverForeColor = Color.Black;
            beepSimpleGrid2.Id = -1;
            beepSimpleGrid2.InactiveBackColor = Color.Gray;
            beepSimpleGrid2.InactiveBorderColor = Color.Gray;
            beepSimpleGrid2.InactiveForeColor = Color.Black;
            beepSimpleGrid2.IsAcceptButton = false;
            beepSimpleGrid2.IsBorderAffectedByTheme = true;
            beepSimpleGrid2.IsCancelButton = false;
            beepSimpleGrid2.IsChild = false;
            beepSimpleGrid2.IsCustomeBorder = false;
            beepSimpleGrid2.IsDefault = false;
            beepSimpleGrid2.IsFocused = false;
            beepSimpleGrid2.IsFramless = false;
            beepSimpleGrid2.IsHovered = false;
            beepSimpleGrid2.IsPressed = false;
            beepSimpleGrid2.IsRounded = true;
            beepSimpleGrid2.IsShadowAffectedByTheme = true;
            beepSimpleGrid2.Location = new Point(237, 274);
            beepSimpleGrid2.Name = "beepSimpleGrid2";
            beepSimpleGrid2.OverrideFontSize = TypeStyleFontSize.None;
            beepSimpleGrid2.ParentBackColor = Color.Empty;
            beepSimpleGrid2.PressedBackColor = Color.Gray;
            beepSimpleGrid2.PressedBorderColor = Color.Gray;
            beepSimpleGrid2.PressedForeColor = Color.Black;
            beepSimpleGrid2.QueryFunction = null;
            beepSimpleGrid2.QueryFunctionName = null;
            beepSimpleGrid2.SavedGuidID = null;
            beepSimpleGrid2.SavedID = null;
            beepSimpleGrid2.ShadowColor = Color.Black;
            beepSimpleGrid2.ShadowOffset = 0;
            beepSimpleGrid2.ShadowOpacity = 0.5F;
            beepSimpleGrid2.ShowAllBorders = true;
            beepSimpleGrid2.ShowBottomBorder = true;
            beepSimpleGrid2.ShowBottomRow = true;
            beepSimpleGrid2.ShowColumnHeaders = true;
            beepSimpleGrid2.ShowCRUDPanel = true;
            beepSimpleGrid2.ShowFilterButton = true;
            beepSimpleGrid2.ShowFocusIndicator = false;
            beepSimpleGrid2.ShowFooter = true;
            beepSimpleGrid2.ShowHeaderPanel = true;
            beepSimpleGrid2.ShowHeaderPanelBorder = true;
            beepSimpleGrid2.ShowLeftBorder = true;
            beepSimpleGrid2.ShowNavigator = true;
            beepSimpleGrid2.ShowRightBorder = true;
            beepSimpleGrid2.ShowRowHeaders = true;
            beepSimpleGrid2.ShowRowNumbers = true;
            beepSimpleGrid2.ShowShadow = false;
            beepSimpleGrid2.ShowSortIcons = true;
            beepSimpleGrid2.ShowTitle = true;
            beepSimpleGrid2.ShowTopBorder = true;
            beepSimpleGrid2.Size = new Size(389, 153);
            beepSimpleGrid2.SlideFrom = SlideDirection.Left;
            beepSimpleGrid2.StaticNotMoving = false;
            beepSimpleGrid2.TabIndex = 4;
            beepSimpleGrid2.Text = "beepSimpleGrid2";
            beepSimpleGrid2.Theme = Vis.Modules.EnumBeepThemes.HighlightTheme;
            beepSimpleGrid2.Title = "BeepSimpleGrid Title";
            beepSimpleGrid2.ToolTipText = "";
            beepSimpleGrid2.UseGradientBackground = false;
            beepSimpleGrid2.XOffset = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(852, 450);
            Controls.Add(beepSimpleGrid2);
            Controls.Add(beepDataNavigator1);
            Controls.Add(beepCard1);
            Controls.Add(beepAppBar1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion
        private BeepSimpleGrid beepSimpleGrid1;
        private BeepDatePicker beepDatePicker1;
        private BeepAppBar beepAppBar1;
        private BeepUIManager beepuiManager1;
        private BeepCard beepCard1;
        private BeepDataNavigator beepDataNavigator1;
        private BeepSimpleGrid beepSimpleGrid2;
    }
}