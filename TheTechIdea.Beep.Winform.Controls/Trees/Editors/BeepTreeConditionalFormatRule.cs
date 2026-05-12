using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Editors
{
    /// <summary>
    /// Defines a conditional formatting rule for tree nodes/cells.
    /// When the condition is met, the specified style is applied.
    /// </summary>
    public class BeepTreeConditionalFormatRule
    {
        /// <summary>
        /// Gets or sets the unique name of the rule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the field name to evaluate (for multi-column mode).
        /// Set to null or empty to evaluate the node's Text property.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the condition type.
        /// </summary>
        public ConditionalFormatCondition Condition { get; set; } = ConditionalFormatCondition.Equal;

        /// <summary>
        /// Gets or sets the value to compare against.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the minimum value for Between/NotBetween conditions.
        /// </summary>
        public object MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value for Between/NotBetween conditions.
        /// </summary>
        public object MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the row style to apply when the condition is met.
        /// </summary>
        public BeepTreeRowConfig RowStyle { get; set; } = new BeepTreeRowConfig();

        /// <summary>
        /// Gets or sets the cell style to apply when the condition is met.
        /// </summary>
        public BeepTreeCellConfig CellStyle { get; set; } = new BeepTreeCellConfig();

        /// <summary>
        /// Gets or sets whether this rule is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the priority of the rule (lower number = higher priority).
        /// </summary>
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Evaluates whether the rule applies to the given node.
        /// </summary>
        public bool Evaluate(SimpleItem node)
        {
            if (!Enabled || node == null)
                return false;

            object cellValue = GetValue(node);
            return EvaluateCondition(cellValue);
        }

        /// <summary>
        /// Gets the value to evaluate from the node.
        /// </summary>
        private object GetValue(SimpleItem node)
        {
            if (string.IsNullOrEmpty(FieldName))
                return node.Text;

            if (node.Data != null && node.Data.TryGetValue(FieldName, out var value))
                return value;

            return null;
        }

        /// <summary>
        /// Evaluates the condition against the given value.
        /// </summary>
        private bool EvaluateCondition(object cellValue)
        {
            switch (Condition)
            {
                case ConditionalFormatCondition.Equal:
                    return CompareValues(cellValue, Value) == 0;

                case ConditionalFormatCondition.NotEqual:
                    return CompareValues(cellValue, Value) != 0;

                case ConditionalFormatCondition.GreaterThan:
                    return CompareValues(cellValue, Value) > 0;

                case ConditionalFormatCondition.GreaterThanOrEqual:
                    return CompareValues(cellValue, Value) >= 0;

                case ConditionalFormatCondition.LessThan:
                    return CompareValues(cellValue, Value) < 0;

                case ConditionalFormatCondition.LessThanOrEqual:
                    return CompareValues(cellValue, Value) <= 0;

                case ConditionalFormatCondition.Between:
                    return CompareValues(cellValue, MinValue) >= 0 && CompareValues(cellValue, MaxValue) <= 0;

                case ConditionalFormatCondition.NotBetween:
                    return CompareValues(cellValue, MinValue) < 0 || CompareValues(cellValue, MaxValue) > 0;

                case ConditionalFormatCondition.Contains:
                    return cellValue?.ToString()?.Contains(Value?.ToString() ?? string.Empty) == true;

                case ConditionalFormatCondition.StartsWith:
                    return cellValue?.ToString()?.StartsWith(Value?.ToString() ?? string.Empty) == true;

                case ConditionalFormatCondition.EndsWith:
                    return cellValue?.ToString()?.EndsWith(Value?.ToString() ?? string.Empty) == true;

                case ConditionalFormatCondition.IsNull:
                    return cellValue == null || string.IsNullOrEmpty(cellValue.ToString());

                case ConditionalFormatCondition.IsNotNull:
                    return cellValue != null && !string.IsNullOrEmpty(cellValue.ToString());

                case ConditionalFormatCondition.IsEmpty:
                    return string.IsNullOrEmpty(cellValue?.ToString());

                case ConditionalFormatCondition.IsNotEmpty:
                    return !string.IsNullOrEmpty(cellValue?.ToString());

                default:
                    return false;
            }
        }

        /// <summary>
        /// Compares two values, handling numeric and string comparisons.
        /// </summary>
        private int CompareValues(object a, object b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            // Try numeric comparison
            if (double.TryParse(a.ToString(), out double numA) && double.TryParse(b.ToString(), out double numB))
            {
                return numA.CompareTo(numB);
            }

            // Try DateTime comparison
            if (DateTime.TryParse(a.ToString(), out DateTime dateA) && DateTime.TryParse(b.ToString(), out DateTime dateB))
            {
                return dateA.CompareTo(dateB);
            }

            // Fallback to string comparison
            return string.Compare(a.ToString(), b.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Collection of conditional formatting rules.
    /// </summary>
    public class BeepTreeConditionalFormatCollection : BindingList<BeepTreeConditionalFormatRule>
    {
        /// <summary>
        /// Evaluates all rules against a node and returns the applicable styles.
        /// Rules are evaluated in priority order (lowest priority number first).
        /// </summary>
        public (BeepTreeRowConfig RowConfig, BeepTreeCellConfig CellConfig) Evaluate(SimpleItem node)
        {
            BeepTreeRowConfig mergedRowConfig = null;
            BeepTreeCellConfig mergedCellConfig = null;

            var applicableRules = this.Where(r => r.Enabled && r.Evaluate(node))
                                      .OrderBy(r => r.Priority)
                                      .ToList();

            foreach (var rule in applicableRules)
            {
                if (rule.RowStyle != null)
                {
                    mergedRowConfig ??= new BeepTreeRowConfig();
                    MergeRowConfig(mergedRowConfig, rule.RowStyle);
                }

                if (rule.CellStyle != null)
                {
                    mergedCellConfig ??= new BeepTreeCellConfig();
                    MergeCellConfig(mergedCellConfig, rule.CellStyle);
                }
            }

            return (mergedRowConfig, mergedCellConfig);
        }

        private void MergeRowConfig(BeepTreeRowConfig target, BeepTreeRowConfig source)
        {
            if (source.UseCustomStyle)
            {
                target.UseCustomStyle = true;

                if (source.BackColor != Color.Empty) target.BackColor = source.BackColor;
                if (source.HoverBackColor != Color.Empty) target.HoverBackColor = source.HoverBackColor;
                if (source.SelectedBackColor != Color.Empty) target.SelectedBackColor = source.SelectedBackColor;
                if (source.ForeColor != Color.Empty) target.ForeColor = source.ForeColor;
                if (source.HoverForeColor != Color.Empty) target.HoverForeColor = source.HoverForeColor;
                if (source.SelectedForeColor != Color.Empty) target.SelectedForeColor = source.SelectedForeColor;
                if (source.Font != null) target.Font = source.Font;
                if (source.BorderColor != Color.Empty) target.BorderColor = source.BorderColor;
                if (source.BorderWidth > 0) target.BorderWidth = source.BorderWidth;
                if (source.CornerRadius > 0) target.CornerRadius = source.CornerRadius;
                if (source.Opacity < 255) target.Opacity = source.Opacity;
            }
        }

        private void MergeCellConfig(BeepTreeCellConfig target, BeepTreeCellConfig source)
        {
            if (source.UseCustomStyle)
            {
                target.UseCustomStyle = true;

                if (source.BackColor != Color.Empty) target.BackColor = source.BackColor;
                if (source.ForeColor != Color.Empty) target.ForeColor = source.ForeColor;
                if (source.Font != null) target.Font = source.Font;
                if (source.BorderColor != Color.Empty) target.BorderColor = source.BorderColor;
                if (source.BorderWidth > 0) target.BorderWidth = source.BorderWidth;
                if (source.CornerRadius > 0) target.CornerRadius = source.CornerRadius;
            }
        }
    }

    /// <summary>
    /// Defines the condition types for conditional formatting.
    /// </summary>
    public enum ConditionalFormatCondition
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Between,
        NotBetween,
        Contains,
        StartsWith,
        EndsWith,
        IsNull,
        IsNotNull,
        IsEmpty,
        IsNotEmpty
    }
}
