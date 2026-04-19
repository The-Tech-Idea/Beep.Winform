using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular | FontStyle.Underline);
        public TypographyStyle GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.White;
        public Color GridForeColor { get; set; } = Color.Black;
        public Color GridHeaderBackColor { get; set; } = Color.LightGray;
        public Color GridHeaderForeColor { get; set; } = Color.Black;
        public Color GridHeaderBorderColor { get; set; } = Color.DarkGray;
        public Color GridHeaderHoverBackColor { get; set; } = Color.Gray;
        public Color GridHeaderHoverForeColor { get; set; } = Color.White;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.DimGray;
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.DarkGray;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.Black;
        public Color GridRowHoverBackColor { get; set; } = Color.LightGray;
        public Color GridRowHoverForeColor { get; set; } = Color.Black;
        public Color GridRowSelectedBackColor { get; set; } = Color.DarkGray;
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.Gray;
        public Color GridRowSelectedBorderColor { get; set; } = Color.Black;
        public Color GridLineColor { get; set; } = Color.LightGray;
        public Color RowBackColor { get; set; } = Color.White;
        public Color RowForeColor { get; set; } = Color.Black;
        public Color AltRowBackColor { get; set; } = Color.Gainsboro;
        public Color SelectedRowBackColor { get; set; } = Color.DimGray;
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
