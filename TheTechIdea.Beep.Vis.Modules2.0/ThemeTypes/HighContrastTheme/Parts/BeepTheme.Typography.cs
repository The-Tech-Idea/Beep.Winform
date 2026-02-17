namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 22f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f
        };

        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.4f
        };

        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f
        };

        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.1f
        };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.5f,
            LetterSpacing = 0.25f
        };

        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.LightGray,
            LineHeight = 1.4f,
            LetterSpacing = 0.25f
        };
        public float BlockquoteBorderWidth { get; set; } = 2f;
        public float BlockquotePadding { get; set; } = 8f;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Yellow,
            LineHeight = 1f,
            LetterSpacing = 0.1f
        };
        public float InlineCodePadding { get; set; } = 4f;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Yellow,
            LineHeight = 1f,
            LetterSpacing = 0.1f
        };
        public float CodeBlockBorderWidth { get; set; } = 2f;
        public float CodeBlockPadding { get; set; } = 8f;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.5f,
            LetterSpacing = 0.25f
        };

        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.5f,
            LetterSpacing = 0.25f
        };

        public float ListItemSpacing { get; set; } = 4f;
        public float ListIndentation { get; set; } = 16f;

        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Underline,
            TextColor = System.Drawing.Color.Cyan,
            LineHeight = 1f,
            LetterSpacing = 0.2f,
            IsUnderlined = true
        };
        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.LightGray,
            LineHeight = 1f,
            LetterSpacing = 0.1f
        };

        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.LightGray,
            LineHeight = 1f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 44f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.1f,
            LetterSpacing = 0.5f
        };

        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 34f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.1f,
            LetterSpacing = 0.4f
        };

        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 26f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.1f,
            LetterSpacing = 0.3f
        };

        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 22f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f
        };

        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f
        };

        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.5f,
            LetterSpacing = 0.25f
        };

        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.5f,
            LetterSpacing = 0.25f
        };

        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.5f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.1f
        };
    }
}
