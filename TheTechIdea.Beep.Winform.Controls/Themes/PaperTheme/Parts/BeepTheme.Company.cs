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
            this.CompanyPopoverBackgroundColor = BackgroundColor;
            this.CompanyTitleColor = BackgroundColor;
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanySubtitleColor = BackgroundColor;
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyDescriptionColor = BackgroundColor;
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyLinkColor = BackgroundColor;
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyButtonBackgroundColor = BackgroundColor;
            this.CompanyButtonTextColor = ForeColor;
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyDropdownBackgroundColor = BackgroundColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = BackgroundColor;
        }
    }
}