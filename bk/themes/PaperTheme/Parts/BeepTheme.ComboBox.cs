using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(250,250,250);
            this.ComboBoxForeColor = Color.FromArgb(33,33,33);
            this.ComboBoxBorderColor = Color.FromArgb(224,224,224);
            this.ComboBoxHoverBackColor = Color.FromArgb(250,250,250);
            this.ComboBoxHoverForeColor = Color.FromArgb(33,33,33);
            this.ComboBoxHoverBorderColor = Color.FromArgb(224,224,224);
            this.ComboBoxSelectedBackColor = Color.FromArgb(250,250,250);
            this.ComboBoxSelectedForeColor = Color.FromArgb(33,33,33);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(224,224,224);
            this.ComboBoxErrorBackColor = Color.FromArgb(250,250,250);
            this.ComboBoxErrorForeColor = Color.FromArgb(33,33,33);
        }
    }
}