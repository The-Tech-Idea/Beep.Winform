using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(245,248,255);
            this.ComboBoxForeColor = Color.FromArgb(24,28,35);
            this.ComboBoxBorderColor = Color.FromArgb(210,220,235);
            this.ComboBoxHoverBackColor = Color.FromArgb(245,248,255);
            this.ComboBoxHoverForeColor = Color.FromArgb(24,28,35);
            this.ComboBoxHoverBorderColor = Color.FromArgb(210,220,235);
            this.ComboBoxSelectedBackColor = Color.FromArgb(245,248,255);
            this.ComboBoxSelectedForeColor = Color.FromArgb(24,28,35);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(210,220,235);
            this.ComboBoxErrorBackColor = Color.FromArgb(245,80,100);
            this.ComboBoxErrorForeColor = Color.FromArgb(24,28,35);
        }
    }
}