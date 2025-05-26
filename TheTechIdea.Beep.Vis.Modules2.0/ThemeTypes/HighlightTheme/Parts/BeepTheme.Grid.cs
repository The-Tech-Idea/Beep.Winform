using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Grid Fonts
//<<<<<<< HEAD
        public TypographyStyle  GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle  GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle  GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle  GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold);
        public TypographyStyle  GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Italic);
        public TypographyStyle  GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle  GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);

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
