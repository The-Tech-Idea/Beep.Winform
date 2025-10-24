using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(248,249,250);
            this.TabForeColor = Color.FromArgb(33,37,41);
            this.TabBorderColor = Color.FromArgb(222,226,230);
            this.TabHoverBackColor = Color.FromArgb(248,249,250);
            this.TabHoverForeColor = Color.FromArgb(33,37,41);
            this.TabSelectedBackColor = Color.FromArgb(248,249,250);
            this.TabSelectedForeColor = Color.FromArgb(33,37,41);
            this.TabSelectedBorderColor = Color.FromArgb(222,226,230);
            this.TabHoverBorderColor = Color.FromArgb(222,226,230);
        }
    }
}