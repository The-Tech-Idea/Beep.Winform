using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Smart tag action list for BeepMultiSplitter providing layout and appearance controls.
    /// </summary>
    internal sealed class BeepMultiSplitterActionList : BeepDesignerActionList
    {
        private BeepMultiSplitter? Splitter => Component as BeepMultiSplitter;
        private TableLayoutPanel? TableLayoutPanel => Splitter?.TableLayoutPanel;
        private IComponentChangeService? ChangeService;

        public BeepMultiSplitterActionList(BaseBeepParentControlDesigner designer)
            : base(designer)
        {
            ChangeService = Designer?.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        public int ColumnCount
        {
            get => TableLayoutPanel?.ColumnCount ?? 1;
            set
            {
                if (TableLayoutPanel != null && value >= 1)
                {
                    ChangeService?.OnComponentChanging(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["ColumnCount"]);
                    TableLayoutPanel.ColumnCount = value;
                    ChangeService?.OnComponentChanged(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["ColumnCount"], null, null);
                    Splitter?.Invalidate();
                }
            }
        }

        public int RowCount
        {
            get => TableLayoutPanel?.RowCount ?? 1;
            set
            {
                if (TableLayoutPanel != null && value >= 1)
                {
                    ChangeService?.OnComponentChanging(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["RowCount"]);
                    TableLayoutPanel.RowCount = value;
                    ChangeService?.OnComponentChanged(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["RowCount"], null, null);
                    Splitter?.Invalidate();
                }
            }
        }

        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get => TableLayoutPanel?.CellBorderStyle ?? TableLayoutPanelCellBorderStyle.Single;
            set
            {
                if (TableLayoutPanel != null)
                {
                    ChangeService?.OnComponentChanging(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["CellBorderStyle"]);
                    TableLayoutPanel.CellBorderStyle = value;
                    ChangeService?.OnComponentChanged(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["CellBorderStyle"], null, null);
                    Splitter?.Invalidate();
                }
            }
        }

        public void AddRow()
        {
            if (TableLayoutPanel == null) return;

            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            int rowIndex = TableLayoutPanel.RowCount;
            TableLayoutPanel.RowCount++;
            TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            for (int i = 0; i < TableLayoutPanel.ColumnCount; i++)
            {
                var label = new Label
                {
                    Text = $"Cell {rowIndex + 1},{i + 1}",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                TableLayoutPanel.Controls.Add(label, i, rowIndex);
            }
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter?.Invalidate();
        }

        public void AddColumn()
        {
            if (TableLayoutPanel == null) return;

            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            int colIndex = TableLayoutPanel.ColumnCount;
            TableLayoutPanel.ColumnCount++;
            TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for (int i = 0; i < TableLayoutPanel.RowCount; i++)
            {
                var label = new Label
                {
                    Text = $"Cell {i + 1},{colIndex + 1}",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                TableLayoutPanel.Controls.Add(label, colIndex, i);
            }
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter?.Invalidate();
        }

        public void RemoveLastRow()
        {
            if (TableLayoutPanel == null || TableLayoutPanel.RowCount <= 1) return;

            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            int rowIndex = TableLayoutPanel.RowCount - 1;

            for (int i = TableLayoutPanel.Controls.Count - 1; i >= 0; i--)
            {
                var control = TableLayoutPanel.Controls[i];
                if (TableLayoutPanel.GetRow(control) == rowIndex)
                {
                    TableLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }
            }

            TableLayoutPanel.RowStyles.RemoveAt(rowIndex);
            TableLayoutPanel.RowCount--;
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter?.Invalidate();
        }

        public void RemoveLastColumn()
        {
            if (TableLayoutPanel == null || TableLayoutPanel.ColumnCount <= 1) return;

            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            int colIndex = TableLayoutPanel.ColumnCount - 1;

            for (int i = TableLayoutPanel.Controls.Count - 1; i >= 0; i--)
            {
                var control = TableLayoutPanel.Controls[i];
                if (TableLayoutPanel.GetColumn(control) == colIndex)
                {
                    TableLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }
            }

            TableLayoutPanel.ColumnStyles.RemoveAt(colIndex);
            TableLayoutPanel.ColumnCount--;
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter?.Invalidate();
        }

        protected override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("ColumnCount", "Column Count", "Layout", "Number of columns"));
            items.Add(new DesignerActionPropertyItem("RowCount", "Row Count", "Layout", "Number of rows"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddRow), "Add Row", "Layout", "Add a new row", true));
            items.Add(new DesignerActionMethodItem(this, nameof(AddColumn), "Add Column", "Layout", "Add a new column", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveLastRow), "Remove Last Row", "Layout", "Remove the last row", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveLastColumn), "Remove Last Column", "Layout", "Remove the last column", true));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("CellBorderStyle", "Cell Border Style", "Appearance", "Border style for cells"));

            return items;
        }
    }
}
