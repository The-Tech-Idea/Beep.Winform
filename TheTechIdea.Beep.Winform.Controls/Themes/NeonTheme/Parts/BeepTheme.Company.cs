using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = PanelGradiantMiddleColor;
            this.CompanyTitleColor = PanelGradiantMiddleColor;
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = PanelGradiantMiddleColor;
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = PanelGradiantMiddleColor;
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = PanelGradiantMiddleColor;
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = PanelGradiantMiddleColor;
            this.CompanyButtonTextColor = ForeColor;
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = PanelGradiantMiddleColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = PanelGradiantMiddleColor;
        }
    }
}