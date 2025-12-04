namespace TheTechIdea.Beep.Winform.Controls.Switchs.Models
{
    /// <summary>
    /// Represents the combined state of the switch (checked + interaction state)
    /// </summary>
    public enum SwitchState
    {
        // Off states
        Off_Normal,
        Off_Hover,
        Off_Pressed,
        Off_Disabled,
        Off_Focused,
        
        // On states
        On_Normal,
        On_Hover,
        On_Pressed,
        On_Disabled,
        On_Focused,
        
        // Transitioning (during animation)
        Transitioning
    }
}

