using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyCompany()
        {
            this.CompanyPopoverBackgroundColor = Color.FromArgb(245,248,255);
            this.CompanyTitleColor = Color.FromArgb(245,248,255);
            this.CompanyTitleFont = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanySubtitleColor = Color.FromArgb(245,248,255);
            this.CompanySubTitleFont = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDescriptionColor = Color.FromArgb(245,248,255);
            this.CompanyDescriptionFont = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyLinkColor = Color.FromArgb(245,248,255);
            this.CompanyLinkFont = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyButtonBackgroundColor = Color.FromArgb(245,248,255);
            this.CompanyButtonTextColor = Color.FromArgb(24,28,35);
            this.CompanyButtonFont = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.CompanyDropdownBackgroundColor = Color.FromArgb(245,248,255);
            this.CompanyDropdownTextColor = Color.FromArgb(24,28,35);
            this.CompanyLogoBackgroundColor = Color.FromArgb(245,248,255);
        }
    }
}