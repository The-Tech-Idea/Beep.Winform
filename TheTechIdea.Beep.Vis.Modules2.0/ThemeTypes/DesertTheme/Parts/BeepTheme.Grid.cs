using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
        public TypographyStyle GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(255, 244, 229);          // Light sandy beige
        public Color GridForeColor { get; set; } = Color.FromArgb(77, 54, 32);              // Dark brown
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(210, 180, 140);    // Tan
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(51, 34, 17);        // Deep brown
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(176, 141, 90);    // Medium tan
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);// Light tan
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(61, 40, 20);   // Dark brown
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(198, 134, 66); // Desert orange
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(180, 120, 60);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(153, 102, 51);

        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(255, 250, 240);
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(80, 60, 30);
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(244, 164, 96);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(198, 134, 66);

        public Color GridLineColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color RowBackColor { get; set; } = Color.FromArgb(255, 244, 229);
        public Color RowForeColor { get; set; } = Color.FromArgb(77, 54, 32);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(250, 240, 215);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(244, 164, 96);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
