using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Modern BeepListBox implementation using painter methodology
    /// A list box control with advanced styling and integrated BaseControl features
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep ListBox")]
    [Category("Beep Controls")]
    [Description("A modern list box control with advanced styling, painter methodology, and integrated features.")]
    public partial class BeepListBox : BaseControl
    {
        // All implementation is in partial classes:
        // - BeepListBox.Core.cs: Core fields and initialization
        // - BeepListBox.Properties.cs: All properties
        // - BeepListBox.Events.cs: Event handlers
        // - BeepListBox.Methods.cs: Public methods
        // - BeepListBox.Drawing.cs: DrawContent override and painting
        
        // Component initialization is handled in BeepListBox.Core.cs constructor
    }
}
