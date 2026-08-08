using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle ScrollListTitleFont { get; set; }
        public TypographyStyle ScrollListSelectedFont { get; set; }
        public TypographyStyle ScrollListUnSelectedFont { get; set; }
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public TypographyStyle ScrollListIItemFont { get; set; }
        public TypographyStyle ScrollListItemSelectedFont { get; set; }
    }
}
