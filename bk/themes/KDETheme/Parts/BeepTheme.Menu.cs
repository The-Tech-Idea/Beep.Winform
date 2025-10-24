using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(248,249,250);
            this.MenuForeColor = Color.FromArgb(33,37,41);
            this.MenuBorderColor = Color.FromArgb(222,226,230);
            this.MenuMainItemForeColor = Color.FromArgb(33,37,41);
            this.MenuMainItemHoverForeColor = Color.FromArgb(33,37,41);
            this.MenuMainItemHoverBackColor = Color.FromArgb(248,249,250);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(248,249,250);
            this.MenuItemForeColor = Color.FromArgb(33,37,41);
            this.MenuItemHoverForeColor = Color.FromArgb(33,37,41);
            this.MenuItemHoverBackColor = Color.FromArgb(248,249,250);
            this.MenuItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.MenuItemSelectedBackColor = Color.FromArgb(248,249,250);
            this.MenuGradiantStartColor = Color.FromArgb(248,249,250);
            this.MenuGradiantEndColor = Color.FromArgb(248,249,250);
            this.MenuGradiantMiddleColor = Color.FromArgb(248,249,250);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}