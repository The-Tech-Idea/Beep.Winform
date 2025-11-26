using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyAppBar()
        {
            // GNOME AppBar - clean light theme
            this.AppBarBackColor = SurfaceColor;  // Light gray (#F5F5F5)
            this.AppBarForeColor = ForeColor;  // Dark text
            this.AppBarButtonForeColor = PrimaryColor;  // Dark gray/primary buttons
            this.AppBarButtonBackColor = SurfaceColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // White text boxes
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = SurfaceColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = SurfaceColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(120, 120, 120);  // Medium gray
            this.AppBarSubTitleBackColor = SurfaceColor;
            
            // System buttons - dark for GNOME
            this.AppBarCloseButtonColor = Color.FromArgb(50, 50, 50);  // Dark gray
            this.AppBarMaxButtonColor = Color.FromArgb(50, 50, 50);
            this.AppBarMinButtonColor = Color.FromArgb(50, 50, 50);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Cantarell", FontSize = 13.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Subtle gradient
            this.AppBarGradiantStartColor = Color.FromArgb(245, 245, 245);
            this.AppBarGradiantEndColor = Color.FromArgb(239, 240, 241);
            this.AppBarGradiantMiddleColor = Color.FromArgb(242, 243, 244);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
