using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BeepLabelState
    {
        public string HeaderText { get; set; } = string.Empty;
        public string SubHeaderText { get; set; } = string.Empty;
        public bool HasImage { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public bool HideText { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public bool Multiline { get; set; }
        public bool WordWrap { get; set; }
        public bool AutoEllipsis { get; set; }
        public TextImageRelation TextImageRelation { get; set; }
        public ContentAlignment TextAlign { get; set; }
        public ContentAlignment ImageAlign { get; set; }
        public Size MaxImageSize { get; set; }
    }
}
