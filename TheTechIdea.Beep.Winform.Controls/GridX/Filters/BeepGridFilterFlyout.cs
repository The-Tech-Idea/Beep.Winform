using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls; // BeepButton, BeepComboBox, BeepCheckBoxBool
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filters
{
    /// <summary>
    /// A lightweight, modeless filter flyout inspired by the provided design.
    /// </summary>
    internal sealed partial class BeepGridFilterFlyout : Form
    {
        private readonly BeepGridPro _grid;

        public event EventHandler ExportRequested; // external export hook

        public BeepGridFilterFlyout(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.White;
            Size = new Size(340, 520);
            Padding = new Padding(10);

            InitializeComponent();

            // Shadow/elevation border
            this.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(40, 0, 0, 0));
                var r = new Rectangle(0, 0, Width - 1, Height - 1);
                e.Graphics.DrawRectangle(pen, r);
            };

            // Apply theme and behavior to Beep controls
            foreach (var cmb in new[] { cmbFilterCol1, cmbFilterVal1, cmbFilterCol2, cmbFilterVal2, cmbGroupBy, cmbOrderBy })
            {
                cmb.Theme = _grid.Theme;
                cmb.AutoWidthToContent = false;
                cmb.ComboBoxAutoSizeForMaterial = false;
            }
            foreach (var btn in new[] { btnExport, btnApply, btnReset, btnClose })
            {
                btn.Theme = _grid.Theme;
                btn.AutoSizeContent = true;
            }
            chkShowHeaders.Theme = _grid.Theme;
            chkShowHeaders.CurrentValue = _grid.ShowColumnHeaders;

            // Configure BeepRadioButton for order direction
            rbOrder.Theme = _grid.Theme;
          
            rbOrder.SelectedValue = "Ascending";

            // Wire events not handled in designer
            cmbFilterCol1.SelectedItemChanged += cmbFilterCol1_SelectedItemChanged;
            cmbFilterCol2.SelectedItemChanged += cmbFilterCol2_SelectedItemChanged;
            btnExport.Click += (s, e) => ExportRequested?.Invoke(this, EventArgs.Empty);
            btnApply.Click += (s, e) => ApplyFilters();
            btnReset.Click += (s, e) => ResetFilters();
            btnClose.Click += (s, e) => Close();

            PopulateFromGrid();

            Deactivate += (s, e) => Close();
        }

        private void cmbFilterCol1_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
            => PopulateValuesForColumn(cmbFilterCol1, cmbFilterVal1);
        private void cmbFilterCol2_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
            => PopulateValuesForColumn(cmbFilterCol2, cmbFilterVal2);

        private void PopulateFromGrid()
        {
            var visibleCols = _grid.Data.Columns.Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn).ToList();
            var items = visibleCols.Select(c => c.ColumnCaption ?? c.ColumnName).ToArray();

            void loadCols(BeepComboBox cmb)
            {
                var list = new BindingList<SimpleItem>(items.Select(s => new SimpleItem { Text = s, Value = s }).ToList());
                cmb.ListItems = list;
                if (list.Count > 0) cmb.SelectedItem = list[0];
            }

            loadCols(cmbFilterCol1);
            loadCols(cmbFilterCol2);
            loadCols(cmbGroupBy);
            loadCols(cmbOrderBy);

            // Chips for visible columns
            pnlChips.Controls.Clear();
            foreach (var col in _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn))
            {
                var chip = new CheckBox
                {
                    Text = col.ColumnCaption ?? col.ColumnName,
                    Checked = col.Visible,
                    AutoSize = true,
                    Appearance = Appearance.Button,
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(4),
                    Padding = new Padding(8, 4, 8, 4)
                };
                chip.FlatAppearance.BorderColor = Color.Silver;
                chip.CheckedChanged += (s, e) => col.Visible = chip.Checked;
                pnlChips.Controls.Add(chip);
            }

            // Initialize value combos based on first selections
            PopulateValuesForColumn(cmbFilterCol1, cmbFilterVal1);
            PopulateValuesForColumn(cmbFilterCol2, cmbFilterVal2);
        }

        private void PopulateValuesForColumn(BeepComboBox columnCombo, BeepComboBox valueCombo)
        {
            var sel = columnCombo.SelectedItem;
            if (sel == null) return;
            string colName = sel.Text;
            var col = _grid.Data.Columns.FirstOrDefault(c => (c.ColumnCaption ?? c.ColumnName) == colName);
            if (col == null)
            {
                valueCombo.ListItems = new BindingList<SimpleItem>();
                return;
            }

            // Collect distinct values from current rows for this column index
            var colIndex = _grid.Data.Columns.IndexOf(col);
            var values = _grid.Data.Rows
                .Select(r => r.Cells.Count > colIndex ? (r.Cells[colIndex].CellValue ?? string.Empty) : string.Empty)
                .Distinct()
                .Take(500) // safeguard
                .ToList();

            var list = new BindingList<SimpleItem>(values.Select(v => new SimpleItem { Text = Convert.ToString(v), Value = v }).ToList());
            valueCombo.ListItems = list;
            if (list.Count > 0) valueCombo.SelectedItem = list[0];
        }

        private void ApplyFilters()
        {
            try
            {
                // Apply Show Headers
                _grid.ShowColumnHeaders = chkShowHeaders.CurrentValue;

                // Order by
                var orderColName = cmbOrderBy.SelectedItem?.Text;
                if (!string.IsNullOrWhiteSpace(orderColName))
                {
                    var col = _grid.Data.Columns.FirstOrDefault(c => (c.ColumnCaption ?? c.ColumnName) == orderColName);
                    if (col != null)
                    {
                        // Use grid's SortFilter helper when available
                        var dir = rbOrder.SelectedValue == "Ascending" ? SortDirection.Ascending : SortDirection.Descending;
                        _grid.SortFilter?.Sort(col.ColumnName, dir);
                    }
                }

                // Quick filters: use first non-empty to call the simple contains filter API (best-effort)
                ApplyContainsFilterIfAny(cmbFilterCol1, cmbFilterVal1);
                ApplyContainsFilterIfAny(cmbFilterCol2, cmbFilterVal2);

                _grid.Invalidate();
            }
            catch { }
            finally { Close(); }
        }

        private void ApplyContainsFilterIfAny(BeepComboBox colCombo, BeepComboBox valCombo)
        {
            var colCaption = colCombo.SelectedItem?.Text;
            var contains = valCombo.SelectedItem?.Text ?? valCombo.SelectedText;
            if (string.IsNullOrWhiteSpace(colCaption) || string.IsNullOrEmpty(contains)) return;

            var col = _grid.Data.Columns.FirstOrDefault(c => (c.ColumnCaption ?? c.ColumnName) == colCaption);
            if (col != null)
            {
                try { _grid.SortFilter?.Filter(col.ColumnName, contains); } catch { }
            }
        }

        private void ResetFilters()
        {
            try
            {
                // Best-effort reset: rebind current data set to clear helper state
                _grid.Data.InitializeData();
                _grid.Layout.Recalculate();
                _grid.Invalidate();
            }
            catch { }
        }

        public void ShowNear(Rectangle anchorScreenRect)
        {
            var screen = Screen.FromRectangle(anchorScreenRect);
            int x = Math.Min(anchorScreenRect.Right - Width, screen.WorkingArea.Right - Width - 8);
            int y = Math.Min(anchorScreenRect.Bottom + 6, screen.WorkingArea.Bottom - Height - 8);
            x = Math.Max(screen.WorkingArea.Left + 8, x);
            y = Math.Max(screen.WorkingArea.Top + 8, y);
            Location = new Point(x, y);
            Show(_grid);
        }

        protected override bool ShowWithoutActivation => true;
    }
}
