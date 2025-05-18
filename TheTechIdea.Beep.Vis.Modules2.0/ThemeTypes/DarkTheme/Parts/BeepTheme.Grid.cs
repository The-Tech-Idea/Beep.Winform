using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Grid Fonts
        public Font GridHeaderFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Bold);
        public Font GridRowFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font GridCellFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font GridCellSelectedFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Bold);
        public Font GridCellHoverFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Italic);
        public Font GridCellErrorFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font GridColumnFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color GridForeColor { get; set; } = Color.LightGray;
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color GridHeaderForeColor { get; set; } = Color.White;
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color GridHeaderHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(100, 100, 100);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(45, 45, 45);
        public Color GridRowHoverForeColor { get; set; } = Color.White;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(90, 90, 90);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(70, 130, 180);
        public Color GridLineColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color RowBackColor { get; set; } = Color.FromArgb(35, 35, 35);
        public Color RowForeColor { get; set; } = Color.LightGray;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(25, 25, 25);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(70, 130, 180);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
