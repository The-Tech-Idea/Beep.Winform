using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(250,250,250);
            this.MenuForeColor = Color.FromArgb(20,20,20);
            this.MenuBorderColor = Color.FromArgb(0,0,0);
            this.MenuMainItemForeColor = Color.FromArgb(20,20,20);
            this.MenuMainItemHoverForeColor = Color.FromArgb(20,20,20);
            this.MenuMainItemHoverBackColor = Color.FromArgb(250,250,250);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(20,20,20);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(250,250,250);
            this.MenuItemForeColor = Color.FromArgb(20,20,20);
            this.MenuItemHoverForeColor = Color.FromArgb(20,20,20);
            this.MenuItemHoverBackColor = Color.FromArgb(250,250,250);
            this.MenuItemSelectedForeColor = Color.FromArgb(20,20,20);
            this.MenuItemSelectedBackColor = Color.FromArgb(250,250,250);
            this.MenuGradiantStartColor = Color.FromArgb(250,250,250);
            this.MenuGradiantEndColor = Color.FromArgb(250,250,250);
            this.MenuGradiantMiddleColor = Color.FromArgb(250,250,250);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }
    }
}