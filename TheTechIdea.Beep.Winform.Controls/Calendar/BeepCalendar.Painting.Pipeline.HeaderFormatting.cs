namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private string GetHeaderText()
        {
            if (_viewPainter == null)
            {
                return _state.CurrentDate.ToString("MMMM yyyy");
            }
            return _viewPainter.GetHeaderText(_state.CurrentDate);
        }
    }
}