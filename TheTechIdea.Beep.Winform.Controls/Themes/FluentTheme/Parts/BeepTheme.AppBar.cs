using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = BackgroundColor;
            this.AppBarForeColor = Color.FromArgb(32,32,32);
            this.AppBarButtonForeColor = Color.FromArgb(32,32,32);
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = BackgroundColor;
            this.AppBarTextBoxForeColor = Color.FromArgb(32,32,32);
            this.AppBarLabelForeColor = Color.FromArgb(32,32,32);
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = Color.FromArgb(32,32,32);
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(32,32,32);
            this.AppBarSubTitleBackColor = BackgroundColor;
            this.AppBarCloseButtonColor = Color.FromArgb(245,246,248);
            this.AppBarMaxButtonColor = Color.FromArgb(245,246,248);
            this.AppBarMinButtonColor = Color.FromArgb(245,246,248);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarGradiantStartColor = Color.FromArgb(245,246,248);
            this.AppBarGradiantEndColor = Color.FromArgb(245,246,248);
            this.AppBarGradiantMiddleColor = Color.FromArgb(245,246,248);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}