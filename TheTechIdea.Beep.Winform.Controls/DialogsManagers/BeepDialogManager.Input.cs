using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Input dialog methods for BeepDialogManager
    /// </summary>
    public partial class BeepDialogManager
    {
        #region Text Input

        public string? InputText(string title, string prompt, string? defaultValue = null)
        {
            using var dialog = CreateInputDialog(title, prompt, defaultValue, multiline: false, password: false);

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.ReturnValue;
            }

            return null;
        }

        public string? InputLargeText(string title, string prompt, string? defaultValue = null)
        {
            using var dialog = CreateInputDialog(title, prompt, defaultValue, multiline: true, password: false);

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.ReturnValue;
            }

            return null;
        }

        public string? InputPassword(string title, string prompt)
        {
            using var dialog = CreateInputDialog(title, prompt, string.Empty, multiline: false, password: true);

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

        #region Input Dialog Helpers

        private BeepInputDialog CreateInputDialog(string title, string prompt, string? defaultValue, bool multiline, bool password)
        {
            var dialog = new BeepInputDialog();
            dialog.Title = title;
            dialog.Message = prompt;
            if (multiline) dialog.InputBoxMultiline = true;
            if (password) dialog.InputBoxUsePasswordChar = true;
            if (!string.IsNullOrEmpty(defaultValue)) dialog.InputDefaultValue = defaultValue;
            return dialog;
        }

        #endregion

        #region Selection Input

        public SimpleItem? InputSelect(string title, string prompt, List<SimpleItem> items)
        {
            if (items == null || items.Count == 0)
                return null;

            using var dialog = new BeepListDialog();
            dialog.Title = title;
            dialog.Message = prompt;
            dialog.Items = items;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                return dialog.ReturnItem;

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
        /// Shows a multi-select dialog with checkboxes (sync)
        /// </summary>
        public List<SimpleItem> InputMultiSelect(string title, string prompt, List<SimpleItem> items, IEnumerable<string>? preSelected = null)
        {
            if (items == null || items.Count == 0)
                return new List<SimpleItem>();

            using var dialog = new BeepMultiSelectDialog();
            dialog.SetItems(title, prompt, items, preSelected);

            if (_defaultTheme != null)
                dialog.CurrentTheme = _defaultTheme;
                dialog.ApplyTheme();       // propagate theme to all child controls

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK
                ? dialog.SelectedItems
                : new List<SimpleItem>();
        }

        #endregion

        #region Date/Time Input

        public DateTime? InputDate(string title, string prompt, DateTime? min = null, DateTime? max = null, DateTime? defaultValue = null)
        {
            using var dialog = CreateDatePickerDialog(title, prompt, min, max, defaultValue);
            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
            return result == System.Windows.Forms.DialogResult.OK && dialog.Tag is DateTime dt ? dt : null;
        }

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

        #region Date/Time Picker Helpers

        private Form CreateDatePickerDialog(string title, string prompt, DateTime? min, DateTime? max, DateTime? defaultValue)
        {
            // Skill § 1: token-driven sizing for the date picker.
            // Note: this method is on BeepDialogManager (not a Control) — we use form as the
            // DPI reference for scaling; the form is the natural container anyway.
            var form = new BeepMessageDialog();
            form.Title = title;
            form.Message = prompt;
            form.StartPosition = FormStartPosition.CenterParent;

            var dtp = new DateTimePicker
            {
                Location = new Point(
                    BeepLayoutMetrics.DialogPadding.Left.ScaleValue(form),
                    BeepLayoutMetrics.LabelStandard.Height.ScaleValue(form) + BeepLayoutMetrics.SmallGap.ScaleValue(form)),
                Size = new Size(
                    BeepLayoutMetrics.FieldStandard.Width.ScaleValue(form) + BeepLayoutMetrics.ButtonSmall.Width.ScaleValue(form),
                    BeepLayoutMetrics.FieldStandard.Height.ScaleValue(form)),
                Format = DateTimePickerFormat.Short
            };

            if (defaultValue.HasValue)
                dtp.Value = defaultValue.Value;
            if (min.HasValue)
                dtp.MinDate = min.Value;
            if (max.HasValue)
                dtp.MaxDate = max.Value;

            form.Controls.Add(dtp);
            form.Tag = dtp;

            form.FormClosed += (s, e) =>
            {
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                    form.Tag = dtp.Value;
            };

            return form;
        }

        #endregion

        #region Color Input

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

        DialogReturn IDialogManager.InputBox(string title, string promptText)
        {
            var value = InputText(title, promptText);
            return CreateValueReturn(value);
        }

        DialogReturn IDialogManager.InputBoxYesNo(string title, string promptText)
        {
            return Show(DialogConfig.CreateQuestion(title, promptText));
        }

        DialogReturn IDialogManager.InputLarge(string title, string promptText)
        {
            return ((IDialogManager)this).InputBox(title, promptText);
        }

        DialogReturn IDialogManager.InputPassword(string title, string promptText, bool masked)
        {
            var value = InputPassword(title, promptText);
            return CreateValueReturn(value);
        }

        DialogReturn IDialogManager.InputInt(string title, string promptText, int? min, int? max, int? @default)
        {
            var value = InputInteger(title, promptText, min, max, @default);
            return CreateTagReturn(value);
        }

        DialogReturn IDialogManager.InputDouble(string title, string promptText, double? min, double? max, double? @default, int? decimals)
        {
            var value = InputNumber(title, promptText, min, max, @default);
            return CreateTagReturn(value);
        }

        DialogReturn IDialogManager.InputDateTime(string title, string promptText, DateTime? min, DateTime? max, DateTime? @default)
        {
            var value = InputDate(title, promptText, min, max, @default);
            return CreateTagReturn(value);
        }

        DialogReturn IDialogManager.InputTimeSpan(string title, string promptText, TimeSpan? min, TimeSpan? max, TimeSpan? @default)
        {
            var value = InputTimeSpan(title, promptText, min, max, @default);
            return CreateTagReturn(value);
        }

        DialogReturn IDialogManager.InputCombo(string title, string promptText, List<SimpleItem> itvalues)
        {
            var value = InputSelect(title, promptText, itvalues);
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

        DialogReturn IDialogManager.InputCombo(string title, string promptText, List<string> values)
        {
            var items = values?.Select(v => new SimpleItem { Text = v, Value = v }).ToList() ?? new List<SimpleItem>();
            return ((IDialogManager)this).InputCombo(title, promptText, items);
        }

        DialogReturn IDialogManager.InputList(string title, string promptText, List<SimpleItem> itvalues)
        {
            return ((IDialogManager)this).InputCombo(title, promptText, itvalues);
        }

        DialogReturn IDialogManager.InputRadioGroup(string title, string promptText, List<SimpleItem> itvalues)
        {
            return ((IDialogManager)this).InputCombo(title, promptText, itvalues);
        }

        DialogReturn IDialogManager.InputCheckList(string title, string promptText, List<SimpleItem> items)
        {
            var values = InputMultiSelect(title, promptText, items);
            return new DialogReturn
            {
                Result = values.Count > 0 ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Items = values,
                Submit = values.Count > 0,
                Cancel = values.Count == 0,
                UserAction = values.Count > 0 ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
            };
        }

        DialogReturn IDialogManager.MultiSelect(string title, string promptText, List<SimpleItem> items)
        {
            return ((IDialogManager)this).InputCheckList(title, promptText, items);
        }

        DialogReturn IDialogManager.DialogCombo(string text, List<SimpleItem> comboSource, string displayMember, string valueMember)
        {
            return ((IDialogManager)this).InputCombo("Select", text, comboSource);
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
                Cancel = !color.HasValue,
                UserAction = color.HasValue ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
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
                Cancel = font == null,
                UserAction = font != null ? BeepDialogButtons.Ok : BeepDialogButtons.Cancel
            };
        }

        #endregion
    }
}
