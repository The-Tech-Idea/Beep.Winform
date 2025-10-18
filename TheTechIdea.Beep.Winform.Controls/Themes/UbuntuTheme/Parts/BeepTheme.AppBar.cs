using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = Color.White;
            this.AppBarForeColor = Color.FromArgb(44,44,44);
            this.AppBarButtonForeColor = Color.FromArgb(44,44,44);
            this.AppBarButtonBackColor = Color.White;
            this.AppBarTextBoxBackColor = Color.White;
            this.AppBarTextBoxForeColor = Color.FromArgb(44,44,44);
            this.AppBarLabelForeColor = Color.FromArgb(44,44,44);
            this.AppBarLabelBackColor = Color.White;
            this.AppBarTitleForeColor = Color.FromArgb(44,44,44);
            this.AppBarTitleBackColor = Color.White;
            this.AppBarSubTitleForeColor = Color.FromArgb(44,44,44);
            this.AppBarSubTitleBackColor = Color.White;
            this.AppBarCloseButtonColor = Color.FromArgb(242,242,245);
            this.AppBarMaxButtonColor = Color.FromArgb(242,242,245);
            this.AppBarMinButtonColor = Color.FromArgb(242,242,245);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(242,242,245);
            this.AppBarGradiantEndColor = Color.FromArgb(242,242,245);
            this.AppBarGradiantMiddleColor = Color.FromArgb(242,242,245);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}