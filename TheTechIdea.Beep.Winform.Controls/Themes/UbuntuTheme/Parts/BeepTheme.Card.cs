using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(44,44,44);
            this.CardBackColor = Color.FromArgb(242,242,245);
            this.CardTitleForeColor = Color.FromArgb(44,44,44);
            this.CardSubTitleForeColor = Color.FromArgb(44,44,44);
            this.CardrGradiantStartColor = Color.FromArgb(242,242,245);
            this.CardGradiantEndColor = Color.FromArgb(242,242,245);
            this.CardGradiantMiddleColor = Color.FromArgb(242,242,245);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}