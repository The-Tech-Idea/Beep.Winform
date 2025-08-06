using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public enum FilterCriteriaDisplayStyle
    {
        Visual,
        Text
    }
    public enum FilterOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        Contains,
        IsBetween // Add this
    }

    public enum FilterLogicalOperator
    {
        And,
        Or
    }

    public abstract class FilterNode
    {
        public FilterLogicalOperator LogicalOperator { get; set; } = FilterLogicalOperator.And;
    }

    public class FilterCondition
    {
        public string FieldName { get; set; }
        public Type DataType { get; set; }
        public FilterOperator Operator { get; set; }
        public object Value { get; set; }
        public object Value2 { get; set; } // Add this for "is between"
        public FilterLogicalOperator LogicalOperator { get; set; } = FilterLogicalOperator.And;

        public override string ToString()
        {
            if (Operator == FilterOperator.IsBetween)
                return $"{FieldName} {Operator} {Value} and {Value2}";
            return $"{FieldName} {Operator} {Value}";
        }
    }

    public class FilterGroup : FilterNode
    {
        public List<FilterNode> Nodes { get; set; } = new List<FilterNode>();

        public override string ToString()
        {
            return $"({string.Join($" {LogicalOperator} ", Nodes.Select(n => n.ToString()))})";
        }
    }

    public class FilterChangedEventArgs : EventArgs
    {
        public List<FilterCondition> Conditions { get; set; }
        public FilterChangedEventArgs(List<FilterCondition> conditions)
        {
            Conditions = conditions;
        }
    }
}
