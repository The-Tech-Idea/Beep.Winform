using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class LabelStyleConfig
    {
        [Category("Layout")]
        public TextImageRelation TextImageRelation { get; set; } = TextImageRelation.ImageBeforeText;

        [Category("Layout")]
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleLeft;

        [Category("Layout")]
        public ContentAlignment ImageAlign { get; set; } = ContentAlignment.MiddleLeft;

        [Category("Layout")]
        public int HeaderSubheaderSpacing { get; set; } = 2;

        [Category("Behavior")]
        public bool Multiline { get; set; }

        [Category("Behavior")]
        public bool WordWrap { get; set; }

        [Category("Behavior")]
        public bool AutoEllipsis { get; set; }
    }
}
