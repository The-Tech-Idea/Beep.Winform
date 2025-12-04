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
            
            // Reinitialize painter when theme changes
            InitializePainter();
            
            // Update metrics and redraw
            UpdateMetrics();
            Invalidate();
        }
    }
}

