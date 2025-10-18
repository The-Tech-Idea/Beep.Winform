using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(245,246,247);
            this.CompanyTitleColor = Color.FromArgb(245,246,247);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanySubtitleColor = Color.FromArgb(245,246,247);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDescriptionColor = Color.FromArgb(245,246,247);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyLinkColor = Color.FromArgb(245,246,247);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(245,246,247);
            this.CompanyButtonTextColor = Color.FromArgb(43,45,48);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(245,246,247);
            this.CompanyDropdownTextColor = Color.FromArgb(43,45,48);
            this.CompanyLogoBackgroundColor = Color.FromArgb(245,246,247);
        }
    }
}