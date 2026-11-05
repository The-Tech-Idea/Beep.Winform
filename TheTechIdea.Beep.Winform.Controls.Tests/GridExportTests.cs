using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Export;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridExportTests
    {
        [Fact]
        public async Task ExportToCsvAsync_WritesTheVisibleSnapshot()
        {
            string path = Path.Combine(Path.GetTempPath(), "grid-export-" + Guid.NewGuid().ToString("N") + ".csv");
            using var grid = CreateGrid();
            try
            {
                await grid.ExportToCsvAsync(path);

                Assert.True(File.Exists(path));
                string csv = File.ReadAllText(path);
                Assert.Contains("Alice", csv);
                Assert.Contains("Bob", csv);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public async Task ExportToCsvAsync_CancellationLeavesNoFile()
        {
            string path = Path.Combine(Path.GetTempPath(), "grid-export-" + Guid.NewGuid().ToString("N") + ".csv");
            using var grid = CreateGrid();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            Assert.False(await grid.ExportToCsvAsync(path, cancellationToken: cancellation.Token));

            Assert.False(File.Exists(path));
        }

        [Fact]
        public void Capture_ReflectsVisibleRows()
        {
            using var grid = CreateGrid();

            var snapshot = GridExportSnapshot.Capture(grid);

            Assert.True(snapshot.Columns.Count >= 2);
            Assert.Equal(2, snapshot.Rows.Count);
            Assert.NotEqual(default, snapshot.CapturedAtUtc);
            Assert.Empty(
                snapshot.Rows.SelectMany(r => r.Values.Values).OfType<System.Windows.Forms.Control>());
        }

        [Fact]
        public void Capture_AfterQuickFilter_ExportsOnlyMatchingRows()
        {
            using var grid = CreateGrid();
            grid.ApplyQuickFilter("Alice");

            var snapshot = GridExportSnapshot.Capture(grid);

            Assert.Single(snapshot.Rows);
        }

        [Fact]
        public void FilteredRowIndices_StayConsistentAfterSort()
        {
            using var grid = CreateGrid();
            grid.ApplyQuickFilter("a");
            int before = grid.FilteredRowIndices?.Count ?? 0;

            var nameColumn = grid.GetColumnByName("Name")!;
            int columnIndex = grid.Columns.IndexOf(nameColumn);
            grid.ToggleColumnSort(columnIndex);

            Assert.Equal(before, grid.FilteredRowIndices?.Count ?? 0);
        }

        [Fact]
        public async Task ExportToCsvAsync_ReportsProgress()
        {
            string path = Path.Combine(Path.GetTempPath(), "grid-export-" + Guid.NewGuid().ToString("N") + ".csv");
            using var grid = CreateGrid();
            var reports = new List<GridExportProgress>();
            try
            {
                await grid.ExportToCsvAsync(path, progress: new SynchronousProgress<GridExportProgress>(reports.Add));

                Assert.NotEmpty(reports);
                Assert.Equal(2, reports[^1].RowsProcessed);
                for (int i = 1; i < reports.Count; i++)
                    Assert.True(reports[i].RowsProcessed >= reports[i - 1].RowsProcessed);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        private sealed class SynchronousProgress<T> : IProgress<T>
        {
            private readonly Action<T> _report;
            public SynchronousProgress(Action<T> report) => _report = report;
            public void Report(T value) => _report(value);
        }

        private static BeepGridPro CreateGrid()
        {
            var grid = new BeepGridPro();
            grid.DataSource = new List<ExportPerson>
            {
                new() { Name = "Alice", Amount = 30 },
                new() { Name = "Bob", Amount = 10 }
            };
            grid.AutoGenerateColumns();
            return grid;
        }

        private sealed class ExportPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
        }
    }
}
