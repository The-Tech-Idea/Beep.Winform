using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(250,250,250);
            this.ComboBoxForeColor = Color.FromArgb(20,20,20);
            this.ComboBoxBorderColor = Color.FromArgb(0,0,0);
            this.ComboBoxHoverBackColor = Color.FromArgb(250,250,250);
            this.ComboBoxHoverForeColor = Color.FromArgb(20,20,20);
            this.ComboBoxHoverBorderColor = Color.FromArgb(0,0,0);
            this.ComboBoxSelectedBackColor = Color.FromArgb(250,250,250);
            this.ComboBoxSelectedForeColor = Color.FromArgb(20,20,20);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(0,0,0);
            this.ComboBoxErrorBackColor = Color.FromArgb(220,0,0);
            this.ComboBoxErrorForeColor = Color.FromArgb(20,20,20);
        }
    }
}