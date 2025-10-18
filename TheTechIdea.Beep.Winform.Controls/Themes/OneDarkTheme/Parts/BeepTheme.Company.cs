using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(40,44,52);
            this.CompanyTitleColor = Color.FromArgb(40,44,52);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Fira Code", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = Color.FromArgb(40,44,52);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Fira Code", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = Color.FromArgb(40,44,52);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Fira Code", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = Color.FromArgb(40,44,52);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Fira Code", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(40,44,52);
            this.CompanyButtonTextColor = Color.FromArgb(171,178,191);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Fira Code", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(40,44,52);
            this.CompanyDropdownTextColor = Color.FromArgb(171,178,191);
            this.CompanyLogoBackgroundColor = Color.FromArgb(40,44,52);
        }
    }
}