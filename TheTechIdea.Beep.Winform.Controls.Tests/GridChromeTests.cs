using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Chrome;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.GridX.Toolbar;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridChromeTests
    {
        [Theory]
        [InlineData(400, GridChromeBreakpoint.Narrow)]
        [InlineData(600, GridChromeBreakpoint.Medium)]
        [InlineData(900, GridChromeBreakpoint.Wide)]
        [InlineData(1500, GridChromeBreakpoint.Full)]
        public void ResolveBreakpoint_ReturnsExpectedBand(int width, GridChromeBreakpoint expected)
        {
            Assert.Equal(expected, GridChromeMetrics.ResolveBreakpoint(width));
        }

        [Fact]
        public void DensityChanges_ScaleChromeMetrics()
        {
            using var grid = new BeepGridPro();
            grid.ChromeDensity = GridChromeDensity.Compact;
            var compact = grid.ChromeMetrics;

            grid.ChromeDensity = GridChromeDensity.Comfortable;
            var comfortable = grid.ChromeMetrics;

            Assert.Equal(GridChromeDensity.Compact, compact.Density);
            Assert.Equal(GridChromeDensity.Comfortable, comfortable.Density);
            Assert.True(comfortable.HeaderMinHeight >= compact.HeaderMinHeight);
            Assert.True(comfortable.MinHitTarget >= compact.MinHitTarget);
        }

        [Fact]
        public void ChromeState_ResolvesThemeColors()
        {
            using var grid = new BeepGridPro();
            var state = grid.ChromeState;

            Assert.NotEqual(System.Drawing.Color.Empty, state.Background);
            Assert.NotEqual(System.Drawing.Color.Empty, state.Foreground);
            Assert.NotEqual(System.Drawing.Color.Empty, state.FocusRing);
            Assert.True(ColorUtils.ContrastRatio(state.Foreground, state.Background) >= 4.5);
            Assert.True(ColorUtils.ContrastRatio(state.MutedForeground, state.Background) >= 3.0);
        }

        [Fact]
        public void ChromeMetrics_ScaleWithDpi()
        {
            var normal = GridChromeMetrics.Create(GridChromeDensity.Standard, 1f, GridChromeBreakpoint.Wide);
            var mid = GridChromeMetrics.Create(GridChromeDensity.Standard, 1.5f, GridChromeBreakpoint.Wide);
            var scaled = GridChromeMetrics.Create(GridChromeDensity.Standard, 2f, GridChromeBreakpoint.Wide);

            Assert.Equal(DpiScalingHelper.ScaleValue(28, 1.5f), mid.HeaderMinHeight);
            Assert.Equal(normal.HeaderMinHeight * 2, scaled.HeaderMinHeight);
            Assert.Equal(normal.NavigatorHeight * 2, scaled.NavigatorHeight);
            Assert.Equal(normal.MinHitTarget * 2, scaled.MinHitTarget);
            Assert.Equal(2f, scaled.DpiScale);
        }

        [Fact]
        public void RowMetrics_IndexesLargeRowSets()
        {
            using var grid = new BeepGridPro();
            for (int i = 0; i < 10_000; i++)
                grid.Data.Rows.Add(new BeepRowConfig { Height = 20 });

            var metrics = new GridRowMetrics(grid);

            Assert.Equal(200_000, metrics.TotalHeight);
            Assert.Equal(100_000, metrics.TopOfRow(5_000));
            Assert.Equal(5_000, metrics.FirstVisibleRow(100_000));
        }

        [Fact]
        public void PaginationPainters_ShrinkPageButtonsAtNarrowWidths()
        {
            var agGrid = new AGGridNavigationPainter();
            var wideAg = agGrid.CalculateLayout(new Rectangle(0, 0, 900, 40), 0, false);
            var narrowAg = agGrid.CalculateLayout(new Rectangle(0, 0, 420, 40), 0, false);

            Assert.Equal(5, wideAg.VisiblePageButtons);
            Assert.True(narrowAg.VisiblePageButtons < wideAg.VisiblePageButtons);
            Assert.True(narrowAg.PageNumberRects.Length == narrowAg.VisiblePageButtons);
            Assert.True(narrowAg.NextButtonRect.Right <= narrowAg.PageSizeComboRect.Left);

            var dataTables = new DataTablesNavigationPainter();
            var wideDt = dataTables.CalculateLayout(new Rectangle(0, 0, 1200, 40), 0, false);
            var narrowDt = dataTables.CalculateLayout(new Rectangle(0, 0, 760, 40), 0, false);

            Assert.Equal(5, wideDt.VisiblePageButtons);
            Assert.True(narrowDt.VisiblePageButtons < wideDt.VisiblePageButtons);
            Assert.True(narrowDt.FirstButtonRect.Right <= narrowDt.AddNewButtonRect.Left);
        }

        [Fact]
        public void ToolbarSearch_ReservesControlsWhenSearchIsActive()
        {
            var state = new BeepGridToolbarState
            {
                SearchText = "alice",
                SearchMatchCount = 12,
                DpiScale = 1f
            };
            state.CalculateLayout(new Rectangle(0, 0, 700, 40));

            Assert.False(state.SearchBoxRect.IsEmpty);
            Assert.False(state.SearchClearRect.IsEmpty);
            Assert.False(state.SearchPrevRect.IsEmpty);
            Assert.False(state.SearchNextRect.IsEmpty);
            Assert.False(state.SearchCountRect.IsEmpty);
            Assert.True(state.SearchTextRightReserve > 0);
            Assert.Equal(BeepGridToolbarState.KeySearchClear,
                state.HitTest(Center(state.SearchClearRect)));
            Assert.Equal(BeepGridToolbarState.KeySearchPrev,
                state.HitTest(Center(state.SearchPrevRect)));
            Assert.Equal(BeepGridToolbarState.KeySearchNext,
                state.HitTest(Center(state.SearchNextRect)));

            state.SearchText = string.Empty;
            state.SearchMatchCount = 0;
            state.CalculateLayout(new Rectangle(0, 0, 700, 40));

            Assert.True(state.SearchClearRect.IsEmpty);
            Assert.True(state.SearchPrevRect.IsEmpty);
            Assert.True(state.SearchNextRect.IsEmpty);
            Assert.Equal(0, state.SearchTextRightReserve);
        }

        private static Point Center(Rectangle rect)
            => new(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);

        [Fact]
        public void NavigationStatus_ReportsTotalRowsAndFilteredState()
        {
            using var grid = new BeepGridPro();
            grid.DataSource = new System.Collections.Generic.List<StatusPerson>
            {
                new() { Name = "Alice" },
                new() { Name = "Bob" }
            };
            grid.AutoGenerateColumns();

            grid.Width = 1000;
            Assert.Contains("2 records", grid.NavigatorPainter.GetNavigationStatusText());

            grid.Width = 400;
            Assert.Equal("2", grid.NavigatorPainter.GetNavigationStatusText());

            grid.ApplyQuickFilter("Alice");

            Assert.Equal("1/2", grid.NavigatorPainter.GetNavigationStatusText());
        }

        [Fact]
        public void HeaderFocus_CanBeSetAndCleared()
        {
            using var grid = new BeepGridPro();
            grid.DataSource = new System.Collections.Generic.List<StatusPerson>
            {
                new() { Name = "Alice" }
            };
            grid.AutoGenerateColumns();
            grid.Layout.Recalculate();

            grid.FocusedHeaderColumnIndex = 0;
            Assert.Equal(0, grid.FocusedHeaderColumnIndex);

            grid.FocusedHeaderColumnIndex = -1;
            Assert.Equal(-1, grid.FocusedHeaderColumnIndex);
        }

        [Fact]
        public void PageSize_ControlsPageNavigation()
        {
            using var grid = new BeepGridPro();
            var rows = new System.Collections.Generic.List<StatusPerson>();
            for (int i = 0; i < 10; i++)
                rows.Add(new StatusPerson { Name = "P" + i });
            grid.DataSource = rows;
            grid.AutoGenerateColumns();
            grid.PageSize = 3;

            Assert.Equal(4, grid.TotalPages);
            Assert.Equal(1, grid.CurrentPage);

            grid.GoToPage(2);

            Assert.Equal(2, grid.CurrentPage);
            Assert.Equal(3, grid.Selection.RowIndex);
        }

        private sealed class StatusPerson
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
