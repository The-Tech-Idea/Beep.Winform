using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docking.Layout;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Consistency checking for the live layout.
    /// </summary>
    /// <remarks>
    /// <see cref="LayoutValidator"/> implements real checks — unreachable groups, circular parent
    /// references, ratios on groups that do not split, panels registered to one group while
    /// belonging to another — but nothing in the product ever constructed one. It was reachable only
    /// from tests, so a tree that had drifted into any of those states stayed broken silently.
    /// <para>
    /// Validation is deliberately <b>not</b> run on every layout pass. It walks the whole tree and
    /// compares every pair of placed panels, which is wasted work on the common path where nothing
    /// structural changed. It runs where the tree is actually rearranged — a drag-commit, a split, a
    /// definition load, a perspective switch — and reports through <see cref="DockingError"/> rather
    /// than throwing, because a layout that is merely inconsistent is still better than no layout.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>
        /// Set false to skip the post-change consistency check. On by default; the cost is bounded
        /// by the panel count and it only runs on structural changes.
        /// </summary>
        public bool ValidateLayoutOnChange { get; set; } = true;

        /// <summary>
        /// Checks the live tree and the computed layout, returning every problem found.
        /// </summary>
        /// <remarks>
        /// Public so a host can run it on demand — after restoring a layout written by an older
        /// build, for instance — rather than only as a side effect of changing something.
        /// </remarks>
        public IReadOnlyList<ValidationError> ValidateLayout()
        {
            if (_layoutTree == null)
                return Array.Empty<ValidationError>();

            var validator = new LayoutValidator(_layoutTree);
            var result = _layoutController?.CalculateLayoutResult();

            if (result != null)
                validator.Validate(result);
            else
                validator.Validate();

            return validator.GetErrors();
        }

        /// <summary>
        /// Runs <see cref="ValidateLayout"/> after a structural change and reports what it finds.
        /// </summary>
        /// <param name="context">Operation that changed the tree, for the error report.</param>
        private void ValidateAfterStructuralChange(string context)
        {
            if (!ValidateLayoutOnChange || _disposed || IsDesignHosted)
                return;

            // Panel state first: a panel whose state disagrees with its membership is the cause of
            // the layout problems the tree check reports, not a separate symptom.
            ReportPanelStateViolations(context);

            var errors = ValidateLayout();
            if (errors.Count == 0)
                return;

            // One report per operation carrying every problem, not one per problem: a tree that has
            // drifted usually trips several checks at once, and a handler that logs or shows a
            // message should not fire six times for one drag.
            var summary = string.Join("; ", errors.Select(e => e.ToString()));
            OnDockingError(context, errors[0].AffectedElement,
                           new InvalidOperationException(
                               $"Layout is inconsistent after {context}: {summary}"));
        }
    }
}
