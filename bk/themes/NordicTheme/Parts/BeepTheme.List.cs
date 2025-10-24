using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(250,250,251);
            this.ListForeColor = Color.FromArgb(31,41,55);
            this.ListBorderColor = Color.FromArgb(229,231,235);
            this.ListItemForeColor = Color.FromArgb(31,41,55);
            this.ListItemHoverForeColor = Color.FromArgb(31,41,55);
            this.ListItemHoverBackColor = Color.FromArgb(250,250,251);
            this.ListItemSelectedForeColor = Color.FromArgb(31,41,55);
            this.ListItemSelectedBackColor = Color.FromArgb(250,250,251);
            this.ListItemSelectedBorderColor = Color.FromArgb(229,231,235);
            this.ListItemBorderColor = Color.FromArgb(229,231,235);
            this.ListItemHoverBorderColor = Color.FromArgb(229,231,235);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}