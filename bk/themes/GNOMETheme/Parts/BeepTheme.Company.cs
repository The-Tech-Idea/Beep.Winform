using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(246,245,244);
            this.CompanyTitleColor = Color.FromArgb(246,245,244);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = Color.FromArgb(246,245,244);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = Color.FromArgb(246,245,244);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = Color.FromArgb(246,245,244);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(246,245,244);
            this.CompanyButtonTextColor = Color.FromArgb(46,52,54);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(246,245,244);
            this.CompanyDropdownTextColor = Color.FromArgb(46,52,54);
            this.CompanyLogoBackgroundColor = Color.FromArgb(246,245,244);
        }
    }
}