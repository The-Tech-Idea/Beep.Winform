using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(242,242,245);
            this.CompanyTitleColor = Color.FromArgb(242,242,245);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = Color.FromArgb(242,242,245);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = Color.FromArgb(242,242,245);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = Color.FromArgb(242,242,245);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(242,242,245);
            this.CompanyButtonTextColor = Color.FromArgb(44,44,44);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(242,242,245);
            this.CompanyDropdownTextColor = Color.FromArgb(44,44,44);
            this.CompanyLogoBackgroundColor = Color.FromArgb(242,242,245);
        }
    }
}