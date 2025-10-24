using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(245,248,255);
            this.RadioButtonForeColor = Color.FromArgb(24,28,35);
            this.RadioButtonBorderColor = Color.FromArgb(210,220,235);
            this.RadioButtonCheckedBackColor = Color.FromArgb(245,248,255);
            this.RadioButtonCheckedForeColor = Color.FromArgb(24,28,35);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(210,220,235);
            this.RadioButtonHoverBackColor = Color.FromArgb(245,248,255);
            this.RadioButtonHoverForeColor = Color.FromArgb(24,28,35);
            this.RadioButtonHoverBorderColor = Color.FromArgb(210,220,235);
            this.RadioButtonSelectedForeColor = Color.FromArgb(24,28,35);
            this.RadioButtonSelectedBackColor = Color.FromArgb(245,248,255);
        }
    }
}