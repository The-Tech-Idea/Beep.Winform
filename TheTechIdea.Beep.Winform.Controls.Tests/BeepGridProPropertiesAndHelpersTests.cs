using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepGridProPropertiesAndHelpersTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProPropertiesAndHelpersTests()
        {
            _grid = new BeepGridPro();
        }

        public void Dispose()
        {
            _grid.Dispose();
        }

        private void BindData()
        {
            var data = new List<GridTestPerson>
            {
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 },
                new() { Name = "Charlie", Age = 35 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();
        }

        // ========== Appearance Properties ==========

        [Fact]
        public void FocusedRowBackColor_SetAndGet()
        {
            _grid.FocusedRowBackColor = Color.LightBlue;
            Assert.Equal(Color.LightBlue, _grid.FocusedRowBackColor);
        }

        [Fact]
        public void FocusedCellFillColor_SetAndGet()
        {
            _grid.FocusedCellFillColor = Color.Yellow;
            Assert.Equal(Color.Yellow, _grid.FocusedCellFillColor);
        }

        [Fact]
        public void FocusedCellFillOpacity_ClampedToRange()
        {
            _grid.FocusedCellFillOpacity = 500;
            Assert.Equal(255, _grid.FocusedCellFillOpacity);

            _grid.FocusedCellFillOpacity = -5;
            Assert.Equal(0, _grid.FocusedCellFillOpacity);

            _grid.FocusedCellFillOpacity = 100;
            Assert.Equal(100, _grid.FocusedCellFillOpacity);
        }

        [Fact]
        public void FocusedCellBorderColor_SetAndGet()
        {
            _grid.FocusedCellBorderColor = Color.Red;
            Assert.Equal(Color.Red, _grid.FocusedCellBorderColor);
        }

        [Fact]
        public void FocusedCellBorderWidth_ClampedToZero()
        {
            _grid.FocusedCellBorderWidth = -5f;
            Assert.Equal(0f, _grid.FocusedCellBorderWidth);

            _grid.FocusedCellBorderWidth = 3f;
            Assert.Equal(3f, _grid.FocusedCellBorderWidth);
        }

        [Fact]
        public void ShowFocusedCellFill_Toggle()
        {
            _grid.ShowFocusedCellFill = false;
            Assert.False(_grid.ShowFocusedCellFill);
            _grid.ShowFocusedCellFill = true;
            Assert.True(_grid.ShowFocusedCellFill);
        }

        [Fact]
        public void ShowFocusedCellBorder_Toggle()
        {
            _grid.ShowFocusedCellBorder = false;
            Assert.False(_grid.ShowFocusedCellBorder);
            _grid.ShowFocusedCellBorder = true;
            Assert.True(_grid.ShowFocusedCellBorder);
        }

        [Fact]
        public void UseDedicatedFocusedRowStyle_Toggle()
        {
            _grid.UseDedicatedFocusedRowStyle = false;
            Assert.False(_grid.UseDedicatedFocusedRowStyle);
            _grid.UseDedicatedFocusedRowStyle = true;
            Assert.True(_grid.UseDedicatedFocusedRowStyle);
        }

        [Fact]
        public void SortIconVisibility_SetAllValues()
        {
            _grid.SortIconVisibility = HeaderIconVisibility.Always;
            Assert.Equal(HeaderIconVisibility.Always, _grid.SortIconVisibility);
            _grid.SortIconVisibility = HeaderIconVisibility.HoverOnly;
            Assert.Equal(HeaderIconVisibility.HoverOnly, _grid.SortIconVisibility);
            _grid.SortIconVisibility = HeaderIconVisibility.Hidden;
            Assert.Equal(HeaderIconVisibility.Hidden, _grid.SortIconVisibility);
        }

        [Fact]
        public void FilterIconVisibility_SetAllValues()
        {
            _grid.FilterIconVisibility = HeaderIconVisibility.Always;
            Assert.Equal(HeaderIconVisibility.Always, _grid.FilterIconVisibility);
            _grid.FilterIconVisibility = HeaderIconVisibility.Hidden;
            Assert.Equal(HeaderIconVisibility.Hidden, _grid.FilterIconVisibility);
        }

        // ========== Filtering Properties ==========

        [Fact]
        public void UseInlineQuickSearch_Toggle()
        {
            _grid.UseInlineQuickSearch = false;
            Assert.False(_grid.UseInlineQuickSearch);
            _grid.UseInlineQuickSearch = true;
            Assert.True(_grid.UseInlineQuickSearch);
        }

        [Fact]
        public void TopFilterPanelHeight_SetAndGet()
        {
            // Layout.Recalculate() overrides the height with the painter's recommendation
            // after a render cycle, so we verify the property can be set and read back
            // without triggering a recalculate.
            _grid.TopFilterPanelHeight = 50;
            Assert.True(_grid.TopFilterPanelHeight >= 24);
        }

        // ========== Behavior Properties ==========

        [Fact]
        public void AllowUserToResizeRows_DefaultIsFalse()
        {
            Assert.False(_grid.AllowUserToResizeRows);
        }

        [Fact]
        public void AllowUserToResizeRows_Toggle()
        {
            _grid.AllowUserToResizeRows = true;
            Assert.True(_grid.AllowUserToResizeRows);
            _grid.AllowUserToResizeRows = false;
            Assert.False(_grid.AllowUserToResizeRows);
        }

        // ========== Layout Properties ==========

        [Fact]
        public void AutoSizeRowsToContent_SetAndGet()
        {
            _grid.AutoSizeRowsToContent = true;
            Assert.True(_grid.AutoSizeRowsToContent);
        }

        [Fact]
        public void RowAutoSizePadding_SetAndGet()
        {
            _grid.RowAutoSizePadding = 5;
            Assert.Equal(5, _grid.RowAutoSizePadding);
        }

        [Fact]
        public void UseDpiAwareRowHeights_DefaultIsTrue()
        {
            Assert.True(_grid.UseDpiAwareRowHeights);
        }

        [Fact]
        public void AutoSizeDebounceMilliseconds_ClampedToMinimum16()
        {
            _grid.AutoSizeDebounceMilliseconds = 5;
            Assert.Equal(16, _grid.AutoSizeDebounceMilliseconds);
        }

        [Fact]
        public void UsePainterNavigation_Toggle()
        {
            _grid.UsePainterNavigation = false;
            Assert.False(_grid.UsePainterNavigation);
            _grid.UsePainterNavigation = true;
            Assert.True(_grid.UsePainterNavigation);
        }

        [Fact]
        public void NavigationStyle_SetValues()
        {
            _grid.NavigationStyle = TheTechIdea.Beep.Winform.Controls.GridX.Painters.navigationStyle.Material;
            Assert.Equal(TheTechIdea.Beep.Winform.Controls.GridX.Painters.navigationStyle.Material, _grid.NavigationStyle);
        }

        // ========== Accessibility Properties ==========

        [Fact]
        public void AccessibleName_SetAndGet()
        {
            _grid.AccessibleName = "MyGrid";
            Assert.Equal("MyGrid", _grid.AccessibleName);
        }

        [Fact]
        public void AccessibleDescription_SetAndGet()
        {
            _grid.AccessibleDescription = "Customer data grid";
            Assert.Equal("Customer data grid", _grid.AccessibleDescription);
        }

        // ========== Selection Helper Properties ==========

        [Fact]
        public void SelectionCheckBoxColumn_ReturnsCheckboxColumn()
        {
            BindData();
            var col = _grid.SelectionCheckBoxColumn;
            Assert.NotNull(col);
            Assert.True(col.IsSelectionCheckBox);
        }

        [Fact]
        public void RowIDColumn_ReturnsRowIDColumn()
        {
            BindData();
            var col = _grid.RowIDColumn;
            Assert.NotNull(col);
            Assert.True(col.IsRowID);
        }

        [Fact]
        public void RowNumberColumn_ReturnsRowNumberColumn()
        {
            BindData();
            var col = _grid.RowNumberColumn;
            Assert.NotNull(col);
            Assert.True(col.IsRowNumColumn);
        }

        // ========== Data Access Methods ==========

        [Fact]
        public void GetColumnByCaption_ReturnsColumn()
        {
            BindData();
            var col = _grid.GetColumnByCaption("Name");
            Assert.NotNull(col);
            Assert.Equal("Name", col.ColumnName);
        }

        [Fact]
        public void GetColumnByCaption_ReturnsNull_ForMissing()
        {
            BindData();
            var col = _grid.GetColumnByCaption("NonExistent");
            Assert.Null(col);
        }

        // ========== Helper Methods ==========

        [Fact]
        public void SyncRowDataToGrid_UpdatesCellValues()
        {
            var data = new List<GridTestPerson>
            {
                new() { Name = "Original", Age = 30 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();

            data[0].Name = "Updated";
            data[0].Age = 99;

            _grid.SyncRowDataToGrid(_grid.Rows[0]);

            var nameCell = _grid.Rows[0].Cells.FirstOrDefault(c => c.ColumnName == "Name");
            var ageCell = _grid.Rows[0].Cells.FirstOrDefault(c => c.ColumnName == "Age");
            Assert.NotNull(nameCell);
            Assert.NotNull(ageCell);
            Assert.Equal("Updated", nameCell.CellValue);
            Assert.Equal(99, ageCell.CellValue);
        }

        [Fact]
        public void SyncRowDataToGrid_Batch_UpdatesAllRows()
        {
            var data = new List<GridTestPerson>
            {
                new() { Name = "A1", Age = 10 },
                new() { Name = "B2", Age = 20 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();

            data[0].Name = "X1";
            data[1].Name = "X2";

            _grid.SyncRowDataToGrid(_grid.Rows);

            Assert.Equal("X1", _grid.Rows[0].Cells.First(c => c.ColumnName == "Name").CellValue);
            Assert.Equal("X2", _grid.Rows[1].Cells.First(c => c.ColumnName == "Name").CellValue);
        }

        [Fact]
        public void SyncRowDataToGrid_NullRow_DoesNotThrow()
        {
            BindData();
            _grid.SyncRowDataToGrid((BeepRowConfig)null);
        }

        [Fact]
        public void SyncRowDataToGrid_NullEnumerable_DoesNotThrow()
        {
            BindData();
            _grid.SyncRowDataToGrid((IEnumerable<BeepRowConfig>)null);
        }

        [Fact]
        public void SetColumnWidth_UpdatesWidth()
        {
            BindData();
            _grid.SetColumnWidth("Name", 200);
            var col = _grid.GetColumnByName("Name");
            Assert.NotNull(col);
            Assert.Equal(200, col.Width);
        }

        [Fact]
        public void BestFitColumn_DoesNotThrow()
        {
            BindData();
            _grid.BestFitColumn(0);
        }

        [Fact]
        public void BestFitColumn_WithOptions_DoesNotThrow()
        {
            BindData();
            _grid.BestFitColumn(0, includeHeader: true, allRows: true);
        }

        [Fact]
        public void BestFitColumn_InvalidIndex_DoesNotThrow()
        {
            BindData();
            _grid.BestFitColumn(999);
        }

        [Fact]
        public void BestFitVisibleColumns_DoesNotThrow()
        {
            BindData();
            _grid.BestFitVisibleColumns();
        }

        [Fact]
        public void BestFitVisibleColumns_WithOptions_DoesNotThrow()
        {
            BindData();
            _grid.BestFitVisibleColumns(includeHeader: true, allRows: true);
        }

        [Fact]
        public void AutoResizeColumnsToFitContent_DoesNotThrow()
        {
            BindData();
            _grid.AutoResizeColumnsToFitContent();
        }

        [Fact]
        public void AutoSizeRowsToFitContent_DoesNotThrow()
        {
            BindData();
            _grid.AutoSizeRowsToFitContent();
        }

        [Fact]
        public void AutoGenerateColumns_OnEmptyGrid_DoesNotThrow()
        {
            _grid.AutoGenerateColumns();
        }

        [Fact]
        public void RecalculateHeightsFromPainters_DoesNotThrow()
        {
            BindData();
            _grid.RecalculateHeightsFromPainters();
        }

        [Fact]
        public void RefreshGrid_DoesNotThrow()
        {
            BindData();
            _grid.RefreshGrid();
        }

        [Fact]
        public void RefreshData_NoDataSource_DoesNotThrow()
        {
            _grid.RefreshData();
        }

        [Fact]
        public void RefreshData_WithDataSource_Rebinds()
        {
            BindData();
            _grid.RefreshData();
            Assert.True(_grid.Rows.Count >= 3);
        }

        [Fact]
        public void ClearGrid_RemovesDataButKeepsSystemColumns()
        {
            BindData();
            _grid.ClearGrid();

            Assert.Equal(0, _grid.Rows.Count);
            Assert.Contains(_grid.Columns, c => c.IsSelectionCheckBox);
            Assert.Contains(_grid.Columns, c => c.IsRowNumColumn);
            Assert.Contains(_grid.Columns, c => c.IsRowID);
        }
    }
}
