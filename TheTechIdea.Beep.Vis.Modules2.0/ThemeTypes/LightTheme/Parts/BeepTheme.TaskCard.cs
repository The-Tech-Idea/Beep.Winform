﻿using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Task Card Fonts & Colors
        public Font TaskCardTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font TaskCardSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font TaskCardUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.White;
        public Color TaskCardForeColor { get; set; } = Color.Black;
        public Color TaskCardBorderColor { get; set; } = Color.LightGray;

        public Color TaskCardTitleForeColor { get; set; } = Color.Black;
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.Black;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.Transparent;
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.LightGray;

        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.DodgerBlue;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.LightCyan;
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.DodgerBlue;

        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            TextColor = Color.Black
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.DodgerBlue;
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.RoyalBlue;

        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
    }
}
