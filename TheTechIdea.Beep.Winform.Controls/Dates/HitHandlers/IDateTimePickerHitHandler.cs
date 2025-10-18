using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Helpers
{
    /// <summary>
    /// Interface for painter-specific hit handlers.
    /// Each painter mode has its own hit handler that:
    /// - Understands that painter's layout structure (single/dual month, time slots, etc.)
    /// - Performs accurate hit testing
    /// - Returns proper date/time values with full context
    /// - Handles click actions specific to that painter mode
    /// </summary>
    internal interface IDateTimePickerHitHandler
    {
        /// <summary>
        /// Gets the painter mode this handler is for.
        /// </summary>
        DatePickerMode Mode { get; }
        
        /// <summary>
        /// Performs hit test and returns detailed hit result with date/time values.
        /// This method has full knowledge of the painter's layout structure.
        /// </summary>
        /// <param name="location">Mouse location to test</param>
        /// <param name="layout">The layout calculated by the painter</param>
        /// <param name="displayMonth">The currently displayed month</param>
        /// <param name="properties">Current picker properties</param>
        /// <returns>Hit test result with area type, date, time, bounds, etc.</returns>
        DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties);
        
        /// <summary>
        /// Handles a click on a hit area and updates the control state.
        /// Returns true if the control should close (for dropdowns).
        /// </summary>
        /// <param name="hitResult">The hit test result</param>
        /// <param name="owner">The owner control to update</param>
        /// <returns>True if dropdown should close</returns>
        bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner);
        
        /// <summary>
        /// Updates hover state for visual feedback.
        /// </summary>
        /// <param name="hitResult">The hit test result</param>
        /// <param name="hoverState">The hover state to update</param>
        void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState);
        
        /// <summary>
        /// Syncs control state TO the handler (control → handler).
        /// Called when the control's values change externally.
        /// </summary>
        /// <param name="owner">The owner control with current values</param>
        void SyncFromControl(BeepDateTimePicker owner);
        
        /// <summary>
        /// Syncs handler state back TO the control (handler → control).
        /// Called when user interaction changes values in the handler.
        /// </summary>
        /// <param name="owner">The owner control to update</param>
        void SyncToControl(BeepDateTimePicker owner);
        
        /// <summary>
        /// Validates if the current selection in the handler is complete and valid.
        /// For example, range pickers need both start and end dates.
        /// </summary>
        /// <returns>True if selection is valid and complete</returns>
        bool IsSelectionComplete();
        
        /// <summary>
        /// Resets the handler's internal state.
        /// </summary>
        void Reset();
    }
}
