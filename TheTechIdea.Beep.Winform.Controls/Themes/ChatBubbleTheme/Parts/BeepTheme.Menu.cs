using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(245,248,255);
            this.MenuForeColor = Color.FromArgb(24,28,35);
            this.MenuBorderColor = Color.FromArgb(210,220,235);
            this.MenuMainItemForeColor = Color.FromArgb(24,28,35);
            this.MenuMainItemHoverForeColor = Color.FromArgb(24,28,35);
            this.MenuMainItemHoverBackColor = Color.FromArgb(245,248,255);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(24,28,35);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(245,248,255);
            this.MenuItemForeColor = Color.FromArgb(24,28,35);
            this.MenuItemHoverForeColor = Color.FromArgb(24,28,35);
            this.MenuItemHoverBackColor = Color.FromArgb(245,248,255);
            this.MenuItemSelectedForeColor = Color.FromArgb(24,28,35);
            this.MenuItemSelectedBackColor = Color.FromArgb(245,248,255);
            this.MenuGradiantStartColor = Color.FromArgb(245,248,255);
            this.MenuGradiantEndColor = Color.FromArgb(245,248,255);
            this.MenuGradiantMiddleColor = Color.FromArgb(245,248,255);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}