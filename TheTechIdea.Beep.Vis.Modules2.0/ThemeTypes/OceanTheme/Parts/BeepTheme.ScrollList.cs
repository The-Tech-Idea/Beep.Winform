using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle ScrollListTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle ScrollListSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color ScrollListItemHoverForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public TypographyStyle ScrollListIItemFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
    }
}