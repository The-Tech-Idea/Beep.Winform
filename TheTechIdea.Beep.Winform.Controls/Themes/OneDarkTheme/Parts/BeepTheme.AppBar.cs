using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyAppBar()
        {
            // One Dark AppBar - popular dark theme
            this.AppBarBackColor = Color.FromArgb(40, 44, 52);  // Dark background
            this.AppBarForeColor = Color.FromArgb(171, 178, 191);  // Warm grey text
            this.AppBarButtonForeColor = Color.FromArgb(97, 175, 239);  // One Dark blue buttons
            this.AppBarButtonBackColor = Color.FromArgb(40, 44, 52);
            this.AppBarTextBoxBackColor = Color.FromArgb(33, 37, 43);
            this.AppBarTextBoxForeColor = Color.FromArgb(171, 178, 191);
            this.AppBarLabelForeColor = Color.FromArgb(171, 178, 191);
            this.AppBarLabelBackColor = Color.FromArgb(40, 44, 52);
            this.AppBarTitleForeColor = Color.FromArgb(171, 178, 191);
            this.AppBarTitleBackColor = Color.FromArgb(40, 44, 52);
            this.AppBarSubTitleForeColor = Color.FromArgb(92, 99, 112);  // #5C6370 dimmed grey
            this.AppBarSubTitleBackColor = Color.FromArgb(40, 44, 52);
            
            // System buttons - One Dark blue
            this.AppBarCloseButtonColor = Color.FromArgb(97, 175, 239);  // One Dark blue
            this.AppBarMaxButtonColor = Color.FromArgb(97, 175, 239);
            this.AppBarMinButtonColor = Color.FromArgb(97, 175, 239);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Fira Code", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Fira Code", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Fira Code", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = Color.FromArgb(40, 44, 52);
            this.AppBarGradiantEndColor = Color.FromArgb(40, 44, 52);
            this.AppBarGradiantMiddleColor = Color.FromArgb(40, 44, 52);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}