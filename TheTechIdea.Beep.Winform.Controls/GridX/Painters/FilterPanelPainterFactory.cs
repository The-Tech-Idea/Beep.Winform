using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Factory for creating top filter panel painters mapped to navigation styles.
    /// </summary>
    public static class FilterPanelPainterFactory
    {
        public static navigationStyle MapGridStyleToNavigationStyle(BeepGridStyle gridStyle)
        {
            return gridStyle switch
            {
                BeepGridStyle.Default => navigationStyle.AGGrid,
                BeepGridStyle.Clean => navigationStyle.AGGrid,
                BeepGridStyle.Bootstrap => navigationStyle.DataTables,
                BeepGridStyle.Material => navigationStyle.AGGrid,
                BeepGridStyle.Flat => navigationStyle.AGGrid,
                BeepGridStyle.Compact => navigationStyle.AGGrid,
                BeepGridStyle.Corporate => navigationStyle.AGGrid,
                BeepGridStyle.Minimal => navigationStyle.AGGrid,
                BeepGridStyle.Card => navigationStyle.AGGrid,
                BeepGridStyle.Borderless => navigationStyle.AGGrid,
                BeepGridStyle.Modern => navigationStyle.AGGrid,
                _ => navigationStyle.AGGrid
            };
        }

        public static IGridFilterPanelPainter CreatePainterForGridStyle(BeepGridStyle gridStyle)
        {
            var mappedStyle = MapGridStyleToNavigationStyle(gridStyle);
            return CreatePainter(mappedStyle);
        }

        public static IGridFilterPanelPainter CreatePainter(navigationStyle style)
        {
            return style switch
            {
                navigationStyle.Material => new MaterialFilterPanelPainter(),
                navigationStyle.Compact => new CompactFilterPanelPainter(),
                navigationStyle.Minimal => new MinimalFilterPanelPainter(),
                navigationStyle.Bootstrap => new BootstrapFilterPanelPainter(),
                navigationStyle.Fluent => new FluentFilterPanelPainter(),
                navigationStyle.AntDesign => new AntDesignFilterPanelPainter(),
                navigationStyle.Telerik => new TelerikFilterPanelPainter(),
                navigationStyle.AGGrid => new AGGridFilterPanelPainter(),
                navigationStyle.DataTables => new DataTablesFilterPanelPainter(),
                navigationStyle.Card => new CardFilterPanelPainter(),
                navigationStyle.Tailwind => new TailwindFilterPanelPainter(),
                navigationStyle.Standard => new StandardFilterPanelPainter(),
                navigationStyle.None => new StandardFilterPanelPainter(),
                _ => new StandardFilterPanelPainter()
            };
        }
    }
}
