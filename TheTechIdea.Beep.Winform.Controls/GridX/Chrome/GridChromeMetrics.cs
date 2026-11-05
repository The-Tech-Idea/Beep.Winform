using System;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Chrome
{
    public enum GridChromeDensity
    {
        Compact,
        Standard,
        Comfortable,
        Touch
    }

    public enum GridChromeBreakpoint
    {
        Narrow,
        Medium,
        Wide,
        Full
    }

    /// <summary>
    /// Shared sizing contract for headers, toolbar/search, filter chips, and
    /// the navigator. Painters may choose presentation, but not their own
    /// spacing, hit-target, or density rules.
    /// </summary>
    public sealed class GridChromeMetrics
    {
        public GridChromeDensity Density { get; init; }
        public GridChromeBreakpoint Breakpoint { get; init; }
        public float DpiScale { get; init; }

        public int HeaderMinHeight { get; init; }
        public int HeaderPadding { get; init; }
        public int HeaderBorderSpacing { get; init; }
        public int NavigatorHeight { get; init; }
        public int ToolbarHeight { get; init; }
        public int ChipHeight { get; init; }
        public int Gap { get; init; }
        public int Padding { get; init; }
        public int IconSize { get; init; }
        public int MinHitTarget { get; init; }
        public int ResizeHandleWidth { get; init; }
        public int FocusRingWidth { get; init; }
        public int FocusRingOffset { get; init; }

        public static GridChromeMetrics Resolve(BeepGridPro grid)
        {
            if (grid == null) return Create(GridChromeDensity.Standard, 1f, GridChromeBreakpoint.Wide);

            float scale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return Create(grid.ChromeDensity, scale, ResolveBreakpoint(grid.Width));
        }

        public static GridChromeBreakpoint ResolveBreakpoint(int width)
        {
            if (width < 480) return GridChromeBreakpoint.Narrow;
            if (width < 768) return GridChromeBreakpoint.Medium;
            if (width < 1200) return GridChromeBreakpoint.Wide;
            return GridChromeBreakpoint.Full;
        }

        internal static GridChromeMetrics Create(GridChromeDensity density, float scale, GridChromeBreakpoint breakpoint)
        {
            int headerMin, headerPadding, headerSpacing, nav, toolbar, chip, gap, padding, icon, hit, resize, focus, focusOffset;
            switch (density)
            {
                case GridChromeDensity.Compact:
                    headerMin = 24; headerPadding = 3; headerSpacing = 2; nav = 28; toolbar = 32;
                    chip = 20; gap = 4; padding = 4; icon = 14; hit = 24; resize = 6; focus = 1; focusOffset = 1;
                    break;
                case GridChromeDensity.Comfortable:
                    headerMin = 32; headerPadding = 5; headerSpacing = 4; nav = 44; toolbar = 48;
                    chip = 28; gap = 8; padding = 8; icon = 18; hit = 32; resize = 6; focus = 2; focusOffset = 2;
                    break;
                case GridChromeDensity.Touch:
                    headerMin = 44; headerPadding = 8; headerSpacing = 6; nav = 52; toolbar = 56;
                    chip = 32; gap = 10; padding = 10; icon = 20; hit = 44; resize = 8; focus = 3; focusOffset = 2;
                    break;
                default:
                    headerMin = 28; headerPadding = 4; headerSpacing = 3; nav = 36; toolbar = 40;
                    chip = 24; gap = 6; padding = 6; icon = 16; hit = 28; resize = 6; focus = 2; focusOffset = 1;
                    break;
            }

            return new GridChromeMetrics
            {
                Density = density,
                Breakpoint = breakpoint,
                DpiScale = scale,
                HeaderMinHeight = Scale(headerMin, scale),
                HeaderPadding = Scale(headerPadding, scale),
                HeaderBorderSpacing = Scale(headerSpacing, scale),
                NavigatorHeight = Scale(nav, scale),
                ToolbarHeight = Scale(toolbar, scale),
                ChipHeight = Scale(chip, scale),
                Gap = Scale(gap, scale),
                Padding = Scale(padding, scale),
                IconSize = Scale(icon, scale),
                MinHitTarget = Scale(hit, scale),
                ResizeHandleWidth = Scale(resize, scale),
                FocusRingWidth = Scale(focus, scale),
                FocusRingOffset = Scale(focusOffset, scale)
            };
        }

        private static int Scale(int value, float scale)
            => DpiScalingHelper.ScaleValue(value, scale);
    }
}
