using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyAppBar()
        {
            // ChatBubble AppBar - soft cyan background with black text
            this.AppBarBackColor = BackgroundColor;  // Light cyan
            this.AppBarForeColor = ForeColor;  // Black text
            this.AppBarButtonForeColor = ForeColor;
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // White text boxes
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = SecondaryColor;  // Dark gray
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // System buttons - standard colors
            this.AppBarCloseButtonColor = ForeColor;
            this.AppBarMaxButtonColor = ForeColor;
            this.AppBarMinButtonColor = ForeColor;
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Nunito", FontSize = 14f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Subtle gradient
            this.AppBarGradiantStartColor = PanelGradiantStartColor;
            this.AppBarGradiantEndColor = PanelGradiantEndColor;
            this.AppBarGradiantMiddleColor = PanelGradiantMiddleColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}