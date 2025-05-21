using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle GridRowFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle GridCellHoverFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public TypographyStyle GridCellErrorFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(255, 100, 100) };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color GridForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color GridHeaderForeColor { get; set; } = Color.White;
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color GridHeaderHoverForeColor { get; set; } = Color.White;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color GridRowHoverForeColor { get; set; } = Color.White;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color GridLineColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color RowBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color RowForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(0, 115, 158);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}