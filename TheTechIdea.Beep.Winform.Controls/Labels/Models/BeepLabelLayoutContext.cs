using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BeepLabelLayoutContext
    {
        public Rectangle Bounds { get; set; }
        public Rectangle ContentBounds { get; set; }
        public Rectangle HeaderBounds { get; set; }
        public Rectangle SubHeaderBounds { get; set; }
        public Rectangle TextBounds { get; set; }
        public Rectangle ImageBounds { get; set; }
        public Size HeaderSize { get; set; }
        public Size SubHeaderSize { get; set; }
        public Size TextSize { get; set; }
        public Size ImageSize { get; set; }
        public bool HasSubHeader { get; set; }
    }
}
