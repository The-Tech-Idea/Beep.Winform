using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // ScrollList Fonts & Colors
//<<<<<<< HEAD
        public Font ScrollListTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font ScrollListSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font ScrollListUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color ScrollListBackColor { get; set; } = Color.Black;
        public Color ScrollListForeColor { get; set; } = Color.White;
        public Color ScrollListBorderColor { get; set; } = Color.White;
        public Color ScrollListItemForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.Yellow;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.DarkSlateGray;
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.Black;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.Yellow;
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.Orange;
        public Color ScrollListItemBorderColor { get; set; } = Color.Gray;
        public Font ScrollListIItemFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Font ScrollListItemSelectedFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Bold);
    }
}
