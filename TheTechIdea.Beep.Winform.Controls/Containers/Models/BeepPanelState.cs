using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Containers.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BeepPanelState
    {
        public string TitleText { get; set; } = string.Empty;
        public bool ShowTitle { get; set; }
        public bool ShowTitleLine { get; set; }
        public bool ShowTitleLineFullWidth { get; set; } = true;
        public ContentAlignment TitleAlignment { get; set; } = ContentAlignment.TopLeft;
        public PanelTitleStyle TitleStyle { get; set; } = PanelTitleStyle.GroupBox;
        public PanelShape PanelShape { get; set; } = PanelShape.RoundedRectangle;
        public int TitleGap { get; set; } = 8;
        public int TitleLineThickness { get; set; } = 1;
        public int BorderThickness { get; set; } = 1;
        public int BorderRadius { get; set; } = 6;
        public Padding Padding { get; set; } = Padding.Empty;
        public bool UseThemeColors { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool ShowTitleIcon { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public string ResolvedIconPath { get; set; } = string.Empty;
        public int TitleIconSize { get; set; } = 16;
        public int TitleIconGap { get; set; } = 6;
        public bool TitleIconTintWithForeColor { get; set; } = true;
    }
}
