using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Modern BeepTree implementation using painter methodology
    /// A hierarchical tree control with advanced styling and BaseControl integration
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep Tree")]
    [Category("Beep Controls")]
    [Description("A modern tree control with advanced styling, painter methodology, and BaseControl features.")]
    public partial class BeepTree : BaseControl
    {
        // All implementation is in partial classes:
        // - BeepTree.Core.cs: Core fields, events, initialization, and constructor
        // - BeepTree.Properties.cs: All properties (TreeStyle, Nodes, Selection, etc.)
        // - BeepTree.Events.cs: Event handlers (mouse, keyboard, context menu)
        // - BeepTree.Methods.cs: Public methods (node operations, search, etc.)
        // - BeepTree.Drawing.cs: DrawContent override and painting delegation
        // - BeepTree.Layout.cs: Layout calculations and visible node rebuilding
        // - BeepTree.Scrolling.cs: Scrollbar management and scrolling logic
        
        // Component initialization is handled in BeepTree.Core.cs constructor
    }
}
