using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Grid Fonts
        public Font GridHeaderFont { get; set; } = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        public Font GridRowFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font GridCellFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font GridCellSelectedFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Bold);
        public Font GridCellHoverFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Italic);
        public Font GridCellErrorFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font GridColumnFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(34, 45, 26);
        public Color GridForeColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(27, 50, 15);
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(200, 230, 201);
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(56, 87, 33);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(41, 76, 20);
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(220, 245, 210);
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(69, 123, 36);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(66, 108, 41);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(84, 135, 45);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(46, 64, 38);
        public Color GridRowHoverForeColor { get; set; } = Color.White;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(61, 90, 27);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(54, 83, 33);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(69, 108, 41);
        public Color GridLineColor { get; set; } = Color.FromArgb(56, 70, 39);
        public Color RowBackColor { get; set; } = Color.FromArgb(28, 40, 19);
        public Color RowForeColor { get; set; } = Color.FromArgb(220, 230, 210);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(35, 50, 25);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(69, 108, 41);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
