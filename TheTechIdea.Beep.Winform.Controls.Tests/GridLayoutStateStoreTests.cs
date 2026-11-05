using System;
using System.Collections.Generic;
using System.IO;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.LayoutState;
using TheTechIdea.Beep.Winform.Controls.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridLayoutStateStoreTests
    {
        [Fact]
        public void SaveAndLoad_RoundTripsColumnLayout()
        {
            string dir = Path.Combine(Path.GetTempPath(), "gridx-layout-" + Guid.NewGuid().ToString("N"));
            try
            {
                var store = new FileGridLayoutStateStore(dir);
                var state = new GridLayoutState
                {
                    GridKey = "app/user/grid",
                    Columns =
                    {
                        new GridColumnLayout { ColumnName = "Name", DisplayOrder = 1, Width = 180, Sticked = true, Visible = true },
                        new GridColumnLayout { ColumnName = "Amount", DisplayOrder = 2, Width = 90, Sticked = false, Visible = false }
                    }
                };

                store.Save("app/user/grid", state);
                var loaded = store.Load("app/user/grid");

                Assert.NotNull(loaded);
                Assert.Equal(2, loaded!.Columns.Count);
                Assert.Equal(180, loaded.Columns[0].Width);
                Assert.True(loaded.Columns[0].Sticked);
                Assert.False(loaded.Columns[1].Visible);
            }
            finally
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
            }
        }

        [Fact]
        public void ApplyLayoutState_IgnoresUnknownAndKeepsNewColumnDefault()
        {
            using var grid = new BeepGridPro();
            grid.DataSource = new List<LayoutPerson>
            {
                new() { Name = "Alice", Amount = 30 }
            };
            grid.AutoGenerateColumns();

            var nameColumn = grid.GetColumnByName("Name")!;
            grid.ApplyLayoutState(new GridLayoutState
            {
                Columns =
                {
                    new GridColumnLayout { ColumnName = "Name", Width = 180, Visible = true },
                    new GridColumnLayout { ColumnName = "Removed", Width = 300, Visible = false }
                }
            });

            Assert.Equal(180, nameColumn.Width);

            grid.Columns.Add(new BeepColumnConfig
            {
                ColumnName = "NewColumn",
                ColumnCaption = "New Column",
                Width = 100,
                Visible = true
            });

            grid.ApplyLayoutState(new GridLayoutState
            {
                Columns =
                {
                    new GridColumnLayout { ColumnName = "Name", Width = 180, Visible = true }
                }
            });

            Assert.Equal(100, grid.GetColumnByName("NewColumn")!.Width);
        }

        private sealed class LayoutPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
        }

        [Fact]
        public void Save_InvalidKeyOrState_ReturnsFalseWithoutThrowing()
        {
            var store = new FileGridLayoutStateStore(Path.Combine(Path.GetTempPath(), "gridx-layout-" + Guid.NewGuid().ToString("N")));

            Assert.False(store.Save(string.Empty, new GridLayoutState()));
            Assert.False(store.Save("key", null!));
        }

        [Fact]
        public void Load_CorruptFile_ReturnsNull()
        {
            string dir = Path.Combine(Path.GetTempPath(), "gridx-layout-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                File.WriteAllText(Path.Combine(dir, "app_user_grid.json"), "{ not valid json");
                var store = new FileGridLayoutStateStore(dir);

                Assert.Null(store.Load("app/user/grid"));
            }
            finally
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
            }
        }
    }
}
