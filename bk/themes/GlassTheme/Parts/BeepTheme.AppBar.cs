using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = Color.FromArgb(180, 255, 255, 255);
            this.AppBarForeColor = Color.FromArgb(17,24,39);
            this.AppBarButtonForeColor = Color.FromArgb(17,24,39);
            this.AppBarButtonBackColor = Color.FromArgb(180, 255, 255, 255);
            this.AppBarTextBoxBackColor = Color.FromArgb(180, 255, 255, 255);
            this.AppBarTextBoxForeColor = Color.FromArgb(17,24,39);
            this.AppBarLabelForeColor = Color.FromArgb(17,24,39);
            this.AppBarLabelBackColor = Color.FromArgb(180, 255, 255, 255);
            this.AppBarTitleForeColor = Color.FromArgb(17,24,39);
            this.AppBarTitleBackColor = Color.FromArgb(180, 255, 255, 255);
            this.AppBarSubTitleForeColor = Color.FromArgb(17,24,39);
            this.AppBarSubTitleBackColor = Color.FromArgb(180, 255, 255, 255);
            this.AppBarCloseButtonColor = Color.FromArgb(236,244,255);
            this.AppBarMaxButtonColor = Color.FromArgb(236,244,255);
            this.AppBarMinButtonColor = Color.FromArgb(236,244,255);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarSubTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(236,244,255);
            this.AppBarGradiantEndColor = Color.FromArgb(236,244,255);
            this.AppBarGradiantMiddleColor = Color.FromArgb(236,244,255);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}