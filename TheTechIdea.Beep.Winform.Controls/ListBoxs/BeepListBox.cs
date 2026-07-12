using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

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
        protected override Size DefaultSize => BeepLayoutMetrics.ListBox;
        protected internal override Padding StylePadding => new Padding(0);
        // All implementation is in partial classes:
        // - BeepListBox.Core.cs: Core fields and initialization
        // - BeepListBox.Properties.cs: All properties
        // - BeepListBox.Events.cs: Event handlers
        // - BeepListBox.Methods.cs: Public methods
        // - BeepListBox.Drawing.cs: DrawContent override and painting
        
        // Component initialization is handled in BeepListBox.Core.cs constructor
    }
}
