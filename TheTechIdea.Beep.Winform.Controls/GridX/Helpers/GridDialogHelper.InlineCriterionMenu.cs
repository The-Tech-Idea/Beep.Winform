using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridDialogHelper
    {
        private ContextMenuStrip? _inlineCriterionMenu;

        /// <summary>
        /// Shows an inline, non-modal chip menu editor for a single filter criterion.
        /// </summary>
        public void ShowInlineCriterionEditor(string columnName, Point? anchorClientPoint = null)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                return;
            }

            var targetColumn = _grid.Data.Columns.FirstOrDefault(c =>
                string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.ColumnCaption, columnName, StringComparison.OrdinalIgnoreCase));

            if (targetColumn == null || string.IsNullOrWhiteSpace(targetColumn.ColumnName))
            {
                return;
            }

            CloseInlineCriterionMenu();

            bool isDateColumn = targetColumn.CellEditor == BeepColumnType.DateTime;
            bool isNumericColumn = targetColumn.CellEditor == BeepColumnType.NumericUpDown || targetColumn.CellEditor == BeepColumnType.ProgressBar;
            var typeForOperators = isDateColumn ? typeof(DateTime) : isNumericColumn ? typeof(decimal) : typeof(string);
            var allowedOperators = FilterOperatorExtensions.GetOperatorsForType(typeForOperators).ToHashSet();

            var criterion = _grid.ActiveFilter?.GetCriterion(targetColumn.ColumnName);
            var selectedOp = criterion != null && allowedOperators.Contains(criterion.Operator)
                ? criterion.Operator
                : allowedOperators.Contains(FilterOperator.Contains)
                    ? FilterOperator.Contains
                    : allowedOperators.FirstOrDefault();

            var value1 = criterion?.Value?.ToString() ?? string.Empty;
            var value2 = criterion?.Value2?.ToString() ?? string.Empty;
            var caseSensitive = criterion?.CaseSensitive ?? false;

            var operatorGroups = new Dictionary<string, FilterOperator[]>
            {
                ["Text matching"] = new[]
                {
                    FilterOperator.Contains,
                    FilterOperator.NotContains,
                    FilterOperator.StartsWith,
                    FilterOperator.EndsWith,
                    FilterOperator.Regex
                },
                ["Equality / list"] = new[]
                {
                    FilterOperator.Equals,
                    FilterOperator.NotEquals,
                    FilterOperator.In,
                    FilterOperator.NotIn
                },
                ["Comparison"] = new[]
                {
                    FilterOperator.GreaterThan,
                    FilterOperator.GreaterThanOrEqual,
                    FilterOperator.LessThan,
                    FilterOperator.LessThanOrEqual
                },
                ["Range"] = new[]
                {
                    FilterOperator.Between,
                    FilterOperator.NotBetween
                },
                ["Null / empty"] = new[]
                {
                    FilterOperator.IsNull,
                    FilterOperator.IsNotNull
                }
            };

            _inlineCriterionMenu = new ContextMenuStrip
            {
                ShowImageMargin = false,
                ShowCheckMargin = true,
                RenderMode = ToolStripRenderMode.System
            };

            var menuBaseFont = SystemFonts.MenuFont ?? _grid.Font;
            _inlineCriterionMenu.Items.Add(new ToolStripLabel($"{targetColumn.ColumnCaption ?? targetColumn.ColumnName} criterion")
            {
                Font = new Font(menuBaseFont, FontStyle.Bold)
            });
            _inlineCriterionMenu.Items.Add(new ToolStripSeparator());

            var operatorCombo = new ToolStripComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                AutoSize = false,
                Width = 220
            };
            foreach (var op in allowedOperators)
            {
                operatorCombo.Items.Add(op);
            }
            operatorCombo.SelectedItem = selectedOp;

            var valueBox = new ToolStripTextBox { AutoSize = false, Width = 220, Text = value1 };
            var value2Box = new ToolStripTextBox { AutoSize = false, Width = 220, Text = value2 };

            var caseItem = new ToolStripMenuItem("Case sensitive") { CheckOnClick = true, Checked = caseSensitive };
            if (isDateColumn || isNumericColumn)
            {
                caseItem.Enabled = false;
                caseItem.Checked = false;
            }

            var opHost = new ToolStripMenuItem("Operator") { Enabled = false };
            _inlineCriterionMenu.Items.Add(opHost);
            _inlineCriterionMenu.Items.Add(operatorCombo);

            var valueHost = new ToolStripMenuItem("Value") { Enabled = false };
            _inlineCriterionMenu.Items.Add(valueHost);
            _inlineCriterionMenu.Items.Add(valueBox);

            var value2Host = new ToolStripMenuItem("Value 2") { Enabled = false };
            _inlineCriterionMenu.Items.Add(value2Host);
            _inlineCriterionMenu.Items.Add(value2Box);

            var groupedOpsMenu = new ToolStripMenuItem("Operator groups");
            foreach (var group in operatorGroups)
            {
                var visibleOps = group.Value.Where(allowedOperators.Contains).ToList();
                if (visibleOps.Count == 0)
                {
                    continue;
                }

                var groupItem = new ToolStripMenuItem(group.Key);
                foreach (var op in visibleOps)
                {
                    var item = new ToolStripMenuItem($"{op.GetSymbol()} {op.GetDisplayName()}")
                    {
                        Tag = op
                    };
                    item.Click += (s, e) =>
                    {
                        if (item.Tag is FilterOperator selected)
                        {
                            operatorCombo.SelectedItem = selected;
                            SyncOperatorUi();
                            if (valueBox.Enabled)
                            {
                                valueBox.Focus();
                                valueBox.SelectAll();
                            }
                        }
                    };
                    groupItem.DropDownItems.Add(item);
                }
                groupedOpsMenu.DropDownItems.Add(groupItem);
            }
            _inlineCriterionMenu.Items.Add(groupedOpsMenu);

            _inlineCriterionMenu.Items.Add(caseItem);

            var quickPresetsMenu = new ToolStripMenuItem("Quick presets");
            _inlineCriterionMenu.Items.Add(quickPresetsMenu);

            string DateToken(DateTime dt) => dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string NumberToken(decimal value) => value.ToString(CultureInfo.InvariantCulture);

            void ApplyPreset(FilterOperator op, string first, string second = "")
            {
                if (!allowedOperators.Contains(op))
                {
                    return;
                }

                operatorCombo.SelectedItem = op;
                valueBox.Text = first ?? string.Empty;
                value2Box.Text = second ?? string.Empty;
                SyncOperatorUi();
                if (valueBox.Enabled)
                {
                    valueBox.Focus();
                    valueBox.SelectAll();
                }
            }

            if (isDateColumn)
            {
                var today = DateTime.Today;
                var thisMonthStart = new DateTime(today.Year, today.Month, 1);
                var thisMonthEnd = thisMonthStart.AddMonths(1).AddDays(-1);
                var thisYearStart = new DateTime(today.Year, 1, 1);
                var thisYearEnd = new DateTime(today.Year, 12, 31);

                var presetToday = new ToolStripMenuItem("Today");
                presetToday.Click += (s, e) => ApplyPreset(FilterOperator.Equals, DateToken(today));
                quickPresetsMenu.DropDownItems.Add(presetToday);

                var presetYesterday = new ToolStripMenuItem("Yesterday");
                presetYesterday.Click += (s, e) => ApplyPreset(FilterOperator.Equals, DateToken(today.AddDays(-1)));
                quickPresetsMenu.DropDownItems.Add(presetYesterday);

                var preset7Days = new ToolStripMenuItem("Last 7 days");
                preset7Days.Click += (s, e) => ApplyPreset(FilterOperator.Between, DateToken(today.AddDays(-6)), DateToken(today));
                quickPresetsMenu.DropDownItems.Add(preset7Days);

                var preset30Days = new ToolStripMenuItem("Last 30 days");
                preset30Days.Click += (s, e) => ApplyPreset(FilterOperator.Between, DateToken(today.AddDays(-29)), DateToken(today));
                quickPresetsMenu.DropDownItems.Add(preset30Days);

                var presetThisMonth = new ToolStripMenuItem("This month");
                presetThisMonth.Click += (s, e) => ApplyPreset(FilterOperator.Between, DateToken(thisMonthStart), DateToken(thisMonthEnd));
                quickPresetsMenu.DropDownItems.Add(presetThisMonth);

                var presetThisYear = new ToolStripMenuItem("This year");
                presetThisYear.Click += (s, e) => ApplyPreset(FilterOperator.Between, DateToken(thisYearStart), DateToken(thisYearEnd));
                quickPresetsMenu.DropDownItems.Add(presetThisYear);
            }
            else if (isNumericColumn)
            {
                var presetEqualsZero = new ToolStripMenuItem("Equals 0");
                presetEqualsZero.Click += (s, e) => ApplyPreset(FilterOperator.Equals, NumberToken(0));
                quickPresetsMenu.DropDownItems.Add(presetEqualsZero);

                var presetGreaterZero = new ToolStripMenuItem("Greater than 0");
                presetGreaterZero.Click += (s, e) => ApplyPreset(FilterOperator.GreaterThan, NumberToken(0));
                quickPresetsMenu.DropDownItems.Add(presetGreaterZero);

                var presetLessZero = new ToolStripMenuItem("Less than 0");
                presetLessZero.Click += (s, e) => ApplyPreset(FilterOperator.LessThan, NumberToken(0));
                quickPresetsMenu.DropDownItems.Add(presetLessZero);

                var presetBetweenZeroHundred = new ToolStripMenuItem("Between 0 and 100");
                presetBetweenZeroHundred.Click += (s, e) => ApplyPreset(FilterOperator.Between, NumberToken(0), NumberToken(100));
                quickPresetsMenu.DropDownItems.Add(presetBetweenZeroHundred);

                var presetBetweenHundredThousand = new ToolStripMenuItem("Between 100 and 1000");
                presetBetweenHundredThousand.Click += (s, e) => ApplyPreset(FilterOperator.Between, NumberToken(100), NumberToken(1000));
                quickPresetsMenu.DropDownItems.Add(presetBetweenHundredThousand);
            }

            if (quickPresetsMenu.DropDownItems.Count > 0)
            {
                quickPresetsMenu.DropDownItems.Add(new ToolStripSeparator());
            }

            var presetEmpty = new ToolStripMenuItem("Is empty");
            presetEmpty.Click += (s, e) => ApplyPreset(FilterOperator.IsNull, string.Empty, string.Empty);
            quickPresetsMenu.DropDownItems.Add(presetEmpty);

            var presetNotEmpty = new ToolStripMenuItem("Is not empty");
            presetNotEmpty.Click += (s, e) => ApplyPreset(FilterOperator.IsNotNull, string.Empty, string.Empty);
            quickPresetsMenu.DropDownItems.Add(presetNotEmpty);

            _inlineCriterionMenu.Items.Add(new ToolStripSeparator());

            var applyItem = new ToolStripMenuItem("Apply") { ShortcutKeys = Keys.Control | Keys.Enter };
            var clearItem = new ToolStripMenuItem("Clear this criterion") { ShortcutKeys = Keys.Control | Keys.Back };
            var closeItem = new ToolStripMenuItem("Close") { ShortcutKeys = Keys.Escape };

            _inlineCriterionMenu.Items.Add(applyItem);
            _inlineCriterionMenu.Items.Add(clearItem);
            _inlineCriterionMenu.Items.Add(closeItem);

            void SyncOperatorUi()
            {
                var selected = operatorCombo.SelectedItem is FilterOperator op ? op : selectedOp;
                bool needsSecond = selected == FilterOperator.Between || selected == FilterOperator.NotBetween;
                bool isUnary = selected == FilterOperator.IsNull || selected == FilterOperator.IsNotNull;

                valueHost.Text = needsSecond ? "From" : "Value";
                valueBox.Enabled = !isUnary;
                value2Host.Visible = needsSecond;
                value2Box.Visible = needsSecond;
                value2Box.Enabled = needsSecond;
            }

            void ApplyCriterion()
            {
                var op = operatorCombo.SelectedItem is FilterOperator selected ? selected : selectedOp;
                bool isUnary = op == FilterOperator.IsNull || op == FilterOperator.IsNotNull;
                bool needsSecond = op == FilterOperator.Between || op == FilterOperator.NotBetween;

                var v1 = valueBox.Text?.Trim() ?? string.Empty;
                var v2 = value2Box.Text?.Trim() ?? string.Empty;

                if (!isUnary && string.IsNullOrWhiteSpace(v1))
                {
                    MessageBox.Show(_grid, "Please enter a value.", "Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (needsSecond && string.IsNullOrWhiteSpace(v2))
                {
                    MessageBox.Show(_grid, "Please enter the second value.", "Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var config = _grid.ActiveFilter ?? new FilterConfiguration("Inline Filter") { IsActive = true };
                var existing = config.GetCriterion(targetColumn.ColumnName);
                if (existing == null)
                {
                    existing = new FilterCriteria { ColumnName = targetColumn.ColumnName, IsEnabled = true };
                    config.AddCriteria(existing);
                }

                existing.Operator = op;
                existing.Value = isUnary ? string.Empty : v1;
                existing.Value2 = needsSecond ? v2 : string.Empty;
                existing.CaseSensitive = caseItem.Checked;
                existing.IsEnabled = true;

                targetColumn.IsFiltered = true;
                targetColumn.Filter = isUnary ? op.GetDisplayName() : v1;

                if (_grid.ActiveFilter == null)
                {
                    _grid.ActiveFilter = config;
                }
                else
                {
                    _grid.ApplyActiveFilter();
                }

                _grid.SafeInvalidate();
            }

            operatorCombo.SelectedIndexChanged += (s, e) => SyncOperatorUi();
            applyItem.Click += (s, e) => ApplyCriterion();

            operatorCombo.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ApplyCriterion();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    CloseInlineCriterionMenu();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            valueBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ApplyCriterion();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    CloseInlineCriterionMenu();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            value2Box.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ApplyCriterion();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    CloseInlineCriterionMenu();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            clearItem.Click += (s, e) =>
            {
                _grid.RemoveFilterCriterion(targetColumn.ColumnName);
                targetColumn.IsFiltered = false;
                targetColumn.Filter = string.Empty;
                _grid.SortFilter.Filter(targetColumn.ColumnName, string.Empty);
                _grid.SafeInvalidate();
            };

            closeItem.Click += (s, e) => CloseInlineCriterionMenu();

            _inlineCriterionMenu.Closed += (s, e) =>
            {
                _inlineCriterionMenu?.Dispose();
                _inlineCriterionMenu = null;
            };

            _inlineCriterionMenu.Opened += (s, e) =>
            {
                if (valueBox.Enabled)
                {
                    valueBox.Focus();
                    valueBox.SelectAll();
                }
                else
                {
                    operatorCombo.Focus();
                }
            };

            SyncOperatorUi();

            var anchor = anchorClientPoint ?? new Point(_grid.Width / 2, _grid.Layout.TopFilterRect.Bottom);
            _inlineCriterionMenu.Show(_grid, anchor);
        }

        private void CloseInlineCriterionMenuCore()
        {
            if (_inlineCriterionMenu != null)
            {
                _inlineCriterionMenu.Close();
                _inlineCriterionMenu.Dispose();
                _inlineCriterionMenu = null;
            }
        }

    }
}
