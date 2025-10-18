using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(248,249,250);
            this.CompanyTitleColor = Color.FromArgb(248,249,250);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanySubtitleColor = Color.FromArgb(248,249,250);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDescriptionColor = Color.FromArgb(248,249,250);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyLinkColor = Color.FromArgb(248,249,250);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(248,249,250);
            this.CompanyButtonTextColor = Color.FromArgb(33,37,41);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(248,249,250);
            this.CompanyDropdownTextColor = Color.FromArgb(33,37,41);
            this.CompanyLogoBackgroundColor = Color.FromArgb(248,249,250);
        }
    }
}