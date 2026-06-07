using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTextBox
    {
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new BeepTextBoxAccessibleObject(this);
        }

        internal void NotifyTextChangedUIA()
        {
            if (!IsHandleCreated) return;
            try
            {
                (AccessibilityObject as BeepTextBoxAccessibleObject)?.NotifyTextChanged();
            }
            catch
            {
                // Accessibility notification failure is non-critical
            }
        }

        private sealed class BeepTextBoxAccessibleObject : Control.ControlAccessibleObject
        {
            private readonly BeepTextBox _owner;

            public BeepTextBoxAccessibleObject(BeepTextBox owner) : base(owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            public override AccessibleRole Role => AccessibleRole.Text;

            public override string Name
            {
                get
                {
                    if (!string.IsNullOrEmpty(_owner.AccessibleName))
                        return _owner.AccessibleName;
                    return _owner.LabelText ?? base.Name ?? string.Empty;
                }
                set => _owner.AccessibleName = value;
            }

            public override string Description
            {
                get
                {
                    string desc = _owner.PlaceholderText ?? string.Empty;
                    if (!string.IsNullOrEmpty(_owner.HelperText))
                        desc = string.IsNullOrEmpty(desc) ? _owner.HelperText : desc + " " + _owner.HelperText;
                    if (_owner.HasError && !string.IsNullOrEmpty(_owner.ErrorText))
                        desc = _owner.ErrorText;
                    return desc;
                }
            }

            public override string Value
            {
                get
                {
                    if (_owner._useSystemPasswordChar && !string.IsNullOrEmpty(_owner._text))
                        return "Password";
                    if (_owner._passwordChar != '\0' && !string.IsNullOrEmpty(_owner._text))
                        return "Password";
                    return _owner._text ?? string.Empty;
                }
            }

            public override string KeyboardShortcut => string.Empty;

            public override AccessibleStates State
            {
                get
                {
                    var state = base.State;
                    if (_owner._readOnly)
                        state |= AccessibleStates.ReadOnly;
                    if (_owner.Focused)
                        state |= AccessibleStates.Focused;
                    return state;
                }
            }

            internal void NotifyTextChanged()
            {
                NotifyClients(AccessibleEvents.ValueChange);
                NotifyClients(AccessibleEvents.NameChange);
            }

            internal void NotifyErrorStateChanged()
            {
                NotifyClients(AccessibleEvents.ValueChange);
            }
        }
    }
}
