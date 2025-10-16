using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(250,250,250);
            this.TabForeColor = Color.FromArgb(20,20,20);
            this.TabBorderColor = Color.FromArgb(0,0,0);
            this.TabHoverBackColor = Color.FromArgb(250,250,250);
            this.TabHoverForeColor = Color.FromArgb(20,20,20);
            this.TabSelectedBackColor = Color.FromArgb(250,250,250);
            this.TabSelectedForeColor = Color.FromArgb(20,20,20);
            this.TabSelectedBorderColor = Color.FromArgb(0,0,0);
            this.TabHoverBorderColor = Color.FromArgb(0,0,0);
        }
    }
}