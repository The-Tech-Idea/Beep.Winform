using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 32,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 28,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 24,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 20,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public float BlockquoteBorderWidth { get; set; } = 4f;
        public float BlockquotePadding { get; set; } = 12f;
        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(65, 65, 145), // Royal blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public float InlineCodePadding { get; set; } = 4f;
        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public float CodeBlockBorderWidth { get; set; } = 2f;
        public float CodeBlockPadding { get; set; } = 12f;
        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public float ListItemSpacing { get; set; } = 8f;
        public float ListIndentation { get; set; } = 20f;
        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(65, 65, 145), // Royal blue
            IsUnderlined = true,
            IsStrikeout = false
        };
        public bool LinkIsUnderline { get; set; } = true;
        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 10,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 48,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 36,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 28,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 28,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 24,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 20,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 20,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}