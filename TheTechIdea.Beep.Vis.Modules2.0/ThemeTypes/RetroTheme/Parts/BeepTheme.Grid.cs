using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle GridRowFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle GridCellHoverFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.Yellow, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle GridCellErrorFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.Red, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color GridForeColor { get; set; } = Color.White;
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color GridHeaderForeColor { get; set; } = Color.White;
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color GridHeaderHoverForeColor { get; set; } = Color.White;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.Black;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color GridRowHoverForeColor { get; set; } = Color.White;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color GridRowSelectedForeColor { get; set; } = Color.Black;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color GridLineColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color RowBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color RowForeColor { get; set; } = Color.White;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color SelectedRowForeColor { get; set; } = Color.Black;
    }
}