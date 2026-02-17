namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Typography Styles

        public TypographyStyle Heading1 { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 22f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.18f
        };

        public TypographyStyle Heading2 { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LineHeight = 1.15f
        };

        public TypographyStyle Heading3 { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(127, 255, 212), // Mint
            LineHeight = 1.14f
        };

        public TypographyStyle Heading4 { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(255, 223, 93), // Lemon
            LineHeight = 1.12f
        };

        public TypographyStyle Heading5 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.10f
        };

        public TypographyStyle Heading6 { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.FromArgb(206, 183, 255), // Pastel Lavender
            LineHeight = 1.09f
        };

        public TypographyStyle Paragraph { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(44, 62, 80), // Navy
            LineHeight = 1.07f
        };

        public TypographyStyle Blockquote { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LineHeight = 1.08f
        };
        public float BlockquoteBorderWidth { get; set; } = 3.0f;
        public float BlockquotePadding { get; set; } = 12f;

        public TypographyStyle InlineCode { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.04f
        };
        public float InlineCodePadding { get; set; } = 4f;

        public TypographyStyle CodeBlock { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 8f,
            FontWeight = FontWeight.Regular,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(44, 62, 80), // Navy
            LineHeight = 1.05f
        };
        public float CodeBlockBorderWidth { get; set; } = 2f;
        public float CodeBlockPadding { get; set; } = 10f;

        public TypographyStyle UnorderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(127, 255, 212), // Mint
            LineHeight = 1.07f
        };

        public TypographyStyle OrderedList { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.07f
        };

        public float ListItemSpacing { get; set; } = 6f;
        public float ListIndentation { get; set; } = 16f;

        public TypographyStyle Link { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Underline,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LineHeight = 1.07f
        };
        public bool LinkIsUnderline { get; set; } = true;

        public TypographyStyle SmallText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(206, 183, 255), // Pastel Lavender
            LineHeight = 1.02f
        };

        public TypographyStyle StrongText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Bold,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.05f
        };

        public TypographyStyle EmphasisText { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LineHeight = 1.04f
        };

        // Display, headline, title, body, and label text
        public TypographyStyle DisplayLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 44f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.2f
        };
        public TypographyStyle DisplayMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 34f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LineHeight = 1.18f
        };
        public TypographyStyle DisplaySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 26f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(127, 255, 212), // Mint
            LineHeight = 1.16f
        };

        public TypographyStyle HeadlineLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 22f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.12f
        };
        public TypographyStyle HeadlineMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LineHeight = 1.11f
        };
        public TypographyStyle HeadlineSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(127, 255, 212), // Mint
            LineHeight = 1.10f
        };

        public TypographyStyle TitleLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(255, 223, 93), // Lemon
            LineHeight = 1.10f
        };
        public TypographyStyle TitleMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LineHeight = 1.09f
        };
        public TypographyStyle TitleSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.08f
        };

        public TypographyStyle BodyLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(44, 62, 80), // Navy
            LineHeight = 1.07f
        };
        public TypographyStyle BodyMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(44, 62, 80), // Navy
            LineHeight = 1.06f
        };
        public TypographyStyle BodySmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(206, 183, 255), // Pastel Lavender
            LineHeight = 1.04f
        };

        public TypographyStyle LabelLarge { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(127, 255, 212), // Mint
            LineHeight = 1.07f
        };
        public TypographyStyle LabelMedium { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LineHeight = 1.06f
        };
        public TypographyStyle LabelSmall { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LineHeight = 1.04f
        };
    }
}
