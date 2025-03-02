
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    partial class uc_ConnnectionDrivers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_ConnnectionDrivers));
            DataBase.EntityStructure entityStructure1 = new DataBase.EntityStructure();
            driversConfigViewModelBindingSource = new BindingSource(components);
            beepSimpleGrid1 = new TheTechIdea.Beep.Winform.Controls.BeepSimpleGrid();
            ((System.ComponentModel.ISupportInitialize)driversConfigViewModelBindingSource).BeginInit();
            SuspendLayout();
            // 
            // driversConfigViewModelBindingSource
            // 
            driversConfigViewModelBindingSource.DataMember = "ConnectionDriversConfigs";
            driversConfigViewModelBindingSource.DataSource = typeof(MVVM.ViewModels.BeepConfig.DriversConfigViewModel);
            // 
            // beepSimpleGrid1
            // 
            beepSimpleGrid1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepSimpleGrid1.AnimationDuration = 500;
            beepSimpleGrid1.AnimationType = Winform.Controls.DisplayAnimationType.None;
            beepSimpleGrid1.ApplyThemeToChilds = false;
            beepSimpleGrid1.BackColor = Color.White;
            beepSimpleGrid1.BadgeBackColor = Color.Red;
            beepSimpleGrid1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepSimpleGrid1.BadgeForeColor = Color.White;
            beepSimpleGrid1.BadgeShape = Winform.Controls.BadgeShape.Circle;
            beepSimpleGrid1.BadgeText = "";
            beepSimpleGrid1.BlockID = null;
            beepSimpleGrid1.BorderColor = Color.FromArgb(200, 200, 200);
            beepSimpleGrid1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepSimpleGrid1.BorderRadius = 3;
            beepSimpleGrid1.BorderStyle = BorderStyle.FixedSingle;
            beepSimpleGrid1.BorderThickness = 1;
            beepSimpleGrid1.BottomoffsetForDrawingRect = 0;
            beepSimpleGrid1.BottomRow = null;
            beepSimpleGrid1.BoundProperty = null;
            beepSimpleGrid1.CanBeFocused = true;
            beepSimpleGrid1.CanBeHovered = false;
            beepSimpleGrid1.CanBePressed = true;
            beepSimpleGrid1.Category = Utilities.DbFieldCategory.String;
            beepSimpleGrid1.ColumnHeaderFont = new Font("Arial", 8F);
            beepSimpleGrid1.ColumnHeaderHeight = 40;
            beepSimpleGrid1.Columns = (List<BeepColumnConfig>)resources.GetObject("beepSimpleGrid1.Columns");
            beepSimpleGrid1.ComponentName = "beepSimpleGrid1";
            beepSimpleGrid1.DataContext = null;
            beepSimpleGrid1.DataSource = driversConfigViewModelBindingSource;
            beepSimpleGrid1.DataSourceProperty = null;
            beepSimpleGrid1.DataSourceType = Winform.Controls.Models.GridDataSourceType.Fixed;
            beepSimpleGrid1.DefaultColumnHeaderWidth = 50;
            beepSimpleGrid1.DisabledBackColor = Color.Gray;
            beepSimpleGrid1.DisabledForeColor = Color.Empty;
            beepSimpleGrid1.Dock = DockStyle.Fill;
            beepSimpleGrid1.DrawingRect = new Rectangle(1, 1, 1030, 694);
            beepSimpleGrid1.Easing = Winform.Controls.EasingType.Linear;
            entityStructure1.Caption = null;
            entityStructure1.Category = null;
            entityStructure1.Created = false;
            entityStructure1.CustomBuildQuery = null;
            entityStructure1.DatabaseType = Utilities.DataSourceType.NONE;
            entityStructure1.DatasourceEntityName = null;
            entityStructure1.DataSourceID = null;
            entityStructure1.DefaultChartType = null;
            entityStructure1.Description = null;
            entityStructure1.Drawn = false;
            entityStructure1.Editable = false;
            entityStructure1.EndRow = 0;
            entityStructure1.EntityName = "ConnectionDriversConfig";
            entityStructure1.EntityPath = null;
            entityStructure1.GuidID = "9c587806-a1ed-4710-8101-9962403811c9";
            entityStructure1.Id = 0;
            entityStructure1.IsCreated = false;
            entityStructure1.IsIdentity = false;
            entityStructure1.IsLoaded = false;
            entityStructure1.IsSaved = false;
            entityStructure1.IsSynced = false;
            entityStructure1.KeyToken = null;
            entityStructure1.OriginalEntityName = null;
            entityStructure1.ParentId = 0;
            entityStructure1.SchemaOrOwnerOrDatabase = null;
            entityStructure1.Show = false;
            entityStructure1.StartRow = 0;
            entityStructure1.StatusDescription = null;
            entityStructure1.ViewID = 0;
            entityStructure1.Viewtype = Utilities.ViewType.Table;
            beepSimpleGrid1.Entity = entityStructure1;
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
            beepSimpleGrid1.GuidID = "5f94a64e-f704-4e9b-b27f-966e25e1d9b2";
            beepSimpleGrid1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepSimpleGrid1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepSimpleGrid1.HoveredBackcolor = Color.Wheat;
            beepSimpleGrid1.HoverForeColor = Color.Black;
            beepSimpleGrid1.Id = -1;
            beepSimpleGrid1.InactiveBackColor = Color.Gray;
            beepSimpleGrid1.InactiveBorderColor = Color.Gray;
            beepSimpleGrid1.InactiveForeColor = Color.Black;
            beepSimpleGrid1.Info = null;
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
            beepSimpleGrid1.IsNew = false;
            beepSimpleGrid1.IsPressed = false;
            beepSimpleGrid1.IsReadOnly = false;
            beepSimpleGrid1.IsRequired = false;
            beepSimpleGrid1.IsRounded = true;
            beepSimpleGrid1.IsRoundedAffectedByTheme = true;
            beepSimpleGrid1.IsSelected = false;
            beepSimpleGrid1.IsShadowAffectedByTheme = true;
            beepSimpleGrid1.IsVisible = false;
            beepSimpleGrid1.Items = (List<object>)resources.GetObject("beepSimpleGrid1.Items");
            beepSimpleGrid1.LeftoffsetForDrawingRect = 0;
            beepSimpleGrid1.LinkedProperty = null;
            beepSimpleGrid1.Location = new Point(0, 0);
            beepSimpleGrid1.Name = "beepSimpleGrid1";
            beepSimpleGrid1.OverrideFontSize = Winform.Controls.TypeStyleFontSize.None;
            beepSimpleGrid1.ParentBackColor = Color.Empty;
            beepSimpleGrid1.ParentControl = null;
            beepSimpleGrid1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepSimpleGrid1.PressedBorderColor = Color.Gray;
            beepSimpleGrid1.PressedForeColor = Color.Black;
            beepSimpleGrid1.QueryFunction = null;
            beepSimpleGrid1.QueryFunctionName = null;
            beepSimpleGrid1.RightoffsetForDrawingRect = 0;
            beepSimpleGrid1.RowHeight = 25;
            beepSimpleGrid1.SavedGuidID = null;
            beepSimpleGrid1.SavedID = null;
            beepSimpleGrid1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepSimpleGrid1.ShadowOffset = 0;
            beepSimpleGrid1.ShadowOpacity = 0.5F;
            beepSimpleGrid1.ShowAllBorders = true;
            beepSimpleGrid1.ShowBottomBorder = true;
            beepSimpleGrid1.ShowBottomRow = false;
            beepSimpleGrid1.ShowColumnHeaders = true;
            beepSimpleGrid1.ShowFilterButton = true;
            beepSimpleGrid1.ShowFocusIndicator = false;
            beepSimpleGrid1.ShowFooter = false;
            beepSimpleGrid1.ShowHeaderPanel = true;
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
            beepSimpleGrid1.Size = new Size(1032, 696);
            beepSimpleGrid1.SlideFrom = Winform.Controls.SlideDirection.Left;
            beepSimpleGrid1.StaticNotMoving = false;
            beepSimpleGrid1.TabIndex = 0;
            beepSimpleGrid1.TempBackColor = Color.Empty;
            beepSimpleGrid1.Text = "beepSimpleGrid1";
            beepSimpleGrid1.TextImageRelation = TextImageRelation.ImageAboveText;
            beepSimpleGrid1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepSimpleGrid1.Title = "BeepSimpleGrid Title";
            beepSimpleGrid1.TitleText = "Simple BeepGrid";
            beepSimpleGrid1.TitleTextFont = new Font("Segoe UI", 9F);
            beepSimpleGrid1.ToolTipText = "";
            beepSimpleGrid1.TopoffsetForDrawingRect = 0;
            beepSimpleGrid1.UseGradientBackground = false;
            beepSimpleGrid1.UseThemeFont = true;
            beepSimpleGrid1.XOffset = 0;
            // 
            // uc_ConnnectionDrivers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(beepSimpleGrid1);
            Name = "uc_ConnnectionDrivers";
            Size = new Size(1032, 696);
            ((System.ComponentModel.ISupportInitialize)driversConfigViewModelBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private BindingSource driversConfigViewModelBindingSource;
        private Controls.BeepSimpleGrid beepSimpleGrid1;
    }
}
