using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Typography Styles with Material Design defaults
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 96f,
            FontWeight = FontWeight.Light,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 60f,
            FontWeight = FontWeight.Light,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 48f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 34f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 24f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 20f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(66, 66, 66)
        };

        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117),
            FontStyle = FontStyle.Italic
        };

        public float BlockquoteBorderWidth { get; set; } = 4f;
        public float BlockquotePadding { get; set; } = 16f;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto Mono",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(211, 47, 47)
        };

        public float InlineCodePadding { get; set; } = 6f;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto Mono",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(211, 47, 47)
        };

        public float CodeBlockBorderWidth { get; set; } = 2f;
        public float CodeBlockPadding { get; set; } = 12f;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public float ListItemSpacing { get; set; } = 8f;
        public float ListIndentation { get; set; } = 20f;

        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(25, 118, 210),
            IsUnderlined = true
        };

        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117)
        };

        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33),
            FontStyle = FontStyle.Italic
        };

        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 112f,
            FontWeight = FontWeight.Light,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 56f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 45f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 32f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 28f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 24f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 22f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 11f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };
    }
}
