using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Query;
using TheTechIdea.Beep.Winform.Controls.GridX.Virtualization;
using TheTechIdea.Beep.Winform.Controls.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridQueryPipelineTests
    {
        [Fact]
        public void Query_FilterSortAndPage_ReturnsExpectedRows()
        {
            var rows = new List<GridViewRow>
            {
                Row(0, "Alpha", 30, "EMEA"),
                Row(1, "Beta", 10, "EMEA"),
                Row(2, "Gamma", 20, "APAC"),
                Row(3, "Delta", 5, "APAC")
            };

            var query = new GridQuery
            {
                PageSize = 2,
                Filters =
                {
                    new GridFilterGroup
                    {
                        Conditions =
                        {
                            new GridFilterCondition
                            {
                                ColumnName = "Amount",
                                Operator = FilterOperator.GreaterThan,
                                Value = "8"
                            }
                        }
                    }
                },
                Sorts =
                {
                    new GridSortDescriptor { ColumnName = "Amount", Direction = SortDirection.Descending }
                }
            };

            var result = GridQueryEngine.Execute(rows, query);

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.Rows.Count);
            Assert.Equal("Alpha", result.Rows[0].Values["Name"]);
            Assert.Equal("Gamma", result.Rows[1].Values["Name"]);
            Assert.Equal(0, result.Rows[0].DisplayIndex);
        }

        [Fact]
        public void Query_Grouping_OrdersGroupsAndSortsWithinThem()
        {
            var rows = new List<GridViewRow>
            {
                Row(0, "Alpha", 30, "EMEA"),
                Row(1, "Beta", 10, "EMEA"),
                Row(2, "Gamma", 20, "APAC")
            };

            var query = new GridQuery();
            query.Groups.Add(new GroupDescriptor { ColumnName = "Region" });
            query.Sorts.Add(new GridSortDescriptor { ColumnName = "Amount", Direction = SortDirection.Descending });

            var result = GridQueryEngine.Execute(rows, query);

            Assert.All(result.Rows, row => Assert.Single(row.GroupPath));
            Assert.Equal("APAC", result.Rows[0].GroupPath[0]);
            Assert.Equal("Gamma", result.Rows[0].Values["Name"]);
        }

        [Fact]
        public async Task VirtualSource_QueryAsync_UsesTheSamePipeline()
        {
            var source = GridVirtualDataSource.FromList(
                new List<QueryItem>
                {
                    new("Alpha", 30, "EMEA"),
                    new("Beta", 10, "EMEA"),
                    new("Gamma", 20, "APAC"),
                    new("Delta", 5, "APAC")
                },
                new[] { "Name", "Amount", "Region" });

            var query = new GridQuery
            {
                PageSize = 2,
                Filters =
                {
                    new GridFilterGroup
                    {
                        Conditions =
                        {
                            new GridFilterCondition
                            {
                                ColumnName = "Region",
                                Operator = FilterOperator.Equals,
                                Value = "APAC"
                            }
                        }
                    }
                }
            };

            var result = await source.QueryAsync(query, CancellationToken.None);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Rows.Count);
        }

        [Fact]
        public async Task VirtualSource_QueryAsync_HonorsCancellation()
        {
            var source = GridVirtualDataSource.FromList(
                new List<QueryItem> { new("Alpha", 30, "EMEA") },
                new[] { "Name", "Amount", "Region" });

            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                source.QueryAsync(new GridQuery(), cancellation.Token));
        }

        [Fact]
        public void GroupBy_StoresTheDescriptorInTheQuery()
        {
            using var grid = new BeepGridPro();
            grid.DataSource = new List<QueryItem>
            {
                new("Alpha", 30, "EMEA"),
                new("Gamma", 20, "APAC")
            };
            grid.AutoGenerateColumns();

            grid.GroupBy("Region");

            Assert.Single(grid.GroupEngine.Query.Groups);
            Assert.Single(grid.GroupEngine.Descriptors);
            Assert.True(grid.GroupEngine.IsGrouped);
        }

        [Fact]
        public async Task Virtualizer_DiscardsAStaleQueryResult()
        {
            using var grid = new BeepGridPro();
            grid.DataSource = new List<QueryItem>
            {
                new("Stale", 10, "EMEA"),
                new("Current", 20, "APAC")
            };
            grid.AutoGenerateColumns();

            var source = new ControlledQuerySource();
            grid.QuerySource = source;
            grid.EnableVirtualization = true;

            grid.RowVirtualizer.UpdateWindow(0, 300, 24);
            Assert.True(source.Pending.Count >= 2, "expected at least two query windows");

            int latest = source.Pending.Count - 1;
            source.Complete(latest, "Current");
            source.Complete(latest - 1, "Stale");

            await Task.Delay(200);

            Assert.Contains(grid.Rows, row =>
                string.Equals(Convert.ToString(row.Cells.First(c => c.ColumnName == "Name").CellValue),
                    "Current", StringComparison.Ordinal));
            Assert.DoesNotContain(grid.Rows, row =>
                string.Equals(Convert.ToString(row.Cells.First(c => c.ColumnName == "Name").CellValue),
                    "Stale", StringComparison.Ordinal));
        }

        [Fact]
        public async Task Virtualizer_ReportsAQueryFailureAndKeepsThePreviousWindow()
        {
            using var grid = new BeepGridPro();
            grid.DataSource = new List<QueryItem>
            {
                new("Kept", 10, "EMEA"),
                new("Next", 20, "APAC")
            };
            grid.AutoGenerateColumns();

            var source = new ControlledQuerySource();
            grid.QuerySource = source;
            grid.EnableVirtualization = true;
            Assert.Single(source.Pending);

            source.Complete(0, "Kept");
            await Task.Delay(200);

            GridErrorEventArgs? captured = null;
            grid.OperationError += (_, e) => captured = e;

            grid.RowVirtualizer.UpdateWindow(0, 300, 24);
            source.Fail(source.Pending.Count - 1, new InvalidOperationException("query failed"));
            await Task.Delay(200);

            Assert.NotNull(captured);
            Assert.Equal("Virtualization.Query", captured!.Section);
            Assert.Contains(grid.Rows, row =>
                string.Equals(Convert.ToString(row.Cells.First(c => c.ColumnName == "Name").CellValue),
                    "Kept", StringComparison.Ordinal));
        }

        private sealed class ControlledQuerySource : IGridQuerySource
        {
            public sealed record PendingQuery(GridQuery Query, TaskCompletionSource<GridQueryResult> Completion);

            public List<PendingQuery> Pending { get; } = new();
            public long TotalCount => 2;
            public event EventHandler? QueryResultInvalidated;

            public Task<GridQueryResult> QueryAsync(GridQuery query, CancellationToken cancellationToken)
            {
                var completion = new TaskCompletionSource<GridQueryResult>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
                Pending.Add(new PendingQuery(query, completion));
                return completion.Task;
            }

            public void Complete(int index, string name)
            {
                var pending = Pending[index];
                pending.Completion.SetResult(new GridQueryResult
                {
                    TotalCount = 2,
                    Revision = pending.Query.Revision,
                    Rows = new[]
                    {
                        new GridViewRow
                        {
                            RowId = index,
                            SourceIndex = index,
                            Values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                            {
                                ["Name"] = name,
                                ["Amount"] = 10,
                                ["Region"] = "EMEA"
                            }
                        }
                    }
                });
            }

            public void Invalidate() => QueryResultInvalidated?.Invoke(this, EventArgs.Empty);

            public void Fail(int index, Exception exception)
                => Pending[index].Completion.SetException(exception);
        }

        private static GridViewRow Row(int index, string name, int amount, string region) => new()
        {
            RowId = index,
            SourceIndex = index,
            Values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["Name"] = name,
                ["Amount"] = amount,
                ["Region"] = region
            }
        };

        private sealed record QueryItem(string Name, int Amount, string Region);
    }
}
