using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // List Fonts & Colors
//<<<<<<< HEAD
        public Font ListTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font ListSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font ListUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.FromArgb(255, 245, 245, 245); // Light gray
        public Color ListForeColor { get; set; } = Color.Black;
        public Color ListBorderColor { get; set; } = Color.DimGray;

        public Color ListItemForeColor { get; set; } = Color.Black;
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.MediumSlateBlue;

        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.Crimson;
        public Color ListItemSelectedBorderColor { get; set; } = Color.DarkRed;

        public Color ListItemBorderColor { get; set; } = Color.Gray;
        public Color ListItemHoverBorderColor { get; set; } = Color.MediumSlateBlue;
    }
}
