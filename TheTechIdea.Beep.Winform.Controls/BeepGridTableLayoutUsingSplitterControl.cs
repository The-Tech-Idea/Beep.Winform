using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepGridTableLayoutUsingSplitterControl:BeepControl
    {

        public BeepMultiSplitter tableLayoutPanel1;
        public BeepDataNavigator beepDataNavigator1;
        public VScrollBar vScrollBar1;
        public HScrollBar hScrollBar1;
        public BeepMultiSplitter GridtableLayoutPanel;
        public BeepLabel TitleLabel;
        public BeepLabel RecordCountLabel;
        public BeepLabel PageCountLabel;
        public BeepLabel PageNumberLabel;

        private BeepGridRowPainterForTableLayout beepGridRowPainter;
        public BeepGridTableLayoutUsingSplitterControl()
        {
            initView();
        }
        public void initView()
        {
            tableLayoutPanel1 = new BeepMultiSplitter();
            TitleLabel = new BeepLabel();
            vScrollBar1 = new VScrollBar();
            beepDataNavigator1 = new BeepDataNavigator();
            hScrollBar1 = new HScrollBar();
            GridtableLayoutPanel = new BeepMultiSplitter();
            Controls.Add(tableLayoutPanel1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel1.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.08991826F));
            tableLayoutPanel1.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 98.91008F));
            tableLayoutPanel1.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(TitleLabel, 0, 0);
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(vScrollBar1, 2, 2);
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(beepDataNavigator1, 1, 5);
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(hScrollBar1, 1, 3);
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(GridtableLayoutPanel, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.tableLayoutPanel.RowCount = 3;
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 9F));
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tableLayoutPanel1.Size = new Size(755, 621);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // TitleLabel
            // 
            TitleLabel.ActiveBackColor = Color.Gray;
            TitleLabel.AnimationDuration = 500;
            TitleLabel.AnimationType = DisplayAnimationType.None;
            TitleLabel.ApplyThemeOnImage = false;
            TitleLabel.ApplyThemeToChilds = true;
            TitleLabel.BackColor = Color.White;
            TitleLabel.BlockID = null;
            TitleLabel.BorderColor = Color.Black;
            TitleLabel.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            TitleLabel.BorderRadius = 1;
            TitleLabel.BorderStyle = BorderStyle.FixedSingle;
            TitleLabel.BorderThickness = 1;
            TitleLabel.BottomoffsetForDrawingRect = 0;
            TitleLabel.BoundProperty = "Text";
            TitleLabel.CanBeFocused = true;
            TitleLabel.CanBeHovered = false;
            TitleLabel.CanBePressed = true;
            TitleLabel.Category = Utilities.DbFieldCategory.String;
            tableLayoutPanel1.tableLayoutPanel.SetColumnSpan(TitleLabel, 3);
            TitleLabel.ComponentName = "beepLabel1";
            TitleLabel.DataContext = null;
            TitleLabel.DataSourceProperty = null;
            TitleLabel.DisabledBackColor = Color.Gray;
            TitleLabel.DisabledForeColor = Color.Empty;
            TitleLabel.Dock = DockStyle.Fill;
            TitleLabel.DrawingRect = new Rectangle(2, 2, 743, 25);
            TitleLabel.Easing = EasingType.Linear;
            TitleLabel.FieldID = null;
            TitleLabel.FocusBackColor = Color.Gray;
            TitleLabel.FocusBorderColor = Color.Gray;
            TitleLabel.FocusForeColor = Color.Black;
            TitleLabel.FocusIndicatorColor = Color.Blue;
            TitleLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            TitleLabel.ForeColor = Color.FromArgb(0, 0, 0);
            TitleLabel.Form = null;
            TitleLabel.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            TitleLabel.GradientEndColor = Color.Gray;
            TitleLabel.GradientStartColor = Color.Gray;
            TitleLabel.GuidID = "cf00f7b4-87be-4872-9f17-afd4702aa6c6";
            TitleLabel.HideText = false;
            TitleLabel.HoverBackColor = Color.FromArgb(230, 230, 230);
            TitleLabel.HoverBorderColor = Color.Gray;
            TitleLabel.HoveredBackcolor = Color.Wheat;
            TitleLabel.HoverForeColor = Color.FromArgb(0, 0, 0);
            TitleLabel.Id = -1;
            TitleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            TitleLabel.ImagePath = null;
            TitleLabel.InactiveBackColor = Color.Gray;
            TitleLabel.InactiveBorderColor = Color.Gray;
            TitleLabel.InactiveForeColor = Color.Black;
            TitleLabel.IsAcceptButton = false;
            TitleLabel.IsBorderAffectedByTheme = true;
            TitleLabel.IsCancelButton = false;
            TitleLabel.IsChild = false;
            TitleLabel.IsCustomeBorder = false;
            TitleLabel.IsDefault = false;
            TitleLabel.IsDeleted = false;
            TitleLabel.IsDirty = false;
            TitleLabel.IsEditable = false;
            TitleLabel.IsFocused = false;
            TitleLabel.IsFramless = true;
            TitleLabel.IsHovered = false;
            TitleLabel.IsNew = false;
            TitleLabel.IsPressed = false;
            TitleLabel.IsReadOnly = false;
            TitleLabel.IsRounded = true;
            TitleLabel.IsRoundedAffectedByTheme = true;
            TitleLabel.IsSelected = false;
            TitleLabel.IsShadowAffectedByTheme = true;
            TitleLabel.IsVisible = false;
            TitleLabel.LabelBackColor = Color.Empty;
            TitleLabel.LeftoffsetForDrawingRect = 0;
            TitleLabel.LinkedProperty = null;
            TitleLabel.Location = new Point(4, 4);
            TitleLabel.Margin = new Padding(4);
            TitleLabel.MaxImageSize = new Size(16, 16);
            TitleLabel.Name = "TitleLabel";
            TitleLabel.OverrideFontSize = TypeStyleFontSize.None;
            TitleLabel.Padding = new Padding(1);
            TitleLabel.ParentBackColor = Color.Empty;
            TitleLabel.ParentControl = null;
            TitleLabel.PressedBackColor = Color.Gray;
            TitleLabel.PressedBorderColor = Color.Gray;
            TitleLabel.PressedForeColor = Color.Black;
            TitleLabel.RightoffsetForDrawingRect = 0;
            TitleLabel.SavedGuidID = null;
            TitleLabel.SavedID = null;
            TitleLabel.ShadowColor = Color.Black;
            TitleLabel.ShadowOffset = 0;
            TitleLabel.ShadowOpacity = 0.5F;
            TitleLabel.ShowAllBorders = true;
            TitleLabel.ShowBottomBorder = true;
            TitleLabel.ShowFocusIndicator = false;
            TitleLabel.ShowLeftBorder = true;
            TitleLabel.ShowRightBorder = true;
            TitleLabel.ShowShadow = false;
            TitleLabel.ShowTopBorder = true;
            TitleLabel.Size = new Size(747, 29);
            TitleLabel.SlideFrom = SlideDirection.Left;
            TitleLabel.StaticNotMoving = false;
            TitleLabel.TabIndex = 1;
            TitleLabel.TempBackColor = Color.Empty;
            TitleLabel.Text = "beepLabel1";
            TitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            TitleLabel.TextFont = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            TitleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            TitleLabel.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            TitleLabel.ToolTipText = "";
            TitleLabel.TopoffsetForDrawingRect = 0;
            TitleLabel.UseGradientBackground = false;
            TitleLabel.UseScaledFont = false;
            TitleLabel.UseThemeFont = false;
            // 
            // vScrollBar1
            // 
            vScrollBar1.Dock = DockStyle.Fill;
            vScrollBar1.Location = new Point(738, 50);
            vScrollBar1.Margin = new Padding(4);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(13, 507);
            vScrollBar1.TabIndex = 2;
            // 
            // beepDataNavigator1
            // 
            beepDataNavigator1.ActiveBackColor = Color.Gray;
            beepDataNavigator1.AnimationDuration = 500;
            beepDataNavigator1.AnimationType = DisplayAnimationType.None;
            beepDataNavigator1.ApplyThemeToChilds = true;
            beepDataNavigator1.BackColor = Color.FromArgb(240, 240, 240);
            beepDataNavigator1.BlockID = null;
            beepDataNavigator1.BorderColor = Color.Black;
            beepDataNavigator1.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            beepDataNavigator1.BorderRadius = 1;
            beepDataNavigator1.BorderStyle = BorderStyle.FixedSingle;
            beepDataNavigator1.BorderThickness = 1;
            beepDataNavigator1.BottomoffsetForDrawingRect = 0;
            beepDataNavigator1.BoundProperty = null;
            beepDataNavigator1.ButtonHeight = 15;
            beepDataNavigator1.ButtonSpacing = 5;
            beepDataNavigator1.ButtonWidth = 15;
            beepDataNavigator1.CanBeFocused = true;
            beepDataNavigator1.CanBeHovered = false;
            beepDataNavigator1.CanBePressed = true;
            beepDataNavigator1.Category = Utilities.DbFieldCategory.String;
            beepDataNavigator1.ComponentName = "beepDataNavigator1";
            beepDataNavigator1.DataContext = null;
            beepDataNavigator1.DataSourceProperty = null;
            beepDataNavigator1.DisabledBackColor = Color.Gray;
            beepDataNavigator1.DisabledForeColor = Color.Empty;
            beepDataNavigator1.Dock = DockStyle.Fill;
            beepDataNavigator1.DrawingRect = new Rectangle(1, 1, 716, 17);
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
            beepDataNavigator1.GuidID = "580af047-d9ab-4c6c-807e-02623a2f96bc";
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
            beepDataNavigator1.IsDeleted = false;
            beepDataNavigator1.IsDirty = false;
            beepDataNavigator1.IsEditable = false;
            beepDataNavigator1.IsFocused = false;
            beepDataNavigator1.IsFramless = true;
            beepDataNavigator1.IsHovered = false;
            beepDataNavigator1.IsNew = false;
            beepDataNavigator1.IsPressed = false;
            beepDataNavigator1.IsReadOnly = false;
            beepDataNavigator1.IsRounded = true;
            beepDataNavigator1.IsRoundedAffectedByTheme = true;
            beepDataNavigator1.IsSelected = false;
            beepDataNavigator1.IsShadowAffectedByTheme = false;
            beepDataNavigator1.IsVisible = false;
            beepDataNavigator1.LeftoffsetForDrawingRect = 0;
            beepDataNavigator1.LinkedProperty = null;
            beepDataNavigator1.Location = new Point(12, 597);
            beepDataNavigator1.Margin = new Padding(4);
            beepDataNavigator1.Name = "beepDataNavigator1";
            beepDataNavigator1.OverrideFontSize = TypeStyleFontSize.None;
            beepDataNavigator1.ParentBackColor = Color.Empty;
            beepDataNavigator1.ParentControl = null;
            beepDataNavigator1.PressedBackColor = Color.Gray;
            beepDataNavigator1.PressedBorderColor = Color.Gray;
            beepDataNavigator1.PressedForeColor = Color.Black;
            beepDataNavigator1.RightoffsetForDrawingRect = 0;
            beepDataNavigator1.SavedGuidID = null;
            beepDataNavigator1.SavedID = null;
            beepDataNavigator1.ShadowColor = Color.Black;
            beepDataNavigator1.ShadowOffset = 0;
            beepDataNavigator1.ShadowOpacity = 0.5F;
            beepDataNavigator1.ShowAllBorders = true;
            beepDataNavigator1.ShowBottomBorder = true;
            beepDataNavigator1.ShowFocusIndicator = false;
            beepDataNavigator1.ShowLeftBorder = true;
            beepDataNavigator1.ShowPrint = false;
            beepDataNavigator1.ShowRightBorder = true;
            beepDataNavigator1.ShowSendEmail = false;
            beepDataNavigator1.ShowShadow = false;
            beepDataNavigator1.ShowTopBorder = true;
            beepDataNavigator1.Size = new Size(718, 19);
            beepDataNavigator1.SlideFrom = SlideDirection.Left;
            beepDataNavigator1.StaticNotMoving = false;
            beepDataNavigator1.TabIndex = 4;
            beepDataNavigator1.TempBackColor = Color.Empty;
            beepDataNavigator1.Text = "beepDataNavigator1";
            beepDataNavigator1.Theme = Vis.Modules.EnumBeepThemes.DefaultTheme;
            beepDataNavigator1.ToolTipText = "";
            beepDataNavigator1.TopoffsetForDrawingRect = 0;
            beepDataNavigator1.UnitOfWork = null;
            beepDataNavigator1.UseGradientBackground = false;
            beepDataNavigator1.UseThemeFont = true;
            beepDataNavigator1.XOffset = 2;
            beepDataNavigator1.YOffset = 1;
            // 
            // hScrollBar1
            // 
            hScrollBar1.Dock = DockStyle.Fill;
            hScrollBar1.Location = new Point(12, 565);
            hScrollBar1.Margin = new Padding(4);
            hScrollBar1.Name = "hScrollBar1";
            hScrollBar1.Size = new Size(718, 16);
            hScrollBar1.TabIndex = 5;
            // 
            // GridtableLayoutPanel
            // 
            GridtableLayoutPanel.tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel1.tableLayoutPanel.SetColumnSpan(GridtableLayoutPanel, 2);
            GridtableLayoutPanel.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            GridtableLayoutPanel.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            GridtableLayoutPanel.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            GridtableLayoutPanel.Dock = DockStyle.Fill;
            GridtableLayoutPanel.Location = new Point(3, 49);
            GridtableLayoutPanel.Name = "GridtableLayoutPanel";
            GridtableLayoutPanel.tableLayoutPanel.RowCount = 2;
            GridtableLayoutPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 55.2062874F));
            GridtableLayoutPanel.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 44.7937126F));
            GridtableLayoutPanel.Size = new Size(728, 509);
            GridtableLayoutPanel.TabIndex = 6;
        }
    }
}
