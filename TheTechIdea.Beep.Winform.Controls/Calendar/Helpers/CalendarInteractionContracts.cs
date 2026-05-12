using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    public enum CalendarInteractionMode
    {
        None,
        SelectDate,
        SelectEvent,
        CreateEvent,
        MoveEvent,
        ResizeStart,
        ResizeEnd,
        RangeSelect
    }

    public enum CalendarInteractionTargetKind
    {
        None,
        DateCell,
        EventBlock,
        EmptySurface
    }

    public enum CalendarEventResizeEdge
    {
        None,
        Start,
        End
    }

    public enum CalendarEventMutationKind
    {
        Create,
        Move,
        ResizeStart,
        ResizeEnd,
        Copy,
        Update,
        Delete
    }

    public enum CalendarEventEditorMode
    {
        QuickEdit,
        DialogEdit
    }

    internal sealed class CalendarInteractionHitTestResult
    {
        public CalendarInteractionTargetKind TargetKind { get; init; }
        public CalendarInteractionMode RequestedMode { get; init; }
        public CalendarEventResizeEdge ResizeEdge { get; init; }
        public DateTime? Date { get; init; }
        public CalendarEvent Event { get; init; }
        public Point Location { get; init; }
        public Rectangle Bounds { get; init; }

        public bool HasTarget => TargetKind != CalendarInteractionTargetKind.None;
    }

    public sealed class CalendarInteractionEventArgs : EventArgs
    {
        public CalendarInteractionMode Mode { get; }
        public CalendarInteractionTargetKind TargetKind { get; }
        public CalendarEvent Event { get; }
        public DateTime? Date { get; }
        public Point Location { get; }
        public Point Delta { get; }
        public bool IsCommit { get; }
        public Keys ModifierKeys { get; }
        public bool IsCopyOperation { get; }
        public CalendarEventResizeEdge ResizeEdge { get; }
        public DateTime? ProposedStart { get; }
        public DateTime? ProposedEnd { get; }
        public IReadOnlyList<CalendarEvent> Conflicts { get; }

        public CalendarInteractionEventArgs(
            CalendarInteractionMode mode,
            CalendarInteractionTargetKind targetKind,
            DateTime? date,
            CalendarEvent calendarEvent,
            Point location,
            Point delta,
            bool isCommit,
            Keys modifierKeys = Keys.None,
            bool isCopyOperation = false,
            CalendarEventResizeEdge resizeEdge = CalendarEventResizeEdge.None,
            DateTime? proposedStart = null,
            DateTime? proposedEnd = null,
            IReadOnlyList<CalendarEvent> conflicts = null)
        {
            Mode = mode;
            TargetKind = targetKind;
            Date = date;
            Event = calendarEvent;
            Location = location;
            Delta = delta;
            IsCommit = isCommit;
            ModifierKeys = modifierKeys;
            IsCopyOperation = isCopyOperation;
            ResizeEdge = resizeEdge;
            ProposedStart = proposedStart;
            ProposedEnd = proposedEnd;
            Conflicts = conflicts ?? Array.Empty<CalendarEvent>();
        }
    }

    public sealed class CalendarEventMutationEventArgs : EventArgs
    {
        public CalendarEventMutationKind MutationKind { get; }
        public CalendarEvent OriginalEvent { get; }
        public CalendarEvent ProposedEvent { get; }
        public CalendarEvent AppliedEvent { get; }
        public bool Cancel { get; set; }
        public bool IsCopyOperation { get; }
        public IReadOnlyList<CalendarEvent> Conflicts { get; }

        public CalendarEventMutationEventArgs(
            CalendarEventMutationKind mutationKind,
            CalendarEvent originalEvent,
            CalendarEvent proposedEvent,
            CalendarEvent appliedEvent,
            bool isCopyOperation,
            IReadOnlyList<CalendarEvent> conflicts = null)
        {
            MutationKind = mutationKind;
            OriginalEvent = originalEvent;
            ProposedEvent = proposedEvent;
            AppliedEvent = appliedEvent;
            IsCopyOperation = isCopyOperation;
            Conflicts = conflicts ?? Array.Empty<CalendarEvent>();
        }
    }

    public sealed class CalendarEventEditorRequest
    {
        public CalendarEventEditorRequest(
            CalendarEventEditorMode mode,
            CalendarEventMutationKind mutationKind,
            CalendarEvent originalEvent,
            CalendarEvent proposedEvent,
            DateTime anchorDate,
            Point location,
            Keys modifierKeys,
            bool isCopyOperation,
            IReadOnlyList<CalendarEvent> conflicts)
        {
            Mode = mode;
            MutationKind = mutationKind;
            OriginalEvent = originalEvent;
            ProposedEvent = proposedEvent;
            AnchorDate = anchorDate;
            Location = location;
            ModifierKeys = modifierKeys;
            IsCopyOperation = isCopyOperation;
            Conflicts = conflicts ?? Array.Empty<CalendarEvent>();
        }

        public CalendarEventEditorMode Mode { get; }
        public CalendarEventMutationKind MutationKind { get; }
        public CalendarEvent OriginalEvent { get; }
        public CalendarEvent ProposedEvent { get; }
        public DateTime AnchorDate { get; }
        public Point Location { get; }
        public Keys ModifierKeys { get; }
        public bool IsCopyOperation { get; }
        public IReadOnlyList<CalendarEvent> Conflicts { get; }
    }

    public interface ICalendarEventEditor
    {
        CalendarEventEditorMode Mode { get; }
        bool TryEdit(CalendarEventEditorRequest request, out CalendarEvent editedEvent);
    }
}
