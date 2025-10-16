using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = BackgroundColor;
            this.AppBarForeColor = Color.FromArgb(28,28,30);
            this.AppBarButtonForeColor = Color.FromArgb(28,28,30);
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = BackgroundColor;
            this.AppBarTextBoxForeColor = Color.FromArgb(28,28,30);
            this.AppBarLabelForeColor = Color.FromArgb(28,28,30);
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = Color.FromArgb(28,28,30);
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(28,28,30);
            this.AppBarSubTitleBackColor = BackgroundColor;
            this.AppBarCloseButtonColor = Color.FromArgb(250,250,252);
            this.AppBarMaxButtonColor = Color.FromArgb(250,250,252);
            this.AppBarMinButtonColor = Color.FromArgb(250,250,252);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarGradiantStartColor = Color.FromArgb(250,250,252);
            this.AppBarGradiantEndColor = Color.FromArgb(250,250,252);
            this.AppBarGradiantMiddleColor = Color.FromArgb(250,250,252);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}