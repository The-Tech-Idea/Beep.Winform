namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void FlushPendingVisualUpdates()
        {
            if (_pendingLayoutUpdate)
            {
                _pendingLayoutUpdate = false;
                UpdateLayout();
            }

            if (_pendingRedraw)
            {
                _pendingRedraw = false;
                Invalidate();
            }
        }
    }
}