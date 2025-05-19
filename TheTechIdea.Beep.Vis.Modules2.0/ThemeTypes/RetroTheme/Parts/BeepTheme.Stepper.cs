using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle StepperUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color StepperBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color StepperForeColor { get; set; } = Color.White;
        public Color StepperBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color StepperItemForeColor { get; set; } = Color.White;
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle StepperSubTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(147, 161, 161), IsUnderlined = false, IsStrikeout = false };
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color StepperItemSelectedForeColor { get; set; } = Color.Black;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.Black;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
    }
}