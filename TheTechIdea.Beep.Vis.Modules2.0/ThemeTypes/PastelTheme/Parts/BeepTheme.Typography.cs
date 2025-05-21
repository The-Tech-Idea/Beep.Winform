using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle() { FontSize = 32, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle Heading2 { get; set; } = new TypographyStyle() { FontSize = 28, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle Heading3 { get; set; } = new TypographyStyle() { FontSize = 24, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle Heading4 { get; set; } = new TypographyStyle() { FontSize = 20, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle Heading5 { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle Heading6 { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle Paragraph { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120), LineHeight = 1.5f };
        public TypographyStyle Blockquote { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120), FontStyle = FontStyle.Italic, LineHeight = 1.5f };
        public float BlockquoteBorderWidth { get; set; } = 4;
        public float BlockquotePadding { get; set; } = 12;
        public TypographyStyle InlineCode { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80), FontFamily = "Consolas" };
        public float InlineCodePadding { get; set; } = 4;
        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80), FontFamily = "Consolas", LineHeight = 1.5f };
        public float CodeBlockBorderWidth { get; set; } = 1;
        public float CodeBlockPadding { get; set; } = 8;
        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120), LineHeight = 1.5f };
        public TypographyStyle OrderedList { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120), LineHeight = 1.5f };
        public float ListItemSpacing { get; set; } = 8;
        public float ListIndentation { get; set; } = 20;
        public TypographyStyle Link { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(245, 183, 203), IsUnderlined = true };
        public bool LinkIsUnderline { get; set; } = true;
        public TypographyStyle SmallText { get; set; } = new TypographyStyle() { FontSize = 10, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle StrongText { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80), FontStyle = FontStyle.Italic };
        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle() { FontSize = 48, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.1f };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle() { FontSize = 36, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.1f };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle() { FontSize = 28, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.1f };
        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle() { FontSize = 32, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle() { FontSize = 28, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle() { FontSize = 24, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.2f };
        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle() { FontSize = 22, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.3f };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle() { FontSize = 18, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.3f };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.3f };
        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120), LineHeight = 1.5f };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120), LineHeight = 1.5f };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120), LineHeight = 1.5f };
        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.4f };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.4f };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle() { FontSize = 10, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80), LineHeight = 1.4f };
    }
}