namespace TheTechIdea.Beep.Winform.Default.Views
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            beepAppBar1 = new TheTechIdea.Beep.Winform.Controls.BeepAppBar();
            beepMenuBar1 = new TheTechIdea.Beep.Winform.Controls.BeepMenuBar();
            beepTreeControl1 = new TheTechIdea.Beep.Winform.Controls.ITrees.BeepTreeView.BeepTreeControl();
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepAppBar = beepAppBar1;
            beepuiManager1.BeepiForm = this;
            beepuiManager1.BeepMenuBar = beepMenuBar1;
            beepuiManager1.ShowBorder = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepuiManager1.Title = "Beep Data Management Platform";
            // 
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = Winform.Controls.DisplayAnimationType.None;
            beepAppBar1.ApplyThemeOnLogo = false;
            beepAppBar1.ApplyThemeToChilds = true;
            beepAppBar1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            beepAppBar1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            beepAppBar1.BackColor = Color.FromArgb(230, 230, 230);
            beepAppBar1.BadgeBackColor = Color.Red;
            beepAppBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepAppBar1.BadgeForeColor = Color.White;
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
            beepAppBar1.DrawingRect = new Rectangle(0, 0, 936, 32);
            beepAppBar1.Easing = Winform.Controls.EasingType.Linear;
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
            beepAppBar1.GuidID = "66e1d05f-e893-42dd-8efc-43776d91971e";
            beepAppBar1.HoverBackColor = Color.Gray;
            beepAppBar1.HoverBorderColor = Color.Gray;
            beepAppBar1.HoveredBackcolor = Color.Wheat;
            beepAppBar1.HoverForeColor = Color.Black;
            beepAppBar1.Id = -1;
            beepAppBar1.InactiveBackColor = Color.Gray;
            beepAppBar1.InactiveBorderColor = Color.Gray;
            beepAppBar1.InactiveForeColor = Color.Black;
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
            beepAppBar1.IsFramless = false;
            beepAppBar1.IsHovered = false;
            beepAppBar1.IsNew = false;
            beepAppBar1.IsPressed = false;
            beepAppBar1.IsReadOnly = false;
            beepAppBar1.IsRequired = false;
            beepAppBar1.IsRounded = true;
            beepAppBar1.IsRoundedAffectedByTheme = false;
            beepAppBar1.IsSelected = false;
            beepAppBar1.IsShadowAffectedByTheme = false;
            beepAppBar1.IsVisible = false;
            beepAppBar1.LeftoffsetForDrawingRect = 0;
            beepAppBar1.LinkedProperty = null;
            beepAppBar1.Location = new Point(4, 4);
            beepAppBar1.Name = "beepAppBar1";
            beepAppBar1.OverrideFontSize = Winform.Controls.TypeStyleFontSize.None;
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
            beepAppBar1.Size = new Size(936, 32);
            beepAppBar1.SlideFrom = Winform.Controls.SlideDirection.Left;
            beepAppBar1.StaticNotMoving = false;
            beepAppBar1.TabIndex = 0;
            beepAppBar1.TempBackColor = Color.Empty;
            beepAppBar1.Text = "beepAppBar1";
            beepAppBar1.TextFont = new Font("Segoe UI", 16F);
            beepAppBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepAppBar1.ToolTipText = "";
            beepAppBar1.TopoffsetForDrawingRect = 0;
            beepAppBar1.UseGradientBackground = false;
            beepAppBar1.UseThemeFont = true;
            // 
            // beepMenuBar1
            // 
            beepMenuBar1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepMenuBar1.AnimationDuration = 500;
            beepMenuBar1.AnimationType = Winform.Controls.DisplayAnimationType.None;
            beepMenuBar1.ApplyThemeToChilds = true;
            beepMenuBar1.BackColor = Color.FromArgb(230, 230, 240);
            beepMenuBar1.BadgeBackColor = Color.Red;
            beepMenuBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepMenuBar1.BadgeForeColor = Color.White;
            beepMenuBar1.BadgeText = "";
            beepMenuBar1.BlockID = null;
            beepMenuBar1.BorderColor = Color.FromArgb(200, 200, 200);
            beepMenuBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepMenuBar1.BorderRadius = 3;
            beepMenuBar1.BorderStyle = BorderStyle.FixedSingle;
            beepMenuBar1.BorderThickness = 1;
            beepMenuBar1.BottomoffsetForDrawingRect = 0;
            beepMenuBar1.BoundProperty = "SelectedMenuItem";
            beepMenuBar1.CanBeFocused = true;
            beepMenuBar1.CanBeHovered = false;
            beepMenuBar1.CanBePressed = true;
            beepMenuBar1.Category = Utilities.DbFieldCategory.String;
            beepMenuBar1.ComponentName = "beepMenuBar1";
            beepMenuBar1.DataContext = null;
            beepMenuBar1.DataSourceProperty = null;
            beepMenuBar1.DisabledBackColor = Color.Gray;
            beepMenuBar1.DisabledForeColor = Color.Empty;
            beepMenuBar1.Dock = DockStyle.Top;
            beepMenuBar1.DrawingRect = new Rectangle(0, 0, 936, 26);
            beepMenuBar1.Easing = Winform.Controls.EasingType.Linear;
            beepMenuBar1.FieldID = null;
            beepMenuBar1.FocusBackColor = Color.White;
            beepMenuBar1.FocusBorderColor = Color.Gray;
            beepMenuBar1.FocusForeColor = Color.Black;
            beepMenuBar1.FocusIndicatorColor = Color.Blue;
            beepMenuBar1.Font = new Font("Segoe UI", 9F);
            beepMenuBar1.Form = null;
            beepMenuBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepMenuBar1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepMenuBar1.GradientStartColor = Color.White;
            beepMenuBar1.GuidID = "192d08e1-d1be-4554-b63c-6d68f8637d12";
            beepMenuBar1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepMenuBar1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepMenuBar1.HoveredBackcolor = Color.Wheat;
            beepMenuBar1.HoverForeColor = Color.Black;
            beepMenuBar1.Id = -1;
            beepMenuBar1.ImageSize = 33;
            beepMenuBar1.InactiveBackColor = Color.Gray;
            beepMenuBar1.InactiveBorderColor = Color.Gray;
            beepMenuBar1.InactiveForeColor = Color.Black;
            beepMenuBar1.IsAcceptButton = false;
            beepMenuBar1.IsBorderAffectedByTheme = true;
            beepMenuBar1.IsCancelButton = false;
            beepMenuBar1.IsChild = false;
            beepMenuBar1.IsCustomeBorder = false;
            beepMenuBar1.IsDefault = false;
            beepMenuBar1.IsDeleted = false;
            beepMenuBar1.IsDirty = false;
            beepMenuBar1.IsEditable = false;
            beepMenuBar1.IsFocused = false;
            beepMenuBar1.IsFramless = true;
            beepMenuBar1.IsHovered = false;
            beepMenuBar1.IsNew = false;
            beepMenuBar1.IsPressed = false;
            beepMenuBar1.IsReadOnly = false;
            beepMenuBar1.IsRequired = false;
            beepMenuBar1.IsRounded = true;
            beepMenuBar1.IsRoundedAffectedByTheme = true;
            beepMenuBar1.IsSelected = false;
            beepMenuBar1.IsShadowAffectedByTheme = true;
            beepMenuBar1.IsVisible = false;
            beepMenuBar1.LeftoffsetForDrawingRect = 0;
            beepMenuBar1.LinkedProperty = null;
            beepMenuBar1.Location = new Point(4, 36);
            beepMenuBar1.MenuItemHeight = 35;
            beepMenuBar1.MenuItemWidth = 60;
            beepMenuBar1.Name = "beepMenuBar1";
            beepMenuBar1.OverrideFontSize = Winform.Controls.TypeStyleFontSize.None;
            beepMenuBar1.ParentBackColor = Color.Empty;
            beepMenuBar1.ParentControl = null;
            beepMenuBar1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepMenuBar1.PressedBorderColor = Color.Gray;
            beepMenuBar1.PressedForeColor = Color.Black;
            beepMenuBar1.RightoffsetForDrawingRect = 0;
            beepMenuBar1.SavedGuidID = null;
            beepMenuBar1.SavedID = null;
            beepMenuBar1.SelectedIndex = -1;
            beepMenuBar1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepMenuBar1.ShadowOffset = 0;
            beepMenuBar1.ShadowOpacity = 0.5F;
            beepMenuBar1.ShowAllBorders = false;
            beepMenuBar1.ShowBottomBorder = false;
            beepMenuBar1.ShowFocusIndicator = false;
            beepMenuBar1.ShowLeftBorder = false;
            beepMenuBar1.ShowRightBorder = false;
            beepMenuBar1.ShowShadow = false;
            beepMenuBar1.ShowTopBorder = false;
            beepMenuBar1.Size = new Size(936, 26);
            beepMenuBar1.SlideFrom = Winform.Controls.SlideDirection.Left;
            beepMenuBar1.StaticNotMoving = false;
            beepMenuBar1.TabIndex = 2;
            beepMenuBar1.TempBackColor = Color.Empty;
            beepMenuBar1.Text = "beepMenuBar1";
            beepMenuBar1.TextFont = new Font("Segoe UI", 9F);
            beepMenuBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepMenuBar1.ToolTipText = "";
            beepMenuBar1.TopoffsetForDrawingRect = 0;
            beepMenuBar1.UseGradientBackground = false;
            beepMenuBar1.UseThemeFont = true;
            // 
            // beepTreeControl1
            // 
            beepTreeControl1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepTreeControl1.AllowMultiSelect = false;
            beepTreeControl1.AnimationDuration = 500;
            beepTreeControl1.AnimationType = Winform.Controls.DisplayAnimationType.None;
            beepTreeControl1.ApplyThemeToChilds = false;
            beepTreeControl1.args = null;
            beepTreeControl1.AutoScroll = true;
            beepTreeControl1.BackColor = Color.FromArgb(240, 240, 240);
            beepTreeControl1.BadgeBackColor = Color.Red;
            beepTreeControl1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepTreeControl1.BadgeForeColor = Color.White;
            beepTreeControl1.BadgeText = "";
            beepTreeControl1.BeepService = null;
            beepTreeControl1.BlockID = null;
            beepTreeControl1.BorderColor = Color.FromArgb(200, 200, 200);
            beepTreeControl1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepTreeControl1.BorderRadius = 3;
            beepTreeControl1.BorderStyle = BorderStyle.FixedSingle;
            beepTreeControl1.BorderThickness = 1;
            beepTreeControl1.BottomoffsetForDrawingRect = 0;
            beepTreeControl1.BoundProperty = null;
            beepTreeControl1.Branches = (List<Vis.Modules.IBranch>)resources.GetObject("beepTreeControl1.Branches");
            beepTreeControl1.CanBeFocused = true;
            beepTreeControl1.CanBeHovered = false;
            beepTreeControl1.CanBePressed = true;
            beepTreeControl1.Category = Utilities.DbFieldCategory.String;
            beepTreeControl1.CategoryIcon = "Category.svg";
            beepTreeControl1.ComponentName = "beepTreeControl1";
            beepTreeControl1.CurrentBranch = null;
            beepTreeControl1.CurrentMenutems = null;
            beepTreeControl1.DataContext = null;
            beepTreeControl1.DataSourceProperty = null;
            beepTreeControl1.DisabledBackColor = Color.Gray;
            beepTreeControl1.DisabledForeColor = Color.Empty;
            beepTreeControl1.DMEEditor = null;
            beepTreeControl1.Dock = DockStyle.Left;
            beepTreeControl1.DrawingRect = new Rectangle(1, 1, 198, 382);
            beepTreeControl1.DropHandler = null;
            beepTreeControl1.Easing = Winform.Controls.EasingType.Linear;
            beepTreeControl1.FieldID = null;
            beepTreeControl1.Filterstring = null;
            beepTreeControl1.FocusBackColor = Color.White;
            beepTreeControl1.FocusBorderColor = Color.Gray;
            beepTreeControl1.FocusForeColor = Color.Black;
            beepTreeControl1.FocusIndicatorColor = Color.Blue;
            beepTreeControl1.Form = null;
            beepTreeControl1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepTreeControl1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepTreeControl1.GradientStartColor = Color.White;
            beepTreeControl1.GuidID = "c480e8c8-d5f3-4d73-aee2-9eaa3162f43d";
            beepTreeControl1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepTreeControl1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepTreeControl1.HoveredBackcolor = Color.Wheat;
            beepTreeControl1.HoverForeColor = Color.Black;
            beepTreeControl1.Id = -1;
            beepTreeControl1.InactiveBackColor = Color.Gray;
            beepTreeControl1.InactiveBorderColor = Color.Gray;
            beepTreeControl1.InactiveForeColor = Color.Black;
            beepTreeControl1.IsAcceptButton = false;
            beepTreeControl1.IsBorderAffectedByTheme = true;
            beepTreeControl1.IsCancelButton = false;
            beepTreeControl1.IsChild = false;
            beepTreeControl1.IsCustomeBorder = false;
            beepTreeControl1.IsDefault = false;
            beepTreeControl1.IsDeleted = false;
            beepTreeControl1.IsDirty = false;
            beepTreeControl1.IsEditable = false;
            beepTreeControl1.IsFocused = false;
            beepTreeControl1.IsFramless = false;
            beepTreeControl1.IsHovered = false;
            beepTreeControl1.IsNew = false;
            beepTreeControl1.IsPopupOpen = false;
            beepTreeControl1.IsPressed = false;
            beepTreeControl1.IsReadOnly = false;
            beepTreeControl1.IsRequired = false;
            beepTreeControl1.IsRounded = true;
            beepTreeControl1.IsRoundedAffectedByTheme = true;
            beepTreeControl1.IsSelected = false;
            beepTreeControl1.IsShadowAffectedByTheme = true;
            beepTreeControl1.IsVisible = false;
            beepTreeControl1.LeftoffsetForDrawingRect = 0;
            beepTreeControl1.LinkedProperty = null;
            beepTreeControl1.Location = new Point(4, 62);
            beepTreeControl1.Name = "beepTreeControl1";
            beepTreeControl1.NodeHeight = 20;
            beepTreeControl1.NodeImageSize = 20;
            beepTreeControl1.Nodeseq = 0;
            beepTreeControl1.NodeWidth = 100;
            beepTreeControl1.ObjectType = "Beep";
            beepTreeControl1.OverrideFontSize = Winform.Controls.TypeStyleFontSize.None;
            beepTreeControl1.Padding = new Padding(1);
            beepTreeControl1.ParentBackColor = Color.Empty;
            beepTreeControl1.ParentControl = null;
            beepTreeControl1.PopupMode = false;
            beepTreeControl1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepTreeControl1.PressedBorderColor = Color.Gray;
            beepTreeControl1.PressedForeColor = Color.Black;
            beepTreeControl1.RightoffsetForDrawingRect = 0;
            beepTreeControl1.SavedGuidID = null;
            beepTreeControl1.SavedID = null;
            beepTreeControl1.SelectedBranchID = 0;
            beepTreeControl1.SelectedBranchs = (List<int>)resources.GetObject("beepTreeControl1.SelectedBranchs");
            beepTreeControl1.SelectedIndex = -1;
            beepTreeControl1.SelectIcon = "Select.svg";
            beepTreeControl1.SeqID = 7;
            beepTreeControl1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepTreeControl1.ShadowOffset = 0;
            beepTreeControl1.ShadowOpacity = 0.5F;
            beepTreeControl1.ShowAllBorders = false;
            beepTreeControl1.ShowBottomBorder = false;
            beepTreeControl1.ShowCheckBox = false;
            beepTreeControl1.ShowFocusIndicator = false;
            beepTreeControl1.ShowLeftBorder = false;
            beepTreeControl1.ShowNodeImage = true;
            beepTreeControl1.ShowRightBorder = false;
            beepTreeControl1.ShowShadow = false;
            beepTreeControl1.ShowTopBorder = false;
            beepTreeControl1.Size = new Size(200, 384);
            beepTreeControl1.SlideFrom = Winform.Controls.SlideDirection.Left;
            beepTreeControl1.StaticNotMoving = false;
            beepTreeControl1.TabIndex = 3;
            beepTreeControl1.TempBackColor = Color.Empty;
            beepTreeControl1.Text = "beepTreeControl1";
            beepTreeControl1.TextAlignment = ContentAlignment.MiddleLeft;
            beepTreeControl1.TextFont = new Font("Arial", 10F);
            beepTreeControl1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepTreeControl1.ToolTipText = "";
            beepTreeControl1.TopoffsetForDrawingRect = 0;
            beepTreeControl1.Treebranchhandler = null;
            beepTreeControl1.TreeType = "Beep";
            beepTreeControl1.UseGradientBackground = false;
            beepTreeControl1.UseScaledFont = false;
            beepTreeControl1.UseThemeFont = true;
            beepTreeControl1.VisManager = null;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(944, 450);
            Controls.Add(beepTreeControl1);
            Controls.Add(beepMenuBar1);
            Controls.Add(beepAppBar1);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
        }

        #endregion

        private Controls.BeepAppBar beepAppBar1;
        private Controls.BeepMenuBar beepMenuBar1;
        private Controls.ITrees.BeepTreeView.BeepTreeControl beepTreeControl1;
    }
}