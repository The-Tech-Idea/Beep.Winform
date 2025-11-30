using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// A combined control that integrates BeepListBox and BeepMultiChipGroup
    /// with optional search functionality. Provides bidirectional selection
    /// synchronization between the list and chip components.
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep ChipListBox")]
    [Category("Beep Controls")]
    [Description("A combined control with ListBox and MultiChipGroup with bidirectional selection sync.")]
    public partial class BeepChipListBox : BaseControl
    {
        // Implementation is split across partial class files:
        // - BeepChipListBox.cs: Main class, constructor, core initialization
        // - BeepChipListBox.Properties.cs: All properties
        // - BeepChipListBox.Events.cs: Event handlers and overrides
        // - BeepChipListBox.Methods.cs: Public methods and helpers
        // - BeepChipListBox.Drawing.cs: Custom drawing logic
    }
}

