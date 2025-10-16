using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(250,250,251);
            this.CompanyTitleColor = Color.FromArgb(250,250,251);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanySubtitleColor = Color.FromArgb(250,250,251);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDescriptionColor = Color.FromArgb(250,250,251);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyLinkColor = Color.FromArgb(250,250,251);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(250,250,251);
            this.CompanyButtonTextColor = Color.FromArgb(31,41,55);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(250,250,251);
            this.CompanyDropdownTextColor = Color.FromArgb(31,41,55);
            this.CompanyLogoBackgroundColor = Color.FromArgb(250,250,251);
        }
    }
}