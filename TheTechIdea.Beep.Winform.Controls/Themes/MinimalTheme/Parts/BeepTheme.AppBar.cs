using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = BackgroundColor;
            this.AppBarForeColor = Color.FromArgb(31,41,55);
            this.AppBarButtonForeColor = Color.FromArgb(31,41,55);
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = BackgroundColor;
            this.AppBarTextBoxForeColor = Color.FromArgb(31,41,55);
            this.AppBarLabelForeColor = Color.FromArgb(31,41,55);
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = Color.FromArgb(31,41,55);
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(31,41,55);
            this.AppBarSubTitleBackColor = BackgroundColor;
            this.AppBarCloseButtonColor = Color.FromArgb(255,255,255);
            this.AppBarMaxButtonColor = Color.FromArgb(255,255,255);
            this.AppBarMinButtonColor = Color.FromArgb(255,255,255);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarGradiantStartColor = Color.FromArgb(255,255,255);
            this.AppBarGradiantEndColor = Color.FromArgb(255,255,255);
            this.AppBarGradiantMiddleColor = Color.FromArgb(255,255,255);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}