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
            this.AppBarBackColor = Color.FromArgb(230, 250, 255);  // Light cyan
            this.AppBarForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.AppBarButtonForeColor = Color.FromArgb(0, 0, 0);
            this.AppBarButtonBackColor = Color.FromArgb(230, 250, 255);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(0, 0, 0);
            this.AppBarLabelForeColor = Color.FromArgb(0, 0, 0);
            this.AppBarLabelBackColor = Color.FromArgb(230, 250, 255);
            this.AppBarTitleForeColor = Color.FromArgb(0, 0, 0);
            this.AppBarTitleBackColor = Color.FromArgb(230, 250, 255);
            this.AppBarSubTitleForeColor = Color.FromArgb(50, 50, 50);  // Dark gray
            this.AppBarSubTitleBackColor = Color.FromArgb(230, 250, 255);
            
            // System buttons - standard colors
            this.AppBarCloseButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMaxButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMinButtonColor = Color.FromArgb(0, 0, 0);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Nunito", FontSize = 14f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Nunito", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Subtle gradient
            this.AppBarGradiantStartColor = Color.FromArgb(245, 248, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(220, 245, 250);
            this.AppBarGradiantMiddleColor = Color.FromArgb(230, 250, 255);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}