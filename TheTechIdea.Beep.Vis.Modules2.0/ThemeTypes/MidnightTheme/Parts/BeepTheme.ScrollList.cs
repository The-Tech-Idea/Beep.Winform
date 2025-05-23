using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // ScrollList Fonts & Colors
//<<<<<<< HEAD
        public Font ScrollListTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font ScrollListSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font ScrollListUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ScrollListForeColor { get; set; } = Color.WhiteSmoke;
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color ScrollListItemForeColor { get; set; } = Color.LightGray;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(90, 90, 90);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Font ScrollListIItemFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Font ScrollListItemSelectedFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Bold);
    }
}
