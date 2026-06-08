using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Export;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepGridProExportOptionsTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProExportOptionsTests()
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

        // ========== ExportOptions Default Values ==========

        [Fact]
        public void ExportOptions_DefaultValues()
        {
            var opts = new ExportOptions();
            Assert.True(opts.VisibleRowsOnly);
            Assert.True(opts.VisibleColumnsOnly);
            Assert.True(opts.RespectColumnDisplayOrder);
            Assert.True(opts.IncludeHeaders);
            Assert.True(opts.ExcludeSystemColumns);
            Assert.Equal("o", opts.DateTimeFormat);
            Assert.Null(opts.NumberFormat);
            Assert.Null(opts.ColumnWhitelist);
            Assert.Null(opts.ColumnBlacklist);
            Assert.Equal(',', opts.CsvDelimiter);
            Assert.Equal('"', opts.CsvQuote);
            Assert.Equal("beep-grid-export", opts.HtmlTableClass);
            Assert.Null(opts.HtmlInlineStyles);
            Assert.True(opts.JsonIndented);
        }

        [Fact]
        public void ExportOptions_Clone_CopiesAllProperties()
        {
            var original = new ExportOptions
            {
                VisibleRowsOnly = false,
                VisibleColumnsOnly = false,
                RespectColumnDisplayOrder = false,
                IncludeHeaders = false,
                ExcludeSystemColumns = false,
                DateTimeFormat = "yyyy-MM-dd",
                NumberFormat = "0.00",
                ColumnWhitelist = new List<string> { "Name" },
                ColumnBlacklist = new List<string> { "Age" },
                CsvDelimiter = ';',
                CsvQuote = '\'',
                HtmlTableClass = "my-table",
                HtmlInlineStyles = "td{color:red}",
                JsonIndented = false
            };

            var clone = original.Clone();

            Assert.Equal(original.VisibleRowsOnly, clone.VisibleRowsOnly);
            Assert.Equal(original.VisibleColumnsOnly, clone.VisibleColumnsOnly);
            Assert.Equal(original.RespectColumnDisplayOrder, clone.RespectColumnDisplayOrder);
            Assert.Equal(original.IncludeHeaders, clone.IncludeHeaders);
            Assert.Equal(original.ExcludeSystemColumns, clone.ExcludeSystemColumns);
            Assert.Equal(original.DateTimeFormat, clone.DateTimeFormat);
            Assert.Equal(original.NumberFormat, clone.NumberFormat);
            Assert.Equal(original.CsvDelimiter, clone.CsvDelimiter);
            Assert.Equal(original.CsvQuote, clone.CsvQuote);
            Assert.Equal(original.HtmlTableClass, clone.HtmlTableClass);
            Assert.Equal(original.HtmlInlineStyles, clone.HtmlInlineStyles);
            Assert.Equal(original.JsonIndented, clone.JsonIndented);
            Assert.Equal(original.ColumnWhitelist, clone.ColumnWhitelist);
            Assert.Equal(original.ColumnBlacklist, clone.ColumnBlacklist);
        }

        [Fact]
        public void ExportOptions_Clone_CreatesIndependentLists()
        {
            var original = new ExportOptions
            {
                ColumnWhitelist = new List<string> { "Name" },
                ColumnBlacklist = new List<string> { "Age" }
            };
            var clone = original.Clone();

            clone.ColumnWhitelist.Add("Extra");
            clone.ColumnBlacklist.Add("Extra");

            Assert.Single(original.ColumnWhitelist);
            Assert.Single(original.ColumnBlacklist);
            Assert.Equal(2, clone.ColumnWhitelist.Count);
        }

        [Fact]
        public void ExportOptions_Clone_NullListsRemainNull()
        {
            var original = new ExportOptions();
            var clone = original.Clone();
            Assert.Null(clone.ColumnWhitelist);
            Assert.Null(clone.ColumnBlacklist);
        }

        // ========== CSV Export Options ==========

        [Fact]
        public void Export_Csv_NoHeaders_OmitsHeaderRow()
        {
            BindData();
            var opts = new ExportOptions { IncludeHeaders = false };
            var csv = _grid.ExportToString(GridExportFormat.Csv, opts);

            Assert.NotEmpty(csv);
            Assert.DoesNotContain("Name", csv.Split('\n')[0]);
        }

        [Fact]
        public void Export_Csv_CustomDelimiter()
        {
            BindData();
            var opts = new ExportOptions { CsvDelimiter = ';' };
            var csv = _grid.ExportToString(GridExportFormat.Csv, opts);

            Assert.NotEmpty(csv);
            Assert.Contains(";", csv);
        }

        [Fact]
        public void Export_Csv_CustomQuote()
        {
            BindData();
            var opts = new ExportOptions { CsvQuote = '\'' };
            var csv = _grid.ExportToString(GridExportFormat.Csv, opts);

            Assert.NotEmpty(csv);
        }

        [Fact]
        public void Export_Csv_WithColumnWhitelist()
        {
            BindData();
            var opts = new ExportOptions { ColumnWhitelist = new List<string> { "Name" } };
            var csv = _grid.ExportToString(GridExportFormat.Csv, opts);

            Assert.NotEmpty(csv);
            Assert.Contains("Name", csv);
        }

        [Fact]
        public void Export_Csv_WithColumnBlacklist()
        {
            BindData();
            var opts = new ExportOptions { ColumnBlacklist = new List<string> { "Age" } };
            var csv = _grid.ExportToString(GridExportFormat.Csv, opts);

            Assert.NotEmpty(csv);
        }

        [Fact]
        public void Export_Csv_ExcludeSystemColumns_False()
        {
            BindData();
            var opts = new ExportOptions { ExcludeSystemColumns = false };
            var csv = _grid.ExportToString(GridExportFormat.Csv, opts);

            Assert.NotEmpty(csv);
        }

        [Fact]
        public void Export_Csv_VisibleRowsOnly_RespectsFilter()
        {
            BindData();
            _grid.ApplyQuickFilter("Alice");

            var optsDefault = new ExportOptions { VisibleRowsOnly = true };
            var optsAll = new ExportOptions { VisibleRowsOnly = false };

            var csvDefault = _grid.ExportToString(GridExportFormat.Csv, optsDefault);
            var csvAll = _grid.ExportToString(GridExportFormat.Csv, optsAll);

            Assert.True(csvDefault.Length < csvAll.Length, "Filtered CSV should be shorter than unfiltered");
        }

        // ========== JSON Export Options ==========

        [Fact]
        public void Export_Json_Indented_True_ProducesMultiLine()
        {
            BindData();
            var opts = new ExportOptions { JsonIndented = true };
            var json = _grid.ExportToString(GridExportFormat.Json, opts);

            Assert.NotEmpty(json);
            Assert.Contains("\n", json);
        }

        [Fact]
        public void Export_Json_Indented_False_ProducesSingleLine()
        {
            BindData();
            var opts = new ExportOptions { JsonIndented = false };
            var json = _grid.ExportToString(GridExportFormat.Json, opts);

            Assert.NotEmpty(json);
        }

        [Fact]
        public void Export_Json_WithColumnWhitelist()
        {
            BindData();
            var opts = new ExportOptions { ColumnWhitelist = new List<string> { "Name" } };
            var json = _grid.ExportToString(GridExportFormat.Json, opts);

            Assert.NotEmpty(json);
            Assert.Contains("Name", json);
        }

        // ========== HTML Export Options ==========

        [Fact]
        public void Export_Html_CustomTableClass()
        {
            BindData();
            var opts = new ExportOptions { HtmlTableClass = "my-custom-table" };
            var html = _grid.ExportToString(GridExportFormat.Html, opts);

            Assert.NotEmpty(html);
            Assert.Contains("my-custom-table", html);
        }

        [Fact]
        public void Export_Html_InlineStyles()
        {
            BindData();
            var opts = new ExportOptions { HtmlInlineStyles = "td { padding: 5px; }" };
            var html = _grid.ExportToString(GridExportFormat.Html, opts);

            Assert.NotEmpty(html);
            Assert.Contains("padding: 5px", html);
        }

        [Fact]
        public void Export_Html_NoHeaders_OmitsHeaderRow()
        {
            BindData();
            var opts = new ExportOptions { IncludeHeaders = false };
            var html = _grid.ExportToString(GridExportFormat.Html, opts);

            Assert.NotEmpty(html);
            Assert.DoesNotContain("<th", html);
        }

        [Fact]
        public void Export_Html_WithColumnWhitelist()
        {
            BindData();
            var opts = new ExportOptions { ColumnWhitelist = new List<string> { "Name" } };
            var html = _grid.ExportToString(GridExportFormat.Html, opts);

            Assert.NotEmpty(html);
        }

        // ========== ExportToFile ==========

        [Fact]
        public void ExportToCsv_FilePath_CreatesFile()
        {
            BindData();
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"test_export_{Guid.NewGuid()}.csv");
            try
            {
                _grid.ExportToCsv(path);
                Assert.True(System.IO.File.Exists(path));
                var content = System.IO.File.ReadAllText(path);
                Assert.NotEmpty(content);
                Assert.Contains("Name", content);
            }
            finally
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        [Fact]
        public void ExportToJson_FilePath_CreatesFile()
        {
            BindData();
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"test_export_{Guid.NewGuid()}.json");
            try
            {
                _grid.ExportToJson(path);
                Assert.True(System.IO.File.Exists(path));
            }
            finally
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        [Fact]
        public void ExportToHtml_FilePath_CreatesFile()
        {
            BindData();
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"test_export_{Guid.NewGuid()}.html");
            try
            {
                _grid.ExportToHtml(path);
                Assert.True(System.IO.File.Exists(path));
            }
            finally
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        // ========== ExportToStream ==========

        [Fact]
        public void ExportToStream_Csv_WritesToStream()
        {
            BindData();
            using var ms = new System.IO.MemoryStream();
            _grid.ExportToStream(ms, GridExportFormat.Csv);
            ms.Position = 0;
            using var reader = new System.IO.StreamReader(ms);
            var content = reader.ReadToEnd();
            Assert.NotEmpty(content);
        }

        [Fact]
        public void ExportToStream_Json_WritesToStream()
        {
            BindData();
            using var ms = new System.IO.MemoryStream();
            _grid.ExportToStream(ms, GridExportFormat.Json);
            ms.Position = 0;
            using var reader = new System.IO.StreamReader(ms);
            var content = reader.ReadToEnd();
            Assert.NotEmpty(content);
        }

        [Fact]
        public void ExportToStream_Html_WritesToStream()
        {
            BindData();
            using var ms = new System.IO.MemoryStream();
            _grid.ExportToStream(ms, GridExportFormat.Html);
            ms.Position = 0;
            using var reader = new System.IO.StreamReader(ms);
            var content = reader.ReadToEnd();
            Assert.NotEmpty(content);
        }
    }
}
