using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
<<<<<<< HEAD
        // Stats Card Fonts & Colors with Material Design defaults
        public Font StatsTitleFont { get; set; } = new Font("Roboto", 16f, FontStyle.Bold);
        public Font StatsSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);
        public Font StatsUnSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.White;
        public Color StatsCardForeColor { get; set; } = Color.Black;
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color StatsCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color StatsCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117)
        };

        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color StatsCardValueBackColor { get; set; } = Color.Transparent;
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400

        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(55, 71, 79); // Blue Grey 800
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(238, 238, 238); // Grey 200
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(158, 158, 158); // Grey 500
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 20f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color StatsCardInfoBackColor { get; set; } = Color.Transparent;
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117)
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(76, 175, 80); // Green 500 (positive trend)
        public Color StatsCardTrendBackColor { get; set; } = Color.Transparent;
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(56, 142, 60); // Green 700
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(76, 175, 80)
        };
=======
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; }
        public TypographyStyle StatsSelectedFont { get; set; }
        public TypographyStyle StatsUnSelectedFont { get; set; }
        public Color StatsCardBackColor { get; set; }
        public Color StatsCardForeColor { get; set; }
        public Color StatsCardBorderColor { get; set; }
        public Color StatsCardTitleForeColor { get; set; }
        public Color StatsCardTitleBackColor { get; set; }
        public TypographyStyle StatsCardTitleStyle { get; set; }
        public Color StatsCardSubTitleForeColor { get; set; }
        public Color StatsCardSubTitleBackColor { get; set; }
        public TypographyStyle StatsCardSubStyleStyle { get; set; }
        public Color StatsCardValueForeColor { get; set; }
        public Color StatsCardValueBackColor { get; set; }
        public Color StatsCardValueBorderColor { get; set; }
        public Color StatsCardValueHoverForeColor { get; set; }
        public Color StatsCardValueHoverBackColor { get; set; }
        public Color StatsCardValueHoverBorderColor { get; set; }
        public TypographyStyle StatsCardValueStyle { get; set; }
        public Color StatsCardInfoForeColor { get; set; }
        public Color StatsCardInfoBackColor { get; set; }
        public Color StatsCardInfoBorderColor { get; set; }
        public TypographyStyle StatsCardInfoStyle { get; set; }
        public Color StatsCardTrendForeColor { get; set; }
        public Color StatsCardTrendBackColor { get; set; }
        public Color StatsCardTrendBorderColor { get; set; }
        public TypographyStyle StatsCardTrendStyle { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
