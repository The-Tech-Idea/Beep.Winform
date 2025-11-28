using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = BackgroundColor;
            this.CompanyTitleColor = ForeColor;
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = ForeColor;
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = ForeColor;
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = PrimaryColor;
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = BackgroundColor;
            this.CompanyButtonTextColor = ForeColor;
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = BackgroundColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = BackgroundColor;
        }
    }
}