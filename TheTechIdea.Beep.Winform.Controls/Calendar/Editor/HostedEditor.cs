using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Editor
{
    /// <summary>
    /// Wraps a single <see cref="System.Windows.Forms.Control"/> that is currently
    /// being hosted inside the <see cref="CalendarEditorLayer"/> for in-place
    /// editing of one <see cref="CalendarEvent"/>.
    ///
    /// The wrapped control is provided by the editor factory registered with
    /// <see cref="CalendarEditorDescriptor"/>. The <see cref="HostedEditor"/>
    /// itself is value-type thin: it tracks the descriptor, the bounds inside the
    /// layer, and the dirty flag so <see cref="CalendarEditorHost.EndEdit"/>
    /// knows whether to commit.
    ///
    /// Lifecycle events:
    /// <list type="bullet">
    ///   <item><see cref="Loading"/> — raised in <see cref="BeginEdit"/> before
    ///         bounds are applied. Sample editor factories subscribe here to
    ///         push the current <see cref="CalendarEvent"/> values into the
    ///         wrapped control (e.g. textbox.Text = evt.Title).</item>
    ///   <item><see cref="Saving"/> — raised in <see cref="EndEdit"/>(commit:true)
    ///         after the dirty flag is set. Sample editor factories subscribe
    ///         here to read the wrapped control's values back into the
    ///         <see cref="CalendarEvent"/>.</item>
    ///   <item><see cref="CommitRequested"/> / <see cref="CancelRequested"/> —
    ///         raised by the wrapped control's key handlers (Enter / Esc).
    ///         <see cref="CalendarEditorHost"/> subscribes to these and
    ///         translates them to <see cref="CalendarEditorHost.EndEdit"/>.</item>
    /// </list>
    /// </summary>
    public sealed class HostedEditor
    {
        public HostedEditor(CalendarEditorDescriptor descriptor, Control control)
        {
            Descriptor = descriptor;
            Control = control;
        }

        public CalendarEditorDescriptor Descriptor { get; }

        public Control Control { get; }

        public Rectangle Bounds
        {
            get => Control != null ? Control.Bounds : Rectangle.Empty;
            set { if (Control != null) Control.Bounds = value; }
        }

        public bool IsDirty { get; set; }

        public CalendarEvent Event { get; private set; } = null!;

        /// <summary>Raised from <see cref="BeginEdit"/> before bounds are applied.</summary>
        public event EventHandler<CalendarEvent> Loading = null!;

        /// <summary>Raised from <see cref="EndEdit"/>(commit:true) after the dirty flag is set.</summary>
        public event EventHandler<CalendarEvent> Saving = null!;

        /// <summary>Raised by the wrapped control to ask the host to end the edit with commit.</summary>
        public event EventHandler CommitRequested;

        /// <summary>Raised by the wrapped control to ask the host to end the edit without committing.</summary>
        public event EventHandler CancelRequested;

        /// <summary>
        /// Called by <see cref="CalendarEditorHost.BeginEdit"/> when the editor
        /// is shown. Raises <see cref="Loading"/> so the factory can push values
        /// into the wrapped control, then positions the control inside the layer.
        /// </summary>
        public void BeginEdit(CalendarEvent evt, Rectangle bounds)
        {
            Event = evt;
            IsDirty = false;
            Loading?.Invoke(this, evt);
            Bounds = bounds;
            if (Control != null) Control.Visible = true;
        }

        /// <summary>
        /// Called by <see cref="CalendarEditorHost.EndEdit"/> when the edit
        /// session ends. Raises <see cref="Saving"/> when committing, then
        /// hides the wrapped control.
        /// </summary>
        public void EndEdit(bool commit)
        {
            IsDirty |= commit;
            if (commit) Saving?.Invoke(this, Event);
            if (Control != null) Control.Visible = false;
        }

        public void RequestCommit() => CommitRequested?.Invoke(this, EventArgs.Empty);
        public void RequestCancel() => CancelRequested?.Invoke(this, EventArgs.Empty);
    }
}
