using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // ScrollList Fonts & Colors
        public Font ScrollListTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font ScrollListSelectedFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Bold);
        public Font ScrollListUnSelectedFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Regular);
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(34, 49, 34);  // Dark forest green
        public Color ScrollListForeColor { get; set; } = Color.WhiteSmoke;
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(56, 142, 60); // Medium green border
        public Color ScrollListItemForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(200, 230, 201); // Light green hover fore
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(56, 142, 60); // Medium green hover back
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(102, 187, 106); // Bright green selected back
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(76, 175, 80); // Selected border green
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(46, 71, 46); // Darker green border
        public Font ScrollListIItemFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Regular);
        public Font ScrollListItemSelectedFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Bold);
    }
}
