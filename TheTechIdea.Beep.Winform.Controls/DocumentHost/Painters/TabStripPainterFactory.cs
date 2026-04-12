using TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public static class TabStripPainterFactory
    {
        public static ITabStripPainter CreatePainter(DocumentTabStyle style)
        {
            return style switch
            {
                DocumentTabStyle.Chrome    => new ChromeTabPainter(),
                DocumentTabStyle.VSCode    => new VSCodeTabPainter(),
                DocumentTabStyle.Underline => new UnderlineTabPainter(),
                DocumentTabStyle.Pill      => new PillTabPainter(),
                DocumentTabStyle.Flat      => new FlatTabPainter(),
                DocumentTabStyle.Rounded   => new RoundedTabPainter(),
                DocumentTabStyle.Trapezoid => new TrapezoidTabPainter(),
                DocumentTabStyle.Office    => new OfficeTabPainter(),
                DocumentTabStyle.Fluent    => new FluentTabPainter(),
                _                          => new ChromeTabPainter()
            };
        }
    }
}
