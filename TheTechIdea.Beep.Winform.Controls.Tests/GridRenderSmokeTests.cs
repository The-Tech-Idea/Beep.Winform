using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Formatting;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridRenderSmokeTests
    {
        [Fact]
        public void DrawToBitmap_WithChromeState_DoesNotThrow()
        {
            using var grid = new BeepGridPro { Size = new Size(640, 420) };
            grid.DataSource = new List<RenderPerson>
            {
                new() { Name = "Alice", Amount = 30 },
                new() { Name = "Bob", Amount = 10 }
            };
            grid.AutoGenerateColumns();
            grid.ShowFooter = true;
            grid.Footer.SetAggregate("Amount", AggregationType.Sum);
            grid.ConditionalFormatting.Add(new ConditionalFormatRule
            {
                ColumnName = "Amount",
                Operator = FilterOperator.GreaterThan,
                Value = "15",
                Style = new GridCellStyle { BackColor = Color.MistyRose }
            });
            grid.FocusedHeaderColumnIndex = 1;
            grid.ApplyQuickFilter("Alice");

            using var bitmap = new Bitmap(640, 420);
            grid.DrawToBitmap(bitmap, new Rectangle(0, 0, 640, 420));

            Assert.Equal(640, bitmap.Width);
            Assert.Equal(420, bitmap.Height);
        }

        [Fact]
        public void DrawToBitmap_NoResultsState_DoesNotThrow()
        {
            using var grid = new BeepGridPro { Size = new Size(640, 420) };
            grid.DataSource = new List<RenderPerson>
            {
                new() { Name = "Alice", Amount = 30 }
            };
            grid.AutoGenerateColumns();
            grid.ApplyQuickFilter("no-such-value");

            using var bitmap = new Bitmap(640, 420);
            grid.DrawToBitmap(bitmap, new Rectangle(0, 0, 640, 420));

            Assert.Equal(640, bitmap.Width);
        }

        public static IEnumerable<object[]> NavigationStyles()
            => Enum.GetValues<navigationStyle>().Select(style => new object[] { style });

        [Theory]
        [MemberData(nameof(NavigationStyles))]
        public void DrawToBitmap_AllNavigationStyles_DoNotThrow(navigationStyle style)
        {
            using var grid = new BeepGridPro { Size = new Size(900, 420) };
            grid.NavigationStyle = style;
            grid.PageSize = 1;
            grid.DataSource = new List<RenderPerson>
            {
                new() { Name = "Alice", Amount = 30 },
                new() { Name = "Bob", Amount = 10 },
                new() { Name = "Charlie", Amount = 20 }
            };
            grid.AutoGenerateColumns();
            grid.ShowFooter = true;
            grid.Footer.SetAggregate("Amount", AggregationType.Sum);
            grid.ApplyQuickFilter("a");
            grid.GoToPage(2);

            using var bitmap = new Bitmap(900, 420);
            grid.DrawToBitmap(bitmap, new Rectangle(0, 0, 900, 420));

            Assert.Equal(900, bitmap.Width);
        }

        private sealed class RenderPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
        }
    }
}
