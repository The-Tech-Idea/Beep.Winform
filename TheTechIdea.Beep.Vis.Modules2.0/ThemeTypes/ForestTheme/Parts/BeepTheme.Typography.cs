using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(34, 77, 34),
            LineHeight = 1.3f,
            LetterSpacing = 0
        };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(34, 77, 34),
            LineHeight = 1.3f,
            LetterSpacing = 0
        };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(50, 90, 50),
            LineHeight = 1.25f,
            LetterSpacing = 0
        };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(50, 90, 50),
            LineHeight = 1.2f,
            LetterSpacing = 0
        };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 110, 60),
            LineHeight = 1.15f,
            LetterSpacing = 0
        };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 110, 60),
            LineHeight = 1.1f,
            LetterSpacing = 0
        };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(80, 120, 80),
            LineHeight = 1.5f,
            LetterSpacing = 0
        };

        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(100, 140, 100),
        };
        public float BlockquoteBorderWidth { get; set; } = 3f;
        public float BlockquotePadding { get; set; } = 15f;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(50, 90, 50),
        };
        public float InlineCodePadding { get; set; } = 4f;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(40, 80, 40),
        };
        public float CodeBlockBorderWidth { get; set; } = 1.5f;
        public float CodeBlockPadding { get; set; } = 10f;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(70, 110, 70),
        };

        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(70, 110, 70),
        };

        public float ListItemSpacing { get; set; } = 6f;
        public float ListIndentation { get; set; } = 18f;

        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(34, 77, 34),
            IsUnderlined = true,
        };
        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(90, 130, 90),
        };

        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(34, 77, 34),
        };

        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(50, 90, 50),
        };

        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 48,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(20, 60, 20),
        };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 40,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(25, 65, 25),
        };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(30, 70, 30),
        };

        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(34, 77, 34),
        };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(40, 80, 40),
        };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(50, 90, 50),
        };

        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 22,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(50, 100, 50),
        };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 110, 60),
        };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(70, 120, 70),
        };

        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(80, 130, 80),
        };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(90, 140, 90),
        };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(100, 150, 100),
        };

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(60, 110, 60),
        };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(70, 120, 70),
        };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(80, 130, 80),
        };
    }
}
