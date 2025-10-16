using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(236,240,243);
            this.MenuForeColor = Color.FromArgb(58,66,86);
            this.MenuBorderColor = Color.FromArgb(221,228,235);
            this.MenuMainItemForeColor = Color.FromArgb(58,66,86);
            this.MenuMainItemHoverForeColor = Color.FromArgb(58,66,86);
            this.MenuMainItemHoverBackColor = Color.FromArgb(236,240,243);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(58,66,86);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(236,240,243);
            this.MenuItemForeColor = Color.FromArgb(58,66,86);
            this.MenuItemHoverForeColor = Color.FromArgb(58,66,86);
            this.MenuItemHoverBackColor = Color.FromArgb(236,240,243);
            this.MenuItemSelectedForeColor = Color.FromArgb(58,66,86);
            this.MenuItemSelectedBackColor = Color.FromArgb(236,240,243);
            this.MenuGradiantStartColor = Color.FromArgb(236,240,243);
            this.MenuGradiantEndColor = Color.FromArgb(236,240,243);
            this.MenuGradiantMiddleColor = Color.FromArgb(236,240,243);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}