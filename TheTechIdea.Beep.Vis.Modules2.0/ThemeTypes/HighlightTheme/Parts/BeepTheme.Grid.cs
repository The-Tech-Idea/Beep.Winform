using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Grid Fonts
        public Font GridHeaderFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font GridRowFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font GridCellFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font GridCellSelectedFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Bold);
        public Font GridCellHoverFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Italic);
        public Font GridCellErrorFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font GridColumnFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.White;
        public Color GridForeColor { get; set; } = Color.Black;
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(255, 230, 180); // Light Orange
        public Color GridHeaderForeColor { get; set; } = Color.Black;
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(255, 200, 150);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(255, 210, 160);
        public Color GridHeaderHoverForeColor { get; set; } = Color.Black;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(255, 180, 100);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(255, 170, 90);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(255, 160, 80);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(255, 245, 220);
        public Color GridRowHoverForeColor { get; set; } = Color.Black;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(255, 210, 150);
        public Color GridRowSelectedForeColor { get; set; } = Color.Black;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(255, 220, 170);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(255, 200, 140);
        public Color GridLineColor { get; set; } = Color.FromArgb(230, 200, 170);
        public Color RowBackColor { get; set; } = Color.White;
        public Color RowForeColor { get; set; } = Color.Black;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(255, 250, 230);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(255, 215, 170);
        public Color SelectedRowForeColor { get; set; } = Color.Black;
    }
}
