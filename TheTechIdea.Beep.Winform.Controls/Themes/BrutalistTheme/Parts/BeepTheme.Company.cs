using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(250,250,250);
            this.CompanyTitleColor = Color.FromArgb(250,250,250);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanySubtitleColor = Color.FromArgb(250,250,250);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyDescriptionColor = Color.FromArgb(250,250,250);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyLinkColor = Color.FromArgb(250,250,250);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(250,250,250);
            this.CompanyButtonTextColor = Color.FromArgb(20,20,20);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(250,250,250);
            this.CompanyDropdownTextColor = Color.FromArgb(20,20,20);
            this.CompanyLogoBackgroundColor = Color.FromArgb(250,250,250);
        }
    }
}