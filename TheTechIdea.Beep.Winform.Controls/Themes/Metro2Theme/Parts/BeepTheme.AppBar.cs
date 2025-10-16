using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = BackgroundColor;
            this.AppBarForeColor = Color.FromArgb(32,31,30);
            this.AppBarButtonForeColor = Color.FromArgb(32,31,30);
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = BackgroundColor;
            this.AppBarTextBoxForeColor = Color.FromArgb(32,31,30);
            this.AppBarLabelForeColor = Color.FromArgb(32,31,30);
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = Color.FromArgb(32,31,30);
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(32,31,30);
            this.AppBarSubTitleBackColor = BackgroundColor;
            this.AppBarCloseButtonColor = Color.FromArgb(243,242,241);
            this.AppBarMaxButtonColor = Color.FromArgb(243,242,241);
            this.AppBarMinButtonColor = Color.FromArgb(243,242,241);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(243,242,241);
            this.AppBarGradiantEndColor = Color.FromArgb(243,242,241);
            this.AppBarGradiantMiddleColor = Color.FromArgb(243,242,241);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}