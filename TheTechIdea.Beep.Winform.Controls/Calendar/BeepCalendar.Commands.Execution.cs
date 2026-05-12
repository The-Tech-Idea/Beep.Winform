namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public bool ExecuteCommand(
            CalendarCommandType commandType,
            DateTime? anchorDate = null,
            CalendarViewMode? targetView = null,
            DateTime? visibleRangeEnd = null)
        {
            var args = new CalendarCommandEventArgs(commandType)
            {
                AnchorDate = anchorDate,
                TargetView = targetView,
                VisibleRangeEnd = visibleRangeEnd
            };

            CommandInvoking?.Invoke(this, args);
            if (args.Cancel)
            {
                return false;
            }

            args.Succeeded = ExecuteCommandCore(args);
            CommandInvoked?.Invoke(this, args);
            return args.Succeeded;
        }

    }
}