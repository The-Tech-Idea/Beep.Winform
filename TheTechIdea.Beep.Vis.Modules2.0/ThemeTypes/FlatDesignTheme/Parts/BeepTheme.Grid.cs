using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold);
        public TypographyStyle GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Italic);
        public TypographyStyle GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.White;
        public Color GridForeColor { get; set; } = Color.Black;
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color GridHeaderForeColor { get; set; } = Color.Black;
        public Color GridHeaderBorderColor { get; set; } = Color.Gray;
        public Color GridHeaderHoverBackColor { get; set; } = Color.LightGray;
        public Color GridHeaderHoverForeColor { get; set; } = Color.Black;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215); // Blue highlight
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.DarkGray;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.DarkBlue;
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color GridRowHoverForeColor { get; set; } = Color.Black;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.DarkGray;
        public Color GridRowSelectedBorderColor { get; set; } = Color.DarkBlue;
        public Color GridLineColor { get; set; } = Color.LightGray;
        public Color RowBackColor { get; set; } = Color.White;
        public Color RowForeColor { get; set; } = Color.Black;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
