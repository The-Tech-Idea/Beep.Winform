using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class MinimalFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Minimal;
        public override string StyleName => "Minimal";
    }
}
