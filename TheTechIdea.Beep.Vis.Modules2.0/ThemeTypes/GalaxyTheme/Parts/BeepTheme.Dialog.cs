﻿using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Dialog Button Colors and Fonts
<<<<<<< HEAD
        public Color DialogBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color DialogForeColor { get; set; } = Color.White;
=======
        public Color DialogBackColor { get; set; }
        public Color DialogForeColor { get; set; }
        public TypographyStyle DialogYesButtonFont { get; set; }
        public TypographyStyle DialogNoButtonFont { get; set; }
        public TypographyStyle DialogOkButtonFont { get; set; }
        public TypographyStyle DialogCancelButtonFont { get; set; }
        public TypographyStyle DialogWarningButtonFont { get; set; }
        public TypographyStyle DialogErrorButtonFont { get; set; }
        public TypographyStyle DialogInformationButtonFont { get; set; }
        public TypographyStyle DialogQuestionButtonFont { get; set; }
        public TypographyStyle DialogHelpButtonFont { get; set; }
        public TypographyStyle DialogCloseButtonFont { get; set; }
        public TypographyStyle DialogYesButtonHoverFont { get; set; }
        public TypographyStyle DialogNoButtonHoverFont { get; set; }
        public TypographyStyle DialogOkButtonHoverFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        public Font DialogYesButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DialogNoButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogOkButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogCancelButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogWarningButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DialogErrorButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DialogInformationButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogQuestionButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);
        public Font DialogHelpButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogCloseButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogYesButtonHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold | FontStyle.Underline);
        public Font DialogNoButtonHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Underline);
        public Font DialogOkButtonHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Underline);

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // Success
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(0x2D, 0xCC, 0x70);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(0x55, 0x55, 0x55);
        public Color DialogCancelButtonForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(0x66, 0x66, 0x66);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(0x80, 0x00, 0x00);
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(0xA0, 0x00, 0x00);
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Light blue

        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(0xF2, 0xA6, 0x00); // Warning
        public Color DialogNoButtonForeColor { get; set; } = Color.Black;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(0xFF, 0xB3, 0x20);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.Black;

        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x5A, 0x9C);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(0xF2, 0xA6, 0x00);
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(0xFF, 0xC1, 0x33);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.Black;

        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(0xFF, 0x45, 0x60); // ErrorColor
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(0xFF, 0x66, 0x7A);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Light blue
        public Color DialogInformationButtonForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(0x6E, 0xD8, 0xFF);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.Black;

        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(0x6A, 0x5A, 0xCD); // Medium slate blue
        public Color DialogQuestionButtonForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(0x7B, 0x6D, 0xE0);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.White;
    }
}
