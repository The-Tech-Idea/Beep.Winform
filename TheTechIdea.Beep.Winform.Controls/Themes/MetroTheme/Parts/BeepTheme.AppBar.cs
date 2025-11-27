using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyAppBar()
        {
            // Metro AppBar - Windows Metro design
            this.AppBarBackColor = PrimaryColor;
            this.AppBarForeColor = OnPrimaryColor;
            this.AppBarButtonForeColor = OnPrimaryColor;
            this.AppBarButtonBackColor = PrimaryColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = OnPrimaryColor;
            this.AppBarLabelBackColor = PrimaryColor;
            this.AppBarTitleForeColor = OnPrimaryColor;
            this.AppBarTitleBackColor = PrimaryColor;
            this.AppBarSubTitleForeColor = OnPrimaryColor;
            this.AppBarSubTitleBackColor = PrimaryColor;
            this.AppBarCloseButtonColor = OnPrimaryColor;
            this.AppBarMaxButtonColor = OnPrimaryColor;
            this.AppBarMinButtonColor = OnPrimaryColor;
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = OnPrimaryColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = OnPrimaryColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = PrimaryColor;
            this.AppBarGradiantEndColor = PrimaryColor;
            this.AppBarGradiantMiddleColor = PrimaryColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
