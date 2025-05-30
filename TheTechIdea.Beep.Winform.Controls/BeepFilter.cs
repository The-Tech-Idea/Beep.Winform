﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Helpers;
namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Filter")]
    [Description("Filter control for filtering data sources")]
    public class BeepFilter : BeepControl
    {
        private FlowLayoutPanel filterPanel;
        private Panel displayPanel; // Can be removed if not needed
        private List<BeepFilterItem> filterItems = new List<BeepFilterItem>();
        private object dataSource;
        private List<string> fieldNames = new List<string>();
        private List<Type> fieldTypes = new List<Type>();
        private Panel headerPanel;

        public event EventHandler<FilterChangedEventArgs> FilterChanged;

        public bool UseMenuForOperandsAndOperators { get; set; } = false;

        public BeepFilter()
        {
            // Header panel for the + button
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.LightGray
            };
            var addButton = new Button
            {
                Text = "+",
                Width = 20,
                Height = 20,
                Location = new Point(5, 5)
            };
            addButton.Click += (s, e) =>
            {
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("And", null, (s2, e2) => AddNewCondition(FilterLogicalOperator.And));
                contextMenu.Items.Add("Or", null, (s2, e2) => AddNewCondition(FilterLogicalOperator.Or));
                contextMenu.Show(addButton, new Point(0, addButton.Height));
            };
            headerPanel.Controls.Add(addButton);
            Controls.Add(headerPanel);

            // Initialize the layout
            filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Visible = true, // Always visible
                BackColor = Color.White
            };
            displayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false // Disable text mode panel since we rely on visual mode
            };
            Controls.Add(filterPanel);
            Controls.Add(displayPanel);

            AddNewCondition(); // Add initial condition with default logical operator
        }

        public object DataSource
        {
            get => dataSource;
            set
            {
                dataSource = value;
                UpdateFieldsFromDataSource();
                UpdateFilterItems();
            }
        }

        public List<FilterCondition> Conditions
        {
            get => filterItems.Select(item => item.Condition).ToList();
        }

        private void UpdateFieldsFromDataSource()
        {
            fieldNames.Clear();
            fieldTypes.Clear();

            if (dataSource == null)
            {
             //  MiscFunctions.SendLog("DataSource is null, no fields extracted");
                return;
            }

            if (dataSource is DataTable dataTable)
            {
             //  MiscFunctions.SendLog("Extracting fields from DataTable");
                foreach (DataColumn column in dataTable.Columns)
                {
                    fieldNames.Add(column.ColumnName);
                    fieldTypes.Add(column.DataType);
                }
            }
            else if (dataSource is BindingSource bindingSrc)
            {
             //  MiscFunctions.SendLog($"Extracting fields from BindingSource: DataSource = {bindingSrc.DataSource?.GetType()}, DataMember = {bindingSrc.DataMember}");
                var data = bindingSrc.DataSource ?? bindingSrc.List;

                if (!string.IsNullOrEmpty(bindingSrc.DataMember))
                {
                //   MiscFunctions.SendLog($"Processing DataMember: {bindingSrc.DataMember}");
                    if (data != null)
                    {
                        PropertyInfo prop = data.GetType().GetProperty(bindingSrc.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (prop != null)
                        {
                            var memberData = prop.GetValue(data);
                      //     MiscFunctions.SendLog($"Resolved DataMember to type: {memberData?.GetType()}");

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
               //    MiscFunctions.SendLog("No DataMember specified, using BindingSource.List or DataSource");
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
             //  MiscFunctions.SendLog("Extracting fields from IList");
                var itemType = list[0].GetType();
                foreach (var prop in itemType.GetProperties())
                {
                    fieldNames.Add(prop.Name);
                    fieldTypes.Add(prop.PropertyType);
                }
            }
            else
            {
           //    MiscFunctions.SendLog($"Unrecognized DataSource type: {dataSource.GetType()}, attempting to extract properties");
                foreach (var prop in dataSource.GetType().GetProperties())
                {
                    fieldNames.Add(prop.Name);
                    fieldTypes.Add(prop.PropertyType);
                }
            }

          // MiscFunctions.SendLog($"Extracted fields: {string.Join(", ", fieldNames)}");
        }

        private void AddNewCondition(FilterLogicalOperator logicalOperator = FilterLogicalOperator.And)
        {
         //  MiscFunctions.SendLog($"Adding new condition with logical operator: {logicalOperator}");
            var newItem = new BeepFilterItem(fieldNames, fieldTypes)
            {
                UseMenuForOperandsAndOperators = UseMenuForOperandsAndOperators,
                ShowLogicalOperator = filterItems.Count > 0,
                Width = filterPanel.Width - 10,
                Height = 30
            };
            newItem.Condition.LogicalOperator = logicalOperator; // Set the logical operator
            newItem.UpdateDisplay(); // Force immediate update to reflect the logical operator
            newItem.ConditionChanged += (s, e) => UpdateConditions();
            newItem.RemoveRequested += (s, e) =>
            {
                filterPanel.Controls.Remove(newItem);
                filterItems.Remove(newItem);
                UpdateConditions();
            };

            filterPanel.Controls.Add(newItem);
            filterItems.Add(newItem);
            UpdateConditions();
         //  MiscFunctions.SendLog($"Added new condition, total items: {filterItems.Count}");
        }

        private void UpdateFilterItems()
        {
            foreach (var item in filterItems)
            {
                item.UseMenuForOperandsAndOperators = UseMenuForOperandsAndOperators;
                item.ShowLogicalOperator = filterItems.IndexOf(item) > 0;
                item.Width = filterPanel.Width - 10;
                item.UpdateDisplay();
            }
        }

        private void UpdateConditions()
        {
            FilterChanged?.Invoke(this, new FilterChangedEventArgs(Conditions));
        //   MiscFunctions.SendLog($"Filter conditions updated: {string.Join(" ", Conditions)}");
        }

        public void ApplyFilter()
        {
            if (dataSource is DataTable dataTable)
            {
                string filterExpression = BuildFilterExpression();
                try
                {
                    dataTable.DefaultView.RowFilter = filterExpression;
           //        MiscFunctions.SendLog($"Applied filter to DataTable: {filterExpression}");
                }
                catch (Exception ex)
                {
                  MiscFunctions.SendLog($"Error applying filter: {ex.Message}");
                }
            }
            else if (dataSource is BindingSource bindingSrc && bindingSrc.DataSource is DataTable dt)
            {
                string filterExpression = BuildFilterExpression();
                try
                {
                    dt.DefaultView.RowFilter = filterExpression;
             //      MiscFunctions.SendLog($"Applied filter to BindingSource DataTable: {filterExpression}");
                }
                catch (Exception ex)
                {
                   MiscFunctions.SendLog($"Error applying filter: {ex.Message}");
                }
            }
            else
            {
               MiscFunctions.SendLog("DataSource does not support filtering or is not set.");
            }
        }

        public void ClearFilter()
        {
            filterPanel.Controls.Clear();
            filterItems.Clear();
            AddNewCondition(); // Add initial condition with default logical operator
            if (dataSource is DataTable dataTable)
            {
                dataTable.DefaultView.RowFilter = "";
           //    MiscFunctions.SendLog("Cleared filter on DataTable");
            }
            else if (dataSource is BindingSource bindingSrc && bindingSrc.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = "";
           //    MiscFunctions.SendLog("Cleared filter on BindingSource DataTable");
            }
            FilterChanged?.Invoke(this, new FilterChangedEventArgs(Conditions));
        }
        // New method to generate IAppFilter objects
        public List<IAppFilter> GenerateAppFilters()
        {
            List<IAppFilter> appFilters = new List<IAppFilter>();
            int idCounter = 1;

            foreach (var condition in Conditions)
            {
                if (string.IsNullOrEmpty(condition.FieldName) || condition.Value == null)
                    continue;

                var appFilter = new AppFilter
                {
                    ID = idCounter++,
                    GuidID = Guid.NewGuid().ToString(),
                    FieldName = condition.FieldName,
                    Operator = condition.Operator.ToString(),
                    FilterValue = condition.Value?.ToString(),
                    FilterValue1 = condition.Operator == FilterOperator.IsBetween ? condition.Value2?.ToString() : null,
                    valueType = condition.DataType.Name // e.g., "string", "int", "DateTime"
                };

                appFilters.Add(appFilter);
          //     MiscFunctions.SendLog($"Generated AppFilter: FieldName={appFilter.FieldName}, Operator={appFilter.Operator}, FilterValue={appFilter.FilterValue}, FilterValue1={appFilter.FilterValue1}, valueType={appFilter.valueType}");
            }

            return appFilters;
        }
        private string BuildFilterExpression()
        {
            if (Conditions.Count == 0)
                return "";

            List<string> expressions = new List<string>();
            foreach (var condition in Conditions)
            {
                if (string.IsNullOrEmpty(condition.FieldName) || condition.Value == null)
                    continue;

                string valueStr;
                if (condition.DataType == typeof(string))
                    valueStr = $"'{condition.Value}'";
                else if (condition.DataType == typeof(DateTime))
                    valueStr = $"#{((DateTime)condition.Value).ToString("MM/dd/yyyy")}#";
                else
                    valueStr = condition.Value.ToString();

                string op;
                switch (condition.Operator)
                {
                    case FilterOperator.Equals: op = "="; break;
                    case FilterOperator.NotEquals: op = "<>"; break;
                    case FilterOperator.GreaterThan: op = ">"; break;
                    case FilterOperator.LessThan: op = "<"; break;
                    case FilterOperator.Contains: op = "LIKE"; valueStr = $"'*{condition.Value}*'"; break;
                    case FilterOperator.IsBetween:
                        string value2Str = condition.DataType == typeof(string) ? $"'{condition.Value2}'" : condition.Value2.ToString();
                        expressions.Add($"[{condition.FieldName}] >= {valueStr} AND [{condition.FieldName}] <= {value2Str}");
                        continue;
                    default: op = "="; break;
                }

                expressions.Add($"[{condition.FieldName}] {op} {valueStr}");
            }

            return expressions.Any() ? string.Join($" {Conditions[0].LogicalOperator} ", expressions) : "";
        }
    }

  
}