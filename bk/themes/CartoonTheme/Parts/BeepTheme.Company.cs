using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(255,251,235);
            this.CompanyTitleColor = Color.FromArgb(255,251,235);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = Color.FromArgb(255,251,235);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = Color.FromArgb(255,251,235);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = Color.FromArgb(255,251,235);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(255,251,235);
            this.CompanyButtonTextColor = Color.FromArgb(33,37,41);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(255,251,235);
            this.CompanyDropdownTextColor = Color.FromArgb(33,37,41);
            this.CompanyLogoBackgroundColor = Color.FromArgb(255,251,235);
        }
    }
}