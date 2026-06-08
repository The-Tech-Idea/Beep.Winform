using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepGridProFilterConfigTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProFilterConfigTests()
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
                new() { Name = "Charlie", Age = 35 },
                new() { Name = "David", Age = 40 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();
        }

        // ========== AddFilterCriterion ==========

        [Fact]
        public void AddFilterCriterion_CreatesActiveFilter()
        {
            BindData();
            var criterion = new FilterCriteria
            {
                ColumnName = "Age",
                Operator = FilterOperator.GreaterThan,
                Value = "30"
            };

            _grid.AddFilterCriterion(criterion);

            Assert.NotNull(_grid.ActiveFilter);
            Assert.True(_grid.IsFiltered);
            // Charlie(35) and David(40) match Age > 30
            Assert.Equal(2, _grid.FilteredRowCount);
        }

        [Fact]
        public void AddFilterCriterion_MultipleCriteria_AllAdded()
        {
            BindData();
            _grid.AddFilterCriterion(new FilterCriteria
            {
                ColumnName = "Age",
                Operator = FilterOperator.GreaterThan,
                Value = "20"
            });
            _grid.AddFilterCriterion(new FilterCriteria
            {
                ColumnName = "Name",
                Operator = FilterOperator.StartsWith,
                Value = "A"
            });

            Assert.NotNull(_grid.ActiveFilter);
            Assert.Equal(2, _grid.ActiveFilter.Criteria.Count);
        }

        // ========== RemoveFilterCriterion ==========

        [Fact]
        public void RemoveFilterCriterion_Existing_ReturnsTrue()
        {
            BindData();
            _grid.AddFilterCriterion(new FilterCriteria
            {
                ColumnName = "Name",
                Operator = FilterOperator.Contains,
                Value = "a"
            });
            Assert.NotNull(_grid.ActiveFilter);

            bool removed = _grid.RemoveFilterCriterion("Name");

            Assert.True(removed);
        }

        [Fact]
        public void RemoveFilterCriterion_NoActiveFilter_ReturnsFalse()
        {
            BindData();

            bool removed = _grid.RemoveFilterCriterion("Name");

            Assert.False(removed);
        }

        [Fact]
        public void RemoveFilterCriterion_NonExistent_ReturnsFalse()
        {
            BindData();
            _grid.AddFilterCriterion(new FilterCriteria
            {
                ColumnName = "Name",
                Operator = FilterOperator.Contains,
                Value = "a"
            });

            bool removed = _grid.RemoveFilterCriterion("NonExistent");

            Assert.False(removed);
        }

        [Fact]
        public void RemoveFilterCriterion_LastCriterion_ClearsFilter()
        {
            BindData();
            _grid.AddFilterCriterion(new FilterCriteria
            {
                ColumnName = "Name",
                Operator = FilterOperator.Contains,
                Value = "a"
            });
            Assert.True(_grid.IsFiltered);

            _grid.RemoveFilterCriterion("Name");

            Assert.False(_grid.IsFiltered);
            Assert.Null(_grid.ActiveFilter);
        }

        // ========== ApplyActiveFilter ==========

        [Fact]
        public void ApplyActiveFilter_NoConfiguration_ClearsFilter()
        {
            BindData();
            _grid.ApplyActiveFilter();
            Assert.False(_grid.IsFiltered);
        }

        [Fact]
        public void ApplyActiveFilter_WithConfiguration_AppliesFilter()
        {
            BindData();
            var config = new FilterConfiguration("Test");
            config.Criteria.Add(new FilterCriteria
            {
                ColumnName = "Name",
                Operator = FilterOperator.Contains,
                Value = "a"
            });
            _grid.ActiveFilter = config;
            _grid.ApplyActiveFilter();

            Assert.True(_grid.IsFiltered);
            // Alice, Charlie, David contain 'a' (case-insensitive Contains)
            Assert.Equal(3, _grid.FilteredRowCount);
        }

        // ========== SaveFilterConfiguration ==========

        [Fact]
        public void SaveFilterConfiguration_NoActiveFilter_Throws()
        {
            BindData();
            var path = Path.Combine(Path.GetTempPath(), $"filter_{Guid.NewGuid()}.json");

            Assert.Throws<InvalidOperationException>(() => _grid.SaveFilterConfiguration(path));
        }

        [Fact]
        public void SaveFilterConfiguration_WithFilter_CreatesFile()
        {
            BindData();
            _grid.ApplyQuickFilter("Alice");

            var path = Path.Combine(Path.GetTempPath(), $"filter_{Guid.NewGuid()}.json");
            try
            {
                _grid.SaveFilterConfiguration(path);
                Assert.True(File.Exists(path));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // ========== LoadFilterConfiguration ==========

        [Fact]
        public void LoadFilterConfiguration_MissingFile_Throws()
        {
            BindData();
            var path = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.json");

            Assert.Throws<FileNotFoundException>(() => _grid.LoadFilterConfiguration(path));
        }

        [Fact]
        public void LoadFilterConfiguration_ValidFile_AppliesFilter()
        {
            BindData();
            _grid.ApplyQuickFilter("Alice");
            var path = Path.Combine(Path.GetTempPath(), $"filter_{Guid.NewGuid()}.json");
            try
            {
                _grid.SaveFilterConfiguration(path);
                _grid.ClearFilter();
                _grid.LoadFilterConfiguration(path);

                Assert.True(_grid.IsFiltered);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // ========== GetSavedFilterConfigurations ==========

        [Fact]
        public void GetSavedFilterConfigurations_NonExistentDirectory_ReturnsEmpty()
        {
            var dir = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}");
            var configs = _grid.GetSavedFilterConfigurations(dir);
            Assert.Empty(configs);
        }

        [Fact]
        public void GetSavedFilterConfigurations_EmptyDirectory_ReturnsEmpty()
        {
            var dir = Path.Combine(Path.GetTempPath(), $"empty_{Guid.NewGuid()}");
            Directory.CreateDirectory(dir);
            try
            {
                var configs = _grid.GetSavedFilterConfigurations(dir);
                Assert.Empty(configs);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Fact]
        public void GetSavedFilterConfigurations_WithFiles_ReturnsConfigs()
        {
            BindData();
            _grid.ApplyQuickFilter("Alice");
            _grid.AddFilterCriterion(new FilterCriteria
            {
                ColumnName = "Name",
                Operator = FilterOperator.Contains,
                Value = "b"
            });

            var dir = Path.Combine(Path.GetTempPath(), $"configs_{Guid.NewGuid()}");
            Directory.CreateDirectory(dir);
            try
            {
                _grid.SaveFilterConfiguration(Path.Combine(dir, "filter1.json"));
                _grid.SaveFilterConfiguration(Path.Combine(dir, "filter2.json"));

                var configs = _grid.GetSavedFilterConfigurations(dir);
                Assert.Equal(2, configs.Count);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Fact]
        public void GetSavedFilterConfigurations_InvalidJsonFiles_AreSkipped()
        {
            var dir = Path.Combine(Path.GetTempPath(), $"mixed_{Guid.NewGuid()}");
            Directory.CreateDirectory(dir);
            try
            {
                File.WriteAllText(Path.Combine(dir, "invalid.json"), "not valid json {{{");
                File.WriteAllText(Path.Combine(dir, "empty.json"), "");

                var configs = _grid.GetSavedFilterConfigurations(dir);
                Assert.Empty(configs);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
