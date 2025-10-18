using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(245,247,255);
            this.CardBackColor = Color.FromArgb(15,16,32);
            this.CardTitleForeColor = Color.FromArgb(245,247,255);
            this.CardSubTitleForeColor = Color.FromArgb(245,247,255);
            this.CardrGradiantStartColor = Color.FromArgb(15,16,32);
            this.CardGradiantEndColor = Color.FromArgb(15,16,32);
            this.CardGradiantMiddleColor = Color.FromArgb(15,16,32);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}