using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Exposes the filter criteria to assistive technology.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="BeepFilter"/> paints its criteria rather than hosting a child control per
    /// criterion, so MSAA has nothing to enumerate on its own. The control already declared
    /// <see cref="AccessibleRole.Grouping"/>, which announced a group — containing nothing. A screen
    /// reader user could hear that a filter existed and not what it filtered on.
    /// </para>
    /// <para>
    /// Each criterion is reported as a <see cref="AccessibleRole.Row"/> named for its column, with
    /// its operator and value as the accessible value, so "Country Equals Norway" is legible without
    /// sight of the pills.
    /// </para>
    /// </remarks>
    public partial class BeepFilter
    {
        protected override AccessibleObject CreateAccessibilityInstance()
            => new FilterAccessibleObject(this);

        /// <summary>
        /// Index of the criterion with keyboard focus, or -1.
        /// </summary>
        /// <remarks>
        /// The keyboard handler has always tracked this — Alt+Up and Alt+Down reorder relative to it
        /// — but nothing read it and no painter drew it, so a keyboard user could neither see the
        /// focus nor establish it. Surfacing it here at least lets assistive technology report which
        /// criterion is current. The painted indicator is still missing; see plans/06.
        /// </remarks>
        internal int FocusedCriterionIndex => _keyboardHandler?.FocusedFilterIndex ?? -1;

        private sealed class FilterAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepFilter _owner;

            public FilterAccessibleObject(BeepFilter owner) : base(owner) => _owner = owner;

            public override AccessibleRole Role => AccessibleRole.Grouping;

            public override int GetChildCount()
                => _owner.ActiveFilter?.Criteria?.Count ?? 0;

            public override AccessibleObject GetChild(int index)
            {
                var criteria = _owner.ActiveFilter?.Criteria;
                if (criteria == null || index < 0 || index >= criteria.Count) return null;
                return new CriterionAccessibleObject(_owner, index);
            }
        }

        private sealed class CriterionAccessibleObject : AccessibleObject
        {
            private readonly BeepFilter _owner;
            private readonly int _index;

            public CriterionAccessibleObject(BeepFilter owner, int index)
            {
                _owner = owner;
                _index = index;
            }

            private FilterCriteria Criterion
            {
                get
                {
                    var criteria = _owner.ActiveFilter?.Criteria;
                    return criteria != null && _index >= 0 && _index < criteria.Count
                        ? criteria[_index]
                        : null;
                }
            }

            public override AccessibleObject Parent => _owner.AccessibilityObject;

            public override AccessibleRole Role => AccessibleRole.Row;

            /// <summary>The column being filtered — what identifies this criterion.</summary>
            public override string Name => Criterion?.ColumnName ?? string.Empty;

            /// <summary>The condition, read as it would be spoken: "Equals Norway".</summary>
            public override string Value
            {
                get
                {
                    var c = Criterion;
                    if (c == null) return string.Empty;
                    return c.Value == null ? c.Operator.ToString() : $"{c.Operator} {c.Value}";
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    var area = _owner.GetHitAreaByName($"Tag_{_index}");
                    return area == null || area.Bounds.IsEmpty
                        ? Rectangle.Empty
                        : _owner.RectangleToScreen(area.Bounds);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    var state = AccessibleStates.Selectable | AccessibleStates.Focusable;
                    if (_index == _owner.FocusedCriterionIndex)
                        state |= AccessibleStates.Focused;
                    return state;
                }
            }
        }
    }
}
