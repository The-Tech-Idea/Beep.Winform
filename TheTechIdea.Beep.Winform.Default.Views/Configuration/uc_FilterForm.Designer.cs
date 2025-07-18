﻿

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    partial class uc_FilterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_FilterForm));
            Controls.Models.BeepRowConfig beepRowConfig1 = new Controls.Models.BeepRowConfig();
            beepLabel1 = new BeepLabel();
            beepNumericUpDown1 = new BeepNumericUpDown();
            beepTextBox1 = new BeepTextBox();
            beepSimpleGrid1 = new BeepSimpleGrid();
            SuspendLayout();
            // 
            // beepLabel1
            // 
            beepLabel1.AnimationDuration = 500;
            beepLabel1.AnimationType = DisplayAnimationType.None;
            beepLabel1.ApplyThemeOnImage = false;
            beepLabel1.ApplyThemeToChilds = false;
            beepLabel1.BackColor = Color.FromArgb(255, 255, 255);
            beepLabel1.BadgeBackColor = Color.Red;
            beepLabel1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepLabel1.BadgeForeColor = Color.White;
            beepLabel1.BadgeShape = BadgeShape.Circle;
            beepLabel1.BadgeText = "";
            beepLabel1.BlockID = null;
            beepLabel1.BorderColor = Color.Black;
            beepLabel1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepLabel1.BorderRadius = 3;
            beepLabel1.BorderStyle = BorderStyle.FixedSingle;
            beepLabel1.BorderThickness = 1;
            beepLabel1.BottomoffsetForDrawingRect = 0;
            beepLabel1.BoundProperty = "Text";
            beepLabel1.CanBeFocused = true;
            beepLabel1.CanBeHovered = false;
            beepLabel1.CanBePressed = true;
            beepLabel1.Category = Utilities.DbFieldCategory.String;
            beepLabel1.ComponentName = "beepLabel1";
            beepLabel1.DataSourceProperty = null;
            beepLabel1.DisabledBackColor = Color.White;
            beepLabel1.DisabledForeColor = Color.Black;
            beepLabel1.DrawingRect = new Rectangle(1, 1, 220, 41);
            beepLabel1.Easing = EasingType.Linear;
            beepLabel1.FieldID = null;
            beepLabel1.FocusBackColor = Color.White;
            beepLabel1.FocusBorderColor = Color.Gray;
            beepLabel1.FocusForeColor = Color.Black;
            beepLabel1.FocusIndicatorColor = Color.Blue;
            beepLabel1.Font = new Font("Arial", 12F);
            beepLabel1.ForeColor = Color.FromArgb(33, 37, 41);
            beepLabel1.Form = null;
            beepLabel1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepLabel1.GradientEndColor = Color.Gray;
            beepLabel1.GradientStartColor = Color.Gray;
            beepLabel1.GuidID = "b6a16bce-4329-420a-bcbf-68edc27af5cc";
            beepLabel1.HeaderSubheaderSpacing = 2;
            beepLabel1.HideText = false;
            beepLabel1.HitAreaEventOn = false;
            beepLabel1.HitTestControl = null;
            beepLabel1.HoverBackColor = Color.FromArgb(30, 140, 235);
            beepLabel1.HoverBorderColor = Color.Gray;
            beepLabel1.HoveredBackcolor = Color.Wheat;
            beepLabel1.HoverForeColor = Color.FromArgb(255, 255, 255);
            beepLabel1.Id = -1;
            beepLabel1.ImageAlign = ContentAlignment.MiddleLeft;
            beepLabel1.ImagePath = "C:\\Users\\f_ald\\OneDrive\\docs_pic\\IMG_1074.jpg";
            beepLabel1.InactiveBorderColor = Color.Gray;
            beepLabel1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepLabel1.Info");
            beepLabel1.IsAcceptButton = false;
            beepLabel1.IsBorderAffectedByTheme = true;
            beepLabel1.IsCancelButton = false;
            beepLabel1.IsChild = false;
            beepLabel1.IsCustomeBorder = false;
            beepLabel1.IsDefault = false;
            beepLabel1.IsDeleted = false;
            beepLabel1.IsDirty = false;
            beepLabel1.IsEditable = false;
            beepLabel1.IsFocused = false;
            beepLabel1.IsFrameless = false;
            beepLabel1.IsHovered = false;
            beepLabel1.IsNew = false;
            beepLabel1.IsPressed = false;
            beepLabel1.IsReadOnly = false;
            beepLabel1.IsRequired = false;
            beepLabel1.IsRounded = true;
            beepLabel1.IsRoundedAffectedByTheme = true;
            beepLabel1.IsSelected = false;
            beepLabel1.IsSelectedOptionOn = true;
            beepLabel1.IsShadowAffectedByTheme = true;
            beepLabel1.IsVisible = false;
            beepLabel1.Items = (List<object>)resources.GetObject("beepLabel1.Items");
            beepLabel1.LabelBackColor = Color.Empty;
            beepLabel1.LeftoffsetForDrawingRect = 0;
            beepLabel1.LinkedProperty = null;
            beepLabel1.Location = new Point(414, 17);
            beepLabel1.Margin = new Padding(0);
            beepLabel1.MaxImageSize = new Size(48, 48);
            beepLabel1.Multiline = false;
            beepLabel1.Name = "beepLabel1";
            beepLabel1.OverrideFontSize = TypeStyleFontSize.None;
            beepLabel1.Padding = new Padding(1);
            beepLabel1.ParentBackColor = Color.Empty;
            beepLabel1.ParentControl = null;
            beepLabel1.PressedBackColor = Color.White;
            beepLabel1.PressedBorderColor = Color.Gray;
            beepLabel1.PressedForeColor = Color.Gray;
            beepLabel1.RightoffsetForDrawingRect = 0;
            beepLabel1.SavedGuidID = null;
            beepLabel1.SavedID = null;
            beepLabel1.SelectedBackColor = Color.White;
            beepLabel1.SelectedForeColor = Color.Black;
            beepLabel1.SelectedValue = null;
            beepLabel1.ShadowColor = Color.Black;
            beepLabel1.ShadowOffset = 0;
            beepLabel1.ShadowOpacity = 0.5F;
            beepLabel1.ShowAllBorders = false;
            beepLabel1.ShowBottomBorder = false;
            beepLabel1.ShowFocusIndicator = false;
            beepLabel1.ShowLeftBorder = false;
            beepLabel1.ShowRightBorder = false;
            beepLabel1.ShowShadow = false;
            beepLabel1.ShowTopBorder = false;
            beepLabel1.Size = new Size(222, 43);
            beepLabel1.SlideFrom = SlideDirection.Left;
            beepLabel1.StaticNotMoving = false;
            beepLabel1.SubHeaderFont = new Font("Arial", 10F);
            beepLabel1.SubHeaderForeColor = Color.FromArgb(33, 37, 41);
            beepLabel1.SubHeaderText = "I.T. Info. Specilaist";
            beepLabel1.TabIndex = 12;
            beepLabel1.TempBackColor = Color.Empty;
            beepLabel1.Text = "Fahad Aldhubaib";
            beepLabel1.TextAlign = ContentAlignment.MiddleLeft;
            beepLabel1.TextFont = new Font("Arial", 12F);
            beepLabel1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepLabel1.Theme = "DefaultTheme";
            beepLabel1.ToolTipText = "";
            beepLabel1.TopoffsetForDrawingRect = 0;
            beepLabel1.UseGradientBackground = false;
            beepLabel1.UseScaledFont = false;
            beepLabel1.UseThemeFont = true;
            // 
            // beepNumericUpDown1
            // 
            beepNumericUpDown1.AnimationDuration = 500;
            beepNumericUpDown1.AnimationType = DisplayAnimationType.None;
            beepNumericUpDown1.ApplyThemeToChilds = false;
            beepNumericUpDown1.BackColor = Color.FromArgb(245, 245, 245);
            beepNumericUpDown1.BadgeBackColor = Color.Red;
            beepNumericUpDown1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepNumericUpDown1.BadgeForeColor = Color.White;
            beepNumericUpDown1.BadgeShape = BadgeShape.Circle;
            beepNumericUpDown1.BadgeText = "";
            beepNumericUpDown1.BlockID = null;
            beepNumericUpDown1.BorderColor = Color.FromArgb(173, 181, 189);
            beepNumericUpDown1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepNumericUpDown1.BorderRadius = 8;
            beepNumericUpDown1.BorderStyle = BorderStyle.FixedSingle;
            beepNumericUpDown1.BorderThickness = 1;
            beepNumericUpDown1.BottomoffsetForDrawingRect = 0;
            beepNumericUpDown1.BoundProperty = null;
            beepNumericUpDown1.CanBeFocused = true;
            beepNumericUpDown1.CanBeHovered = false;
            beepNumericUpDown1.CanBePressed = true;
            beepNumericUpDown1.Category = Utilities.DbFieldCategory.String;
            beepNumericUpDown1.ComponentName = "beepNumericUpDown1";
            beepNumericUpDown1.DataSourceProperty = null;
            beepNumericUpDown1.DisabledBackColor = Color.White;
            beepNumericUpDown1.DisabledForeColor = Color.Black;
            beepNumericUpDown1.DrawingRect = new Rectangle(0, 0, 73, 37);
            beepNumericUpDown1.Easing = EasingType.Linear;
            beepNumericUpDown1.FieldID = null;
            beepNumericUpDown1.FocusBackColor = Color.White;
            beepNumericUpDown1.FocusBorderColor = Color.Gray;
            beepNumericUpDown1.FocusForeColor = Color.Black;
            beepNumericUpDown1.FocusIndicatorColor = Color.Blue;
            beepNumericUpDown1.Form = null;
            beepNumericUpDown1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepNumericUpDown1.GradientEndColor = Color.FromArgb(255, 255, 255);
            beepNumericUpDown1.GradientStartColor = Color.FromArgb(245, 245, 245);
            beepNumericUpDown1.GuidID = "f3dc591a-13b0-4bf8-ac3d-ac97c7924bac";
            beepNumericUpDown1.HitAreaEventOn = false;
            beepNumericUpDown1.HitTestControl = null;
            beepNumericUpDown1.HoverBackColor = Color.White;
            beepNumericUpDown1.HoverBorderColor = Color.Gray;
            beepNumericUpDown1.HoveredBackcolor = Color.Wheat;
            beepNumericUpDown1.HoverForeColor = Color.Black;
            beepNumericUpDown1.Id = -1;
            beepNumericUpDown1.InactiveBorderColor = Color.Gray;
            beepNumericUpDown1.IncrementValue = new decimal(new int[] { 1, 0, 0, 0 });
            beepNumericUpDown1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepNumericUpDown1.Info");
            beepNumericUpDown1.IsAcceptButton = false;
            beepNumericUpDown1.IsBorderAffectedByTheme = true;
            beepNumericUpDown1.IsCancelButton = false;
            beepNumericUpDown1.IsChild = false;
            beepNumericUpDown1.IsCustomeBorder = false;
            beepNumericUpDown1.IsDefault = false;
            beepNumericUpDown1.IsDeleted = false;
            beepNumericUpDown1.IsDirty = false;
            beepNumericUpDown1.IsEditable = false;
            beepNumericUpDown1.IsFocused = false;
            beepNumericUpDown1.IsFrameless = false;
            beepNumericUpDown1.IsHovered = false;
            beepNumericUpDown1.IsNew = false;
            beepNumericUpDown1.IsPressed = false;
            beepNumericUpDown1.IsReadOnly = false;
            beepNumericUpDown1.IsRequired = false;
            beepNumericUpDown1.IsRounded = true;
            beepNumericUpDown1.IsRoundedAffectedByTheme = true;
            beepNumericUpDown1.IsSelected = false;
            beepNumericUpDown1.IsSelectedOptionOn = true;
            beepNumericUpDown1.IsShadowAffectedByTheme = true;
            beepNumericUpDown1.IsVisible = false;
            beepNumericUpDown1.Items = (List<object>)resources.GetObject("beepNumericUpDown1.Items");
            beepNumericUpDown1.LeftoffsetForDrawingRect = 0;
            beepNumericUpDown1.LinkedProperty = null;
            beepNumericUpDown1.Location = new Point(60, 41);
            beepNumericUpDown1.Margin = new Padding(0);
            beepNumericUpDown1.MaximumValue = new decimal(new int[] { 1000, 0, 0, 0 });
            beepNumericUpDown1.MinimumValue = new decimal(new int[] { 0, 0, 0, 0 });
            beepNumericUpDown1.Name = "beepNumericUpDown1";
            beepNumericUpDown1.OverrideFontSize = TypeStyleFontSize.None;
            beepNumericUpDown1.ParentBackColor = Color.Empty;
            beepNumericUpDown1.ParentControl = null;
            beepNumericUpDown1.PressedBackColor = Color.White;
            beepNumericUpDown1.PressedBorderColor = Color.Gray;
            beepNumericUpDown1.PressedForeColor = Color.Gray;
            beepNumericUpDown1.RightoffsetForDrawingRect = 0;
            beepNumericUpDown1.SavedGuidID = null;
            beepNumericUpDown1.SavedID = null;
            beepNumericUpDown1.SelectedBackColor = Color.White;
            beepNumericUpDown1.SelectedForeColor = Color.Black;
            beepNumericUpDown1.SelectedValue = null;
            beepNumericUpDown1.ShadowColor = Color.FromArgb(173, 181, 189);
            beepNumericUpDown1.ShadowOffset = 0;
            beepNumericUpDown1.ShadowOpacity = 0.5F;
            beepNumericUpDown1.ShowAllBorders = false;
            beepNumericUpDown1.ShowBottomBorder = false;
            beepNumericUpDown1.ShowFocusIndicator = false;
            beepNumericUpDown1.ShowLeftBorder = false;
            beepNumericUpDown1.ShowRightBorder = false;
            beepNumericUpDown1.ShowShadow = false;
            beepNumericUpDown1.ShowTopBorder = false;
            beepNumericUpDown1.Size = new Size(73, 37);
            beepNumericUpDown1.SlideFrom = SlideDirection.Left;
            beepNumericUpDown1.StaticNotMoving = false;
            beepNumericUpDown1.TabIndex = 14;
            beepNumericUpDown1.TempBackColor = Color.Empty;
            beepNumericUpDown1.Text = "beepNumericUpDown1";
            beepNumericUpDown1.Theme = "DefaultTheme";
            beepNumericUpDown1.ToolTipText = "";
            beepNumericUpDown1.TopoffsetForDrawingRect = 0;
            beepNumericUpDown1.UseGradientBackground = false;
            beepNumericUpDown1.UseThemeFont = true;
            beepNumericUpDown1.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // beepTextBox1
            // 
            beepTextBox1.AcceptsReturn = false;
            beepTextBox1.AcceptsTab = false;
            beepTextBox1.AnimationDuration = 500;
            beepTextBox1.AnimationType = DisplayAnimationType.None;
            beepTextBox1.ApplyThemeOnImage = false;
            beepTextBox1.ApplyThemeToChilds = false;
            beepTextBox1.AutoCompleteMode = AutoCompleteMode.None;
            beepTextBox1.AutoCompleteSource = AutoCompleteSource.None;
            beepTextBox1.BackColor = Color.FromArgb(255, 250, 240);
            beepTextBox1.BadgeBackColor = Color.Red;
            beepTextBox1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepTextBox1.BadgeForeColor = Color.White;
            beepTextBox1.BadgeShape = BadgeShape.Circle;
            beepTextBox1.BadgeText = "";
            beepTextBox1.BlockID = null;
            beepTextBox1.BorderColor = Color.FromArgb(184, 134, 11);
            beepTextBox1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepTextBox1.BorderRadius = 3;
            beepTextBox1.BorderStyle = BorderStyle.FixedSingle;
            beepTextBox1.BorderThickness = 1;
            beepTextBox1.BottomoffsetForDrawingRect = 0;
            beepTextBox1.BoundProperty = "Text";
            beepTextBox1.CanBeFocused = true;
            beepTextBox1.CanBeHovered = false;
            beepTextBox1.CanBePressed = true;
            beepTextBox1.Category = Utilities.DbFieldCategory.String;
            beepTextBox1.ComponentName = "beepTextBox1";
            beepTextBox1.CustomMask = "";
            beepTextBox1.DataSourceProperty = null;
            beepTextBox1.DateFormat = "MM/dd/yyyy HH:mm:ss";
            beepTextBox1.DateTimeFormat = "MM/dd/yyyy HH:mm:ss";
            beepTextBox1.DisabledBackColor = Color.FromArgb(200, 200, 200);
            beepTextBox1.DisabledForeColor = Color.FromArgb(150, 150, 150);
            beepTextBox1.DrawingRect = new Rectangle(4, 4, 192, 20);
            beepTextBox1.Easing = EasingType.Linear;
            beepTextBox1.FieldID = null;
            beepTextBox1.FocusBackColor = Color.White;
            beepTextBox1.FocusBorderColor = Color.Gray;
            beepTextBox1.FocusForeColor = Color.Black;
            beepTextBox1.FocusIndicatorColor = Color.Blue;
            beepTextBox1.Font = new Font("Garamond", 11F);
            beepTextBox1.ForeColor = Color.FromArgb(51, 51, 51);
            beepTextBox1.Form = null;
            beepTextBox1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepTextBox1.GradientEndColor = Color.FromArgb(230, 230, 250);
            beepTextBox1.GradientStartColor = Color.FromArgb(245, 245, 220);
            beepTextBox1.GuidID = "76d52a8f-ce6c-40ce-933c-c57583353d9e";
            beepTextBox1.HideSelection = true;
            beepTextBox1.HitAreaEventOn = false;
            beepTextBox1.HitTestControl = null;
            beepTextBox1.HoverBackColor = Color.FromArgb(245, 240, 230);
            beepTextBox1.HoverBorderColor = Color.Gray;
            beepTextBox1.HoveredBackcolor = Color.Wheat;
            beepTextBox1.HoverForeColor = Color.FromArgb(51, 51, 51);
            beepTextBox1.Id = -1;
            beepTextBox1.ImageAlign = ContentAlignment.MiddleLeft;
            beepTextBox1.ImagePath = null;
            beepTextBox1.InactiveBorderColor = Color.Gray;
            beepTextBox1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepTextBox1.Info");
            beepTextBox1.IsAcceptButton = false;
            beepTextBox1.IsBorderAffectedByTheme = false;
            beepTextBox1.IsCancelButton = false;
            beepTextBox1.IsChild = false;
            beepTextBox1.IsCustomeBorder = false;
            beepTextBox1.IsDefault = false;
            beepTextBox1.IsDeleted = false;
            beepTextBox1.IsDirty = false;
            beepTextBox1.IsEditable = false;
            beepTextBox1.IsFocused = false;
            beepTextBox1.IsFrameless = false;
            beepTextBox1.IsHovered = false;
            beepTextBox1.IsNew = false;
            beepTextBox1.IsPressed = false;
            beepTextBox1.IsReadOnly = false;
            beepTextBox1.IsRequired = false;
            beepTextBox1.IsRounded = true;
            beepTextBox1.IsRoundedAffectedByTheme = true;
            beepTextBox1.IsSelected = false;
            beepTextBox1.IsSelectedOptionOn = false;
            beepTextBox1.IsShadowAffectedByTheme = false;
            beepTextBox1.IsVisible = false;
            beepTextBox1.Items = (List<object>)resources.GetObject("beepTextBox1.Items");
            beepTextBox1.LeftoffsetForDrawingRect = 0;
            beepTextBox1.LinkedProperty = null;
            beepTextBox1.Location = new Point(90, 95);
            beepTextBox1.MaskFormat = TextBoxMaskFormat.None;
            beepTextBox1.MaxImageSize = new Size(16, 16);
            beepTextBox1.Modified = false;
            beepTextBox1.Multiline = false;
            beepTextBox1.Name = "beepTextBox1";
            beepTextBox1.OnlyCharacters = false;
            beepTextBox1.OnlyDigits = false;
            beepTextBox1.OverrideFontSize = TypeStyleFontSize.None;
            beepTextBox1.Padding = new Padding(3);
            beepTextBox1.ParentBackColor = Color.Empty;
            beepTextBox1.ParentControl = null;
            beepTextBox1.PasswordChar = '\0';
            beepTextBox1.PlaceholderText = "sdsdsd";
            beepTextBox1.PressedBackColor = Color.White;
            beepTextBox1.PressedBorderColor = Color.Gray;
            beepTextBox1.PressedForeColor = Color.Gray;
            beepTextBox1.ReadOnly = false;
            beepTextBox1.RightoffsetForDrawingRect = 0;
            beepTextBox1.SavedGuidID = null;
            beepTextBox1.SavedID = null;
            beepTextBox1.ScrollBars = ScrollBars.None;
            beepTextBox1.SelectedBackColor = Color.FromArgb(255, 250, 240);
            beepTextBox1.SelectedForeColor = Color.FromArgb(51, 51, 51);
            beepTextBox1.SelectedValue = null;
            beepTextBox1.SelectionStart = 0;
            beepTextBox1.ShadowColor = Color.FromArgb(184, 134, 11);
            beepTextBox1.ShadowOffset = 0;
            beepTextBox1.ShadowOpacity = 0.5F;
            beepTextBox1.ShowAllBorders = true;
            beepTextBox1.ShowBottomBorder = true;
            beepTextBox1.ShowFocusIndicator = false;
            beepTextBox1.ShowLeftBorder = true;
            beepTextBox1.ShowRightBorder = true;
            beepTextBox1.ShowScrollbars = false;
            beepTextBox1.ShowShadow = false;
            beepTextBox1.ShowTopBorder = true;
            beepTextBox1.ShowVerticalScrollBar = false;
            beepTextBox1.Size = new Size(200, 28);
            beepTextBox1.SlideFrom = SlideDirection.Left;
            beepTextBox1.StaticNotMoving = false;
            beepTextBox1.TabIndex = 15;
            beepTextBox1.TempBackColor = Color.Empty;
            beepTextBox1.TextAlignment = HorizontalAlignment.Left;
            beepTextBox1.TextFont = new Font("Garamond", 11F);
            beepTextBox1.TextImageRelation = TextImageRelation.ImageBeforeText;
            beepTextBox1.Theme = "DefaultTheme";
            beepTextBox1.TimeFormat = "HH:mm:ss";
            beepTextBox1.ToolTipText = "";
            beepTextBox1.TopoffsetForDrawingRect = 0;
            beepTextBox1.UseGradientBackground = false;
            beepTextBox1.UseSystemPasswordChar = false;
            beepTextBox1.UseThemeFont = true;
            beepTextBox1.WordWrap = true;
            // 
            // beepSimpleGrid1
            // 
            beepRowConfig1.DisplayIndex = -1;
            beepRowConfig1.Height = 25;
            beepRowConfig1.Id = "93ade503-07ac-481d-8db4-63214a3dc7ab";
            beepRowConfig1.Index = 1;
            beepRowConfig1.IsAggregation = true;
            beepRowConfig1.IsDataLoaded = false;
            beepRowConfig1.IsDeleted = false;
            beepRowConfig1.IsDirty = false;
            beepRowConfig1.IsEditable = false;
            beepRowConfig1.IsNew = false;
            beepRowConfig1.IsReadOnly = false;
            beepRowConfig1.IsSelected = false;
            beepRowConfig1.IsVisible = false;
            beepRowConfig1.OldDisplayIndex = 0;
            beepRowConfig1.RowData = null;
            beepRowConfig1.UpperX = 0;
            beepRowConfig1.UpperY = 0;
            beepRowConfig1.Width = 100;
            beepSimpleGrid1.aggregationRow = beepRowConfig1;
            beepSimpleGrid1.AnimationDuration = 500;
            beepSimpleGrid1.AnimationType = DisplayAnimationType.None;
            beepSimpleGrid1.ApplyThemeToChilds = false;
            beepSimpleGrid1.BackColor = Color.FromArgb(255, 255, 255);
            beepSimpleGrid1.BadgeBackColor = Color.Red;
            beepSimpleGrid1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepSimpleGrid1.BadgeForeColor = Color.White;
            beepSimpleGrid1.BadgeShape = BadgeShape.Circle;
            beepSimpleGrid1.BadgeText = "";
            beepSimpleGrid1.BlockID = null;
            beepSimpleGrid1.BorderColor = Color.Black;
            beepSimpleGrid1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSimpleGrid1.BorderRadius = 8;
            beepSimpleGrid1.BorderStyle = BorderStyle.FixedSingle;
            beepSimpleGrid1.BorderThickness = 1;
            beepSimpleGrid1.BottomoffsetForDrawingRect = 0;
            beepSimpleGrid1.BoundProperty = null;
            beepSimpleGrid1.CanBeFocused = true;
            beepSimpleGrid1.CanBeHovered = false;
            beepSimpleGrid1.CanBePressed = true;
            beepSimpleGrid1.Category = Utilities.DbFieldCategory.String;
            beepSimpleGrid1.ColumnHeaderFont = new Font("Arial", 8F);
            beepSimpleGrid1.ColumnHeaderHeight = 40;
            beepSimpleGrid1.Columns = (List<Controls.Models.BeepColumnConfig>)resources.GetObject("beepSimpleGrid1.Columns");
            beepSimpleGrid1.ComponentName = "beepSimpleGrid1";
            beepSimpleGrid1.DataNavigator = null;
            beepSimpleGrid1.DataSource = null;
            beepSimpleGrid1.DataSourceProperty = null;
            beepSimpleGrid1.DataSourceType = GridDataSourceType.Fixed;
            beepSimpleGrid1.DefaultColumnHeaderWidth = 50;
            beepSimpleGrid1.DisabledBackColor = Color.White;
            beepSimpleGrid1.DisabledForeColor = Color.Black;
            beepSimpleGrid1.DrawingRect = new Rectangle(0, 0, 749, 505);
            beepSimpleGrid1.Easing = EasingType.Linear;
            beepSimpleGrid1.EntityName = null;
            beepSimpleGrid1.FieldID = null;
            beepSimpleGrid1.FocusBackColor = Color.FromArgb(255, 255, 255);
            beepSimpleGrid1.FocusBorderColor = Color.Gray;
            beepSimpleGrid1.FocusForeColor = Color.FromArgb(33, 37, 41);
            beepSimpleGrid1.FocusIndicatorColor = Color.Blue;
            beepSimpleGrid1.ForeColor = Color.FromArgb(33, 37, 41);
            beepSimpleGrid1.Form = null;
            beepSimpleGrid1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSimpleGrid1.GradientEndColor = Color.Gray;
            beepSimpleGrid1.GradientStartColor = Color.Gray;
            beepSimpleGrid1.GuidID = "8f70e6e9-14bf-41e8-9d73-94248d65be2d";
            beepSimpleGrid1.HitAreaEventOn = false;
            beepSimpleGrid1.HitTestControl = null;
            beepSimpleGrid1.HoverBackColor = Color.FromArgb(255, 255, 255);
            beepSimpleGrid1.HoverBorderColor = Color.Gray;
            beepSimpleGrid1.HoveredBackcolor = Color.Wheat;
            beepSimpleGrid1.HoverForeColor = Color.FromArgb(33, 37, 41);
            beepSimpleGrid1.Id = -1;
            beepSimpleGrid1.InactiveBorderColor = Color.Gray;
            beepSimpleGrid1.Info = (Controls.Models.SimpleItem)resources.GetObject("beepSimpleGrid1.Info");
            beepSimpleGrid1.IsAcceptButton = false;
            beepSimpleGrid1.IsBorderAffectedByTheme = true;
            beepSimpleGrid1.IsCancelButton = false;
            beepSimpleGrid1.IsChild = false;
            beepSimpleGrid1.IsCustomeBorder = false;
            beepSimpleGrid1.IsDefault = false;
            beepSimpleGrid1.IsDeleted = false;
            beepSimpleGrid1.IsDirty = false;
            beepSimpleGrid1.IsEditable = false;
            beepSimpleGrid1.IsFocused = false;
            beepSimpleGrid1.IsFrameless = false;
            beepSimpleGrid1.IsHovered = false;
            beepSimpleGrid1.IsLogging = false;
            beepSimpleGrid1.IsNew = false;
            beepSimpleGrid1.IsPressed = false;
            beepSimpleGrid1.IsReadOnly = false;
            beepSimpleGrid1.IsRequired = false;
            beepSimpleGrid1.IsRounded = true;
            beepSimpleGrid1.IsRoundedAffectedByTheme = true;
            beepSimpleGrid1.IsSelected = false;
            beepSimpleGrid1.IsSelectedOptionOn = false;
            beepSimpleGrid1.IsShadowAffectedByTheme = true;
            beepSimpleGrid1.IsVisible = false;
            beepSimpleGrid1.Items = (List<object>)resources.GetObject("beepSimpleGrid1.Items");
            beepSimpleGrid1.LeftoffsetForDrawingRect = 0;
            beepSimpleGrid1.LinkedProperty = null;
            beepSimpleGrid1.Location = new Point(120, 176);
            beepSimpleGrid1.Name = "beepSimpleGrid1";
            beepSimpleGrid1.OverrideFontSize = TypeStyleFontSize.None;
            beepSimpleGrid1.ParentBackColor = Color.Empty;
            beepSimpleGrid1.ParentControl = null;
            beepSimpleGrid1.PercentageText = "";
            beepSimpleGrid1.PressedBackColor = Color.White;
            beepSimpleGrid1.PressedBorderColor = Color.Gray;
            beepSimpleGrid1.PressedForeColor = Color.Gray;
            beepSimpleGrid1.QueryFunction = null;
            beepSimpleGrid1.QueryFunctionName = null;
            beepSimpleGrid1.RightoffsetForDrawingRect = 0;
            beepSimpleGrid1.RowHeight = 25;
            beepSimpleGrid1.SavedGuidID = null;
            beepSimpleGrid1.SavedID = null;
            beepSimpleGrid1.SelectedBackColor = Color.FromArgb(255, 255, 255);
            beepSimpleGrid1.SelectedForeColor = Color.FromArgb(33, 37, 41);
            beepSimpleGrid1.SelectedValue = null;
            beepSimpleGrid1.SelectionColumnWidth = 30;
            beepSimpleGrid1.ShadowColor = Color.Black;
            beepSimpleGrid1.ShadowOffset = 0;
            beepSimpleGrid1.ShadowOpacity = 0.5F;
            beepSimpleGrid1.ShowAggregationRow = false;
            beepSimpleGrid1.ShowAllBorders = false;
            beepSimpleGrid1.ShowBottomBorder = false;
            beepSimpleGrid1.ShowCheckboxes = false;
            beepSimpleGrid1.ShowColumnHeaders = true;
            beepSimpleGrid1.ShowFilter = false;
            beepSimpleGrid1.ShowFocusIndicator = false;
            beepSimpleGrid1.ShowFooter = false;
            beepSimpleGrid1.ShowHeaderPanel = true;
            beepSimpleGrid1.ShowHeaderPanelBorder = true;
            beepSimpleGrid1.ShowHorizontalGridLines = true;
            beepSimpleGrid1.ShowHorizontalScrollBar = true;
            beepSimpleGrid1.ShowLeftBorder = false;
            beepSimpleGrid1.ShowNavigator = true;
            beepSimpleGrid1.ShowRightBorder = false;
            beepSimpleGrid1.ShowRowHeaders = true;
            beepSimpleGrid1.ShowRowNumbers = true;
            beepSimpleGrid1.ShowShadow = false;
            beepSimpleGrid1.ShowSortIcons = true;
            beepSimpleGrid1.ShowTopBorder = false;
            beepSimpleGrid1.ShowVerticalGridLines = true;
            beepSimpleGrid1.ShowVerticalScrollBar = true;
            beepSimpleGrid1.Size = new Size(749, 505);
            beepSimpleGrid1.SlideFrom = SlideDirection.Left;
            beepSimpleGrid1.StaticNotMoving = false;
            beepSimpleGrid1.TabIndex = 16;
            beepSimpleGrid1.TempBackColor = Color.Empty;
            beepSimpleGrid1.Text = "beepSimpleGrid1";
            beepSimpleGrid1.TextImageRelation = TextImageRelation.ImageAboveText;
            beepSimpleGrid1.Theme = "DefaultTheme";
            beepSimpleGrid1.TitleHeaderImage = "simpleinfoapps.svg";
            beepSimpleGrid1.TitleText = "Simple BeepGrid";
            beepSimpleGrid1.TitleTextFont = new Font("Segoe UI", 9F);
            beepSimpleGrid1.ToolTipText = "";
            beepSimpleGrid1.TopoffsetForDrawingRect = 0;
            beepSimpleGrid1.UpdateLog = (Dictionary<DateTime, Editor.EntityUpdateInsertLog>)resources.GetObject("beepSimpleGrid1.UpdateLog");
            beepSimpleGrid1.UseGradientBackground = false;
            beepSimpleGrid1.UseThemeFont = true;
            beepSimpleGrid1.XOffset = 0;
            // 
            // uc_FilterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            MainTemplatePanel.Controls.Add(beepSimpleGrid1);
            MainTemplatePanel.Controls.Add(beepTextBox1);
            MainTemplatePanel.Controls.Add(beepNumericUpDown1);
            MainTemplatePanel.Controls.Add(beepLabel1);
            Name = "uc_FilterForm";
            Size = new Size(1044, 738);
            ResumeLayout(false);
        }

        #endregion
        private BeepLabel beepLabel1;
        private BeepNumericUpDown beepNumericUpDown1;
        private BeepTextBox beepTextBox1;
        private BeepSimpleGrid beepSimpleGrid1;
    }
}
