using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Grouping;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridTestProduct
    {
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class BeepGridProGroupSummaryTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProGroupSummaryTests()
        {
            _grid = new BeepGridPro();
        }

        public void Dispose()
        {
            _grid.Dispose();
        }

        private void BindProductData()
        {
            var data = new List<GridTestProduct>
            {
                new() { Category = "Electronics", Price = 100m },
                new() { Category = "Electronics", Price = 200m },
                new() { Category = "Clothing", Price = 50m },
                new() { Category = "Clothing", Price = 75m },
                new() { Category = "Electronics", Price = 300m }
            };
            _grid.DataSource = data;
            _grid.RefreshData();
        }

        // ========== Group Summary Row Engine ==========

        [Fact]
        public void ComputeForGroup_NoAggregateColumns_SummaryRowExistsButEmpty()
        {
            BindProductData();
            _grid.GroupBy("Category");

            var group = _grid.GroupEngine.Groups.First();
            GridGroupAggregateEngine.ComputeForGroup(_grid, group);

            Assert.NotNull(group.SummaryRow);
            Assert.Empty(group.SummaryRow.Values);
        }

        [Fact]
        public void ComputeForGroup_SumAggregate_ComputesSum()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.Sum;

            _grid.GroupBy("Category");

            var electronicsGroup = _grid.GroupEngine.Groups.First(g => g.Key == "Electronics");
            GridGroupAggregateEngine.ComputeForGroup(_grid, electronicsGroup);

            Assert.NotNull(electronicsGroup.SummaryRow);
            Assert.Contains("Price", electronicsGroup.SummaryRow.Values.Keys);
            Assert.Equal(600m, Convert.ToDecimal(electronicsGroup.SummaryRow.Values["Price"]));
        }

        [Fact]
        public void ComputeForGroup_AverageAggregate_ComputesAverage()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.Average;

            _grid.GroupBy("Category");

            var clothingGroup = _grid.GroupEngine.Groups.First(g => g.Key == "Clothing");
            GridGroupAggregateEngine.ComputeForGroup(_grid, clothingGroup);

            Assert.Equal(62.5m, Convert.ToDecimal(clothingGroup.SummaryRow.Values["Price"]));
        }

        [Fact]
        public void ComputeForGroup_CountAggregate_CountsAllValues()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.Count;

            _grid.GroupBy("Category");

            var electronicsGroup = _grid.GroupEngine.Groups.First(g => g.Key == "Electronics");
            GridGroupAggregateEngine.ComputeForGroup(_grid, electronicsGroup);

            Assert.Equal(3, Convert.ToInt32(electronicsGroup.SummaryRow.Values["Price"]));
        }

        [Fact]
        public void ComputeForGroup_MinAggregate_ComputesMinimum()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.Min;

            _grid.GroupBy("Category");

            var electronicsGroup = _grid.GroupEngine.Groups.First(g => g.Key == "Electronics");
            GridGroupAggregateEngine.ComputeForGroup(_grid, electronicsGroup);

            Assert.Equal(100m, Convert.ToDecimal(electronicsGroup.SummaryRow.Values["Price"]));
        }

        [Fact]
        public void ComputeForGroup_MaxAggregate_ComputesMaximum()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.Max;

            _grid.GroupBy("Category");

            var clothingGroup = _grid.GroupEngine.Groups.First(g => g.Key == "Clothing");
            GridGroupAggregateEngine.ComputeForGroup(_grid, clothingGroup);

            Assert.Equal(75m, Convert.ToDecimal(clothingGroup.SummaryRow.Values["Price"]));
        }

        [Fact]
        public void ComputeForGroup_FirstAggregate_ReturnsFirstValue()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.First;

            _grid.GroupBy("Category");

            var electronicsGroup = _grid.GroupEngine.Groups.First(g => g.Key == "Electronics");
            GridGroupAggregateEngine.ComputeForGroup(_grid, electronicsGroup);

            Assert.NotNull(electronicsGroup.SummaryRow.Values["Price"]);
        }

        [Fact]
        public void ComputeForGroup_LastAggregate_ReturnsLastValue()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.Last;

            _grid.GroupBy("Category");

            var clothingGroup = _grid.GroupEngine.Groups.First(g => g.Key == "Clothing");
            GridGroupAggregateEngine.ComputeForGroup(_grid, clothingGroup);

            Assert.NotNull(clothingGroup.SummaryRow.Values["Price"]);
        }

        [Fact]
        public void ComputeForGroup_DistinctCountAggregate_CountsDistinct()
        {
            var data = new List<GridTestProduct>
            {
                new() { Category = "A", Price = 10m },
                new() { Category = "A", Price = 10m },
                new() { Category = "A", Price = 20m },
                new() { Category = "A", Price = 30m }
            };
            _grid.DataSource = data;
            _grid.RefreshData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.DistinctCount;

            _grid.GroupBy("Category");

            var group = _grid.GroupEngine.Groups.First();
            GridGroupAggregateEngine.ComputeForGroup(_grid, group);

            Assert.Equal(3, Convert.ToInt32(group.SummaryRow.Values["Price"]));
        }

        [Fact]
        public void ComputeForGroup_CustomAggregate_ReturnsNull()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.Custom;

            _grid.GroupBy("Category");

            var group = _grid.GroupEngine.Groups.First();
            GridGroupAggregateEngine.ComputeForGroup(_grid, group);

            Assert.Null(group.SummaryRow.Values["Price"]);
        }

        [Fact]
        public void ComputeForGroup_NoneAggregate_ColumnExcluded()
        {
            BindProductData();

            var priceCol = _grid.Columns.First(c => c.ColumnName == "Price");
            priceCol.AggregationType = AggregationType.None;

            _grid.GroupBy("Category");

            var group = _grid.GroupEngine.Groups.First();
            GridGroupAggregateEngine.ComputeForGroup(_grid, group);

            Assert.DoesNotContain("Price", group.SummaryRow.Values.Keys);
        }

        // ========== SummaryRow Properties ==========

        [Fact]
        public void SummaryRow_Height_DefaultIs22()
        {
            var row = new GridGroupSummaryRow { Group = null! };
            Assert.Equal(22, row.Height);
        }

        [Fact]
        public void SummaryRow_Height_SetAndGet()
        {
            var row = new GridGroupSummaryRow { Group = null!, Height = 40 };
            Assert.Equal(40, row.Height);
        }

        [Fact]
        public void SummaryRow_IsVisible_NullGroup_False()
        {
            var row = new GridGroupSummaryRow { Group = null! };
            // Group?.IsCollapsed == null, and null == false is false
            Assert.False(row.IsVisible);
        }

        [Fact]
        public void ComputeForGroup_SetsHeightAtLeast22()
        {
            BindProductData();

            _grid.GroupBy("Category");

            var group = _grid.GroupEngine.Groups.First();
            GridGroupAggregateEngine.ComputeForGroup(_grid, group);

            // Height = Math.Max(22, grid.RowHeight) - must be at least 22
            Assert.True(group.SummaryRow.Height >= 22);
        }
    }
}
