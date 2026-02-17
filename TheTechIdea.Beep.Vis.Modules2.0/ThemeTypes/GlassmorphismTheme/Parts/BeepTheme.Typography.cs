namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 22f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 20f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 18f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 16f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle Paragraph { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Normal, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle Blockquote { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 10f, FontWeight = FontWeight.Light, FontStyle = System.Drawing.FontStyle.Italic, TextColor = System.Drawing.Color.DarkSlateGray };
        public float BlockquoteBorderWidth { get; set; } = 2f;
        public float BlockquotePadding { get; set; } = 8f;
        public TypographyStyle InlineCode { get; set; } = new TypographyStyle { FontFamily = "Consolas", FontSize = 8f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.DarkSlateBlue };
        public float InlineCodePadding { get; set; } = 4f;
        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle { FontFamily = "Consolas", FontSize = 8f, FontWeight = FontWeight.Normal, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public float CodeBlockBorderWidth { get; set; } = 1f;
        public float CodeBlockPadding { get; set; } = 10f;
        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Normal, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle OrderedList { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Normal, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public float ListItemSpacing { get; set; } = 6f;
        public float ListIndentation { get; set; } = 16f;
        public TypographyStyle Link { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Normal, FontStyle = System.Drawing.FontStyle.Underline, TextColor = System.Drawing.Color.Blue };
        public bool LinkIsUnderline { get; set; } = true;
        public TypographyStyle SmallText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Light, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Gray };
        public TypographyStyle StrongText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Regular, FontStyle = System.Drawing.FontStyle.Italic, TextColor = System.Drawing.Color.DarkSlateGray };
        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 44f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 34f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 26f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 22f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 20f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 18f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 16f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Bold, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 10f, FontWeight = FontWeight.Normal, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Normal, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 10f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 8f, FontWeight = FontWeight.Medium, FontStyle = System.Drawing.FontStyle.Regular, TextColor = System.Drawing.Color.Black };
    }
}
