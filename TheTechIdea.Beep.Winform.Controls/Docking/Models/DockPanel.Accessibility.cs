using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// Exposes a docked panel to assistive technology.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="DockPanel"/> is a region of the workspace, so it reports
    /// <see cref="AccessibleRole.Pane"/> named by its <see cref="Title"/> — the same text the caption
    /// shows. Without a name a screen reader announces "panel" for every region and the user cannot
    /// tell Explorer from Output.
    /// </para>
    /// <para>
    /// The default <see cref="Control.AccessibilityObject"/> would report the role WinForms infers
    /// for a <see cref="Panel"/> (<see cref="AccessibleRole.Client"/>) and no name at all, since
    /// <see cref="Title"/> is this control's caption rather than its <see cref="Control.Text"/>.
    /// </para>
    /// </remarks>
    public partial class DockPanel
    {
        protected override AccessibleObject CreateAccessibilityInstance()
            => new DockPanelAccessibleObject(this);

        private sealed class DockPanelAccessibleObject : Control.ControlAccessibleObject
        {
            private readonly DockPanel _owner;

            public DockPanelAccessibleObject(DockPanel owner) : base(owner) => _owner = owner;

            public override AccessibleRole Role => AccessibleRole.Pane;

            public override string Name
            {
                get => string.IsNullOrWhiteSpace(_owner.Title) ? base.Name : _owner.Title;
                set => base.Name = value;
            }

            /// <summary>
            /// Adds the states a docking region has beyond a plain control: whether it currently
            /// holds focus, and whether it is concealed (maximise conceals siblings with
            /// <see cref="Control.Visible"/> while leaving them docked).
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    var state = base.State;

                    if (_owner.ContainsFocus)
                        state |= AccessibleStates.Focused;

                    if (!_owner.Visible)
                        state |= AccessibleStates.Invisible;

                    return state;
                }
            }
        }
    }
}
