using TheTechIdea.Beep.Winform.Controls.Switchs.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSwitch - Theme integration
    /// </summary>
    public partial class BeepSwitch
    {
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            // Apply font theme based on ControlStyle
            SwitchFontHelpers.ApplyFontTheme(ControlStyle);
            
            // Reinitialize painter when theme changes
            InitializePainter();
            
            // Update metrics and redraw
            UpdateMetrics();
            Invalidate();
        }
    }
}

