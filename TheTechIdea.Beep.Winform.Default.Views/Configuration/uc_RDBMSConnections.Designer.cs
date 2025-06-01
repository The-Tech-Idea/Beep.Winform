using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    partial class uc_RDBMSConnections
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_RDBMSConnections));
            beepSimpleGrid1 = new TheTechIdea.Beep.Winform.Controls.BeepSimpleGrid();
            dataConnectionViewModelBindingSource = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)dataConnectionViewModelBindingSource).BeginInit();
            SuspendLayout();
            // 
            // beepSimpleGrid1
            // 
          
            beepSimpleGrid1.aggregationRow = null;
            beepSimpleGrid1.AnimationDuration = 500;
            beepSimpleGrid1.AnimationType = DisplayAnimationType.None;
            beepSimpleGrid1.ApplyThemeToChilds = false;
            beepSimpleGrid1.BackColor = Color.White;
            beepSimpleGrid1.BadgeBackColor = Color.Red;
            beepSimpleGrid1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepSimpleGrid1.BadgeForeColor = Color.White;
            beepSimpleGrid1.BadgeShape = BadgeShape.Circle;
            beepSimpleGrid1.BadgeText = "";
            beepSimpleGrid1.BlockID = null;
            beepSimpleGrid1.BorderColor = Color.FromArgb(200, 200, 200);
            beepSimpleGrid1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSimpleGrid1.BorderRadius = 3;
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
            beepSimpleGrid1.DataContext = null;
            beepSimpleGrid1.DataNavigator = null;
            beepSimpleGrid1.DataSource = dataConnectionViewModelBindingSource;
            beepSimpleGrid1.DataSourceProperty = null;
            beepSimpleGrid1.DataSourceType = GridDataSourceType.Fixed;
            beepSimpleGrid1.DefaultColumnHeaderWidth = 50;
            beepSimpleGrid1.DisabledBackColor = Color.Gray;
            beepSimpleGrid1.DisabledForeColor = Color.Empty;
            beepSimpleGrid1.Dock = DockStyle.Fill;
            beepSimpleGrid1.DrawingRect = new Rectangle(1, 1, 1154, 661);
            beepSimpleGrid1.Easing = EasingType.Linear;
            beepSimpleGrid1.EntityName = "ConnectionProperties";
            beepSimpleGrid1.FieldID = null;
            beepSimpleGrid1.FocusBackColor = Color.White;
            beepSimpleGrid1.FocusBorderColor = Color.Gray;
            beepSimpleGrid1.FocusForeColor = Color.Black;
            beepSimpleGrid1.FocusIndicatorColor = Color.Blue;
            beepSimpleGrid1.ForeColor = Color.FromArgb(0, 0, 0);
            beepSimpleGrid1.Form = null;
            beepSimpleGrid1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSimpleGrid1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepSimpleGrid1.GradientStartColor = Color.White;
            beepSimpleGrid1.GuidID = "d24e2a3e-2858-43c7-9f06-8803215888e4";
            beepSimpleGrid1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepSimpleGrid1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepSimpleGrid1.HoveredBackcolor = Color.Wheat;
            beepSimpleGrid1.HoverForeColor = Color.Black;
            beepSimpleGrid1.Id = -1;
           
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
            beepSimpleGrid1.IsFrameless = true;
            beepSimpleGrid1.IsHovered = false;
            beepSimpleGrid1.IsLogging = false;
            beepSimpleGrid1.IsNew = false;
            beepSimpleGrid1.IsPressed = false;
            beepSimpleGrid1.IsReadOnly = false;
            beepSimpleGrid1.IsRequired = false;
            beepSimpleGrid1.IsRounded = false;
            beepSimpleGrid1.IsRoundedAffectedByTheme = false;
            beepSimpleGrid1.IsSelected = false;
            beepSimpleGrid1.IsShadowAffectedByTheme = false;
            beepSimpleGrid1.IsVisible = false;
            beepSimpleGrid1.Items = (List<object>)resources.GetObject("beepSimpleGrid1.Items");
            beepSimpleGrid1.LeftoffsetForDrawingRect = 0;
            beepSimpleGrid1.LinkedProperty = null;
            beepSimpleGrid1.Location = new Point(0, 0);
            beepSimpleGrid1.Name = "beepSimpleGrid1";
            beepSimpleGrid1.OverrideFontSize = TypeStyleFontSize.None;
            beepSimpleGrid1.ParentBackColor = Color.Empty;
            beepSimpleGrid1.ParentControl = null;
            beepSimpleGrid1.PercentageText = "36%";
            beepSimpleGrid1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepSimpleGrid1.PressedBorderColor = Color.Gray;
            beepSimpleGrid1.PressedForeColor = Color.Black;
            beepSimpleGrid1.QueryFunction = null;
            beepSimpleGrid1.QueryFunctionName = null;
            beepSimpleGrid1.RightoffsetForDrawingRect = 0;
            beepSimpleGrid1.RowHeight = 25;
            beepSimpleGrid1.SavedGuidID = null;
            beepSimpleGrid1.SavedID = null;
            beepSimpleGrid1.SelectionColumnWidth = 30;
            beepSimpleGrid1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepSimpleGrid1.ShadowOffset = 0;
            beepSimpleGrid1.ShadowOpacity = 0.5F;
            beepSimpleGrid1.ShowAggregationRow = false;
            beepSimpleGrid1.ShowAllBorders = true;
            beepSimpleGrid1.ShowBottomBorder = true;
            beepSimpleGrid1.ShowCheckboxes = false;
            beepSimpleGrid1.ShowColumnHeaders = true;
            beepSimpleGrid1.ShowFilter = false;
            beepSimpleGrid1.ShowFocusIndicator = false;
            beepSimpleGrid1.ShowFooter = false;
            beepSimpleGrid1.ShowHeaderPanel = false;
            beepSimpleGrid1.ShowHeaderPanelBorder = true;
            beepSimpleGrid1.ShowHorizontalGridLines = true;
            beepSimpleGrid1.ShowHorizontalScrollBar = true;
            beepSimpleGrid1.ShowLeftBorder = true;
            beepSimpleGrid1.ShowNavigator = true;
            beepSimpleGrid1.ShowRightBorder = true;
            beepSimpleGrid1.ShowRowHeaders = true;
            beepSimpleGrid1.ShowRowNumbers = true;
         
            beepSimpleGrid1.ShowShadow = false;
            beepSimpleGrid1.ShowSortIcons = true;
            beepSimpleGrid1.ShowTopBorder = true;
            beepSimpleGrid1.ShowVerticalGridLines = true;
            beepSimpleGrid1.ShowVerticalScrollBar = true;
            beepSimpleGrid1.Size = new Size(1156, 663);
            beepSimpleGrid1.SlideFrom = SlideDirection.Left;
            beepSimpleGrid1.StaticNotMoving = false;
            beepSimpleGrid1.TabIndex = 0;
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
            // dataConnectionViewModelBindingSource
            // 
            dataConnectionViewModelBindingSource.DataMember = "DataConnections";
            dataConnectionViewModelBindingSource.DataSource = typeof(MVVM.ViewModels.BeepConfig.DataConnectionViewModel);
            // 
            // uc_RDBMSConnections
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            MainTemplatePanel.Controls.Add(beepSimpleGrid1);
            Name = "uc_RDBMSConnections";
            Size = new Size(1156, 663);
            ((System.ComponentModel.ISupportInitialize)dataConnectionViewModelBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Controls.BeepSimpleGrid beepSimpleGrid1;
        private BindingSource dataConnectionViewModelBindingSource;
    }
}
