namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public bool UndoLastMutation()
        {
            if (_undoStack.Count == 0)
            {
                return false;
            }

            var record = _undoStack.Peek();
            if (!ApplyMutationRecord(record, undo: true))
            {
                return false;
            }

            _undoStack.Pop();
            _redoStack.Push(record);
            UpdateViewButtonStates();
            return true;
        }

        public bool RedoLastMutation()
        {
            if (_redoStack.Count == 0)
            {
                return false;
            }

            var record = _redoStack.Peek();
            if (!ApplyMutationRecord(record, undo: false))
            {
                return false;
            }

            _redoStack.Pop();
            _undoStack.Push(record);
            UpdateViewButtonStates();
            return true;
        }
    }
}