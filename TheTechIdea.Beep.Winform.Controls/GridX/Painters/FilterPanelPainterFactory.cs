using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Factory for creating top filter panel painters mapped to navigation styles.
    /// </summary>
    public static class FilterPanelPainterFactory
    {
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
