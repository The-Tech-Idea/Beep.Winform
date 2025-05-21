using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 32f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 28f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 24f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 20f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 18f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.LightGray };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 16f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Italic, TextColor = System.Drawing.Color.LightGray };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.LightGray };
        public TypographyStyle Blockquote { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Light, FontStyle = FontStyle.Italic, TextColor = System.Drawing.Color.Gray };
        public float BlockquoteBorderWidth { get; set; } = 2f;
        public float BlockquotePadding { get; set; } = 8f;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle { FontFamily = "Consolas", FontSize = 12f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.Orange };
        public float InlineCodePadding { get; set; } = 4f;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle { FontFamily = "Consolas", FontSize = 12f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.LightGreen };
        public float CodeBlockBorderWidth { get; set; } = 1f;
        public float CodeBlockPadding { get; set; } = 8f;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle OrderedList { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public float ListItemSpacing { get; set; } = 4f;
        public float ListIndentation { get; set; } = 12f;

        public TypographyStyle Link { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Underline, TextColor = System.Drawing.Color.DeepSkyBlue };
        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 10f, FontWeight = FontWeight.Light, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.Gray };
        public TypographyStyle StrongText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Italic, TextColor = System.Drawing.Color.White };

        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 48f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 36f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 30f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };

        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 26f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 22f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 18f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };

        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 20f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 16f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };

        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 16f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Regular, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 10f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = System.Drawing.Color.White };
    }
}
