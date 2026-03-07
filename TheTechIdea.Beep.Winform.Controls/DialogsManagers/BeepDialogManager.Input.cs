using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
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
        public Task<string?> InputTextAsync(string title, string prompt, string? defaultValue = null)
        {
            var config = new DialogConfig
            {
                Title = title,
                Message = prompt,
                Preset = DialogPreset.None,
                IconType = BeepDialogIcon.Question,
                Buttons = new[] { BeepDialogButtons.Cancel, BeepDialogButtons.Ok }
            };

            using var dialog = new BeepDialogForm();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputString;

            if (_defaultTheme != null)
                dialog.CurrentTheme = _defaultTheme;

            dialog.StartPosition = FormStartPosition.CenterParent;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var value = dialog.ReturnValue;
                if (config.FieldValidators.TryGetValue("value", out var validator))
                {
                    var validation = validator(value ?? string.Empty);
                    config.ValidationState["value"] = validation;
                    if (!validation.Valid)
                    {
                        DialogMotionEngine.ShakeDialog(dialog);
                        return Task.FromResult<string?>(null);
                    }
                }
                StoreRecentInput(config, value ?? string.Empty);
                return Task.FromResult<string?>(value);
            }

            return Task.FromResult<string?>(null);
        }

        /// <summary>
        /// Shows a text input dialog (sync)
        /// </summary>
        public string? InputText(string title, string prompt, string? defaultValue = null)
        {
            using var dialog = new BeepDialogForm();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputString;

            if (_defaultTheme != null)
                dialog.CurrentTheme = _defaultTheme;

            dialog.StartPosition = FormStartPosition.CenterParent;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var value = dialog.ReturnValue;
                if (_defaultTheme != null && _recentInputMemory.TryGetValue($"{title}:{prompt}", out var queue) && queue.Count > 0)
                {
                    // no-op placeholder for future autofill suggestions
                }
                return value;
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
            using var dialog = new BeepDialogForm();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputString;

            if (_defaultTheme != null)
                dialog.CurrentTheme = _defaultTheme;

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

        public Task<string?> InputTextValidatedAsync(
            string title,
            string prompt,
            Func<string, (bool Valid, string Error)> validator,
            string? defaultValue = null)
        {
            var config = new DialogConfig
            {
                Title = title,
                Message = prompt,
                Buttons = new[] { BeepDialogButtons.Cancel, BeepDialogButtons.Ok },
                DialogKey = $"{title}:{prompt}",
                EnableRecentInputMemory = true
            };
            config.FieldValidators["value"] = validator;
            return InputTextAsync(title, prompt, defaultValue);
        }

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
        public Task<SimpleItem?> InputSelectAsync(string title, string prompt, List<SimpleItem> items)
        {
            if (items == null || items.Count == 0)
                return Task.FromResult<SimpleItem?>(null);

            using var dialog = new BeepDialogForm();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputFromList;
            dialog.Items = items;

            if (_defaultTheme != null)
                dialog.CurrentTheme = _defaultTheme;

            dialog.StartPosition = FormStartPosition.CenterParent;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return Task.FromResult<SimpleItem?>(dialog.ReturnItem);
            }

            return Task.FromResult<SimpleItem?>(null);
        }

        /// <summary>
        /// Shows a selection dialog from a list of items (sync)
        /// </summary>
        public SimpleItem? InputSelect(string title, string prompt, List<SimpleItem> items)
        {
            if (items == null || items.Count == 0)
                return null;

            using var dialog = new BeepDialogForm();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.DialogType = DialogType.GetInputFromList;
            dialog.Items = items;

            if (_defaultTheme != null)
                dialog.CurrentTheme = _defaultTheme;

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
        public Task<Color?> InputColorAsync(string? title = null, Color? initialColor = null)
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
                return Task.FromResult<Color?>(cd.Color);
            }

            return Task.FromResult<Color?>(null);
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
        public Task<Font?> InputFontAsync(string? title = null, Font? initialFont = null)
        {
            using var fd = new FontDialog();

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? fd.ShowDialog(owner) : fd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return Task.FromResult<Font?>(fd.Font);
            }

            return Task.FromResult<Font?>(null);
        }

        /// <summary>
        /// Shows a font picker dialog (sync)
        /// </summary>
        public Font? InputFont(string? title = null, Font? initialFont = null)
        {
            using var fd = new FontDialog();

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

        private static DialogReturn CreateValueReturn(string? value, object? tag = null)
        {
            return new DialogReturn
            {
                Result = value != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value ?? string.Empty,
                Tag = tag,
                Submit = value != null,
                Cancel = value == null,
                UserAction = value != null ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
            };
        }

        private static DialogReturn CreateTagReturn<T>(T? value) where T : struct
        {
            return new DialogReturn
            {
                Result = value.HasValue ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value?.ToString() ?? string.Empty,
                Tag = value,
                Submit = value.HasValue,
                Cancel = !value.HasValue,
                UserAction = value.HasValue ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
            };
        }

        async Task<DialogReturn> IDialogManager.InputBoxAsync(string title, string promptText, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = await InputTextAsync(title, promptText);
            return CreateValueReturn(value);
        }

        async Task<DialogReturn> IDialogManager.InputBoxYesNoAsync(string title, string promptText, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await ShowAsync(DialogConfig.CreateQuestion(title, promptText), cancellationToken);
        }

        Task<DialogReturn> IDialogManager.InputLargeBoxAsync(string title, string promptText, System.Threading.CancellationToken cancellationToken)
        {
            return ((IDialogManager)this).InputBoxAsync(title, promptText, cancellationToken);
        }

        async Task<DialogReturn> IDialogManager.InputPasswordAsync(string title, string promptText, bool masked, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = InputPassword(title, promptText);
            return await Task.FromResult(CreateValueReturn(value));
        }

        async Task<DialogReturn> IDialogManager.InputIntAsync(string title, string promptText, int? min, int? max, int? @default, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = InputInteger(title, promptText, min, max, @default);
            return await Task.FromResult(CreateTagReturn(value));
        }

        async Task<DialogReturn> IDialogManager.InputDoubleAsync(string title, string promptText, double? min, double? max, double? @default, int? decimals, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = await InputNumberAsync(title, promptText, min, max, @default);
            return CreateTagReturn(value);
        }

        async Task<DialogReturn> IDialogManager.InputDateTimeAsync(string title, string promptText, DateTime? min, DateTime? max, DateTime? @default, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = await InputDateAsync(title, promptText, min, max, @default);
            return CreateTagReturn(value);
        }

        async Task<DialogReturn> IDialogManager.InputTimeSpanAsync(string title, string promptText, TimeSpan? min, TimeSpan? max, TimeSpan? @default, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = InputTimeSpan(title, promptText, min, max, @default);
            return await Task.FromResult(CreateTagReturn(value));
        }

        async Task<DialogReturn> IDialogManager.InputComboBoxAsync(string title, string promptText, List<SimpleItem> itvalues, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = await InputSelectAsync(title, promptText, itvalues);
            return new DialogReturn
            {
                Result = value != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = value?.Text ?? string.Empty,
                Tag = value,
                Submit = value != null,
                Cancel = value == null,
                UserAction = value != null ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
            };
        }

        async Task<DialogReturn> IDialogManager.InputComboBoxAsync(string title, string promptText, List<string> values, System.Threading.CancellationToken cancellationToken)
        {
            var items = values?.Select(v => new SimpleItem { Text = v, Value = v }).ToList() ?? new List<SimpleItem>();
            return await ((IDialogManager)this).InputComboBoxAsync(title, promptText, items, cancellationToken);
        }

        Task<DialogReturn> IDialogManager.InputListBoxAsync(string title, string promptText, List<SimpleItem> itvalues, System.Threading.CancellationToken cancellationToken)
        {
            return ((IDialogManager)this).InputComboBoxAsync(title, promptText, itvalues, cancellationToken);
        }

        Task<DialogReturn> IDialogManager.InputRadioGroupBoxAsync(string title, string promptText, List<SimpleItem> itvalues, System.Threading.CancellationToken cancellationToken)
        {
            return ((IDialogManager)this).InputComboBoxAsync(title, promptText, itvalues, cancellationToken);
        }

        async Task<DialogReturn> IDialogManager.InputCheckListAsync(string title, string promptText, List<SimpleItem> items, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var values = await InputMultiSelectAsync(title, promptText, items);
            return new DialogReturn
            {
                Result = values.Count > 0 ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Items = values,
                Submit = values.Count > 0,
                Cancel = values.Count == 0,
                UserAction = values.Count > 0 ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
            };
        }

        Task<DialogReturn> IDialogManager.MultiSelectAsync(string title, string promptText, List<SimpleItem> items, System.Threading.CancellationToken cancellationToken)
        {
            return ((IDialogManager)this).InputCheckListAsync(title, promptText, items, cancellationToken);
        }

        Task<DialogReturn> IDialogManager.DialogComboAsync(string text, List<SimpleItem> comboSource, string displayMember, string valueMember, System.Threading.CancellationToken cancellationToken)
        {
            return ((IDialogManager)this).InputComboBoxAsync("Select", text, comboSource, cancellationToken);
        }

        async Task<DialogReturn> IDialogManager.SelectColorAsync(string? title, string? initialColor, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Color? initial = null;
            if (!string.IsNullOrEmpty(initialColor))
            {
                try { initial = ColorTranslator.FromHtml(initialColor); } catch { }
            }

            var color = await InputColorAsync(title, initial);
            return new DialogReturn
            {
                Result = color.HasValue ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = color.HasValue ? ColorTranslator.ToHtml(color.Value) : string.Empty,
                Tag = color,
                Submit = color.HasValue,
                Cancel = !color.HasValue,
                UserAction = color.HasValue ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
            };
        }

        async Task<DialogReturn> IDialogManager.SelectFontAsync(string? title, string? initialFont, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var font = await InputFontAsync(title);
            return new DialogReturn
            {
                Result = font != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = font?.Name ?? string.Empty,
                Tag = font,
                Submit = font != null,
                Cancel = font == null,
                UserAction = font != null ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
            };
        }

        #endregion
    }
}

