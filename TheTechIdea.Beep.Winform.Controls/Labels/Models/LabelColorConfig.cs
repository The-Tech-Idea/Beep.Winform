using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class LabelColorConfig
    {
        [Category("Colors")]
        public Color BackColor { get; set; } = Color.Empty;

        [Category("Colors")]
        public Color ForeColor { get; set; } = Color.Empty;

        [Category("Colors")]
        public Color SubHeaderForeColor { get; set; } = Color.Empty;

        [Category("Colors")]
        public Color DisabledBackColor { get; set; } = Color.Empty;

        [Category("Colors")]
        public Color DisabledForeColor { get; set; } = Color.Empty;

        [Category("Colors")]
        public Color BorderColor { get; set; } = Color.Empty;
    }
}
