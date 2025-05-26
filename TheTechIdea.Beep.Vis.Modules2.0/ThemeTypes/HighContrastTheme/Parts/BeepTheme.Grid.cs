using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Grid Fonts
//<<<<<<< HEAD
        public TypographyStyle  GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle  GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle  GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle  GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold);
        public TypographyStyle  GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Italic);
        public TypographyStyle  GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Underline);
        public TypographyStyle  GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.Black;
        public Color GridForeColor { get; set; } = Color.White;
        public Color GridHeaderBackColor { get; set; } = Color.DarkSlateGray;
        public Color GridHeaderForeColor { get; set; } = Color.White;
        public Color GridHeaderBorderColor { get; set; } = Color.White;
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color GridHeaderHoverForeColor { get; set; } = Color.Yellow;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.Yellow;
        public Color GridHeaderSelectedForeColor { get; set; } = Color.Black;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.Yellow;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.Black;
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color GridRowHoverForeColor { get; set; } = Color.White;
        public Color GridRowSelectedBackColor { get; set; } = Color.White;
        public Color GridRowSelectedForeColor { get; set; } = Color.Black;
        public Color GridRowHoverBorderColor { get; set; } = Color.Gray;
        public Color GridRowSelectedBorderColor { get; set; } = Color.Yellow;
        public Color GridLineColor { get; set; } = Color.White;
        public Color RowBackColor { get; set; } = Color.Black;
        public Color RowForeColor { get; set; } = Color.White;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(20, 20, 20);
        public Color SelectedRowBackColor { get; set; } = Color.Yellow;
        public Color SelectedRowForeColor { get; set; } = Color.Black;
    }
}
