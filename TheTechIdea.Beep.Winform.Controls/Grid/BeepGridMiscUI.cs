using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
     public static class BeepGridMiscUI
    {


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        public static void InitializeComponent(Control control,DataGridView Grid)
        {
             MainSplitContainer = new SplitContainer();
            HeaderPanel = new Panel();
            ColumnHeaderPanel = new Panel();
            FilterColumnHeaderPanel = new Panel();
            ScrollHorizPanel = new Panel();
            ScrollVerticalPanel = new Panel();
            FooterPanel = new Panel();
            TotalsPanel = new Panel();
            DataNavigatorPanel = new Panel();
            beepScrollBar1 = new BeepScrollBar();
            beepScrollBar2 = new BeepScrollBar();
            DataGridViewPanel = new Panel();
       
            MainSplitContainer.Panel1.SuspendLayout();
            MainSplitContainer.Panel2.SuspendLayout();
            MainSplitContainer.SuspendLayout();
            ScrollHorizPanel.SuspendLayout();
            ScrollVerticalPanel.SuspendLayout();
            FooterPanel.SuspendLayout();
            control.SuspendLayout();
            // 
            // MainSplitContainer
            // 
            MainSplitContainer.Dock = DockStyle.Fill;
            MainSplitContainer.Location = new Point(5, 5);
            MainSplitContainer.Name = "MainSplitContainer";
            MainSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // MainSplitContainer.Panel1
            // 
            MainSplitContainer.Panel1.Controls.Add(HeaderPanel);
            MainSplitContainer.Panel1.Controls.Add(ColumnHeaderPanel);
            MainSplitContainer.Panel1.Controls.Add(FilterColumnHeaderPanel);
            // 
            // MainSplitContainer.Panel2
            // 
            MainSplitContainer.Panel2.Controls.Add(DataGridViewPanel);
            MainSplitContainer.Panel2.Controls.Add(ScrollHorizPanel);
            MainSplitContainer.Panel2.Controls.Add(ScrollVerticalPanel);
            MainSplitContainer.Panel2.Controls.Add(FooterPanel);

            MainSplitContainer.Size = new Size(796, 614);
            MainSplitContainer.SplitterDistance = 135;
            MainSplitContainer.TabIndex = 0;

            // 
            // HeaderPanel
            // 
            HeaderPanel.Dock = DockStyle.Fill;
            HeaderPanel.Location = new Point(0, 0);
            HeaderPanel.Name = "HeaderPanel";
            HeaderPanel.Size = new Size(796, 75);
            HeaderPanel.TabIndex = 0;
            // 
            // ColumnHeaderPanel
            // 
            ColumnHeaderPanel.Dock = DockStyle.Bottom;
            ColumnHeaderPanel.Location = new Point(0, 75);
            ColumnHeaderPanel.Name = "ColumnHeaderPanel";
            ColumnHeaderPanel.Size = new Size(796, 30);
            ColumnHeaderPanel.TabIndex = 2;
            // 
            // FilterColumnHeaderPanel
            // 
            FilterColumnHeaderPanel.Dock = DockStyle.Bottom;
            FilterColumnHeaderPanel.Location = new Point(0, 105);
            FilterColumnHeaderPanel.Name = "FilterColumnHeaderPanel";
            FilterColumnHeaderPanel.Size = new Size(796, 30);
            FilterColumnHeaderPanel.TabIndex = 1;
            // 
            // ScrollHorizPanel
            // 
            ScrollHorizPanel.Controls.Add(beepScrollBar2);
            ScrollHorizPanel.Dock = DockStyle.Bottom;
            ScrollHorizPanel.Location = new Point(0, 377);
            ScrollHorizPanel.Name = "ScrollHorizPanel";
            ScrollHorizPanel.Size = new Size(760, 24);
            ScrollHorizPanel.TabIndex = 1;
            // 
            // ScrollVerticalPanel
            // 
            ScrollVerticalPanel.Controls.Add(beepScrollBar1);
            ScrollVerticalPanel.Dock = DockStyle.Right;
            ScrollVerticalPanel.Location = new Point(760, 0);
            ScrollVerticalPanel.Name = "ScrollVerticalPanel";
            ScrollVerticalPanel.Size = new Size(36, 401);
            ScrollVerticalPanel.TabIndex = 0;
            // 
            // FooterPanel
            // 
            FooterPanel.Controls.Add(TotalsPanel);
            FooterPanel.Controls.Add(DataNavigatorPanel);
            FooterPanel.Dock = DockStyle.Bottom;
            FooterPanel.Location = new Point(0, 401);
            FooterPanel.Name = "FooterPanel";
            FooterPanel.Size = new Size(796, 74);
            FooterPanel.TabIndex = 0;
            // 
            // TotalsPanel
            // 
            TotalsPanel.Dock = DockStyle.Bottom;
            TotalsPanel.Location = new Point(0, 3);
            TotalsPanel.Name = "TotalsPanel";
            TotalsPanel.Size = new Size(796, 37);
            TotalsPanel.TabIndex = 1;
            // 
            // DataNavigatorPanel
            // 
            DataNavigatorPanel.Dock = DockStyle.Bottom;
            DataNavigatorPanel.Location = new Point(0, 40);
            DataNavigatorPanel.Name = "DataNavigatorPanel";
            DataNavigatorPanel.Size = new Size(796, 34);
            DataNavigatorPanel.TabIndex = 0;
            // 
            // beepScrollBar1
            // 
            beepScrollBar1.ActiveBackColor = Color.Gray;
            beepScrollBar1.AnimationDuration = 500;
            beepScrollBar1.AnimationType = DisplayAnimationType.None;
            beepScrollBar1.ApplyThemeToChilds = true;
            beepScrollBar1.BadgeBackColor = Color.Red;
            beepScrollBar1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepScrollBar1.BadgeForeColor = Color.White;
            beepScrollBar1.BadgeText = "";
            beepScrollBar1.BlockID = null;
            beepScrollBar1.BorderColor = Color.Black;
            beepScrollBar1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepScrollBar1.BorderRadius = 3;
            beepScrollBar1.BorderStyle = BorderStyle.FixedSingle;
            beepScrollBar1.BorderThickness = 1;
            beepScrollBar1.BottomoffsetForDrawingRect = 0;
            beepScrollBar1.BoundProperty = null;
            beepScrollBar1.CanBeFocused = true;
            beepScrollBar1.CanBeHovered = false;
            beepScrollBar1.CanBePressed = true;
            beepScrollBar1.Category = Utilities.DbFieldCategory.String;
            beepScrollBar1.ComponentName = "beepScrollBar1";
            beepScrollBar1.DataContext = null;
            beepScrollBar1.DataSourceProperty = null;
            beepScrollBar1.DisabledBackColor = Color.Gray;
            beepScrollBar1.DisabledForeColor = Color.Empty;
            beepScrollBar1.Dock = DockStyle.Fill;
            beepScrollBar1.DrawingRect = new Rectangle(1, 1, 34, 399);
            beepScrollBar1.Easing = EasingType.Linear;
            beepScrollBar1.FieldID = null;
            beepScrollBar1.FocusBackColor = Color.Gray;
            beepScrollBar1.FocusBorderColor = Color.Gray;
            beepScrollBar1.FocusForeColor = Color.Black;
            beepScrollBar1.FocusIndicatorColor = Color.Blue;
            beepScrollBar1.Form = null;
            beepScrollBar1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepScrollBar1.GradientEndColor = Color.Gray;
            beepScrollBar1.GradientStartColor = Color.Gray;
            beepScrollBar1.GuidID = "fd1f650b-ce07-4f84-9c77-abc82fed9e7d";
            beepScrollBar1.HoverBackColor = Color.Gray;
            beepScrollBar1.HoverBorderColor = Color.Gray;
            beepScrollBar1.HoveredBackcolor = Color.Wheat;
            beepScrollBar1.HoverForeColor = Color.Black;
            beepScrollBar1.Id = -1;
            beepScrollBar1.InactiveBackColor = Color.Gray;
            beepScrollBar1.InactiveBorderColor = Color.Gray;
            beepScrollBar1.InactiveForeColor = Color.Black;
            beepScrollBar1.IsAcceptButton = false;
            beepScrollBar1.IsBorderAffectedByTheme = true;
            beepScrollBar1.IsCancelButton = false;
            beepScrollBar1.IsChild = false;
            beepScrollBar1.IsCustomeBorder = false;
            beepScrollBar1.IsDefault = false;
            beepScrollBar1.IsDeleted = false;
            beepScrollBar1.IsDirty = false;
            beepScrollBar1.IsEditable = false;
            beepScrollBar1.IsFocused = false;
            beepScrollBar1.IsFramless = false;
            beepScrollBar1.IsHovered = false;
            beepScrollBar1.IsNew = false;
            beepScrollBar1.IsPressed = false;
            beepScrollBar1.IsReadOnly = false;
            beepScrollBar1.IsRequired = false;
            beepScrollBar1.IsRounded = true;
            beepScrollBar1.IsRoundedAffectedByTheme = true;
            beepScrollBar1.IsSelected = false;
            beepScrollBar1.IsShadowAffectedByTheme = true;
            beepScrollBar1.IsVisible = false;
            beepScrollBar1.LeftoffsetForDrawingRect = 0;
            beepScrollBar1.LinkedProperty = null;
            beepScrollBar1.Location = new Point(0, 0);
            beepScrollBar1.Name = "beepScrollBar1";
            beepScrollBar1.OverrideFontSize = TypeStyleFontSize.None;
            beepScrollBar1.ParentBackColor = Color.Empty;
            beepScrollBar1.ParentControl = null;
            beepScrollBar1.PressedBackColor = Color.Gray;
            beepScrollBar1.PressedBorderColor = Color.Gray;
            beepScrollBar1.PressedForeColor = Color.Black;
            beepScrollBar1.RightoffsetForDrawingRect = 0;
            beepScrollBar1.SavedGuidID = null;
            beepScrollBar1.SavedID = null;
            beepScrollBar1.ShadowColor = Color.Black;
            beepScrollBar1.ShadowOffset = 0;
            beepScrollBar1.ShadowOpacity = 0.5F;
            beepScrollBar1.ShowAllBorders = true;
            beepScrollBar1.ShowBottomBorder = true;
            beepScrollBar1.ShowFocusIndicator = false;
            beepScrollBar1.ShowLeftBorder = true;
            beepScrollBar1.ShowRightBorder = true;
            beepScrollBar1.ShowShadow = false;
            beepScrollBar1.ShowTopBorder = true;
            beepScrollBar1.Size = new Size(36, 401);
            beepScrollBar1.SlideFrom = SlideDirection.Left;
            beepScrollBar1.StaticNotMoving = false;
            beepScrollBar1.TabIndex = 2;
            beepScrollBar1.TempBackColor = Color.Empty;
            beepScrollBar1.Text = "beepScrollBar1";
            beepScrollBar1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepScrollBar1.ToolTipText = "";
            beepScrollBar1.TopoffsetForDrawingRect = 0;
            beepScrollBar1.UseGradientBackground = false;
            beepScrollBar1.UseThemeFont = true;
            // 
            // beepScrollBar2
            // 
            beepScrollBar2.ActiveBackColor = Color.Gray;
            beepScrollBar2.AnimationDuration = 500;
            beepScrollBar2.AnimationType = DisplayAnimationType.None;
            beepScrollBar2.ApplyThemeToChilds = true;
            beepScrollBar2.BadgeBackColor = Color.Red;
            beepScrollBar2.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepScrollBar2.BadgeForeColor = Color.White;
            beepScrollBar2.BadgeText = "";
            beepScrollBar2.BlockID = null;
            beepScrollBar2.BorderColor = Color.Black;
            beepScrollBar2.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepScrollBar2.BorderRadius = 3;
            beepScrollBar2.BorderStyle = BorderStyle.FixedSingle;
            beepScrollBar2.BorderThickness = 1;
            beepScrollBar2.BottomoffsetForDrawingRect = 0;
            beepScrollBar2.BoundProperty = null;
            beepScrollBar2.CanBeFocused = true;
            beepScrollBar2.CanBeHovered = false;
            beepScrollBar2.CanBePressed = true;
            beepScrollBar2.Category = Utilities.DbFieldCategory.String;
            beepScrollBar2.ComponentName = "beepScrollBar2";
            beepScrollBar2.DataContext = null;
            beepScrollBar2.DataSourceProperty = null;
            beepScrollBar2.DisabledBackColor = Color.Gray;
            beepScrollBar2.DisabledForeColor = Color.Empty;
            beepScrollBar2.Dock = DockStyle.Fill;
            beepScrollBar2.DrawingRect = new Rectangle(1, 1, 758, 22);
            beepScrollBar2.Easing = EasingType.Linear;
            beepScrollBar2.FieldID = null;
            beepScrollBar2.FocusBackColor = Color.Gray;
            beepScrollBar2.FocusBorderColor = Color.Gray;
            beepScrollBar2.FocusForeColor = Color.Black;
            beepScrollBar2.FocusIndicatorColor = Color.Blue;
            beepScrollBar2.Form = null;
            beepScrollBar2.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepScrollBar2.GradientEndColor = Color.Gray;
            beepScrollBar2.GradientStartColor = Color.Gray;
            beepScrollBar2.GuidID = "c9958e55-5e49-417f-8328-0a5128f579b2";
            beepScrollBar2.HoverBackColor = Color.Gray;
            beepScrollBar2.HoverBorderColor = Color.Gray;
            beepScrollBar2.HoveredBackcolor = Color.Wheat;
            beepScrollBar2.HoverForeColor = Color.Black;
            beepScrollBar2.Id = -1;
            beepScrollBar2.InactiveBackColor = Color.Gray;
            beepScrollBar2.InactiveBorderColor = Color.Gray;
            beepScrollBar2.InactiveForeColor = Color.Black;
            beepScrollBar2.IsAcceptButton = false;
            beepScrollBar2.IsBorderAffectedByTheme = true;
            beepScrollBar2.IsCancelButton = false;
            beepScrollBar2.IsChild = false;
            beepScrollBar2.IsCustomeBorder = false;
            beepScrollBar2.IsDefault = false;
            beepScrollBar2.IsDeleted = false;
            beepScrollBar2.IsDirty = false;
            beepScrollBar2.IsEditable = false;
            beepScrollBar2.IsFocused = false;
            beepScrollBar2.IsFramless = false;
            beepScrollBar2.IsHovered = false;
            beepScrollBar2.IsNew = false;
            beepScrollBar2.IsPressed = false;
            beepScrollBar2.IsReadOnly = false;
            beepScrollBar2.IsRequired = false;
            beepScrollBar2.IsRounded = true;
            beepScrollBar2.IsRoundedAffectedByTheme = true;
            beepScrollBar2.IsSelected = false;
            beepScrollBar2.IsShadowAffectedByTheme = true;
            beepScrollBar2.IsVisible = false;
            beepScrollBar2.LeftoffsetForDrawingRect = 0;
            beepScrollBar2.LinkedProperty = null;
            beepScrollBar2.Location = new Point(0, 0);
            beepScrollBar2.Name = "beepScrollBar2";
            beepScrollBar2.OverrideFontSize = TypeStyleFontSize.None;
            beepScrollBar2.ParentBackColor = Color.Empty;
            beepScrollBar2.ParentControl = null;
            beepScrollBar2.PressedBackColor = Color.Gray;
            beepScrollBar2.PressedBorderColor = Color.Gray;
            beepScrollBar2.PressedForeColor = Color.Black;
            beepScrollBar2.RightoffsetForDrawingRect = 0;
            beepScrollBar2.SavedGuidID = null;
            beepScrollBar2.SavedID = null;
            beepScrollBar2.ScrollOrientation = Orientation.Horizontal;
            beepScrollBar2.ShadowColor = Color.Black;
            beepScrollBar2.ShadowOffset = 0;
            beepScrollBar2.ShadowOpacity = 0.5F;
            beepScrollBar2.ShowAllBorders = true;
            beepScrollBar2.ShowBottomBorder = true;
            beepScrollBar2.ShowFocusIndicator = false;
            beepScrollBar2.ShowLeftBorder = true;
            beepScrollBar2.ShowRightBorder = true;
            beepScrollBar2.ShowShadow = false;
            beepScrollBar2.ShowTopBorder = true;
            beepScrollBar2.Size = new Size(760, 24);
            beepScrollBar2.SlideFrom = SlideDirection.Left;
            beepScrollBar2.StaticNotMoving = false;
            beepScrollBar2.TabIndex = 0;
            beepScrollBar2.TempBackColor = Color.Empty;
            beepScrollBar2.Text = "beepScrollBar2";
            beepScrollBar2.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepScrollBar2.ToolTipText = "";
            beepScrollBar2.TopoffsetForDrawingRect = 0;
            beepScrollBar2.UseGradientBackground = false;
            beepScrollBar2.UseThemeFont = true;
            // 
            // DataGridViewPanel
            // 
            DataGridViewPanel.Dock = DockStyle.Fill;
            DataGridViewPanel.Location = new Point(0, 0);
            DataGridViewPanel.Name = "DataGridViewPanel";
            DataGridViewPanel.Size = new Size(760, 377);
            DataGridViewPanel.TabIndex = 2;
            DataGridViewPanel.Controls.Add(Grid);
            // 
            // UserControl1
            // 

            control.Controls.Add(MainSplitContainer);

            MainSplitContainer.Panel1.ResumeLayout(false);
            MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MainSplitContainer).EndInit();
            MainSplitContainer.ResumeLayout(false);
            ScrollHorizPanel.ResumeLayout(false);
            ScrollVerticalPanel.ResumeLayout(false);
            FooterPanel.ResumeLayout(false);
            control.ResumeLayout(false);
        }

        public static SplitContainer MainSplitContainer { get; set; }
        public static Panel FilterColumnHeaderPanel { get; set; }
        public static Panel HeaderPanel { get; set; }
        public static Panel ColumnHeaderPanel { get; set; }
        public static Panel ScrollVerticalPanel { get; set; }
        public static Panel FooterPanel { get; set; }
        public static Panel ScrollHorizPanel { get; set; }
        public static Panel TotalsPanel { get; set; }
        public static Panel DataNavigatorPanel { get; set; }
        public static BeepScrollBar beepScrollBar2 { get; set; }
        public static BeepScrollBar beepScrollBar1 { get; set; }
        public static Panel DataGridViewPanel { get; set; }

        public static bool HideColumnHeaders
        {
            get { return ColumnHeaderPanel.Visible; }
            set { ColumnHeaderPanel.Visible = value; }
        }
        public static bool HideFilterColumnHeaders
        {
            get { return FilterColumnHeaderPanel.Visible; }
            set { FilterColumnHeaderPanel.Visible = value; }
        }
        public static bool HideTotals
        {
            get { return TotalsPanel.Visible; }
            set { TotalsPanel.Visible = value; }
        }
        public static bool HideDataNavigator
        {
            get { return DataNavigatorPanel.Visible; }
            set { DataNavigatorPanel.Visible = value; }
        }
        public static bool HideFooter
        {
            get { return FooterPanel.Visible; }
            set { FooterPanel.Visible = value; }
        }
        public static bool HideScrollVertical
        {
            get { return ScrollVerticalPanel.Visible; }
            set { ScrollVerticalPanel.Visible = value; }
        }
        public static bool HideScrollHorizontal
        {
            get { return ScrollHorizPanel.Visible; }
            set { ScrollHorizPanel.Visible = value; }
        }
        public static bool HideHeader
        {
            get { return HeaderPanel.Visible; }
            set { HeaderPanel.Visible = value; }
        }
    }
}
