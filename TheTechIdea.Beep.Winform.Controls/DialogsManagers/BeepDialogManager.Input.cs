using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Input dialog methods for BeepDialogManager
    /// </summary>
    public partial class BeepDialogManager
    {
        #region Text Input

        /// <summary>
        /// Shows a text input dialog (async)
        /// </summary>
        public async Task<string?> InputTextAsync(string title, string prompt, string? defaultValue = null)
        {
            var config = new DialogConfig
            {
                Title = title,
                Message = prompt,
                Preset = DialogPreset.None,
                IconType = BeepDialogIcon.Question,
                Buttons = new[] { BeepDialogButtons.Cancel, BeepDialogButtons.Ok }
            };

            using var dialog = new BeepDialogModal();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputString;

            if (_defaultTheme != null)
                dialog.Theme = _defaultTheme.ThemeName;

            dialog.StartPosition = FormStartPosition.CenterParent;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.ReturnValue;
            }

            return null;
        }

        /// <summary>
        /// Shows a text input dialog (sync)
        /// </summary>
        public string? InputText(string title, string prompt, string? defaultValue = null)
        {
            using var dialog = new BeepDialogModal();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputString;

            if (_defaultTheme != null)
                dialog.Theme = _defaultTheme.ThemeName;

            dialog.StartPosition = FormStartPosition.CenterParent;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.ReturnValue;
            }

            return null;
        }

        /// <summary>
        /// Shows a large text input dialog (multiline)
        /// </summary>
        public string? InputLargeText(string title, string prompt, string? defaultValue = null)
        {
            return InputText(title, prompt, defaultValue);
        }

        /// <summary>
        /// Shows a password input dialog
        /// </summary>
        public string? InputPassword(string title, string prompt)
        {
            using var dialog = new BeepDialogModal();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputString;

            if (_defaultTheme != null)
                dialog.Theme = _defaultTheme.ThemeName;

            dialog.StartPosition = FormStartPosition.CenterParent;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.ReturnValue;
            }

            return null;
        }

        #endregion

        #region Numeric Input

        /// <summary>
        /// Shows a number input dialog (async)
        /// </summary>
        public async Task<double?> InputNumberAsync(string title, string prompt, double? min = null, double? max = null, double? defaultValue = null)
        {
            var result = await InputTextAsync(title, prompt, defaultValue?.ToString());
            
            if (result != null && double.TryParse(result, out double value))
            {
                if (min.HasValue && value < min.Value)
                {
                    await Error("Invalid Input", $"Value must be at least {min.Value}");
                    return null;
                }
                if (max.HasValue && value > max.Value)
                {
                    await Error("Invalid Input", $"Value must be at most {max.Value}");
                    return null;
                }
                return value;
            }

            return null;
        }

        /// <summary>
        /// Shows a number input dialog (sync)
        /// </summary>
        public double? InputNumber(string title, string prompt, double? min = null, double? max = null, double? defaultValue = null)
        {
            var result = InputText(title, prompt, defaultValue?.ToString());

            if (result != null && double.TryParse(result, out double value))
            {
                if (min.HasValue && value < min.Value)
                {
                    ShowError("Invalid Input", $"Value must be at least {min.Value}");
                    return null;
                }
                if (max.HasValue && value > max.Value)
                {
                    ShowError("Invalid Input", $"Value must be at most {max.Value}");
                    return null;
                }
                return value;
            }

            return null;
        }

        /// <summary>
        /// Shows an integer input dialog
        /// </summary>
        public int? InputInteger(string title, string prompt, int? min = null, int? max = null, int? defaultValue = null)
        {
            var result = InputNumber(title, prompt, min, max, defaultValue);
            return result.HasValue ? (int)result.Value : null;
        }

        #endregion

        #region Selection Input

        /// <summary>
        /// Shows a selection dialog from a list of items (async)
        /// </summary>
        public async Task<SimpleItem?> InputSelectAsync(string title, string prompt, List<SimpleItem> items)
        {
            if (items == null || items.Count == 0)
                return null;

            using var dialog = new BeepDialogModal();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputFromList;
            dialog.Items = items;

            if (_defaultTheme != null)
                dialog.Theme = _defaultTheme.ThemeName;

            dialog.StartPosition = FormStartPosition.CenterParent;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.ReturnItem;
            }

            return null;
        }

        /// <summary>
        /// Shows a selection dialog from a list of items (sync)
        /// </summary>
        public SimpleItem? InputSelect(string title, string prompt, List<SimpleItem> items)
        {
            if (items == null || items.Count == 0)
                return null;

            using var dialog = new BeepDialogModal();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputFromList;
            dialog.Items = items;

            if (_defaultTheme != null)
                dialog.Theme = _defaultTheme.ThemeName;

            dialog.StartPosition = FormStartPosition.CenterParent;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.ReturnItem;
            }

            return null;
        }

        /// <summary>
        /// Shows a selection dialog from a list of strings
        /// </summary>
        public string? InputSelectString(string title, string prompt, List<string> items)
        {
            var simpleItems = items.Select(s => new SimpleItem { Text = s, Value = s }).ToList();
            var result = InputSelect(title, prompt, simpleItems);
            return result?.Text;
        }

        /// <summary>
        /// Shows a multi-select dialog (async)
        /// </summary>
        public async Task<List<SimpleItem>> InputMultiSelectAsync(string title, string prompt, List<SimpleItem> items)
        {
            // For now, return single selection wrapped in list
            // TODO: Implement proper multi-select dialog
            var result = await InputSelectAsync(title, prompt, items);
            return result != null ? new List<SimpleItem> { result } : new List<SimpleItem>();
        }

        /// <summary>
        /// Shows a multi-select dialog (sync)
        /// </summary>
        public List<SimpleItem> InputMultiSelect(string title, string prompt, List<SimpleItem> items)
        {
            // For now, return single selection wrapped in list
            // TODO: Implement proper multi-select dialog
            var result = InputSelect(title, prompt, items);
            return result != null ? new List<SimpleItem> { result } : new List<SimpleItem>();
        }

        #endregion

        #region Date/Time Input

        /// <summary>
        /// Shows a date input dialog (async)
        /// </summary>
        public async Task<DateTime?> InputDateAsync(string title, string prompt, DateTime? min = null, DateTime? max = null, DateTime? defaultValue = null)
        {
            // For now, use text input with parsing
            // TODO: Implement proper date picker dialog
            var result = await InputTextAsync(title, prompt, defaultValue?.ToString("yyyy-MM-dd"));

            if (result != null && DateTime.TryParse(result, out DateTime date))
            {
                if (min.HasValue && date < min.Value)
                {
                    await Error("Invalid Date", $"Date must be on or after {min.Value:yyyy-MM-dd}");
                    return null;
                }
                if (max.HasValue && date > max.Value)
                {
                    await Error("Invalid Date", $"Date must be on or before {max.Value:yyyy-MM-dd}");
                    return null;
                }
                return date;
            }

            return null;
        }

        /// <summary>
        /// Shows a date input dialog (sync)
        /// </summary>
        public DateTime? InputDate(string title, string prompt, DateTime? min = null, DateTime? max = null, DateTime? defaultValue = null)
        {
            var result = InputText(title, prompt, defaultValue?.ToString("yyyy-MM-dd"));

            if (result != null && DateTime.TryParse(result, out DateTime date))
            {
                if (min.HasValue && date < min.Value)
                {
                    ShowError("Invalid Date", $"Date must be on or after {min.Value:yyyy-MM-dd}");
                    return null;
                }
                if (max.HasValue && date > max.Value)
                {
                    ShowError("Invalid Date", $"Date must be on or before {max.Value:yyyy-MM-dd}");
                    return null;
                }
                return date;
            }

            return null;
        }

        /// <summary>
        /// Shows a time span input dialog
        /// </summary>
        public TimeSpan? InputTimeSpan(string title, string prompt, TimeSpan? min = null, TimeSpan? max = null, TimeSpan? defaultValue = null)
        {
            var result = InputText(title, prompt, defaultValue?.ToString());

            if (result != null && TimeSpan.TryParse(result, out TimeSpan timeSpan))
            {
                if (min.HasValue && timeSpan < min.Value)
                {
                    ShowError("Invalid Time", $"Time must be at least {min.Value}");
                    return null;
                }
                if (max.HasValue && timeSpan > max.Value)
                {
                    ShowError("Invalid Time", $"Time must be at most {max.Value}");
                    return null;
                }
                return timeSpan;
            }

            return null;
        }

        #endregion

        #region Color Input

        /// <summary>
        /// Shows a color picker dialog (async)
        /// </summary>
        public async Task<Color?> InputColorAsync(string? title = null, Color? initialColor = null)
        {
            using var cd = new ColorDialog();

            if (initialColor.HasValue)
            {
                cd.Color = initialColor.Value;
            }

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? cd.ShowDialog(owner) : cd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return cd.Color;
            }

            return null;
        }

        /// <summary>
        /// Shows a color picker dialog (sync)
        /// </summary>
        public Color? InputColor(string? title = null, Color? initialColor = null)
        {
            using var cd = new ColorDialog();

            if (initialColor.HasValue)
            {
                cd.Color = initialColor.Value;
            }

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? cd.ShowDialog(owner) : cd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return cd.Color;
            }

            return null;
        }

        #endregion

        #region Font Input

        /// <summary>
        /// Shows a font picker dialog (async)
        /// </summary>
        public async Task<Font?> InputFontAsync(string? title = null, Font? initialFont = null)
        {
            using var fd = new FontDialog();

            if (initialFont != null)
            {
                fd.Font = initialFont;
            }

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? fd.ShowDialog(owner) : fd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return fd.Font;
            }

            return null;
        }

        /// <summary>
        /// Shows a font picker dialog (sync)
        /// </summary>
        public Font? InputFont(string? title = null, Font? initialFont = null)
        {
            using var fd = new FontDialog();

            if (initialFont != null)
            {
                fd.Font = initialFont;
            }

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? fd.ShowDialog(owner) : fd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return fd.Font;
            }

            return null;
        }

        #endregion

        #region IDialogManager Implementation (Input Methods)

        DialogReturn IDialogManager.InputBox(string title, string promptText)
        {
            var value = InputText(title, promptText);
            return new DialogReturn
            {
                Result = value != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value ?? string.Empty,
                Submit = value != null,
                Cancel = value == null
            };
        }

        DialogReturn IDialogManager.InputBoxYesNo(string title, string promptText)
        {
            var result = ShowQuestion(title, promptText);
            return result;
        }

        DialogReturn IDialogManager.InputLargeBox(string title, string promptText)
        {
            var value = InputLargeText(title, promptText);
            return new DialogReturn
            {
                Result = value != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value ?? string.Empty,
                Submit = value != null,
                Cancel = value == null
            };
        }

        DialogReturn IDialogManager.InputPassword(string title, string promptText, bool masked)
        {
            var value = InputPassword(title, promptText);
            return new DialogReturn
            {
                Result = value != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value ?? string.Empty,
                Submit = value != null,
                Cancel = value == null
            };
        }

        DialogReturn IDialogManager.InputInt(string title, string promptText, int? min, int? max, int? @default)
        {
            var value = InputInteger(title, promptText, min, max, @default);
            return new DialogReturn
            {
                Result = value.HasValue ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value?.ToString() ?? string.Empty,
                Submit = value.HasValue,
                Cancel = !value.HasValue
            };
        }

        DialogReturn IDialogManager.InputDouble(string title, string promptText, double? min, double? max, double? @default, int? decimals)
        {
            var value = InputNumber(title, promptText, min, max, @default);
            return new DialogReturn
            {
                Result = value.HasValue ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value?.ToString() ?? string.Empty,
                Submit = value.HasValue,
                Cancel = !value.HasValue
            };
        }

        DialogReturn IDialogManager.InputDateTime(string title, string promptText, DateTime? min, DateTime? max, DateTime? @default)
        {
            var value = InputDate(title, promptText, min, max, @default);
            return new DialogReturn
            {
                Result = value.HasValue ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value?.ToString() ?? string.Empty,
                Tag = value,
                Submit = value.HasValue,
                Cancel = !value.HasValue
            };
        }

        DialogReturn IDialogManager.InputTimeSpan(string title, string promptText, TimeSpan? min, TimeSpan? max, TimeSpan? @default)
        {
            var value = InputTimeSpan(title, promptText, min, max, @default);
            return new DialogReturn
            {
                Result = value.HasValue ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value?.ToString() ?? string.Empty,
                Tag = value,
                Submit = value.HasValue,
                Cancel = !value.HasValue
            };
        }

        DialogReturn IDialogManager.InputComboBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            var value = InputSelect(title, promptText, itvalues);
            return new DialogReturn
            {
                Result = value != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value?.Text ?? string.Empty,
                Tag = value,
                Submit = value != null,
                Cancel = value == null
            };
        }

        DialogReturn IDialogManager.InputComboBox(string title, string promptText, List<string> values)
        {
            var value = InputSelectString(title, promptText, values);
            return new DialogReturn
            {
                Result = value != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value ?? string.Empty,
                Submit = value != null,
                Cancel = value == null
            };
        }

        DialogReturn IDialogManager.InputListBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            return ((IDialogManager)this).InputComboBox(title, promptText, itvalues);
        }

        DialogReturn IDialogManager.InputRadioGroupBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            return ((IDialogManager)this).InputComboBox(title, promptText, itvalues);
        }

        DialogReturn IDialogManager.InputCheckList(string title, string promptText, List<SimpleItem> items)
        {
            var values = InputMultiSelect(title, promptText, items);
            return new DialogReturn
            {
                Result = values.Count > 0 ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Items = values,
                Submit = values.Count > 0,
                Cancel = values.Count == 0
            };
        }

        DialogReturn IDialogManager.MultiSelect(string title, string promptText, List<SimpleItem> items)
        {
            return ((IDialogManager)this).InputCheckList(title, promptText, items);
        }

        DialogReturn IDialogManager.DialogCombo(string text, List<SimpleItem> comboSource, string DisplyMember, string ValueMember)
        {
            return ((IDialogManager)this).InputComboBox("Select", text, comboSource);
        }

        DialogReturn IDialogManager.SelectColor(string? title, string? initialColor)
        {
            Color? initial = null;
            if (!string.IsNullOrEmpty(initialColor))
            {
                try { initial = ColorTranslator.FromHtml(initialColor); } catch { }
            }

            var color = InputColor(title, initial);
            return new DialogReturn
            {
                Result = color.HasValue ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = color.HasValue ? ColorTranslator.ToHtml(color.Value) : string.Empty,
                Tag = color,
                Submit = color.HasValue,
                Cancel = !color.HasValue
            };
        }

        DialogReturn IDialogManager.SelectFont(string? title, string? initialFont)
        {
            var font = InputFont(title);
            return new DialogReturn
            {
                Result = font != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = font?.Name ?? string.Empty,
                Tag = font,
                Submit = font != null,
                Cancel = font == null
            };
        }

        #endregion
    }
}

