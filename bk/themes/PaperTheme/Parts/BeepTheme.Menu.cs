using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(250,250,250);
            this.MenuForeColor = Color.FromArgb(33,33,33);
            this.MenuBorderColor = Color.FromArgb(224,224,224);
            this.MenuMainItemForeColor = Color.FromArgb(33,33,33);
            this.MenuMainItemHoverForeColor = Color.FromArgb(33,33,33);
            this.MenuMainItemHoverBackColor = Color.FromArgb(250,250,250);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(33,33,33);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(250,250,250);
            this.MenuItemForeColor = Color.FromArgb(33,33,33);
            this.MenuItemHoverForeColor = Color.FromArgb(33,33,33);
            this.MenuItemHoverBackColor = Color.FromArgb(250,250,250);
            this.MenuItemSelectedForeColor = Color.FromArgb(33,33,33);
            this.MenuItemSelectedBackColor = Color.FromArgb(250,250,250);
            this.MenuGradiantStartColor = Color.FromArgb(250,250,250);
            this.MenuGradiantEndColor = Color.FromArgb(250,250,250);
            this.MenuGradiantMiddleColor = Color.FromArgb(250,250,250);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}