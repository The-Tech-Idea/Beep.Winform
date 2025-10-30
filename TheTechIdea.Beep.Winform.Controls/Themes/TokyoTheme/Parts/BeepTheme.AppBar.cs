using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyAppBar()
        {
            // Tokyo Night AppBar - inspired by Tokyo Night VSCode theme
            this.AppBarBackColor = Color.FromArgb(36, 40, 59);  // #24283B caption
            this.AppBarForeColor = Color.FromArgb(169, 177, 214);  // Light purple-blue text
            this.AppBarButtonForeColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan buttons
            this.AppBarButtonBackColor = Color.FromArgb(36, 40, 59);
            this.AppBarTextBoxBackColor = Color.FromArgb(26, 27, 38);
            this.AppBarTextBoxForeColor = Color.FromArgb(169, 177, 214);
            this.AppBarLabelForeColor = Color.FromArgb(169, 177, 214);
            this.AppBarLabelBackColor = Color.FromArgb(36, 40, 59);
            this.AppBarTitleForeColor = Color.FromArgb(169, 177, 214);
            this.AppBarTitleBackColor = Color.FromArgb(36, 40, 59);
            this.AppBarSubTitleForeColor = Color.FromArgb(65, 72, 104);  // #414868 dimmed purple
            this.AppBarSubTitleBackColor = Color.FromArgb(36, 40, 59);
            
            // System buttons - Tokyo cyan
            this.AppBarCloseButtonColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan
            this.AppBarMaxButtonColor = Color.FromArgb(122, 162, 247);
            this.AppBarMinButtonColor = Color.FromArgb(122, 162, 247);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.16f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = Color.FromArgb(36, 40, 59);
            this.AppBarGradiantEndColor = Color.FromArgb(36, 40, 59);
            this.AppBarGradiantMiddleColor = Color.FromArgb(26, 27, 38);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}