using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = Color.FromArgb(94, 57, 34), // Dark warm brown
            LineHeight = 38f,
            LetterSpacing = 0.5f
        };

        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(112, 73, 42),
            LineHeight = 34f,
            LetterSpacing = 0.4f
        };

        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(129, 89, 52),
            LineHeight = 30f,
            LetterSpacing = 0.3f
        };

        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(147, 105, 62),
            LineHeight = 26f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(160, 114, 68),
            LineHeight = 24f,
            LetterSpacing = 0.15f
        };

        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(173, 124, 74),
            LineHeight = 22f,
            LetterSpacing = 0.1f
        };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(111, 85, 60),
            LineHeight = 20f,
            LetterSpacing = 0f
        };

        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Italic",
            FontSize = 14f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = Color.FromArgb(130, 100, 70),
            LineHeight = 22f,
            LetterSpacing = 0f
        };

        public float BlockquoteBorderWidth { get; set; } = 3f;
        public float BlockquotePadding { get; set; } = 10f;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(115, 82, 56),
            LineHeight = 18f,
            LetterSpacing = 0f
        };
        public float InlineCodePadding { get; set; } = 3f;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(115, 82, 56),
            LineHeight = 20f,
            LetterSpacing = 0f
        };
        public float CodeBlockBorderWidth { get; set; } = 2f;
        public float CodeBlockPadding { get; set; } = 8f;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(111, 85, 60),
            LineHeight = 20f,
            LetterSpacing = 0f
        };

        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(111, 85, 60),
            LineHeight = 20f,
            LetterSpacing = 0f
        };

        public float ListItemSpacing { get; set; } = 6f;
        public float ListIndentation { get; set; } = 15f;

        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(183, 110, 65),
            IsUnderlined = true
        };
        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(120, 90, 60)
        };

        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(94, 57, 34)
        };

        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Italic",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = Color.FromArgb(111, 85, 60)
        };

        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 40f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(94, 57, 34),
            LineHeight = 48f
        };

        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(112, 73, 42),
            LineHeight = 38f
        };

        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(129, 89, 52),
            LineHeight = 30f
        };

        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(147, 105, 62),
            LineHeight = 26f
        };

        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(160, 114, 68),
            LineHeight = 24f
        };

        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(173, 124, 74),
            LineHeight = 22f
        };

        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(160, 114, 68),
            LineHeight = 24f
        };

        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(173, 124, 74),
            LineHeight = 22f
        };

        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(183, 133, 89),
            LineHeight = 20f
        };

        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(111, 85, 60),
            LineHeight = 22f
        };

        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(120, 90, 60),
            LineHeight = 20f
        };

        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Light,
            TextColor = Color.FromArgb(140, 105, 75),
            LineHeight = 18f
        };

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(94, 57, 34)
        };

        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(111, 85, 60)
        };

        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(130, 100, 70)
        };
    }
}
