using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11.5f, FontStyle.Bold);
        public TypographyStyle GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);
        public TypographyStyle GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);
        public TypographyStyle GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
        public TypographyStyle GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Italic);
        public TypographyStyle GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(18, 18, 32);                 // Cyberpunk Black
        public Color GridForeColor { get; set; } = Color.FromArgb(0, 255, 255);                // Neon Cyan

        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(34, 34, 68);           // Cyberpunk Panel
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(255, 0, 255);          // Neon Magenta
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon Cyan

        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(0, 255, 255);     // Neon Cyan
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(255, 0, 255);   // Neon Magenta

        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);  // Neon Magenta
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128);// Neon Green

        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(34, 34, 68);         // Slightly lighter
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(0, 255, 128);        // Neon Green
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);      // Neon Yellow

        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(0, 102, 255);     // Neon Blue
        public Color GridRowSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon Cyan

        public Color GridLineColor { get; set; } = Color.FromArgb(54, 162, 235);               // Neon Soft Blue

        public Color RowBackColor { get; set; } = Color.FromArgb(24, 24, 48);                  // Cyberpunk Black (row)
        public Color RowForeColor { get; set; } = Color.FromArgb(0, 255, 255);                 // Neon Cyan
        public Color AltRowBackColor { get; set; } = Color.FromArgb(34, 34, 68);               // Slightly lighter for alt rows

        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(255, 0, 255);         // Neon Magenta (selection)
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
