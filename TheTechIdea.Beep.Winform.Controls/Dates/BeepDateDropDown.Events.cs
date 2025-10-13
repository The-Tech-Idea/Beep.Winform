using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown
    {
        // Events and input handling
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_readOnly) return;
            var content = GetContentRectForDrawing();
            var btn = GetButtonRect(content);
            if (_showDropDown && btn.Contains(e.Location))
            {
                TogglePopup();
            }
            else
            {
                _isEditing = true;
                _inputText = SelectedDateTime == DateTime.MinValue ? string.Empty : SelectedDateTime.ToString(CultureInfo.CurrentCulture);
                Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_readOnly) return;
            if (e.KeyCode == Keys.F4 || (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Down))
            {
                TogglePopup();
                e.Handled = true;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (_readOnly) return;
            if (!_isEditing) _isEditing = true;
            if (e.KeyChar == (char)Keys.Escape) { _isEditing = false; _inputText = SelectedDateTime == DateTime.MinValue ? string.Empty : SelectedDateTime.ToString(CultureInfo.CurrentCulture); Invalidate(); return; }
            if (e.KeyChar == (char)Keys.Return)
            {
                if (string.IsNullOrWhiteSpace(_inputText))
                {
                    if (_allowEmpty) { SelectedDateTime = DateTime.MinValue; }
                    _isEditing = false; Invalidate(); return;
                }
                if (DateTime.TryParse(_inputText, CultureInfo.CurrentCulture, DateTimeStyles.None, out var result) && IsDateValid(result))
                {
                    SelectedDateTime = result;
                    _isEditing = false;
                }
                else
                {
                    // show validation feedback using BaseControl helper
                    SetValidationError("Invalid date");
                }
                Invalidate();
                return;
            }

            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar) || "/- :".Contains(e.KeyChar))
            {
                _inputText += e.KeyChar;
                Invalidate();
            }
            else
            {
                e.Handled = true;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _isEditing = false;
            _inputText = string.Empty;
            Invalidate();
        }

        private bool IsDateValid(DateTime date)
        {
            if (_minDate.HasValue && date.Date < _minDate.Value.Date) return false;
            if (_maxDate.HasValue && date.Date > _maxDate.Value.Date) return false;
            return true;
        }

        // Calendar callbacks
        private void CalendarView_OkClicked(object sender, TheTechIdea.Beep.Winform.Controls.DateTimeDialogResultEventArgs e)
        {
            if (e.SelectedDateTime.HasValue)
            {
                SelectedDateTime = e.SelectedDateTime.Value;
                ClearValidationError();
            }
            ClosePopup();
        }

        private void CalendarView_CancelClicked(object sender, TheTechIdea.Beep.Winform.Controls.DateTimeDialogResultEventArgs e)
        {
            ClosePopup();
        }
    }
}
