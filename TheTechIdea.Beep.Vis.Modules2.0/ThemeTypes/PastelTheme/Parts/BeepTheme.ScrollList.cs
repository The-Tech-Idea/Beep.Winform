using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle ScrollListTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle ScrollListSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public TypographyStyle ScrollListIItemFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
    }
}