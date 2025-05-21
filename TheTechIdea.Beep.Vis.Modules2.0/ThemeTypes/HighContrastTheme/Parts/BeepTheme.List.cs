using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // List Fonts & Colors
//<<<<<<< HEAD
        public Font ListTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font ListSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font ListUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.Black;
        public Color ListForeColor { get; set; } = Color.White;
        public Color ListBorderColor { get; set; } = Color.White;
        public Color ListItemForeColor { get; set; } = Color.White;
        public Color ListItemHoverForeColor { get; set; } = Color.Black;
        public Color ListItemHoverBackColor { get; set; } = Color.Yellow;
        public Color ListItemSelectedForeColor { get; set; } = Color.Black;
        public Color ListItemSelectedBackColor { get; set; } = Color.Cyan;
        public Color ListItemSelectedBorderColor { get; set; } = Color.White;
        public Color ListItemBorderColor { get; set; } = Color.White;
        public Color ListItemHoverBorderColor { get; set; } = Color.Yellow;
    }
}
