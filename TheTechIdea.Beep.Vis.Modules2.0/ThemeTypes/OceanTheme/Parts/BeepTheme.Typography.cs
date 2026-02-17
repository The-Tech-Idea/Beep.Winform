using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle() { FontSize = 22f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle() { FontSize = 20f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle() { FontSize = 18f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle() { FontSize = 16f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle() { FontSize = 12f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle Paragraph { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180), LineHeight = 1.5f };
        public TypographyStyle Blockquote { get; set; } = new TypographyStyle() { FontSize = 10f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180), FontStyle = FontStyle.Italic, LineHeight = 1.5f };
        public float BlockquoteBorderWidth { get; set; } = 4;
        public float BlockquotePadding { get; set; } = 12;
        public TypographyStyle InlineCode { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 80, 120), FontFamily = "Consolas" };
        public float InlineCodePadding { get; set; } = 4;
        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 80, 120), FontFamily = "Consolas", LineHeight = 1.5f };
        public float CodeBlockBorderWidth { get; set; } = 1;
        public float CodeBlockPadding { get; set; } = 8;
        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180), LineHeight = 1.5f };
        public TypographyStyle OrderedList { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180), LineHeight = 1.5f };
        public float ListItemSpacing { get; set; } = 8;
        public float ListIndentation { get; set; } = 20;
        public TypographyStyle Link { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 150, 200), IsUnderlined = true };
        public bool LinkIsUnderline { get; set; } = true;
        public TypographyStyle SmallText { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180) };
        public TypographyStyle StrongText { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120) };
        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 80, 120), FontStyle = FontStyle.Italic };
        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle() { FontSize = 44f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.1f };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle() { FontSize = 34f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.1f };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle() { FontSize = 26f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.1f };
        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle() { FontSize = 22f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle() { FontSize = 20f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle() { FontSize = 18f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.2f };
        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle() { FontSize = 16f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.3f };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.3f };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle() { FontSize = 12f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.3f };
        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle() { FontSize = 12f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180), LineHeight = 1.5f };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle() { FontSize = 10f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180), LineHeight = 1.5f };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180), LineHeight = 1.5f };
        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle() { FontSize = 12f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.4f };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle() { FontSize = 10f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.4f };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 80, 120), LineHeight = 1.4f };
    }
}
