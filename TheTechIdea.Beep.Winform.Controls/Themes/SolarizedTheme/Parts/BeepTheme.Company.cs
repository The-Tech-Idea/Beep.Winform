using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(0,43,54);
            this.CompanyTitleColor = Color.FromArgb(0,43,54);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanySubtitleColor = Color.FromArgb(0,43,54);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDescriptionColor = Color.FromArgb(0,43,54);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyLinkColor = Color.FromArgb(0,43,54);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(0,43,54);
            this.CompanyButtonTextColor = Color.FromArgb(147,161,161);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(0,43,54);
            this.CompanyDropdownTextColor = Color.FromArgb(147,161,161);
            this.CompanyLogoBackgroundColor = Color.FromArgb(0,43,54);
        }
    }
}