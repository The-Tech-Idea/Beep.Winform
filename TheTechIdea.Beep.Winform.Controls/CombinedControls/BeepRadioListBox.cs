using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// A combined control that integrates BeepRadioGroup and BeepListBox
    /// with optional search functionality. Provides bidirectional selection
    /// synchronization between the radio group and list components.
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep RadioListBox")]
    [Category("Beep Controls")]
    [Description("A combined control with RadioGroup and ListBox with bidirectional selection sync.")]
    public partial class BeepRadioListBox : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.RadioList;
        protected internal override Padding StylePadding => new Padding(0);
        // Implementation is split across partial class files:
        // - BeepRadioListBox.cs: Main class definition
        // - BeepRadioListBox.Core.cs: Fields, constructor, core initialization
        // - BeepRadioListBox.Properties.cs: All properties
        // - BeepRadioListBox.Events.cs: Event handlers and overrides
        // - BeepRadioListBox.Methods.cs: Public methods and helpers
    }
}

