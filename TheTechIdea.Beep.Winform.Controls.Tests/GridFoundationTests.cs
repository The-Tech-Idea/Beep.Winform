using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridFoundationTests
    {
        [Fact]
        public void RowHeight_ExplicitValueSurvivesRecalculation()
        {
            using var grid = CreateGrid();
            grid.RowHeight = 37;

            grid.RecalculateHeightsFromPainters();

            Assert.Equal(37, grid.RowHeight);
        }

        [Fact]
        public void ColumnHeaderHeight_ExplicitValueSurvivesRecalculation()
        {
            using var grid = CreateGrid();
            grid.ColumnHeaderHeight = 41;

            grid.RecalculateHeightsFromPainters();

            Assert.Equal(41, grid.ColumnHeaderHeight);
        }

        [Fact]
        public void SwitchingFromCardToBootstrap_ResetsCardFlags()
        {
            using var grid = CreateGrid();
            grid.GridStyle = BeepGridStyle.Card;
            Assert.True(grid.Render.CardStyle);
            Assert.True(grid.Render.UseElevation);

            grid.GridStyle = BeepGridStyle.Bootstrap;

            Assert.False(grid.Render.CardStyle);
            Assert.False(grid.Render.UseElevation);
        }

        [Fact]
        public void ApplyTheme_KeepsTheHostNavigationStyle()
        {
            using var grid = CreateGrid();
            grid.GridStyle = BeepGridStyle.Material;
            grid.NavigationStyle = navigationStyle.Compact;

            grid.ApplyTheme();

            Assert.Equal(navigationStyle.Compact, grid.NavigationStyle);
        }

        [Fact]
        public void RowMetrics_ReflectVisibilityAndExposeRowOffsets()
        {
            using var grid = new BeepGridPro { RowHeight = 20 };
            grid.DataSource = new List<FoundationPerson>
            {
                new() { Name = "A", Amount = 1 },
                new() { Name = "B", Amount = 2 },
                new() { Name = "C", Amount = 3 }
            };
            grid.AutoGenerateColumns();
            grid.Layout.Recalculate();

            var metrics = grid.Layout.RowMetrics;
            Assert.Equal(60, metrics.TotalHeight);
            Assert.Equal(0, metrics.TopOfRow(0));
            Assert.Equal(20, metrics.TopOfRow(1));
            Assert.Equal(1, metrics.FirstVisibleRow(25));

            grid.Rows[0].IsVisible = false;
            grid.Layout.RowMetrics.Invalidate();
            grid.Layout.Recalculate();

            Assert.Equal(40, metrics.TotalHeight);
            Assert.Equal(0, metrics.TopOfRow(1));
            Assert.Equal(1, metrics.FirstVisibleRow(10));
        }

        [Fact]
        public void BindingList_IsOwnedByDataHelperAndItemChangesReachTheGrid()
        {
            var list = new BindingList<InpcPerson>
            {
                new() { Name = "Alice", Amount = 10 }
            };
            using var grid = new BeepGridPro();
            grid.DataSource = list;
            grid.AutoGenerateColumns();

            Assert.True(grid.Data.IsSubscribedTo(list));

            list[0].Name = "Changed";

            var nameCell = grid.Rows[0].Cells.First(c => c.ColumnName == "Name");
            Assert.Equal("Changed", nameCell.CellValue);
        }

        [Fact]
        public void HidingToolbar_DoesNotSilentlyEnableTheLegacyFilterPanel()
        {
            using var grid = new BeepGridPro();
            grid.ShowTopFilterPanel = false;

            grid.ShowToolbar = false;

            Assert.False(grid.ShowTopFilterPanel);
        }

        [Fact]
        public void PropertyLookup_IsCaseInsensitive()
        {
            using var grid = new BeepGridPro();
            grid.DataSource = new List<FoundationPerson>
            {
                new() { Name = "Alice", Amount = 10 }
            };
            grid.AutoGenerateColumns();

            var nameColumn = grid.GetColumnByName("Name")!;
            nameColumn.ColumnName = "name";
            grid.RefreshGrid();

            var cell = grid.Rows[0].Cells.First(c =>
                string.Equals(c.ColumnName, "name", StringComparison.OrdinalIgnoreCase));
            Assert.Equal("Alice", cell.CellValue);
        }

        private sealed class InpcPerson : INotifyPropertyChanged
        {
            private string _name = string.Empty;
            public string Name
            {
                get => _name;
                set
                {
                    if (_name == value) return;
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }

            public int Amount { get; set; }
            public event PropertyChangedEventHandler? PropertyChanged;
        }

        private static BeepGridPro CreateGrid()
        {
            var grid = new BeepGridPro();
            grid.DataSource = new List<FoundationPerson>
            {
                new() { Name = "Alice", Amount = 30 }
            };
            grid.AutoGenerateColumns();
            return grid;
        }

        private sealed class FoundationPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
        }
    }
}
