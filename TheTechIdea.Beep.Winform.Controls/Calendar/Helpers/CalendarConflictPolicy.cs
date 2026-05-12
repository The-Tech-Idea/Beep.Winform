using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    internal interface ICalendarConflictPolicy
    {
        CalendarConflictPolicyMode Mode { get; }

        bool CanSchedule(CalendarEvent candidate, IEnumerable<CalendarEvent> existingEvents, out CalendarConflictResult result);
    }

    internal sealed class CalendarConflictPolicy : ICalendarConflictPolicy
    {
        public CalendarConflictPolicyMode Mode { get; }

        public CalendarConflictPolicy(CalendarConflictPolicyMode mode)
        {
            Mode = mode;
        }

        public bool CanSchedule(CalendarEvent candidate, IEnumerable<CalendarEvent> existingEvents, out CalendarConflictResult result)
        {
            var conflicts = FindConflicts(candidate, existingEvents);
            result = new CalendarConflictResult(candidate, conflicts, Mode);

            if (Mode == CalendarConflictPolicyMode.AllowOverlap)
            {
                return true;
            }

            if (Mode == CalendarConflictPolicyMode.WarnOnOverlap)
            {
                return true;
            }

            return conflicts.Count == 0;
        }

        private static List<CalendarEvent> FindConflicts(CalendarEvent candidate, IEnumerable<CalendarEvent> existingEvents)
        {
            if (candidate == null)
            {
                return new List<CalendarEvent>();
            }

            var source = existingEvents ?? Enumerable.Empty<CalendarEvent>();
            return source.Where(e => e != null && candidate.OverlapsWith(e)).ToList();
        }
    }

    public sealed class CalendarConflictResult
    {
        public CalendarEvent Candidate { get; }
        public IReadOnlyList<CalendarEvent> Conflicts { get; }
        public CalendarConflictPolicyMode PolicyMode { get; }
        public bool HasConflicts => Conflicts.Count > 0;

        public CalendarConflictResult(CalendarEvent candidate, IReadOnlyList<CalendarEvent> conflicts, CalendarConflictPolicyMode policyMode)
        {
            Candidate = candidate;
            Conflicts = conflicts ?? Array.Empty<CalendarEvent>();
            PolicyMode = policyMode;
        }
    }
}
