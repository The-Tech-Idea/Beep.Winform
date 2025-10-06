using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Report;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.DataBase; // EntityStructure, EntityField

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Filter")]
    [Description("Filter control for filtering data sources")]
    public class BeepFilter : BaseControl
    {
        // Group/Tree model
        private interface IFilterNode { }
        private class FilterGroupNode : IFilterNode
        {
            public FilterLogicalOperator GroupOperator { get; set; } = FilterLogicalOperator.And;
            public List<IFilterNode> Children { get; } = new List<IFilterNode>();
        }
        private class FilterConditionNode : IFilterNode
        {
            public BeepFilterItem Item { get; }
            public FilterConditionNode(BeepFilterItem item) => Item = item;
        }

        private FlowLayoutPanel filterPanel;
        private Panel headerPanel;
        private TextBox expressionPreview; // expression preview like DevExpress

        private readonly List<BeepFilterItem> filterItems = new List<BeepFilterItem>();
        private FilterGroupNode RootGroup = new FilterGroupNode();

        private object dataSource;
        private EntityStructure _entity; // optional entity-based source
        private readonly List<string> fieldNames = new List<string>();
        private readonly List<Type> fieldTypes = new List<Type>();

        public event EventHandler<FilterChangedEventArgs> FilterChanged;

        // New: parent can handle these (Bind from a dialog with OK/Cancel/Apply)
        public event EventHandler<FilterActionEventArgs> OkClicked;
        public event EventHandler<FilterActionEventArgs> ApplyClicked;
        public event EventHandler CancelClicked;

        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Use context menu instead of combo boxes for operands/operators.")]
        public bool UseMenuForOperandsAndOperators { get; set; } = false;

        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Show SQL-like expression preview text.")]
        public bool ShowExpressionPreview { get; set; } = true;

        public BeepFilter()
        {
            // Header panel for global actions
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 34,
                BackColor = Color.LightGray
            };

            var addConditionBtn = new Button
            {
                Text = "+ Condition",
                AutoSize = true,
                Location = new Point(5, 5)
            };
            addConditionBtn.Click += (s, e) => AddNewCondition(RootGroup);

            var addGroupBtn = new Button
            {
                Text = "+ Group",
                AutoSize = true,
                Location = new Point(addConditionBtn.Right + 6, 5)
            };
            addGroupBtn.Click += (s, e) => AddNewGroup(RootGroup);

            var toggleOpBtn = new Button
            {
                Text = "And",
                AutoSize = true,
                Location = new Point(addGroupBtn.Right + 12, 5)
            };
            toggleOpBtn.Click += (s, e) =>
            {
                RootGroup.GroupOperator = RootGroup.GroupOperator == FilterLogicalOperator.And
                    ? FilterLogicalOperator.Or
                    : FilterLogicalOperator.And;
                toggleOpBtn.Text = RootGroup.GroupOperator.ToString();
                Render();
            };

            headerPanel.Controls.Add(addConditionBtn);
            headerPanel.Controls.Add(addGroupBtn);
            headerPanel.Controls.Add(toggleOpBtn);
            Controls.Add(headerPanel);

            // Main filter surface
            filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White
            };
            Controls.Add(filterPanel);

            // Expression preview area at bottom
            expressionPreview = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 64,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                ForeColor = Color.DarkSlateGray,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = ScrollBars.Vertical,
                Visible = ShowExpressionPreview
            };
            Controls.Add(expressionPreview);

            // Footer with OK/Cancel/Apply
            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = Color.Gainsboro
            };
            var okBtn = new Button { Text = "OK", Width = 80, Height = 28, Anchor = AnchorStyles.Right | AnchorStyles.Top };
            var cancelBtn = new Button { Text = "Cancel", Width = 80, Height = 28, Anchor = AnchorStyles.Right | AnchorStyles.Top };
            var applyBtn = new Button { Text = "Apply", Width = 80, Height = 28, Anchor = AnchorStyles.Right | AnchorStyles.Top };

            // Lay them out aligned to right
            footer.Resize += (s, e) =>
            {
                int pad = 8;
                applyBtn.Left = footer.Width - applyBtn.Width - pad;
                cancelBtn.Left = applyBtn.Left - cancelBtn.Width - pad;
                okBtn.Left = cancelBtn.Left - okBtn.Width - pad;
                okBtn.Top = cancelBtn.Top = applyBtn.Top = (footer.Height - okBtn.Height) / 2;
            };

            okBtn.Click += (s, e) => RaiseOk();
            applyBtn.Click += (s, e) => RaiseApply();
            cancelBtn.Click += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);

            footer.Controls.Add(okBtn);
            footer.Controls.Add(cancelBtn);
            footer.Controls.Add(applyBtn);
            Controls.Add(footer);

            // Start with one empty condition like DevExpress
            AddNewCondition(RootGroup);
            Render();
        }

        private void RaiseOk()
        {
            var expr = BuildExpressionString(RootGroup);
            var conds = Conditions;
            var appFilters = GenerateAppFilters();
            OkClicked?.Invoke(this, new FilterActionEventArgs(expr, conds, appFilters.Cast<IAppFilter>().ToList()));
        }

        private void RaiseApply()
        {
            var expr = BuildExpressionString(RootGroup);
            var conds = Conditions;
            var appFilters = GenerateAppFilters();
            ApplyClicked?.Invoke(this, new FilterActionEventArgs(expr, conds, appFilters.Cast<IAppFilter>().ToList()));
        }

        // New: allow supplying EntityStructure as the field source
        [Browsable(false)]
        public EntityStructure Entity
        {
            get => _entity;
            set
            {
                _entity = value;
                UpdateFieldsFromEntity();
                Render();
            }
        }

        public object DataSource
        {
            get => dataSource;
            set
            {
                dataSource = value;
                _entity = null; // dataSource takes precedence unless explicitly set again
                UpdateFieldsFromDataSource();
                Render();
            }
        }

        public List<FilterCondition> Conditions => CollectConditions(RootGroup);

        // Recursively collect conditions
        private static List<FilterCondition> CollectConditions(FilterGroupNode group)
        {
            var list = new List<FilterCondition>();
            foreach (var child in group.Children)
            {
                if (child is FilterConditionNode cn)
                {
                    if (cn.Item?.Condition != null)
                        list.Add(cn.Item.Condition);
                }
                else if (child is FilterGroupNode gn)
                {
                    list.AddRange(CollectConditions(gn));
                }
            }
            return list;
        }

        // UI helpers
        private void Render()
        {
            filterPanel.SuspendLayout();
            filterPanel.Controls.Clear();

            // Render root group
            var rootUI = CreateGroupUI(RootGroup, isRoot: true);
            filterPanel.Controls.Add(rootUI);

            filterPanel.ResumeLayout();

            // Update expression
            expressionPreview.Visible = ShowExpressionPreview;
            if (ShowExpressionPreview)
            {
                expressionPreview.Text = BuildExpressionString(RootGroup);
            }

            // Fire changed
            UpdateConditions();
        }

        private Control CreateGroupUI(FilterGroupNode group, bool isRoot)
        {
            var groupPanel = new Panel
            {
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(6),
                Margin = new Padding(6)
            };

            // Header
            var header = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            var opButton = new Button
            {
                Text = group.GroupOperator.ToString(),
                AutoSize = true
            };
            opButton.Click += (s, e) =>
            {
                group.GroupOperator = group.GroupOperator == FilterLogicalOperator.And
                    ? FilterLogicalOperator.Or
                    : FilterLogicalOperator.And;
                opButton.Text = group.GroupOperator.ToString();
                Render();
            };
            header.Controls.Add(opButton);

            var addCondBtn = new Button { Text = "+ Condition", AutoSize = true };
            addCondBtn.Click += (s, e) => AddNewCondition(group);
            header.Controls.Add(addCondBtn);

            var addGroupBtn = new Button { Text = "+ Group", AutoSize = true };
            addGroupBtn.Click += (s, e) => AddNewGroup(group);
            header.Controls.Add(addGroupBtn);

            if (!isRoot)
            {
                var removeBtn = new Button { Text = "Remove", AutoSize = true };
                removeBtn.Click += (s, e) =>
                {
                    // Find and remove this group from its parent
                    RemoveGroup(RootGroup, group);
                    Render();
                };
                header.Controls.Add(removeBtn);
            }

            groupPanel.Controls.Add(header);

            // Children stack
            var childrenPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Margin = new Padding(20, 6, 6, 6)
            };

            foreach (var child in group.Children)
            {
                if (child is FilterConditionNode cn)
                {
                    var item = cn.Item;
                    item.UseMenuForOperandsAndOperators = UseMenuForOperandsAndOperators;
                    item.ShowLogicalOperator = false; // handled by group header
                    item.Width = Math.Max(360, filterPanel.Width - 40);
                    item.ConditionChanged -= OnFilterItemChanged;
                    item.ConditionChanged += OnFilterItemChanged;
                    item.RemoveRequested -= OnFilterItemRemoveRequested;
                    item.RemoveRequested += OnFilterItemRemoveRequested;
                    childrenPanel.Controls.Add(item);
                }
                else if (child is FilterGroupNode gn)
                {
                    var childGroupUI = CreateGroupUI(gn, isRoot: false);
                    childrenPanel.Controls.Add(childGroupUI);
                }
            }

            groupPanel.Controls.Add(childrenPanel);
            return groupPanel;
        }

        private void OnFilterItemChanged(object? sender, FilterChangedEventArgs e)
        {
            UpdateConditions();
            if (ShowExpressionPreview)
                expressionPreview.Text = BuildExpressionString(RootGroup);
        }

        private void OnFilterItemRemoveRequested(object? sender, EventArgs e)
        {
            // Find and remove the node that owns this item
            if (sender is BeepFilterItem item)
            {
                RemoveItem(RootGroup, item);
                Render();
            }
        }

        private static bool RemoveGroup(FilterGroupNode parent, FilterGroupNode target)
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (ReferenceEquals(parent.Children[i], target))
                {
                    parent.Children.RemoveAt(i);
                    return true;
                }
                if (parent.Children[i] is FilterGroupNode gn)
                {
                    if (RemoveGroup(gn, target)) return true;
                }
            }
            return false;
        }

        private static bool RemoveItem(FilterGroupNode parent, BeepFilterItem target)
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is FilterConditionNode cn && ReferenceEquals(cn.Item, target))
                {
                    parent.Children.RemoveAt(i);
                    return true;
                }
                if (parent.Children[i] is FilterGroupNode gn)
                {
                    if (RemoveItem(gn, target)) return true;
                }
            }
            return false;
        }

        private void AddNewCondition(FilterGroupNode group)
        {
            var newItem = new BeepFilterItem(fieldNames, fieldTypes)
            {
                UseMenuForOperandsAndOperators = UseMenuForOperandsAndOperators,
                ShowLogicalOperator = false,
                Width = filterPanel.Width - 10,
                Height = 30
            };
            newItem.ConditionChanged += OnFilterItemChanged;
            newItem.RemoveRequested += OnFilterItemRemoveRequested;

            filterItems.Add(newItem);
            group.Children.Add(new FilterConditionNode(newItem));

            UpdateConditions();
            Render();
        }

        private void AddNewGroup(FilterGroupNode parent)
        {
            var g = new FilterGroupNode { GroupOperator = FilterLogicalOperator.And };
            // Seed the group with one condition for usability
            var item = new BeepFilterItem(fieldNames, fieldTypes)
            {
                UseMenuForOperandsAndOperators = UseMenuForOperandsAndOperators,
                ShowLogicalOperator = false,
                Width = filterPanel.Width - 10,
                Height = 30
            };
            item.ConditionChanged += OnFilterItemChanged;
            item.RemoveRequested += OnFilterItemRemoveRequested;
            filterItems.Add(item);
            g.Children.Add(new FilterConditionNode(item));

            parent.Children.Add(g);
            UpdateConditions();
            Render();
        }

        private void UpdateFieldsFromEntity()
        {
            fieldNames.Clear();
            fieldTypes.Clear();
            if (_entity?.Fields != null)
            {
                foreach (var f in _entity.Fields)
                {
                    fieldNames.Add(f.fieldname);
                    // try parse type name to Type, fallback to string
                    var t = Type.GetType(f.fieldtype, throwOnError: false, ignoreCase: true) ?? typeof(string);
                    fieldTypes.Add(t);
                }
            }
        }

        private void UpdateFieldsFromDataSource()
        {
            fieldNames.Clear();
            fieldTypes.Clear();

            if (dataSource == null)
            {
                return;
            }

            if (dataSource is DataTable dataTable)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    fieldNames.Add(column.ColumnName);
                    fieldTypes.Add(column.DataType);
                }
            }
            else if (dataSource is BindingSource bindingSrc)
            {
                var data = bindingSrc.DataSource ?? bindingSrc.List;

                if (!string.IsNullOrEmpty(bindingSrc.DataMember))
                {
                    if (data != null)
                    {
                        PropertyInfo prop = data.GetType().GetProperty(bindingSrc.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (prop != null)
                        {
                            var memberData = prop.GetValue(data);
                            if (memberData is DataTable dt)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    fieldNames.Add(column.ColumnName);
                                    fieldTypes.Add(column.DataType);
                                }
                            }
                            else if (memberData is IList list && list.Count > 0)
                            {
                                var itemType = list[0].GetType();
                                foreach (var propInfo in itemType.GetProperties())
                                {
                                    fieldNames.Add(propInfo.Name);
                                    fieldTypes.Add(propInfo.PropertyType);
                                }
                            }
                            else if (memberData != null)
                            {
                                foreach (var propInfo in memberData.GetType().GetProperties())
                                {
                                    fieldNames.Add(propInfo.Name);
                                    fieldTypes.Add(propInfo.PropertyType);
                                }
                            }
                        }
                    }

                    if (fieldNames.Count == 0 && bindingSrc.List != null)
                    {
                        data = bindingSrc.List;
                        if (data is DataTable dtFallback)
                        {
                            foreach (DataColumn column in dtFallback.Columns)
                            {
                                fieldNames.Add(column.ColumnName);
                                fieldTypes.Add(column.DataType);
                            }
                        }
                        else if (data is IList listFallback && listFallback.Count > 0)
                        {
                            var itemType = listFallback[0].GetType();
                            foreach (var propInfo in itemType.GetProperties())
                            {
                                fieldNames.Add(propInfo.Name);
                                fieldTypes.Add(propInfo.PropertyType);
                            }
                        }
                    }
                }
                else
                {
                    if (data is DataTable dt)
                    {
                        foreach (DataColumn column in dt.Columns)
                        {
                            fieldNames.Add(column.ColumnName);
                            fieldTypes.Add(column.DataType);
                        }
                    }
                    else if (data is IList list && list.Count > 0)
                    {
                        var itemType = list[0].GetType();
                        foreach (var propInfo in itemType.GetProperties())
                        {
                            fieldNames.Add(propInfo.Name);
                            fieldTypes.Add(propInfo.PropertyType);
                        }
                    }
                }
            }
            else if (dataSource is IList list && list.Count > 0)
            {
                var itemType = list[0].GetType();
                foreach (var prop in itemType.GetProperties())
                {
                    fieldNames.Add(prop.Name);
                    fieldTypes.Add(prop.PropertyType);
                }
            }
            else
            {
                foreach (var prop in dataSource.GetType().GetProperties())
                {
                    fieldNames.Add(prop.Name);
                    fieldTypes.Add(prop.PropertyType);
                }
            }
        }

        private void UpdateConditions()
        {
            FilterChanged?.Invoke(this, new FilterChangedEventArgs(Conditions));
        }

        public void ApplyFilter()
        {
            string filterExpression = BuildExpressionString(RootGroup);

            if (dataSource is DataTable dataTable)
            {
                try { dataTable.DefaultView.RowFilter = filterExpression; }
                catch { /* swallow */ }
            }
            else if (dataSource is BindingSource bindingSrc && bindingSrc.DataSource is DataTable dt)
            {
                try { dt.DefaultView.RowFilter = filterExpression; }
                catch { /* swallow */ }
            }
        }

        // Build classic DataView RowFilter string with parentheses respecting groups
        public string BuildExpressionString() => BuildExpressionString(RootGroup);
        private string BuildExpressionString(FilterGroupNode group)
        {
            var parts = new List<string>();
            foreach (var child in group.Children)
            {
                if (child is FilterConditionNode cn)
                {
                    var c = cn.Item?.Condition;
                    if (c == null || string.IsNullOrEmpty(c.FieldName)) continue;

                    string valueStr;
                    if (c.DataType == typeof(string))
                    {
                        // handle LIKE and functions
                        if (c.Operator == FilterOperator.Contains)
                        {
                            parts.Add($"Convert([{c.FieldName}], 'System.String') LIKE '*{EscapeLike(Convert.ToString(c.Value))}*'");
                            continue;
                        }
                        else if (c.Operator == FilterOperator.StartsWith)
                        {
                            parts.Add($"Convert([{c.FieldName}], 'System.String') LIKE '{EscapeLike(Convert.ToString(c.Value))}*'");
                            continue;
                        }
                        else if (c.Operator == FilterOperator.EndsWith)
                        {
                            parts.Add($"Convert([{c.FieldName}], 'System.String') LIKE '*{EscapeLike(Convert.ToString(c.Value))}'");
                            continue;
                        }
                        valueStr = $"'{c.Value}'";
                    }
                    else if (c.DataType == typeof(DateTime))
                    {
                        valueStr = $"#{((DateTime)c.Value).ToString("MM/dd/yyyy") }#";
                    }
                    else if (c.DataType == typeof(bool))
                    {
                        valueStr = (c.Value is bool b && b) ? "True" : "False";
                    }
                    else
                    {
                        valueStr = c.Value?.ToString() ?? "NULL";
                    }

                    string op = c.Operator switch
                    {
                        FilterOperator.Equals => "=",
                        FilterOperator.NotEquals => "<>",
                        FilterOperator.GreaterThan => ">",
                        FilterOperator.LessThan => "<",
                        // Some solutions may have these members; if not, fallback handled below
                        // FilterOperator.GreaterThanOrEqual => ">=",
                        // FilterOperator.LessThanOrEqual => "<=",
                        FilterOperator.Contains => "LIKE",
                        FilterOperator.IsBetween => "BETWEEN",
                        _ => "="
                    };

                    // Fallback for operators that may not exist in enum but represented by string names
                    if (op == "=" && c.Operator.ToString().Equals("GreaterThanOrEqual", StringComparison.OrdinalIgnoreCase))
                        op = ">=";
                    else if (op == "=" && c.Operator.ToString().Equals("LessThanOrEqual", StringComparison.OrdinalIgnoreCase))
                        op = "<=";

                    if (c.Operator == FilterOperator.IsBetween)
                    {
                        var v1 = valueStr;
                        var v2 = c.DataType == typeof(string) ? $"'{c.Value2}'" : c.Value2?.ToString() ?? "NULL";
                        parts.Add($"([{c.FieldName}] >= {v1} AND [{c.FieldName}] <= {v2})");
                    }
                    else if (op == "LIKE")
                    {
                        parts.Add($"Convert([{c.FieldName}], 'System.String') LIKE '*{EscapeLike(Convert.ToString(c.Value))}*'");
                    }
                    else
                    {
                        parts.Add($"[{c.FieldName}] {op} {valueStr}");
                    }
                }
                else if (child is FilterGroupNode gn)
                {
                    var inner = BuildExpressionString(gn);
                    if (!string.IsNullOrWhiteSpace(inner))
                        parts.Add($"({inner})");
                }
            }
            string joinOp = $" {group.GroupOperator} ";
            return string.Join(joinOp, parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private static string EscapeLike(string? s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("%", "[%]").Replace("*", "[*]").Replace("'", "''");
        }

        public void ClearFilter()
        {
            RootGroup = new FilterGroupNode();
            filterItems.Clear();
            AddNewCondition(RootGroup);
            if (dataSource is DataTable dataTable)
            {
                dataTable.DefaultView.RowFilter = "";
            }
            else if (dataSource is BindingSource bindingSrc && bindingSrc.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = "";
            }
            UpdateConditions();
            Render();
        }

        // Flatten leaf conditions to AppFilter list (grouping is reflected only in expression string)
        public List<IAppFilter> GenerateAppFilters()
        {
            List<IAppFilter> appFilters = new List<IAppFilter>();
            int id = 1;
            foreach (var c in Conditions)
            {
                if (string.IsNullOrEmpty(c.FieldName)) continue;
                var af = new AppFilter
                {
                    ID = id++,
                    GuidID = Guid.NewGuid().ToString(),
                    FieldName = c.FieldName,
                    Operator = c.Operator.ToString(),
                    FilterValue = c.Value?.ToString(),
                    FilterValue1 = c.Operator == FilterOperator.IsBetween ? c.Value2?.ToString() : null,
                    valueType = c.DataType?.Name ?? "string",
                    FieldType = c.DataType ?? typeof(string)
                };
                appFilters.Add(af);
            }
            return appFilters;
        }
    }
}