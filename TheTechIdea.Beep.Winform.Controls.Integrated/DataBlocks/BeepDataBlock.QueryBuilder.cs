using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Enhanced Query-by-Example
    /// Provides advanced QBE functionality beyond basic Oracle Forms
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        private Dictionary<string, QueryOperator> _queryOperators = new Dictionary<string, QueryOperator>();
        private List<QueryTemplate> _queryTemplates = new List<QueryTemplate>();
        private List<AppFilter> _lastQuery;
        
        #endregion
        
        #region Query Operator Configuration
        
        /// <summary>
        /// Set query operator for a field
        /// </summary>
        public void SetQueryOperator(string fieldName, QueryOperator op)
        {
            _queryOperators[fieldName] = op;
        }
        
        /// <summary>
        /// Get query operator for a field
        /// </summary>
        public QueryOperator GetQueryOperator(string fieldName)
        {
            return _queryOperators.ContainsKey(fieldName) ? _queryOperators[fieldName] : QueryOperator.Equals;
        }
        
        /// <summary>
        /// Clear all query operators (reset to default =)
        /// </summary>
        public void ClearQueryOperators()
        {
            _queryOperators.Clear();
        }
        
        #endregion
        
        #region Enhanced Query Execution
        
        /// <summary>
        /// Build filters from UI controls with operators
        /// </summary>
        private List<AppFilter> BuildEnhancedQueryFilters()
        {
            var filters = new List<AppFilter>();
            
            foreach (var component in UIComponents.Values)
            {
                if (!string.IsNullOrEmpty(component.BoundProperty))
                {
                    var value = component.GetValue();
                    
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        var fieldName = component.BoundProperty;
                        var op = GetQueryOperator(fieldName);
                        
                        filters.Add(new AppFilter
                        {
                            FieldName = fieldName,
                            Operator = GetOperatorString(op),
                            FilterValue = value.ToString()
                        });
                    }
                }
            }
            
            return filters;
        }
        
        /// <summary>
        /// Execute query with enhanced operators
        /// </summary>
        public async Task<bool> ExecuteEnhancedQueryAsync()
        {
            var filters = BuildEnhancedQueryFilters();
            _lastQuery = filters;
            
            // Store query for future reference
            _lastQuery = filters;
            
            return await CoordinatedQuery(filters);
        }
        
        #endregion
        
        #region Query Templates
        
        /// <summary>
        /// Save current query as template
        /// </summary>
        public void SaveQueryAsTemplate(string templateName)
        {
            var filters = BuildEnhancedQueryFilters();
            
            var template = new QueryTemplate
            {
                Name = templateName,
                Filters = filters,
                CreatedDate = DateTime.Now
            };
            
            // Remove existing template with same name
            _queryTemplates.RemoveAll(t => t.Name == templateName);
            _queryTemplates.Add(template);
        }
        
        /// <summary>
        /// Load query template
        /// </summary>
        public bool LoadQueryTemplate(string templateName)
        {
            var template = _queryTemplates.FirstOrDefault(t => t.Name == templateName);
            
            if (template == null)
                return false;
                
            // Set operators and values from template
            foreach (var filter in template.Filters)
            {
                SetQueryOperator(filter.FieldName, GetOperatorFromString(filter.Operator));
                
                // Set value to component
                var component = UIComponents.Values.FirstOrDefault(c => c.BoundProperty == filter.FieldName);
                component?.SetValue(filter.FilterValue);
            }
            
            return true;
        }
        
        /// <summary>
        /// Get all query templates
        /// </summary>
        public List<QueryTemplate> GetQueryTemplates()
        {
            return new List<QueryTemplate>(_queryTemplates);
        }
        
        /// <summary>
        /// Delete a query template
        /// </summary>
        public bool DeleteQueryTemplate(string templateName)
        {
            return _queryTemplates.RemoveAll(t => t.Name == templateName) > 0;
        }
        
        #endregion
        
        #region Operator Conversion
        
        private string GetOperatorString(QueryOperator op)
        {
            return op switch
            {
                QueryOperator.Equals => "=",
                QueryOperator.NotEquals => "!=",
                QueryOperator.GreaterThan => ">",
                QueryOperator.LessThan => "<",
                QueryOperator.GreaterThanOrEqual => ">=",
                QueryOperator.LessThanOrEqual => "<=",
                QueryOperator.Like => "LIKE",
                QueryOperator.NotLike => "NOT LIKE",
                QueryOperator.In => "IN",
                QueryOperator.NotIn => "NOT IN",
                QueryOperator.Between => "BETWEEN",
                QueryOperator.IsNull => "IS NULL",
                QueryOperator.IsNotNull => "IS NOT NULL",
                QueryOperator.StartsWith => "LIKE",
                QueryOperator.EndsWith => "LIKE",
                QueryOperator.Contains => "LIKE",
                _ => "="
            };
        }
        
        private QueryOperator GetOperatorFromString(string op)
        {
            return op.ToUpper() switch
            {
                "=" => QueryOperator.Equals,
                "!=" or "<>" => QueryOperator.NotEquals,
                ">" => QueryOperator.GreaterThan,
                "<" => QueryOperator.LessThan,
                ">=" => QueryOperator.GreaterThanOrEqual,
                "<=" => QueryOperator.LessThanOrEqual,
                "LIKE" => QueryOperator.Like,
                "NOT LIKE" => QueryOperator.NotLike,
                "IN" => QueryOperator.In,
                "NOT IN" => QueryOperator.NotIn,
                "BETWEEN" => QueryOperator.Between,
                "IS NULL" => QueryOperator.IsNull,
                "IS NOT NULL" => QueryOperator.IsNotNull,
                _ => QueryOperator.Equals
            };
        }
        
        #endregion
    }
    
    #region Supporting Classes
    
    /// <summary>
    /// Query operator enum
    /// </summary>
    public enum QueryOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Like,
        NotLike,
        In,
        NotIn,
        Between,
        IsNull,
        IsNotNull,
        StartsWith,
        EndsWith,
        Contains
    }
    
    /// <summary>
    /// Query template for saved queries
    /// </summary>
    public class QueryTemplate
    {
        public string Name { get; set; }
        public List<AppFilter> Filters { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
    
    #endregion
}

