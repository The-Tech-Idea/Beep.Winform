using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 32f, FontWeight = FontWeight.Bold, TextColor = Color.Black };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 28f, FontWeight = FontWeight.SemiBold, TextColor = Color.Black };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 24f, FontWeight = FontWeight.SemiBold, TextColor = Color.Black };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 20f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 18f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 16f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle Paragraph { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Normal, TextColor = Color.FromArgb(40, 40, 40) };
        public TypographyStyle Blockquote { get; set; } = new TypographyStyle { FontFamily = "Georgia", FontSize = 14f, FontStyle = FontStyle.Italic, TextColor = Color.DarkSlateGray };
        public float BlockquoteBorderWidth { get; set; } = 2f;
        public float BlockquotePadding { get; set; } = 8f;
        public TypographyStyle InlineCode { get; set; } = new TypographyStyle { FontFamily = "Consolas", FontSize = 13f, TextColor = Color.Maroon };
        public float InlineCodePadding { get; set; } = 4f;
        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle { FontFamily = "Consolas", FontSize = 13f, FontWeight = FontWeight.Normal, TextColor = Color.Black };
        public float CodeBlockBorderWidth { get; set; } = 1f;
        public float CodeBlockPadding { get; set; } = 8f;
        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Normal, TextColor = Color.Black };
        public TypographyStyle OrderedList { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Normal, TextColor = Color.Black };
        public float ListItemSpacing { get; set; } = 6f;
        public float ListIndentation { get; set; } = 12f;
        public TypographyStyle Link { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Medium, TextColor = Color.Blue, IsUnderlined = true };
        public bool LinkIsUnderline { get; set; } = true;
        public TypographyStyle SmallText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 10f, TextColor = Color.Gray };
        public TypographyStyle StrongText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.Black };
        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontStyle = FontStyle.Italic, TextColor = Color.Black };
        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 40f, FontWeight = FontWeight.Bold, TextColor = Color.Black };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 36f, FontWeight = FontWeight.Bold, TextColor = Color.Black };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 32f, FontWeight = FontWeight.Bold, TextColor = Color.Black };
        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 28f, FontWeight = FontWeight.SemiBold, TextColor = Color.Black };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 24f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 20f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 18f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 16f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 16f, FontWeight = FontWeight.Normal, TextColor = Color.Black };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Normal, TextColor = Color.Black };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, TextColor = Color.Black };
        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.SemiBold, TextColor = Color.Black };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Medium, TextColor = Color.Black };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 10f, FontWeight = FontWeight.Normal, TextColor = Color.Gray };
    }
}
