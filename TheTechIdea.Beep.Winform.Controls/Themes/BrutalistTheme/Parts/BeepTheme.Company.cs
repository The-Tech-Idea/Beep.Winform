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
            this.CompanyPopoverBackgroundColor = SurfaceColor;
            this.CompanyTitleColor = ForeColor;
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanySubtitleColor = ForeColor;
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyDescriptionColor = ForeColor;
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyLinkColor = AccentColor;
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AccentColor, LineHeight = 1.45f };
            this.CompanyButtonBackgroundColor = SurfaceColor;
            this.CompanyButtonTextColor = ForeColor;
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyDropdownBackgroundColor = SurfaceColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = SurfaceColor;
        }
    }
}