using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Containers.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BeepPanelLayoutContext
    {
        public Rectangle OuterBounds { get; set; }
        public Rectangle BorderBounds { get; set; }
        public Rectangle TitleBounds { get; set; }
        public Rectangle TitleClusterBounds { get; set; }
        public Rectangle IconBounds { get; set; }
        public Rectangle TitleGapBounds { get; set; }
        public Rectangle TitleLineBounds { get; set; }
        public int TitleLineStartX { get; set; }
        public Rectangle HeaderBounds { get; set; }
        public Rectangle ContentBounds { get; set; }
        public Size TitleSize { get; set; }
        public bool HasTitle { get; set; }
        public bool HasIcon { get; set; }
        public bool HasTitleLine { get; set; }
    }
}
