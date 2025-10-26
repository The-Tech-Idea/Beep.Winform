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
            this.CompanyPopoverBackgroundColor = SurfaceColor;
            this.CompanyTitleColor = ForeColor;
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanySubtitleColor = ThemeUtil.Lighten(ForeColor, 0.12);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = CompanySubtitleColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = ForeColor;
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 11.5f, FontWeight = FontWeight.Light, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.6f };
            this.CompanyLinkColor = AccentColor;
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = CompanyLinkColor, LineHeight = 1.55f };
            this.CompanyButtonBackgroundColor = PrimaryColor;
            this.CompanyButtonTextColor = OnPrimaryColor;
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = CompanyButtonTextColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = SurfaceColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = SurfaceColor;
        }
    }
}
