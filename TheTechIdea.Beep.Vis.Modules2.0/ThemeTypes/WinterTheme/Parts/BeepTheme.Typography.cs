using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28,
            LineHeight = 1.2f,
            LetterSpacing = 0.4f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public float BlockquoteBorderWidth { get; set; } = 4f;
        public float BlockquotePadding { get; set; } = 8f;
        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.1f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public float InlineCodePadding { get; set; } = 2f;
        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.1f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public float CodeBlockBorderWidth { get; set; } = 1f;
        public float CodeBlockPadding { get; set; } = 8f;
        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public float ListItemSpacing { get; set; } = 4f;
        public float ListIndentation { get; set; } = 16f;
        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(100, 149, 237),
            IsUnderlined = true,
            IsStrikeout = false
        };
        public bool LinkIsUnderline { get; set; } = true;
        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 48,
            LineHeight = 1.1f,
            LetterSpacing = 0.6f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 36,
            LineHeight = 1.1f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28,
            LineHeight = 1.1f,
            LetterSpacing = 0.4f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28,
            LineHeight = 1.2f,
            LetterSpacing = 0.4f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 22,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}