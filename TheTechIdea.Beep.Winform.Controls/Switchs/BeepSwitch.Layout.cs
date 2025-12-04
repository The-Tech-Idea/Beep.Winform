using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSwitch - Layout and hit area management
    /// </summary>
    public partial class BeepSwitch
    {
        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateMetricsAndHitAreas();
        }

        private void UpdateMetricsAndHitAreas()
        {
            UpdateMetrics();
            RegisterHitAreas();
        }

        private void RegisterHitAreas()
        {
            // Clear existing hit areas
            ClearHitList();
            
            // Track hit area - click anywhere on track to toggle
            AddHitArea("track", _metrics.TrackRect, null, () => Toggle());
            
            // Thumb hit area - for drag support
            AddHitArea("thumb", _metrics.ThumbCurrentRect, null, () => Toggle());
            
            // Label hit areas - click labels to toggle
            if (!string.IsNullOrEmpty(OnLabel))
            {
                AddHitArea("onLabel", _metrics.OnLabelRect, null, () => SetChecked(true));
            }
            
            if (!string.IsNullOrEmpty(OffLabel))
            {
                AddHitArea("offLabel", _metrics.OffLabelRect, null, () => SetChecked(false));
            }
        }

        private void Toggle()
        {
            if (!Enabled) return;
            Checked = !Checked;
        }

        private void SetChecked(bool value)
        {
            if (!Enabled) return;
            Checked = value;
        }
    }
}

