using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Typography Styles with inline property initializers
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
            FontWeight = FontWeight.Medium,
            TextColor = Color.Black
        };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Medium,
            TextColor = Color.Black
        };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            TextColor = Color.Black
        };
        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Italic",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = Color.Gray
        };
        public float BlockquoteBorderWidth { get; set; } = 2f;
        public float BlockquotePadding { get; set; } = 10f;
        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13,
            TextColor = Color.DarkRed,
            FontWeight = FontWeight.Normal,
            IsUnderlined = false
        };
        public float InlineCodePadding { get; set; } = 3f;
        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13,
            TextColor = Color.DarkRed,
            FontWeight = FontWeight.Normal
        };
        public float CodeBlockBorderWidth { get; set; } = 2f;
        public float CodeBlockPadding { get; set; } = 8f;
        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            TextColor = Color.Black
        };
        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            TextColor = Color.Black
        };
        public float ListItemSpacing { get; set; } = 4f;
        public float ListIndentation { get; set; } = 16f;
        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            TextColor = Color.Blue,
            IsUnderlined = true
        };
        public bool LinkIsUnderline { get; set; } = true;
        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            TextColor = Color.Gray
        };
        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };
        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Italic",
            FontSize = 14,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = Color.Black
        };
        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 57,
            FontWeight = FontWeight.Light,
            TextColor = Color.Black
        };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 45,
            FontWeight = FontWeight.Light,
            TextColor = Color.Black
        };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 36,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 22,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };
        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };
    }
}
