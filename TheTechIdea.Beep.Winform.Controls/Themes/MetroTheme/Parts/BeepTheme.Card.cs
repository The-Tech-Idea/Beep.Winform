using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(32,31,30);
            this.CardBackColor = Color.FromArgb(243,242,241);
            this.CardTitleForeColor = Color.FromArgb(32,31,30);
            this.CardSubTitleForeColor = Color.FromArgb(32,31,30);
            this.CardrGradiantStartColor = Color.FromArgb(243,242,241);
            this.CardGradiantEndColor = Color.FromArgb(243,242,241);
            this.CardGradiantMiddleColor = Color.FromArgb(243,242,241);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}