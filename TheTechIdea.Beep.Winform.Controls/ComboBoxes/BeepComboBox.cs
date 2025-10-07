using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Modern BeepComboBox implementation using painter methodology
    /// A dropdown combo box control with advanced styling and BaseControl integration
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep ComboBox")]
    [Category("Beep Controls")]
    [Description("A modern combo box control with advanced styling, painter methodology, and BaseControl features.")]
    public partial class BeepComboBox : BaseControl
    {
        // All implementation is in partial classes:
        // - BeepComboBox.Core.cs: Core fields and initialization
        // - BeepComboBox.Properties.cs: All properties
        // - BeepComboBox.Events.cs: Event handlers
        // - BeepComboBox.Methods.cs: Public methods
        // - BeepComboBox.Drawing.cs: DrawContent override and painting
        
        // Component initialization is handled in BeepComboBox.Core.cs constructor
    }
}
