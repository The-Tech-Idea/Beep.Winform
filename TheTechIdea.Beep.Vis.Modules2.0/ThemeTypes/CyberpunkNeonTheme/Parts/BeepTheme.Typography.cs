using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Typography Styles
        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 36,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(0, 255, 255), // Neon Cyan
            IsUnderlined = false
        };

        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 30,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(255, 0, 255) // Neon Magenta
        };

        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 24,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(255, 255, 0) // Neon Yellow
        };

        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 20,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(0, 255, 128) // Neon Green
        };

        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 18,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(255, 0, 255) // Neon Magenta
        };

        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 16,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(0, 255, 255) // Neon Cyan
        };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.Regular,
            TextColor = Color.White
        };

        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
             FontStyle= FontStyle.Italic,
            TextColor = Color.FromArgb(128, 255, 255, 255) // Semi-transparent white
        };

        public float BlockquoteBorderWidth { get; set; } = 2;
        public float BlockquotePadding { get; set; } = 8;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 0, 255) // Neon Magenta
        };

        public float InlineCodePadding { get; set; } = 4;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(0, 255, 255) // Neon Cyan
        };

        public float CodeBlockBorderWidth { get; set; } = 2;
        public float CodeBlockPadding { get; set; } = 8;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            TextColor = Color.White
        };

        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            TextColor = Color.White
        };

        public float ListItemSpacing { get; set; } = 6;
        public float ListIndentation { get; set; } = 20;

        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            TextColor = Color.FromArgb(0, 255, 255), // Neon Cyan
            IsUnderlined = true
        };

        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 10,
            TextColor = Color.Gray
        };

        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontStyle = FontStyle.Italic,
            TextColor = Color.White
        };

        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 48,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(0, 255, 255)
        };

        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 40,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 0, 255)
        };

        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 32,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 255, 0)
        };

        public TypographyStyle HeadlineLarge { get; set; } 
        public TypographyStyle HeadlineMedium { get; set; }
        public TypographyStyle HeadlineSmall { get; set; }

        public TypographyStyle TitleLarge { get; set; } 
        public TypographyStyle TitleMedium { get; set; } 
        public TypographyStyle TitleSmall { get; set; } 

        public TypographyStyle BodyLarge { get; set; } 
        public TypographyStyle BodyMedium { get; set; } 
        public TypographyStyle BodySmall { get; set; } 

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            TextColor = Color.White
        };

        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 10,
            TextColor = Color.Gray
        };
    }
}
