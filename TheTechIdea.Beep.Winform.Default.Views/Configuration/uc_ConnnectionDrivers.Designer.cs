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
            beepDataGridView1 = new Controls.Grid.BeepDataGridView();
            ((System.ComponentModel.ISupportInitialize)driversConfigViewModelBindingSource).BeginInit();
            SuspendLayout();
            // 
            // driversConfigViewModelBindingSource
            // 
            driversConfigViewModelBindingSource.DataMember = "ConnectionDriversConfigs";
            driversConfigViewModelBindingSource.DataSource = typeof(MVVM.ViewModels.BeepConfig.DriversConfigViewModel);
            // 
            // beepDataGridView1
            // 
            beepDataGridView1.ActiveBackColor = Color.FromArgb(0, 120, 215);
            beepDataGridView1.AnimationDuration = 500;
            beepDataGridView1.AnimationType = Winform.Controls.DisplayAnimationType.None;
            beepDataGridView1.ApplyThemeToChilds = true;
            beepDataGridView1.BadgeBackColor = Color.Red;
            beepDataGridView1.BadgeFont = new Font("Arial", 8F, FontStyle.Bold);
            beepDataGridView1.BadgeForeColor = Color.White;
            beepDataGridView1.BadgeText = "";
            beepDataGridView1.BlockID = null;
            beepDataGridView1.BorderColor = Color.FromArgb(200, 200, 200);
            beepDataGridView1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepDataGridView1.BorderRadius = 3;
            beepDataGridView1.BorderStyle = BorderStyle.FixedSingle;
            beepDataGridView1.BorderThickness = 1;
            beepDataGridView1.BottomoffsetForDrawingRect = 0;
            beepDataGridView1.BoundProperty = null;
            beepDataGridView1.CanBeFocused = true;
            beepDataGridView1.CanBeHovered = false;
            beepDataGridView1.CanBePressed = true;
            beepDataGridView1.Category = Utilities.DbFieldCategory.String;
            beepDataGridView1.CellPainting = null;
            beepDataGridView1.ComponentName = "beepDataGridView1";
            beepDataGridView1.DataContext = null;
            beepDataGridView1.DataSource = driversConfigViewModelBindingSource;
            beepDataGridView1.DataSourceProperty = null;
            beepDataGridView1.DisabledBackColor = Color.Gray;
            beepDataGridView1.DisabledForeColor = Color.Empty;
            beepDataGridView1.DMEEditor = null;
            beepDataGridView1.DrawingRect = new Rectangle(3, 3, 830, 429);
            beepDataGridView1.Easing = Winform.Controls.EasingType.Linear;
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
            entityStructure1.GuidID = "87a7ebe3-7c4c-4e3f-95e6-069a70f9b724";
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
            beepDataGridView1.Entity = entityStructure1;
            beepDataGridView1.FieldID = null;
            beepDataGridView1.FocusBackColor = Color.White;
            beepDataGridView1.FocusBorderColor = Color.Gray;
            beepDataGridView1.FocusForeColor = Color.Black;
            beepDataGridView1.FocusIndicatorColor = Color.Blue;
            beepDataGridView1.Form = null;
            beepDataGridView1.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            beepDataGridView1.GradientEndColor = Color.FromArgb(230, 230, 230);
            beepDataGridView1.GradientStartColor = Color.White;
            beepDataGridView1.GuidID = "e4cfc9f7-f7e1-4769-b1a0-50220fb79bc6";
            beepDataGridView1.HoverBackColor = Color.FromArgb(230, 230, 230);
            beepDataGridView1.HoverBorderColor = Color.FromArgb(0, 120, 215);
            beepDataGridView1.HoveredBackcolor = Color.Wheat;
            beepDataGridView1.HoverForeColor = Color.Black;
            beepDataGridView1.Id = -1;
            beepDataGridView1.InactiveBackColor = Color.Gray;
            beepDataGridView1.InactiveBorderColor = Color.Gray;
            beepDataGridView1.InactiveForeColor = Color.Black;
            beepDataGridView1.IsAcceptButton = false;
            beepDataGridView1.IsBorderAffectedByTheme = true;
            beepDataGridView1.IsCancelButton = false;
            beepDataGridView1.IsChild = false;
            beepDataGridView1.IsCustomeBorder = false;
            beepDataGridView1.IsDefault = false;
            beepDataGridView1.IsDeleted = false;
            beepDataGridView1.IsDirty = false;
            beepDataGridView1.IsEditable = false;
            beepDataGridView1.IsFocused = false;
            beepDataGridView1.IsFramless = false;
            beepDataGridView1.IsHovered = false;
            beepDataGridView1.IsNew = false;
            beepDataGridView1.IsPressed = false;
            beepDataGridView1.IsReadOnly = false;
            beepDataGridView1.IsRequired = false;
            beepDataGridView1.IsRounded = true;
            beepDataGridView1.IsRoundedAffectedByTheme = true;
            beepDataGridView1.IsSelected = false;
            beepDataGridView1.IsShadowAffectedByTheme = true;
            beepDataGridView1.IsVisible = false;
            beepDataGridView1.LeftoffsetForDrawingRect = 0;
            beepDataGridView1.LinkedProperty = null;
            beepDataGridView1.Location = new Point(112, 175);
            beepDataGridView1.Margin = new Padding(2);
            beepDataGridView1.Name = "beepDataGridView1";
            beepDataGridView1.OverrideFontSize = Winform.Controls.TypeStyleFontSize.None;
            beepDataGridView1.Padding = new Padding(2);
            beepDataGridView1.ParentBackColor = Color.Empty;
            beepDataGridView1.ParentControl = null;
            beepDataGridView1.PressedBackColor = Color.FromArgb(0, 120, 215);
            beepDataGridView1.PressedBorderColor = Color.Gray;
            beepDataGridView1.PressedForeColor = Color.Black;
            beepDataGridView1.RightoffsetForDrawingRect = 0;
            beepDataGridView1.SavedGuidID = null;
            beepDataGridView1.SavedID = null;
            beepDataGridView1.ShadowColor = Color.FromArgb(100, 0, 0, 0);
            beepDataGridView1.ShadowOffset = 0;
            beepDataGridView1.ShadowOpacity = 0.5F;
            beepDataGridView1.ShowAllBorders = true;
            beepDataGridView1.ShowBottomBorder = true;
            beepDataGridView1.ShowDataNavigator = true;
            beepDataGridView1.ShowFilter = false;
            beepDataGridView1.ShowFocusIndicator = false;
            beepDataGridView1.ShowLeftBorder = true;
            beepDataGridView1.ShowRightBorder = true;
            beepDataGridView1.ShowShadow = false;
            beepDataGridView1.ShowTopBorder = true;
            beepDataGridView1.ShowTotalsPanel = false;
            beepDataGridView1.Size = new Size(836, 435);
            beepDataGridView1.SlideFrom = Winform.Controls.SlideDirection.Left;
            beepDataGridView1.StaticNotMoving = false;
            beepDataGridView1.TabIndex = 0;
            beepDataGridView1.TempBackColor = Color.Empty;
            beepDataGridView1.Text = "beepDataGridView1";
            beepDataGridView1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepDataGridView1.TitleFont = new Font("Arial", 10F);
            beepDataGridView1.ToolTipText = "";
            beepDataGridView1.TopoffsetForDrawingRect = 0;
            beepDataGridView1.UseGradientBackground = false;
            beepDataGridView1.UseThemeFont = true;
            // 
            // uc_ConnnectionDrivers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(beepDataGridView1);
            Name = "uc_ConnnectionDrivers";
            Size = new Size(1032, 696);
            ((System.ComponentModel.ISupportInitialize)driversConfigViewModelBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private BindingSource driversConfigViewModelBindingSource;
        private Controls.Grid.BeepDataGridView beepDataGridView1;
    }
}
