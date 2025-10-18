using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(46,52,64);
            this.CompanyTitleColor = Color.FromArgb(46,52,64);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = Color.FromArgb(46,52,64);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = Color.FromArgb(46,52,64);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = Color.FromArgb(46,52,64);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(46,52,64);
            this.CompanyButtonTextColor = Color.FromArgb(216,222,233);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(46,52,64);
            this.CompanyDropdownTextColor = Color.FromArgb(216,222,233);
            this.CompanyLogoBackgroundColor = Color.FromArgb(46,52,64);
        }
    }
}