namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public class ValidationStateChangedEventArgs : EventArgs
    {
        public ValidationState OldState { get; }
        public ValidationState NewState { get; }
        public string? Message { get; }

        public ValidationStateChangedEventArgs(ValidationState oldState, ValidationState newState, string? message)
        {
            OldState = oldState;
            NewState = newState;
            Message = message;
        }
    }
}
