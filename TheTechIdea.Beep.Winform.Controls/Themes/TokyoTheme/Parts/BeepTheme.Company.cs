using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(26,27,38);
            this.CompanyTitleColor = Color.FromArgb(26,27,38);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = Color.FromArgb(26,27,38);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = Color.FromArgb(26,27,38);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = Color.FromArgb(26,27,38);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(26,27,38);
            this.CompanyButtonTextColor = Color.FromArgb(192,202,245);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(26,27,38);
            this.CompanyDropdownTextColor = Color.FromArgb(192,202,245);
            this.CompanyLogoBackgroundColor = Color.FromArgb(26,27,38);
        }
    }
}