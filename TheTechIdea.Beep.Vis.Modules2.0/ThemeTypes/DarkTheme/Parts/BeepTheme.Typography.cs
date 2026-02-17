using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 22f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f
        };

        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.WhiteSmoke,
            LineHeight = 1.18f
        };

        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Gainsboro,
            LineHeight = 1.15f
        };

        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray,
            LineHeight = 1.13f
        };

        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.Silver,
            LineHeight = 1.11f
        };

        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.Gray,
            LineHeight = 1.10f
        };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray,
            LineHeight = 1.1f
        };

        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = FontStyle.Italic,
            TextColor = Color.DimGray,
            LineHeight = 1.1f
        };

        public float BlockquoteBorderWidth { get; set; } = 2f;
        public float BlockquotePadding { get; set; } = 10f;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGreen,
            LineHeight = 1.05f
        };

        public float InlineCodePadding { get; set; } = 3f;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGreen,
            LineHeight = 1.05f
        };

        public float CodeBlockBorderWidth { get; set; } = 1f;
        public float CodeBlockPadding { get; set; } = 8f;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Silver,
            LineHeight = 1.1f
        };

        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Silver,
            LineHeight = 1.1f
        };

        public float ListItemSpacing { get; set; } = 6f;
        public float ListIndentation { get; set; } = 16f;

        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Underline,
            TextColor = Color.CornflowerBlue,
            LineHeight = 1.1f
        };

        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Light,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Gray,
            LineHeight = 1.05f
        };

        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.WhiteSmoke,
            LineHeight = 1.1f
        };

        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Light,
            FontStyle = FontStyle.Italic,
            TextColor = Color.Silver,
            LineHeight = 1.05f
        };

        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 44f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.25f
        };

        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 34f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.WhiteSmoke,
            LineHeight = 1.22f
        };

        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 26f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Gainsboro,
            LineHeight = 1.2f
        };

        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 22f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.15f
        };

        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.WhiteSmoke,
            LineHeight = 1.13f
        };

        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Gainsboro,
            LineHeight = 1.1f
        };

        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightSteelBlue,
            LineHeight = 1.12f
        };

        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightSteelBlue,
            LineHeight = 1.1f
        };

        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Silver,
            LineHeight = 1.08f
        };

        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray,
            LineHeight = 1.1f
        };

        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Silver,
            LineHeight = 1.08f
        };

        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Gray,
            LineHeight = 1.06f
        };

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightSteelBlue,
            LineHeight = 1.07f
        };

        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Silver,
            LineHeight = 1.06f
        };

        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Gray,
            LineHeight = 1.04f
        };
    }
}
