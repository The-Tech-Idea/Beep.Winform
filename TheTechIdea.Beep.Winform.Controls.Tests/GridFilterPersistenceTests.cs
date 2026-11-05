using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridFilterPersistenceTests
    {
        [Fact]
        public void SaveFilterConfiguration_WritesVersionedEnvelope()
        {
            string path = Path.Combine(Path.GetTempPath(),
                "grid-filter-" + System.Guid.NewGuid().ToString("N") + ".json");
            using var grid = CreateGrid();
            try
            {
                grid.ActiveFilter = new FilterConfiguration("Saved")
                {
                    Criteria =
                    {
                        new FilterCriteria("Name", FilterOperator.Contains, "Ali")
                    }
                };

                grid.SaveFilterConfiguration(path);

                string json = File.ReadAllText(path);
                Assert.Contains("\"Version\"", json);
                Assert.Contains("\"Filter\"", json);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void LoadFilterConfiguration_RemovesUnknownColumnsAndReports()
        {
            string path = Path.Combine(Path.GetTempPath(),
                "grid-filter-" + System.Guid.NewGuid().ToString("N") + ".json");
            using var grid = CreateGrid();
            try
            {
                File.WriteAllText(path,
                    "{\"Version\":1,\"Filter\":{\"Name\":\"Saved\",\"Logic\":0,\"Criteria\":[" +
                    "{\"ColumnName\":\"Nope\",\"Operator\":2,\"Value\":\"x\"}," +
                    "{\"ColumnName\":\"Name\",\"Operator\":2,\"Value\":\"Ali\"}]}}");

                GridErrorEventArgs? reported = null;
                grid.OperationError += (_, e) => reported = e;

                grid.LoadFilterConfiguration(path);

                Assert.NotNull(grid.ActiveFilter);
                Assert.Single(grid.ActiveFilter!.Criteria);
                Assert.Equal("Name", grid.ActiveFilter.Criteria[0].ColumnName);
                Assert.Equal("Filter.LoadSaved", reported?.Section);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        private static BeepGridPro CreateGrid()
        {
            var grid = new BeepGridPro();
            grid.DataSource = new List<FilterPerson>
            {
                new() { Name = "Alice" },
                new() { Name = "Bob" }
            };
            grid.AutoGenerateColumns();
            return grid;
        }

        private sealed class FilterPerson
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
