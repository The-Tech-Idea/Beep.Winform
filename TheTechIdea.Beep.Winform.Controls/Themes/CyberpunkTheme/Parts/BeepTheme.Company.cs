using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = BackgroundColor;
            this.CompanyTitleColor = ForeColor;
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanySubtitleColor = ForeColor;
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyDescriptionColor = InactiveBorderColor;
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyLinkColor = PrimaryColor;
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyButtonBackgroundColor = PrimaryColor;
            this.CompanyButtonTextColor = OnPrimaryColor;
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyDropdownBackgroundColor = PanelBackColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = PanelBackColor;
        }
    }
}