using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // Menu Fonts & Colors with default inline values
        public Font MenuTitleFont { get; set; } = new Font("Roboto", 18f, FontStyle.Bold);
        public Font MenuItemSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);
        public Font MenuItemUnSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.FromArgb(250, 250, 250);
        public Color MenuForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue 500
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(25, 118, 210); // Material Blue 700
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(55, 71, 79); // Blue Grey 800
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(30, 136, 229); // Blue 600
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 800

        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(238, 238, 238);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(245, 245, 245);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
