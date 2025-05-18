using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // ScrollList Fonts & Colors
        public Font ScrollListTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font ScrollListSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font ScrollListUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color ScrollListBackColor { get; set; } = Color.FromArgb(250, 240, 230); // soft sandy beige
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(101, 67, 33); // dark brown text
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // tan border

        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(101, 67, 33); // dark brown
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(139, 69, 19); // saddle brown hover
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(255, 248, 220); // cornsilk hover bg

        public Color ScrollListItemSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 240); // ivory selected text
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(210, 180, 140); // tan selected bg
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // saddle brown selected border

        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // tan item border
        public Color ScrollListItemHoverBorderColor { get; set; } = Color.FromArgb(244, 164, 96); // sandy brown hover border

        public Font ScrollListIItemFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Font ScrollListItemSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
    }
}
