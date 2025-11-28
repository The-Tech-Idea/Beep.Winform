using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyAppBar()
        {
            // KDE AppBar - clean Linux design
            this.AppBarBackColor = BackgroundColor;  // Light gray
            this.AppBarForeColor = ForeColor;  // Dark gray text
            this.AppBarButtonForeColor = PrimaryColor;  // KDE blue buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // White text boxes
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = ThemeUtil.Lighten(ForeColor, 0.6);  // Medium gray
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // System buttons - dark colors
            this.AppBarCloseButtonColor = ForeColor;
            this.AppBarMaxButtonColor = ForeColor;
            this.AppBarMinButtonColor = ForeColor;
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = PanelGradiantStartColor;
            this.AppBarGradiantEndColor = PanelGradiantEndColor;
            this.AppBarGradiantMiddleColor = PanelGradiantMiddleColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
