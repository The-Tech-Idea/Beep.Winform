using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Grid Fonts
//<<<<<<< HEAD
        public Font GridHeaderFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font GridRowFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font GridCellFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font GridCellSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font GridCellHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);
        public Font GridCellErrorFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Strikeout);
        public Font GridColumnFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color GridForeColor { get; set; } = Color.White;
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color GridHeaderForeColor { get; set; } = Color.White;
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Dark Border
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor
        public Color GridHeaderHoverForeColor { get; set; } = Color.LightGray;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(0x2D, 0xCC, 0x70); // Greenish
        public Color GridHeaderSelectedForeColor { get; set; } = Color.Black;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.Gray;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.White;

        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Deep hover
        public Color GridRowHoverForeColor { get; set; } = Color.White;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Light Blue
        public Color GridRowSelectedForeColor { get; set; } = Color.Black;
        public Color GridRowHoverBorderColor { get; set; } = Color.LightBlue;
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1);

        public Color GridLineColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle grid lines
        public Color RowBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color RowForeColor { get; set; } = Color.White;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(0x25, 0x21, 0x3F); // Slightly lighter
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
