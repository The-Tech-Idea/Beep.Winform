﻿using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Stats Card Fonts & Colors
<<<<<<< HEAD
        public Font StatsTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font StatsSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font StatsUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color StatsCardForeColor { get; set; } = Color.White;
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33);

        public Color StatsCardTitleForeColor { get; set; } = Color.White;
        public Color StatsCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color StatsCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.LightGray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color StatsCardValueForeColor { get; set; } = Color.White;
        public Color StatsCardValueBackColor { get; set; } = Color.Transparent;
        public Color StatsCardValueBorderColor { get; set; } = Color.White;
        public Color StatsCardValueHoverForeColor { get; set; } = Color.Black;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Light blue
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.White;
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.Gray;
        public Color StatsCardInfoBackColor { get; set; } = Color.Transparent;
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33);
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Light,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Gray,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // Greenish
        public Color StatsCardTrendBackColor { get; set; } = Color.Transparent;
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C);
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(0x23, 0xB9, 0x5C),
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };
=======
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
