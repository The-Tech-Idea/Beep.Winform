namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool ApplyMutationRecord(CalendarMutationRecord record, bool undo)
        {
            if (record == null)
            {
                return false;
            }

            var target = undo ? record.BeforeEvent : record.AfterEvent;

            _suspendHistory = true;
            try
            {
                switch (record.Kind)
                {
                    case CalendarEventMutationKind.Create:
                    case CalendarEventMutationKind.Copy:
                        if (undo)
                        {
                            if (record.AfterEvent == null)
                            {
                                return false;
                            }

                            return RemoveEventById(record.AfterEvent.Id);
                        }

                        return AddOrReplaceEvent(target);

                    case CalendarEventMutationKind.Delete:
                        if (undo)
                        {
                            return AddOrReplaceEvent(record.BeforeEvent);
                        }

                        if (record.BeforeEvent == null)
                        {
                            return false;
                        }

                        return RemoveEventById(record.BeforeEvent.Id);

                    case CalendarEventMutationKind.Update:
                    case CalendarEventMutationKind.Move:
                    case CalendarEventMutationKind.ResizeStart:
                    case CalendarEventMutationKind.ResizeEnd:
                        return AddOrReplaceEvent(target);

                    default:
                        return false;
                }
            }
            finally
            {
                _suspendHistory = false;
            }
        }

    }
}