using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplySwitch()
        {
            // UISwitch: gray off track, green on track, white thumb. The previous stamp set
            // every slot to the page background/ink, so ON was indistinguishable from OFF
            // and the ON caption (accent = SwitchSelectedBackColor) vanished light-on-light.
            this.SwitchBackColor = Color.FromArgb(229, 229, 234);      // systemGray5 off track
            this.SwitchBorderColor = BorderColor;
            this.SwitchForeColor = ForeColor;
            this.SwitchSelectedBackColor = SuccessColor;               // iOS green on track
            this.SwitchSelectedBorderColor = SuccessColor;
            this.SwitchSelectedForeColor = OnPrimaryColor;             // white thumb
            this.SwitchHoverBackColor = Color.FromArgb(209, 209, 214); // systemGray4 off hover
            this.SwitchHoverBorderColor = BorderColor;
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}