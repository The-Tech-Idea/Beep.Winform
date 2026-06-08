using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Vis.Modules;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepGridProSelectionNavigationTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProSelectionNavigationTests()
        {
            _grid = new BeepGridPro();
        }

        public void Dispose()
        {
            _grid.Dispose();
        }

        private void BindSampleData(int count = 5)
        {
            var data = new List<GridTestPerson>();
            for (int i = 0; i < count; i++)
            {
                data.Add(new GridTestPerson { Name = $"Person_{i}", Age = 20 + i });
            }
            _grid.DataSource = data;
            _grid.RefreshData();
        }

        // ========== Selection Tests ==========

        [Fact]
        public void SelectionMode_Default_IsFullRowSelect()
        {
            Assert.Equal(BeepGridSelectionMode.FullRowSelect, _grid.SelectionMode);
        }

        [Fact]
        public void SelectionMode_SwitchTo_CellSelect()
        {
            BindSampleData();
            _grid.SelectionMode = BeepGridSelectionMode.CellSelect;
            Assert.Equal(BeepGridSelectionMode.CellSelect, _grid.SelectionMode);
        }

        [Fact]
        public void SelectionMode_SwitchTo_MultiRowSelect()
        {
            BindSampleData();
            _grid.SelectionMode = BeepGridSelectionMode.MultiRowSelect;
            Assert.Equal(BeepGridSelectionMode.MultiRowSelect, _grid.SelectionMode);
        }

        [Fact]
        public void SelectionMode_SwitchTo_FullColumnSelect()
        {
            BindSampleData();
            _grid.SelectionMode = BeepGridSelectionMode.FullColumnSelect;
            Assert.Equal(BeepGridSelectionMode.FullColumnSelect, _grid.SelectionMode);
        }

        [Fact]
        public void SelectCell_ValidIndex_DoesNotThrow()
        {
            BindSampleData();
            _grid.SelectCell(0, 0);
        }

        [Fact]
        public void SelectCell_InvalidRowIndex_DoesNotThrow()
        {
            BindSampleData();
            _grid.SelectCell(999, 0);
        }

        [Fact]
        public void CurrentRowIndex_Default_IsNegative()
        {
            BindSampleData();
            Assert.Equal(-1, _grid.CurrentRowIndex);
        }

        [Fact]
        public void SelectedRows_StartsEmpty()
        {
            BindSampleData();
            Assert.Empty(_grid.SelectedRows);
        }

        [Fact]
        public void ShowCheckBox_Default_IsFalse()
        {
            Assert.False(_grid.ShowCheckBox);
        }

        [Fact]
        public void ShowCheckBox_Toggle()
        {
            BindSampleData();
            _grid.ShowCheckBox = true;
            Assert.True(_grid.ShowCheckBox);
            _grid.ShowCheckBox = false;
            Assert.False(_grid.ShowCheckBox);
        }

        // ========== Navigation Tests ==========

        [Fact]
        public void MoveFirst_MoveNext_MoveLast_DoNotThrow()
        {
            BindSampleData(10);
            _grid.MoveFirst();
            _grid.MoveNext();
            _grid.MoveLast();
            _grid.MovePrevious();
        }

        [Fact]
        public void InsertNew_DeleteCurrent_DoNotThrow()
        {
            BindSampleData();
            _grid.InsertNew();
            _grid.DeleteCurrent();
        }

        [Fact]
        public void Save_Cancel_DoNotThrow()
        {
            BindSampleData();
            _grid.Save();
            _grid.Cancel();
        }

        // ========== Clipboard Tests ==========

        [Fact]
        public void CopyToClipboard_NoData_DoesNotThrow()
        {
            _grid.CopyToClipboard();
        }

        [Fact]
        public void CopyToClipboard_WithData_DoesNotThrow()
        {
            BindSampleData();
            _grid.CopyToClipboard(false, false);
        }

        [Fact]
        public void CutToClipboard_NoData_DoesNotThrow()
        {
            _grid.CutToClipboard();
        }

        [Fact]
        public void CopyCellToClipboard_NoData_DoesNotThrow()
        {
            _grid.CopyCellToClipboard();
        }

        // ========== Virtualization Tests ==========

        [Fact]
        public void EnableVirtualization_Default_IsFalse()
        {
            Assert.False(_grid.EnableVirtualization);
        }

        [Fact]
        public void EnableColumnVirtualization_Default_IsFalse()
        {
            Assert.False(_grid.EnableColumnVirtualization);
        }

        [Fact]
        public void EnableVirtualization_Toggle()
        {
            BindSampleData();
            _grid.EnableVirtualization = true;
            Assert.True(_grid.EnableVirtualization);
            _grid.EnableVirtualization = false;
            Assert.False(_grid.EnableVirtualization);
        }

        [Fact]
        public void EnableColumnVirtualization_Toggle()
        {
            BindSampleData();
            _grid.EnableColumnVirtualization = true;
            Assert.True(_grid.EnableColumnVirtualization);
            _grid.EnableColumnVirtualization = false;
            Assert.False(_grid.EnableColumnVirtualization);
        }

        // ========== Theme/Style Tests ==========

        [Fact]
        public void GridStyle_Default_IsBootstrap()
        {
            Assert.Equal(BeepGridStyle.Bootstrap, _grid.GridStyle);
        }

        [Fact]
        public void GridStyle_SwitchToAll_DoesNotThrow()
        {
            BindSampleData();

            _grid.GridStyle = BeepGridStyle.Default;
            _grid.GridStyle = BeepGridStyle.Clean;
            _grid.GridStyle = BeepGridStyle.Material;
            _grid.GridStyle = BeepGridStyle.Flat;
            _grid.GridStyle = BeepGridStyle.Compact;
            _grid.GridStyle = BeepGridStyle.Corporate;
            _grid.GridStyle = BeepGridStyle.Minimal;
            _grid.GridStyle = BeepGridStyle.Card;
            _grid.GridStyle = BeepGridStyle.Borderless;
            _grid.GridStyle = BeepGridStyle.Modern;
            _grid.GridStyle = BeepGridStyle.Bootstrap;

            Assert.Equal(BeepGridStyle.Bootstrap, _grid.GridStyle);
        }

        [Fact]
        public void ShowToolbar_Default_IsTrue()
        {
            Assert.True(_grid.ShowToolbar);
        }

        [Fact]
        public void ShowToolbar_Toggle()
        {
            BindSampleData();
            _grid.ShowToolbar = false;
            Assert.False(_grid.ShowToolbar);
            _grid.ShowToolbar = true;
            Assert.True(_grid.ShowToolbar);
        }

        [Fact]
        public void GridTitle_SetAndGet()
        {
            BindSampleData();
            _grid.GridTitle = "Test Grid";
            Assert.Equal("Test Grid", _grid.GridTitle);
        }

        [Fact]
        public void ReadOnly_Default_IsFalse()
        {
            Assert.False(_grid.ReadOnly);
        }

        [Fact]
        public void MultiSelect_Default_IsTrue()
        {
            Assert.True(_grid.MultiSelect);
        }

        // ========== Layout Tests ==========

        [Fact]
        public void RowHeight_SetAndGet_Minimum18()
        {
            _grid.RowHeight = 10;
            Assert.Equal(18, _grid.RowHeight);

            _grid.RowHeight = 30;
            Assert.Equal(30, _grid.RowHeight);
        }

        [Fact]
        public void ColumnHeaderHeight_SetAndGet_Minimum22()
        {
            _grid.ColumnHeaderHeight = 10;
            Assert.Equal(22, _grid.ColumnHeaderHeight);

            _grid.ColumnHeaderHeight = 40;
            Assert.Equal(40, _grid.ColumnHeaderHeight);
        }

        [Fact]
        public void ShowColumnHeaders_Default_IsTrue()
        {
            Assert.True(_grid.ShowColumnHeaders);
        }

        [Fact]
        public void ShowTopFilterPanel_Default_IsFalse()
        {
            Assert.False(_grid.ShowTopFilterPanel);
        }

        [Fact]
        public void AllowUserToResizeColumns_Default_IsTrue()
        {
            Assert.True(_grid.AllowUserToResizeColumns);
        }

        [Fact]
        public void AllowColumnReorder_Default_IsTrue()
        {
            Assert.True(_grid.AllowColumnReorder);
        }

        // ========== Toolbar Property Tests ==========

        [Fact]
        public void ToolbarState_NotNull()
        {
            Assert.NotNull(_grid.ToolbarState);
        }

        [Fact]
        public void ToolbarColor_HaveDefaultValues()
        {
            Assert.NotEqual(default, _grid.ToolbarBackColor);
            Assert.NotEqual(default, _grid.ToolbarForeColor);
            Assert.NotEqual(default, _grid.ToolbarBorderColor);
        }

        // ========== Invalidation Tests ==========

        [Fact]
        public void InvalidateRow_ValidIndex_DoesNotThrow()
        {
            BindSampleData();
            _grid.InvalidateRow(0);
        }

        [Fact]
        public void InvalidateRow_InvalidIndex_DoesNotThrow()
        {
            BindSampleData();
            _grid.InvalidateRow(999);
        }

        [Fact]
        public void InvalidateCell_ValidIndices_DoesNotThrow()
        {
            BindSampleData();
            _grid.InvalidateCell(0, 0);
        }

        [Fact]
        public void InvalidateRows_ValidRange_DoesNotThrow()
        {
            BindSampleData(10);
            _grid.InvalidateRows(0, 3);
        }

        // ========== LayoutPreset Tests ==========

        [Fact]
        public void LayoutPreset_SwitchToAllPresets_DoesNotThrow()
        {
            BindSampleData();

            var presets = Enum.GetValues<TheTechIdea.Beep.Winform.Controls.GridX.Layouts.GridLayoutPreset>();
            foreach (var preset in presets)
            {
                _grid.LayoutPreset = preset;
            }
        }

        // ========== VisibleRowCapacity Tests ==========

        [Fact]
        public void VisibleRowCapacity_ReturnsPositiveValue()
        {
            Assert.True(_grid.VisibleRowCapacity > 0);
        }

        // ========== AutoSize Tests ==========

        [Fact]
        public void AutoSizeColumnsMode_Default_IsNone()
        {
            Assert.Equal(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None, _grid.AutoSizeColumnsMode);
        }

        [Fact]
        public void AutoSizeTriggerMode_Default_IsOnDataBind()
        {
            Assert.Equal(AutoSizeTriggerMode.OnDataBind, _grid.AutoSizeTriggerMode);
        }

        // ========== ShowGridLines/AlternateRowColor Tests ==========

        [Fact]
        public void ShowGridLines_Toggle()
        {
            BindSampleData();
            bool initial = _grid.ShowGridLines;
            _grid.ShowGridLines = !initial;
            Assert.Equal(!initial, _grid.ShowGridLines);
            _grid.ShowGridLines = initial;
        }

        [Fact]
        public void AlternateRowColor_Toggle()
        {
            BindSampleData();
            bool initial = _grid.AlternateRowColor;
            _grid.AlternateRowColor = !initial;
            Assert.Equal(!initial, _grid.AlternateRowColor);
            _grid.AlternateRowColor = initial;
        }
    }
}
