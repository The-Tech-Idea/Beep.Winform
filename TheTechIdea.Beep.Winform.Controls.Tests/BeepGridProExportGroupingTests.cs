using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Export;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepGridProExportGroupingTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProExportGroupingTests()
        {
            _grid = new BeepGridPro();
        }

        public void Dispose()
        {
            _grid.Dispose();
        }

        private void BindSampleData()
        {
            var data = new List<GridTestPerson>
            {
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 },
                new() { Name = "Charlie", Age = 35 },
                new() { Name = "David", Age = 25 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();
        }

        // ========== Export Tests ==========

        [Fact]
        public void ExportEngine_IsRegistered_CsvFormat()
        {
            var engine = _grid.ExportEngine;
            Assert.True(engine.IsRegistered(GridExportFormat.Csv));
            Assert.True(engine.IsAvailable(GridExportFormat.Csv));
        }

        [Fact]
        public void ExportEngine_IsRegistered_JsonFormat()
        {
            var engine = _grid.ExportEngine;
            Assert.True(engine.IsRegistered(GridExportFormat.Json));
            Assert.True(engine.IsAvailable(GridExportFormat.Json));
        }

        [Fact]
        public void ExportEngine_IsRegistered_HtmlFormat()
        {
            var engine = _grid.ExportEngine;
            Assert.True(engine.IsRegistered(GridExportFormat.Html));
            Assert.True(engine.IsAvailable(GridExportFormat.Html));
        }

        [Fact]
        public void ExportToString_Csv_ContainsHeadersAndData()
        {
            BindSampleData();
            var csv = _grid.ExportToString(GridExportFormat.Csv);

            Assert.NotEmpty(csv);
            Assert.Contains("Name", csv);
            Assert.Contains("Alice", csv);
        }

        [Fact]
        public void ExportToString_Json_ContainsHeadersAndData()
        {
            BindSampleData();
            var json = _grid.ExportToString(GridExportFormat.Json);

            Assert.NotEmpty(json);
            Assert.Contains("Name", json);
            Assert.Contains("Alice", json);
        }

        [Fact]
        public void ExportToString_Html_ContainsTableTags()
        {
            BindSampleData();
            var html = _grid.ExportToString(GridExportFormat.Html);

            Assert.NotEmpty(html);
            Assert.Contains("<table", html, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExportOptions_GetFileExtension_ReturnsCorrectExtension()
        {
            var engine = _grid.ExportEngine;
            Assert.Equal(".csv", engine.GetFileExtension(GridExportFormat.Csv));
            Assert.Equal(".json", engine.GetFileExtension(GridExportFormat.Json));
            Assert.Equal(".html", engine.GetFileExtension(GridExportFormat.Html));
        }

        [Fact]
        public void ExportEngine_StubFormats_RegisteredButNotAvailable()
        {
            var engine = _grid.ExportEngine;
            Assert.True(engine.IsRegistered(GridExportFormat.Excel));
            Assert.False(engine.IsAvailable(GridExportFormat.Excel));
            Assert.True(engine.IsRegistered(GridExportFormat.Pdf));
            Assert.False(engine.IsAvailable(GridExportFormat.Pdf));
        }

        [Fact]
        public void ExportToString_ExcelFormat_ThrowsInvalidOperation()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _grid.ExportToString(GridExportFormat.Excel));
        }

        // ========== Grouping Tests ==========

        private void BindGroupData()
        {
            var data = new List<GridTestPerson>
            {
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 },
                new() { Name = "Alice", Age = 35 },
                new() { Name = "David", Age = 25 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();
        }

        [Fact]
        public void GroupBy_SingleColumn_CreatesGroups()
        {
            BindGroupData();

            _grid.GroupBy("Name");

            Assert.True(_grid.GroupEngine.IsGrouped);
            Assert.True(_grid.GroupEngine.Groups.Count > 0);
        }

        [Fact]
        public void GroupBy_MultipleColumns_CreatesNestedGroups()
        {
            BindGroupData();

            _grid.GroupBy(new[] { "Name", "Age" });

            Assert.True(_grid.GroupEngine.IsGrouped);
            Assert.True(_grid.GroupEngine.Groups.Count > 0);
        }

        [Fact]
        public void Ungroup_ClearsAllGroups()
        {
            BindGroupData();
            _grid.GroupBy("Name");
            Assert.True(_grid.GroupEngine.IsGrouped);

            _grid.Ungroup();

            Assert.False(_grid.GroupEngine.IsGrouped);
            Assert.Empty(_grid.GroupEngine.Groups);
        }

        [Fact]
        public void ExpandAllGroups_DoesNotThrow()
        {
            BindGroupData();
            _grid.GroupBy("Name");

            _grid.ExpandAllGroups();
        }

        [Fact]
        public void CollapseAllGroups_DoesNotThrow()
        {
            BindGroupData();
            _grid.GroupBy("Name");

            _grid.CollapseAllGroups();
        }

        [Fact]
        public void GroupEngine_IsGrouped_FalseByDefault()
        {
            BindGroupData();

            Assert.False(_grid.GroupEngine.IsGrouped);
        }

        [Fact]
        public void GroupBy_EmptyGrid_DoesNotThrow()
        {
            var data = new List<GridTestPerson>();
            _grid.DataSource = data;
            _grid.RefreshData();

            _grid.GroupBy("Name");
        }
    }
}
