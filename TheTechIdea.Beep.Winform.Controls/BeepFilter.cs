using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Filter")]
    [Description("Filter control for filtering data sources")]
     public class BeepFilter : BeepControl
        {
            private FlowLayoutPanel filterPanel;
            private Panel displayPanel;
            private List<BeepFilterItem> filterItems = new List<BeepFilterItem>();
            private object dataSource;
            private List<string> fieldNames = new List<string>();
            private List<Type> fieldTypes = new List<Type>();
            private Panel headerPanel; // Panel for the + button

            public event EventHandler<FilterChangedEventArgs> FilterChanged;

            public FilterCriteriaDisplayStyle DisplayStyle { get; set; } = FilterCriteriaDisplayStyle.Visual;
            public bool UseMenuForOperandsAndOperators { get; set; } = false;

            public BeepFilter()
            {
                // Header panel for the + button
                headerPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 30,
                    BackColor = Color.LightGray // For visibility
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
                    contextMenu.Items.Add("Add Condition", null, (s2, e2) => AddNewCondition());
                //    contextMenu.Items.Add("Add Group", null, (s2, e2) => Debug.WriteLine("Add Group not yet implemented"));
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
                    Visible = DisplayStyle == FilterCriteriaDisplayStyle.Visual,
                    BackColor = Color.White // For visibility
                };
                displayPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Visible = DisplayStyle == FilterCriteriaDisplayStyle.Text,
                    BackColor = Color.LightYellow // For visibility
                };
                Controls.Add(filterPanel);
                Controls.Add(displayPanel);

                // Toggle display style button
                var toggleButton = new Button { Text = "Toggle Display", Dock = DockStyle.Bottom };
                toggleButton.Click += (s, e) =>
                {
                    DisplayStyle = DisplayStyle == FilterCriteriaDisplayStyle.Visual ? FilterCriteriaDisplayStyle.Text : FilterCriteriaDisplayStyle.Visual;
                    filterPanel.Visible = DisplayStyle == FilterCriteriaDisplayStyle.Visual;
                    displayPanel.Visible = DisplayStyle == FilterCriteriaDisplayStyle.Text;
                    UpdateDisplay();
                    Debug.WriteLine($"Toggled Display to {DisplayStyle}, filterPanel.Visible = {filterPanel.Visible}, displayPanel.Visible = {displayPanel.Visible}");
                };
                Controls.Add(toggleButton);

                // Apply and Clear buttons
                var applyButton = new Button { Text = "Apply Filter", Dock = DockStyle.Bottom };
                applyButton.Click += (s, e) => ApplyFilter();
                var clearButton = new Button { Text = "Clear Filter", Dock = DockStyle.Bottom };
                clearButton.Click += (s, e) => ClearFilter();
                Controls.Add(applyButton);
                Controls.Add(clearButton);

                AddNewCondition(); // Add initial condition
            }

            public object DataSource
            {
                get => dataSource;
                set
                {
                    dataSource = value;
                    UpdateFieldsFromDataSource();
                    UpdateFilterItems();
                    UpdateDisplay();
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
                    Debug.WriteLine("DataSource is null, no fields extracted");
                    return;
                }

                if (dataSource is DataTable dataTable)
                {
                    Debug.WriteLine("Extracting fields from DataTable");
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        fieldNames.Add(column.ColumnName);
                        fieldTypes.Add(column.DataType);
                    }
                }
                else if (dataSource is BindingSource bindingSrc)
                {
                    Debug.WriteLine($"Extracting fields from BindingSource: DataSource = {bindingSrc.DataSource?.GetType()}, DataMember = {bindingSrc.DataMember}");
                    var data = bindingSrc.DataSource ?? bindingSrc.List;

                    if (!string.IsNullOrEmpty(bindingSrc.DataMember))
                    {
                        Debug.WriteLine($"Processing DataMember: {bindingSrc.DataMember}");
                        if (data != null)
                        {
                            PropertyInfo prop = data.GetType().GetProperty(bindingSrc.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (prop != null)
                            {
                                var memberData = prop.GetValue(data);
                                Debug.WriteLine($"Resolved DataMember to type: {memberData?.GetType()}");

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
                                else
                                {
                                    Debug.WriteLine($"DataMember '{bindingSrc.DataMember}' resolved to null, falling back to BindingSource.List or DataSource");
                                }
                            }
                            else
                            {
                                Debug.WriteLine($"DataMember '{bindingSrc.DataMember}' not found, falling back to BindingSource.List or DataSource");
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
                        Debug.WriteLine("No DataMember specified, using BindingSource.List or DataSource");
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
                    Debug.WriteLine("Extracting fields from IList");
                    var itemType = list[0].GetType();
                    foreach (var prop in itemType.GetProperties())
                    {
                        fieldNames.Add(prop.Name);
                        fieldTypes.Add(prop.PropertyType);
                    }
                }
                else
                {
                    Debug.WriteLine($"Unrecognized DataSource type: {dataSource.GetType()}, attempting to extract properties");
                    foreach (var prop in dataSource.GetType().GetProperties())
                    {
                        fieldNames.Add(prop.Name);
                        fieldTypes.Add(prop.PropertyType);
                    }
                }

                Debug.WriteLine($"Extracted fields: {string.Join(", ", fieldNames)}");
            }

            private void AddNewCondition()
            {
                Debug.WriteLine("Adding new condition...");
                var newItem = new BeepFilterItem(fieldNames, fieldTypes)
                {
                    UseMenuForOperandsAndOperators = UseMenuForOperandsAndOperators,
                    DisplayStyle = DisplayStyle,
                    ShowLogicalOperator = filterItems.Count > 0,
                    Width = filterPanel.Width - 10, // Adjust width to fit the panel
                    Height = 30 // Set a reasonable height
                };
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
                Debug.WriteLine($"Added new condition, total items: {filterItems.Count}");
            }

            private void UpdateFilterItems()
            {
                foreach (var item in filterItems)
                {
                    item.UseMenuForOperandsAndOperators = UseMenuForOperandsAndOperators;
                    item.DisplayStyle = DisplayStyle;
                    item.ShowLogicalOperator = filterItems.IndexOf(item) > 0;
                    item.Width = filterPanel.Width - 10; // Ensure consistent width
                    item.UpdateDisplay();
                }
            }

            private void UpdateConditions()
            {
                FilterChanged?.Invoke(this, new FilterChangedEventArgs(Conditions));
       //         UpdateDisplay();
                Debug.WriteLine($"Filter conditions updated: {string.Join(" ", Conditions)}");
            }

            private void UpdateDisplay()
            {
            //displayPanel.Controls.Clear();
            //Debug.WriteLine($"Updating display for DisplayStyle: {DisplayStyle}");
            //if (DisplayStyle == FilterCriteriaDisplayStyle.Text)
            //{
            //    var textBox = new TextBox
            //    {
            //        Dock = DockStyle.Fill,
            //        Multiline = true,
            //        ReadOnly = true,
            //        Text = BuildFilterExpression()
            //    };
            //    displayPanel.Controls.Add(textBox);
            //    Debug.WriteLine($"Text mode: Added TextBox with text: {textBox.Text}");
            //}
            //else
            //{
            //    Debug.WriteLine($"Visual mode: filterPanel contains {filterPanel.Controls.Count} items");
            //}
            foreach (var item in filterItems)
            {
                item.DisplayStyle = DisplayStyle;
                item.UpdateDisplay();
            }
        }

            public void ApplyFilter()
            {
                if (dataSource is DataTable dataTable)
                {
                    string filterExpression = BuildFilterExpression();
                    try
                    {
                        dataTable.DefaultView.RowFilter = filterExpression;
                        Debug.WriteLine($"Applied filter to DataTable: {filterExpression}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error applying filter: {ex.Message}");
                    }
                }
                else if (dataSource is BindingSource bindingSrc && bindingSrc.DataSource is DataTable dt)
                {
                    string filterExpression = BuildFilterExpression();
                    try
                    {
                        dt.DefaultView.RowFilter = filterExpression;
                        Debug.WriteLine($"Applied filter to BindingSource DataTable: {filterExpression}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error applying filter: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine("DataSource does not support filtering or is not set.");
                }
            }

            public void ClearFilter()
            {
                filterPanel.Controls.Clear();
                filterItems.Clear();
                AddNewCondition();
                if (dataSource is DataTable dataTable)
                {
                    dataTable.DefaultView.RowFilter = "";
                    Debug.WriteLine("Cleared filter on DataTable");
                }
                else if (dataSource is BindingSource bindingSrc && bindingSrc.DataSource is DataTable dt)
                {
                    dt.DefaultView.RowFilter = "";
                    Debug.WriteLine("Cleared filter on BindingSource DataTable");
                }
                FilterChanged?.Invoke(this, new FilterChangedEventArgs(Conditions));
                UpdateDisplay();
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


