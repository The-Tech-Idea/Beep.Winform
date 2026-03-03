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

        // ── ENH-12: Accessibility ───────────────────────────────────────────
        /// <summary>Returns an accessible object that exposes the combo box value to screen readers.</summary>
        protected override System.Windows.Forms.AccessibleObject CreateAccessibilityInstance()
            => new BeepComboBoxAccessibleObject(this);

        private sealed class BeepComboBoxAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepComboBox _owner;
            public BeepComboBoxAccessibleObject(BeepComboBox owner) : base(owner) => _owner = owner;

            public override string Value
                => _owner.SelectedItem?.Text ?? _owner.Text ?? string.Empty;

            public override System.Windows.Forms.AccessibleRole Role
                => System.Windows.Forms.AccessibleRole.ComboBox;

            public override System.Windows.Forms.AccessibleStates State
            {
                get
                {
                    var s = base.State;
                    if (_owner._isDropdownOpen)
                        s |= System.Windows.Forms.AccessibleStates.Expanded;
                    else
                        s |= System.Windows.Forms.AccessibleStates.Collapsed;
                    return s;
                }
            }
        }
    }
}
