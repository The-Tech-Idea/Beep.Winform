using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridHeaderLayoutTests
    {
        [Theory]
        [InlineData(navigationStyle.Standard)]
        [InlineData(navigationStyle.Material)]
        [InlineData(navigationStyle.AGGrid)]
        [InlineData(navigationStyle.Compact)]
        public void HeaderPainters_ProduceUsableCellGeometry(navigationStyle style)
        {
            using var grid = new BeepGridPro { Size = new Size(800, 400) };
            grid.DataSource = new List<HeaderPerson> { new() { Name = "Alice" } };
            grid.AutoGenerateColumns();

            var painter = HeaderPainterFactory.CreateHeaderPainter(style);
            var column = new BeepColumnConfig
            {
                ColumnName = "Name",
                ColumnCaption = "Name",
                AllowSort = true,
                ShowFilterIcon = true,
                Width = 160
            };
            var cell = new Rectangle(0, 0, 160, 32);

            var layout = painter.CalculateHeaderCellLayout(cell, column, grid, 1f);

            Assert.False(layout.TextRect.IsEmpty);
            Assert.True(layout.TextRect.Width > 0);
            Assert.True(layout.TextRect.Height > 0);
            Assert.True(layout.SortIndicatorRect.Width >= 0);
            Assert.True(layout.MenuButtonRect.Width >= 0);
        }

        private sealed class HeaderPerson
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
