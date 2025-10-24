using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(40,40,40);
            this.CompanyTitleColor = Color.FromArgb(40,40,40);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = Color.FromArgb(40,40,40);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = Color.FromArgb(40,40,40);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = Color.FromArgb(40,40,40);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(40,40,40);
            this.CompanyButtonTextColor = Color.FromArgb(235,219,178);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(40,40,40);
            this.CompanyDropdownTextColor = Color.FromArgb(235,219,178);
            this.CompanyLogoBackgroundColor = Color.FromArgb(40,40,40);
        }
    }
}