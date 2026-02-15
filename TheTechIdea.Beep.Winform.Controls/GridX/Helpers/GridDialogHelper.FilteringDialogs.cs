using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridDialogHelper
    {
        private Panel CreateFilterPanel(string? preferredColumnName = null, string? preferredFilterText = null)
        {
            var panel = new Panel
            {
                Size = new Size(500, 200),
                BorderStyle = BorderStyle.None
            };

            var filterLabel = new Label
            {
                Text = "Filter by Column:",
                Location = new Point(10, 15),
                Size = new Size(100, 20)
            };

            var columnCombo = new ComboBox
            {
                Location = new Point(120, 12),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var filterableColumns = _grid.Data.Columns
                .Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn)
                .ToList();

            columnCombo.Items.Add("All Columns");
            foreach (var col in filterableColumns)
            {
                columnCombo.Items.Add(col.ColumnCaption ?? col.ColumnName);
            }
            columnCombo.SelectedIndex = 0;

            var filterTextLabel = new Label
            {
                Text = "Filter Text:",
                Location = new Point(10, 50),
                Size = new Size(100, 20)
            };

            var filterTextBox = new TextBox
            {
                Location = new Point(120, 47),
                Size = new Size(200, 25),
                Name = "FilterTextBox"
            };

            if (!string.IsNullOrWhiteSpace(preferredColumnName))
            {
                for (int i = 0; i < filterableColumns.Count; i++)
                {
                    var col = filterableColumns[i];
                    if (string.Equals(col.ColumnName, preferredColumnName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(col.ColumnCaption, preferredColumnName, StringComparison.OrdinalIgnoreCase))
                    {
                        columnCombo.SelectedIndex = i + 1;
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(preferredFilterText))
            {
                filterTextBox.Text = preferredFilterText.Trim();
            }
            else if (columnCombo.SelectedIndex > 0)
            {
                var selectedCol = filterableColumns[columnCombo.SelectedIndex - 1];
                filterTextBox.Text = selectedCol.Filter ?? string.Empty;
            }

            var applyButton = new Button
            {
                Text = "Apply Filter",
                Location = new Point(120, 85),
                Size = new Size(100, 30)
            };

            var clearButton = new Button
            {
                Text = "Clear Filter",
                Location = new Point(230, 85),
                Size = new Size(100, 30)
            };

            applyButton.Click += (s, e) => ApplyFilter(columnCombo.SelectedItem?.ToString() ?? string.Empty, filterTextBox.Text);
            clearButton.Click += (s, e) => ClearFilter();

            panel.Controls.AddRange(new Control[]
            {
                filterLabel, columnCombo, filterTextLabel, filterTextBox, applyButton, clearButton
            });

            ApplyThemeToContainer(panel);

            return panel;
        }

        private Panel CreateSearchPanel()
        {
            var panel = new Panel
            {
                Size = new Size(400, 150),
                BorderStyle = BorderStyle.None
            };

            var searchLabel = new Label
            {
                Text = "Search for:",
                Location = new Point(10, 15),
                Size = new Size(80, 20)
            };

            var searchTextBox = new TextBox
            {
                Location = new Point(100, 12),
                Size = new Size(200, 25),
                Name = "SearchTextBox"
            };

            var searchButton = new Button
            {
                Text = "Search",
                Location = new Point(100, 50),
                Size = new Size(80, 30)
            };

            var searchNextButton = new Button
            {
                Text = "Find Next",
                Location = new Point(190, 50),
                Size = new Size(80, 30)
            };

            searchButton.Click += (s, e) => PerformSearch(searchTextBox.Text, true);
            searchNextButton.Click += (s, e) => PerformSearch(searchTextBox.Text, false);

            panel.Controls.AddRange(new Control[]
            {
                searchLabel, searchTextBox, searchButton, searchNextButton
            });

            ApplyThemeToContainer(panel);

            return panel;
        }

        private Panel CreateColumnConfigPanel()
        {
            var panel = new Panel
            {
                Size = new Size(600, 400),
                BorderStyle = BorderStyle.None
            };

            var columnListBox = new CheckedListBox
            {
                Location = new Point(10, 30),
                Size = new Size(200, 300),
                Name = "ColumnListBox"
            };

            foreach (var col in _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID))
            {
                columnListBox.Items.Add(col.ColumnCaption ?? col.ColumnName, col.Visible);
            }

            var columnLabel = new Label
            {
                Text = "Visible Columns:",
                Location = new Point(10, 10),
                Size = new Size(100, 20)
            };

            var widthLabel = new Label
            {
                Text = "Column Width:",
                Location = new Point(230, 30),
                Size = new Size(100, 20)
            };

            var widthNumeric = new NumericUpDown
            {
                Location = new Point(230, 55),
                Size = new Size(100, 25),
                Minimum = 20,
                Maximum = 500,
                Name = "WidthNumeric"
            };

            var applyWidthButton = new Button
            {
                Text = "Apply Width",
                Location = new Point(230, 90),
                Size = new Size(100, 30)
            };

            columnListBox.SelectedIndexChanged += (s, e) =>
            {
                if (columnListBox.SelectedIndex >= 0)
                {
                    var selectedCol = _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList()[columnListBox.SelectedIndex];
                    widthNumeric.Value = selectedCol.Width;
                }
            };

            applyWidthButton.Click += (s, e) =>
            {
                if (columnListBox.SelectedIndex >= 0)
                {
                    var selectedCol = _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList()[columnListBox.SelectedIndex];
                    selectedCol.Width = (int)widthNumeric.Value;
                }
            };

            panel.Controls.AddRange(new Control[]
            {
                columnLabel, columnListBox, widthLabel, widthNumeric, applyWidthButton
            });

            ApplyThemeToContainer(panel);

            return panel;
        }

        private Form CreateFilterDialog(Panel filterPanel)
        {
            var dialog = new Form
            {
                Text = "Filter Data",
                Size = new Size(520, 280),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            filterPanel.Location = new Point(10, 10);
            dialog.Controls.Add(filterPanel);

            var closeButton = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(420, 210),
                Size = new Size(75, 25)
            };

            dialog.Controls.Add(closeButton);
            dialog.AcceptButton = closeButton;

            ApplyThemeToContainer(dialog);

            return dialog;
        }

        private Form CreateSearchDialog(Panel searchPanel)
        {
            var dialog = new Form
            {
                Text = "Search Data",
                Size = new Size(420, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            searchPanel.Location = new Point(10, 10);
            dialog.Controls.Add(searchPanel);

            var closeButton = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(320, 130),
                Size = new Size(75, 25)
            };

            dialog.Controls.Add(closeButton);
            dialog.AcceptButton = closeButton;

            ApplyThemeToContainer(dialog);

            return dialog;
        }

        private Form CreateColumnConfigDialog(Panel configPanel)
        {
            var dialog = new Form
            {
                Text = "Column Configuration",
                Size = new Size(620, 480),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            configPanel.Location = new Point(10, 10);
            dialog.Controls.Add(configPanel);

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(450, 420),
                Size = new Size(75, 25)
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(530, 420),
                Size = new Size(75, 25)
            };

            dialog.Controls.AddRange(new Control[] { okButton, cancelButton });
            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;

            ApplyThemeToContainer(dialog);

            return dialog;
        }

        private void ApplyThemeToContainer(Control container)
        {
            var theme = ResolveDialogTheme();
            if (theme == null)
            {
                return;
            }

            var bodyFont = BeepThemesManager.ToFont(theme.GridCellFont)
                           ?? BeepThemesManager.ToFont(theme.BodyStyle)
                           ?? _grid.Font
                           ?? SystemFonts.DefaultFont;

            var headerFont = BeepThemesManager.ToFont(theme.GridHeaderFont)
                             ?? BeepThemesManager.ToFont(theme.LabelFont)
                             ?? bodyFont;

            var containerBack = ResolveColor(
                container is Form ? theme.DialogBackColor : theme.PanelBackColor,
                theme.GridBackColor,
                theme.BackColor,
                SystemColors.Control);

            var containerFore = ResolveColor(
                container is Form ? theme.DialogForeColor : theme.GridForeColor,
                theme.ForeColor,
                SystemColors.ControlText);

            container.BackColor = containerBack;
            container.ForeColor = containerFore;
            container.Font = bodyFont;

            foreach (Control child in container.Controls)
            {
                ApplyThemeToControl(child, theme, bodyFont, headerFont, containerBack, containerFore);
            }
        }

        private void ApplyThemeToControl(Control control, IBeepTheme theme, Font bodyFont, Font headerFont, Color parentBack, Color parentFore)
        {
            switch (control)
            {
                case Panel:
                    control.BackColor = ResolveColor(theme.PanelBackColor, theme.GridBackColor, parentBack, SystemColors.Control);
                    control.ForeColor = ResolveColor(theme.GridForeColor, parentFore, SystemColors.ControlText);
                    control.Font = bodyFont;
                    break;

                case Label:
                    control.BackColor = ResolveColor(theme.LabelBackColor, parentBack, SystemColors.Control);
                    control.ForeColor = ResolveColor(theme.LabelForeColor, theme.GridHeaderForeColor, parentFore, SystemColors.ControlText);
                    control.Font = headerFont;
                    break;

                case TextBox:
                    control.BackColor = ResolveColor(theme.TextBoxBackColor, theme.GridBackColor, SystemColors.Window);
                    control.ForeColor = ResolveColor(theme.TextBoxForeColor, theme.GridForeColor, SystemColors.WindowText);
                    control.Font = bodyFont;
                    break;

                case ComboBox:
                    control.BackColor = ResolveColor(theme.ComboBoxBackColor, theme.TextBoxBackColor, theme.GridBackColor, SystemColors.Window);
                    control.ForeColor = ResolveColor(theme.ComboBoxForeColor, theme.TextBoxForeColor, theme.GridForeColor, SystemColors.WindowText);
                    control.Font = bodyFont;
                    break;

                case CheckedListBox:
                    control.BackColor = ResolveColor(theme.ListBackColor, theme.GridBackColor, SystemColors.Window);
                    control.ForeColor = ResolveColor(theme.ListForeColor, theme.GridForeColor, SystemColors.WindowText);
                    control.Font = bodyFont;
                    break;

                case NumericUpDown:
                    control.BackColor = ResolveColor(theme.TextBoxBackColor, theme.GridBackColor, SystemColors.Window);
                    control.ForeColor = ResolveColor(theme.TextBoxForeColor, theme.GridForeColor, SystemColors.WindowText);
                    control.Font = bodyFont;
                    break;

                case Button button:
                    button.UseVisualStyleBackColor = false;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackColor = ResolveColor(theme.ButtonBackColor, theme.GridHeaderBackColor, parentBack, SystemColors.Control);
                    button.ForeColor = ResolveColor(theme.ButtonForeColor, theme.GridHeaderForeColor, parentFore, SystemColors.ControlText);
                    button.FlatAppearance.BorderColor = ResolveColor(theme.ButtonBorderColor, theme.GridLineColor, SystemColors.ControlDark);
                    button.Font = bodyFont;
                    break;

                default:
                    control.BackColor = ResolveColor(control.BackColor, parentBack, SystemColors.Control);
                    control.ForeColor = ResolveColor(control.ForeColor, parentFore, SystemColors.ControlText);
                    control.Font = bodyFont;
                    break;
            }

            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child, theme, bodyFont, headerFont, control.BackColor, control.ForeColor);
            }
        }

        private IBeepTheme? ResolveDialogTheme()
        {
            if (_grid._currentTheme != null)
            {
                return _grid._currentTheme;
            }

            _grid._currentTheme = BeepThemesManager.GetTheme(_grid.Theme)
                                 ?? BeepThemesManager.CurrentTheme
                                 ?? BeepThemesManager.GetDefaultTheme();

            return _grid._currentTheme;
        }

        private static Color ResolveColor(params Color[] candidates)
        {
            foreach (var color in candidates)
            {
                if (color != Color.Empty)
                {
                    return color;
                }
            }

            return SystemColors.Control;
        }

        private void ApplyFilter(string columnName, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
            {
                ClearFilter();
                return;
            }

            var text = filterText.Trim();

            if (string.IsNullOrWhiteSpace(columnName) || columnName.Equals("All Columns", StringComparison.OrdinalIgnoreCase))
            {
                _grid.ApplyQuickFilter(text);
                foreach (var col in _grid.Data.Columns)
                {
                    col.IsFiltered = false;
                    col.Filter = string.Empty;
                }
                _grid.SafeInvalidate();
                return;
            }

            var targetColumn = _grid.Data.Columns.FirstOrDefault(c =>
                string.Equals(c.ColumnCaption, columnName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));

            if (targetColumn == null)
            {
                return;
            }

            _grid.SortFilter.Filter(targetColumn.ColumnName, text);
            foreach (var col in _grid.Data.Columns)
            {
                col.IsFiltered = string.Equals(col.ColumnName, targetColumn.ColumnName, StringComparison.OrdinalIgnoreCase);
                col.Filter = col.IsFiltered ? text : string.Empty;
            }
            _grid.SafeInvalidate();
        }

        private void ClearFilter()
        {
            _grid.SortFilter.ClearFilters();
            if (_grid.IsFiltered)
            {
                _grid.ClearFilter();
            }

            foreach (var col in _grid.Data.Columns)
            {
                col.IsFiltered = false;
                col.Filter = string.Empty;
            }
            _grid.SafeInvalidate();
        }

        private void PerformSearch(string searchText, bool fromStart)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return;
            }

            string query = searchText.Trim();
            if (query.Length == 0)
            {
                return;
            }

            // Save the current search filter for persistence 
            _currentSearchFilter = query;

            // Apply quick filter to actually filter rows based on search text
            // This shows only rows that match the search, stored as an active filter chip
            _grid.ApplyQuickFilter(query, null);
            
            // Now find and select the first matching cell in the filtered results
            int rowCount = _grid.Rows.Count;
            int colCount = _grid.Columns.Count;
            if (rowCount == 0 || colCount == 0)
            {
                return;
            }

            if (!string.Equals(_lastSearchQuery, query, StringComparison.OrdinalIgnoreCase))
            {
                fromStart = true;
            }

            int startRow = fromStart ? 0 : Math.Max(0, _lastSearchRow);
            int startCol = fromStart ? 0 : Math.Max(0, _lastSearchCol + 1);

            bool found = false;
            int foundRow = -1;
            int foundCol = -1;

            for (int pass = 0; pass < 2 && !found; pass++)
            {
                int rowFrom = pass == 0 ? startRow : 0;
                int rowTo = pass == 0 ? rowCount : startRow;

                for (int rowIndex = rowFrom; rowIndex < rowTo && !found; rowIndex++)
                {
                    if (!_grid.Rows[rowIndex].IsVisible)
                    {
                        continue;
                    }

                    int colFrom = (pass == 0 && rowIndex == startRow) ? startCol : 0;
                    for (int colIndex = colFrom; colIndex < colCount; colIndex++)
                    {
                        if (!_grid.Columns[colIndex].Visible || _grid.Columns[colIndex].IsSelectionCheckBox)
                        {
                            continue;
                        }

                        var cell = _grid.Rows[rowIndex].Cells[colIndex];
                        string text = cell?.CellValue?.ToString() ?? string.Empty;
                        if (text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            found = true;
                            foundRow = rowIndex;
                            foundCol = colIndex;
                            break;
                        }
                    }
                }

                startCol = 0;
            }

            _lastSearchQuery = query;

            if (!found)
            {
                _lastSearchRow = -1;
                _lastSearchCol = -1;
                return;
            }

            _lastSearchRow = foundRow;
            _lastSearchCol = foundCol;
            _grid.Selection.SelectCell(foundRow, foundCol);
            _grid.Selection.EnsureVisible();
            _grid.SafeInvalidate();
        }

        private void ApplyColumnChanges(Panel configPanel)
        {
            var columnListBox = configPanel.Controls.OfType<CheckedListBox>().FirstOrDefault(c => c.Name == "ColumnListBox");
            if (columnListBox == null)
            {
                return;
            }

            var visibleColumns = _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList();

            for (int i = 0; i < columnListBox.Items.Count && i < visibleColumns.Count; i++)
            {
                visibleColumns[i].Visible = columnListBox.GetItemChecked(i);
            }
        }
    }
}
