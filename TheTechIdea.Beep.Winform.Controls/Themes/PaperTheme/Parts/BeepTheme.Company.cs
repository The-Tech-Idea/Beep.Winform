using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(250,250,250);
            this.CompanyTitleColor = Color.FromArgb(250,250,250);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanySubtitleColor = Color.FromArgb(250,250,250);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyDescriptionColor = Color.FromArgb(250,250,250);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyLinkColor = Color.FromArgb(250,250,250);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(250,250,250);
            this.CompanyButtonTextColor = Color.FromArgb(33,33,33);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(250,250,250);
            this.CompanyDropdownTextColor = Color.FromArgb(33,33,33);
            this.CompanyLogoBackgroundColor = Color.FromArgb(250,250,250);
        }
    }
}