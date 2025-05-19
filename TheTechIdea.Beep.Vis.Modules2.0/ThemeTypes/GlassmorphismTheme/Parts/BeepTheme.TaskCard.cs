using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Task Card Fonts & Colors
<<<<<<< HEAD
        public Font TaskCardTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font TaskCardSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font TaskCardUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.White;
        public Color TaskCardForeColor { get; set; } = Color.Black;
        public Color TaskCardBorderColor { get; set; } = Color.LightGray;

        public Color TaskCardTitleForeColor { get; set; } = Color.Black;
        public Color TaskCardTitleBackColor { get; set; } = Color.WhiteSmoke;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.DimGray;
        public Color TaskCardSubTitleBackColor { get; set; } = Color.WhiteSmoke;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.DimGray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.Black;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.White;
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.LightGray;

        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.Black;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.LightBlue;
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.SteelBlue;

        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.Black;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.LightGray;

        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };
=======
        public TypographyStyle TaskCardTitleFont { get; set; }
        public TypographyStyle TaskCardSelectedFont { get; set; }
        public TypographyStyle TaskCardUnSelectedFont { get; set; }
        public Color TaskCardBackColor { get; set; }
        public Color TaskCardForeColor { get; set; }
        public Color TaskCardBorderColor { get; set; }
        public Color TaskCardTitleForeColor { get; set; }
        public Color TaskCardTitleBackColor { get; set; }
        public TypographyStyle TaskCardTitleStyle { get; set; }
        public Color TaskCardSubTitleForeColor { get; set; }
        public Color TaskCardSubTitleBackColor { get; set; }
        public TypographyStyle TaskCardSubStyleStyle { get; set; }
        public Color TaskCardMetricTextForeColor { get; set; }
        public Color TaskCardMetricTextBackColor { get; set; }
        public Color TaskCardMetricTextBorderColor { get; set; }
        public Color TaskCardMetricTextHoverForeColor { get; set; }
        public Color TaskCardMetricTextHoverBackColor { get; set; }
        public Color TaskCardMetricTextHoverBorderColor { get; set; }
        public TypographyStyle TaskCardMetricTextStyle { get; set; }
        public Color TaskCardProgressValueForeColor { get; set; }
        public Color TaskCardProgressValueBackColor { get; set; }
        public Color TaskCardProgressValueBorderColor { get; set; }
        public TypographyStyle TaskCardProgressValueStyle { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
