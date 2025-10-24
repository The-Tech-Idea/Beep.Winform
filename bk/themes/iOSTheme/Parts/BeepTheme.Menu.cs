using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(242,242,247);
            this.MenuForeColor = Color.FromArgb(28,28,30);
            this.MenuBorderColor = Color.FromArgb(198,198,207);
            this.MenuMainItemForeColor = Color.FromArgb(28,28,30);
            this.MenuMainItemHoverForeColor = Color.FromArgb(28,28,30);
            this.MenuMainItemHoverBackColor = Color.FromArgb(242,242,247);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(28,28,30);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(242,242,247);
            this.MenuItemForeColor = Color.FromArgb(28,28,30);
            this.MenuItemHoverForeColor = Color.FromArgb(28,28,30);
            this.MenuItemHoverBackColor = Color.FromArgb(242,242,247);
            this.MenuItemSelectedForeColor = Color.FromArgb(28,28,30);
            this.MenuItemSelectedBackColor = Color.FromArgb(242,242,247);
            this.MenuGradiantStartColor = Color.FromArgb(242,242,247);
            this.MenuGradiantEndColor = Color.FromArgb(242,242,247);
            this.MenuGradiantMiddleColor = Color.FromArgb(242,242,247);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}