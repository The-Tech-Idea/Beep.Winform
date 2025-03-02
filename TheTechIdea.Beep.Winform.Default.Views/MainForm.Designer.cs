﻿namespace TheTechIdea.Beep.Winform.Default.Views
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            beepTreeControl1 = new TheTechIdea.Beep.Winform.Controls.ITrees.BeepTreeView.BeepAppTree();
            beepAppBar1 = new TheTechIdea.Beep.Winform.Controls.BeepAppBar();
            beepMenuAppBar1 = new TheTechIdea.Beep.Winform.Controls.MenuBar.BeepMenuAppBar();
            driversConfigViewModelBindingSource = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)driversConfigViewModelBindingSource).BeginInit();
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
            beepuiManager1.Title = "Beep Data Management Platform";
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
            beepTreeControl1.BadgeShape = Winform.Controls.BadgeShape.Circle;
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
            beepTreeControl1.ClickedNode = null;
            beepTreeControl1.ComponentName = "beepTreeControl1";
            beepTreeControl1.CurrentBranch = null;
            beepTreeControl1.CurrentMenutems = null;
            beepTreeControl1.DataContext = null;
            beepTreeControl1.DataSourceProperty = null;
            beepTreeControl1.DisabledBackColor = Color.Gray;
            beepTreeControl1.DisabledForeColor = Color.Empty;
            beepTreeControl1.DMEEditor = null;
            beepTreeControl1.Dock = DockStyle.Left;
            beepTreeControl1.DrawingRect = new Rectangle(1, 1, 198, 778);
            beepTreeControl1.DropHandler = null;
            beepTreeControl1.Easing = Winform.Controls.EasingType.Linear;
            beepTreeControl1.FieldID = null;
            beepTreeControl1.Filterstring = null;
            beepTreeControl1.FocusBackColor = Color.White;
            beepTreeControl1.FocusBorderColor = Color.Gray;
            beepTreeControl1.FocusForeColor = Color.Black;
            beepTreeControl1.FocusIndicatorColor = Color.Blue;
            beepTreeControl1.Font = new Font("Arial", 10F);
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
            beepTreeControl1.Info = (Desktop.Common.SimpleItem)resources.GetObject("beepTreeControl1.Info");
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
            beepTreeControl1.IsFrameless = false;
            beepTreeControl1.IsHovered = false;
            beepTreeControl1.IsNew = false;
            beepTreeControl1.IsPressed = false;
            beepTreeControl1.IsReadOnly = false;
            beepTreeControl1.IsRequired = false;
            beepTreeControl1.IsRounded = false;
            beepTreeControl1.IsRoundedAffectedByTheme = true;
            beepTreeControl1.IsSelected = false;
            beepTreeControl1.IsShadowAffectedByTheme = true;
            beepTreeControl1.IsVisible = false;
            beepTreeControl1.Items = (List<object>)resources.GetObject("beepTreeControl1.Items");
            beepTreeControl1.LeftoffsetForDrawingRect = 0;
            beepTreeControl1.LinkedProperty = null;
            beepTreeControl1.Location = new Point(4, 72);
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
            beepTreeControl1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepTreeControl1.PressedBorderColor = Color.Gray;
            beepTreeControl1.PressedForeColor = Color.Black;
            beepTreeControl1.RightoffsetForDrawingRect = 0;
            beepTreeControl1.SavedGuidID = null;
            beepTreeControl1.SavedID = null;
            beepTreeControl1.SelectedBranch = null;
            beepTreeControl1.SelectedBranchID = 0;
            beepTreeControl1.SelectedBranchs = (List<int>)resources.GetObject("beepTreeControl1.SelectedBranchs");
            beepTreeControl1.SelectedItem = null;
            beepTreeControl1.SelectIcon = "Select.svg";
            beepTreeControl1.SeqID = 3;
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
            beepTreeControl1.Size = new Size(200, 780);
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
            // beepAppBar1
            // 
            beepAppBar1.ActiveBackColor = Color.Gray;
            beepAppBar1.AnimationDuration = 500;
            beepAppBar1.AnimationType = Winform.Controls.DisplayAnimationType.None;
            beepAppBar1.ApplyThemeButtons = false;
            beepAppBar1.ApplyThemeOnLogo = false;
            beepAppBar1.ApplyThemeToChilds = false;
            beepAppBar1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            beepAppBar1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            beepAppBar1.BackColor = Color.FromArgb(230, 230, 230);
            beepAppBar1.BadgeBackColor = Color.Red;
            beepAppBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepAppBar1.BadgeForeColor = Color.White;
            beepAppBar1.BadgeShape = Winform.Controls.BadgeShape.Circle;
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
            beepAppBar1.DrawingRect = new Rectangle(0, 0, 1173, 40);
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
            beepAppBar1.GuidID = "85d4fc8b-a08d-4393-b6e8-12e804c7a6e1";
            beepAppBar1.HoverBackColor = Color.Gray;
            beepAppBar1.HoverBorderColor = Color.Gray;
            beepAppBar1.HoveredBackcolor = Color.Wheat;
            beepAppBar1.HoverForeColor = Color.Black;
            beepAppBar1.Id = -1;
            beepAppBar1.InactiveBackColor = Color.Gray;
            beepAppBar1.InactiveBorderColor = Color.Gray;
            beepAppBar1.InactiveForeColor = Color.Black;
            beepAppBar1.Info = (Desktop.Common.SimpleItem)resources.GetObject("beepAppBar1.Info");
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
            beepAppBar1.IsFrameless = false;
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
            beepAppBar1.Size = new Size(1173, 40);
            beepAppBar1.SlideFrom = Winform.Controls.SlideDirection.Left;
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
            // beepMenuAppBar1
            // 
            beepMenuAppBar1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepMenuAppBar1.AnimationDuration = 500;
            beepMenuAppBar1.AnimationType = Winform.Controls.DisplayAnimationType.None;
            beepMenuAppBar1.ApplyThemeToChilds = false;
            beepMenuAppBar1.BackColor = Color.FromArgb(240, 240, 240);
            beepMenuAppBar1.BadgeBackColor = Color.Red;
            beepMenuAppBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepMenuAppBar1.BadgeForeColor = Color.White;
            beepMenuAppBar1.BadgeShape = Winform.Controls.BadgeShape.Circle;
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
            beepMenuAppBar1.DrawingRect = new Rectangle(0, 0, 1173, 28);
            beepMenuAppBar1.Easing = Winform.Controls.EasingType.Linear;
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
            beepMenuAppBar1.GuidID = "22ec9a31-0e4b-4f20-b443-0acd90b7fe9c";
            beepMenuAppBar1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepMenuAppBar1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepMenuAppBar1.HoveredBackcolor = Color.Wheat;
            beepMenuAppBar1.HoverForeColor = Color.Black;
            beepMenuAppBar1.Id = -1;
            beepMenuAppBar1.ImageSize = 20;
            beepMenuAppBar1.InactiveBackColor = Color.Gray;
            beepMenuAppBar1.InactiveBorderColor = Color.Gray;
            beepMenuAppBar1.InactiveForeColor = Color.Black;
            beepMenuAppBar1.Info = (Desktop.Common.SimpleItem)resources.GetObject("beepMenuAppBar1.Info");
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
            beepMenuAppBar1.IsRoundedAffectedByTheme = true;
            beepMenuAppBar1.IsSelected = false;
            beepMenuAppBar1.IsShadowAffectedByTheme = true;
            beepMenuAppBar1.IsVisible = false;
            beepMenuAppBar1.Items = (List<object>)resources.GetObject("beepMenuAppBar1.Items");
            beepMenuAppBar1.LeftoffsetForDrawingRect = 0;
            beepMenuAppBar1.LinkedProperty = null;
            beepMenuAppBar1.Location = new Point(4, 44);
            beepMenuAppBar1.MenuItemHeight = 35;
            beepMenuAppBar1.MenuItemWidth = 60;
            beepMenuAppBar1.Name = "beepMenuAppBar1";
            beepMenuAppBar1.OverrideFontSize = Winform.Controls.TypeStyleFontSize.None;
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
            beepMenuAppBar1.Size = new Size(1173, 28);
            beepMenuAppBar1.SlideFrom = Winform.Controls.SlideDirection.Left;
            beepMenuAppBar1.StaticNotMoving = false;
            beepMenuAppBar1.TabIndex = 6;
            beepMenuAppBar1.TempBackColor = Color.Empty;
            beepMenuAppBar1.Text = "beepMenuAppBar1";
            beepMenuAppBar1.TextFont = new Font("Segoe UI", 9F);
            beepMenuAppBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepMenuAppBar1.ToolTipText = "";
            beepMenuAppBar1.TopoffsetForDrawingRect = 0;
            beepMenuAppBar1.UseGradientBackground = false;
            beepMenuAppBar1.UseThemeFont = true;
            // 
            // driversConfigViewModelBindingSource
            // 
            driversConfigViewModelBindingSource.DataMember = "ConnectionDriversConfigs";
            driversConfigViewModelBindingSource.DataSource = typeof(MVVM.ViewModels.BeepConfig.DriversConfigViewModel);
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(1181, 856);
            Controls.Add(beepTreeControl1);
            Controls.Add(beepMenuAppBar1);
            Controls.Add(beepAppBar1);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)driversConfigViewModelBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Controls.ITrees.BeepTreeView.BeepAppTree beepTreeControl1;
        private Controls.BeepAppBar beepAppBar1;
        private Controls.Containers.uc_Container uc_Container1;
        private Controls.MenuBar.BeepMenuAppBar beepMenuAppBar1;
        private BindingSource driversConfigViewModelBindingSource;
    }
}