using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // List Fonts & Colors
        public Font ListTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font ListSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font ListUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color ListForeColor { get; set; } = Color.Black;
        public Color ListBorderColor { get; set; } = Color.FromArgb(200, 210, 220);

        public Color ListItemForeColor { get; set; } = Color.Black;
        public Color ListItemHoverForeColor { get; set; } = Color.Black;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(230, 240, 250);

        public Color ListItemSelectedForeColor { get; set; } = Color.Black;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color ListItemSelectedBorderColor { get; set; } = Color.SteelBlue;

        public Color ListItemBorderColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color ListItemHoverBorderColor { get; set; } = Color.CornflowerBlue;
    }
}
