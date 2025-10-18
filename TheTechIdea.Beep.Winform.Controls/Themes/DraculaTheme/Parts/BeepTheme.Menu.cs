using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(40,42,54);
            this.MenuForeColor = Color.FromArgb(248,248,242);
            this.MenuBorderColor = Color.FromArgb(98,114,164);
            this.MenuMainItemForeColor = Color.FromArgb(248,248,242);
            this.MenuMainItemHoverForeColor = Color.FromArgb(248,248,242);
            this.MenuMainItemHoverBackColor = Color.FromArgb(40,42,54);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(248,248,242);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(40,42,54);
            this.MenuItemForeColor = Color.FromArgb(248,248,242);
            this.MenuItemHoverForeColor = Color.FromArgb(248,248,242);
            this.MenuItemHoverBackColor = Color.FromArgb(40,42,54);
            this.MenuItemSelectedForeColor = Color.FromArgb(248,248,242);
            this.MenuItemSelectedBackColor = Color.FromArgb(40,42,54);
            this.MenuGradiantStartColor = Color.FromArgb(40,42,54);
            this.MenuGradiantEndColor = Color.FromArgb(40,42,54);
            this.MenuGradiantMiddleColor = Color.FromArgb(40,42,54);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}