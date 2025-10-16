using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(236,240,243);
            this.CompanyTitleColor = Color.FromArgb(236,240,243);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanySubtitleColor = Color.FromArgb(236,240,243);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDescriptionColor = Color.FromArgb(236,240,243);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyLinkColor = Color.FromArgb(236,240,243);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(236,240,243);
            this.CompanyButtonTextColor = Color.FromArgb(58,66,86);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(236,240,243);
            this.CompanyDropdownTextColor = Color.FromArgb(58,66,86);
            this.CompanyLogoBackgroundColor = Color.FromArgb(236,240,243);
        }
    }
}