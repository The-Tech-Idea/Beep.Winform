using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    partial class uc_DataEdit
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
            BeepRowConfig beepRowConfig1 = new BeepRowConfig();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_DataEdit));
            BeepColumnConfig beepColumnConfig1 = new BeepColumnConfig();
            BeepColumnConfig beepColumnConfig2 = new BeepColumnConfig();
            BeepColumnConfig beepColumnConfig3 = new BeepColumnConfig();
            beepSimpleGrid1 = new TheTechIdea.Beep.Winform.Controls.BeepSimpleGrid();
            MainTemplatePanel.SuspendLayout();
            SuspendLayout();
            // 
            // MainTemplatePanel
            // 
            MainTemplatePanel.Controls.Add(beepSimpleGrid1);
            MainTemplatePanel.Dock = DockStyle.Fill;
            MainTemplatePanel.DrawingRect = new Rectangle(0, 0, 918, 707);
            MainTemplatePanel.Size = new Size(918, 707);
            // 
            // beepSimpleGrid1
            // 
            beepRowConfig1.DisplayIndex = -1;
            beepRowConfig1.Height = 25;
            beepRowConfig1.Id = "d2295fdc-62cf-4bd3-97ce-a21b064e6df8";
            beepRowConfig1.IsAggregation = true;
            beepRowConfig1.IsDataLoaded = false;
            beepRowConfig1.IsDeleted = false;
            beepRowConfig1.IsDirty = false;
            beepRowConfig1.IsEditable = false;
            beepRowConfig1.IsNew = false;
            beepRowConfig1.IsReadOnly = false;
            beepRowConfig1.IsSelected = false;
            beepRowConfig1.IsVisible = true;
            beepRowConfig1.OldDisplayIndex = 0;
            beepRowConfig1.RowCheckRect = new Rectangle(0, 0, 0, 0);
            beepRowConfig1.RowData = null;
            beepRowConfig1.RowIndex = 0;
            beepRowConfig1.UpperX = 0;
            beepRowConfig1.UpperY = 0;
            beepRowConfig1.Width = 100;
            beepSimpleGrid1.aggregationRow = beepRowConfig1;
            beepSimpleGrid1.AllowUserToAddRows = false;
            beepSimpleGrid1.AllowUserToDeleteRows = false;
            beepSimpleGrid1.AnimationDuration = 500;
            beepSimpleGrid1.AnimationType = DisplayAnimationType.None;
            beepSimpleGrid1.ApplyThemeToChilds = false;
            beepSimpleGrid1.AutoDrawHitListComponents = true;
            beepSimpleGrid1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            beepSimpleGrid1.BackColor = Color.White;
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
            beepColumnConfig1.AggregationType = AggregationType.None;
            beepColumnConfig1.AllowSort = true;
            beepColumnConfig1.CellEditor = BeepColumnType.CheckBoxBool;
            beepColumnConfig1.ColumnCaption = "☑";
            beepColumnConfig1.ColumnName = "Sel";
            beepColumnConfig1.ColumnType = Utilities.DbFieldCategory.Boolean;
            beepColumnConfig1.CustomControlName = null;
            beepColumnConfig1.Date = new DateTime(0L);
            beepColumnConfig1.DecimalPlaces = 0;
            beepColumnConfig1.DefaultValue = null;
            beepColumnConfig1.EnumSourceType = null;
            beepColumnConfig1.Filter = null;
            beepColumnConfig1.Format = null;
            beepColumnConfig1.GuidID = "e1aa9c23-8dfa-4de5-b8fa-629ed97f9eb8";
            beepColumnConfig1.HasTotal = false;
            beepColumnConfig1.ImagePath = null;
            beepColumnConfig1.Index = 0;
            beepColumnConfig1.IsAutoIncrement = false;
            beepColumnConfig1.IsFilteOn = false;
            beepColumnConfig1.IsFiltered = false;
            beepColumnConfig1.IsForeignKey = false;
            beepColumnConfig1.IsNullable = false;
            beepColumnConfig1.IsPrimaryKey = false;
            beepColumnConfig1.IsRequired = false;
            beepColumnConfig1.IsRowID = false;
            beepColumnConfig1.IsRowNumColumn = false;
            beepColumnConfig1.IsSelectionCheckBox = true;
            beepColumnConfig1.IsSorted = false;
            beepColumnConfig1.IsTotalOn = false;
            beepColumnConfig1.IsUnbound = true;
            beepColumnConfig1.IsUnique = false;
            beepColumnConfig1.MaxImageHeight = 32;
            beepColumnConfig1.MaxImageWidth = 32;
            beepColumnConfig1.MaxValue = 0;
            beepColumnConfig1.MinValue = 0;
            beepColumnConfig1.OldValue = new decimal(new int[] { 0, 0, 0, 0 });
            beepColumnConfig1.ParentColumnName = null;
            beepColumnConfig1.PropertyTypeName = "System.Boolean, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e";
            beepColumnConfig1.QueryToGetValues = null;
            beepColumnConfig1.ReadOnly = false;
            beepColumnConfig1.Resizable = DataGridViewTriState.False;
            beepColumnConfig1.ShowFilterIcon = true;
            beepColumnConfig1.ShowSortIcon = true;
            beepColumnConfig1.SortDirection = SortDirection.Ascending;
            beepColumnConfig1.SortMode = DataGridViewColumnSortMode.NotSortable;
            beepColumnConfig1.Sticked = true;
            beepColumnConfig1.Total = new decimal(new int[] { 0, 0, 0, 0 });
            beepColumnConfig1.Visible = false;
            beepColumnConfig1.Width = 30;
            beepColumnConfig2.AggregationType = AggregationType.Count;
            beepColumnConfig2.AllowSort = true;
            beepColumnConfig2.ColumnCaption = "#";
            beepColumnConfig2.ColumnName = "RowNum";
            beepColumnConfig2.ColumnType = Utilities.DbFieldCategory.Numeric;
            beepColumnConfig2.CustomControlName = null;
            beepColumnConfig2.Date = new DateTime(0L);
            beepColumnConfig2.DecimalPlaces = 0;
            beepColumnConfig2.DefaultValue = null;
            beepColumnConfig2.EnumSourceType = null;
            beepColumnConfig2.Filter = null;
            beepColumnConfig2.Format = null;
            beepColumnConfig2.GuidID = "b653cf50-1c73-4e84-9046-41d9633113ca";
            beepColumnConfig2.HasTotal = false;
            beepColumnConfig2.ImagePath = null;
            beepColumnConfig2.Index = 1;
            beepColumnConfig2.IsAutoIncrement = false;
            beepColumnConfig2.IsFilteOn = false;
            beepColumnConfig2.IsFiltered = false;
            beepColumnConfig2.IsForeignKey = false;
            beepColumnConfig2.IsNullable = false;
            beepColumnConfig2.IsPrimaryKey = false;
            beepColumnConfig2.IsRequired = false;
            beepColumnConfig2.IsRowID = false;
            beepColumnConfig2.IsRowNumColumn = true;
            beepColumnConfig2.IsSelectionCheckBox = false;
            beepColumnConfig2.IsSorted = false;
            beepColumnConfig2.IsTotalOn = false;
            beepColumnConfig2.IsUnbound = true;
            beepColumnConfig2.IsUnique = false;
            beepColumnConfig2.MaxImageHeight = 32;
            beepColumnConfig2.MaxImageWidth = 32;
            beepColumnConfig2.MaxValue = 0;
            beepColumnConfig2.MinValue = 0;
            beepColumnConfig2.OldValue = new decimal(new int[] { 0, 0, 0, 0 });
            beepColumnConfig2.ParentColumnName = null;
            beepColumnConfig2.PropertyTypeName = "System.Int32, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e";
            beepColumnConfig2.QueryToGetValues = null;
            beepColumnConfig2.ReadOnly = true;
            beepColumnConfig2.Resizable = DataGridViewTriState.False;
            beepColumnConfig2.ShowFilterIcon = true;
            beepColumnConfig2.ShowSortIcon = true;
            beepColumnConfig2.SortDirection = SortDirection.Ascending;
            beepColumnConfig2.SortMode = DataGridViewColumnSortMode.NotSortable;
            beepColumnConfig2.Sticked = true;
            beepColumnConfig2.Total = new decimal(new int[] { 0, 0, 0, 0 });
            beepColumnConfig2.Width = 30;
            beepColumnConfig3.AggregationType = AggregationType.None;
            beepColumnConfig3.AllowSort = true;
            beepColumnConfig3.ColumnCaption = "RowID";
            beepColumnConfig3.ColumnName = "RowID";
            beepColumnConfig3.ColumnType = Utilities.DbFieldCategory.Numeric;
            beepColumnConfig3.CustomControlName = null;
            beepColumnConfig3.Date = new DateTime(0L);
            beepColumnConfig3.DecimalPlaces = 0;
            beepColumnConfig3.DefaultValue = null;
            beepColumnConfig3.EnumSourceType = null;
            beepColumnConfig3.Filter = null;
            beepColumnConfig3.Format = null;
            beepColumnConfig3.GuidID = "68021d57-edcf-4832-ac78-6762cfd25ce6";
            beepColumnConfig3.HasTotal = false;
            beepColumnConfig3.ImagePath = null;
            beepColumnConfig3.Index = 2;
            beepColumnConfig3.IsAutoIncrement = false;
            beepColumnConfig3.IsFilteOn = false;
            beepColumnConfig3.IsFiltered = false;
            beepColumnConfig3.IsForeignKey = false;
            beepColumnConfig3.IsNullable = false;
            beepColumnConfig3.IsPrimaryKey = false;
            beepColumnConfig3.IsRequired = false;
            beepColumnConfig3.IsRowID = true;
            beepColumnConfig3.IsRowNumColumn = false;
            beepColumnConfig3.IsSelectionCheckBox = false;
            beepColumnConfig3.IsSorted = false;
            beepColumnConfig3.IsTotalOn = false;
            beepColumnConfig3.IsUnbound = true;
            beepColumnConfig3.IsUnique = false;
            beepColumnConfig3.MaxImageHeight = 32;
            beepColumnConfig3.MaxImageWidth = 32;
            beepColumnConfig3.MaxValue = 0;
            beepColumnConfig3.MinValue = 0;
            beepColumnConfig3.OldValue = new decimal(new int[] { 0, 0, 0, 0 });
            beepColumnConfig3.ParentColumnName = null;
            beepColumnConfig3.PropertyTypeName = "System.Int32, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e";
            beepColumnConfig3.QueryToGetValues = null;
            beepColumnConfig3.ReadOnly = true;
            beepColumnConfig3.Resizable = DataGridViewTriState.False;
            beepColumnConfig3.ShowFilterIcon = true;
            beepColumnConfig3.ShowSortIcon = true;
            beepColumnConfig3.SortDirection = SortDirection.Ascending;
            beepColumnConfig3.SortMode = DataGridViewColumnSortMode.NotSortable;
            beepColumnConfig3.Sticked = true;
            beepColumnConfig3.Total = new decimal(new int[] { 0, 0, 0, 0 });
            beepColumnConfig3.Visible = false;
            beepColumnConfig3.Width = 30;
            beepSimpleGrid1.Columns.Add(beepColumnConfig1);
            beepSimpleGrid1.Columns.Add(beepColumnConfig2);
            beepSimpleGrid1.Columns.Add(beepColumnConfig3);
            beepSimpleGrid1.ComponentName = "beepSimpleGrid1";
            beepSimpleGrid1.DataNavigator = null;
            beepSimpleGrid1.DataSource = null;
            beepSimpleGrid1.DataSourceProperty = null;
            beepSimpleGrid1.DataSourceType = GridDataSourceType.Fixed;
            beepSimpleGrid1.DefaultColumnHeaderWidth = 50;
            beepSimpleGrid1.DisabledBackColor = Color.White;
            beepSimpleGrid1.DisabledBorderColor = Color.Empty;
            beepSimpleGrid1.DisabledForeColor = Color.Black;
            //beepSimpleGrid1.DisableDpiAndScaling = true;
            beepSimpleGrid1.Dock = DockStyle.Fill;
            beepSimpleGrid1.DrawInBlackAndWhite = false;
            beepSimpleGrid1.DrawingRect = new Rectangle(0, 0, 918, 707);
            beepSimpleGrid1.Easing = EasingType.Linear;
            beepSimpleGrid1.EnableHighQualityRendering = true;
            beepSimpleGrid1.EnableRippleEffect = false;
            beepSimpleGrid1.EnableSplashEffect = true;
            beepSimpleGrid1.EntityName = null;
            beepSimpleGrid1.ExternalDrawingLayer = DrawingLayer.AfterAll;
            beepSimpleGrid1.FieldID = null;
            beepSimpleGrid1.FilledBackgroundColor = Color.FromArgb(20, 0, 0, 0);
            beepSimpleGrid1.FitColumnToContent = false;
            beepSimpleGrid1.FloatingLabel = true;
            beepSimpleGrid1.FocusBackColor = Color.White;
            beepSimpleGrid1.FocusBorderColor = Color.Gray;
            beepSimpleGrid1.FocusForeColor = Color.FromArgb(40, 40, 40);
            beepSimpleGrid1.FocusIndicatorColor = Color.Blue;
            beepSimpleGrid1.ForeColor = Color.FromArgb(40, 40, 40);
            beepSimpleGrid1.Form = null;
            beepSimpleGrid1.GlassmorphismBlur = 10F;
            beepSimpleGrid1.GlassmorphismOpacity = 0.1F;
            beepSimpleGrid1.GradientAngle = 0F;
            beepSimpleGrid1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepSimpleGrid1.GradientEndColor = Color.Gray;
            beepSimpleGrid1.GradientStartColor = Color.Gray;
            beepSimpleGrid1.GridMode = false;
            beepSimpleGrid1.GuidID = "9dc3064c-eb17-44ff-a3c0-b14cf63b5266";
            beepSimpleGrid1.HelperText = "";
            beepSimpleGrid1.HitAreaEventOn = false;
            beepSimpleGrid1.HitTestControl = null;
            beepSimpleGrid1.HoverBackColor = Color.White;
            beepSimpleGrid1.HoverBorderColor = Color.Gray;
            beepSimpleGrid1.HoveredBackcolor = Color.Wheat;
            beepSimpleGrid1.HoverForeColor = Color.FromArgb(40, 40, 40);
            beepSimpleGrid1.Id = -1;
            beepSimpleGrid1.InactiveBorderColor = Color.Gray;
            beepSimpleGrid1.IsAcceptButton = false;
            beepSimpleGrid1.IsBorderAffectedByTheme = true;
            beepSimpleGrid1.IsCancelButton = false;
            beepSimpleGrid1.IsChild = false;
            beepSimpleGrid1.IsCustomeBorder = false;
            beepSimpleGrid1.IsDefault = false;
            beepSimpleGrid1.IsDeleted = false;
            beepSimpleGrid1.IsDirty = false;
            beepSimpleGrid1.IsEditable = false;
            beepSimpleGrid1.IsFiltered = false;
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
            beepSimpleGrid1.IsVisible = true;
            beepSimpleGrid1.Items = null;
            beepSimpleGrid1.LabelText = "";
            beepSimpleGrid1.LeftoffsetForDrawingRect = 0;
            beepSimpleGrid1.LinkedProperty = null;
            beepSimpleGrid1.Location = new Point(0, 0);
            beepSimpleGrid1.MaterialBorderVariant = MaterialTextFieldVariant.Standard;
            beepSimpleGrid1.MaxHitListDrawPerFrame = 0;
            beepSimpleGrid1.ModernGradientType = ModernGradientType.None;
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
            beepSimpleGrid1.RadialCenter = (PointF)resources.GetObject("beepSimpleGrid1.RadialCenter");
            beepSimpleGrid1.ReadOnly = false;
            beepSimpleGrid1.RightoffsetForDrawingRect = 0;
            beepSimpleGrid1.RowHeight = 25;
            beepSimpleGrid1.SavedGuidID = null;
            beepSimpleGrid1.SavedID = null;
            beepSimpleGrid1.SelectedBackColor = Color.White;
            beepSimpleGrid1.SelectedBorderColor = Color.Empty;
            beepSimpleGrid1.SelectedForeColor = Color.FromArgb(40, 40, 40);
            beepSimpleGrid1.SelectedValue = null;
            beepSimpleGrid1.SelectionColumnWidth = 30;
            beepSimpleGrid1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            beepSimpleGrid1.ShadowColor = Color.Black;
            beepSimpleGrid1.ShadowOffset = 0;
            beepSimpleGrid1.ShadowOpacity = 0.5F;
            beepSimpleGrid1.ShowAggregationRow = false;
            beepSimpleGrid1.ShowAllBorders = false;
            beepSimpleGrid1.ShowBottomBorder = false;
            beepSimpleGrid1.ShowCheckboxes = false;
            beepSimpleGrid1.ShowColumnHeaders = true;
            beepSimpleGrid1.ShowFilter = false;
            beepSimpleGrid1.ShowFilterIcon = false;
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
            beepSimpleGrid1.ShowSortIcon = false;
            beepSimpleGrid1.ShowSortIcons = true;
            beepSimpleGrid1.ShowTopBorder = false;
            beepSimpleGrid1.ShowVerticalGridLines = true;
            beepSimpleGrid1.ShowVerticalScrollBar = true;
            beepSimpleGrid1.Size = new Size(918, 707);
            beepSimpleGrid1.SlideFrom = SlideDirection.Left;
            beepSimpleGrid1.SortDirection = SortDirection.None;
            beepSimpleGrid1.StaticNotMoving = false;
            beepSimpleGrid1.TabIndex = 0;
            beepSimpleGrid1.Tag = MainTemplatePanel;
            beepSimpleGrid1.TempBackColor = Color.Empty;
            beepSimpleGrid1.Text = "beepSimpleGrid1";
            beepSimpleGrid1.TextImageRelation = TextImageRelation.ImageAboveText;
            beepSimpleGrid1.Theme = "DefaultType";
            beepSimpleGrid1.TitleHeaderImage = "simpleinfoapps.svg";
            beepSimpleGrid1.TitleText = "Simple BeepGrid";
            beepSimpleGrid1.TitleTextFont = new Font("Segoe UI", 9F);
            beepSimpleGrid1.ToolTipText = "";
          
            beepSimpleGrid1.UpdateLog = null;
            beepSimpleGrid1.UseExternalBufferedGraphics = false;
            beepSimpleGrid1.UseGlassmorphism = false;
            beepSimpleGrid1.UseGradientBackground = false;
            beepSimpleGrid1.UseThemeFont = true;
            beepSimpleGrid1.XOffset = 0;
            // 
            // uc_DataEdit
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "uc_DataEdit";
            Size = new Size(918, 707);
            MainTemplatePanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Controls.BeepSimpleGrid beepSimpleGrid1;
    }
}
