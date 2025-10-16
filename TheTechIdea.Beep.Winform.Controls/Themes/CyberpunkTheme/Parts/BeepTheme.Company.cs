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
            this.CompanyPopoverBackgroundColor = Color.FromArgb(10,8,20);
            this.CompanyTitleColor = Color.FromArgb(10,8,20);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanySubtitleColor = Color.FromArgb(10,8,20);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyDescriptionColor = Color.FromArgb(10,8,20);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyLinkColor = Color.FromArgb(10,8,20);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(10,8,20);
            this.CompanyButtonTextColor = Color.FromArgb(228,244,255);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(10,8,20);
            this.CompanyDropdownTextColor = Color.FromArgb(228,244,255);
            this.CompanyLogoBackgroundColor = Color.FromArgb(10,8,20);
        }
    }
}