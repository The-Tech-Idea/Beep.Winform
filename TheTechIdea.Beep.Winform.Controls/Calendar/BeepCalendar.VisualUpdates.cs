namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private int _visualUpdateSuspendCount;
        private bool _pendingLayoutUpdate;
        private bool _pendingRedraw;

        public void BeginVisualUpdate()
        {
            _visualUpdateSuspendCount++;
        }

        public void EndVisualUpdate(bool flush = true)
        {
            if (_visualUpdateSuspendCount > 0)
            {
                _visualUpdateSuspendCount--;
            }

            if (flush && _visualUpdateSuspendCount == 0)
            {
                FlushPendingVisualUpdates();
            }
        }

        private bool IsVisualUpdateSuspended => _visualUpdateSuspendCount > 0;

        private void RequestLayoutUpdate()
        {
            if (IsVisualUpdateSuspended)
            {
                _pendingLayoutUpdate = true;
                return;
            }

            UpdateLayout();
        }

        private void RequestRedraw()
        {
            if (IsVisualUpdateSuspended)
            {
                _pendingRedraw = true;
                return;
            }

            Invalidate();
        }

        private void RequestLayoutAndRedraw()
        {
            if (IsVisualUpdateSuspended)
            {
                _pendingLayoutUpdate = true;
                _pendingRedraw = true;
                return;
            }

            UpdateLayout();
            Invalidate();
        }

    }
}