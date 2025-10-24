using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(235,219,178);
            this.CardBackColor = Color.FromArgb(40,40,40);
            this.CardTitleForeColor = Color.FromArgb(235,219,178);
            this.CardSubTitleForeColor = Color.FromArgb(235,219,178);
            this.CardrGradiantStartColor = Color.FromArgb(40,40,40);
            this.CardGradiantEndColor = Color.FromArgb(40,40,40);
            this.CardGradiantMiddleColor = Color.FromArgb(40,40,40);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}