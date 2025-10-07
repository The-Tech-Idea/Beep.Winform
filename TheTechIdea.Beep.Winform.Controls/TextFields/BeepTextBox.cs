using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// A modern, high-performance text box control with advanced capabilities.
    /// Refactored into partial classes for better organization.
    /// </summary>
    [ToolboxItem(true)]
    [Description("A modern, high-performance text box control with advanced capabilities.")]
    [DisplayName("Beep TextBox")]
    [Category("Beep Controls")]
    public partial class BeepTextBox : IBeepTextBox
    {
        // This is the main entry point for BeepTextBox
        // The implementation is split across multiple partial class files:
        // - BeepTextBox.Core.cs: Core fields, constructor, initialization
        // - BeepTextBox.Properties.cs: All properties
        // - BeepTextBox.Events.cs: Event handlers and overrides
        // - BeepTextBox.Input.cs: Keyboard input and text operations
        // - BeepTextBox.Methods.cs: Helper methods and IBeepTextBox implementation
        // - BeepTextBox.Drawing.cs: Drawing methods
        // - BeepTextBox.Theme.cs: Theme and styling
    }
}
