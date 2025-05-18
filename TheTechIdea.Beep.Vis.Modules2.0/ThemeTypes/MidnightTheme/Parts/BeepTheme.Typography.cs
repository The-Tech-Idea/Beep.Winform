using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Typography Styles with default dark theme settings
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.White
        };

        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.WhiteSmoke
        };

        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.Gainsboro
        };

        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gainsboro
        };

        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Italic",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };

        public float BlockquoteBorderWidth { get; set; } = 2f;
        public float BlockquotePadding { get; set; } = 8f;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 11f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.OrangeRed
        };

        public float InlineCodePadding { get; set; } = 4f;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.OrangeRed
        };

        public float CodeBlockBorderWidth { get; set; } = 1.5f;
        public float CodeBlockPadding { get; set; } = 10f;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public float ListItemSpacing { get; set; } = 6f;
        public float ListIndentation { get; set; } = 15f;

        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.CornflowerBlue,
            IsUnderlined = true
        };

        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };

        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Italic",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.White
        };

        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 40f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 36f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 32f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 28f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.WhiteSmoke
        };

        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.Gainsboro
        };

        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Gainsboro
        };

        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.Gainsboro
        };

        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.LightGray
        };

        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.WhiteSmoke
        };

        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Gainsboro
        };
    }
}
