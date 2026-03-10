using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Containers.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class PanelColorConfig
    {
        [Category("Colors")]
        public Color BackgroundColor { get; set; } = Color.Empty;

        [Category("Colors")]
        public Color ForegroundColor { get; set; } = Color.Empty;

        [Category("Colors")]
        public Color BorderColor { get; set; } = Color.Empty;

        [Category("Colors")]
        public Color TitleLineColor { get; set; } = Color.Empty;
    }
}
