using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(15,16,32);
            this.CompanyTitleColor = Color.FromArgb(15,16,32);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanySubtitleColor = Color.FromArgb(15,16,32);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyDescriptionColor = Color.FromArgb(15,16,32);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyLinkColor = Color.FromArgb(15,16,32);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(15,16,32);
            this.CompanyButtonTextColor = Color.FromArgb(245,247,255);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(15,16,32);
            this.CompanyDropdownTextColor = Color.FromArgb(245,247,255);
            this.CompanyLogoBackgroundColor = Color.FromArgb(15,16,32);
        }
    }
}