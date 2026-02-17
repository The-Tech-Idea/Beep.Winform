using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Cell content rendering and drawing logic
    /// </summary>
    internal partial class GridRenderHelper
    {
        /// <summary>
        /// Draws the content of a grid cell
        /// </summary>
        private void DrawCellContent(Graphics g, BeepColumnConfig column, BeepCellConfig cell, 
            Rectangle rect, Color foreColor, Color backColor)
        {
            if (g == null || column == null || cell == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            try
            {
                if (column.IsSelectionCheckBox)
                {
                    _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                    _rowCheck.CurrentValue = (bool)(cell.CellValue ?? false);
                    _rowCheck.Draw(g, rect);
                    return;
                }

                // For simple text/numeric columns, use direct text rendering
                if (column.CellEditor == BeepColumnType.Text || 
                    column.CellEditor == BeepColumnType.Link || 
                    column.CellEditor == BeepColumnType.NumericUpDown ||
                    column.CellEditor == BeepColumnType.DateTime)
                {
                    DrawCellAsText(g, column, cell, rect, foreColor);
                    return;
                }

                var drawer = GetDrawerForColumn(column);
                if (drawer == null)
                {
                    DrawCellAsText(g, column, cell, rect, foreColor);
                    return;
                }

                // Try drawing with the control drawer
                try
                {
                    drawer.Theme = _grid.Theme;
                    if (drawer is Control control)
                    {
                        control.BackColor = backColor;
                        control.ForeColor = foreColor;
                        control.Bounds = rect;
                    }

                    // Populate list-based controls
                    if (drawer is BeepComboBox combo)
                    {
                        var items = GetFilteredItems(column, cell);
                        combo.ListItems = new BindingList<SimpleItem>(items);
                    }
                    else if (drawer is BeepListBox listBox)
                    {
                        var items = GetFilteredItems(column, cell);
                        listBox.ListItems = new BindingList<SimpleItem>(items);
                    }
                    else if (drawer is BeepListofValuesBox lov)
                    {
                        var items = GetFilteredItems(column, cell);
                        lov.ListItems = new List<SimpleItem>(items);
                    }

                    if (drawer is IBeepUIComponent ic)
                    {
                        try { ic.SetValue(cell.CellValue); } catch { }
                    }

                    drawer.Draw(g, rect);
                }
                catch
                {
                    // Fallback to plain text rendering if drawer fails
                    DrawCellAsText(g, column, cell, rect, foreColor);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DrawCellContent error for column '{column.ColumnName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Draws cell value as plain text using TextRenderer
        /// </summary>
        private void DrawCellAsText(Graphics g, BeepColumnConfig column, BeepCellConfig cell, 
            Rectangle rect, Color foreColor)
        {
            string text = cell.CellValue?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(text)) return;

            var font = GetSafeCellFont();
            var textRect = new Rectangle(rect.X + 2, rect.Y + 1, Math.Max(1, rect.Width - 4), 
                Math.Max(1, rect.Height - 2));
            var flags = GetTextFormatFlagsForAlignment(column.CellTextAlignment, true);
            TextRenderer.DrawText(g, text, font, textRect, foreColor, flags);
        }

        /// <summary>
        /// Gets filtered items for list-based controls
        /// </summary>
        private List<SimpleItem> GetFilteredItems(BeepColumnConfig column, BeepCellConfig cell)
        {
            var baseItems = column?.Items ?? new List<SimpleItem>();
            if (baseItems == null || baseItems.Count == 0) 
                return new List<SimpleItem>();

            // Simple parent filtering if configured
            if (column != null && !string.IsNullOrEmpty(column.ParentColumnName))
            {
                object? parentValue = cell?.ParentCellValue;
                if (cell?.FilterdList != null && cell.FilterdList.Count > 0)
                    return cell.FilterdList;
                    
                if (parentValue != null)
                {
                    return baseItems.Where(i => i.ParentValue?.ToString() == parentValue.ToString()).ToList();
                }
            }
            return baseItems.ToList();
        }

        /// <summary>
        /// Gets or creates a drawer for a column
        /// </summary>
        private IBeepUIComponent? GetDrawerForColumn(BeepColumnConfig? col)
        {
            if (col == null) return null;
            
            string key = col.ColumnName ?? col.ColumnCaption ?? col.GuidID ?? col.GetHashCode().ToString();
            if (_columnDrawerCache.TryGetValue(key, out var cached) && cached != null)
                return cached;

            IBeepUIComponent drawer = col.CellEditor switch
            {
                BeepColumnType.CheckBoxBool => new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.CheckBoxChar => new BeepCheckBoxChar { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.CheckBoxString => new BeepCheckBoxString { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.ComboBox => new BeepComboBox { IsChild = true, GridMode = true },
                BeepColumnType.DateTime => new BeepDatePicker { IsChild = true, GridMode = true },
                BeepColumnType.Image => new BeepImage { IsChild = true },
                BeepColumnType.Button => new BeepButton { IsChild = true, IsFrameless = true },
                BeepColumnType.ProgressBar => new BeepProgressBar { IsChild = true },
                BeepColumnType.NumericUpDown => new BeepNumericUpDown { IsChild = true, GridMode = true },
                BeepColumnType.Radio => new BeepRadioGroup { IsChild = true, GridMode = true },
                BeepColumnType.ListBox => new BeepListBox { IsChild = true, GridMode = true },
                BeepColumnType.ListOfValue => new BeepListofValuesBox { IsChild = true, GridMode = true },
                BeepColumnType.Text => new BeepTextBox { IsChild = true, GridMode = true, IsFrameless = true, ShowAllBorders = false },
                _ => new BeepTextBox { IsChild = true, GridMode = true, IsFrameless = true, ShowAllBorders = false }
            };

            drawer.Theme = _grid.Theme;
            _columnDrawerCache[key] = drawer;
            return drawer;
        }
    }
}
