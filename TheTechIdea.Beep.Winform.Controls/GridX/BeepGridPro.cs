using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Refactored, helper-driven grid control inspired by BeepSimpleGrid.")]
    [DisplayName("Beep Grid Pro")]
    public class BeepGridPro : BeepControl
    {
        internal Helpers.GridLayoutHelper Layout { get; }
        internal Helpers.GridDataHelper Data { get; }
        internal Helpers.GridRenderHelper Render { get; }
        internal Helpers.GridSelectionHelper Selection { get; }
        internal Helpers.GridInputHelper Input { get; }
        internal Helpers.GridScrollHelper Scroll { get; }
        internal Helpers.GridSortFilterHelper SortFilter { get; }
        internal Helpers.GridEditHelper Edit { get; }
        internal Helpers.GridThemeHelper ThemeHelper { get; }
        internal Helpers.GridNavigatorHelper Navigator { get; }

        [Browsable(true)]
        [Category("Data")] 
        public object DataSource 
        {
            get => Data.DataSource;
            set { Data.Bind(value); Invalidate(); }
        }

        [Browsable(false)]
        public BindingList<BeepRowConfig> Rows => Data.Rows;

        [Browsable(false)]
        public List<BeepColumnConfig> Columns => Data.Columns;

        [Browsable(true)]
        [Category("Layout")] 
        public int RowHeight 
        { 
            get => Layout.RowHeight;
            set { Layout.RowHeight = Math.Max(18, value); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public int ColumnHeaderHeight 
        { 
            get => Layout.ColumnHeaderHeight; 
            set { Layout.ColumnHeaderHeight = Math.Max(22, value); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public bool ShowColumnHeaders 
        { 
            get => Layout.ShowColumnHeaders; 
            set { Layout.ShowColumnHeaders = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowUserToResizeColumns { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowUserToResizeRows { get; set; } = false;

        public BeepGridPro()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            Layout = new Helpers.GridLayoutHelper(this);
            Data = new Helpers.GridDataHelper(this);
            Render = new Helpers.GridRenderHelper(this);
            Selection = new Helpers.GridSelectionHelper(this);
            Input = new Helpers.GridInputHelper(this);
            Scroll = new Helpers.GridScrollHelper(this);
            SortFilter = new Helpers.GridSortFilterHelper(this);
            Edit = new Helpers.GridEditHelper(this);
            ThemeHelper = new Helpers.GridThemeHelper(this);
            Navigator = new Helpers.GridNavigatorHelper(this);

            RowHeight = 25;
            ColumnHeaderHeight = 28;
            ShowColumnHeaders = true;

            this.MouseDown += (s, e) => Input.HandleMouseDown(e);
            this.MouseMove += (s, e) => Input.HandleMouseMove(e);
            this.MouseUp += (s, e) => Input.HandleMouseUp(e);
            this.MouseWheel += (s, e) => Scroll.HandleMouseWheel(e);
            this.KeyDown += (s, e) => Input.HandleKeyDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Layout.Recalculate();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Layout.EnsureCalculated();
            Render.Draw(e.Graphics);
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            ThemeHelper.ApplyTheme();
            Invalidate();
        }

        public void AutoGenerateColumns()
        {
            Data.AutoGenerateColumns();
            Invalidate();
        }

        public void RefreshGrid()
        {
            Data.RefreshRows();
            Invalidate();
        }

        public void SelectCell(int rowIndex, int columnIndex)
        {
            Selection.SelectCell(rowIndex, columnIndex);
            Invalidate();
        }

        public void AttachNavigator(BeepBindingNavigator navigator, object dataSource)
        {
            Navigator.Attach(navigator, dataSource);
        }
    }
}
